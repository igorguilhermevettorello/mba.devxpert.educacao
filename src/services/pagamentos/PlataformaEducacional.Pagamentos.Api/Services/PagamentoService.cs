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

        public PagamentoService(IPagamentoFacade pagamentoFacade, IPagamentoRepository pagamentoRepository, IMessageBus bus)
        {
            _pagamentoFacade = pagamentoFacade;
            _pagamentoRepository = pagamentoRepository;
            _bus = bus;
        }

        public async Task<ResponseMessage> AutorizarPagamento(Pagamento pagamento)
        {
            var transacao = await _pagamentoFacade.AutorizarPagamento(pagamento);
            var validationResult = new ValidationResult();

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

                // Cancelar pagamento no gateway
                await CancelarPagamento(pagamento.MatriculaId);

                return new ResponseMessage(validationResult);
            }

            if (!PublicarEventoPagamentoConfirmado(pagamento))
            {
                await CancelarPagamento(pagamento.MatriculaId);
            }

            return new ResponseMessage(validationResult);
        }

        public async Task<ResponseMessage> CapturarPagamento(Guid pedidoId)
        {
            //TODO: avaliar necessidade deste método @@@@@

            var transacoes = await _pagamentoRepository.ObterTransacaoesPorMatriculaId(pedidoId);
            var transacaoAutorizada = transacoes?.FirstOrDefault(t => t.Status == StatusTransacao.Autorizado);
            var validationResult = new ValidationResult();

            if (transacaoAutorizada == null) throw new DomainException($"Transação não encontrada para o pedido {pedidoId}");

            var transacao = await _pagamentoFacade.CapturarPagamento(transacaoAutorizada);

            if (transacao.Status != StatusTransacao.Pago)
            {
                validationResult.Errors.Add(new ValidationFailure("Pagamento",
                    $"Não foi possível capturar o pagamento do pedido {pedidoId}"));

                return new ResponseMessage(validationResult);
            }

            transacao.AtualizarPagamentoId(transacaoAutorizada.PagamentoId);
            _pagamentoRepository.AdicionarTransacao(transacao);

            if (!await _pagamentoRepository.UnitOfWork.Commit())
            {
                validationResult.Errors.Add(new ValidationFailure("Pagamento",
                    $"Não foi possível persistir a captura do pagamento do pedido {pedidoId}"));

                return new ResponseMessage(validationResult);
            }

            return new ResponseMessage(validationResult);
        }

        public async Task<ResponseMessage> CancelarPagamento(Guid pedidoId)
        {
            var validationResult = new ValidationResult();
            var transacoes = await _pagamentoRepository.ObterTransacaoesPorMatriculaId(pedidoId);
            var transacaoAutorizada = transacoes?.FirstOrDefault(t => t.Status == StatusTransacao.Autorizado);

            if (transacaoAutorizada == null)
                throw new DomainException($"Transação não encontrada para o pedido {pedidoId}");

            var transacao = await _pagamentoFacade.CancelarAutorizacao(transacaoAutorizada);

            if (transacao.Status != StatusTransacao.Cancelado)
            {
                validationResult.Errors.Add(new ValidationFailure("Pagamento", $"Não foi possível cancelar o pagamento do pedido {pedidoId}"));
                return new ResponseMessage(validationResult);
            }

            transacao.AtualizarPagamentoId(transacaoAutorizada.PagamentoId);
            _pagamentoRepository.AdicionarTransacao(transacao);

            if (!await _pagamentoRepository.UnitOfWork.Commit())
            {
                validationResult.Errors.Add(new ValidationFailure("Pagamento", $"Não foi possível persistir o cancelamento do pagamento do pedido {pedidoId}"));
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
    }
}
