using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PlataformaEducacional.Pagamentos.Api.Models;
using PlataformaEducacional.Pagamentos.Api.Models.Enums;
using PlataformaEducacional.Pagamentos.EducaPag;

namespace PlataformaEducacional.Pagamentos.Api.Facade
{
    public class PagamentoCartaoCreditoFacade : IPagamentoFacade
    {
        private readonly PagamentoConfig _pagamentoConfig;
        private readonly ILogger<PagamentoCartaoCreditoFacade> _logger;

        public PagamentoCartaoCreditoFacade(IOptions<PagamentoConfig> pagamentoConfig, ILogger<PagamentoCartaoCreditoFacade> logger)
        {
            _pagamentoConfig = pagamentoConfig.Value;
            _logger = logger;
        }

        public async Task<Transacao> AutorizarPagamento(Pagamento pagamento)
        {
            _logger.LogInformation("Iniciando autorização de pagamento com cartão de crédito - Valor {Valor}", pagamento.Valor);

            var educaPagSvc = new EducaPagService(_pagamentoConfig.DefaultApiKey, _pagamentoConfig.DefaultEncryptionKey);

            var cardHashGen = new CardHash(educaPagSvc)
            {
                CardNumber = pagamento.CartaoCredito.Numero,
                CardHolderName = pagamento.CartaoCredito.Titular,
                CardExpirationDate = pagamento.CartaoCredito.MesAnoVencimento,
                CardCvv = pagamento.CartaoCredito.CVV
            };

            var cardHash = cardHashGen.Generate();

            var transacao = new Transaction(educaPagSvc)
            {
                CardHash = cardHash,
                CardNumber = pagamento.CartaoCredito.Numero,
                CardHolderName = pagamento.CartaoCredito.Titular,
                CardExpirationDate = pagamento.CartaoCredito.MesAnoVencimento,
                CardCvv = pagamento.CartaoCredito.CVV,
                PaymentMethod = PaymentMethod.CreditCard,
                Amount = pagamento.Valor
            };

            var transacaoResult = ParaTransacao(await transacao.AuthorizeCardTransaction());

            if (transacaoResult.Status == StatusTransacao.Autorizado)
                _logger.LogInformation("Pagamento autorizado com sucesso - Valor {Valor}, Código de Autorização {CodigoAuth}",
                    pagamento.Valor, transacaoResult.CodigoAutorizacao);
            else
                _logger.LogWarning("Pagamento recusado - Valor {Valor}, Status {Status}",
                    pagamento.Valor, transacaoResult.Status);

            return transacaoResult;

        }

        public async Task<Transacao> CapturarPagamento(Transacao transacao)
        {
            _logger.LogInformation("Capturando pagamento - Transação {TransacaoId}, Valor {Valor}", transacao.TID, transacao.ValorTotal);

            var educaPagSvc = new EducaPagService(_pagamentoConfig.DefaultApiKey,
                _pagamentoConfig.DefaultEncryptionKey);

            var transaction = ParaTransaction(transacao, educaPagSvc);

            var transacaoResult = ParaTransacao(await transaction.CaptureCardTransaction());

            if (transacaoResult.Status == StatusTransacao.Pago)
                _logger.LogInformation("Pagamento capturado com sucesso - Transação {TransacaoId}", transacao.TID);
            else
                _logger.LogWarning("Falha ao capturar pagamento - Transação {TransacaoId}, Status {Status}", transacao.TID, transacaoResult.Status);

            return transacaoResult;
        }

        public async Task<Transacao> CancelarAutorizacao(Transacao transacao)
        {
            _logger.LogInformation("Cancelando autorização de pagamento - Transação {TransacaoId}, Valor {Valor}", transacao.TID, transacao.ValorTotal);

            var educaPagSvc = new EducaPagService(_pagamentoConfig.DefaultApiKey, _pagamentoConfig.DefaultEncryptionKey);
            var transaction = ParaTransaction(transacao, educaPagSvc);

            var transacaoResult = ParaTransacao(await transaction.CancelAuthorization());

            if (transacaoResult.Status == StatusTransacao.Cancelado)
                _logger.LogInformation("Pagamento cancelado com sucesso - Transação {TransacaoId}", transacao.TID);
            else
                _logger.LogWarning("Falha ao cancelar pagamento - Transação {TransacaoId}, Status {Status}", transacao.TID, transacaoResult.Status);

            return transacaoResult;
        }

        public static Transacao ParaTransacao(Transaction transaction)
        {
            return new Transacao(
                transaction.AuthorizationCode,
                transaction.CardBrand,
                transaction.TransactionDate,
                transaction.Amount,
                transaction.Cost,
                (StatusTransacao)transaction.Status,
                transaction.Tid,
                transaction.Nsu);
        }

        public static Transaction ParaTransaction(Transacao transacao, EducaPagService educaPagService)
        {
            return new Transaction(educaPagService)
            {
                Status = (TransactionStatus)transacao.Status,
                Amount = transacao.ValorTotal,
                CardBrand = transacao.BandeiraCartao,
                AuthorizationCode = transacao.CodigoAutorizacao,
                Cost = transacao.CustoTransacao,
                Nsu = transacao.NSU,
                Tid = transacao.TID
            };
        }
    }
}
