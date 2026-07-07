using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using PlataformaEducacional.Bff.Api.Services;
using PlataformaEducacional.Core.Communication;
using Xunit;

namespace PlataformaEducacional.Bff.Api.Tests
{
    // Test double to expose protected members of ServiceBase
    internal class TestService : ServiceBase
    {
        public StringContent ObterConteudoPublic(object dado) => ObterConteudo(dado);

        public Task<T> DeserializarObjetoResponsePublic<T>(HttpResponseMessage response) => DeserializarObjetoResponse<T>(response);

        public bool TratarErrosResponsePublic(HttpResponseMessage response) => TratarErrosResponse(response);

        public ResponseResult RetornoOkPublic() => RetornoOk();
    }

    public class ServiceBaseTests
    {
        [Fact]
        public void ObterConteudo_SerializesObject_WithJsonContentType()
        {
            var svc = new TestService();
            var obj = new { Id = 123, Nome = "Teste" };

            var content = svc.ObterConteudoPublic(obj);

            Assert.Equal("application/json", content.Headers.ContentType?.MediaType);
            var json = content.ReadAsStringAsync().Result;
            Assert.Contains("\"Id\":", json);
            Assert.Contains("\"Nome\":", json);
        }

        private class ExampleDto { public int Id { get; set; } }

        [Fact]
        public async Task DeserializarObjetoResponse_Deserializes_WhenValidJson()
        {
            var svc = new TestService();
            var dto = new ExampleDto { Id = 42 };
            var json = JsonSerializer.Serialize(dto);
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };

            var result = await svc.DeserializarObjetoResponsePublic<ExampleDto>(response);

            Assert.NotNull(result);
            Assert.Equal(42, result.Id);
        }

        [Fact]
        public async Task DeserializarObjetoResponse_Throws_WhenNullAndNotSuccessStatus()
        {
            var svc = new TestService();
            var response = new HttpResponseMessage(HttpStatusCode.InternalServerError)
            {
                Content = new StringContent("null", Encoding.UTF8, "application/json")
            };

            await Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await svc.DeserializarObjetoResponsePublic<ExampleDto>(response));
        }

        [Fact]
        public void TratarErrosResponse_ReturnsFalse_OnBadRequest()
        {
            var svc = new TestService();
            var response = new HttpResponseMessage(HttpStatusCode.BadRequest);

            var ok = svc.TratarErrosResponsePublic(response);

            Assert.False(ok);
        }

        [Fact]
        public void TratarErrosResponse_Throws_OnServerError()
        {
            var svc = new TestService();
            var response = new HttpResponseMessage(HttpStatusCode.InternalServerError);

            Assert.Throws<HttpRequestException>(() => svc.TratarErrosResponsePublic(response));
        }

        [Fact]
        public void RetornoOk_ReturnsResponseResultInstance()
        {
            var svc = new TestService();
            var result = svc.RetornoOkPublic();
            Assert.NotNull(result);
            Assert.IsType<ResponseResult>(result);
        }
    }
}