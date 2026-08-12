using System;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using PlataformaEducacional.Bff.Api.Services;
using PlataformaEducacional.Bff.Api.Interfaces;
using PlataformaEducacional.Bff.Api.Models;
using Xunit;

namespace PlataformaEducacional.Bff.Api.Tests
{
    public class MatriculaServiceTests
    {
        private static HttpClient CreateHttpClient(Func<HttpRequestMessage, HttpResponseMessage> respond)
        {
            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            handlerMock
               .Protected()
               .Setup<Task<HttpResponseMessage>>(
                  "SendAsync",
                  ItExpr.IsAny<HttpRequestMessage>(),
                  ItExpr.IsAny<CancellationToken>()
               )
               .ReturnsAsync((HttpRequestMessage req, CancellationToken _) => respond(req));

            return new HttpClient(handlerMock.Object) { BaseAddress = new Uri("https://test") };
        }

        [Fact]
        public async Task ObterMatriculaPendentesAsync_ReturnsEmpty_OnEmptyArray()
        {
            var json = JsonSerializer.Serialize(new object[] { });
            var http = CreateHttpClient(_ => new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(json) });
            var loggerMock = new Mock<ILogger<MatriculaService>>();
            var svc = new MatriculaService(http, loggerMock.Object);

            var result = await svc.ObterMatriculaPendentesAsync(Guid.NewGuid());
            Assert.Empty(result ?? new List<MatriculaDto>());
        }

        [Fact]
        public async Task ObterMatriculaPendentesAsync_ReturnsEmptyList_OnNonSuccess()
        {
            var http = CreateHttpClient(_ => new HttpResponseMessage(HttpStatusCode.NotFound));
            var loggerMock = new Mock<ILogger<MatriculaService>>();
            var svc = new MatriculaService(http, loggerMock.Object);

            var result = await svc.ObterMatriculaPendentesAsync(Guid.NewGuid());
            Assert.Empty(result ?? new List<MatriculaDto>());
        }

        [Fact]
        public async Task ObterMatriculaPorIdAsync_ReturnsMatricula_OnSuccess()
        {
            var dto = new MatriculaDto { Id = Guid.NewGuid() };
            var json = JsonSerializer.Serialize(dto);
            var http = CreateHttpClient(_ => new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(json) });
            var loggerMock = new Mock<ILogger<MatriculaService>>();
            var svc = new MatriculaService(http, loggerMock.Object);

            var result = await svc.ObterMatriculaPorIdAsync(dto.Id);
            Assert.NotNull(result);
        }

        [Fact]
        public async Task ObterMatriculaPorIdAsync_ReturnsNull_OnNonSuccess()
        {
            var http = CreateHttpClient(_ => new HttpResponseMessage(HttpStatusCode.NotFound));
            var loggerMock = new Mock<ILogger<MatriculaService>>();
            var svc = new MatriculaService(http, loggerMock.Object);

            var result = await svc.ObterMatriculaPorIdAsync(Guid.NewGuid());
            Assert.Null(result);
        }

        [Fact]
        public async Task RealizarMatriculaAsync_ReturnsMatricula_OnSuccess()
        {
            var dto = new MatriculaDto { Id = Guid.NewGuid() };
            var json = JsonSerializer.Serialize(dto);
            var http = CreateHttpClient(_ => new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(json) });
            var loggerMock = new Mock<ILogger<MatriculaService>>();
            var svc = new MatriculaService(http, loggerMock.Object);

            var result = await svc.RealizarMatriculaAsync(Guid.NewGuid(), new RealizarMatriculaDto { CursoId = Guid.NewGuid() });
            Assert.NotNull(result);
        }

        [Fact]
        public async Task RealizarMatriculaAsync_Throws_OnNonSuccess()
        {
            var http = CreateHttpClient(_ => new HttpResponseMessage(HttpStatusCode.BadRequest));
            var loggerMock = new Mock<ILogger<MatriculaService>>();
            var svc = new MatriculaService(http, loggerMock.Object);

            await Assert.ThrowsAsync<JsonException>(() => svc.RealizarMatriculaAsync(Guid.NewGuid(), new RealizarMatriculaDto { CursoId = Guid.NewGuid() }));
        }
    }
}
