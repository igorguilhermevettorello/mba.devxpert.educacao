using System.Text.RegularExpressions;
using FluentValidation;
using FluentValidation.Results;

namespace PlataformaEducacional.Pagamentos.Api.Models.ValueObjects
{
    public sealed record CartaoCredito
    {
        public string Titular { get; init; }
        public string Numero { get; init; }
        public string MesAnoVencimento { get; init; }
        public string CVV { get; init; }

        private CartaoCredito(string titular, string numero, string mesAnoVencimento, string cvv)
        {
            Titular = titular;
            Numero = numero;
            MesAnoVencimento = mesAnoVencimento;
            CVV = cvv;
        }

        public static (bool IsValid, CartaoCredito? Card, ValidationResult ValidationResult) TryCreate(
            string titular,
            string numero,
            string mesAnoVencimento,
            string cvv)
        {
            var numeroDigitos = Regex.Replace(numero ?? string.Empty, @"\D", "");
            var input = new CartaoCreditoInput(titular, numeroDigitos, mesAnoVencimento, cvv);
            var validator = new CartaoCreditoValidator();
            var validationResult = validator.Validate(input);

            if (!validationResult.IsValid)
                return (false, null, validationResult);

            // normalized values
            var nomeTrim = titular!.Trim();
            var validade = mesAnoVencimento!.Trim();
            var cvvTrim = cvv!.Trim();

            var card = new CartaoCredito(nomeTrim, numeroDigitos, validade, cvvTrim);
            return (true, card, validationResult);
        }

        private static bool NaoEstaVencido(string validade)
        {
            if (string.IsNullOrWhiteSpace(validade))
                return false;

            var parts = validade.Split('/');
            if (parts.Length != 2)
                return false;

            if (!int.TryParse(parts[0], out var month))
                return false;

            if (!int.TryParse(parts[1], out var yearPart))
                return false;

            var year = yearPart;
            if (parts[1].Length == 2)
            {
                year += 2000; // assume 2000-2099 for two-digit years
            }

            if (month < 1 || month > 12)
                return false;

            try
            {
                var lastDayOfMonth = DateTime.DaysInMonth(year, month);
                var expirationDate = new DateTime(year, month, lastDayOfMonth, 23, 59, 59, DateTimeKind.Utc);
                return expirationDate >= DateTime.UtcNow;
            }
            catch
            {
                return false;
            }
        }

        private static bool ValidarAlgoritmoLuhn(string digits)
        {
            if (string.IsNullOrWhiteSpace(digits))
                return false;

            int sum = 0;
            bool alternate = false;
            for (int i = digits.Length - 1; i >= 0; i--)
            {
                if (!char.IsDigit(digits[i]))
                    return false;

                int n = digits[i] - '0';
                if (alternate)
                {
                    n *= 2;
                    if (n > 9) n -= 9;
                }
                sum += n;
                alternate = !alternate;
            }
            return sum % 10 == 0;
        }

        private sealed record CartaoCreditoInput(string Titular, string Numero, string MesAnoVencimento, string CVV);

        private sealed class CartaoCreditoValidator : AbstractValidator<CartaoCreditoInput>
        {
            private const string NomeRegex = @"^[\p{L} .'\-]+$";
            private const string ValidadeRegex = @"^(0[1-9]|1[0-2])\/(\d{2}|\d{4})$";

            public CartaoCreditoValidator()
            {
                //CascadeMode = CascadeMode.Stop;

                RuleFor(x => x.Titular)
                    .NotEmpty()
                    .WithMessage("O campo TitularCartao é obrigatório")
                    .Must(t => !string.IsNullOrWhiteSpace(t) && t.Trim().Length >= 3)
                    .WithMessage("O TitularCartao deve ter no mínimo 3 caracteres")
                    .Must(t => !string.IsNullOrWhiteSpace(t) && t.Trim().Length <= 100)
                    .WithMessage("O TitularCartao deve ter no máximo 100 caracteres")
                    .Matches(NomeRegex).WithMessage("O TitularCartao contém caracteres inválidos");

                RuleFor(x => x.Numero)
                    .NotEmpty().WithMessage("O campo NumeroCartao é obrigatório")
                    .NotEmpty().WithMessage("Número de cartão inválido")
                    .MaximumLength(23).WithMessage("Número de cartão inválido")
                    .Must(ValidarAlgoritmoLuhn).WithMessage("Número de cartão inválido");

                RuleFor(x => x.MesAnoVencimento)
                    .NotEmpty().WithMessage("O campo ValidadeCartao é obrigatório")
                    .Matches(ValidadeRegex).WithMessage("Validade inválida. Use MM/AA ou MM/AAAA")
                    .Must(NaoEstaVencido).WithMessage("Validade inválida ou cartão expirado. Use MM/AA ou MM/AAAA");

                RuleFor(x => x.CVV)
                    .NotEmpty().WithMessage("O campo CodigoSegurancaCartao é obrigatório")
                    .Matches(@"^\d{3,4}$").WithMessage("O CodigoSegurancaCartao deve conter 3 ou 4 dígitos");
            }
        }
    }
}