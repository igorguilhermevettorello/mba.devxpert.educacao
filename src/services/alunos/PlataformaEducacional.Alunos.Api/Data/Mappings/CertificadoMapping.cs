using Microsoft.EntityFrameworkCore;
using PlataformaEducacional.Alunos.Api.Models;

namespace PlataformaEducacional.Alunos.Api.Data.Mappings;

public class CertificadoMapping : IEntityTypeConfiguration<Certificado>
{
    public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Certificado> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.MatriculaId)
               .IsRequired();

        builder.Property(c => c.CodigoValidacao)
               .IsRequired();

        builder.Property(c => c.DataEmissao)
               .IsRequired();

        builder.HasOne(c => c.Matricula)
               .WithOne(m => m.Certificado)
               .HasForeignKey<Certificado>(c => c.MatriculaId);

    }
}