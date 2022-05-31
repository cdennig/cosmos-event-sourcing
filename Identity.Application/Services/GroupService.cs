using ES.Shared.Repository;
using Identity.Domain;

namespace Identity.Application.Services;

public class GroupService : IGroupService
{
    private readonly ITenantEventsRepository<Guid, Group, Guid, Guid> _groupEventsRepository;

    public GroupService(ITenantEventsRepository<Guid, Group, Guid, Guid> groupEventsRepository)
    {
        _groupEventsRepository = groupEventsRepository;
    }

    private async Task<Group> ReadGroupById(Guid tenantId, Guid groupId, CancellationToken cancellationToken = default)
    {
        return await _groupEventsRepository.RehydrateAsync(tenantId, groupId, cancellationToken);
    }

    public async Task<Group> ReadGroupFromIdAsync(Guid tenantId, Guid groupId, bool includeDeleted = false,
        CancellationToken cancellationToken = default)
    {
        var group = await ReadGroupById(tenantId, groupId, cancellationToken);
        
        if (group.Deleted)
            return includeDeleted ? group : null;
        
        return group;
    }

    public async Task<bool> IsGroupValidForRoleAssignment(Guid tenantId, Guid groupId)
    {
        var group = await ReadGroupById(tenantId, groupId);

        return !group.Deleted;
    }
}