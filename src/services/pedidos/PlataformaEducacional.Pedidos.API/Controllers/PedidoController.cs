using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PlataformaEducacional.Core.Mediator;
using PlataformaEducacional.Pedidos.API.Application.Commands;
using PlataformaEducacional.Pedidos.API.Application.Queries;
using PlataformaEducacional.Pedidos.API.DTOs;
using PlataformaEducacional.WebApi.Core.Controllers;
using PlataformaEducacional.WebApi.Core.User;

namespace PlataformaEducacional.Pedidos.API.Controllers
{
    [Authorize]
    public class PedidoController : MainController
    {
        private readonly IMediatorHandler _mediator;
        private readonly IAspNetUser _user;
        private readonly IPedidoQueries _pedidoQueries;

        public PedidoController(IMediatorHandler mediator,
            IAspNetUser user,
            IPedidoQueries pedidoQueries)
        {
            _mediator = mediator;
            _user = user;
            _pedidoQueries = pedidoQueries;
        }

        [HttpPost("pedido")]
        public async Task<IActionResult> AdicionarPedido(AdicionarPedidoDto pedido)
        {
            var command = new AdicionarPedidoCommand()
            {
                ClienteId = _user.ObterUserId(),
                CvvCartao = pedido.CvvCartao,
                Desconto = pedido.Desconto,
                Endereco = pedido.Endereco,
                ExpiracaoCartao = pedido.ExpiracaoCartao,
                NomeCartao = pedido.NomeCartao,
                NumeroCartao = pedido.NumeroCartao,
                PedidoItems = pedido.PedidoItems,
                ValorTotal = pedido.ValorTotal,
                VoucherCodigo = pedido.VoucherCodigo,
                VoucherUtilizado = pedido.VoucherUtilizado,
            };

            return CustomResponse(await _mediator.SendCommand(command));
        }

        [HttpGet("pedido/ultimo")]
        public async Task<IActionResult> UltimoPedido()
        {
            var pedido = await _pedidoQueries.ObterUltimoPedido(_user.ObterUserId());

            return pedido == null ? NotFound() : CustomResponse(pedido);
        }

        [HttpGet("pedido/lista-cliente")]
        public async Task<IActionResult> ListaPorCliente()
        {
            var pedidos = await _pedidoQueries.ObterListaPorClienteId(_user.ObterUserId());

            return pedidos == null ? NotFound() : CustomResponse(pedidos);
        }
    }
}
