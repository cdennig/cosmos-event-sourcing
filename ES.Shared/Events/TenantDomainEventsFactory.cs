using System.ComponentModel;
using System.Reflection;
using ES.Shared.Attributes;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ES.Shared.Events;

public class
    TenantDomainEventsFactory<TTenantKey, TKey, TPrincipalKey> : ITenantDomainEventsFactory<TTenantKey, TKey,
        TPrincipalKey>
{
    private readonly List<Assembly> _assemblies = new();
    private readonly Dictionary<string, Dictionary<double, ConstructorInfo>> _eventConstructors = new();

    public ITenantDomainEvent<TTenantKey, TKey, TPrincipalKey> BuildEvent(string eventType, double eventVersion,
        string tenantId, string raisedBy,
        string aggregateId, string aggregateType, string timestamp, long version, JObject rawEvent)
    {
        var ci = GetConstructorInfo(eventType, eventVersion);
        var @event = ci.Invoke(new object[]
            {
                aggregateType,
                TypeDescriptor.GetConverter(typeof(TTenantKey)).ConvertFromString(tenantId),
                TypeDescriptor.GetConverter(typeof(TPrincipalKey)).ConvertFromString(raisedBy),
                TypeDescriptor.GetConverter(typeof(TKey)).ConvertFromString(aggregateId),
                version,
                DateTimeOffset.Parse(timestamp)
            }) as
            ITenantDomainEvent<TTenantKey, TKey, TPrincipalKey>;
        JsonConvert.PopulateObject(rawEvent.ToString(), @event);
        return @event;
    }

    private ConstructorInfo GetConstructorInfo(string eventType, double eventVersion)
    {
        if (_eventConstructors.Count == 0)
        {
            throw new ApplicationException("TenantDomainEventsFactory not initialized.");
        }

        if (_eventConstructors.ContainsKey(eventType) && _eventConstructors[eventType].ContainsKey(eventVersion))
        {
            return _eventConstructors[eventType][eventVersion];
        }

        throw new ArgumentException($"No event class found for {eventType} / {eventVersion}");
    }

    public void Initialize(IEnumerable<Type> types)
    {
        if (_assemblies.Count == 0)
        {
            foreach (var type in types)
            {
                _assemblies.Add(Assembly.GetAssembly(type));
            }
        }

        foreach (var type in _assemblies.Select(assembly => assembly.GetTypes()).SelectMany(types => types))
        {
            if (type.GetCustomAttribute(typeof(EventAttribute)) is not EventAttribute attr) continue;
            var constructorInfo = type.GetConstructor(
                BindingFlags.Instance | BindingFlags.NonPublic,
                new[]
                {
                    typeof(string), typeof(TTenantKey), typeof(TPrincipalKey), typeof(TKey), typeof(long),
                    typeof(DateTimeOffset)
                });
            if (constructorInfo == null)
                throw new ApplicationException(
                    $"No constructor found for: {attr.EventType} / {attr.EventVersion}.");
            if (_eventConstructors.ContainsKey(attr.EventType))
            {
                if (_eventConstructors[attr.EventType] != null &&
                    _eventConstructors[attr.EventType].ContainsKey(attr.EventVersion))
                {
                    throw new ApplicationException(
                        $"Multiple event version classes found for type: {attr.EventType} / {attr.EventVersion}.");
                }

                _eventConstructors[attr.EventType].Add(attr.EventVersion, constructorInfo);
            }
            else
            {
                _eventConstructors[attr.EventType] = new Dictionary<double, ConstructorInfo>()
                {
                    {
                        attr.EventVersion, constructorInfo
                    }
                };
            }
        }
    }
}