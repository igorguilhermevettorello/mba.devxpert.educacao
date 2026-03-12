using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PlataformaEducacional.Pedidos.Domain.Pedidos;

namespace PlataformaEducacional.Pedidos.Data.Mappings
{
    public class PedidoMapping : IEntityTypeConfiguration<Pedido>
    {
        private readonly bool isSqlServer;
        public PedidoMapping(bool isSqlServer = false)
        {
            this.isSqlServer = isSqlServer;
        }

        public void Configure(EntityTypeBuilder<Pedido> builder)
        {
            builder.HasKey(c => c.Id);

            // Mapeando Endereco como entidade relacionada (1:1)
            builder.HasOne(p => p.Endereco)
                .WithOne()
                .HasForeignKey<Pedido>("EnderecoId")
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            if (isSqlServer)
            {
                builder.Property(c => c.Codigo)
                       .HasDefaultValueSql("NEXT VALUE FOR MinhaSequencia");
            }
            else
            {
                builder.Property(c => c.Codigo)
                       .HasMaxLength(10)
                       .IsRequired();
            }
            
            // 1 : N => Pedido : PedidoItems
            builder.HasMany(c => c.PedidoItems)
                .WithOne(c => c.Pedido)
                .HasForeignKey(c => c.PedidoId);

            builder.ToTable("Pedidos");
        }
    }
}
