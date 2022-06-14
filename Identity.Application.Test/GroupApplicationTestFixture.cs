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
using Identity.Application.Commands.Handlers.User;
using Identity.Application.Commands.Responses.User;
using Identity.Application.Commands.User;
using Identity.Application.Services;
using MediatR;
using MediatR.Registration;
using Microsoft.Extensions.DependencyInjection;

namespace Identity.Application.Test;

public class GroupApplicationTestFixture : IDisposable
{
    public ITenantEventsRepository<Guid, Domain.Group, Guid, Guid> CurrentRepo;
    public Guid CurrentId { get; set; }
    public Guid CurerntValidUser { get; set; }
    public Guid TenantId => Guid.Parse("c4b355d5-8d4d-4ca2-87ec-0964c63fc103");
    public Guid PrincipalId => Guid.Parse("3c7a9bd7-89bf-423e-99f5-ee6a59c4b488");
    public IMediator CurrentMediator;
    public ServiceProvider Provider { get; }

    public GroupApplicationTestFixture()
    {
        var serviceConfig = new MediatRServiceConfiguration();
        var services = new ServiceCollection()
            .AddEasyCaching(options => { options.UseInMemory("memory"); })
            .AddLogging()
            .AddSingleton<ICache, InMemoryCache>()
            .AddSingleton<ITenantAggregateRootFactory<Guid, Domain.Group, Guid, Guid>,
                TenantAggregateRootFactory<Guid, Domain.Group, Guid, Guid>>()
            .AddSingleton<IAggregateRootFactory<Domain.User, Guid, Guid>,
                AggregateRootFactory<Domain.User, Guid, Guid>>()
            .AddScoped<ITenantEventsRepository<Guid, Domain.Group, Guid, Guid>,
                InMemoryTenantEventsRepository<Guid, Domain.Group, Guid, Guid>>()
            .AddScoped<IEventsRepository<Domain.User, Guid, Guid>,
                InMemoryEventsRepository<Domain.User, Guid, Guid>>()
            .AddSingleton<IUserService, UserService>()
            .AddValidatorsFromAssembly(typeof(CreateGroupCommand).Assembly)
            .AddValidatorsFromAssembly(typeof(CreateUserCommand).Assembly)
            .AddTransient(typeof(IPipelineBehavior<,>), typeof(CachingPipelineBehavior<,>))
            .AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationPipelineBehavior<,>))
            .AddScoped<IRequestHandler<CreateUserCommand, CreateUserCommandResponse>, CreateUserCommandHandler>()
            .AddScoped<IRequestHandler<ConfirmUserCommand, ConfirmUserCommandResponse>, ConfirmUserCommandHandler>()
            .AddScoped<IRequestHandler<CreateGroupCommand, CreateGroupCommandResponse>, CreateGroupCommandHandler>()
            .AddScoped<IRequestHandler<AddMemberGroupCommand, AddMemberGroupCommandResponse>,
                AddMemberGroupCommandHandler>()
            .AddScoped<IRequestHandler<DeleteGroupCommand, DeleteGroupCommandResponse>, DeleteGroupCommandHandler>()
            .AddScoped<IRequestHandler<UndeleteGroupCommand, UndeleteGroupCommandResponse>,
                UndeleteGroupCommandHandler>()
            .AddScoped<IRequestHandler<RemoveMemberGroupCommand, RemoveMemberGroupCommandResponse>,
                RemoveMemberGroupCommandHandler>()
            .AddScoped<IRequestHandler<UpdateGeneralInformationGroupCommand,
                UpdateGeneralInformationGroupCommandResponse>, UpdateGeneralInformationGroupCommandHandler>();
        ServiceRegistrar.AddRequiredServices(services, serviceConfig);
        Provider = services.BuildServiceProvider();
        CurrentMediator = Provider.GetService<IMediator>();
        CurrentRepo = Provider.GetService<ITenantEventsRepository<Guid, Domain.Group, Guid, Guid>>();
        CurerntValidUser = CreateUserAsync().GetAwaiter().GetResult();
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

    public void Dispose()
    {
        Provider.Dispose();
    }
}