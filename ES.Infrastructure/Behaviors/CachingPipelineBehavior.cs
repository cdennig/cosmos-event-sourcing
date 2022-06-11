using ES.Shared.Cache;
using MediatR;
using Newtonsoft.Json;

namespace ES.Infrastructure.Behaviors;

public class CachingPipelineBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ICache _cache;
    private readonly TimeSpan defaultExpiration = TimeSpan.FromHours(1);

    public CachingPipelineBehavior(ICache cache)
    {
        _cache = cache;
    }

    public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken,
        RequestHandlerDelegate<TResponse> next)
    {
        if (request is ICachedQuery)
        {
            var cacheResult = await _cache.GetItemAsync(((ICachedQuery)request).CacheKey);
            if (!string.IsNullOrWhiteSpace(cacheResult))
                return JsonConvert.DeserializeObject<TResponse>(cacheResult);

            var result = await next();
            var cacheValue = JsonConvert.SerializeObject(result);
            await _cache.SetItemAsync(((ICachedQuery)request).CacheKey, cacheValue, defaultExpiration);
            return result;
        }

        if (request is IInvalidatesCacheCommand)
        {
            var result = await next();
            await _cache.InvalidateItemAsync(((IInvalidatesCacheCommand)request).CacheKey);
            return result;
        }

        return await next();
    }
}