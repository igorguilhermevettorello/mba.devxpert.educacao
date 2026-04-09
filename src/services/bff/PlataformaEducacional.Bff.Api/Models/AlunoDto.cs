using PlataformaEducacional.Core.DomainObjects;

namespace PlataformaEducacional.Bff.Api.Models;

public class AlunoDto
{
    public Guid Id { get; set; }
    public string Nome { get; private set; } = string.Empty;
    public Email Email { get; private set; } = null!;
    public Cpf Cpf { get; private set; } = null!;
    public bool Excluido { get; private set; }
}
