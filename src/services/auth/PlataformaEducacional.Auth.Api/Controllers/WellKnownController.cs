using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using PlataformaEducacional.Auth.Api.Security;
using PlataformaEducacional.WebApi.Core.Identity;

namespace PlataformaEducacional.Auth.Api.Controllers;

[AllowAnonymous]
public class WellKnownController : ControllerBase
{
    [HttpGet("/.well-known/openid-configuration")]
    [Produces("application/json")]
    public ActionResult OpenIdConfiguration([FromServices] IOptions<JwtSettings> jwtOptions)
    {
        var settings = jwtOptions.Value;
        var issuer = settings.Emissor.Trim().TrimEnd('/');
        return Ok(new
        {
            issuer,
            jwks_uri = $"{issuer}/.well-known/jwks.json"
        });
    }

    [HttpGet("/.well-known/jwks.json")]
    [Produces("application/json")]
    public ContentResult Jwks([FromServices] IJwtRsaSigningCredentialsProvider signing)
    {
        return Content(signing.JwksJson, "application/json");
    }
}
