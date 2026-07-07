using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using PlataformaEducacional.Bff.Api.Extensions;
using PlataformaEducacional.Bff.Api.Models;
using PlataformaEducacional.Bff.Api.Services;
using Xunit;

namespace PlataformaEducacional.Bff.Api.Tests
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

            return new HttpClient(handlerMock.Object);
        }

        [Fact]
        public async Task ObterCursoDisponiveisAsync_ReturnsNull_OnNotFound()
        {
            var http = CreateHttpClient(_ => new HttpResponseMessage(HttpStatusCode.NotFound));
            var settings = Options.Create(new AppServicesSettings { ConteudoApiUrl = "https://test" });
            var svc = new ConteudoService(http, settings);

            var result = await svc.ObterCursoDisponiveisAsync();
            Assert.Null(result);
        }

        [Fact]
        public async Task ObterCursoDisponiveisAsync_ReturnsResult_OnSuccess()
        {
            var cursos = new List<CursoDto> { new CursoDto { Id = Guid.NewGuid(), Titulo = "T" } };

            var resultDto = new ResultDto<IEnumerable<CursoDto>>(); //{ cursos };
            resultDto.Data = cursos;

            //var resultDto = ResultDto<IEnumerable<CursoDto>>.Ok(cursos, "ok");
            var json = JsonSerializer.Serialize(resultDto);

            var http = CreateHttpClient(_ => new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(json)
            });
            var settings = Options.Create(new AppServicesSettings { ConteudoApiUrl = "https://test" });
            var svc = new ConteudoService(http, settings);

            var result = await svc.ObterCursoDisponiveisAsync();
            Assert.NotNull(result);
            Assert.True(result.Success);
            Assert.NotNull(result.Data);
        }
    }
}