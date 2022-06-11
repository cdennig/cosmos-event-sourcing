using ES.Shared.Cache;
using Identity.Application.Commands.Responses.User;
using MediatR;

namespace Identity.Application.Commands.User;

public class UpdatePersonalInformationUserCommand : IRequest<UpdatePersonalInformationUserCommandResponse>,
    IInvalidatesCacheCommand
{
    public Guid PrincipalId { get; set; }
    public Guid Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Description { get; set; }
    public string PictureUri { get; set; }
    public string CacheKey => $"user/{Id}";
}