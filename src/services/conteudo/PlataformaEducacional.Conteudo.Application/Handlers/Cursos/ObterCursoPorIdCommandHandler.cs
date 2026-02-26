using MediatR;
using PlataformaEducacional.Conteudo.Application.Commands.Cursos;
using PlataformaEducacional.Conteudo.Domain.Entities;
using PlataformaEducacional.Conteudo.Domain.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlataformaEducacional.Conteudo.Application.Handlers.Cursos
{
    public class ObterCursoPorIdCommandHandler : IRequestHandler<ObterCursoPorIdCommand, Curso>
    {
        private readonly ICursoRepository _cursoRepository;

        public ObterCursoPorIdCommandHandler(ICursoRepository cursoRepository)
        {
            _cursoRepository = cursoRepository;
        }

        public async Task<Curso> Handle(ObterCursoPorIdCommand request, CancellationToken cancellationToken)
        {
            return await _cursoRepository.BuscarPorIdAsync(request.Id);
        }
    }
}
