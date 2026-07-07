using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace PlataformaEducacional.WebApi.Core.Identity;

public static class JwtConfig
{
    public static void AddJwtConfiguration(this IServiceCollection services, IConfiguration configuration,
        IHostEnvironment? hostEnvironment = null)
    {
        var appSettingsSection = configuration.GetSection("JwtSettings");
        services.Configure<JwtSettings>(appSettingsSection);

        var jwSettings = appSettingsSection.Get<JwtSettings>();
        if (jwSettings == null)
        {
            throw new InvalidOperationException("JwtSettings section is not configured properly.");
        }

        var useAuthority = !string.IsNullOrWhiteSpace(jwSettings.Authority);
        var requireHttpsMetadata = hostEnvironment is null || !hostEnvironment.IsDevelopment();

        services.AddAuthentication(x =>
        {
            x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(x =>
        {
            x.RequireHttpsMetadata = false; /// requireHttpsMetadata;
            x.SaveToken = true;

            if (useAuthority)
            {
                var authority = jwSettings.Authority.Trim().TrimEnd('/');
                x.Authority = authority;
                x.Audience = jwSettings.ValidoEm;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidAudience = jwSettings.ValidoEm,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromMinutes(5)
                };
                return;
            }

            if (string.IsNullOrEmpty(jwSettings.Secret))
            {
                throw new InvalidOperationException("JwtSettings.Secret is required when JwtSettings.Authority is not set.");
            }

            var key = Encoding.UTF8.GetBytes(jwSettings.Secret);

            x.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidAudience = jwSettings.ValidoEm,
                ValidIssuer = jwSettings.Emissor
            };
        });
    }

    public static void UseAuthConfiguration(this IApplicationBuilder app)
    {
        app.UseAuthentication();
        app.UseAuthorization();
    }
}
