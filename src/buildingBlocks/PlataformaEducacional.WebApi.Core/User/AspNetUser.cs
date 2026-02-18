using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace PlataformaEducacional.WebApi.Core.User;

public class AspNetUser : IAspNetUser
{
    private readonly IHttpContextAccessor _accessor;

    public AspNetUser(IHttpContextAccessor accessor)
    {
        _accessor = accessor;
    }

    public string Name => _accessor.HttpContext?.User?.Identity?.Name ?? string.Empty;

    public Guid ObterUserId()
    {
        return EstaAutenticado() ? Guid.Parse(_accessor.HttpContext?.User?.GetUserId() ?? Guid.Empty.ToString()) : Guid.Empty;
    }

    public string ObterUserEmail()
    {
        return EstaAutenticado() ? _accessor.HttpContext?.User?.GetUserEmail() ?? string.Empty : string.Empty;
    }

    public string ObterUserToken()
    {
        return EstaAutenticado() ? _accessor.HttpContext?.User?.GetUserToken() ?? string.Empty : string.Empty;
    }

    public bool EstaAutenticado()
    {
        return _accessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;
    }

    public bool PossuiRole(string role)
    {
        return _accessor.HttpContext?.User?.IsInRole(role) ?? false;
    }

    public IEnumerable<Claim> ObterClaims()
    {
        return _accessor.HttpContext?.User?.Claims ?? Enumerable.Empty<Claim>();
    }

    public HttpContext ObterHttpContext()
    {
        return _accessor.HttpContext!;
    }
}
