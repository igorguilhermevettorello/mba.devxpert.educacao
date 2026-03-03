using MediatR;
using PlataformaEducacional.Conteudo.Application.Validators;
using PlataformaEducacional.Conteudo.Domain.Enums;
using PlataformaEducacional.Core.Messages;

namespace PlataformaEducacional.Conteudo.Application.Commands.Cursos
{
    public class AtualizarCursoCommand : Command, IRequest<bool>
    {
        public Guid Id { get; set; }
        public string Titulo { get; set; }
        public string Descricao { get; set; }
        public NivelCurso Nivel { get; set; }
        public ConteudoProgramaticoCommand? ConteudoProgramatico { get; set; }

        public override bool IsValid()
        {
            ValidationResult = new AtualizarCursoCommandValidator().Validate(this);
            return ValidationResult.IsValid;
        }
    }
}
