
namespace PlataformaEducacional.Pedidos.Domain.Pedidos
{
    public enum PedidoStatus
    {
        Autorizado = 1,
        Pago = 2,
        Recusado = 3,
        Entregue = 4,
        Cancelado = 5,
        Processado = 6,
        AguardandoProcessamento = 7
    }
}
