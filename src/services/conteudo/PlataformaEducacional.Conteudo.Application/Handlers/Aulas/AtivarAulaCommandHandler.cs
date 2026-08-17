using FluentValidation.Results;
using MediatR;
using Microsoft.Extensions.Logging;
using PlataformaEducacional.Conteudo.Application.Commands.Aulas;
using PlataformaEducacional.Conteudo.Domain.Interfaces.Repositories;
using PlataformaEducacional.Core.Messages;

namespace PlataformaEducacional.Conteudo.Application.Handlers.Aulas
{
    public class AtivarAulaCommandHandler : CommandHandler, IRequestHandler<AtivarAulaCommand, ValidationResult>
    {
        private readonly IAulaRepository _aulaRepository;
        private readonly ILogger<AtivarAulaCommandHandler> _logger;

        public AtivarAulaCommandHandler(IAulaRepository aulaRepository, ILogger<AtivarAulaCommandHandler> logger)
        {
            _aulaRepository = aulaRepository;
            _logger = logger;
        }

        public async Task<ValidationResult> Handle(AtivarAulaCommand request, CancellationToken cancellationToken)
        {
            if (!request.IsValid())
                return request.ValidationResult;

            var aula = await _aulaRepository.BuscarPorIdAsync(request.AulaId);

            if (aula == null)
            {
                _logger.LogWarning("Aula não encontrada - AulaId {AulaId}", request.AulaId);
                AddError("Aula não encontrada");
                return ValidationResult;
            }

            aula.Ativar();
            _aulaRepository.Alterar(aula);
            var result = await PersistData(_aulaRepository.UnitOfWork);
            if (result.IsValid)
                _logger.LogInformation("Aula ativada com sucesso - AulaId {AulaId}", request.AulaId);
            return result;
        }
    }
}
