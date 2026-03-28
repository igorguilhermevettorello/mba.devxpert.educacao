using FluentValidation.Results;
using MediatR;
using PlataformaEducacional.Conteudo.Application.Commands.Cursos;
using PlataformaEducacional.Conteudo.Domain.Interfaces.Repositories;
using PlataformaEducacional.Conteudo.Domain.ValueObjects;
using PlataformaEducacional.Core.Messages;
using PlataformaEducacional.Core.Notifications;

namespace PlataformaEducacional.Conteudo.Application.Handlers.Cursos
{
    public class AtualizarCursoCommandHandler : CommandHandler, IRequestHandler<AtualizarCursoCommand, ValidationResult>
    {
        private readonly ICursoRepository _cursoRepository;
        private readonly INotificador _notificador;

        public AtualizarCursoCommandHandler(ICursoRepository cursoRepository, INotificador notificador)
        {
            _cursoRepository = cursoRepository;
            _notificador = notificador;
        }

        public async Task<ValidationResult> Handle(AtualizarCursoCommand request, CancellationToken cancellationToken)
        {
            if (!request.IsValid())
            {
                foreach (var error in request.ValidationResult.Errors)
                {
                    _notificador.Handle(new Notificacao
                    {
                        Campo = error.PropertyName,
                        Mensagem = error.ErrorMessage
                    });
                }
                return request.ValidationResult;
            }

            var curso = await _cursoRepository.BuscarPorIdAsync(request.Id);

            if (curso == null)
            {
                _notificador.Handle(new Notificacao
                {
                    Campo = "Id",
                    Mensagem = "Curso não encontrado"
                });
                return new ValidationResult(new[] { new ValidationFailure("Id", "Curso não encontrado") });
            }

            try
            {
                curso.AtualizarInformacoes(request.Titulo, request.Descricao, curso.Instrutor, request.Nivel, curso.Valor);

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

                _cursoRepository.Alterar(curso);
                return await PersistData(_cursoRepository.UnitOfWork);
            }
            catch (ArgumentException ex)
            {
                _notificador.Handle(new Notificacao
                {
                    Campo = "Curso",
                    Mensagem = ex.Message
                });
                return new ValidationResult(new[] { new ValidationFailure("Curso", ex.Message) });
            }
        }
    }
}
