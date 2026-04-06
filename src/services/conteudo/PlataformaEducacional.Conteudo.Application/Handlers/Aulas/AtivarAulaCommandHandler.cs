using FluentValidation.Results;
using MediatR;
using PlataformaEducacional.Conteudo.Application.Commands.Aulas;
using PlataformaEducacional.Conteudo.Domain.Interfaces.Repositories;
using PlataformaEducacional.Core.Messages;

namespace PlataformaEducacional.Conteudo.Application.Handlers.Aulas
{
    public class AtivarAulaCommandHandler : CommandHandler, IRequestHandler<AtivarAulaCommand, ValidationResult>
    {
        private readonly IAulaRepository _aulaRepository;

        public AtivarAulaCommandHandler(IAulaRepository aulaRepository)
        {
            _aulaRepository = aulaRepository;
        }

        public async Task<ValidationResult> Handle(AtivarAulaCommand request, CancellationToken cancellationToken)
        {
            if (!request.IsValid())
                return request.ValidationResult;

            var aula = await _aulaRepository.BuscarPorIdAsync(request.AulaId);

            if (aula == null)
            {
                AddError("Aula não encontrada");
                return ValidationResult;
            }

            aula.Ativar();
            _aulaRepository.Alterar(aula);
            return await PersistData(_aulaRepository.UnitOfWork);
        }
    }
}
