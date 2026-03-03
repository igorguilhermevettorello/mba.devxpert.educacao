using MediatR;
using PlataformaEducacional.Conteudo.Domain.Entities;

namespace PlataformaEducacional.Conteudo.Application.Queries
{
    public class ObterAulasPorCursoQuery : IRequest<IEnumerable<Aula>>
    {
        public Guid CursoId { get; set; }

        public ObterAulasPorCursoQuery(Guid cursoId)
        {
            CursoId = cursoId;
        }
    }
}
