namespace ES.Shared.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class EventAttribute : Attribute
{
    public string EventType { get; }
    public double EventVersion { get; }

    public EventAttribute(string eventType, double eventVersion)
    {
        EventType = eventType;
        EventVersion = eventVersion;
    }
}