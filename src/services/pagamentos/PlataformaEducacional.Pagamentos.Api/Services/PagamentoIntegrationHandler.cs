using PlataformaEducacional.Core.DomainObjects;
using PlataformaEducacional.Core.Messages.Integration;
using PlataformaEducacional.MessageBus;
using PlataformaEducacional.Pagamentos.Api.Models;

namespace PlataformaEducacional.Pagamentos.Api.Services
{
    public class PagamentoIntegrationHandler : Microsoft.Extensions.Hosting.BackgroundService
    {
        private readonly IMessageBus _bus;
        private readonly IServiceProvider _serviceProvider;

        public PagamentoIntegrationHandler(IServiceProvider serviceProvider, IMessageBus bus)
        {
            _serviceProvider = serviceProvider;
            _bus = bus;
        }

        private void SetResponder()
        {
            //_bus.RespondAsync<MatriculaIniciadaIntegrationEvent, ResponseMessage>(async request => await AutorizarPagamento(request));
            _bus.Respond<MatriculaIniciadaIntegrationEvent, ResponseMessage>(request => AutorizarPagamento(request));

            _bus.AdvancedBus.Connected += OnConnect;
        }

        private void SetSubscribers()
        {

            //TODO: avaliar necessidade destes eventos de integração @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
            //_bus.SubscribeAsync<PedidoCanceladoIntegrationEvent>("PedidoCancelado", async request =>
            //await CancelarPagamento(request));

            //_bus.SubscribeAsync<PedidoBaixadoEstoqueIntegrationEvent>("PedidoBaixadoEstoque", async request =>
            //await CapturarPagamento(request));
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            SetResponder();
            SetSubscribers();
            return Task.CompletedTask;
        }

        private void OnConnect(object? s, EventArgs e)
        {
            SetResponder();
        }

        private ResponseMessage AutorizarPagamento(MatriculaIniciadaIntegrationEvent message)
        {
            using var scope = _serviceProvider.CreateScope();
            var pagamentoService = scope.ServiceProvider.GetRequiredService<IPagamentoService>();

            var cartaoCredito = new CartaoCredito(message.Titular, message.NumeroCartao, message.Validade, message.CodigoSeguranca);

            var pagamento = new Pagamento(
                message.MatriculaId,
                TipoPagamento.CartaoCredito,    //Tipo pagamento chumbado pois o propósito da api é apenas didático
                message.ValorCurso,
                cartaoCredito);

            var response = pagamentoService.AutorizarPagamento(pagamento).Result;

            return response;
        }

        //private async Task<ResponseMessage> AutorizarPagamento(MatriculaIniciadaIntegrationEvent message)
        //{
        //    using var scope = _serviceProvider.CreateScope();
        //    var pagamentoService = scope.ServiceProvider.GetRequiredService<IPagamentoService>();

        //    var pagamento = new Pagamento
        //    {
        //        MatriculaId = message.MatriculaId,
        //        TipoPagamento = (TipoPagamento)message.TipoPagamento,
        //        Valor = message.ValorCurso,
        //        CartaoCredito = new CartaoCredito(message.Titular, message.NumeroCartao, message.Validade, message.CodigoSeguranca),
        //    };

        //    var response = await pagamentoService.AutorizarPagamento(pagamento);

        //    return response;
        //}

        //private async Task CancelarPagamento(PedidoCanceladoIntegrationEvent message)
        //{
        //    using (var scope = _serviceProvider.CreateScope())
        //    {
        //        var pagamentoService = scope.ServiceProvider.GetRequiredService<IPagamentoService>();

        //        var response = await pagamentoService.CancelarPagamento(message.PedidoId);

        //        if (!response.ValidationResult.IsValid)
        //            throw new DomainException($"Falha ao cancelar pagamento do pedido {message.PedidoId}");
        //    }
        //}

        //private async Task CapturarPagamento(PedidoBaixadoEstoqueIntegrationEvent message)
        //{
        //    using (var scope = _serviceProvider.CreateScope())
        //    {
        //        var pagamentoService = scope.ServiceProvider.GetRequiredService<IPagamentoService>();

        //        var response = await pagamentoService.CapturarPagamento(message.PedidoId);

        //        if (!response.ValidationResult.IsValid)
        //            throw new DomainException($"Falha ao capturar pagamento do pedido {message.PedidoId}");

        //        await _bus.PublishAsync(new PedidoPagoIntegrationEvent(message.ClienteId, message.PedidoId));
        //    }
        //}
    }
}
