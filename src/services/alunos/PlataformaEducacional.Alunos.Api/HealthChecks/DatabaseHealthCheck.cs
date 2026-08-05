using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using PlataformaEducacional.Alunos.Data;

namespace PlataformaEducacional.Alunos.Api.HealthChecks;

public class DatabaseHealthCheck(AlunosContext dbContext) : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        var canConnect = await dbContext.Database.CanConnectAsync(cancellationToken);

        return canConnect
            ? HealthCheckResult.Healthy("Banco de dados acessível.")
            : HealthCheckResult.Unhealthy("Banco de dados indisponível.");
    }
}
