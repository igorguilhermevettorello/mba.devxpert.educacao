using Microsoft.EntityFrameworkCore;
using PlataformaEducacional.Alunos.Data;
using PlataformaEducacional.Alunos.Domain.Models;
using PlataformaEducacional.SeedDados;

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

        AddAlunos(context);
        AddMatriculas(context);
        AddProgressoAulas(context);
        AddCertificadoConclusao(context);
        await context.SaveChangesAsync();
    }

    private static void AddAlunos(AlunosContext context)
    {
        var aluno1 = new Aluno(SeedUsuario.ALUNO1_ID, SeedUsuario.ALUNO1_NOME, SeedUsuario.ALUNO1_EMAIL, SeedUsuario.ALUNO1_CPF);
        var endereco1 = new Endereco("Rua Teste", "123", "Apto 101", "Bairro Teste", "12345678", "Cidade Teste", "SP", aluno1.Id);
        aluno1.AtribuirEndereco(endereco1);

        var aluno2 = new Aluno(SeedUsuario.ALUNO2_ID, SeedUsuario.ALUNO2_NOME, SeedUsuario.ALUNO2_EMAIL, SeedUsuario.ALUNO2_CPF);
        var endereco2 = new Endereco("Rua das Flores", "456", "Apto 202", "Jardim América", "23456789", "São Paulo", "SP", aluno2.Id);
        aluno2.AtribuirEndereco(endereco2);

        context.Alunos.Add(aluno1);
        context.Alunos.Add(aluno2);
    }

    private static void AddMatriculas(AlunosContext context)
    {
        var matricula1 = new Matricula(SeedMatriculas.MATRICULA_ALUNO1_ID, SeedUsuario.ALUNO1_ID, SeedConteudo.CURSO_FUNDAMENTOS_CSHARP_ID);
        var matricula2 = new Matricula(SeedMatriculas.MATRICULA_ALUNO2_ID, SeedUsuario.ALUNO2_ID, SeedConteudo.CURSO_SQL_ID);

        matricula1.Concluir();  //Este aluno terá concluido todas as aulas do curso
        matricula2.Ativar();

        context.Matriculas.Add(matricula1);
        context.Matriculas.Add(matricula2);
    }

    private static void AddProgressoAulas(AlunosContext context)
    {
        var progressoAulas = new List<ProgressoAula>()
        {
            new ProgressoAula(SeedMatriculas.MATRICULA_ALUNO1_ID, SeedConteudo.AULA_01_ID),
            new ProgressoAula(SeedMatriculas.MATRICULA_ALUNO1_ID, SeedConteudo.AULA_02_ID),
            new ProgressoAula(SeedMatriculas.MATRICULA_ALUNO1_ID, SeedConteudo.AULA_03_ID),
            new ProgressoAula(SeedMatriculas.MATRICULA_ALUNO1_ID, SeedConteudo.AULA_04_ID),
            new ProgressoAula(SeedMatriculas.MATRICULA_ALUNO1_ID, SeedConteudo.AULA_05_ID),
            new ProgressoAula(SeedMatriculas.MATRICULA_ALUNO1_ID, SeedConteudo.AULA_06_ID),
            new ProgressoAula(SeedMatriculas.MATRICULA_ALUNO1_ID, SeedConteudo.AULA_07_ID),
            new ProgressoAula(SeedMatriculas.MATRICULA_ALUNO1_ID, SeedConteudo.AULA_08_ID),
            new ProgressoAula(SeedMatriculas.MATRICULA_ALUNO1_ID, SeedConteudo.AULA_09_ID),
            new ProgressoAula(SeedMatriculas.MATRICULA_ALUNO1_ID, SeedConteudo.AULA_10_ID),
        };

        context.ProgressoAulas.AddRange(progressoAulas);
    }

    private static void AddCertificadoConclusao(AlunosContext context)
    {
        var certificado = new Certificado(SeedMatriculas.MATRICULA_ALUNO1_ID);
        context.Certificados.Add(certificado);
    }
}
