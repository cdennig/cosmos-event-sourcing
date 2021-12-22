namespace Identity.Domain;

[Flags]
public enum TenantStatus : short
{
    None = 0,
    Requested = 1,
    PrimaryContactAssigned = 2,
    DirectoryCreated = 4,
}