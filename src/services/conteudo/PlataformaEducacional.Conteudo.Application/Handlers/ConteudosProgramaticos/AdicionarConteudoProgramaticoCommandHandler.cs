using MediatR;
using PlataformaEducacional.Conteudo.Application.Commands.ConteudosProgramaticos;
using PlataformaEducacional.Conteudo.Domain.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlataformaEducacional.Conteudo.Application.Handlers.ConteudosProgramaticos
{
    public class AdicionarConteudoProgramaticoCommandHandler : IRequestHandler<AdicionarConteudoProgramaticoCommand, bool>
    {
        private readonly ICursoRepository _cursoRepository;
        private readonly IMediator _mediator;

        public AdicionarConteudoProgramaticoCommandHandler(ICursoRepository cursoRepository, IMediator mediator)
        {
            _cursoRepository = cursoRepository;
            _mediator = mediator;
        }

        public async Task<bool> Handle(AdicionarConteudoProgramaticoCommand request, CancellationToken cancellationToken)
        {
            var curso = await _cursoRepository.BuscarPorIdAsync(request.CursoId);

            // var curso = new Curso(request.Titulo, request.Descricao, NivelCurso.Avancado);
            //var curso = new Domain.Entities.Curso(request.Titulo, request.Descricao, NivelCurso.Avancado);

            // _cursoRepository.Adicionar(curso);
            //
            // await _mediator.Publish(new CriarCursoEvent(curso.Id, request.Titulo, request.Descricao), cancellationToken);

            return true;
        }
    }
}
