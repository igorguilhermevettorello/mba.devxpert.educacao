namespace PlataformaEducacional.Core.Messages.Integration;

public class PagamentoConfirmadoIntegrationEvent : IntegrationEvent
{
    public Guid MatriculaId { get; set; }
    public DateTime DataConfirmacao { get; set; }
}