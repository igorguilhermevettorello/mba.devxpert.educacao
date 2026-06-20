using System;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Moq.Protected;
using PlataformaEducacional.Pagamentos.Api.Services;
using PlataformaEducacional.Pagamentos.Api.Models.DTOs;
using Xunit;

namespace PlataformaEducacional.Pagamentos.Api.Tests
{
    public class AlunoServiceTests
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
        public async Task ObterMatriculaPorIdAsync_ReturnsMatricula_OnSuccess()
        {
            var dto = new MatriculaDto { Id = Guid.NewGuid(), AlunoId = Guid.NewGuid(), CursoId = Guid.NewGuid() };
            var json = JsonSerializer.Serialize(dto);

            var http = CreateHttpClient(_ => new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(json)
            });

            var svc = new AlunoService(http);

            var result = await svc.ObterMatriculaPorIdAsync(dto.Id);

            Assert.NotNull(result);
            Assert.Equal(dto.Id, result!.Id);
        }

        [Fact]
        public async Task ObterMatriculaPorIdAsync_ReturnsNull_OnNonSuccess()
        {
            var http = CreateHttpClient(_ => new HttpResponseMessage(HttpStatusCode.BadRequest));
            var svc = new AlunoService(http);

            var result = await svc.ObterMatriculaPorIdAsync(Guid.NewGuid());

            Assert.Null(result);
        }

        [Fact]
        public async Task ObterMatriculaPorIdAsync_ReturnsNull_OnException()
        {
            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            handlerMock
               .Protected()
               .Setup<Task<HttpResponseMessage>>(
                  "SendAsync",
                  ItExpr.IsAny<HttpRequestMessage>(),
                  ItExpr.IsAny<CancellationToken>()
               )
               .ThrowsAsync(new Exception("network"));

            var http = new HttpClient(handlerMock.Object) { BaseAddress = new Uri("https://test") };
            var svc = new AlunoService(http);

            var result = await svc.ObterMatriculaPorIdAsync(Guid.NewGuid());

            Assert.Null(result);
        }
    }
}