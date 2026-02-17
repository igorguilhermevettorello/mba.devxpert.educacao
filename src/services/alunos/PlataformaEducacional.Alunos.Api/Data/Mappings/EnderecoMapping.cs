using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PlataformaEducacional.Alunos.Api.Models;

namespace PlataformaEducacional.Alunos.Api.Data.Mappings;

public class EnderecoMapping : IEntityTypeConfiguration<Endereco>
{
    public void Configure(EntityTypeBuilder<Endereco> builder)
    {
        builder.ToTable("Enderecos");
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Logradouro)
               .HasColumnType("varchar(200)")
               .IsRequired();

        builder.Property(c => c.Numero)
               .HasColumnType("varchar(50)")
               .IsRequired();

        builder.Property(c => c.Cep)
               .HasColumnType("varchar(20)")
               .IsRequired();

        builder.Property(c => c.Complemento)
               .HasColumnType("varchar(250)");

        builder.Property(c => c.Bairro)
               .HasColumnType("varchar(100)")
               .IsRequired();

        builder.Property(c => c.Cidade)
               .HasColumnType("varchar(100)")
               .IsRequired();

        builder.Property(c => c.Estado)
               .HasColumnType("varchar(50)")
               .IsRequired();
    }
}
