using Microsoft.EntityFrameworkCore;
using PlataformaEducacional.Alunos.Data;
using PlataformaEducacional.Alunos.Domain.Models;

namespace PlataformaEducacional.Alunos.Api.Configuration.Seed;

public static class DatabaseMigrationStartDataExtension
{
    public static void UseDatabaseMigrationStartData(this WebApplication app)
    {
        EnsureSeedData(app).Wait();
    }

    private static async Task EnsureSeedData(WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var services = scope.ServiceProvider;
        var env = services.GetRequiredService<IWebHostEnvironment>();

        try
        {
            if (env.IsDevelopment() || env.IsEnvironment("Docker") || env.IsStaging())
            {
                var context = services.GetRequiredService<AlunosContext>();

                await context.Database.MigrateAsync();

                await EnsureSeedAlunos(context);
            }
        }
        catch (Exception ex)
        {
            var logger = services.GetRequiredService<ILogger<Program>>();
            logger.LogError(ex, "An error occurred while migrating the database.");
        }
    }

    private static async Task EnsureSeedAlunos(AlunosContext context)
    {
        if (await context.Alunos.AnyAsync())
            return;

        var alunos = new List<Aluno>
        {
            // Aluno 1: Teste Principal
            new Aluno(Guid.Parse("65EFB6D9-2374-4E87-8D83-C8E76C2B9765"), 
                "Aluno Teste", 
                "aluno.teste@educa.com", 
                "32009883985"),
            
            // Aluno 2: João Silva
            new Aluno(Guid.Parse("75EFB6D9-2374-4E87-8D83-C8E76C2B9766"), 
                "João Silva", 
                "joao.silva@educa.com", 
                "32009883985"),
            
            // Aluno 3: Maria Santos
            new Aluno(Guid.Parse("85EFB6D9-2374-4E87-8D83-C8E76C2B9767"), 
                "Maria Santos", 
                "maria.santos@educa.com", 
                "32009883985"),
            
            // Aluno 4: Pedro Oliveira
            new Aluno(Guid.Parse("95EFB6D9-2374-4E87-8D83-C8E76C2B9768"), 
                "Pedro Oliveira", 
                "pedro.oliveira@educa.com", 
                "32009883985"),
        };

        // Adicionando endereços para cada aluno
        var enderecos = new List<Endereco>
        {
            // Endereço do Aluno 1
            new Endereco(
                "Rua Teste", "123", "Apto 101", "Bairro Teste", "12345678", 
                "Cidade Teste", "SP", alunos[0].Id),
            
            // Endereço do Aluno 2
            new Endereco(
                "Rua das Flores", "456", "Apto 202", "Jardim América", "23456789", 
                "São Paulo", "SP", alunos[1].Id),
            
            // Endereço do Aluno 3
            new Endereco(
                "Avenida Paulista", "789", "Sala 1001", "Bela Vista", "34567890", 
                "São Paulo", "SP", alunos[2].Id),
            
            // Endereço do Aluno 4
            new Endereco(
                "Rua da Paz", "321", "Casa 01", "Vila Mariana", "45678901", 
                "São Paulo", "SP", alunos[3].Id),
        };

        // Associando endereços aos alunos
        for (int i = 0; i < alunos.Count; i++)
        {
            alunos[i].AtribuirEndereco(enderecos[i]);
        }

        // Adicionando matrículas para os alunos
        var matriculas = new List<Matricula>
        {
            // Matrículas do Aluno 1 (Aluno Teste)
            new Matricula(alunos[0].Id, Guid.Parse("11111111-1111-1111-1111-111111111111")), // C# Fundamentos
            new Matricula(alunos[0].Id, Guid.Parse("22222222-2222-2222-2222-222222222222")), // ASP.NET Core Web API
            
            // Matrículas do Aluno 2 (João Silva)
            new Matricula(alunos[1].Id, Guid.Parse("11111111-1111-1111-1111-111111111111")), // C# Fundamentos
            new Matricula(alunos[1].Id, Guid.Parse("33333333-3333-3333-3333-333333333333")), // Clean Architecture
            
            // Matrículas do Aluno 3 (Maria Santos)
            new Matricula(alunos[2].Id, Guid.Parse("22222222-2222-2222-2222-222222222222")), // ASP.NET Core Web API
            new Matricula(alunos[2].Id, Guid.Parse("44444444-4444-4444-4444-444444444444")), // SQL Server e EF Core
            
            // Matrículas do Aluno 4 (Pedro Oliveira)
            new Matricula(alunos[3].Id, Guid.Parse("44444444-4444-4444-4444-444444444444")), // SQL Server e EF Core
            new Matricula(alunos[3].Id, Guid.Parse("55555555-5555-5555-5555-555555555555")), // Testes Automatizados
        };

        // Ativando matrículas para simular fluxo completo
        foreach (var matricula in matriculas)
        {
            matricula.Ativar();
        }

        // Adicionando dados ao contexto
        context.Alunos.AddRange(alunos);
        context.Matriculas.AddRange(matriculas);
        
        await context.SaveChangesAsync();
    }
}
