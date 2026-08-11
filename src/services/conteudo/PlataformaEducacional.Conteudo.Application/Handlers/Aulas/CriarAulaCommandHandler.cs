using FluentValidation.Results;
using MediatR;
using Microsoft.Extensions.Logging;
using PlataformaEducacional.Conteudo.Application.Commands.Aulas;
using PlataformaEducacional.Conteudo.Domain.Entities;
using PlataformaEducacional.Conteudo.Domain.Interfaces.Repositories;
using PlataformaEducacional.Core.Messages;

namespace PlataformaEducacional.Conteudo.Application.Handlers.Aulas
{
    public class CriarAulaCommandHandler : CommandHandler, IRequestHandler<CriarAulaCommand, ValidationResult>
    {
        private readonly IAulaRepository _aulaRepository;
        private readonly ICursoRepository _cursoRepository;
        private readonly ILogger<CriarAulaCommandHandler> _logger;

        public CriarAulaCommandHandler(
            IAulaRepository aulaRepository,
            ICursoRepository cursoRepository,
            ILogger<CriarAulaCommandHandler> logger)
        {
            _aulaRepository = aulaRepository;
            _cursoRepository = cursoRepository;
            _logger = logger;
        }

        public async Task<ValidationResult> Handle(CriarAulaCommand request, CancellationToken cancellationToken)
        {
            if (!request.IsValid())
                return request.ValidationResult;

            var curso = await _cursoRepository.BuscarPorIdAsync(request.CursoId);

            if (curso == null)
            {
                _logger.LogWarning("Curso não encontrado - CursoId {CursoId}", request.CursoId);
                AddError("Curso não encontrado");
                return ValidationResult;
            }

            if (!curso.Ativo)
            {
                _logger.LogWarning("Tentativa de adicionar aula a curso inativo - CursoId {CursoId}", request.CursoId);
                AddError("Não é possível adicionar aulas a um curso inativo");
                return ValidationResult;
            }

            try
            {
                var aula = new Aula(
                    request.Titulo,
                    request.Descricao,
                    request.DuracaoMinutos,
                    request.Ordem);

                aula.AssociarCurso(request.CursoId);
                curso.AdicionarAula(aula);

                request.SetAggregateId(aula.Id);
                _aulaRepository.Adicionar(aula);

                var result = await PersistData(_aulaRepository.UnitOfWork);
                if (result.IsValid)
                    _logger.LogInformation("Aula criada com sucesso - AulaId {AulaId}, CursoId {CursoId}", aula.Id, request.CursoId);
                return result;
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Erro ao criar aula - CursoId {CursoId}", request.CursoId);
                AddError(ex.Message);
                return ValidationResult;
            }
        }
    }
}
