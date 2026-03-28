using PlataformaEducacional.Conteudo.Application.Validators;
using PlataformaEducacional.Core.Messages;

namespace PlataformaEducacional.Conteudo.Application.Commands.Cursos
{
    public class DeletarCursoCommand : Command
    {
        public Guid Id { get; set; }

        public DeletarCursoCommand(Guid id)
        {
            Id = id;
        }

        public override bool IsValid()
        {
            ValidationResult = new DeletarCursoCommandValidator().Validate(this);
            return ValidationResult.IsValid;
        }
    }
}
