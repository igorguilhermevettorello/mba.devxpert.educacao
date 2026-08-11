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
    public class PagamentoServiceTests
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
        public async Task RealizarPagamentoAsync_ReturnsTrue_OnSuccess()
        {
            var http = CreateHttpClient(_ => new HttpResponseMessage(HttpStatusCode.OK));
            var loggerMock = new Mock<ILogger<PagamentoService>>();
            var svc = new PagamentoService(http, loggerMock.Object);

            var result = await svc.RealizarPagamentoAsync(new RealizarPagamentoDto { MatriculaId = Guid.NewGuid(), ValorCurso = 100m });
            Assert.True(result);
        }

        [Fact]
        public async Task RealizarPagamentoAsync_ReturnsFalse_OnNonSuccess()
        {
            var http = CreateHttpClient(_ => new HttpResponseMessage(HttpStatusCode.BadRequest));
            var loggerMock = new Mock<ILogger<PagamentoService>>();
            var svc = new PagamentoService(http, loggerMock.Object);

            var result = await svc.RealizarPagamentoAsync(new RealizarPagamentoDto { MatriculaId = Guid.NewGuid(), ValorCurso = 100m });
            Assert.False(result);
        }

        [Fact]
        public async Task RealizarPagamentoAsync_ReturnsFalse_OnException()
        {
            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            handlerMock
               .Protected()
               .Setup<Task<HttpResponseMessage>>(
                  "SendAsync",
                  ItExpr.IsAny<HttpRequestMessage>(),
                  ItExpr.IsAny<CancellationToken>()
               )
               .ThrowsAsync(new HttpRequestException("fail"));

            var http = new HttpClient(handlerMock.Object) { BaseAddress = new Uri("https://test") };
            var loggerMock = new Mock<ILogger<PagamentoService>>();
            var svc = new PagamentoService(http, loggerMock.Object);

            await Assert.ThrowsAsync<HttpRequestException>(() => svc.RealizarPagamentoAsync(new RealizarPagamentoDto { MatriculaId = Guid.NewGuid(), ValorCurso = 100m }));
        }
    }
}
