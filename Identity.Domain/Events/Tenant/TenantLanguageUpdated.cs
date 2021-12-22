using ES.Shared.Attributes;
using ES.Shared.Events;
using Newtonsoft.Json;

namespace Identity.Domain.Events.Tenant;

[Event(nameof(TenantLanguageUpdated), 1.0)]
public class TenantLanguageUpdated : DomainEvent<Domain.Tenant, Guid, Guid>
{
    private TenantLanguageUpdated(string aggregateType, Guid raisedBy, Guid aggregateId, long version,
        DateTimeOffset timestamp) : base(
        aggregateType, raisedBy, aggregateId, version, timestamp)
    {
    }

    public TenantLanguageUpdated(Domain.Tenant tenant, Guid raisedBy, string language) : base(tenant, raisedBy)
    {
        OldLanguage = tenant.Language;
        Language = language;
    }

    [JsonProperty] public string Language { get; private set; }
    [JsonProperty] public string OldLanguage { get; private set; }
}