using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PlataformaEducacional.Alunos.Domain.Models;

namespace PlataformaEducacional.Alunos.Data.Mappings;

public class ProgressoAulaMapping : IEntityTypeConfiguration<ProgressoAula>
{
    public void Configure(EntityTypeBuilder<ProgressoAula> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.AulaId)
            .IsRequired();

        builder.Property(c => c.DataConclusao)
            .IsRequired();

        builder.HasOne(c => c.Matricula)
            .WithMany(c => c.ProgressoAulas)
            .HasForeignKey(c => c.MatriculaId);

    }
}
