using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PlataformaEducacional.Alunos.Api.Models;
using PlataformaEducacional.Core.DomainObjects;

namespace PlataformaEducacional.Alunos.Api.Data.Mappings;

public class AlunoMapping : IEntityTypeConfiguration<Aluno>
{
    public void Configure(EntityTypeBuilder<Aluno> builder)
    {
        builder.ToTable("Alunos");
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Nome)
               .HasColumnType("varchar(200)")
               .IsRequired();

        builder.OwnsOne(c => c.Cpf, tf =>
        {
            tf.Property(c => c.Numero)
                .IsRequired()
                .HasMaxLength(Cpf.CpfMaxLength)
                .HasColumnName("Cpf")
                .HasColumnType($"varchar({Cpf.CpfMaxLength})");
        });

        builder.OwnsOne(c => c.Email, tf =>
        {
            tf.Property(c => c.Endereco)
                .IsRequired()
                .HasColumnName("Email")
                .HasColumnType($"varchar({Email.EnderecoMaxLength})");
        });

        // 1 : 1 => Aluno : Endereco
        builder.HasOne(c => c.Endereco)
               .WithOne(c => c.Aluno);
        
    }
}
