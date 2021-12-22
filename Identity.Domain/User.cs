using ES.Shared.Aggregate;
using ES.Shared.Events;
using Identity.Domain.Events;
using Identity.Domain.Events.User;

namespace Identity.Domain;

public class User : AggregateRoot<User, Guid, Guid>
{
    private User(Guid id) : base(id)
    {
    }

    private User(Guid principalId, Guid userId,
        string firstName,
        string lastName,
        string email,
        string description = "",
        string pictureUri = "") : base(userId)
    {
        var uc = new UserCreated(this, principalId, firstName,
            lastName, email, description, pictureUri);
        AddEvent(uc);
    }

    public static User Initialize(Guid principalId,
        Guid userId, string firstName,
        string lastName,
        string email,
        string description = "",
        string pictureUri = "")
    {
        return new User(principalId, userId, firstName, lastName,
            email, description, pictureUri);
    }

    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public string Description { get; private set; }
    public string Email { get; private set; }
    public string PictureUri { get; private set; }
    public UserStatus Status { get; private set; }

    public override string ResourceId => $"/users/{Id}";

    private bool IsWritable()
    {
        return !Deleted;
    }

    public void ConfirmUser(Guid by)
    {
        if (!IsWritable())
            throw new Exception("User readonly");
        var userConfirmed = new UserConfirmed(this, by);
        AddEvent(userConfirmed);
    }

    public void UpdatePersonalInformation(Guid by, string firstName, string lastName, string description,
        string pictureUri)
    {
        if (!IsWritable())
            throw new Exception("User readonly");
        if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName))
            throw new ArgumentException("FirstName or LastName may not be empty");
        var userPersonalInformationUpdated =
            new UserPersonalInformationUpdated(this, by, firstName, lastName, description, pictureUri);
        AddEvent(userPersonalInformationUpdated);
    }
    
    public void UpdateEmail(Guid by, string email)
    {
        if (!IsWritable())
            throw new Exception("User readonly");
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email may not be empty");
        var userEmailUpdated =
            new UserEmailUpdated(this, by, email);
        AddEvent(userEmailUpdated);
    }
    
    
    public void DeleteUser(Guid by)
    {
        if (Deleted)
            throw new ArgumentException("User already deleted.");
        var deleted = new UserDeleted(this, by);
        AddEvent(deleted);
    }

    public void UndeleteUser(Guid by)
    {
        if (!Deleted)
            throw new ArgumentException("User not deleted.");
        var undeleted = new UserUndeleted(this, by);
        AddEvent(undeleted);
    }

    protected override void Apply(IDomainEvent<Guid, Guid> @event)
    {
        ApplyEvent((dynamic) @event);
    }

    private void ApplyEvent(UserCreated userCreated)
    {
        FirstName = userCreated.FirstName;
        LastName = userCreated.LastName;
        Description = userCreated.Description;
        Email = userCreated.Email;
        PictureUri = userCreated.PictureUri;
        CreatedAt = userCreated.Timestamp;
        CreatedBy = userCreated.RaisedBy;
        Status = UserStatus.ConfirmationRequested;
    }

    private void ApplyEvent(UserConfirmed userConfirmed)
    {
        ModifiedAt = userConfirmed.Timestamp;
        ModifiedBy = userConfirmed.RaisedBy;
        Status = UserStatus.Confirmed;
    }

    private void ApplyEvent(UserPersonalInformationUpdated userPersonalInformationUpdated)
    {
        FirstName = userPersonalInformationUpdated.FirstName;
        LastName = userPersonalInformationUpdated.LastName;
        Description = userPersonalInformationUpdated.Description;
        ModifiedAt = userPersonalInformationUpdated.Timestamp;
        ModifiedBy = userPersonalInformationUpdated.RaisedBy;
        PictureUri = userPersonalInformationUpdated.PictureUri;
    }
    
    private void ApplyEvent(UserEmailUpdated userEmailUpdated)
    {
        Email = userEmailUpdated.Email;
        ModifiedAt = userEmailUpdated.Timestamp;
        ModifiedBy = userEmailUpdated.RaisedBy;
    }
    
    private void ApplyEvent(UserDeleted userEmailUpdated)
    {
        Deleted = true;
        DeletedAt = userEmailUpdated.Timestamp;
        DeletedBy = userEmailUpdated.RaisedBy;
    }
    
    private void ApplyEvent(UserUndeleted userUndeleted)
    {
        Deleted = false;
        ModifiedAt = userUndeleted.Timestamp;
        ModifiedBy = userUndeleted.RaisedBy;
    }
}