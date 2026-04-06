using FluentValidation.Results;
using MediatR;
using PlataformaEducacional.Conteudo.Application.Commands.Aulas;
using PlataformaEducacional.Conteudo.Domain.Interfaces.Repositories;
using PlataformaEducacional.Core.Messages;

namespace PlataformaEducacional.Conteudo.Application.Handlers.Aulas
{
    public class InativarAulaCommandHandler : CommandHandler, IRequestHandler<InativarAulaCommand, ValidationResult>
    {
        private readonly IAulaRepository _aulaRepository;

        public InativarAulaCommandHandler(IAulaRepository aulaRepository)
        {
            _aulaRepository = aulaRepository;
        }

        public async Task<ValidationResult> Handle(InativarAulaCommand request, CancellationToken cancellationToken)
        {
            if (!request.IsValid())
                return request.ValidationResult;

            var aula = await _aulaRepository.BuscarPorIdAsync(request.Id);

            if (aula == null)
            {
                AddError("Aula não encontrada");
                return ValidationResult;
            }

            aula.Inativar();
            _aulaRepository.Alterar(aula);
            return await PersistData(_aulaRepository.UnitOfWork);
        }
    }
}
