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
            ValidarTitulo(titulo);
            ValidarDescricao(descricao);
            ValidarDuracao(duracaoMinutos);
            ValidarOrdem(ordem);

            Id = Guid.NewGuid();
            Titulo = titulo;
            Descricao = descricao;
            DuracaoMinutos = duracaoMinutos;
            Ordem = ordem;
            DataCriacao = DateTime.UtcNow;
            Ativa = true;
        }

        public void AtualizarTitulo(string novoTitulo)
        {
            ValidarTitulo(novoTitulo);
            Titulo = novoTitulo;
        }

        public void AtualizarDescricao(string novaDescricao)
        {
            ValidarDescricao(novaDescricao);
            Descricao = novaDescricao;
        }

        public void AtualizarDuracao(int novaDuracao)
        {
            ValidarDuracao(novaDuracao);
            DuracaoMinutos = novaDuracao;
        }

        public void AtualizarOrdem(int novaOrdem)
        {
            ValidarOrdem(novaOrdem);
            Ordem = novaOrdem;
        }

        public void AtualizarInformacoes(string titulo, string descricao, int duracaoMinutos, int ordem)
        {
            ValidarTitulo(titulo);
            ValidarDescricao(descricao);
            ValidarDuracao(duracaoMinutos);
            ValidarOrdem(ordem);

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

        // Métodos de validação privados
        private static void ValidarTitulo(string titulo)
        {
            if (string.IsNullOrWhiteSpace(titulo))
                throw new ArgumentException("Título da aula é obrigatório", nameof(titulo));

            if (titulo.Length < 3)
                throw new ArgumentException("Título da aula deve ter no mínimo 3 caracteres", nameof(titulo));

            if (titulo.Length > 200)
                throw new ArgumentException("Título da aula deve ter no máximo 200 caracteres", nameof(titulo));
        }

        private static void ValidarDescricao(string descricao)
        {
            if (string.IsNullOrWhiteSpace(descricao))
                throw new ArgumentException("Descrição da aula é obrigatória", nameof(descricao));

            if (descricao.Length < 10)
                throw new ArgumentException("Descrição da aula deve ter no mínimo 10 caracteres", nameof(descricao));

            if (descricao.Length > 1000)
                throw new ArgumentException("Descrição da aula deve ter no máximo 1000 caracteres", nameof(descricao));
        }

        private static void ValidarDuracao(int duracaoMinutos)
        {
            if (duracaoMinutos <= 0)
                throw new ArgumentException("Duração deve ser maior que zero", nameof(duracaoMinutos));

            if (duracaoMinutos > 600) // 10 horas
                throw new ArgumentException("Duração da aula não pode exceder 600 minutos (10 horas)", nameof(duracaoMinutos));
        }

        private static void ValidarOrdem(int ordem)
        {
            if (ordem <= 0)
                throw new ArgumentException("Ordem deve ser maior que zero", nameof(ordem));

            if (ordem > 9999)
                throw new ArgumentException("Ordem não pode exceder 9999", nameof(ordem));
        }
    }
}
