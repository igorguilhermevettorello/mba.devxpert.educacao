using PlataformaEducacional.Conteudo.Application.Validators;
using PlataformaEducacional.Core.Messages;
using PlataformaEducacional.WebApi.Core.Enumerators;

namespace PlataformaEducacional.Conteudo.Application.Commands.Cursos;

public class AtualizarCursoCommand : Command
{
    public Guid Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public string Instrutor { get; set; } = string.Empty;
    public NivelCurso Nivel { get; set; }
    public decimal Valor { get; set; }

    public ConteudoProgramaticoCommand? ConteudoProgramatico { get; set; }

    public override bool IsValid()
    {
        ValidationResult = new AtualizarCursoCommandValidator().Validate(this);
        return ValidationResult.IsValid;
    }
}
