using FluentValidation;
using PlataformaEducacional.Pagamentos.Api.Models.Commands;

namespace PlataformaEducacional.Pagamentos.Api.Validators
{
    public class RealizarPagamentoCommandValidator : AbstractValidator<RealizarPagamentoCommand>
    {
        public RealizarPagamentoCommandValidator()
        {
            RuleFor(x => x.MatriculaId)
                .NotEmpty()
                .WithMessage("O campo MatriculaId é obrigatório");

            RuleFor(x => x.ValorCurso)
                .GreaterThan(0m)
                .WithMessage("O campo ValorCurso deve ser maior que zero");

            RuleFor(x => x.NumeroCartao)
                .NotEmpty()
                .WithMessage("O campo NumeroCartao é obrigatório")
                .CreditCard()
                .WithMessage("Número de cartão inválido")
                .MaximumLength(23)
                .WithMessage("Número de cartão inválido");

            RuleFor(x => x.TitularCartao)
                .NotEmpty()
                .WithMessage("O campo TitularCartao é obrigatório")
                .MaximumLength(100)
                .WithMessage("O TitularCartao deve ter no máximo 100 caracteres")
                .MinimumLength(3)
                .WithMessage("O TitularCartao deve ter no mínimo 3 caracteres")
                .Matches(@"^[\p{L} .'\-]+$")
                .WithMessage("O TitularCartao contém caracteres inválidos");

            RuleFor(x => x.ValidadeCartao)
                .NotEmpty()
                .WithMessage("O campo ValidadeCartao é obrigatório")
                .Matches(@"^(0[1-9]|1[0-2])\/(\d{2}|\d{4})$")
                .WithMessage("Validade inválida. Use MM/AA ou MM/AAAA")
                .Must(NotBeExpired)
                .WithMessage("Validade inválida ou cartão expirado. Use MM/AA ou MM/AAAA");

            RuleFor(x => x.CodigoSegurancaCartao)
                .NotEmpty()
                .WithMessage("O campo CodigoSegurancaCartao é obrigatório")
                .Matches(@"^\d{3,4}$")
                .WithMessage("O CodigoSegurancaCartao deve conter 3 ou 4 dígitos");
        }

        private bool NotBeExpired(string validade)
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

            int year = yearPart;
            if (parts[1].Length == 2)
            {
                // assume 2000-2099 for two-digit years
                year += 2000;
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
    }
}
