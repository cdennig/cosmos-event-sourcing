using Newtonsoft.Json.Linq;

namespace ES.Shared.Events;

public interface IDomainEventsFactory<out TKey, out TPrincipalKey>
{
    void Initialize(IEnumerable<Type> types);

    IDomainEvent<TKey, TPrincipalKey> BuildEvent(string eventType, double eventVersion,
        string raisedBy, string aggregateId, string aggregateType,
        string timestamp, long version, JObject rawEvent);
}