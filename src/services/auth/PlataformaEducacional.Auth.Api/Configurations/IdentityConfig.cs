using Microsoft.AspNetCore.Identity;
using PlataformaEducacional.Auth.Api.Data;
using PlataformaEducacional.Auth.Api.Extensions;
using PlataformaEducacional.WebApi.Core.Identity;

namespace PlataformaEducacional.Auth.Api.Configurations;

public static class IdentityConfig
{
    public static IServiceCollection AddIdentityConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDefaultIdentity<IdentityUser>()
            .AddRoles<IdentityRole>()
            .AddErrorDescriber<IdentityMensagensPortugues>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        services.AddJwtConfiguration(configuration);

        return services;
    }
}
