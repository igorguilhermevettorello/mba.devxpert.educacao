using Microsoft.IdentityModel.Tokens;

namespace PlataformaEducacional.Auth.Api.Security;

public interface IJwtRsaSigningCredentialsProvider
{
    SigningCredentials SigningCredentials { get; }

    string JwksJson { get; }
}
