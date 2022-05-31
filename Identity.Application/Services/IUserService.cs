using Identity.Domain;

namespace Identity.Application.Services;

public interface IUserService
{
    Task<User?> ReadUserFromIdAsync(Guid userId, bool includeDeleted = false,
        CancellationToken cancellationToken = default);

    Task<bool> IsUserValidAsPrimaryContact(Guid userId);
    Task<bool> IsUserValidAsGroupMember(Guid userId);
}