using PlataformaEducacional.Core.DomainObjects;

namespace PlataformaEducacional.Core.Data
{
    public interface IRepository<T> : IDisposable where T : IAggregateRoot
    {
        IUnitOfWork UnitOfWork { get; }
    }
}
