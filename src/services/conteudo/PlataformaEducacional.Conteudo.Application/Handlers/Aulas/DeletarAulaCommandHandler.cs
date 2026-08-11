using FluentValidation.Results;
using MediatR;
using Microsoft.Extensions.Logging;
using PlataformaEducacional.Conteudo.Application.Commands.Aulas;
using PlataformaEducacional.Conteudo.Domain.Interfaces.Repositories;
using PlataformaEducacional.Core.Messages;

namespace PlataformaEducacional.Conteudo.Application.Handlers.Aulas
{
    public class DeletarAulaCommandHandler : CommandHandler, IRequestHandler<DeletarAulaCommand, ValidationResult>
    {
        private readonly IAulaRepository _aulaRepository;
        private readonly ILogger<DeletarAulaCommandHandler> _logger;

        public DeletarAulaCommandHandler(IAulaRepository aulaRepository, ILogger<DeletarAulaCommandHandler> logger)
        {
            _aulaRepository = aulaRepository;
            _logger = logger;
        }

        public async Task<ValidationResult> Handle(DeletarAulaCommand request, CancellationToken cancellationToken)
        {
            if (!request.IsValid())
                return request.ValidationResult;

            var aula = await _aulaRepository.BuscarPorIdAsync(request.Id);

            if (aula == null)
            {
                _logger.LogWarning("Aula não encontrada - AulaId {AulaId}", request.Id);
                AddError("Aula não encontrada");
                return ValidationResult;
            }

            _aulaRepository.Remover(aula);
            var result = await PersistData(_aulaRepository.UnitOfWork);
            if (result.IsValid)
                _logger.LogInformation("Aula deletada com sucesso - AulaId {AulaId}", request.Id);
            return result;
        }
    }
}
