using System.ComponentModel.DataAnnotations;

namespace PlataformaEducacional.Bff.Api.Models;

public class RealizarPagamentoDto
{
    [Required(ErrorMessage = "O campo {0} é obrigatório")]
    public Guid MatriculaId { get; set; }

    [Required(ErrorMessage = "O campo {0} é obrigatório")]
    public decimal ValorCurso { get; set; }

    [Required(ErrorMessage = "O campo {0} é obrigatório")]
    [CreditCard(ErrorMessage = "Número de cartão inválido")]
    [StringLength(23, ErrorMessage = "Número de cartão inválido")] // permite espaços/hífens se houver
    public string NumeroCartao { get; set; } = string.Empty;

    [Required(ErrorMessage = "O campo {0} é obrigatório")]
    [StringLength(100, ErrorMessage = "O {0} deve ter no máximo 100 caracteres")]
    [RegularExpression(@"^[\p{L} .'\-]+$", ErrorMessage = "O {0} contém caracteres inválidos")]
    public string TitularCartao { get; set; } = string.Empty;

    [Required(ErrorMessage = "O campo {0} é obrigatório")]    
    public string ValidadeCartao { get; set; } = string.Empty;

    [Required(ErrorMessage = "O campo {0} é obrigatório")]
    [RegularExpression(@"^\d{3,4}$", ErrorMessage = "O {0} deve conter 3 ou 4 dígitos")]
    public string CodigoSegurancaCartao { get; set; } = string.Empty;

}
