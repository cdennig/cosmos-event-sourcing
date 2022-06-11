namespace ES.Shared.Cache;

public interface IInvalidatesCacheCommand
{
    public string CacheKey { get; }
}