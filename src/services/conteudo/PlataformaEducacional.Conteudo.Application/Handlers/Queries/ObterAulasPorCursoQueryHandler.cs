using MediatR;
using PlataformaEducacional.Conteudo.Application.Queries;
using PlataformaEducacional.Conteudo.Domain.Entities;
using PlataformaEducacional.Conteudo.Domain.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlataformaEducacional.Conteudo.Application.Handlers.Queries
{
    public class ObterAulasPorCursoQueryHandler : IRequestHandler<ObterAulasPorCursoQuery, IEnumerable<Aula>>
    {
        private readonly IAulaRepository _aulaRepository;

        public ObterAulasPorCursoQueryHandler(IAulaRepository aulaRepository)
        {
            _aulaRepository = aulaRepository;
        }

        public async Task<IEnumerable<Aula>> Handle(ObterAulasPorCursoQuery request, CancellationToken cancellationToken)
        {
            var aulas = await _aulaRepository.ObterAtivasPorCursoIdAsync(request.CursoId);
            return aulas;
        }
    }
}
