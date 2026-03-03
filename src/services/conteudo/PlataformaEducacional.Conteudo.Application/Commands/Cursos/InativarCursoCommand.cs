using MediatR;
using PlataformaEducacional.Conteudo.Application.Validators;
using PlataformaEducacional.Core.Messages;

namespace PlataformaEducacional.Conteudo.Application.Commands.Cursos
{
    public class InativarCursoCommand : Command, IRequest<bool>
    {
        public Guid CursoId { get; set; }

        public InativarCursoCommand(Guid cursoId)
        {
            CursoId = cursoId;
        }

        public override bool IsValid()
        {
            ValidationResult = new InativarCursoCommandValidator().Validate(this);
            return ValidationResult.IsValid;
        }


    }
}
