using PlataformaEducacional.Core.Messages;
using PlataformaEducacional.Pagamentos.Api.Validators;
using System.ComponentModel.DataAnnotations;

namespace PlataformaEducacional.Pagamentos.Api.Models.Commands
{
    public class RealizarPagamentoCommand : Command
    {
        public Guid MatriculaId { get; set; }
        public decimal ValorCurso { get; set; }
        public string NumeroCartao { get; set; } = string.Empty;
        public string TitularCartao { get; set; } = string.Empty;
        public string ValidadeCartao { get; set; } = string.Empty;
        public string CodigoSegurancaCartao { get; set; } = string.Empty;

        public override bool IsValid()
        {
            ValidationResult = new RealizarPagamentoCommandValidator().Validate(this);
            return ValidationResult.IsValid;
        }
    }
}
