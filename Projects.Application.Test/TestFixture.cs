using System;
using ES.Infrastructure.Repository;
using ES.Shared.Repository;
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
    public IMediator CurrentMediator;
    public ServiceProvider Provider { get; }

    public TestFixture()
    {
        var serviceConfig = new MediatRServiceConfiguration();
        var services = new ServiceCollection()
            .AddScoped<ITenantEventsRepository<Guid, Project, Guid, Guid>,
                InMemoryTenantEventsRepository<Guid, Project, Guid, Guid>>()
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