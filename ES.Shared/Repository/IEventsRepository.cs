using System.Threading;
using System.Threading.Tasks;
using ES.Shared.Aggregate;

namespace ES.Shared.Repository
{
    public interface IEventsRepository<TTenantId, TA, TKey>
        where TA : class, IAggregateRoot<TTenantId, TKey>
    {
        Task AppendAsync(TA aggregateRoot, CancellationToken cancellationToken);
        Task<TA> RehydrateAsync(TTenantId tenantId, TKey id, CancellationToken cancellationToken);
    }
}