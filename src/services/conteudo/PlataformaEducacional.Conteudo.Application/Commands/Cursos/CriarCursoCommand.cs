using MediatR;
using PlataformaEducacional.Conteudo.Application.Validators;
using PlataformaEducacional.Conteudo.Domain.Enums;
using PlataformaEducacional.Core.Messages;

namespace PlataformaEducacional.Conteudo.Application.Commands.Cursos
{
    public class CriarCursoCommand : Command
    {
        public string Titulo { get; set; }
        public string Descricao { get; set; }
        public string Instrutor { get; set; }
        public NivelCurso Nivel { get; set; }
        public decimal Valor { get; set; }
        public ConteudoProgramaticoCommand? ConteudoProgramatico { get; set; }

        public CriarCursoCommand(string titulo, string descricao, string instrutor, NivelCurso nivel, decimal valor, ConteudoProgramaticoCommand? conteudoProgramatico = null)
        {
            Titulo = titulo;
            Descricao = descricao;
            Instrutor = instrutor;
            Nivel = nivel;
            Valor = valor;
            ConteudoProgramatico = conteudoProgramatico;
        }

        public override bool IsValid()
        {
            ValidationResult = new CriarCursoCommandValidator().Validate(this);
            return ValidationResult.IsValid;
        }

        public void SetAggregateId(Guid aggregateId)
        {
            AggregateId = aggregateId;
        }
    }
}
