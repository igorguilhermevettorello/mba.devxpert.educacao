using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Moq;
using PlataformaEducacional.Auth.Api.Controllers;
using PlataformaEducacional.Auth.Api.Security;
using PlataformaEducacional.WebApi.Core.Identity;
using Xunit;

namespace PlataformaEducacional.Auth.Api.Tests
{
    public class WellKnownControllerTests
    {
        [Fact]
        public void OpenIdConfiguration_TrimsIssuer_AndBuildsJwksUri()
        {
            var jwtSettings = Options.Create(new JwtSettings
            {
                Emissor = " https://issuer.example.com/ "
            });

            var controller = new WellKnownController();

            var result = controller.OpenIdConfiguration(jwtSettings) as OkObjectResult;
            Assert.NotNull(result);

            var value = result.Value!;
            var issuerProp = value.GetType().GetProperty("issuer");
            Assert.NotNull(issuerProp);
            var issuer = (string)issuerProp.GetValue(value)!;
            Assert.Equal("https://issuer.example.com", issuer);

            var jwksProp = value.GetType().GetProperty("jwks_uri");
            Assert.NotNull(jwksProp);
            var jwks = (string)jwksProp.GetValue(value)!;
            Assert.Equal("https://issuer.example.com/.well-known/jwks.json", jwks);
        }

        [Fact]
        public void Jwks_ReturnsContent_WithCorrectMediaType()
        {
            var signing = new Mock<IJwtRsaSigningCredentialsProvider>();
            signing.SetupGet(s => s.JwksJson).Returns("{\"keys\":[]}");
            var controller = new WellKnownController();

            var content = controller.Jwks(signing.Object);
            Assert.Equal("application/json", content.ContentType);
            Assert.Equal("{\"keys\":[]}", content.Content);
        }
    }
}