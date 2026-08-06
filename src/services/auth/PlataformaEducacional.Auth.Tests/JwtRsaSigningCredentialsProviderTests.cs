using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Hosting;
using Moq;
using Xunit;
using PlataformaEducacional.WebApi.Core.Identity;
using PlataformaEducacional.Auth.Api.Security;

namespace PlataformaEducacional.Auth.Api.Tests
{
    public class JwtRsaSigningCredentialsProviderTests
    {
        [Fact]
        public void Ctor_Throws_When_SigningKeyPath_IsMissing()
        {
            var settings = Options.Create(new JwtSettings
            {
                SigningKeyPath = string.Empty,
                SigningKeyId = "kid"
            });

            var env = new Mock<IWebHostEnvironment>();
            env.SetupGet(e => e.ContentRootPath).Returns(Path.GetTempPath());

            var ex = Assert.Throws<InvalidOperationException>(() => new JwtRsaSigningCredentialsProvider(settings, env.Object));
            Assert.Contains("SigningKeyPath é obrigatório", ex.Message);
        }

        [Fact]
        public void Ctor_Throws_When_SigningKeyId_IsMissing()
        {
            var settings = Options.Create(new JwtSettings
            {
                SigningKeyPath = "any.pem",
                SigningKeyId = string.Empty
            });

            var env = new Mock<IWebHostEnvironment>();
            env.SetupGet(e => e.ContentRootPath).Returns(Path.GetTempPath());

            var ex = Assert.Throws<InvalidOperationException>(() => new JwtRsaSigningCredentialsProvider(settings, env.Object));
            Assert.Contains("SigningKeyId é obrigatório", ex.Message);
        }

        [Fact]
        public void Ctor_Throws_When_PemFile_DoesNotExist()
        {
            var settings = Options.Create(new JwtSettings
            {
                SigningKeyPath = "nonexistent.pem",
                SigningKeyId = "kid"
            });

            var env = new Mock<IWebHostEnvironment>();
            var tempRoot = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempRoot);
            env.SetupGet(e => e.ContentRootPath).Returns(tempRoot);

            var ex = Assert.Throws<InvalidOperationException>(() => new JwtRsaSigningCredentialsProvider(settings, env.Object));
            Assert.Contains("Chave JWT não encontrada", ex.Message);

            Directory.Delete(tempRoot, recursive: true);
        }

        [Fact]
        public void Ctor_Creates_SigningCredentials_And_Generates_JwksJson()
        {
            string tempDir = Path.Combine(Path.GetTempPath(), "jwttests", Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempDir);
            var pemPathRelative = "private_key.pem";
            var pemFullPath = Path.Combine(tempDir, pemPathRelative);

            try
            {
                // generate PEM (PKCS#8) private key
                using (var rsa = RSA.Create(2048))
                {
                    var pkcs8 = rsa.ExportPkcs8PrivateKey();
                    var base64 = Convert.ToBase64String(pkcs8);
                    var sb = new StringBuilder();
                    sb.AppendLine("-----BEGIN PRIVATE KEY-----");
                    for (int i = 0; i < base64.Length; i += 64)
                    {
                        sb.AppendLine(base64.Substring(i, Math.Min(64, base64.Length - i)));
                    }
                    sb.AppendLine("-----END PRIVATE KEY-----");
                    File.WriteAllText(pemFullPath, sb.ToString());
                }

                var settings = Options.Create(new JwtSettings
                {
                    SigningKeyPath = pemPathRelative,
                    SigningKeyId = "test-kid"
                });

                var env = new Mock<IWebHostEnvironment>();
                env.SetupGet(e => e.ContentRootPath).Returns(tempDir);

                var provider = new JwtRsaSigningCredentialsProvider(settings, env.Object);

                // SigningCredentials assertions
                Assert.NotNull(provider.SigningCredentials);
                Assert.Equal(SecurityAlgorithms.RsaSha256, provider.SigningCredentials.Algorithm);

                var rsaKey = Assert.IsType<RsaSecurityKey>(provider.SigningCredentials.Key);
                Assert.Equal("test-kid", rsaKey.KeyId);

                // JWKs assertions: parse JSON and validate fields
                Assert.False(string.IsNullOrWhiteSpace(provider.JwksJson));
                using var doc = JsonDocument.Parse(provider.JwksJson);
                var root = doc.RootElement;
                Assert.True(root.TryGetProperty("keys", out var keysArray));
                Assert.Equal(JsonValueKind.Array, keysArray.ValueKind);
                Assert.True(keysArray.GetArrayLength() > 0);
                var key = keysArray[0];
                Assert.Equal("RSA", key.GetProperty("kty").GetString());
                Assert.Equal("test-kid", key.GetProperty("kid").GetString());
                Assert.Equal("sig", key.GetProperty("use").GetString());
                Assert.Equal("RS256", key.GetProperty("alg").GetString());

                var n = key.GetProperty("n").GetString();
                var e = key.GetProperty("e").GetString();
                Assert.False(string.IsNullOrWhiteSpace(n));
                Assert.False(string.IsNullOrWhiteSpace(e));
            }
            finally
            {
                try { File.Delete(pemFullPath); } catch { }
                try { Directory.Delete(tempDir, recursive: true); } catch { }
            }
        }
    }
}