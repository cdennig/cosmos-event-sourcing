using System.Reflection;
using ES.Shared.Events;

namespace ES.Shared.Aggregate;

public class
    AggregateRootFactory<TAggregate, TKey, TPrincipalKey> : IAggregateRootFactory<TAggregate, TKey, TPrincipalKey>
    where TAggregate : class, IAggregateRoot<TKey, TPrincipalKey>
{
    private readonly ConstructorInfo _constructorInfo;

    public AggregateRootFactory()
    {
        var aggregateType = typeof(TAggregate);
        _constructorInfo = aggregateType.GetConstructor(
                               BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public,
                               null, new[] { typeof(TKey), typeof(IEnumerable<IDomainEvent<TKey, TPrincipalKey>>) },
                               Array.Empty<ParameterModifier>()) ??
                           throw new InvalidOperationException();
        if (null == _constructorInfo)
            throw new InvalidOperationException(
                $"Unable to find required private constructor for Aggregate of type '{aggregateType.Name}'");
    }

    public TAggregate Create(TKey id,
        IEnumerable<IDomainEvent<TKey, TPrincipalKey>> events)
    {
        if (null == id)
            throw new ArgumentNullException(nameof(id));
        if (null == events || !events.Any())
            throw new ArgumentNullException(nameof(events));
        var result = (TAggregate)_constructorInfo.Invoke(new object[] { id, events });

        result.ClearEvents();

        return result;
    }
}