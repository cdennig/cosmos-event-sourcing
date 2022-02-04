using Identity.Domain;

namespace Identity.Application.Services;

public interface IUserService
{
    Task<User> ReadUserFromIdAsync(Guid userId);
    Task<bool> IsUserValidAsPrimaryContact(Guid userId);
}