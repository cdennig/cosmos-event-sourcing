namespace Identity.Domain;

public record RoleAssignment(Role ParentRole, Guid GroupId);