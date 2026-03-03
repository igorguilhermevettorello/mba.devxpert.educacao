using FluentValidation;
using PlataformaEducacional.Conteudo.Application.Commands.Cursos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlataformaEducacional.Conteudo.Application.Validators
{
    public class DeletarCursoCommandValidator : AbstractValidator<DeletarCursoCommand>
    {
        public DeletarCursoCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage("Id do curso é obrigatório.");
        }
    }
}
