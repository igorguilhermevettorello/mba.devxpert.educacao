using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PlataformaEducacional.Conteudo.Domain.Entities;

namespace PlataformaEducacional.Conteudo.Data.Mappings
{
    public class CursoMapping : IEntityTypeConfiguration<Curso>
    {
        public void Configure(EntityTypeBuilder<Curso> builder)
        {
            builder.ToTable("Cursos");

            // Chave (assumindo que Entity tem Id do tipo Guid)
            builder.HasKey(c => c.Id);

            builder.Property(c => c.Titulo)
                .IsRequired()
                .HasColumnType("varchar(200)");

            builder.Property(c => c.Descricao)
                .IsRequired()
                .HasColumnType("varchar(1000)");

            builder.Property(c => c.Instrutor)
                .IsRequired()
                .HasColumnType("varchar(150)");

            // Enum - por padrão como int; se preferir string, troque por .HasConversion<string>()
            builder.Property(c => c.Nivel)
                .IsRequired();

            builder.Property(c => c.Valor)
                .HasPrecision(15, 2)
                .IsRequired();

            builder.Property(c => c.DataCriacao)
                .IsRequired();

            builder.Property(c => c.Ativo)
                .IsRequired();

            builder.OwnsOne(c => c.ConteudoProgramatico, cp =>
            {
                cp.Property(p => p.Ementa)
                    .HasColumnType("varchar(2000)")
                    .HasColumnName("Ementa");

                cp.Property(p => p.Objetivo)
                    .HasColumnType("varchar(1000)")
                    .HasColumnName("Objetivo");

                cp.Property(p => p.Bibliografia)
                    .HasColumnType("varchar(2000)")
                    .HasColumnName("Bibliografia");

                cp.Property(p => p.MaterialUrl)
                    .HasColumnType("varchar(500)")
                    .HasColumnName("MaterialUrl");
            });

            builder.HasMany(c => c.Aulas)
                .WithOne(c => c.Curso)
                .HasForeignKey(c => c.CursoId);
        }
    }
}
