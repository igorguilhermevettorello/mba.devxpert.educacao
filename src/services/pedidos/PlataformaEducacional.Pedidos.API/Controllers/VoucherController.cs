using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PlataformaEducacional.Core.Mediator;
using PlataformaEducacional.Pedidos.API.Application.Commands;
using PlataformaEducacional.Pedidos.API.Application.DTO;
using PlataformaEducacional.Pedidos.API.Application.Queries;
using PlataformaEducacional.WebApi.Core.Controllers;
using System.Net;

namespace PlataformaEducacional.Pedidos.API.Controllers
{
    [Authorize]
    public class VoucherController : MainController
    {
        private readonly IVoucherQueries _voucherQueries;
        private readonly IMediatorHandler _mediator;

        public VoucherController(IVoucherQueries voucherQueries, IMediatorHandler mediator)
        {
            _voucherQueries = voucherQueries;
            _mediator = mediator;
        }

        [HttpGet("voucher/{codigo}")]
        [ProducesResponseType(typeof(VoucherDTO), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> ObterPorCodigo(string codigo)
        {
            if (string.IsNullOrEmpty(codigo)) return NotFound();

            var voucher = await _voucherQueries.ObterVoucherPorCodigo(codigo);

            return voucher == null ? NotFound() : CustomResponse(voucher);
        }

        [HttpPost("voucher")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> AdicionarVoucher(AdicionarVoucherDTO voucherDto)
        {
            var command = new AdicionarVoucherCommand
            {
                Codigo = voucherDto.Codigo,
                Percentual = voucherDto.Percentual,
                ValorDesconto = voucherDto.ValorDesconto,
                Quantidade = voucherDto.Quantidade,
                TipoDesconto = voucherDto.TipoDesconto,
                DataValidade = voucherDto.DataValidade
            };

            return CustomResponse(await _mediator.SendCommand(command));
        }
    }
}


