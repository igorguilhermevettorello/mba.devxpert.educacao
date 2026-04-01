namespace PlataformaEducacional.WebApi.Core.Identity;

public class JwtSettings
{
    /// <summary>Segredo HMAC (modo legado). Ignorado quando <see cref="Authority"/> está definido.</summary>
    public string Secret { get; set; } = string.Empty;

    public int ExpiracaoHoras { get; set; }

    /// <summary>Valor do claim iss (emissor). Deve coincidir com o issuer do OpenID discovery quando usar JWKS.</summary>
    public string Emissor { get; set; } = string.Empty;

    public string ValidoEm { get; set; } = string.Empty;

    /// <summary>URL base do Auth para discovery OIDC (ex.: https://localhost:5001). Quando preenchido, validação usa JWKS.</summary>
    public string Authority { get; set; } = string.Empty;

    /// <summary>Caminho relativo à raiz do content root do PEM com chave privada RSA (somente API de Auth).</summary>
    public string SigningKeyPath { get; set; } = string.Empty;

    /// <summary>Identificador da chave (kid) em JWT e JWKS.</summary>
    public string SigningKeyId { get; set; } = string.Empty;
}
