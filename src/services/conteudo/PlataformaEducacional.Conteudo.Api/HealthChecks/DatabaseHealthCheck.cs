using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using PlataformaEducacional.Conteudo.Data.Context;

namespace PlataformaEducacional.Conteudo.Api.HealthChecks;

public class DatabaseHealthCheck(CursoContext dbContext) : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        var canConnect = await dbContext.Database.CanConnectAsync(cancellationToken);

        return canConnect
            ? HealthCheckResult.Healthy("Banco de dados acessível.")
            : HealthCheckResult.Unhealthy("Banco de dados indisponível.");
    }
}
