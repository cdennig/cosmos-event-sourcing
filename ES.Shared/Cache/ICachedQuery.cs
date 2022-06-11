namespace ES.Shared.Cache;

public interface ICachedQuery
{
    public string CacheKey { get; }
    public TimeSpan ExpiresIn { get; }
}