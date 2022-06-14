using System;
using System.Threading.Tasks;
using ES.Infrastructure.Behaviors;
using ES.Infrastructure.Cache;
using ES.Infrastructure.Repository;
using ES.Shared.Aggregate;
using ES.Shared.Cache;
using ES.Shared.Repository;
using FluentValidation;
using Identity.Application.Commands.Handlers.Group;
using Identity.Application.Commands.Responses.Group;
using Identity.Application.Commands.Group;
using Identity.Application.Commands.Handlers.Role;
using Identity.Application.Commands.Handlers.Tenant;
using Identity.Application.Commands.Handlers.User;
using Identity.Application.Commands.Responses.Role;
using Identity.Application.Commands.Responses.Tenant;
using Identity.Application.Commands.Responses.User;
using Identity.Application.Commands.Role;
using Identity.Application.Commands.Tenant;
using Identity.Application.Commands.User;
using Identity.Application.Services;
using MediatR;
using MediatR.Registration;
using Microsoft.Extensions.DependencyInjection;

namespace Identity.Application.Test;

public class TenantApplicationTestFixture : IDisposable
{
    public IEventsRepository<Domain.Tenant, Guid, Guid> CurrentRepo;
    public ITenantEventsRepository<Guid, Domain.Role, Guid, Guid> RoleRepo;
    public ITenantEventsRepository<Guid, Domain.Group, Guid, Guid> GroupRepo;
    public Guid CurrentId { get; set; }
    public Guid CurrentValidUser { get; set; }
    public Guid PrincipalId => Guid.Parse("3c7a9bd7-89bf-423e-99f5-ee6a59c4b488");
    public IMediator CurrentMediator;
    public ServiceProvider Provider { get; }

    public TenantApplicationTestFixture()
    {
        var serviceConfig = new MediatRServiceConfiguration();
        var services = new ServiceCollection()
            .AddEasyCaching(options => { options.UseInMemory("memory"); })
            .AddLogging()
            .AddSingleton<ICache, InMemoryCache>()
            .AddSingleton<IAggregateRootFactory<Domain.Tenant, Guid, Guid>,
                AggregateRootFactory<Domain.Tenant, Guid, Guid>>()
            .AddSingleton<ITenantAggregateRootFactory<Guid, Domain.Role, Guid, Guid>,
                TenantAggregateRootFactory<Guid, Domain.Role, Guid, Guid>>()
            .AddSingleton<ITenantAggregateRootFactory<Guid, Domain.Group, Guid, Guid>,
                TenantAggregateRootFactory<Guid, Domain.Group, Guid, Guid>>()
            .AddSingleton<IAggregateRootFactory<Domain.User, Guid, Guid>,
                AggregateRootFactory<Domain.User, Guid, Guid>>()
            .AddScoped<IEventsRepository<Domain.Tenant, Guid, Guid>,
                InMemoryEventsRepository<Domain.Tenant, Guid, Guid>>()
            .AddScoped<ITenantEventsRepository<Guid, Domain.Role, Guid, Guid>,
                InMemoryTenantEventsRepository<Guid, Domain.Role, Guid, Guid>>()
            .AddScoped<ITenantEventsRepository<Guid, Domain.Group, Guid, Guid>,
                InMemoryTenantEventsRepository<Guid, Domain.Group, Guid, Guid>>()
            .AddScoped<IEventsRepository<Domain.User, Guid, Guid>,
                InMemoryEventsRepository<Domain.User, Guid, Guid>>()
            .AddSingleton<IUserService, UserService>()
            .AddSingleton<IGroupService, GroupService>()
            .AddValidatorsFromAssembly(typeof(CreateTenantCommand).Assembly)
            .AddValidatorsFromAssembly(typeof(CreateRoleCommand).Assembly)
            .AddValidatorsFromAssembly(typeof(CreateGroupCommand).Assembly)
            .AddValidatorsFromAssembly(typeof(CreateUserCommand).Assembly)
            .AddTransient(typeof(IPipelineBehavior<,>), typeof(CachingPipelineBehavior<,>))
            .AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationPipelineBehavior<,>))
            .AddScoped<IRequestHandler<CreateUserCommand, CreateUserCommandResponse>, CreateUserCommandHandler>()
            .AddScoped<IRequestHandler<ConfirmUserCommand, ConfirmUserCommandResponse>, ConfirmUserCommandHandler>()
            .AddScoped<IRequestHandler<CreateGroupCommand, CreateGroupCommandResponse>, CreateGroupCommandHandler>()
            .AddScoped<IRequestHandler<AddMemberGroupCommand, AddMemberGroupCommandResponse>,
                AddMemberGroupCommandHandler>()
            .AddScoped<IRequestHandler<CreateRoleCommand, CreateRoleCommandResponse>, CreateRoleCommandHandler>()
            .AddScoped<IRequestHandler<AssignRoleToGroupCommand, AssignRoleToGroupCommandResponse>,
                AssignRoleToGroupCommandHandler>()
            .AddScoped<IRequestHandler<CreateTenantCommand, CreateTenantCommandResponse>,
                CreateTenantCommandHandler>()
            .AddScoped<IRequestHandler<SetPrimaryContactTenantCommand, SetPrimaryContactTenantCommandResponse>,
                SetPrimaryContactTenantCommandHandler>()
            .AddScoped<IRequestHandler<UpdateGeneralInformationTenantCommand,
                    UpdateGeneralInformationTenantCommandResponse>,
                UpdateGeneralInformationTenantCommandHandler>()
            .AddScoped<IRequestHandler<UpdateLanguageTenantCommand, UpdateLanguageTenantCommandResponse>,
                UpdateLanguageTenantCommandHandler>()
            .AddScoped<IRequestHandler<UpdateLocationTenantCommand, UpdateLocationTenantCommandResponse>,
                UpdateLocationTenantCommandHandler>()
            .AddScoped<IRequestHandler<SetDirectoryCreatedTenantCommand,
                SetDirectoryCreatedTenantCommandResponse>, SetDirectoryCreatedTenantCommandHandler>();
        ServiceRegistrar.AddRequiredServices(services, serviceConfig);
        Provider = services.BuildServiceProvider();
        CurrentMediator = Provider.GetService<IMediator>();
        CurrentRepo = Provider.GetService<IEventsRepository<Domain.Tenant, Guid, Guid>>();
        CurrentValidUser = CreateUserAsync().GetAwaiter().GetResult();
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


    public async Task<(Guid, Guid)> CreateGroupsAsync(Guid tenantId)
    {
        var resAdmin = await CurrentMediator.Send(new CreateGroupCommand()
        {
            TenantId = tenantId,
            PrincipalId = PrincipalId,
            Name = "Admin Group",
            Description = "Admin Group",
            PictureUri = "https://example.com/picture.jpg"
        });

        await CurrentMediator.Send(new AddMemberGroupCommand()
        {
            TenantId = tenantId,
            PrincipalId = PrincipalId,
            Id = resAdmin.Id,
            MemberId = CurrentValidUser
        });

        var resUsers = await CurrentMediator.Send(new CreateGroupCommand()
        {
            TenantId = tenantId,
            PrincipalId = PrincipalId,
            Name = "Users Group",
            Description = "Users Group",
            PictureUri = "https://example.com/picture.jpg"
        });

        await CurrentMediator.Send(new AddMemberGroupCommand()
        {
            TenantId = tenantId,
            PrincipalId = PrincipalId,
            Id = resUsers.Id,
            MemberId = CurrentValidUser
        });

        return (resAdmin.Id, resUsers.Id);
    }

    public async Task<(Guid, Guid)> CreateRolesAsync(Guid tenantId, Guid adminGroup, Guid usersGroup)
    {
        var resAdmin = await CurrentMediator.Send(new CreateRoleCommand()
        {
            TenantId = tenantId,
            PrincipalId = PrincipalId,
            Name = "Tenant Admin Role",
            Description = "Tenant Admin Role",
            Actions = new()
            {
                new($"/t", "create"),
                new($"/t/{tenantId}", "*"),
                new("/u/*", "read"),
                new($"/u/{PrincipalId}", "*"),
                new($"/t/{tenantId}/groups", "*"),
                new($"/t/{tenantId}/roles", "*"),
                new($"/t/{tenantId}/projects", "*"),
                new($"/t/{tenantId}/projects/*/tasks", "*")
            },
            NotActions = new(),
            IsBuiltIn = true
        });

        await CurrentMediator.Send(new AssignRoleToGroupCommand()
        {
            TenantId = tenantId,
            PrincipalId = PrincipalId,
            Id = resAdmin.Id,
            GroupId = adminGroup
        });

        var resUser = await CurrentMediator.Send(new CreateRoleCommand()
        {
            TenantId = tenantId,
            PrincipalId = PrincipalId,
            Name = "Tenant Users Role",
            Description = "Tenant Users Role",
            Actions = new()
            {
                new($"/t", "create"),
                new($"/t/{tenantId}", "read"),
                new("/u/*", "read"),
                new($"/u/{PrincipalId}", "*"),
                new($"/t/{tenantId}/groups", "read"),
                new($"/t/{tenantId}/roles", "read"),
                new($"/t/{tenantId}/projects", "create"),
            },
            NotActions = new(),
            IsBuiltIn = true
        });

        await CurrentMediator.Send(new AssignRoleToGroupCommand()
        {
            TenantId = tenantId,
            PrincipalId = PrincipalId,
            Id = resUser.Id,
            GroupId = usersGroup
        });

        return (resAdmin.Id, resUser.Id);
    }

    public void Dispose()
    {
        Provider.Dispose();
    }
}