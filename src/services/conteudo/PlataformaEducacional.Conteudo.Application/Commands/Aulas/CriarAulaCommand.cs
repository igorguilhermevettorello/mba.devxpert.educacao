using MediatR;
using PlataformaEducacional.Conteudo.Application.Validators;
using PlataformaEducacional.Core.Messages;

namespace PlataformaEducacional.Conteudo.Application.Commands.Aulas
{
    public class CriarAulaCommand : Command, IRequest<bool>
    {
        public Guid CursoId { get; set; }
        public string Titulo { get; set; } = default!;
        public string Descricao { get; set; } = default!;
        public int DuracaoMinutos { get; set; }
        public int Ordem { get; set; }

        public CriarAulaCommand(Guid cursoId, string titulo, string descricao, int duracaoMinutos, int ordem)
        {
            CursoId = cursoId;
            Titulo = titulo;
            Descricao = descricao;
            DuracaoMinutos = duracaoMinutos;
            Ordem = ordem;
        }

        public override bool IsValid()
        {
            ValidationResult = new CriarAulaCommandValidator().Validate(this);
            return ValidationResult.IsValid;
        }

        public void SetAggregateId(Guid aggregateId)
        {
            AggregateId = aggregateId;
        }
    }
}
