using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using PlataformaEducacional.WebApi.Core.Identity;
using System.Security.Cryptography;
using System.Text.Json;

namespace PlataformaEducacional.Auth.Api.Security;

public sealed class JwtRsaSigningCredentialsProvider : IJwtRsaSigningCredentialsProvider
{
    private const string RsaSha256 = "RS256";

    public JwtRsaSigningCredentialsProvider(IOptions<JwtSettings> jwtOptions, IWebHostEnvironment env)
    {
        var settings = jwtOptions.Value;

        if (string.IsNullOrWhiteSpace(settings.SigningKeyPath))
        {
            throw new InvalidOperationException("JwtSettings.SigningKeyPath é obrigatório para emissão JWT com RSA.");
        }

        if (string.IsNullOrWhiteSpace(settings.SigningKeyId))
        {
            throw new InvalidOperationException("JwtSettings.SigningKeyId é obrigatório para emissão JWT com RSA.");
        }

        var pemPath = Path.Combine(env.ContentRootPath, settings.SigningKeyPath);
        if (!File.Exists(pemPath))
        {
            throw new InvalidOperationException($"Chave JWT não encontrada em '{pemPath}'.");
        }

        var pem = File.ReadAllText(pemPath);
        using var rsa = RSA.Create();
        rsa.ImportFromPem(pem);

        var keyParameters = rsa.ExportParameters(includePrivateParameters: true);
        var rsaSecurityKey = new RsaSecurityKey(keyParameters) { KeyId = settings.SigningKeyId };

        SigningCredentials = new SigningCredentials(rsaSecurityKey, SecurityAlgorithms.RsaSha256);

        var publicOnly = new RSAParameters
        {
            Modulus = keyParameters.Modulus,
            Exponent = keyParameters.Exponent
        };
        JwksJson = BuildJwksJson(publicOnly, settings.SigningKeyId);
    }

    public SigningCredentials SigningCredentials { get; }

    public string JwksJson { get; }

    private static string BuildJwksJson(RSAParameters pubParameters, string kid)
    {
        var n = Base64UrlEncoder.Encode(pubParameters.Modulus!);
        var e = Base64UrlEncoder.Encode(pubParameters.Exponent!);

        var keys = new[]
        {
            new Dictionary<string, string>(StringComparer.Ordinal)
            {
                ["kty"] = "RSA",
                ["kid"] = kid,
                ["use"] = "sig",
                ["alg"] = RsaSha256,
                ["n"] = n,
                ["e"] = e
            }
        };

        return JsonSerializer.Serialize(new { keys });
    }
}
