using ES.Shared.Repository;
using Identity.Domain;

namespace Identity.Application.Services;

public class UserService : IUserService
{
    private readonly IEventsRepository<User, Guid, Guid> _userEventsRepository;

    public UserService(IEventsRepository<User, Guid, Guid> userEventsRepository)
    {
        _userEventsRepository = userEventsRepository;
    }

    private Task<User> ReadUserById(Guid userId, CancellationToken cancellationToken = default)
    {
        return _userEventsRepository.RehydrateAsync(userId, cancellationToken);
    }

    public async Task<User?> ReadUserFromIdAsync(Guid userId, bool includeDeleted = false,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await ReadUserById(userId, cancellationToken);
        
            if (user.Deleted)
                return includeDeleted ? user : null;
            return user;
        }
        catch (ArgumentException)
        {
            return null;
        }
    }

    public async Task<bool> IsUserValidAsPrimaryContact(Guid userId)
    {
        try
        {
            var user = await ReadUserById(userId);
            if (user.Deleted)
                return false;
            if (user.Status == UserStatus.ConfirmationRequested)
                return false;
            return true;
        }
        catch (ArgumentException)
        {
            return false;
        }
    }

    public async Task<bool> IsUserValidAsGroupMember(Guid userId)
    {
        try
        {
            var user = await ReadUserById(userId);
            if (user.Deleted)
                return false;
            if (user.Status == UserStatus.ConfirmationRequested)
                return false;
            return true;
        }
        catch (ArgumentException)
        {
            return false;
        }
    }
}