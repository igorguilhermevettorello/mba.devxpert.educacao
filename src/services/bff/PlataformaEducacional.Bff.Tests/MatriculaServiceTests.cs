using System;
using System.Collections.Generic;
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
        public async Task ObterMatriculaPendentesAsync_ReturnsNull_OnNotFound()
        {
            var http = CreateHttpClient(_ => new HttpResponseMessage(HttpStatusCode.NotFound));
            var svc = new MatriculaService(http);

            var result = await svc.ObterMatriculaPendentesAsync(Guid.NewGuid());
            Assert.Null(result);
        }

        [Fact]
        public async Task ObterMatriculaPendentesAsync_ReturnsList_OnSuccess()
        {
            var list = new List<MatriculaDto> { new MatriculaDto { Id = Guid.NewGuid() } };
            var json = JsonSerializer.Serialize(list);
            var http = CreateHttpClient(_ => new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(json)
            });
            var svc = new MatriculaService(http);

            var result = await svc.ObterMatriculaPendentesAsync(Guid.NewGuid());
            Assert.NotNull(result);
            Assert.Single(result);
        }

        [Fact]
        public async Task ObterMatriculaPorId_ReturnsNull_OnNotFound()
        {
            var http = CreateHttpClient(_ => new HttpResponseMessage(HttpStatusCode.NotFound));
            var svc = new MatriculaService(http);

            var result = await svc.ObterMatriculaPorId(Guid.NewGuid());
            Assert.Null(result);
        }

        [Fact]
        public async Task ObterMatriculaPorId_ReturnsDto_OnSuccess()
        {
            var dto = new MatriculaDto { Id = Guid.NewGuid(), AlunoId = Guid.NewGuid(), CursoId = Guid.NewGuid() };
            var http = CreateHttpClient(_ => new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonSerializer.Serialize(dto))
            });
            var svc = new MatriculaService(http);

            var result = await svc.ObterMatriculaPorId(Guid.NewGuid());
            Assert.NotNull(result);
            Assert.Equal(dto.Id, result.Id);
        }

        [Fact]
        public async Task RealizarMatriculaAsync_ReturnsNull_OnNotFound()
        {
            var http = CreateHttpClient(_ => new HttpResponseMessage(HttpStatusCode.NotFound));
            var svc = new MatriculaService(http);

            var result = await svc.RealizarMatriculaAsync(Guid.NewGuid(), new RealizarMatriculaDto { CursoId = Guid.NewGuid() });
            Assert.Null(result);
        }

        [Fact]
        public async Task RealizarMatriculaAsync_ReturnsDto_OnSuccess()
        {
            var dto = new MatriculaDto { Id = Guid.NewGuid(), AlunoId = Guid.NewGuid(), CursoId = Guid.NewGuid() };
            var http = CreateHttpClient(_ => new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonSerializer.Serialize(dto))
            });
            var svc = new MatriculaService(http);

            var result = await svc.RealizarMatriculaAsync(Guid.NewGuid(), new RealizarMatriculaDto { CursoId = Guid.NewGuid() });
            Assert.NotNull(result);
            Assert.Equal(dto.Id, result.Id);
        }
    }
}