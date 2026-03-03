using MediatR;
using PlataformaEducacional.Conteudo.Application.Commands.Aulas;
using PlataformaEducacional.Conteudo.Domain.Entities;
using PlataformaEducacional.Conteudo.Domain.Interfaces.Repositories;
using PlataformaEducacional.Core.Notifications;

namespace PlataformaEducacional.Conteudo.Application.Handlers.Aulas
{
    public class CriarAulaCommandHandler : IRequestHandler<CriarAulaCommand, bool>
    {
        private readonly IAulaRepository _aulaRepository;
        private readonly ICursoRepository _cursoRepository;
        private readonly INotificador _notificador;

        public CriarAulaCommandHandler(
            IAulaRepository aulaRepository,
            ICursoRepository cursoRepository,
            INotificador notificador)
        {
            _aulaRepository = aulaRepository;
            _cursoRepository = cursoRepository;
            _notificador = notificador;
        }

        public async Task<bool> Handle(CriarAulaCommand request, CancellationToken cancellationToken)
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

            // Validar se o curso existe e está ativo
            var curso = await _cursoRepository.BuscarPorIdAsync(request.CursoId);

            if (curso == null)
            {
                _notificador.Handle(new Notificacao
                {
                    Campo = "CursoId",
                    Mensagem = "Curso não encontrado"
                });
                return false;
            }

            if (!curso.Ativo)
            {
                _notificador.Handle(new Notificacao
                {
                    Campo = "CursoId",
                    Mensagem = "Não é possível adicionar aulas a um curso inativo"
                });
                return false;
            }

            try
            {
                var aula = new Aula(
                    request.Titulo,
                    request.Descricao,
                    request.DuracaoMinutos,
                    request.Ordem);

                // Associar a aula ao curso
                aula.AssociarCurso(request.CursoId);
                curso.AdicionarAula(aula);

                request.SetAggregateId(aula.Id);
                _aulaRepository.Adicionar(aula);

                return await _aulaRepository.UnitOfWork.Commit();
            }
            catch (ArgumentException ex)
            {
                _notificador.Handle(new Notificacao
                {
                    Campo = "Aula",
                    Mensagem = ex.Message
                });
                return false;
            }
        }
    }
}
