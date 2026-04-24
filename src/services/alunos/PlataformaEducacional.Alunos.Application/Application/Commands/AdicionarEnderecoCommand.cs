using FluentValidation;
using PlataformaEducacional.Core.Messages;

namespace PlataformaEducacional.Alunos.Application.Commands;

public class AdicionarEnderecoCommand : Command
{
    public AdicionarEnderecoCommand(Guid alunoId, string logradouro, string numero, string complemento, string bairro, string cep, string cidade, string estado)
    {
        AlunoId = alunoId;
        Logradouro = logradouro;
        Numero = numero;
        Complemento = complemento;
        Bairro = bairro;
        Cep = cep;
        Cidade = cidade;
        Estado = estado;
    }

    public string Logradouro { get; set; }
    public string Numero { get; set; }
    public string Complemento { get; set; }
    public string Bairro { get; set; }
    public string Cep { get; set; }
    public string Cidade { get; set; }
    public string Estado { get; set; }
    public Guid AlunoId { get; set; }

    public override bool IsValid()
    {
        ValidationResult = new EnderecoValidation().Validate(this);
        return ValidationResult.IsValid;
    }
}

public class EnderecoValidation : AbstractValidator<AdicionarEnderecoCommand>
{
    public EnderecoValidation()
    {
        RuleFor(c => c.AlunoId)
            .NotEqual(Guid.Empty)
            .WithMessage("Id do aluno inválido");

        RuleFor(c => c.Logradouro)
            .NotEmpty()
            .WithMessage("Logradouro é obrigatório")
            .MaximumLength(200)
            .WithMessage("Logradouro deve ter no máximo 200 caracteres");

        RuleFor(c => c.Numero)
            .NotEmpty()
            .WithMessage("Numero é obrigatório")
            .MaximumLength(20)
            .WithMessage("Numero deve ter no máximo 20 caracteres");

        RuleFor(c => c.Cep)
            .NotEmpty()
            .WithMessage("Cep é obrigatório")
            .Length(8)
            .WithMessage("Cep deve ter 8 caracteres");

        RuleFor(c => c.Bairro)
            .NotEmpty()
            .WithMessage("Bairro é obrigatório")
            .MaximumLength(100)
            .WithMessage("Bairro deve ter no máximo 100 caracteres");

        RuleFor(c => c.Cidade)
            .NotEmpty()
            .WithMessage("Cidade é obrigatório")
            .MaximumLength(100)
            .WithMessage("Cidade deve ter no máximo 100 caracteres");

        RuleFor(c => c.Estado)
            .NotEmpty()
            .WithMessage("Estado é obrigatório")
            .Length(2)
            .WithMessage("Estado deve ter 2 caracteres");
    }
}
