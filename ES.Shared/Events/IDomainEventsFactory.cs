using Newtonsoft.Json.Linq;

namespace ES.Shared.Events;

public interface IDomainEventsFactory<out TTenantKey, out TKey, out TPrincipalKey>
{
    void Initialize();

    IDomainEvent<TTenantKey, TKey, TPrincipalKey> BuildEvent(string eventType, double eventVersion,
        string tenantId,
        string raisedBy, string aggregateId, string aggregateType,
        string timestamp, long version, JObject rawEvent);
}