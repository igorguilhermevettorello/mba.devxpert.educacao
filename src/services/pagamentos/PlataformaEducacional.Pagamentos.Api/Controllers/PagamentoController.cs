using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PlataformaEducacional.Core.Mediator;
using PlataformaEducacional.Pagamentos.Api.Models;
using PlataformaEducacional.Pagamentos.Api.Models.DTOs;
using PlataformaEducacional.WebApi.Core.Controllers;

namespace PlataformaEducacional.Pagamentos.Api.Controllers
{
    [ApiController]
    [Route("api/pagamentos")]
    public class PagamentoController : MainController
    {
        readonly IPagamentoRepository _pagamentoRepository;
        private readonly IMediatorHandler _mediatorHandler;
        private readonly IMapper _mapper;

        public PagamentoController(
            IPagamentoRepository pagamentoRepository,
            IMediatorHandler mediatorHandler,
            IMapper mapper)
        {
            _pagamentoRepository = pagamentoRepository;
            _mediatorHandler = mediatorHandler;
            _mapper = mapper;
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> ObterPorId([FromRoute] Guid id)
        {
            //TODO: validar autorização
            var pagamento = await _pagamentoRepository.ObterPorId(id);

            if (pagamento == null)
            {
                return NotFound();
            }

            var pagamentoDto = _mapper.Map<PagamentoDto>(pagamento);
            return Ok(pagamentoDto);
        }


        [HttpGet("matricula/{matriculaId:guid}")]
        public async Task<IActionResult> ObterPorMatriculaId([FromRoute] Guid matriculaId)
        {
            //TODO: validar autorização
            var pagamento = await _pagamentoRepository.ObterPorMatriculaId(matriculaId);

            if (pagamento == null)
            {
                return NotFound();
            }

            var pagamentoDto = _mapper.Map<PagamentoDto>(pagamento);
            return Ok(pagamentoDto);
        }
    }
}
