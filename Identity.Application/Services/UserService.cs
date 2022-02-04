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

    private Task<User> ReadUserById(Guid userId)
    {
        return _userEventsRepository.RehydrateAsync(userId);
    }

    public Task<User> ReadUserFromIdAsync(Guid userId)
    {
        return ReadUserById(userId);
    }

    public async Task<bool> IsUserValidAsPrimaryContact(Guid userId)
    {
        var user = await ReadUserById(userId);
        if (user.Deleted)
            return false;
        if (user.Status == UserStatus.ConfirmationRequested)
            return false;
        return true;
    }
}