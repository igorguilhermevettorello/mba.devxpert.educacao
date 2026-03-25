using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace PlataformaEducacional.Pagamentos.Api.Models.DTOs
{
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
        [CardExpiration(ErrorMessage = "Validade inválida ou cartão expirado. Use MM/AA ou MM/AAAA")]
        public string ValidadeCartao { get; set; } = string.Empty;

        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        [RegularExpression(@"^\d{3,4}$", ErrorMessage = "O {0} deve conter 3 ou 4 dígitos")]
        public string CodigoSegurancaCartao { get; set; } = string.Empty;
    }

    /// <summary>
    /// Valida formato MM/AA ou MM/AAAA e se a data de validade ainda não expirou.
    /// </summary>
    public class CardExpirationAttribute : ValidationAttribute
    {
        //TODO: remover classe
        private static readonly Regex _regex = new(@"^(0[1-9]|1[0-2])\/?([0-9]{2}|[0-9]{4})$",
                                                   RegexOptions.Compiled | RegexOptions.CultureInvariant);

        public override bool IsValid(object value)
        {
            if (value is not string s) return false;
            s = s.Trim();
            if (string.IsNullOrEmpty(s)) return false;

            var m = _regex.Match(s);
            if (!m.Success) return false;

            if (!int.TryParse(m.Groups[1].Value, out int month)) return false;
            if (!int.TryParse(m.Groups[2].Value, out int yearPart)) return false;

            int year = yearPart;
            if (m.Groups[2].Value.Length == 2)
            {
                // assume 2000+ for two-digit years (common for card expiration)
                year += 2000;
            }

            try
            {
                int lastDay = DateTime.DaysInMonth(year, month);
                // consider expiration at the end of the month (end of day)
                var expiry = new DateTime(year, month, lastDay, 23, 59, 59, DateTimeKind.Utc);
                return expiry >= DateTime.UtcNow;
            }
            catch
            {
                return false;
            }
        }

        public override string FormatErrorMessage(string name) => ErrorMessage ?? base.FormatErrorMessage(name);
    }
}
