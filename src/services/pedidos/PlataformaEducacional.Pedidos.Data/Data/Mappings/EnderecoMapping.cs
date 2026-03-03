using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PlataformaEducacional.Pedidos.Domain.Pedidos;

public class EnderecoMapping : IEntityTypeConfiguration<Endereco>
{
    public void Configure(EntityTypeBuilder<Endereco> builder)
    {
        builder.Property(e => e.Logradouro)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(e => e.Numero)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(e => e.Complemento)
            .HasMaxLength(250);

        builder.Property(e => e.Bairro)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(e => e.Cep)
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(e => e.Cidade)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(e => e.Estado)
            .HasMaxLength(50)
            .IsRequired();

        builder.ToTable("Enderecos");
    }
}