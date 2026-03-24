namespace PlataformaEducacional.Core.Messages.Integration;

public class MatriculaIniciadaIntegrationEvent : IntegrationEvent
{
    //TODO: renomear classe para MatriculaRealizada
    public Guid MatriculaId { get; set; }
    public Guid AlunoId { get; set; }
    public Guid CursoId { get; set; }
    public decimal ValorCurso { get; set; }

    public int TipoPagamento { get; set; }
    public string Titular { get; set; } = string.Empty;
    public string NumeroCartao { get; set; } = string.Empty;
    public string Validade { get; set; } = string.Empty;
    public string CodigoSeguranca { get; set; } = string.Empty;
}