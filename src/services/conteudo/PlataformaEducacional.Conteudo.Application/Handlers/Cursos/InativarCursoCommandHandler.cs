using FluentValidation.Results;
using MediatR;
using PlataformaEducacional.Conteudo.Application.Commands.Cursos;
using PlataformaEducacional.Conteudo.Domain.Interfaces.Repositories;
using PlataformaEducacional.Core.Messages;

namespace PlataformaEducacional.Conteudo.Application.Handlers.Cursos
{
    public class InativarCursoCommandHandler : CommandHandler, IRequestHandler<InativarCursoCommand, ValidationResult>
    {
        private readonly ICursoRepository _cursoRepository;

        public InativarCursoCommandHandler(ICursoRepository cursoRepository)
        {
            _cursoRepository = cursoRepository;
        }

        public async Task<ValidationResult> Handle(InativarCursoCommand request, CancellationToken cancellationToken)
        {
            if (!request.IsValid())
                return request.ValidationResult;

            var curso = await _cursoRepository.BuscarPorIdAsync(request.CursoId);

            if (curso == null)
            {
                return new ValidationResult([
                    new ValidationFailure(nameof(InativarCursoCommand.CursoId), "Curso não encontrado")
                ]);
            }

            curso.Inativar();
            _cursoRepository.Alterar(curso);
            return await PersistData(_cursoRepository.UnitOfWork);
        }
    }
}
