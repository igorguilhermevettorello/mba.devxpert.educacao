using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using PlataformaEducacional.Conteudo.Data.Context;

namespace PlataformaEducacional.Conteudo.IntegrationTests.Factories
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>, IAsyncDisposable
    {
        private SqliteConnection _connUsuarios = null!;
        private SqliteConnection _connAlunos = null!;
        private SqliteConnection _connCursos = null!;
        private SqliteConnection _connPagamentos = null!;

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Testing");

            builder.ConfigureServices(services =>
            {
                RemoverDbContexts(services);
                CriarConexoesIsoladas();
                AddDbContexts(services);
                AplicarMigrations(services);
                AddAutenticacao(services);
            });
        }

        private void RemoverDbContexts(IServiceCollection services)
        {
            services.RemoveAll(typeof(DbContextOptions<CursoContext>));
            //services.RemoveAll(typeof(DbContextOptions<AlunosDbContext>));
            //services.RemoveAll(typeof(DbContextOptions<CursosDbContext>));
            //services.RemoveAll(typeof(DbContextOptions<PagamentosDbContext>));
        }

        private void CriarConexoesIsoladas()
        {
            //_connUsuarios = CriarConexao();
            //_connAlunos = CriarConexao();
            _connCursos = CriarConexao();
            //_connPagamentos = CriarConexao();
        }

        private static SqliteConnection CriarConexao()
        {
            var conn = new SqliteConnection("DataSource=:memory:");
            conn.Open();
            return conn;
        }

        private void AddDbContexts(IServiceCollection services)
        {
            //services.AddDbContext<UsuariosDbContext>(o => o.UseSqlite(_connUsuarios));
            //services.AddDbContext<AlunosDbContext>(o => o.UseSqlite(_connAlunos));
            services.AddDbContext<CursoContext>(o => o.UseSqlite(_connCursos));
            //services.AddDbContext<PagamentosDbContext>(o => o.UseSqlite(_connPagamentos));
        }

        private void AplicarMigrations(IServiceCollection services)
        {
            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            //scope.ServiceProvider.GetRequiredService<UsuariosDbContext>().Database.Migrate();
            //scope.ServiceProvider.GetRequiredService<AlunosDbContext>().Database.Migrate();
            scope.ServiceProvider.GetRequiredService<CursoContext>().Database.Migrate();
            //scope.ServiceProvider.GetRequiredService<PagamentosDbContext>().Database.Migrate();
        }

        private void AddAutenticacao(IServiceCollection services)
        {
            services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = "Test";
                    options.DefaultChallengeScheme = "Test";
                })
                .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("Test", _ => { });
        }

        public async ValueTask DisposeAsync()
        {
            await _connUsuarios.DisposeAsync();
            await _connAlunos.DisposeAsync();
            await _connCursos.DisposeAsync();
            await _connPagamentos.DisposeAsync();
        }
    }
}
