using PlataformaEducacional.Core.DomainObjects;

namespace PlataformaEducacional.Alunos.Api.Models
{
    public class ProgressoAula : Entity
    {
        protected ProgressoAula() { }
        public ProgressoAula(Guid matriculaId, Guid aulaId)
        {
            MatriculaId = matriculaId;
            AulaId = aulaId;
            DataConclusao = DateTime.UtcNow;
        }
        public Guid MatriculaId { get; private set; }
        public Guid AulaId { get; private set; }
        public DateTime DataConclusao { get; private set; }
        public Matricula Matricula { get; protected set; }
    }
}
