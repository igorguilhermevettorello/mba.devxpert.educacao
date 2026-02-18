using FluentValidation;
using PlataformaEducacional.Core.Messages;

namespace PlataformaEducacional.Alunos.Api.Application.Commands;

public class AdicionarEnderecoCommand : Command
{
    public AdicionarEnderecoCommand(string logradouro, string numero, string complemento, string bairro, string cep, string cidade, string estado)
    {
        Logradouro = logradouro;
        Numero = numero;
        Complemento = complemento;
        Bairro = bairro;
        Cep = cep;
        Cidade = cidade;
        Estado = estado;
    }

    public string Logradouro { get;  set; }
    public string Numero { get;  set; }
    public string Complemento { get;  set; }
    public string Bairro { get;  set; }
    public string Cep { get;  set; }
    public string Cidade { get;  set; }
    public string Estado { get;  set; }
    public Guid AlunoId { get;  set; }

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
        RuleFor(c => c.Logradouro)
            .NotEmpty()
            .WithMessage("Logradouro é obrigatório");

        RuleFor(c => c.Numero)
            .NotEmpty()
            .WithMessage("Numero é obrigatório");
        RuleFor(c => c.Cep)
            .NotEmpty()
            .WithMessage("Cep é obrigatório");

        RuleFor(c => c.Bairro)
            .NotEmpty()
            .WithMessage("Bairro é obrigatório");
        RuleFor(c => c.Cidade)
            .NotEmpty()
            .WithMessage("Cidade é obrigatório");

        RuleFor(c => c.Estado)
            .NotEmpty()
            .WithMessage("Estado é obrigatório");
    }
}
