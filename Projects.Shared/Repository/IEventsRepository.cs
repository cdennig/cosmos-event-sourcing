using System.Threading.Tasks;
using Projects.Shared.Aggregate;

namespace Projects.Shared.Repository
{
    public interface IEventsRepository<TTenantId, TA, TKey>
        where TA : class, IAggregateRoot<TTenantId, TKey>
    {
        Task AppendAsync(TA aggregateRoot);
        Task<TA> RehydrateAsync(TTenantId tenantId, TKey key);
    }
}