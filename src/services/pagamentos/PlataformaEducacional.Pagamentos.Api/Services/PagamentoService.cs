using FluentValidation.Results;
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

        public PagamentoService(IPagamentoFacade pagamentoFacade, IPagamentoRepository pagamentoRepository, IMessageBus bus, IConteudoService conteudoService, IAlunoService alunoService)
        {
            _pagamentoFacade = pagamentoFacade;
            _pagamentoRepository = pagamentoRepository;
            _bus = bus;
            _conteudoService = conteudoService;
            _alunoService = alunoService;
        }

        public async Task<ResponseMessage> AutorizarPagamento(Pagamento pagamento)
        {
            var validationResult = new ValidationResult();

            if (!await MatriculaExiste(pagamento.MatriculaId))
            {
                validationResult.Errors.Add(new ValidationFailure("Matricula", $"Matrícula {pagamento.MatriculaId} não encontrada"));
                return new ResponseMessage(validationResult);
            }

            if (await ExistePagamentoParaAMatricula(pagamento.MatriculaId))
            {
                validationResult.Errors.Add(new ValidationFailure("Pagamento", $"Já existe pagamento para a matrícula {pagamento.MatriculaId}"));
                return new ResponseMessage(validationResult);
            }

            if (!await ValidarValorDoPagamento(pagamento))
            {
                validationResult.Errors.Add(new ValidationFailure("Pagamento", "Valor do pagamento não corresponde ao preço do curso"));
                return new ResponseMessage(validationResult);
            }

            var transacao = await _pagamentoFacade.AutorizarPagamento(pagamento);

            if (transacao.Status != StatusTransacao.Autorizado)
            {
                validationResult.Errors.Add(new ValidationFailure("Pagamento", "Pagamento recusado, entre em contato com a sua operadora de cartão"));
                return new ResponseMessage(validationResult);
            }

            pagamento.AdicionarTransacao(transacao);
            _pagamentoRepository.AdicionarPagamento(pagamento);

            if (!await _pagamentoRepository.UnitOfWork.Commit())
            {
                validationResult.Errors.Add(new ValidationFailure("Pagamento", "Houve um erro ao realizar o pagamento."));
                await CancelarPagamento(pagamento.MatriculaId);
                return new ResponseMessage(validationResult);
            }

            if (!PublicarEventoPagamentoConfirmado(pagamento))
            {
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
                return true;
            }
            catch (Exception ex)
            {
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
