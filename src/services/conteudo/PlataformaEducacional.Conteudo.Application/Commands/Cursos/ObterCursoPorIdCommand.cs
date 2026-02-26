using MediatR;
using PlataformaEducacional.Conteudo.Domain.Entities;

namespace PlataformaEducacional.Conteudo.Application.Commands.Cursos
{
    public class ObterCursoPorIdCommand : IRequest<Curso>
    {
        public Guid Id { get; set; }

        public ObterCursoPorIdCommand(Guid id)
        {
            Id = id;
        }
    }
}
