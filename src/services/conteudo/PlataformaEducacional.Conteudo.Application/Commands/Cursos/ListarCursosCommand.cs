using MediatR;
using PlataformaEducacional.Conteudo.Domain.Entities;

namespace PlataformaEducacional.Conteudo.Application.Commands.Cursos
{
    public class ListarCursosCommand : IRequest<IEnumerable<Curso>>
    {
        public bool ApenasAtivos { get; set; }

        public ListarCursosCommand(bool apenasAtivos = false)
        {
            ApenasAtivos = apenasAtivos;
        }
    }
}
