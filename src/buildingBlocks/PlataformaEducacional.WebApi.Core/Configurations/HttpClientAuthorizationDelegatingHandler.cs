using PlataformaEducacional.WebApi.Core.User;
using System.Net.Http.Headers;
using Microsoft.Extensions.Primitives;

namespace PlataformaEducacional.WebApi.Core.Configurations;

public class HttpClientAuthorizationDelegatingHandler : DelegatingHandler
{
    private readonly IAspNetUser _aspNetUser;

    public HttpClientAuthorizationDelegatingHandler(IAspNetUser aspNetUser)
    {
        _aspNetUser = aspNetUser;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var authorizationHeader = _aspNetUser.ObterHttpContext().Request.Headers["Authorization"];

        if (!StringValues.IsNullOrEmpty(authorizationHeader))
        {
            request.Headers.Add("Authorization", authorizationHeader.ToArray());
        }

        var token = _aspNetUser.ObterUserToken();

        if (token != null)
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        return await base.SendAsync(request, cancellationToken);
    }
}
