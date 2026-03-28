using FluentValidation.Results;
using MediatR;
using PlataformaEducacional.Conteudo.Application.Commands.Cursos;
using PlataformaEducacional.Conteudo.Domain.Interfaces.Repositories;
using PlataformaEducacional.Core.Messages;
using PlataformaEducacional.Core.Notifications;

namespace PlataformaEducacional.Conteudo.Application.Handlers.Cursos
{
    public class DeletarCursoCommandHandler : CommandHandler, IRequestHandler<DeletarCursoCommand, ValidationResult>
    {
        private readonly ICursoRepository _cursoRepository;
        private readonly INotificador _notificador;

        public DeletarCursoCommandHandler(ICursoRepository cursoRepository, INotificador notificador)
        {
            _cursoRepository = cursoRepository;
            _notificador = notificador;
        }

        public async Task<ValidationResult> Handle(DeletarCursoCommand request, CancellationToken cancellationToken)
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
                    Campo = nameof(DeletarCursoCommand.Id),
                    Mensagem = "Curso não encontrado"
                });
                return new ValidationResult([
                    new ValidationFailure(nameof(DeletarCursoCommand.Id), "Curso não encontrado")
                ]);
            }

            _cursoRepository.Remover(curso);
            return await PersistData(_cursoRepository.UnitOfWork);
        }
    }
}
