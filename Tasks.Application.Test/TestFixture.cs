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
using Tasks.Application.Commands;
using Tasks.Application.Commands.Handlers;
using Tasks.Application.Commands.Responses;
using Tasks.Domain;

namespace Tasks.Application.Test;

public class TestFixture : IDisposable
{
    public ITenantEventsRepository<Guid, Domain.Task, Guid, Guid> CurrentRepo;
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
            .AddSingleton<ICache, InMemoryCache>()
            .AddSingleton<ITenantAggregateRootFactory<Guid, Domain.Task, Guid, Guid>>(
                new TenantAggregateRootFactory<Guid, Domain.Task, Guid, Guid>())
            .AddScoped<ITenantEventsRepository<Guid, Domain.Task, Guid, Guid>,
                InMemoryTenantEventsRepository<Guid, Domain.Task, Guid, Guid>>()
            .AddValidatorsFromAssembly(typeof(CreateTaskCommand).Assembly)
            .AddTransient(typeof(IPipelineBehavior<,>), typeof(CachingPipelineBehavior<,>))
            .AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationPipelineBehavior<,>))
            .AddScoped<IRequestHandler<CreateTaskCommand, CreateTaskCommandResponse>,
                CreateTaskCommandHandler>()
            .AddScoped<IRequestHandler<AssignToProjectTaskCommand, AssignToProjectTaskCommandResponse>,
                AssignToProjectTaskCommandHandler>()
            .AddScoped<IRequestHandler<ChangeTimeLogEntryCommentCommand, ChangeTimeLogEntryCommentCommandResponse>,
                ChangeTimeLogEntryCommentCommandHandler>()
            .AddScoped<IRequestHandler<DeleteTaskCommand, DeleteTaskCommandResponse>,
                DeleteTaskCommandHandler>()
            .AddScoped<IRequestHandler<DeleteTimeLogEntryCommand, DeleteTimeLogEntryCommandResponse>,
                DeleteTimeLogEntryCommandHandler>()
            .AddScoped<IRequestHandler<LogTimeTaskCommand, LogTimeTaskCommandResponse>,
                LogTimeTaskCommandHandler>()
            .AddScoped<IRequestHandler<RemoveFromProjectTaskCommand, RemoveFromProjectTaskCommandResponse>,
                RemoveFromProjectTaskCommandHandler>()
            .AddScoped<IRequestHandler<SetCompleteTaskCommand, SetCompleteTaskCommandResponse>,
                SetCompleteTaskCommandHandler>()
            .AddScoped<IRequestHandler<SetDatesTaskCommand, SetDatesTaskCommandResponse>,
                SetDatesTaskCommandHandler>()
            .AddScoped<IRequestHandler<UndeleteTaskCommand, UndeleteTaskCommandResponse>,
                UndeleteTaskCommandHandler>()
            .AddScoped<IRequestHandler<SetDescriptionsTaskCommand, SetDescriptionsTaskCommandResponse>,
                SetDescriptionsTaskCommandHandler>()
            .AddScoped<IRequestHandler<SetIncompleteTaskCommand, SetIncompleteTaskCommandResponse>,
                SetIncompleteTaskCommandHandler>()
            .AddScoped<IRequestHandler<SetPriorityTaskCommand, SetPriorityTaskCommandResponse>,
                SetPriorityTaskCommandHandler>()
            .AddScoped<IRequestHandler<SetTimeEstimationTaskCommand, SetTimeEstimationTaskCommandResponse>,
                SetTimeEstimationTaskCommandHandler>();
        ServiceRegistrar.AddRequiredServices(services, serviceConfig);
        Provider = services.BuildServiceProvider();
        CurrentMediator = Provider.GetService<IMediator>();
        CurrentRepo = Provider.GetService<ITenantEventsRepository<Guid, Domain.Task, Guid, Guid>>();
    }

    public void Dispose()
    {
    }
}