using FluentValidation.Results;
using MediatR;
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

        public CriarAulaCommandHandler(
            IAulaRepository aulaRepository,
            ICursoRepository cursoRepository)
        {
            _aulaRepository = aulaRepository;
            _cursoRepository = cursoRepository;
        }

        public async Task<ValidationResult> Handle(CriarAulaCommand request, CancellationToken cancellationToken)
        {
            if (!request.IsValid())
                return request.ValidationResult;

            var curso = await _cursoRepository.BuscarPorIdAsync(request.CursoId);

            if (curso == null)
            {
                AddError("Curso não encontrado");
                return ValidationResult;
            }

            if (!curso.Ativo)
            {
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

                return await PersistData(_aulaRepository.UnitOfWork);
            }
            catch (ArgumentException ex)
            {
                AddError(ex.Message);
                return ValidationResult;
            }
        }
    }
}
