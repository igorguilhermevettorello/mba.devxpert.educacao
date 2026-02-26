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
    public class ListarCursosCommandHandler : IRequestHandler<ListarCursosCommand, IEnumerable<Curso>>
    {
        private readonly ICursoRepository _cursoRepository;

        public ListarCursosCommandHandler(ICursoRepository cursoRepository)
        {
            _cursoRepository = cursoRepository;
        }

        public async Task<IEnumerable<Curso>> Handle(ListarCursosCommand request, CancellationToken cancellationToken)
        {
            if (request.ApenasAtivos)
            {
                return await _cursoRepository.ObterAtivosAsync();
            }

            return await _cursoRepository.ObterTodosAsync();
        }
    }
}
