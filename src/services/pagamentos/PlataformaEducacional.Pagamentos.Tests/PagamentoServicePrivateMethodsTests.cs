using System;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using PlataformaEducacional.Pagamentos.Api.Facade;
using PlataformaEducacional.Pagamentos.Api.Models;
using PlataformaEducacional.Pagamentos.Api.Models.DTOs;
using PlataformaEducacional.Pagamentos.Api.Models.ValueObjects;
using PlataformaEducacional.Pagamentos.Api.Services;
using Xunit;

namespace PlataformaEducacional.Pagamentos.Api.Tests
{
    public class PagamentoServicePrivateMethodsTests
    {
        private PagamentoService CreateService(
            out Mock<IPagamentoFacade> facadeMock,
            out Mock<IPagamentoRepository> repoMock,
            out Mock<PlataformaEducacional.MessageBus.IMessageBus> busMock,
            out Mock<IConteudoService> conteudoMock,
            out Mock<IAlunoService> alunoMock)
        {
            facadeMock = new Mock<IPagamentoFacade>();
            repoMock = new Mock<IPagamentoRepository>();
            busMock = new Mock<PlataformaEducacional.MessageBus.IMessageBus>();
            conteudoMock = new Mock<IConteudoService>();
            alunoMock = new Mock<IAlunoService>();

            var loggerMock = new Mock<Microsoft.Extensions.Logging.ILogger<PagamentoService>>();
            return new PagamentoService(facadeMock.Object, repoMock.Object, busMock.Object, conteudoMock.Object, alunoMock.Object, loggerMock.Object);
        }

        [Fact]
        public async Task MatriculaExiste_PrivateMethod_ReturnsFalse_WhenNotFound()
        {
            var service = CreateService(out _, out _, out _, out _, out var alunoMock);
            alunoMock.Setup(a => a.ObterMatriculaPorIdAsync(It.IsAny<Guid>())).ReturnsAsync((MatriculaDto?)null);

            var method = typeof(PagamentoService).GetMethod("MatriculaExiste", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!;
            var task = (Task<bool>)method.Invoke(service, new object[] { Guid.NewGuid() })!;
            var result = await task;

            Assert.False(result);
        }

        [Fact]
        public async Task MatriculaExiste_PrivateMethod_ReturnsTrue_WhenFound()
        {
            var service = CreateService(out _, out _, out _, out _, out var alunoMock);
            alunoMock.Setup(a => a.ObterMatriculaPorIdAsync(It.IsAny<Guid>())).ReturnsAsync(new MatriculaDto { Id = Guid.NewGuid() });

            var method = typeof(PagamentoService).GetMethod("MatriculaExiste", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!;
            var task = (Task<bool>)method.Invoke(service, new object[] { Guid.NewGuid() })!;
            var result = await task;

            Assert.True(result);
        }

        [Fact]
        public async Task ValidarValorDoPagamento_PrivateMethod_ValidatesPrice()
        {
            var service = CreateService(out _, out _, out _, out var conteudoMock, out var alunoMock);

            var matriculaId = Guid.NewGuid();
            var cursoId = Guid.NewGuid();
            alunoMock.Setup(a => a.ObterMatriculaPorIdAsync(matriculaId))
                .ReturnsAsync(new MatriculaDto { Id = matriculaId, CursoId = cursoId });

            conteudoMock.Setup(c => c.ObterCursoPorIdAsync(cursoId))
                .ReturnsAsync(new CursoDto { Id = cursoId, Valor = 50m });

            var cartao = CartaoCredito.TryCreate("T", "4111111111111111", "12/30", "123").Card!;
            var pagamento = new Pagamento(matriculaId, Models.Enums.TipoPagamento.CartaoCredito, 50m, cartao);

            var method = typeof(PagamentoService).GetMethod("ValidarValorDoPagamento", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!;
            var task = (Task<bool>)method.Invoke(service, new object[] { pagamento })!;
            var result = await task;

            Assert.True(result);

            var pagamento2 = new Pagamento(matriculaId, Models.Enums.TipoPagamento.CartaoCredito, 100m, cartao);
            var task2 = (Task<bool>)method.Invoke(service, new object[] { pagamento2 })!;
            var result2 = await task2;
            Assert.False(result2);
        }
    }
}
