using System.Reflection;
using ES.Shared.Events;

namespace ES.Shared.Aggregate;

public class
    TenantAggregateRootFactory<TTenantKey, TAggregate, TKey, TPrincipalKey> : ITenantAggregateRootFactory<TTenantKey,
        TAggregate, TKey, TPrincipalKey>
    where TAggregate : class, ITenantAggregateRoot<TTenantKey, TKey, TPrincipalKey>
{
    private readonly ConstructorInfo _constructorInfo;

    public TenantAggregateRootFactory()
    {
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
            throw new InvalidOperationException(
                $"Unable to find required private constructor for Aggregate of type '{aggregateType.Name}'");
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

        var result = (TAggregate)_constructorInfo.Invoke(new object[] { tenantId, id, events });

        result.ClearEvents();

        return result;
    }
}