using FluentValidation;
using PlataformaEducacional.Conteudo.Application.Commands.Cursos;

namespace PlataformaEducacional.Conteudo.Application.Validators
{
    public class CriarCursoCommandValidator : AbstractValidator<CriarCursoCommand>
    {
        public CriarCursoCommandValidator()
        {
            RuleFor(x => x.Titulo)
                .NotEmpty()
                .WithMessage("Título do curso é obrigatório")
                .MaximumLength(200)
                .WithMessage("Título do curso deve ter no máximo 200 caracteres")
                .MinimumLength(3)
                .WithMessage("Título do curso deve ter no mínimo 3 caracteres");

            RuleFor(x => x.Descricao)
                .NotEmpty()
                .WithMessage("Descrição do curso é obrigatória")
                .MaximumLength(1000)
                .WithMessage("Descrição do curso deve ter no máximo 1000 caracteres")
                .MinimumLength(10)
                .WithMessage("Descrição do curso deve ter no mínimo 10 caracteres");

            RuleFor(x => x.Instrutor)
                .NotEmpty()
                .WithMessage("Instrutor do curso é obrigatória")
                .MaximumLength(200)
                .WithMessage("Instrutor do curso deve ter no máximo 200 caracteres")
                .MinimumLength(3)
                .WithMessage("Instrutor do curso deve ter no mínimo 3 caracteres");

            RuleFor(x => x.Nivel)
                .IsInEnum()
                .WithMessage("Nível do curso deve ser válido");

            RuleFor(x => x.Valor)
                .GreaterThan(0)
                .WithMessage("Valor do curso deve ser maior que zero");

            // Validação opcional do ConteudoProgramatico
            When(x => x.ConteudoProgramatico != null, () =>
            {
                RuleFor(x => x.ConteudoProgramatico.Ementa)
                    .NotEmpty()
                    .WithMessage("Ementa do conteúdo programático é obrigatória")
                    .MaximumLength(2000)
                    .WithMessage("Ementa deve ter no máximo 2000 caracteres")
                    .MinimumLength(10)
                    .WithMessage("Ementa deve ter no mínimo 10 caracteres");

                RuleFor(x => x.ConteudoProgramatico.Objetivo)
                    .NotEmpty()
                    .WithMessage("Objetivo do conteúdo programático é obrigatório")
                    .MaximumLength(1000)
                    .WithMessage("Objetivo deve ter no máximo 1000 caracteres")
                    .MinimumLength(10)
                    .WithMessage("Objetivo deve ter no mínimo 10 caracteres");

                RuleFor(x => x.ConteudoProgramatico.Bibliografia)
                    .NotEmpty()
                    .WithMessage("Bibliografia do conteúdo programático é obrigatória")
                    .MaximumLength(2000)
                    .WithMessage("Bibliografia deve ter no máximo 2000 caracteres")
                    .MinimumLength(10)
                    .WithMessage("Bibliografia deve ter no mínimo 10 caracteres");

                RuleFor(x => x.ConteudoProgramatico.MaterialUrl)
                    .NotEmpty()
                    .WithMessage("URL do material do conteúdo programático é obrigatória")
                    .MaximumLength(500)
                    .WithMessage("URL do material deve ter no máximo 500 caracteres")
                    .Must(url => Uri.TryCreate(url, UriKind.Absolute, out _))
                    .WithMessage("URL do material não é válida");
            });
        }
    }
}
