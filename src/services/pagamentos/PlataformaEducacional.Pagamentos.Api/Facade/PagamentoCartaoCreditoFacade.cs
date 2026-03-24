using Microsoft.Extensions.Options;
using PlataformaEducacional.Pagamentos.Api.Models;
using PlataformaEducacional.Pagamentos.EducaPag;

namespace PlataformaEducacional.Pagamentos.Api.Facade
{
    public class PagamentoCartaoCreditoFacade : IPagamentoFacade
    {
        private readonly PagamentoConfig _pagamentoConfig;

        public PagamentoCartaoCreditoFacade(IOptions<PagamentoConfig> pagamentoConfig)
        {
            _pagamentoConfig = pagamentoConfig.Value;
        }

        public async Task<Transacao> AutorizarPagamento(Pagamento pagamento)
        {
            try
            {
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

                return ParaTransacao(await transacao.AuthorizeCardTransaction());
            }
            catch (Exception ex)
            {
                //TODO: avaliar necessidade de try/catch
                throw ex;
            }
        }

        public async Task<Transacao> CapturarPagamento(Transacao transacao)
        {
            var educaPagSvc = new EducaPagService(_pagamentoConfig.DefaultApiKey,
                _pagamentoConfig.DefaultEncryptionKey);

            var transaction = ParaTransaction(transacao, educaPagSvc);

            return ParaTransacao(await transaction.CaptureCardTransaction());
        }

        public async Task<Transacao> CancelarAutorizacao(Transacao transacao)
        {
            var educaPagSvc = new EducaPagService(_pagamentoConfig.DefaultApiKey, _pagamentoConfig.DefaultEncryptionKey);
            var transaction = ParaTransaction(transacao, educaPagSvc);
            return ParaTransacao(await transaction.CancelAuthorization());
        }

        public static Transacao ParaTransacao(Transaction transaction)
        {
            return new Transacao
            {
                Id = Guid.NewGuid(),
                Status = (StatusTransacao)transaction.Status,
                ValorTotal = transaction.Amount,
                BandeiraCartao = transaction.CardBrand,
                CodigoAutorizacao = transaction.AuthorizationCode,
                CustoTransacao = transaction.Cost,
                DataTransacao = transaction.TransactionDate,
                NSU = transaction.Nsu,
                TID = transaction.Tid
            };
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
