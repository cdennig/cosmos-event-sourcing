namespace Identity.Domain;

public record GroupMember(Group ParentGoup, Guid MemberPrincipalId);