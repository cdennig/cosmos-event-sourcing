﻿using System;
using ES.Infrastructure.Repository;
using ES.Shared.Repository;
using MediatR;
using MediatR.Registration;
using Microsoft.Extensions.DependencyInjection;
using Projects.Application.Commands;
using Projects.Application.Commands.Handlers;
using Projects.Application.Commands.Responses;
using Projects.Domain;

namespace Projects.Application.Test
{
    public class TestFixture : IDisposable
    {
        public InMemoryEventsRepository<Guid, Project, Guid> CurrentRepo = new();
        public Guid CurrentId { get; set; }
        public Guid TenantId => Guid.Parse("c4b355d5-8d4d-4ca2-87ec-0964c63fc103");
        public IMediator CurrentMediator;
        public ServiceProvider Provider { get; }

        public TestFixture()
        {
            var serviceConfig = new MediatRServiceConfiguration();
            var services = new ServiceCollection()
                .AddScoped<IEventsRepository<Guid, Project, Guid>, InMemoryEventsRepository<Guid, Project, Guid>>()
                .AddScoped<IRequestHandler<CreateProjectCommand, CreateProjectCommandResponse>,
                    CreateProjectCommandHandler>()
                .AddScoped<IRequestHandler<PauseProjectCommand, PauseProjectCommandResponse>,
                    PauseProjectCommandHandler>()
                .AddScoped<IRequestHandler<ResumeProjectCommand, ResumeProjectCommandResponse>,
                    ResumeProjectCommandHandler>()
                .AddScoped<IRequestHandler<StartProjectCommand, StartProjectCommandResponse>,
                    StartProjectCommandHandler>();
            ServiceRegistrar.AddRequiredServices(services, serviceConfig);
            Provider = services.BuildServiceProvider();
            CurrentMediator = Provider.GetService<IMediator>();
        }

        public void Dispose()
        {
        }
    }
}