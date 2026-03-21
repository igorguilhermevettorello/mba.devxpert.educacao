using FluentValidation.Results;
using MediatR;
using PlataformaEducacional.Core.Messages;
using PlataformaEducacional.Pedidos.Domain.Vouchers;

namespace PlataformaEducacional.Pedidos.API.Application.Commands
{
    public class VoucherCommandHandler : CommandHandler,
        IRequestHandler<AdicionarVoucherCommand, ValidationResult>
    {
        private readonly IVoucherRepository _voucherRepository;

        public VoucherCommandHandler(IVoucherRepository voucherRepository)
        {
            _voucherRepository = voucherRepository;
        }

        public async Task<ValidationResult> Handle(AdicionarVoucherCommand message, CancellationToken cancellationToken)
        {
            if (!message.IsValid()) return message.ValidationResult;

            var voucherExistente = await _voucherRepository.ObterVoucherPorCodigo(message.Codigo);
            
            if (voucherExistente != null)
            {
                AddError("Já existe um voucher com este código");
                return ValidationResult;
            }

            var voucher = Voucher.VoucherFactory(
                message.Codigo,
                message.Percentual,
                message.ValorDesconto,
                message.Quantidade,
                (TipoDescontoVoucher)message.TipoDesconto,
                message.DataValidade
            );

            _voucherRepository.Adicionar(voucher);

            return await PersistData(_voucherRepository.UnitOfWork);
        }
    }
}