namespace PlataformaEducacional.Bff.Api.Extensions;

/// <summary>
/// Configurações das URLs dos serviços utilizados pelo BFF (Backend for Frontend).
/// </summary>
public class AppServicesSettings
{
    /// <summary>
    /// URL base da API de Alunos.
    /// </summary>
    public string AlunoApiUrl { get; set; } = string.Empty;

    /// <summary>
    /// URL base da API de Conteúdo.
    /// </summary>
    public string ConteudoApiUrl { get; set; } = string.Empty;

    /// <summary>
    /// URL base da API de Pagamento.
    /// </summary>
    public string PagamentoApiUrl { get; set; } = string.Empty;
}
