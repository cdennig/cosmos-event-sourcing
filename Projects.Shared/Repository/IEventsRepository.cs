using System.Threading.Tasks;
using Projects.Shared.Aggregate;

namespace Projects.Shared.Repository
{
    public interface IEventsRepository<TA, TKey>
        where TA : class, IAggregateRoot<TKey>
    {
        Task AppendAsync(TA aggregateRoot);
        Task<TA> RehydrateAsync(TKey key);
    }
}