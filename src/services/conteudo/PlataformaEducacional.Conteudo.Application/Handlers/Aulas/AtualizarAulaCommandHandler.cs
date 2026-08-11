using FluentValidation.Results;
using MediatR;
using Microsoft.Extensions.Logging;
using PlataformaEducacional.Conteudo.Application.Commands.Aulas;
using PlataformaEducacional.Conteudo.Domain.Interfaces.Repositories;
using PlataformaEducacional.Core.Messages;

namespace PlataformaEducacional.Conteudo.Application.Handlers.Aulas
{
    public class AtualizarAulaCommandHandler : CommandHandler, IRequestHandler<AtualizarAulaCommand, ValidationResult>
    {
        private readonly IAulaRepository _aulaRepository;
        private readonly ILogger<AtualizarAulaCommandHandler> _logger;

        public AtualizarAulaCommandHandler(IAulaRepository aulaRepository, ILogger<AtualizarAulaCommandHandler> logger)
        {
            _aulaRepository = aulaRepository;
            _logger = logger;
        }

        public async Task<ValidationResult> Handle(AtualizarAulaCommand request, CancellationToken cancellationToken)
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

            try
            {
                aula.AtualizarTitulo(request.Titulo);
                aula.AtualizarDescricao(request.Descricao);
                aula.AtualizarDuracao(request.DuracaoMinutos);
                aula.AtualizarOrdem(request.Ordem);

                _aulaRepository.Alterar(aula);
                var result = await PersistData(_aulaRepository.UnitOfWork);
                if (result.IsValid)
                    _logger.LogInformation("Aula atualizada com sucesso - AulaId {AulaId}", request.Id);
                return result;
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Erro ao atualizar aula - AulaId {AulaId}", request.Id);
                AddError(ex.Message);
                return ValidationResult;
            }
        }
    }
}
