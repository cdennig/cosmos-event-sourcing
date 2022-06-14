using System.Reflection;
using ES.Shared.Events;
using Microsoft.Extensions.Logging;

namespace ES.Shared.Aggregate;

public class
    TenantAggregateRootFactory<TTenantKey, TAggregate, TKey, TPrincipalKey> : ITenantAggregateRootFactory<TTenantKey,
        TAggregate, TKey, TPrincipalKey>
    where TAggregate : class, ITenantAggregateRoot<TTenantKey, TKey, TPrincipalKey>
{
    private readonly ConstructorInfo _constructorInfo;
    private readonly ILogger<TenantAggregateRootFactory<TTenantKey, TAggregate, TKey, TPrincipalKey>> _logger;

    public TenantAggregateRootFactory(
        ILogger<TenantAggregateRootFactory<TTenantKey, TAggregate, TKey, TPrincipalKey>> logger)
    {
        _logger = logger;
        var aggregateType = typeof(TAggregate);
        _constructorInfo = aggregateType.GetConstructor(
                               BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public,
                               null,
                               new[]
                               {
                                   typeof(TTenantKey), typeof(TKey),
                                   typeof(IEnumerable<ITenantDomainEvent<TTenantKey, TKey, TPrincipalKey>>)
                               }, Array.Empty<ParameterModifier>()) ??
                           throw new InvalidOperationException();
        if (null == _constructorInfo)
        {
            _logger.LogError("Unable to find required private constructor for Aggregate of type {AggregateRootType}",
                aggregateType.Name);
            throw new InvalidOperationException(
                $"Unable to find required private constructor for Aggregate of type '{aggregateType.Name}'");
        }
    }

    public TAggregate Create(TTenantKey tenantId,
        TKey id,
        IEnumerable<ITenantDomainEvent<TTenantKey, TKey, TPrincipalKey>> events)
    {
        if (null == tenantId)
            throw new ArgumentNullException(nameof(tenantId));
        if (null == id)
            throw new ArgumentNullException(nameof(id));
        if (null == events || !events.Any())
            throw new ArgumentNullException(nameof(events));

        _logger.LogInformation(
            "Creating Aggregate of type {AggregateRootType} with id {Id} / tenant {TenantId}. Invoking constructor with {EventCount} events",
            typeof(TAggregate).Name, id, tenantId, events.Count());
        var result = (TAggregate)_constructorInfo.Invoke(new object[] { tenantId, id, events });
        _logger.LogInformation("Created Aggregate of type {AggregateRootType} with id {Id} / tenant {TenantId}",
            typeof(TAggregate).Name, id, tenantId);
        result.ClearEvents();

        return result;
    }
}