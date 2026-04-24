using PlataformaEducacional.Conteudo.Domain.ValueObjects;
using PlataformaEducacional.Core.DomainObjects;
using PlataformaEducacional.WebApi.Core.Enumerators;

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


            Titulo = titulo;
            Descricao = descricao;
            Instrutor = instrutor;
            Nivel = nivel;
            Valor = valor;
            DataCriacao = DateTime.UtcNow;
            Ativo = true;
        }

        //construtor usado p/ o seed
        public Curso(Guid id, string titulo, string descricao, string instrutor, NivelCurso nivel, decimal valor) : this()
        {
            Id = id;
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

            Nivel = novoNivel;
        }

        public void AtualizarInformacoes(string titulo, string descricao, string instrutor, NivelCurso nivel, decimal valor)
        {


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

            //if (ConteudoProgramatico != null) 
            //{
            //    throw new InvalidOperationException("Curso já possui conteúdo programático. Use o método de atualização");
            //}

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

            Titulo = novoTitulo;
        }

        public void AtualizarDescricao(string novaDescricao)
        {

            Descricao = novaDescricao;
        }

        public void AtualizarInstrutor(string novoInstrutor)
        {

            Instrutor = novoInstrutor;
        }

        public void AtualizarValor(decimal novoValor)
        {

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



    }
}
