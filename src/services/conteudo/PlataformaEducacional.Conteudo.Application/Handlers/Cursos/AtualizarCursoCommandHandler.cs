using FluentValidation.Results;
using MediatR;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<AtualizarCursoCommandHandler> _logger;

        public AtualizarCursoCommandHandler(ICursoRepository cursoRepository, INotificador notificador, ILogger<AtualizarCursoCommandHandler> logger)
        {
            _cursoRepository = cursoRepository;
            _notificador = notificador;
            _logger = logger;
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
                _logger.LogWarning("Curso não encontrado - CursoId {CursoId}", request.Id);
                _notificador.Handle(new Notificacao
                {
                    Campo = "Id",
                    Mensagem = "Curso não encontrado"
                });
                return new ValidationResult(new[] { new ValidationFailure("Id", "Curso não encontrado") });
            }

            try
            {
                curso.AtualizarInformacoes(request.Titulo, request.Descricao, request.Instrutor, request.Nivel, request.Valor);

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
                var result = await PersistData(_cursoRepository.UnitOfWork);
                if (result.IsValid)
                    _logger.LogInformation("Curso atualizado com sucesso - CursoId {CursoId}, Titulo {Titulo}", request.Id, request.Titulo);
                return result;
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Erro ao atualizar curso - CursoId {CursoId}", request.Id);
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
