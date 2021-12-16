using ES.Shared.Aggregate;
using ES.Shared.Events;
using Identity.Domain.Events;

namespace Identity.Domain;

public class TenantUser : BaseAggregateRoot<Guid, TenantUser, Guid, Guid>
{
    private TenantUser(Guid tenantId, Guid id) : base(tenantId, id)
    {
    }

    private TenantUser(Guid tenantId, Guid principalId, Guid userId,
        string firstName,
        string lastName,
        string email,
        string description = "",
        string pictureUri = "") : base(tenantId, userId)
    {
        var uc = new TenantUserCreated(this, principalId, firstName,
            lastName, email, description, pictureUri);
        Apply(uc);
    }

    public static TenantUser Initialize(Guid tenantId, Guid principalId,
        Guid userId, string firstName,
        string lastName,
        string email,
        string description = "",
        string pictureUri = "")
    {
        return new TenantUser(tenantId, principalId, userId, firstName, lastName,
            email, description, pictureUri);
    }

    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public string Description { get; private set; }
    public string Email { get; private set; }
    public string PictureUri { get; private set; }

    public override string ResourceId => $"/t/{TenantId}/users/{Id}";

    private bool IsWritable()
    {
        return !Deleted;
    }

    protected override void Apply(IDomainEvent<Guid, Guid, Guid> @event)
    {
        ApplyEvent((dynamic) @event);
    }

    private void ApplyEvent(TenantUserCreated tenantUserCreated)
    {
        FirstName = tenantUserCreated.FirstName;
        LastName = tenantUserCreated.LastName;
        Description = tenantUserCreated.Description;
        Email = tenantUserCreated.Email;
        PictureUri = tenantUserCreated.PictureUri;
        CreatedAt = tenantUserCreated.Timestamp;
        CreatedBy = tenantUserCreated.RaisedBy;
    }
}