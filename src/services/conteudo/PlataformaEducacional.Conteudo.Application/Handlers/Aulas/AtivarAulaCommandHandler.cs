using MediatR;
using PlataformaEducacional.Conteudo.Application.Commands.Aulas;
using PlataformaEducacional.Conteudo.Domain.Interfaces.Repositories;
using PlataformaEducacional.Core.Notifications;

namespace PlataformaEducacional.Conteudo.Application.Handlers.Aulas
{
    public class AtivarAulaCommandHandler : IRequestHandler<AtivarAulaCommand, bool>
    {
        private readonly IAulaRepository _aulaRepository;
        private readonly INotificador _notificador;

        public AtivarAulaCommandHandler(
            IAulaRepository aulaRepository,
            INotificador notificador)
        {
            _aulaRepository = aulaRepository;
            _notificador = notificador;
        }

        public async Task<bool> Handle(AtivarAulaCommand request, CancellationToken cancellationToken)
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

            var aula = await _aulaRepository.BuscarPorIdAsync(request.AulaId);

            if (aula == null)
            {
                _notificador.Handle(new Notificacao
                {
                    Campo = "AulaId",
                    Mensagem = "Aula não encontrada"
                });
                return false;
            }

            aula.Ativar();
            _aulaRepository.Alterar(aula);
            return await _aulaRepository.UnitOfWork.Commit();
        }
    }
}
