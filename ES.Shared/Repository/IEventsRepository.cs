using System.Threading;
using System.Threading.Tasks;
using ES.Shared.Aggregate;

namespace ES.Shared.Repository
{
    public interface IEventsRepository<TTenantId, TA, TKey, TPrincipalId>
        where TA : class, IAggregateRoot<TTenantId, TKey, TPrincipalId>
    {
        Task AppendAsync(TA aggregateRoot, CancellationToken cancellationToken);
        Task<TA> RehydrateAsync(TTenantId tenantId, TKey id, CancellationToken cancellationToken = default);
    }
}