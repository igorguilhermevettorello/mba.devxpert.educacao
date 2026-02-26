using MediatR;
using PlataformaEducacional.Conteudo.Data.Context;
using PlataformaEducacional.Core.DomainObjects;

namespace PlataformaEducacional.Conteudo.Data.Extensions
{
    public static class MediatorExtension
    {
        public static async Task PublicarEventos(this IMediator mediator, CursoContext ctx)
        {
            //var domainEntities = ctx.ChangeTracker
            //    .Entries<Entity>()
            //    .Where(x => x.Entity.Notificacoes != null && x.Entity.Notificacoes.Any());

            //var domainEvents = domainEntities
            //    .SelectMany(x => x.Entity.Notificacoes)
            //    .ToList();

            //domainEntities.ToList()
            //    .ForEach(entity => entity.Entity.LimparEventos());

            //var tasks = domainEvents
            //    .Select(async (domainEvent) => {
            //        await mediator.Publish(domainEvent);
            //    });

            //await Task.WhenAll(tasks);

            var domainEntities = ctx.ChangeTracker
            .Entries<Entity>()
            .Where(x => x.Entity.Notificacoes != null && x.Entity.Notificacoes.Any());

            var domainEvents = domainEntities
                .SelectMany(x => x.Entity.Notificacoes)
                .ToList();

            domainEntities.ToList()
                .ForEach(entity => entity.Entity.ClearEvents());

            var tasks = domainEvents
                .Select(async (domainEvent) => {
                    await mediator.Publish(domainEvent);
                });

            await Task.WhenAll(tasks);
        }
    }
}
