using FluentValidation;
using PlataformaEducacional.Core.Messages;

namespace PlataformaEducacional.Alunos.Application.Commands;

public class RealizarMatriculaCommand : Command
{
    public Guid AlunoId { get; private set; }
    public Guid CursoId { get; private set; }
    public decimal ValorCurso { get; set; }

    // Cartao
    public string NumeroCartao { get; set; }
    public string TitularCartao { get; set; }
    public string ValidadeCartao { get; set; }
    public string CodigoSegurancaCartao { get; set; }


    public RealizarMatriculaCommand(Guid alunoId, Guid cursoId, decimal valorCurso, string numeroCartao, string titular, string validade, string codigoSeguranca)
    {
        AlunoId = alunoId;
        CursoId = cursoId;
        ValorCurso = valorCurso;
        NumeroCartao = numeroCartao;
        TitularCartao = titular;
        ValidadeCartao = validade;
        CodigoSegurancaCartao = codigoSeguranca;
    }

    public override bool IsValid()
    {
        ValidationResult = new RealizarMatriculaValidation().Validate(this);
        return ValidationResult.IsValid;
    }
}

public class RealizarMatriculaValidation : AbstractValidator<RealizarMatriculaCommand>
{
    public RealizarMatriculaValidation()
    {
        RuleFor(c => c.AlunoId)
            .NotEqual(Guid.Empty)
            .WithMessage("Id do aluno inválido");

        RuleFor(c => c.CursoId)
            .NotEqual(Guid.Empty)
            .WithMessage("Id do curso inválido");

        RuleFor(c => c.NumeroCartao)
                .CreditCard()
                .WithMessage("Cartão de crédito inválido");

        RuleFor(c => c.TitularCartao)
            .NotNull()
            .WithMessage("O titular é obrigatório");

        RuleFor(c => c.CodigoSegurancaCartao.Length)
            .GreaterThan(2)
            .LessThan(5)
            .WithMessage("O código de segurança deve possuir 3 or 4 números");

        RuleFor(c => c.ValidadeCartao)
            .NotNull()
            .WithMessage("A Validade do cartão é obrigatória");
    }
}
