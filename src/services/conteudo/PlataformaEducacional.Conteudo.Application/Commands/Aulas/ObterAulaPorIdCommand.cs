using MediatR;
using PlataformaEducacional.Conteudo.Domain.Entities;
using PlataformaEducacional.Core.Messages;

namespace PlataformaEducacional.Conteudo.Application.Commands.Aulas
{
    public class ObterAulaPorIdCommand : Command, IRequest<Aula?>
    {
        public Guid Id { get; set; }

        public ObterAulaPorIdCommand(Guid id)
        {
            Id = id;
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
