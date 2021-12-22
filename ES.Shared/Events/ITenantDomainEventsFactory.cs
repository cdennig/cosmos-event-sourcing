using Newtonsoft.Json.Linq;

namespace ES.Shared.Events;

public interface ITenantDomainEventsFactory<out TTenantKey, out TKey, out TPrincipalKey>
{
    void Initialize(IEnumerable<Type> types);

    ITenantDomainEvent<TTenantKey, TKey, TPrincipalKey> BuildEvent(string eventType, double eventVersion,
        string tenantId,
        string raisedBy, string aggregateId, string aggregateType,
        string timestamp, long version, JObject rawEvent);
}