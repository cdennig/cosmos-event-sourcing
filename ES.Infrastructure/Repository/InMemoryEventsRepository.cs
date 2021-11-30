using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ES.Infrastructure.Data;
using ES.Shared.Aggregate;
using ES.Shared.Events;
using ES.Shared.Repository;

namespace ES.Infrastructure.Repository
{
    public class InMemoryEventsRepository<TTenantId, TA, TKey> : IEventsRepository<TTenantId, TA, TKey>
        where TA : class, IAggregateRoot<TTenantId, TKey>
    {
        private Dictionary<string, List<EventData<TTenantId, TKey>>> _events = new();

        public Task AppendAsync(TA aggregateRoot, CancellationToken cancellationToken = default)
        {
            var domainEvents = aggregateRoot.DomainEvents;
            if (domainEvents.Count == 0) return Task.CompletedTask;

            var key = $"{aggregateRoot.TenantId}:{aggregateRoot.Id}";
            List<EventData<TTenantId, TKey>> currentEvents;

            if (_events.ContainsKey(key))
                currentEvents = _events[key];
            else
            {
                currentEvents = new List<EventData<TTenantId, TKey>>();
                _events.Add(key, currentEvents);
            }

            foreach (var domainEvent in domainEvents)
            {
                var ed = new EventData<TTenantId, TKey>(@domainEvent);
                currentEvents.Add(ed);
            }

            return Task.CompletedTask;
        }

        public Task<TA> RehydrateAsync(TTenantId tenantId, TKey id, CancellationToken cancellationToken = default)
        {
            var key = $"{tenantId}:{id}";
            List<EventData<TTenantId, TKey>> currentEvents;
            if (_events.ContainsKey(key))
                currentEvents = _events[key];
            else
            {
                throw new ArgumentException("No events for aggregate.");
            }

            var events = currentEvents.Select(eventData => eventData.Event).ToList();
            var aggregateRoot = BaseAggregateRoot<TTenantId, TA, TKey>.Create(tenantId, id, events);
            return Task.FromResult(aggregateRoot);
        }
    }
}