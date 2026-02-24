using Microsoft.EntityFrameworkCore;
using PlataformaEducacional.Alunos.Api.Models;

namespace PlataformaEducacional.Alunos.Api.Data.Mappings;

public class MatriculaMapping : IEntityTypeConfiguration<Matricula>
{
    public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Matricula> builder)
    {
        builder.HasKey(m => m.Id);

        builder.Property(m => m.AlunoId)
               .IsRequired();

        builder.Property(m => m.CursoId)
               .IsRequired();

        builder.Property(m => m.Valor)
               .IsRequired();

        builder.Property(m => m.DataMatricula)
               .IsRequired();

        builder.Property(m => m.Status)
               .HasConversion<int>()
               .IsRequired();

        builder.HasOne(m => m.Aluno)
               .WithMany(a => a.Matriculas)
               .HasForeignKey(m => m.AlunoId);

        builder.HasOne(m => m.Certificado)
               .WithOne(c => c.Matricula)
               .HasForeignKey<Certificado>(c => c.MatriculaId);

        builder.HasMany(m => m.ProgressoAulas)
               .WithOne(p => p.Matricula)
               .HasForeignKey(p => p.MatriculaId);
    }
}