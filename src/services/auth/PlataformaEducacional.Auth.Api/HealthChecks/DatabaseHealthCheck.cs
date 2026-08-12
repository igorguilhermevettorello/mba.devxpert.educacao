using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using PlataformaEducacional.Auth.Api.Data;

namespace PlataformaEducacional.Auth.Api.HealthChecks;

public class DatabaseHealthCheck(ApplicationDbContext dbContext) : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        var canConnect = await dbContext.Database.CanConnectAsync(cancellationToken);

        return canConnect
            ? HealthCheckResult.Healthy("Banco de dados acessível.")
            : HealthCheckResult.Unhealthy("Banco de dados indisponível.");
    }
}
