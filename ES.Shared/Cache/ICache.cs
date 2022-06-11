namespace ES.Shared.Cache;

public interface ICache
{
    public Task<string> GetItemAsync(string key);
    public Task SetItemAsync(string key, string value, TimeSpan expiresIn);
    public Task InvalidateItemAsync(string key);
}