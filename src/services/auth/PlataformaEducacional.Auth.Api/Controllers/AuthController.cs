using Microsoft.AspNetCore.Mvc;
using PlataformaEducacional.WebApi.Core.Identity;

namespace PlataformaEducacional.Auth.Api.Controllers;

[Route("api/identidade")]
public class AuthController : MainController
{
    [HttpGet("Test")]
    public string Get()
    {
        return "Auth API is running.";
    }
}
