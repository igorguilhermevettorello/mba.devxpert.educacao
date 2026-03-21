using FluentValidation;
using PlataformaEducacional.Core.Messages;
using PlataformaEducacional.Pedidos.Domain.Vouchers;

namespace PlataformaEducacional.Pedidos.API.Application.Commands
{
    public class AdicionarVoucherCommand : Command
    {
        public string Codigo { get; set; }
        public decimal? Percentual { get; set; }
        public decimal? ValorDesconto { get; set; }
        public int Quantidade { get; set; }
        public TipoDescontoVoucher TipoDesconto { get; set; }
        public DateTime DataValidade { get; set; }

        public override bool IsValid()
        {
            ValidationResult = new AdicionarVoucherValidation().Validate(this);
            return ValidationResult.IsValid;
        }

        public class AdicionarVoucherValidation : AbstractValidator<AdicionarVoucherCommand>
        {
            public AdicionarVoucherValidation()
            {
                RuleFor(c => c.Codigo)
                    .NotEmpty()
                    .WithMessage("O código do voucher é obrigatório")
                    .MaximumLength(50)
                    .WithMessage("O código deve ter no máximo 50 caracteres");

                RuleFor(c => c.Quantidade)
                    .GreaterThan(0)
                    .WithMessage("A quantidade deve ser maior que zero");

                RuleFor(c => c.TipoDesconto)
                    .IsInEnum()
                    .WithMessage("Tipo de desconto inválido");

                RuleFor(c => c.DataValidade)
                    .GreaterThan(DateTime.Now)
                    .WithMessage("A data de validade deve ser futura");

                RuleFor(c => c)
                    .Must(HaveValidDiscount)
                    .WithMessage("Informe o percentual ou o valor de desconto");
            }

            private bool HaveValidDiscount(AdicionarVoucherCommand command)
            {
                if (command.TipoDesconto == 0) // Porcentagem
                {
                    return command.Percentual.HasValue && command.Percentual > 0 && command.Percentual <= 100;
                }

                // Valor
                return command.ValorDesconto.HasValue && command.ValorDesconto > 0;
            }
        }
    }
}