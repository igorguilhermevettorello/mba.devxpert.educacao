using MediatR;
using PlataformaEducacional.Conteudo.Domain.Entities;
using PlataformaEducacional.Core.Messages;

namespace PlataformaEducacional.Conteudo.Application.Commands.Cursos
{
    public class ListarCursosCommand : Command, IRequest<IEnumerable<Curso>>
    {
        public bool ApenasAtivos { get; set; }

        public ListarCursosCommand(bool apenasAtivos = false)
        {
            ApenasAtivos = apenasAtivos;
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
