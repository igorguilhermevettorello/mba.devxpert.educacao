using PlataformaEducacional.Core.DomainObjects;

namespace PlataformaEducacional.Conteudo.Domain.Entities
{
    public class Aula : Entity
    {
        public string Titulo { get; private set; }
        public string Descricao { get; private set; }
        public int DuracaoMinutos { get; private set; }
        public int Ordem { get; private set; }
        public DateTime DataCriacao { get; private set; }
        public bool Ativa { get; private set; }
        public Guid CursoId { get; private set; }
        public Curso? Curso { get; private set; }
        private Aula() { }
        public Aula(string titulo, string descricao, int duracaoMinutos, int ordem)
        {

            Id = Guid.NewGuid();
            Titulo = titulo;
            Descricao = descricao;
            DuracaoMinutos = duracaoMinutos;
            Ordem = ordem;
            DataCriacao = DateTime.UtcNow;
            Ativa = true;
        }

        //construtor p/ seed
        public Aula(Guid id, string titulo, string descricao, int duracaoMinutos, int ordem)
        {

            Id = id;
            Titulo = titulo;
            Descricao = descricao;
            DuracaoMinutos = duracaoMinutos;
            Ordem = ordem;
            DataCriacao = DateTime.UtcNow;
            Ativa = true;
        }

        public void AtualizarTitulo(string novoTitulo)
        {

            Titulo = novoTitulo;
        }

        public void AtualizarDescricao(string novaDescricao)
        {

            Descricao = novaDescricao;
        }

        public void AtualizarDuracao(int novaDuracao)
        {

            DuracaoMinutos = novaDuracao;
        }

        public void AtualizarOrdem(int novaOrdem)
        {

            Ordem = novaOrdem;
        }

        public void AtualizarInformacoes(string titulo, string descricao, int duracaoMinutos, int ordem)
        {

            Titulo = titulo;
            Descricao = descricao;
            DuracaoMinutos = duracaoMinutos;
            Ordem = ordem;
        }

        public void Inativar()
        {
            Ativa = false;
        }

        public void Ativar()
        {
            Ativa = true;
        }

        public void AssociarCurso(Guid cursoId)
        {
            if (cursoId == Guid.Empty)
                throw new ArgumentException("ID do curso é inválido", nameof(cursoId));

            if (CursoId != Guid.Empty && CursoId != cursoId)
                throw new InvalidOperationException("Aula já está associada a outro curso");

            CursoId = cursoId;
        }

        public bool EstaConcluida(int progressoPercentual)
        {
            return progressoPercentual >= 100;
        }


    }
}
