using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PlataformaEducacional.Pedidos.Domain.Vouchers;

namespace PlataformaEducacional.Pedidos.Data.Mappings
{
    public class VoucherMapping : IEntityTypeConfiguration<Voucher>
    {
        public void Configure(EntityTypeBuilder<Voucher> builder)
        {
            builder.ToTable("Vouchers");
            builder.HasKey(c => c.Id);

            builder.Property(c => c.Codigo)
                .HasColumnType("varchar(100)")
                .IsRequired();

            builder.Property(c => c.Percentual)
                .HasColumnType("decimal(18,2)");

            builder.Property(c => c.ValorDesconto)
                .HasColumnType("decimal(18,2)");

            builder.Property(c => c.Quantidade)
                .IsRequired();

            builder.Property(c => c.TipoDesconto)
                .HasConversion<int>()
                .IsRequired();

            builder.Property(c => c.DataCriacao)
                .IsRequired();

            builder.Property(c => c.DataUtilizacao);

            builder.Property(c => c.DataValidade)
                .IsRequired();

            builder.Property(c => c.Ativo)
                .IsRequired();

            builder.Property(c => c.Utilizado)
                .IsRequired();

        }
    }
}
