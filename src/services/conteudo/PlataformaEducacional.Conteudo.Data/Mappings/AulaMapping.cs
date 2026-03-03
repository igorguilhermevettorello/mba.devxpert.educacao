using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PlataformaEducacional.Conteudo.Domain.Entities;

namespace PlataformaEducacional.Conteudo.Data.Mappings
{
    public class AulaMapping : IEntityTypeConfiguration<Aula>
    {
        public void Configure(EntityTypeBuilder<Aula> builder)
        {
            builder.ToTable("Aulas");

            builder.HasKey(a => a.Id);

            builder.Property(a => a.Titulo)
                .IsRequired()
                .HasColumnType("varchar(200)");

            builder.Property(a => a.Descricao)
                .IsRequired()
                .HasColumnType("varchar(1000)");

            builder.Property(a => a.DuracaoMinutos)
                .IsRequired();

            builder.Property(a => a.Ordem)
                .IsRequired();

            builder.Property(a => a.DataCriacao)
                .IsRequired();

            builder.Property(a => a.Ativa)
                .IsRequired();

            builder.Property<Guid>("CursoId")
                .IsRequired();
        }
    }
}
