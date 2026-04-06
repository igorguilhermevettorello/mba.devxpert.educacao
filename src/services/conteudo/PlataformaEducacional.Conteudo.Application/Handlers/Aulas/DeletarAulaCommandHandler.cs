using FluentValidation.Results;
using MediatR;
using PlataformaEducacional.Conteudo.Application.Commands.Aulas;
using PlataformaEducacional.Conteudo.Domain.Interfaces.Repositories;
using PlataformaEducacional.Core.Messages;

namespace PlataformaEducacional.Conteudo.Application.Handlers.Aulas
{
    public class DeletarAulaCommandHandler : CommandHandler, IRequestHandler<DeletarAulaCommand, ValidationResult>
    {
        private readonly IAulaRepository _aulaRepository;

        public DeletarAulaCommandHandler(IAulaRepository aulaRepository)
        {
            _aulaRepository = aulaRepository;
        }

        public async Task<ValidationResult> Handle(DeletarAulaCommand request, CancellationToken cancellationToken)
        {
            if (!request.IsValid())
                return request.ValidationResult;

            var aula = await _aulaRepository.BuscarPorIdAsync(request.Id);

            if (aula == null)
            {
                AddError("Aula não encontrada");
                return ValidationResult;
            }

            _aulaRepository.Remover(aula);
            return await PersistData(_aulaRepository.UnitOfWork);
        }
    }
}
