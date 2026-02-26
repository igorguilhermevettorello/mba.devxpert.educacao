using System.ComponentModel;

namespace PlataformaEducacional.Conteudo.Domain.Enums
{
    public enum NivelCurso
    {
        [Description("Básico")]
        Basico = 1,
        [Description("Intermediário")]
        Intermediario = 2,
        [Description("Avançado")]
        Avancado = 3
    }
}
