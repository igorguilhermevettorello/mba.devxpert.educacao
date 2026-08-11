using System;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using PlataformaEducacional.Pagamentos.Api.Services;
using PlataformaEducacional.Pagamentos.Api.Models.DTOs;
using Xunit;

namespace PlataformaEducacional.Pagamentos.Api.Tests
{
    public class ConteudoServiceTests
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
        public async Task ObterCursoPorIdAsync_ReturnsCurso_WhenSuccess()
        {
            var curso = new { data = new { id = Guid.NewGuid(), titulo = "Curso X", valor = 123.45m } };
            var json = JsonSerializer.Serialize(curso);

            var http = CreateHttpClient(_ => new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(json)
            });

            var loggerMock = new Mock<ILogger<ConteudoService>>();
            var svc = new ConteudoService(http, loggerMock.Object);

            var result = await svc.ObterCursoPorIdAsync(Guid.NewGuid());

            Assert.NotNull(result);
            Assert.Equal("Curso X", result!.Titulo);
            Assert.Equal(123.45m, result.Valor);
        }

        [Fact]
        public async Task ObterCursoPorIdAsync_ReturnsNull_OnNonSuccessStatus()
        {
            var http = CreateHttpClient(_ => new HttpResponseMessage(HttpStatusCode.NotFound));
            var loggerMock = new Mock<ILogger<ConteudoService>>();
            var svc = new ConteudoService(http, loggerMock.Object);

            var result = await svc.ObterCursoPorIdAsync(Guid.NewGuid());

            Assert.Null(result);
        }

        [Fact]
        public async Task ObterCursoPorIdAsync_ReturnsNull_OnException()
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
            var loggerMock = new Mock<ILogger<ConteudoService>>();
            var svc = new ConteudoService(http, loggerMock.Object);

            var result = await svc.ObterCursoPorIdAsync(Guid.NewGuid());

            Assert.Null(result);
        }
    }
}
