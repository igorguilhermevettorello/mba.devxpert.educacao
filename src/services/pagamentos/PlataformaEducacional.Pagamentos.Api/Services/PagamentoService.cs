using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using PlataformaEducacional.Core.DomainObjects;
using PlataformaEducacional.Core.Messages.Integration;
using PlataformaEducacional.MessageBus;
using PlataformaEducacional.Pagamentos.Api.Facade;
using PlataformaEducacional.Pagamentos.Api.Models;
using PlataformaEducacional.Pagamentos.Api.Models.Enums;

namespace PlataformaEducacional.Pagamentos.Api.Services
{
    public class PagamentoService : IPagamentoService
    {
        private readonly IPagamentoFacade _pagamentoFacade;
        private readonly IPagamentoRepository _pagamentoRepository;
        private readonly IMessageBus _bus;
        private readonly IConteudoService _conteudoService;
        private readonly IAlunoService _alunoService;
        private readonly ILogger<PagamentoService> _logger;

        public PagamentoService(IPagamentoFacade pagamentoFacade, IPagamentoRepository pagamentoRepository, IMessageBus bus, IConteudoService conteudoService, IAlunoService alunoService, ILogger<PagamentoService> logger)
        {
            _pagamentoFacade = pagamentoFacade;
            _pagamentoRepository = pagamentoRepository;
            _bus = bus;
            _conteudoService = conteudoService;
            _alunoService = alunoService;
            _logger = logger;
        }

        public async Task<ResponseMessage> AutorizarPagamento(Pagamento pagamento)
        {
            var validationResult = new ValidationResult();

            _logger.LogInformation("Iniciando autorização de Pagamento {PagamentoId} para Matrícula {MatriculaId}, Valor {Valor}",
                pagamento.Id, pagamento.MatriculaId, pagamento.Valor);

            if (!await MatriculaExiste(pagamento.MatriculaId))
            {
                _logger.LogWarning("Matrícula {MatriculaId} não encontrada para Pagamento {PagamentoId}", pagamento.MatriculaId, pagamento.Id);
                validationResult.Errors.Add(new ValidationFailure("Matricula", $"Matrícula {pagamento.MatriculaId} não encontrada"));
                return new ResponseMessage(validationResult);
            }

            if (await ExistePagamentoParaAMatricula(pagamento.MatriculaId))
            {
                _logger.LogWarning("Já existe pagamento para Matrícula {MatriculaId}", pagamento.MatriculaId);
                validationResult.Errors.Add(new ValidationFailure("Pagamento", $"Já existe pagamento para a matrícula {pagamento.MatriculaId}"));
                return new ResponseMessage(validationResult);
            }

            if (!await ValidarValorDoPagamento(pagamento))
            {
                _logger.LogWarning("Valor do pagamento {Valor} não corresponde ao preço do curso para Matrícula {MatriculaId}", pagamento.Valor, pagamento.MatriculaId);
                validationResult.Errors.Add(new ValidationFailure("Pagamento", "Valor do pagamento não corresponde ao preço do curso"));
                return new ResponseMessage(validationResult);
            }

            var transacao = await _pagamentoFacade.AutorizarPagamento(pagamento);

            if (transacao.Status != StatusTransacao.Autorizado)
            {
                _logger.LogWarning("Pagamento recusado - Transação {TransacaoId} com status {Status} para Matrícula {MatriculaId}",
                    transacao.Id, transacao.Status, pagamento.MatriculaId);
                validationResult.Errors.Add(new ValidationFailure("Pagamento", "Pagamento recusado, entre em contato com a sua operadora de cartão"));
                return new ResponseMessage(validationResult);
            }

            pagamento.AdicionarTransacao(transacao);
            _pagamentoRepository.AdicionarPagamento(pagamento);

            if (!await _pagamentoRepository.UnitOfWork.Commit())
            {
                _logger.LogError("Erro ao persistir Pagamento {PagamentoId} para Matrícula {MatriculaId}", pagamento.Id, pagamento.MatriculaId);
                validationResult.Errors.Add(new ValidationFailure("Pagamento", "Houve um erro ao realizar o pagamento."));
                await CancelarPagamento(pagamento.MatriculaId);
                return new ResponseMessage(validationResult);
            }

            _logger.LogInformation("Pagamento autorizado com sucesso - PagamentoId {PagamentoId}, MatriculaId {MatriculaId}, Transação {TransacaoId}",
                pagamento.Id, pagamento.MatriculaId, transacao.Id);

            if (!PublicarEventoPagamentoConfirmado(pagamento))
            {
                _logger.LogWarning("Falha ao publicar evento de pagamento confirmado para Matrícula {MatriculaId}", pagamento.MatriculaId);
                await CancelarPagamento(pagamento.MatriculaId);
            }

            return new ResponseMessage(validationResult);
        }

        public async Task<ResponseMessage> CapturarPagamento(Guid matriculaId)
        {
            var transacoes = await _pagamentoRepository.ObterTransacaoesPorMatriculaId(matriculaId);
            var transacaoAutorizada = transacoes?.FirstOrDefault(t => t.Status == StatusTransacao.Autorizado);
            var validationResult = new ValidationResult();

            if (transacaoAutorizada == null) throw new DomainException($"Transação não encontrada para a matrícula {matriculaId}");

            var transacao = await _pagamentoFacade.CapturarPagamento(transacaoAutorizada);

            if (transacao.Status != StatusTransacao.Pago)
            {
                validationResult.Errors.Add(new ValidationFailure("Pagamento",
                    $"Não foi possível capturar o pagamento da matrícula {matriculaId}"));

                return new ResponseMessage(validationResult);
            }

            transacao.AtualizarPagamentoId(transacaoAutorizada.PagamentoId);
            _pagamentoRepository.AdicionarTransacao(transacao);

            if (!await _pagamentoRepository.UnitOfWork.Commit())
            {
                validationResult.Errors.Add(new ValidationFailure("Pagamento",
                    $"Não foi possível persistir a captura do pagamento da matrícula {matriculaId}"));

                return new ResponseMessage(validationResult);
            }

            return new ResponseMessage(validationResult);
        }

        public async Task<ResponseMessage> CancelarPagamento(Guid matriculaId)
        {
            var validationResult = new ValidationResult();
            var transacoes = await _pagamentoRepository.ObterTransacaoesPorMatriculaId(matriculaId);
            var transacaoAutorizada = transacoes?.FirstOrDefault(t => t.Status == StatusTransacao.Autorizado);

            if (transacaoAutorizada == null)
                throw new DomainException($"Transação não encontrada para a matrícula {matriculaId}");

            var transacao = await _pagamentoFacade.CancelarAutorizacao(transacaoAutorizada);

            if (transacao.Status != StatusTransacao.Cancelado)
            {
                validationResult.Errors.Add(new ValidationFailure("Pagamento", $"Não foi possível cancelar o pagamento da matrícula {matriculaId}"));
                return new ResponseMessage(validationResult);
            }

            transacao.AtualizarPagamentoId(transacaoAutorizada.PagamentoId);
            _pagamentoRepository.AdicionarTransacao(transacao);

            if (!await _pagamentoRepository.UnitOfWork.Commit())
            {
                validationResult.Errors.Add(new ValidationFailure("Pagamento", $"Não foi possível persistir o cancelamento do pagamento da matrícula {matriculaId}"));
                return new ResponseMessage(validationResult);
            }

            return new ResponseMessage(validationResult);
        }

        private bool PublicarEventoPagamentoConfirmado(Pagamento pagamento)
        {
            var pagamentoConfirmadoEvent = new PagamentoConfirmadoIntegrationEvent
            {
                MatriculaId = pagamento.MatriculaId,
                DataConfirmacao = DateTime.Now
            };

            try
            {
                _bus.Publish<PagamentoConfirmadoIntegrationEvent>(pagamentoConfirmadoEvent);
                _logger.LogInformation("Evento de pagamento confirmado publicado com sucesso para MatriculaId {MatriculaId}", pagamento.MatriculaId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Falha ao publicar evento de pagamento confirmado para MatriculaId {MatriculaId}", pagamento.MatriculaId);
                return false;
            }
        }

        private async Task<bool> ExistePagamentoParaAMatricula(Guid matriculaId)
        {
            var pagamentoAnterior = await _pagamentoRepository.ObterPorMatriculaId(matriculaId);
            return pagamentoAnterior != null ? true : false;
        }

        private async Task<bool> ValidarValorDoPagamento(Pagamento pagamento)
        {
            var matricula = await _alunoService.ObterMatriculaPorIdAsync(pagamento.MatriculaId);
            var curso = await _conteudoService.ObterCursoPorIdAsync(matricula?.CursoId);
            return curso?.Valor == pagamento.Valor;
        }

        private async Task<bool> MatriculaExiste(Guid matriculaId)
        {
            var matricula = await _alunoService.ObterMatriculaPorIdAsync(matriculaId);
            return matricula != null;
        }
    }
}
