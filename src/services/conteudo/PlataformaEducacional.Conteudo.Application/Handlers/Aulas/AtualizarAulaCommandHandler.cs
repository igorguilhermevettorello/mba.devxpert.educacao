using FluentValidation.Results;
using MediatR;
using PlataformaEducacional.Conteudo.Application.Commands.Aulas;
using PlataformaEducacional.Conteudo.Domain.Interfaces.Repositories;
using PlataformaEducacional.Core.Messages;

namespace PlataformaEducacional.Conteudo.Application.Handlers.Aulas
{
    public class AtualizarAulaCommandHandler : CommandHandler, IRequestHandler<AtualizarAulaCommand, ValidationResult>
    {
        private readonly IAulaRepository _aulaRepository;

        public AtualizarAulaCommandHandler(IAulaRepository aulaRepository)
        {
            _aulaRepository = aulaRepository;
        }

        public async Task<ValidationResult> Handle(AtualizarAulaCommand request, CancellationToken cancellationToken)
        {
            if (!request.IsValid())
                return request.ValidationResult;

            var aula = await _aulaRepository.BuscarPorIdAsync(request.Id);

            if (aula == null)
            {
                AddError("Aula não encontrada");
                return ValidationResult;
            }

            try
            {
                aula.AtualizarTitulo(request.Titulo);
                aula.AtualizarDescricao(request.Descricao);
                aula.AtualizarDuracao(request.DuracaoMinutos);
                aula.AtualizarOrdem(request.Ordem);

                _aulaRepository.Alterar(aula);
                return await PersistData(_aulaRepository.UnitOfWork);
            }
            catch (ArgumentException ex)
            {
                AddError(ex.Message);
                return ValidationResult;
            }
        }
    }
}
