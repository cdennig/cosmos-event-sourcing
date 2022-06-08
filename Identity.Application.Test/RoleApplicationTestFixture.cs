using System;
using System.Threading.Tasks;
using ES.Infrastructure.Behaviors;
using ES.Infrastructure.Repository;
using ES.Shared.Aggregate;
using ES.Shared.Repository;
using FluentValidation;
using Identity.Application.Commands.Handlers.Group;
using Identity.Application.Commands.Responses.Group;
using Identity.Application.Commands.Group;
using Identity.Application.Commands.Handlers.Role;
using Identity.Application.Commands.Handlers.User;
using Identity.Application.Commands.Responses.Role;
using Identity.Application.Commands.Responses.User;
using Identity.Application.Commands.Role;
using Identity.Application.Commands.User;
using Identity.Application.Services;
using MediatR;
using MediatR.Registration;
using Microsoft.Extensions.DependencyInjection;

namespace Identity.Application.Test;

public class RoleApplicationTestFixture : IDisposable
{
    public ITenantEventsRepository<Guid, Domain.Role, Guid, Guid> CurrentRepo;
    public ITenantEventsRepository<Guid, Domain.Group, Guid, Guid> GroupRepo;
    public Guid CurrentId { get; set; }
    public Guid CurrentValidUser { get; set; }
    public Guid CurrentValidGroup { get; set; }
    public Guid TenantId => Guid.Parse("c4b355d5-8d4d-4ca2-87ec-0964c63fc103");
    public Guid PrincipalId => Guid.Parse("3c7a9bd7-89bf-423e-99f5-ee6a59c4b488");
    public IMediator CurrentMediator;
    public ServiceProvider Provider { get; }

    public RoleApplicationTestFixture()
    {
        var serviceConfig = new MediatRServiceConfiguration();
        var services = new ServiceCollection()
            .AddSingleton<ITenantAggregateRootFactory<Guid, Domain.Role, Guid, Guid>>(
                new TenantAggregateRootFactory<Guid, Domain.Role, Guid, Guid>())
            .AddSingleton<ITenantAggregateRootFactory<Guid, Domain.Group, Guid, Guid>>(
                new TenantAggregateRootFactory<Guid, Domain.Group, Guid, Guid>())
            .AddSingleton<IAggregateRootFactory<Domain.User, Guid, Guid>>(
                new AggregateRootFactory<Domain.User, Guid, Guid>())
            .AddScoped<ITenantEventsRepository<Guid, Domain.Role, Guid, Guid>,
                InMemoryTenantEventsRepository<Guid, Domain.Role, Guid, Guid>>()
            .AddScoped<ITenantEventsRepository<Guid, Domain.Group, Guid, Guid>,
                InMemoryTenantEventsRepository<Guid, Domain.Group, Guid, Guid>>()
            .AddScoped<IEventsRepository<Domain.User, Guid, Guid>,
                InMemoryEventsRepository<Domain.User, Guid, Guid>>()
            .AddSingleton<IUserService, UserService>()
            .AddSingleton<IGroupService, GroupService>()
            .AddValidatorsFromAssembly(typeof(CreateRoleCommand).Assembly)
            .AddValidatorsFromAssembly(typeof(CreateGroupCommand).Assembly)
            .AddValidatorsFromAssembly(typeof(CreateUserCommand).Assembly)
            .AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationPipelineBehavior<,>))
            .AddScoped<IRequestHandler<CreateUserCommand, CreateUserCommandResponse>, CreateUserCommandHandler>()
            .AddScoped<IRequestHandler<ConfirmUserCommand, ConfirmUserCommandResponse>, ConfirmUserCommandHandler>()
            .AddScoped<IRequestHandler<CreateGroupCommand, CreateGroupCommandResponse>, CreateGroupCommandHandler>()
            .AddScoped<IRequestHandler<AddMemberGroupCommand, AddMemberGroupCommandResponse>,
                AddMemberGroupCommandHandler>()
            .AddScoped<IRequestHandler<CreateRoleCommand, CreateRoleCommandResponse>, CreateRoleCommandHandler>()
            .AddScoped<IRequestHandler<AssignRoleToGroupCommand, AssignRoleToGroupCommandResponse>,
                AssignRoleToGroupCommandHandler>()
            .AddScoped<IRequestHandler<RemoveRoleFromGroupCommand, RemoveRoleFromGroupCommandResponse>,
                RemoveRoleFromGroupCommandHandler>()
            .AddScoped<IRequestHandler<UpdateGeneralInformationRoleCommand,
                UpdateGeneralInformationRoleCommandResponse>, UpdateGeneralInformationRoleCommandHandler>();
        ServiceRegistrar.AddRequiredServices(services, serviceConfig);
        Provider = services.BuildServiceProvider();
        CurrentMediator = Provider.GetService<IMediator>();
        CurrentRepo = Provider.GetService<ITenantEventsRepository<Guid, Domain.Role, Guid, Guid>>();
        CurrentValidUser = CreateUserAsync().GetAwaiter().GetResult();
        CurrentValidGroup = CreateGroupAsync().GetAwaiter().GetResult();
    }
    
    private async Task<Guid> CreateUserAsync()
    {
        var res = await CurrentMediator.Send(new CreateUserCommand()
        {
            PrincipalId = PrincipalId,
            FirstName = "Jane",
            LastName = "Doe",
            Email = "jane.doe@example.com",
            Description = "Test User",
            PictureUri = "https://example.com/picture.jpg"
        });

        await CurrentMediator.Send(new ConfirmUserCommand()
        {
            PrincipalId = PrincipalId,
            Id = res.Id
        });

        return res.Id;
    }
    
    
    private async Task<Guid> CreateGroupAsync()
    {
        var res = await CurrentMediator.Send(new CreateGroupCommand()
        {
            TenantId = TenantId,
            PrincipalId = PrincipalId,
            Name = "Test Group",
            Description = "Test User",
            PictureUri = "https://example.com/picture.jpg"
        });

        await CurrentMediator.Send(new AddMemberGroupCommand()
        {
            TenantId = TenantId,
            PrincipalId = PrincipalId,
            Id = res.Id,
            MemberId = CurrentValidUser
        });

        return res.Id;
    }

    public void Dispose()
    {
        Provider.Dispose();
    }
}