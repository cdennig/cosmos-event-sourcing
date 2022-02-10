namespace ES.Shared.Exceptions;

public class AggregateReadOnlyException: Exception
{
    public AggregateReadOnlyException()
    {
    }

    public AggregateReadOnlyException(string message) : base(message)
    {
    }

    public AggregateReadOnlyException(string message, Exception inner) : base(message, inner)
    {
    }
}