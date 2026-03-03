using PlataformaEducacional.Core.Messages;

namespace PlataformaEducacional.Conteudo.Application.Events.Cursos
{
    public class CriarCursoEvent : Event
    {
        public Guid Id { get; private set; }
        public string Titulo { get; private set; }
        public string Descricao { get; private set; }
        public CriarCursoEvent(Guid id, string titulo, string descricao)
        {
            Id = id;
            Titulo = titulo;
            Descricao = descricao;
        }
    }
}
