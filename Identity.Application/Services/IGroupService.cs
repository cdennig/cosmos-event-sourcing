using Identity.Domain;

namespace Identity.Application.Services;

public interface IGroupService
{
    Task<Group> ReadGroupFromIdAsync(Guid tenantId, Guid groupId, bool includeDeleted = false,
        CancellationToken cancellationToken = default);
    Task<bool> IsGroupValidForRoleAssignment(Guid tenantId, Guid groupId);
}