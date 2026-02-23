using PlataformaEducacional.Alunos.Api.Models.Enums;
using PlataformaEducacional.Core.DomainObjects;

namespace PlataformaEducacional.Alunos.Api.Models
{
    public class Matricula : Entity
    {
        private readonly List<ProgressoAula> _progressoAulas;
        protected Matricula()
        {
            _progressoAulas = new List<ProgressoAula>();
        }
        public Matricula(Guid alunoId, Guid cursoId, decimal valor) : this()
        {
            Id = Guid.NewGuid();
            AlunoId = alunoId;
            CursoId = cursoId;
            Valor = valor;
            DataMatricula = DateTime.UtcNow;
            Status = EnumStatusMatricula.Pendente;
        }
        public Guid AlunoId { get; private set; }
        public Guid CursoId { get; private set; }
        public decimal Valor { get; private set; }
        public DateTime DataMatricula { get; private set; }
        public EnumStatusMatricula Status { get; private set; }

        public Aluno Aluno { get; protected set; }
        public Certificado Certificado { get; protected set; }
        public IReadOnlyCollection<ProgressoAula> ProgressoAulas => _progressoAulas;


        public void Ativar()
        {
            Status = EnumStatusMatricula.Ativa;
        }
        public void Cancelar()
        {
            Status = EnumStatusMatricula.Cancelada;
        }
        public void Concluir()
        {
            Status = EnumStatusMatricula.Concluida;
        }
        public void AdicionarProgresso(ProgressoAula progresso)
        {
            _progressoAulas.Add(progresso);
        }
    }
}
