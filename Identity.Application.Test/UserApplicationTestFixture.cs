using System;
using ES.Infrastructure.Behaviors;
using ES.Infrastructure.Cache;
using ES.Infrastructure.Repository;
using ES.Shared.Aggregate;
using ES.Shared.Cache;
using ES.Shared.Repository;
using FluentValidation;
using Identity.Application.Commands.Handlers.User;
using Identity.Application.Commands.Responses.User;
using Identity.Application.Commands.User;
using MediatR;
using MediatR.Registration;
using Microsoft.Extensions.DependencyInjection;

namespace Identity.Application.Test;

public class UserApplicationTestFixture : IDisposable
{
    public IEventsRepository<Domain.User, Guid, Guid> CurrentRepo;
    public Guid CurrentId { get; set; }
    public Guid TenantId => Guid.Parse("c4b355d5-8d4d-4ca2-87ec-0964c63fc103");
    public Guid PrincipalId => Guid.Parse("3c7a9bd7-89bf-423e-99f5-ee6a59c4b488");
    public IMediator CurrentMediator;
    public ServiceProvider Provider { get; }

    public UserApplicationTestFixture()
    {
        var serviceConfig = new MediatRServiceConfiguration();
        var services = new ServiceCollection()
            .AddEasyCaching(options => { options.UseInMemory("memory"); })
            .AddLogging()
            .AddSingleton<ICache, InMemoryCache>()
            .AddSingleton<IAggregateRootFactory<Domain.User, Guid, Guid>,
                AggregateRootFactory<Domain.User, Guid, Guid>>()
            .AddScoped<IEventsRepository<Domain.User, Guid, Guid>,
                InMemoryEventsRepository<Domain.User, Guid, Guid>>()
            .AddValidatorsFromAssembly(typeof(CreateUserCommand).Assembly)
            .AddTransient(typeof(IPipelineBehavior<,>), typeof(CachingPipelineBehavior<,>))
            .AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationPipelineBehavior<,>))
            .AddScoped<IRequestHandler<CreateUserCommand, CreateUserCommandResponse>, CreateUserCommandHandler>()
            .AddScoped<IRequestHandler<ConfirmUserCommand, ConfirmUserCommandResponse>, ConfirmUserCommandHandler>()
            .AddScoped<IRequestHandler<DeleteUserCommand, DeleteUserCommandResponse>, DeleteUserCommandHandler>()
            .AddScoped<IRequestHandler<UndeleteUserCommand, UndeleteUserCommandResponse>,
                UndeleteUserCommandHandler>()
            .AddScoped<IRequestHandler<UpdateEmailUserCommand, UpdateEmailUserCommandResponse>,
                UpdateEmailUserCommandHandler>()
            .AddScoped<IRequestHandler<UpdatePersonalInformationUserCommand,
                UpdatePersonalInformationUserCommandResponse>, UpdatePersonalInformationUserCommandHandler>();
        ServiceRegistrar.AddRequiredServices(services, serviceConfig);
        Provider = services.BuildServiceProvider();
        CurrentMediator = Provider.GetService<IMediator>();
        CurrentRepo = Provider.GetService<IEventsRepository<Domain.User, Guid, Guid>>();
    }

    public void Dispose()
    {
        Provider.Dispose();
    }
}