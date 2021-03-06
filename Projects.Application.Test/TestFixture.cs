using System;
using ES.Infrastructure.Behaviors;
using ES.Infrastructure.Cache;
using ES.Infrastructure.Repository;
using ES.Shared.Aggregate;
using ES.Shared.Cache;
using ES.Shared.Repository;
using FluentValidation;
using MediatR;
using MediatR.Registration;
using Microsoft.Extensions.DependencyInjection;
using Projects.Application.Commands;
using Projects.Application.Commands.Handlers;
using Projects.Application.Commands.Responses;
using Projects.Domain;

namespace Projects.Application.Test;

public class TestFixture : IDisposable
{
    public ITenantEventsRepository<Guid, Project, Guid, Guid> CurrentRepo;
    public Guid CurrentId { get; set; }
    public Guid TenantId => Guid.Parse("c4b355d5-8d4d-4ca2-87ec-0964c63fc103");
    public Guid PrincipalId => Guid.Parse("3c7a9bd7-89bf-423e-99f5-ee6a59c4b488");
    public IMediator CurrentMediator;
    public ServiceProvider Provider { get; }

    public TestFixture()
    {
        var serviceConfig = new MediatRServiceConfiguration();
        var services = new ServiceCollection()
            .AddEasyCaching(options => { options.UseInMemory("memory"); })
            .AddLogging()
            .AddSingleton<ICache, InMemoryCache>()
            .AddSingleton<ITenantAggregateRootFactory<Guid, Project, Guid, Guid>,
                TenantAggregateRootFactory<Guid, Project, Guid, Guid>>()
            .AddScoped<ITenantEventsRepository<Guid, Project, Guid, Guid>,
                InMemoryTenantEventsRepository<Guid, Project, Guid, Guid>>()
            .AddValidatorsFromAssembly(typeof(CreateProjectCommand).Assembly)
            .AddTransient(typeof(IPipelineBehavior<,>), typeof(CachingPipelineBehavior<,>))
            .AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationPipelineBehavior<,>))
            .AddScoped<IRequestHandler<CreateProjectCommand, CreateProjectCommandResponse>,
                CreateProjectCommandHandler>()
            .AddScoped<IRequestHandler<PauseProjectCommand, PauseProjectCommandResponse>,
                PauseProjectCommandHandler>()
            .AddScoped<IRequestHandler<ResumeProjectCommand, ResumeProjectCommandResponse>,
                ResumeProjectCommandHandler>()
            .AddScoped<IRequestHandler<FinishProjectCommand, FinishProjectCommandResponse>,
                FinishProjectCommandHandler>()
            .AddScoped<IRequestHandler<CancelProjectCommand, CancelProjectCommandResponse>,
                CancelProjectCommandHandler>()
            .AddScoped<IRequestHandler<SetProjectDatesCommand, SetProjectDatesCommandResponse>,
                SetProjectDatesCommandHandler>()
            .AddScoped<IRequestHandler<SetProjectPriorityCommand, SetProjectPriorityCommandResponse>,
                SetProjectPriorityCommandHandler>()
            .AddScoped<IRequestHandler<SetProjectDescriptionsCommand, SetProjectDescriptionsCommandResponse>,
                SetProjectDescriptionsCommandHandler>()
            .AddScoped<IRequestHandler<DeleteProjectCommand, DeleteProjectCommandResponse>,
                DeleteProjectCommandHandler>()
            .AddScoped<IRequestHandler<UndeleteProjectCommand, UndeleteProjectCommandResponse>,
                UndeleteProjectCommandHandler>()
            .AddScoped<IRequestHandler<StartProjectCommand, StartProjectCommandResponse>,
                StartProjectCommandHandler>();
        ServiceRegistrar.AddRequiredServices(services, serviceConfig);
        Provider = services.BuildServiceProvider();
        CurrentMediator = Provider.GetService<IMediator>();
        CurrentRepo = Provider.GetService<ITenantEventsRepository<Guid, Project, Guid, Guid>>();
    }

    public void Dispose()
    {
    }
}