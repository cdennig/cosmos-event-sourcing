using ES.Shared.Aggregate;
using ES.Shared.Events;
using ES.Shared.Exceptions;
using Identity.Domain.Events.Tenant;

namespace Identity.Domain;

public class Tenant : AggregateRoot<Tenant, Guid, Guid>
{
    private Tenant(Guid id, IEnumerable<IDomainEvent<Guid, Guid>> @events) : base(id, events)
    {
    }

    private Tenant(Guid principalId, Guid tenantId,
        string name,
        string description,
        string language,
        string location = "",
        string pictureUri = "") : base(tenantId)
    {
        var uc = new TenantCreated(this, principalId, name,
            description, language, location, pictureUri);
        AddEvent(uc);
    }

    public static Tenant Initialize(Guid principalId, Guid tenantId,
        string name,
        string description,
        string language,
        string location = "",
        string pictureUri = "")
    {
        return new Tenant(principalId, tenantId, name, description,
            language, location, pictureUri);
    }

    public override string ResourceId => $"/t/{Id}";

    public string Name { get; private set; }
    public string Description { get; private set; }
    public string Language { get; private set; }
    public string Location { get; private set; }
    public string PictureUri { get; private set; }
    public Guid? PrimaryContact { get; private set; }
    public Guid? AdminGroup { get; private set; }
    public Guid? UsersGroup { get; private set; }
    public Guid? AdminRole { get; private set; }
    public Guid? UsersRole { get; private set; }
    public TenantStatus Status { get; private set; }

    private bool IsWritable()
    {
        return !Deleted;
    }

    public void UpdateGeneralInformation(Guid by, string name, string description, string pictureUri)
    {
        if (!IsWritable())
            throw new AggregateReadOnlyException("Tenant readonly");
        var informationUpdated = new TenantGeneralInformationUpdated(this, by, name, description, pictureUri);
        AddEvent(informationUpdated);
    }

    public void UpdateLanguage(Guid by, string language)
    {
        if (!IsWritable())
            throw new AggregateReadOnlyException("Tenant readonly");
        var languageUpdated = new TenantLanguageUpdated(this, by, language);
        AddEvent(languageUpdated);
    }

    public void UpdateLocation(Guid by, string location)
    {
        if (!IsWritable())
            throw new AggregateReadOnlyException("Tenant readonly");
        var locationUpdated = new TenantLocationUpdated(this, by, location);
        AddEvent(locationUpdated);
    }

    public void SetPrimaryContact(Guid by, Guid contactId)
    {
        if (!IsWritable())
            throw new AggregateReadOnlyException("Tenant readonly");
        if (contactId.Equals(Guid.Empty))
            throw new ArgumentException("Contact cannot be empty.");
        var primaryContactSet = new TenantPrimaryContactSet(this, by, contactId);
        AddEvent(primaryContactSet);
    }

    public void SetDirectoryCreated(Guid by, Guid adminGroupId, Guid usersGroupId, Guid adminRole, Guid usersRole)
    {
        if (!IsWritable())
            throw new AggregateReadOnlyException("Tenant readonly");
        if (AdminGroup != null || UsersGroup != null || AdminRole != null || UsersRole != null)
        {
            throw new ArgumentException("Tenant Directory already created.");
        }

        var tenantDirectoryCreated =
            new TenantDirectoryCreated(this, by, adminGroupId, usersGroupId, adminRole, usersRole);
        AddEvent(tenantDirectoryCreated);
    }

    protected override void Apply(IDomainEvent<Guid, Guid> @event)
    {
        ApplyEvent((dynamic)@event);
    }

    private void ApplyEvent(TenantCreated tenantCreated)
    {
        Name = tenantCreated.Name;
        Description = tenantCreated.Description;
        Location = tenantCreated.Location;
        PictureUri = tenantCreated.PictureUri;
        Language = tenantCreated.Language;
        CreatedAt = tenantCreated.Timestamp;
        CreatedBy = tenantCreated.RaisedBy;
        Status = TenantStatus.Requested;
    }

    private void ApplyEvent(TenantGeneralInformationUpdated informationUpdated)
    {
        ModifiedAt = informationUpdated.Timestamp;
        ModifiedBy = informationUpdated.RaisedBy;
        Name = informationUpdated.Name;
        Description = informationUpdated.Description;
        PictureUri = informationUpdated.PictureUri;
    }

    private void ApplyEvent(TenantLanguageUpdated languageUpdated)
    {
        ModifiedAt = languageUpdated.Timestamp;
        ModifiedBy = languageUpdated.RaisedBy;
        Language = languageUpdated.Language;
    }

    private void ApplyEvent(TenantLocationUpdated locationUpdated)
    {
        ModifiedAt = locationUpdated.Timestamp;
        ModifiedBy = locationUpdated.RaisedBy;
        Location = locationUpdated.Location;
    }

    private void ApplyEvent(TenantPrimaryContactSet primaryContactSet)
    {
        ModifiedAt = primaryContactSet.Timestamp;
        ModifiedBy = primaryContactSet.RaisedBy;
        Status |= TenantStatus.PrimaryContactAssigned;
        PrimaryContact = primaryContactSet.PrimaryContact;
    }

    private void ApplyEvent(TenantDirectoryCreated tenantDirectoryCreated)
    {
        ModifiedAt = tenantDirectoryCreated.Timestamp;
        ModifiedBy = tenantDirectoryCreated.RaisedBy;
        Status |= TenantStatus.DirectoryCreated;
        AdminGroup = tenantDirectoryCreated.AdminGroupId;
        UsersGroup = tenantDirectoryCreated.UsersGroupId;
        AdminRole = tenantDirectoryCreated.AdminRoleId;
        UsersRole = tenantDirectoryCreated.UsersRoleId;
    }
}