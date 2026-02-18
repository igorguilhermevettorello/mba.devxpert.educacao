using System.ComponentModel;

namespace PlataformaEducacional.Core.Enumerators;

/// <summary>
/// Enumerator para Tipo de Usuario
/// </summary>
public enum TipoUsuario
{
    [Description("Administrador")]
    Administrador,

    [Description("Aluno")]
    Aluno
}
