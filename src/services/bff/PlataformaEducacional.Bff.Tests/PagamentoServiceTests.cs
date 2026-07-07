using System;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Moq.Protected;
using PlataformaEducacional.Bff.Api.Models;
using PlataformaEducacional.Bff.Api.Services;
using Xunit;

namespace PlataformaEducacional.Bff.Api.Tests
{
    public class PagamentoServiceTests
    {
        private static HttpClient CreateHttpClient(HttpResponseMessage response, Action<HttpRequestMessage>? inspect = null)
        {
            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            handlerMock
               .Protected()
               .Setup<Task<HttpResponseMessage>>(
                  "SendAsync",
                  ItExpr.IsAny<HttpRequestMessage>(),
                  ItExpr.IsAny<CancellationToken>()
               )
               .ReturnsAsync((HttpRequestMessage req, CancellationToken _) =>
               {
                   inspect?.Invoke(req);
                   return response;
               });

            return new HttpClient(handlerMock.Object)
            {
                BaseAddress = new Uri("https://test")
            };
        }

        [Fact]
        public async Task RealizarPagamentoAsync_ReturnsTrue_OnSuccessStatus()
        {
            var response = new HttpResponseMessage(HttpStatusCode.OK);
            var http = CreateHttpClient(response, req =>
            {
                Assert.Equal(HttpMethod.Post, req.Method);
                Assert.Equal("/api/pagamentos", req.RequestUri?.AbsolutePath);
            });

            var service = new PagamentoService(http);

            var dto = new RealizarPagamentoDto
            {
                MatriculaId = Guid.NewGuid(),
                ValorCurso = 10m,
                NumeroCartao = "4111111111111111",
                TitularCartao = "Teste",
                ValidadeCartao = "12/30",
                CodigoSegurancaCartao = "123"
            };

            var result = await service.RealizarPagamentoAsync(dto);
            Assert.True(result);
        }

        [Fact]
        public async Task RealizarPagamentoAsync_ReturnsFalse_OnBadRequest()
        {
            var response = new HttpResponseMessage(HttpStatusCode.BadRequest);
            var http = CreateHttpClient(response);

            var service = new PagamentoService(http);

            var dto = new RealizarPagamentoDto
            {
                MatriculaId = Guid.NewGuid(),
                ValorCurso = 10m,
                NumeroCartao = "invalid",
                TitularCartao = "Teste",
                ValidadeCartao = "12/30",
                CodigoSegurancaCartao = "123"
            };

            var result = await service.RealizarPagamentoAsync(dto);
            Assert.False(result);
        }
    }
}