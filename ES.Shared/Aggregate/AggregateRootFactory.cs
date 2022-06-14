using System.Reflection;
using ES.Shared.Events;
using Microsoft.Extensions.Logging;

namespace ES.Shared.Aggregate;

public class
    AggregateRootFactory<TAggregate, TKey, TPrincipalKey> : IAggregateRootFactory<TAggregate, TKey, TPrincipalKey>
    where TAggregate : class, IAggregateRoot<TKey, TPrincipalKey>
{
    private readonly ConstructorInfo _constructorInfo;
    private readonly ILogger<AggregateRootFactory<TAggregate, TKey, TPrincipalKey>> _logger;

    public AggregateRootFactory(ILogger<AggregateRootFactory<TAggregate, TKey, TPrincipalKey>> logger)
    {
        _logger = logger;
        var aggregateType = typeof(TAggregate);
        _constructorInfo = aggregateType.GetConstructor(
                               BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public,
                               null, new[] { typeof(TKey), typeof(IEnumerable<IDomainEvent<TKey, TPrincipalKey>>) },
                               Array.Empty<ParameterModifier>()) ??
                           throw new InvalidOperationException();
        if (null == _constructorInfo)
        {
            _logger.LogError("Unable to find required private constructor for Aggregate of type {AggregateRootType}",
                aggregateType.Name);
            throw new InvalidOperationException(
                $"Unable to find required private constructor for Aggregate of type '{aggregateType.Name}'");
        }
    }

    public TAggregate Create(TKey id,
        IEnumerable<IDomainEvent<TKey, TPrincipalKey>> events)
    {
        if (null == id)
            throw new ArgumentNullException(nameof(id));
        if (null == events || !events.Any())
            throw new ArgumentNullException(nameof(events));
        _logger.LogInformation("Creating Aggregate of type {AggregateRootType} with id {AggregateRootId}. Invoking constructor with {EventCount} events",
            typeof(TAggregate).Name, id, events.Count());
        var result = (TAggregate)_constructorInfo.Invoke(new object[] { id, events });
        _logger.LogInformation("Created Aggregate of type {AggregateRootType} with id {AggregateRootId}",
            typeof(TAggregate).Name, id);
        result.ClearEvents();

        return result;
    }
}