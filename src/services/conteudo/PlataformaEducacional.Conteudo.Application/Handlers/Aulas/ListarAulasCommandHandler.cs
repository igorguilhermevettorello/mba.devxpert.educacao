using MediatR;
using PlataformaEducacional.Conteudo.Application.Commands.Aulas;
using PlataformaEducacional.Conteudo.Domain.Entities;
using PlataformaEducacional.Conteudo.Domain.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlataformaEducacional.Conteudo.Application.Handlers.Aulas
{
    public class ListarAulasCommandHandler : IRequestHandler<ListarAulasCommand, IEnumerable<Aula>>
    {
        private readonly IAulaRepository _aulaRepository;

        public ListarAulasCommandHandler(IAulaRepository aulaRepository)
        {
            _aulaRepository = aulaRepository;
        }

        public async Task<IEnumerable<Aula>> Handle(ListarAulasCommand request, CancellationToken cancellationToken)
        {
            // Se foi especificado um curso, filtrar por curso
            if (request.CursoId.HasValue)
            {
                return request.ApenasAtivas
                    ? await _aulaRepository.ObterAtivasPorCursoIdAsync(request.CursoId.Value)
                    : await _aulaRepository.ObterPorCursoIdAsync(request.CursoId.Value);
            }

            // Caso contrário, retornar todas as aulas
            return request.ApenasAtivas
                ? await _aulaRepository.ObterAtivasAsync()
                : await _aulaRepository.ObterTodasAsync();
        }
    }
}
