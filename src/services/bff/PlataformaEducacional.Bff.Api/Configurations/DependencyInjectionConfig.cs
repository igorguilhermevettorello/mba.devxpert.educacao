using PlataformaEducacional.Bff.Api.Interfaces;
using PlataformaEducacional.Bff.Api.Services;

namespace PlataformaEducacional.Bff.Api.Configurations;

public static class DependencyInjectionConfig
{
    public static void RegisterServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IMatriculaService, MatriculaService>();
        services.AddScoped<IConteudoService, ConteudoService>();

        var alunoUrl = configuration.GetValue<string>("AlunoApiUrl");

        if (string.IsNullOrEmpty(alunoUrl))
        {
            throw new InvalidOperationException("AlunoApiUrl is not configured properly.");
        }

        services.AddHttpClient<IMatriculaService, MatriculaService>(client =>
        {
            client.BaseAddress = new Uri(alunoUrl); // URL da Matricula API
        });

        var conteudoUrl = configuration.GetValue<string>("ConteudoApiUrl");

        if (string.IsNullOrEmpty(conteudoUrl))
        {
            throw new InvalidOperationException("ConteudoApiUrl is not configured properly.");
        }
        services.AddHttpClient<IConteudoService, ConteudoService>(client =>
        {
            client.BaseAddress = new Uri(conteudoUrl); // URL da Conteudo API
        });
    }
}
