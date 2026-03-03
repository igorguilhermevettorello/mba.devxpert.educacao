using MediatR;
using PlataformaEducacional.Conteudo.Application.Validators;
using PlataformaEducacional.Core.Messages;

namespace PlataformaEducacional.Conteudo.Application.Commands.Cursos
{
    public class AtivarCursoCommand : Command, IRequest<bool>
    {
        public Guid CursoId { get; set; }

        public override bool IsValid()
        {
            ValidationResult = new AtivarCursoCommandValidator().Validate(this);
            return ValidationResult.IsValid;
        }
    }
}
