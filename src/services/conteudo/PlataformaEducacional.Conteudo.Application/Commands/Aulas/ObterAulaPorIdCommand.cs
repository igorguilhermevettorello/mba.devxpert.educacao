using MediatR;
using PlataformaEducacional.Conteudo.Domain.Entities;

namespace PlataformaEducacional.Conteudo.Application.Commands.Aulas
{
    public class ObterAulaPorIdCommand : IRequest<Aula?>
    {
        public Guid Id { get; set; }

        public ObterAulaPorIdCommand(Guid id)
        {
            Id = id;
        }
    }
}
