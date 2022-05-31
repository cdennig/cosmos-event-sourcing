namespace Identity.Domain;

public record GroupMember(Group parentGoup, Guid memberPrincipalId);