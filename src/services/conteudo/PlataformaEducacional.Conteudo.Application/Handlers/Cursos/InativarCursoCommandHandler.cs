using MediatR;
using PlataformaEducacional.Conteudo.Application.Commands.Cursos;
using PlataformaEducacional.Conteudo.Domain.Interfaces.Repositories;
using System.ComponentModel.DataAnnotations;

namespace PlataformaEducacional.Conteudo.Application.Handlers.Cursos
{
    public class InativarCursoCommandHandler : IRequestHandler<InativarCursoCommand, bool>
    {
        private readonly ICursoRepository _cursoRepository;
        private readonly IMediator _mediator;

        public InativarCursoCommandHandler(ICursoRepository cursoRepository, IMediator mediator)
        {
            _cursoRepository = cursoRepository;
            _mediator = mediator;
        }

        public async Task<bool> Handle(InativarCursoCommand request, CancellationToken cancellationToken)
        {
            var curso = await _cursoRepository.BuscarPorIdAsync(request.CursoId);

            if (curso != null)
            {
                curso.Inativar();
                _cursoRepository.Alterar(curso);
                return await _cursoRepository.UnitOfWork.Commit();
            }

            return false;
        }
    }
}
