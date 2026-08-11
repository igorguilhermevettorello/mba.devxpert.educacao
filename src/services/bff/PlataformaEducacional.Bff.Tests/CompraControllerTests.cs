using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using PlataformaEducacional.Bff.Api.Controllers;
using PlataformaEducacional.Bff.Api.Interfaces;
using PlataformaEducacional.Bff.Api.Models;
using PlataformaEducacional.Bff.Api.Services;
using PlataformaEducacional.WebApi.Core.User;
using Xunit;

namespace PlataformaEducacional.Bff.Api.Tests
{
    public class CompraControllerTests
    {
        private readonly Mock<IMediator> _mediator = new();
        private readonly Mock<IAspNetUser> _user = new();
        private readonly Mock<IConteudoService> _conteudo = new();
        private readonly Mock<IMatriculaService> _matricula = new();
        private readonly Mock<IPagamentoService> _pagamento = new();

        private CompraController CreateController()
        {
            var loggerMock = new Mock<ILogger<CompraController>>();
            return new CompraController(_mediator.Object, _user.Object, _conteudo.Object, _matricula.Object, _pagamento.Object, loggerMock.Object);
        }

        [Fact]
        public async Task ListarConteudosDisponiveis_ReturnsOk_WithValue()
        {
            var controller = CreateController();
            var result = await controller.ListarConteudosDisponiveis();
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task ListarMatriculaPendente_ReturnsOk_WithValue()
        {
            var alunoId = Guid.NewGuid();
            var list = new List<MatriculaDto> { new MatriculaDto { Id = Guid.NewGuid() } };
            _matricula.Setup(m => m.ObterMatriculaPendentesAsync(alunoId)).ReturnsAsync(list);

            var controller = CreateController();
            var result = await controller.ListarMatriculaPendente(alunoId);

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(list, ok.Value);
        }

        [Fact]
        public async Task RealizarMatricula_ReturnsOk_WithMatriculaDto()
        {
            var alunoId = Guid.NewGuid();
            var dto = new MatriculaDto { Id = Guid.NewGuid() };
            _matricula.Setup(m => m.RealizarMatriculaAsync(alunoId, It.IsAny<RealizarMatriculaDto>())).ReturnsAsync(dto);

            var controller = CreateController();
            var result = await controller.RealizarMatricula(alunoId, new RealizarMatriculaDto { CursoId = Guid.NewGuid() });

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(dto, ok.Value);
        }

        [Fact]
        public async Task RealizarPagamento_Validations_ReturnExpectedMessages()
        {
            var controller = CreateController();

            var badModel = new RealizarPagamentoDto { MatriculaId = Guid.Empty, ValorCurso = 10m };
            var res1 = await controller.RealizarPagamento(Guid.NewGuid(), badModel);
            var ok1 = Assert.IsType<BadRequestObjectResult>(res1);
            Assert.IsType<ValidationProblemDetails>(ok1.Value);

            var id = Guid.NewGuid();
            var badModel2 = new RealizarPagamentoDto { MatriculaId = Guid.NewGuid(), ValorCurso = 10m };
            var res2 = await controller.RealizarPagamento(id, badModel2);
            var ok2 = Assert.IsType<BadRequestObjectResult>(res2);
            Assert.IsType<ValidationProblemDetails>(ok2.Value);

            controller.ModelState.AddModelError("X", "err");
            var res3 = await controller.RealizarPagamento(id, badModel2);
            Assert.IsType<BadRequestObjectResult>(res3);

            controller.ModelState.Clear();
            var validModel = new RealizarPagamentoDto { MatriculaId = id, ValorCurso = 10m, NumeroCartao = "4111111111111111", TitularCartao = "T", ValidadeCartao = "12/30", CodigoSegurancaCartao = "123" };
            _pagamento.Setup(p => p.RealizarPagamentoAsync(validModel)).ReturnsAsync(true);
            var ctrl2 = CreateController();
            _pagamento.Setup(p => p.RealizarPagamentoAsync(It.IsAny<RealizarPagamentoDto>())).ReturnsAsync(true);
            var res4 = await ctrl2.RealizarPagamento(id, validModel);
            var ok4 = Assert.IsType<OkObjectResult>(res4);
            Assert.Equal(true, ok4.Value);
        }
    }
}
