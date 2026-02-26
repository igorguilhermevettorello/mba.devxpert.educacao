using MediatR;
using PlataformaEducacional.Conteudo.Application.Validators;
using PlataformaEducacional.Core.Messages;

namespace PlataformaEducacional.Conteudo.Application.Commands.ConteudosProgramaticos
{
    public class AdicionarConteudoProgramaticoCommand : Command, IRequest<bool>
    {
        public Guid CursoId { get; set; }
        public string Titulo { get; set; }
        public string Descricao { get; set; }
        public int Ordem { get; set; }

        public override bool IsValid()
        {
            ValidationResult = new AdicionarConteudoProgramaticoCommandValidator().Validate(this);
            return ValidationResult.IsValid;
        }
    }
}
