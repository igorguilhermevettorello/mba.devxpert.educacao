using System.Text.RegularExpressions;

namespace PlataformaEducacional.Pagamentos.Api.Models
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

        public static CartaoCredito Criar(string titular, string numero, string mesAnoVencimento, string cvv)
        {
            if (string.IsNullOrWhiteSpace(titular))
                throw new ArgumentException("O campo TitularCartao é obrigatório", nameof(titular));

            var nomeTrim = titular.Trim();
            if (nomeTrim.Length < 3)
                throw new ArgumentException("O TitularCartao deve ter no mínimo 3 caracteres", nameof(titular));

            if (nomeTrim.Length > 100)
                throw new ArgumentException("O TitularCartao deve ter no máximo 100 caracteres", nameof(titular));

            if (!Regex.IsMatch(nomeTrim, @"^[\p{L} .'\-]+$"))
                throw new ArgumentException("O TitularCartao contém caracteres inválidos", nameof(titular));

            // Numero validations (remove non-digits)
            if (string.IsNullOrWhiteSpace(numero))
                throw new ArgumentException("O campo NumeroCartao é obrigatório", nameof(numero));

            var numeroDigitos = Regex.Replace(numero, @"\D", "");
            if (numeroDigitos.Length == 0)
                throw new ArgumentException("Número de cartão inválido", nameof(numero));

            if (numeroDigitos.Length > 23)
                throw new ArgumentException("Número de cartão inválido", nameof(numero));

            if (!ValidarAlgoritmoLuhn(numeroDigitos))
                throw new ArgumentException("Número de cartão inválido", nameof(numero));

            // Validade validations (MM/AA or MM/AAAA)
            if (string.IsNullOrWhiteSpace(mesAnoVencimento))
                throw new ArgumentException("O campo ValidadeCartao é obrigatório", nameof(mesAnoVencimento));

            var validade = mesAnoVencimento.Trim();
            if (!Regex.IsMatch(validade, @"^(0[1-9]|1[0-2])\/(\d{2}|\d{4})$"))
                throw new ArgumentException("Validade inválida. Use MM/AA ou MM/AAAA", nameof(mesAnoVencimento));

            if (!NaoEstaVencido(validade))
                throw new ArgumentException("Validade inválida ou cartão expirado. Use MM/AA ou MM/AAAA", nameof(mesAnoVencimento));

            // CVV validations
            if (string.IsNullOrWhiteSpace(cvv))
                throw new ArgumentException("O campo CodigoSegurancaCartao é obrigatório", nameof(cvv));

            var cvvTrim = cvv.Trim();
            if (!Regex.IsMatch(cvvTrim, @"^\d{3,4}$"))
                throw new ArgumentException("O CodigoSegurancaCartao deve conter 3 ou 4 dígitos", nameof(cvv));

            return new CartaoCredito(nomeTrim, numeroDigitos, validade, cvvTrim);
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
    }
}