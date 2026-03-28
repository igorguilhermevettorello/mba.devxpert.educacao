using FluentValidation.Results;
using MediatR;
using PlataformaEducacional.Conteudo.Application.Commands.Cursos;
using PlataformaEducacional.Conteudo.Domain.Interfaces.Repositories;
using PlataformaEducacional.Core.Messages;

namespace PlataformaEducacional.Conteudo.Application.Handlers.Cursos
{
    public class AtivarCursoCommandHandler : CommandHandler, IRequestHandler<AtivarCursoCommand, ValidationResult>
    {
        private readonly ICursoRepository _cursoRepository;

        public AtivarCursoCommandHandler(ICursoRepository cursoRepository)
        {
            _cursoRepository = cursoRepository;
        }

        public async Task<ValidationResult> Handle(AtivarCursoCommand request, CancellationToken cancellationToken)
        {
            if (!request.IsValid())
                return request.ValidationResult;

            var curso = await _cursoRepository.BuscarPorIdAsync(request.CursoId);

            if (curso == null)
            {
                return new ValidationResult([
                    new ValidationFailure(nameof(AtivarCursoCommand.CursoId), "Curso não encontrado")
                ]);
            }

            curso.Ativar();
            _cursoRepository.Alterar(curso);
            return await PersistData(_cursoRepository.UnitOfWork);
        }
    }
}
