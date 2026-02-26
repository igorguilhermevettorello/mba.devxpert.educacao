using Microsoft.EntityFrameworkCore;
using PlataformaEducacional.Conteudo.Domain.Entities;
using PlataformaEducacional.Core.Data;
using PlataformaEducacional.Core.Messages;

namespace PlataformaEducacional.Conteudo.Data.Context
{
    public class CursoContext : DbContext, IUnitOfWork
    {
        public CursoContext(DbContextOptions<CursoContext> options) : base(options) { }

        public DbSet<Curso> Cursos { get; set; }

        public DbSet<Aula> Aulas { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            foreach (var property in modelBuilder.Model
                         .GetEntityTypes()
                         .SelectMany(e => e.GetProperties())
                         .Where(p => p.ClrType == typeof(string)))
            {
                property.SetColumnType("varchar(100)");
            }

            // Ignorar propriedades de domínio que não devem ser persistidas
            modelBuilder.Ignore<Event>();

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(CursoContext).Assembly);
        }

        public async Task<bool> Commit()
        {
            var success = (await base.SaveChangesAsync()) > 0;
            // if (success) await _mediator.PublicarEventos(this);
            return success;
        }
    }
}
