using MediatR;
using PlataformaEducacional.Conteudo.Application.Commands.Cursos;
using PlataformaEducacional.Conteudo.Domain.Interfaces.Repositories;
using PlataformaEducacional.Conteudo.Domain.ValueObjects;
using PlataformaEducacional.Core.Notifications;

namespace PlataformaEducacional.Conteudo.Application.Handlers.Cursos
{
    public class AtualizarCursoCommandHandler : IRequestHandler<AtualizarCursoCommand, bool>
    {
        private readonly ICursoRepository _cursoRepository;
        private readonly INotificador _notificador;

        public AtualizarCursoCommandHandler(ICursoRepository cursoRepository, INotificador notificador)
        {
            _cursoRepository = cursoRepository;
            _notificador = notificador;
        }

        public async Task<bool> Handle(AtualizarCursoCommand request, CancellationToken cancellationToken)
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

            try
            {
                curso.AtualizarInformacoes(request.Titulo, request.Descricao, curso.Instrutor, request.Nivel, curso.Valor);

                // Atualizar ConteudoProgramatico se fornecido
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
                return await _cursoRepository.UnitOfWork.Commit();
            }
            catch (ArgumentException ex)
            {
                _notificador.Handle(new Notificacao
                {
                    Campo = "Curso",
                    Mensagem = ex.Message
                });
                return false;
            }
        }
    }
}
