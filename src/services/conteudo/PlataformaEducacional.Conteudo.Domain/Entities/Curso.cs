using PlataformaEducacional.Conteudo.Domain.Enums;
using PlataformaEducacional.Conteudo.Domain.ValueObjects;
using PlataformaEducacional.Core.DomainObjects;

namespace PlataformaEducacional.Conteudo.Domain.Entities
{
    public class Curso : Entity, IAggregateRoot
    {
        public string Titulo { get; private set; }
        public string Descricao { get; private set; }
        public string Instrutor { get; private set; }
        public NivelCurso Nivel { get; private set; }
        public decimal Valor { get; private set; }
        public DateTime DataCriacao { get; private set; }
        public bool Ativo { get; private set; }
        private readonly List<Aula> _aulas;
        public IReadOnlyCollection<Aula> Aulas => _aulas.AsReadOnly();
        public ConteudoProgramatico ConteudoProgramatico { get; private set; }

        private Curso()
        {
            _aulas = new List<Aula>();
        }

        public Curso(string titulo, string descricao, string instrutor, NivelCurso nivel, decimal valor) : this()
        {
            ValidarTitulo(titulo);
            ValidarDescricao(descricao);
            ValidarInstrutor(instrutor);
            ValidarNivel(nivel);
            ValidarValor(valor);

            Titulo = titulo;
            Descricao = descricao;
            Instrutor = instrutor;
            Nivel = nivel;
            Valor = valor;
            DataCriacao = DateTime.UtcNow;
            Ativo = true;
        }

        public void AdicionarAula(Aula aula)
        {
            if (aula == null)
                throw new ArgumentNullException(nameof(aula));

            if (!Ativo)
                throw new InvalidOperationException("Não é possível adicionar aulas a um curso inativo");

            _aulas.Add(aula);
        }

        public bool VerificarSeAulaEstaCadastrada(Guid aulaId)
        {
            return _aulas.Any(a => a.Id == aulaId);
        }

        public void Inativar()
        {
            Ativo = false;
        }

        public void Ativar()
        {
            Ativo = true;
        }

        public void AtualizarNivel(NivelCurso novoNivel)
        {
            ValidarNivel(novoNivel);
            Nivel = novoNivel;
        }

        public void AtualizarInformacoes(string titulo, string descricao, string instrutor, NivelCurso nivel, decimal valor)
        {
            ValidarTitulo(titulo);
            ValidarDescricao(descricao);
            ValidarInstrutor(instrutor);
            ValidarNivel(nivel);
            ValidarValor(valor);

            Titulo = titulo;
            Descricao = descricao;
            Instrutor = instrutor;
            Nivel = nivel;
            Valor = valor;
        }

        public void AdicionarConteudoProgramatico(ConteudoProgramatico conteudo)
        {
            if (conteudo == null)
                throw new ArgumentNullException(nameof(conteudo), "Conteúdo programático não pode ser nulo");

            if (ConteudoProgramatico != null)
                throw new InvalidOperationException("Curso já possui conteúdo programático. Use o método de atualização");

            ConteudoProgramatico = conteudo;
        }

        public void AtualizarConteudoProgramatico(ConteudoProgramatico conteudo)
        {
            if (conteudo == null)
                throw new ArgumentNullException(nameof(conteudo), "Conteúdo programático não pode ser nulo");

            ConteudoProgramatico = conteudo;
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

        public void AtualizarInstrutor(string novoInstrutor)
        {
            ValidarInstrutor(novoInstrutor);
            Instrutor = novoInstrutor;
        }

        public void AtualizarValor(decimal novoValor)
        {
            ValidarValor(novoValor);
            Valor = novoValor;
        }

        public void RemoverAula(Guid aulaId)
        {
            var aula = _aulas.FirstOrDefault(a => a.Id == aulaId);
            if (aula == null)
                throw new InvalidOperationException($"Aula com ID {aulaId} não encontrada no curso");

            _aulas.Remove(aula);
        }

        public Aula ObterAulaPorId(Guid aulaId)
        {
            var aula = _aulas.FirstOrDefault(a => a.Id == aulaId);
            if (aula == null)
                throw new InvalidOperationException($"Aula com ID {aulaId} não encontrada no curso");

            return aula;
        }

        public int ObterTotalAulas()
        {
            return _aulas.Count;
        }

        public int ObterDuracaoTotalMinutos()
        {
            return _aulas.Sum(a => a.DuracaoMinutos);
        }

        // Métodos de validação privados
        private static void ValidarTitulo(string titulo)
        {
            if (string.IsNullOrWhiteSpace(titulo))
                throw new ArgumentException("Título do curso é obrigatório", nameof(titulo));

            if (titulo.Length < 3)
                throw new ArgumentException("Título do curso deve ter no mínimo 3 caracteres", nameof(titulo));

            if (titulo.Length > 200)
                throw new ArgumentException("Título do curso deve ter no máximo 200 caracteres", nameof(titulo));
        }

        private static void ValidarDescricao(string descricao)
        {
            if (string.IsNullOrWhiteSpace(descricao))
                throw new ArgumentException("Descrição do curso é obrigatória", nameof(descricao));

            if (descricao.Length < 10)
                throw new ArgumentException("Descrição do curso deve ter no mínimo 10 caracteres", nameof(descricao));

            if (descricao.Length > 1000)
                throw new ArgumentException("Descrição do curso deve ter no máximo 1000 caracteres", nameof(descricao));
        }

        private static void ValidarInstrutor(string instrutor)
        {
            if (string.IsNullOrWhiteSpace(instrutor))
                throw new ArgumentException("Instrutor do curso é obrigatório", nameof(instrutor));

            if (instrutor.Length < 3)
                throw new ArgumentException("Nome do instrutor deve ter no mínimo 3 caracteres", nameof(instrutor));

            if (instrutor.Length > 100)
                throw new ArgumentException("Nome do instrutor deve ter no máximo 100 caracteres", nameof(instrutor));
        }

        private static void ValidarNivel(NivelCurso nivel)
        {
            if (!Enum.IsDefined(typeof(NivelCurso), nivel))
                throw new ArgumentException("Nível do curso inválido", nameof(nivel));
        }

        private static void ValidarValor(decimal valor)
        {
            if (valor <= 0)
                throw new ArgumentException("Valor do curso deve ser maior que zero", nameof(valor));

            if (valor > 999999.99m)
                throw new ArgumentException("Valor do curso excede o limite máximo permitido", nameof(valor));
        }

    }
}
