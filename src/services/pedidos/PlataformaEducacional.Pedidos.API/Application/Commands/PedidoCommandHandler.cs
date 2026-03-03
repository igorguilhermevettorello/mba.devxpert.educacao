using EasyNetQ;
using FluentValidation.Results;
using MediatR;
using PlataformaEducacional.Core.Messages;
using PlataformaEducacional.Core.Messages.Integration;
using PlataformaEducacional.MessageBus;
using PlataformaEducacional.Pedidos.API.Application.DTO;
using PlataformaEducacional.Pedidos.API.Application.Events;
using PlataformaEducacional.Pedidos.Domain.Pedidos;
using PlataformaEducacional.Pedidos.Domain.Vouchers;


namespace PlataformaEducacional.Pedidos.API.Application.Commands
{
    public class PedidoCommandHandler : CommandHandler,
        IRequestHandler<AdicionarPedidoCommand, ValidationResult>
    {
        private readonly IPedidoRepository _pedidoRepository;
        private readonly IVoucherRepository _voucherRepository;
        private readonly IMessageBus bus;

        public PedidoCommandHandler(IVoucherRepository voucherRepository,
                                    IPedidoRepository pedidoRepository)
        {
            _voucherRepository = voucherRepository;
            _pedidoRepository = pedidoRepository;
        }

        public async Task<ValidationResult> Handle(AdicionarPedidoCommand message, CancellationToken cancellationToken)
        {
            // Validação do comando
            if (!message.IsValid()) return message.ValidationResult;

            // Mapear Pedido
            var pedido = MapearPedido(message);

            var pedidoCodigo = await _pedidoRepository.ObterProximoCodigo();
            pedido.AtribuirCodigo(pedidoCodigo);

            // Aplicar voucher se houver
            if (!await AplicarVoucher(message, pedido)) return ValidationResult;

            // Validar pedido
            if (!ValidarPedido(pedido)) return ValidationResult;

            // Processar pagamento
            if (!await ProcessarPagamento(pedido, message)) return ValidationResult;

            // Se pagamento tudo ok!
            pedido.AutorizarPedido();

            // Adicionar Evento
            pedido.AddEvent(new PedidoRealizadoEvent(pedido.Id, pedido.ClienteId));

            // Adicionar Pedido Repositorio
            _pedidoRepository.Adicionar(pedido);

            // Persistir dados de pedido e voucher
            return await PersistData(_pedidoRepository.UnitOfWork);
        }

        private Pedido MapearPedido(AdicionarPedidoCommand message)
        {
            var endereco = (Endereco)Activator.CreateInstance(
                typeof(Endereco),
                nonPublic: true
            );

            typeof(Endereco).GetProperty(nameof(Endereco.Logradouro))!
                .SetValue(endereco, message.Endereco.Logradouro);
            typeof(Endereco).GetProperty(nameof(Endereco.Numero))!
                .SetValue(endereco, message.Endereco.Numero);
            typeof(Endereco).GetProperty(nameof(Endereco.Complemento))!
                .SetValue(endereco, message.Endereco.Complemento);
            typeof(Endereco).GetProperty(nameof(Endereco.Bairro))!
                .SetValue(endereco, message.Endereco.Bairro);
            typeof(Endereco).GetProperty(nameof(Endereco.Cep))!
                .SetValue(endereco, message.Endereco.Cep);
            typeof(Endereco).GetProperty(nameof(Endereco.Cidade))!
                .SetValue(endereco, message.Endereco.Cidade);
            typeof(Endereco).GetProperty(nameof(Endereco.Estado))!
                .SetValue(endereco, message.Endereco.Estado);

            var pedido = new Pedido(
                message.ClienteId,
                message.ValorTotal,
                message.PedidoItems.Select(PedidoItemDTO.ParaPedidoItem).ToList(),
                message.VoucherUtilizado,
                message.Desconto
            );

            pedido.AtribuirEndereco(endereco);
            return pedido;
        }

        private async Task<bool> AplicarVoucher(AdicionarPedidoCommand message, Pedido pedido)
        {
            if (!message.VoucherUtilizado) return true;

            var voucher = await _voucherRepository.ObterVoucherPorCodigo(message.VoucherCodigo);
            if (voucher == null)
            {
                AddError("O voucher informado não existe!");
                return false;
            }

            //var voucherValidation = new VoucherValidation().Validate(voucher);
            //if (!voucherValidation.IsValid)
            //{
            //    voucherValidation.Errors.ToList().ForEach(m => AddError(m.ErrorMessage));
            //    return false;
            //}

            pedido.AtribuirVoucher(voucher);
            voucher.DebitarQuantidade();

            _voucherRepository.Atualizar(voucher);

            return true;
        }

        private bool ValidarPedido(Pedido pedido)
        {
            var pedidoValorOriginal = pedido.ValorTotal;
            var pedidoDesconto = pedido.Desconto;

            pedido.CalcularValorPedido();

            if (pedido.ValorTotal != pedidoValorOriginal)
            {
                AddError("O valor total do pedido não confere com o cálculo do pedido");
                return false;
            }

            if (pedido.Desconto != pedidoDesconto)
            {
                AddError("O valor total não confere com o cálculo do pedido");
                return false;
            }

            return true;
        }

        public async Task<bool> ProcessarPagamento(Pedido pedido, AdicionarPedidoCommand message)
        {

            var pedidoIniciado = new PedidoIniciadoIntegrationEvent
            {
                PedidoId = pedido.Id,
                ClienteId = pedido.ClienteId,
                TipoPagamento = 1, // Fixo, Alterar se tiver mais de um tipo de pagamento
                Valor = pedido.ValorTotal,
                NomeCartao = message.NomeCartao,
                NumeroCartao = message.NumeroCartao,
                MesAnoVencimento = message.ExpiracaoCartao,
                CVV = message.CvvCartao
            };

            var result = await bus.RequestAsync<PedidoIniciadoIntegrationEvent, ResponseMessage>(pedidoIniciado);

            if (result.ValidationResult.IsValid) return true;

            foreach (var error in result.ValidationResult.Errors)
            {
                AddError(error.ErrorMessage);
            }

            return false;
        }
    }
}
