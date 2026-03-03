using FluentValidation.Results;
using MediatR;
using PlataformaEducacional.Conteudo.Application.Commands.Cursos;
using PlataformaEducacional.Conteudo.Domain.Entities;
using PlataformaEducacional.Conteudo.Domain.Interfaces.Repositories;
using PlataformaEducacional.Conteudo.Domain.ValueObjects;
using PlataformaEducacional.Core.Messages;

namespace PlataformaEducacional.Conteudo.Application.Handlers.Cursos
{
    public class CriarCursoCommandHandler : CommandHandler, IRequestHandler<CriarCursoCommand, ValidationResult>
    {
        private readonly ICursoRepository _cursoRepository;

        public CriarCursoCommandHandler(ICursoRepository cursoRepository)
        {
            _cursoRepository = cursoRepository;
        }

        public async Task<ValidationResult> Handle(CriarCursoCommand request, CancellationToken cancellationToken)
        {
            if (!request.IsValid())
                return request.ValidationResult;

            var curso = new Curso(request.Titulo, request.Descricao, request.Instrutor, request.Nivel, request.Valor);

            if (request.ConteudoProgramatico != null)
            {
                var conteudoProgramatico = new ConteudoProgramatico(
                    request.ConteudoProgramatico.Ementa,
                    request.ConteudoProgramatico.Objetivo,
                    request.ConteudoProgramatico.Bibliografia,
                    request.ConteudoProgramatico.MaterialUrl
                );

                curso.AdicionarConteudoProgramatico(conteudoProgramatico);
            }

            request.SetAggregateId(curso.Id);
            _cursoRepository.Adicionar(curso);

            return await PersistData(_cursoRepository.UnitOfWork);
        }
    }
}
