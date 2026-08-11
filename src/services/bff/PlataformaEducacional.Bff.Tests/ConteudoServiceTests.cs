using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using PlataformaEducacional.Bff.Api.Extensions;
using PlataformaEducacional.Bff.Api.Services;
using PlataformaEducacional.Bff.Api.Interfaces;
using PlataformaEducacional.Bff.Api.Models;
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

            return new HttpClient(handlerMock.Object) { BaseAddress = new Uri("https://test") };
        }

        [Fact]
        public async Task ObterCursoDisponiveisAsync_ReturnsEmpty_WhenEmptyArray()
        {
            var json = JsonSerializer.Serialize(new { data = new object[] { } });
            var http = CreateHttpClient(_ => new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(json) });
            var settings = Options.Create(new AppServicesSettings { });
            var loggerMock = new Mock<ILogger<ConteudoService>>();
            var svc = new ConteudoService(http, settings, loggerMock.Object);

            var result = await svc.ObterCursoDisponiveisAsync();
            Assert.Null(result?.Data);
        }

        [Fact]
        public async Task ObterCursoDisponiveisAsync_ReturnsNull_OnNonSuccess()
        {
            var http = CreateHttpClient(_ => new HttpResponseMessage(HttpStatusCode.BadRequest));
            var settings = Options.Create(new AppServicesSettings { });
            var loggerMock = new Mock<ILogger<ConteudoService>>();
            var svc = new ConteudoService(http, settings, loggerMock.Object);

            var result = await svc.ObterCursoDisponiveisAsync();
            Assert.Null(result);
        }

        [Fact]
        public async Task ObterCursoPorIdAsync_ReturnsCurso_WhenSuccess()
        {
<<<<<<< HEAD
            var curso = new { data = new { id = Guid.NewGuid(), titulo = "Curso X", valor = 123.45m } };
            var json = JsonSerializer.Serialize(curso);
            var http = CreateHttpClient(_ => new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(json) });
            var settings = Options.Create(new AppServicesSettings { });
            var loggerMock = new Mock<ILogger<ConteudoService>>();
            var svc = new ConteudoService(http, settings, loggerMock.Object);

            var result = await svc.ObterCursoPorIdAsync(Guid.NewGuid());
=======
            var cursos = new List<CursoDto> { new CursoDto { Id = Guid.NewGuid(), Titulo = "T" } };
            var resultDto = new ResultDto<IEnumerable<CursoDto>>() { Data = cursos, Success = true };

            //var resultDto = ResultDto<IEnumerable<CursoDto>>.Ok(cursos, "ok");
            var json = JsonSerializer.Serialize(resultDto);

            var http = CreateHttpClient(_ => new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(json)
            });
            var settings = Options.Create(new AppServicesSettings { ConteudoApiUrl = "https://test" });
            var svc = new ConteudoService(http, settings);

            var result = await svc.ObterCursoDisponiveisAsync();
>>>>>>> origin/staging
            Assert.NotNull(result);
        }
    }
}
