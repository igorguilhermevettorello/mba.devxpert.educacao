using MediatR;
using PlataformaEducacional.Conteudo.Application.Commands.Cursos;
using PlataformaEducacional.Conteudo.Domain.Interfaces.Repositories;
using PlataformaEducacional.Core.Notifications;

namespace PlataformaEducacional.Conteudo.Application.Handlers.Cursos
{
    public class DeletarCursoCommandHandler : IRequestHandler<DeletarCursoCommand, bool>
    {
        private readonly ICursoRepository _cursoRepository;
        private readonly INotificador _notificador;

        public DeletarCursoCommandHandler(ICursoRepository cursoRepository, INotificador notificador)
        {
            _cursoRepository = cursoRepository;
            _notificador = notificador;
        }

        public async Task<bool> Handle(DeletarCursoCommand request, CancellationToken cancellationToken)
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
                return false;
            }

            var curso = await _cursoRepository.BuscarPorIdAsync(request.Id);

            if (curso == null)
            {
                _notificador.Handle(new Notificacao
                {
                    Campo = "Id",
                    Mensagem = "Curso não encontrado"
                });
                return false;
            }

            _cursoRepository.Remover(curso);
            return await _cursoRepository.UnitOfWork.Commit();
        }
    }
}
