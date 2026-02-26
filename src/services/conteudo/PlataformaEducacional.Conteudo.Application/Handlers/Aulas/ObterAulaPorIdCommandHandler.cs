using MediatR;
using PlataformaEducacional.Conteudo.Application.Commands.Aulas;
using PlataformaEducacional.Conteudo.Domain.Entities;
using PlataformaEducacional.Conteudo.Domain.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlataformaEducacional.Conteudo.Application.Handlers.Aulas
{
    public class ObterAulaPorIdCommandHandler : IRequestHandler<ObterAulaPorIdCommand, Aula?>
    {
        private readonly IAulaRepository _aulaRepository;

        public ObterAulaPorIdCommandHandler(IAulaRepository aulaRepository)
        {
            _aulaRepository = aulaRepository;
        }

        public async Task<Aula?> Handle(ObterAulaPorIdCommand request, CancellationToken cancellationToken)
        {
            return await _aulaRepository.BuscarPorIdAsync(request.Id);
        }
    }
}
