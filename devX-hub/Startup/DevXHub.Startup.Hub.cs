using System;
using System.Threading.Tasks;
using MassTransit.ExtensionsDependencyInjectionIntegration;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Quali.Colony.Redis;

namespace Quali.Colony.Services.Common.devX_hub
{
    partial class DevXHub
    {
        partial class Startup
        {
            class Hub : IStartup
            {
                class One2One :
                    MassTransit.IConsumer<NotifyMsg>
                {
                    private readonly ILog _log;
                    private readonly IRelay _relay;

                    public One2One(ILog log, IRelay relay)
                    {
                        _log = log;
                        _relay = relay;
                    }

                    public Task Consume(MassTransit.ConsumeContext<NotifyMsg> context) => Try.Catch.Handle(
                        () => _relay.Notify(context.Message),
                        ex => _log.Warn(ex));
                }

                class One2Many :
                    IConsumer<UserIsHereMsg>,
                    IConsumer<ClientIsGoneMsg>,
                    IConsumer<MapSessionMsg>,
                    IConsumer<MapFlowMsg>
                {
                    private readonly ILog _log;
                    private readonly IRelay _relay;

                    public One2Many(ILog log, IRelay relay)
                    {
                        _log = log;
                        _relay = relay;
                    }

                    public Task Consume(ConsumeContext<UserIsHereMsg> context) => Try.Catch.Handle(
                        () => _relay.UserIsHere(context.Message),
                        ex => _log.Warn(ex));

                    public Task Consume(ConsumeContext<ClientIsGoneMsg> context) => Try.Catch.Handle(
                        () => _relay.ClientIsGone(context.Message),
                        ex => _log.Warn(ex));

                    public Task Consume(ConsumeContext<MapSessionMsg> context) => Try.Catch.Handle(
                        () => _relay.MapSession(context.Message),
                        ex => _log.Warn(ex));

                    public Task Consume(ConsumeContext<MapFlowMsg> context) => Try.Catch.Handle(
                        () => _relay.MapFlow(context.Message),
                        ex => _log.Warn(ex));
                }

                void IStartup.AddDependencies(IServiceCollection services, IConfiguration configuration,
                    Settings settings)
                {
                    services
                        .AddSingleton<IRelayRepo, RelayRepo>()
                        .AddTransient<IRelay, Relay>();
                }

                void IStartup.AddOne2OneEndpoints(IServiceCollectionBusConfigurator bus, Settings settings)
                {
                    bus
                        .AddConsumer<One2One>();
                }

                void IStartup.AddOne2ManyEndpoints(IServiceCollection services, Settings settings)
                {
                    services
                        .AddConsumer<One2Many, UserIsHereMsg>()
                        .AddConsumer<One2Many, ClientIsGoneMsg>()
                        .AddConsumer<One2Many, MapSessionMsg>()
                        .AddConsumer<One2Many, MapFlowMsg>();
                }

                void IStartup.AddMiddlewares(IApplicationBuilder app, IConfiguration configuration, Settings settings)
                {
                }

                void IStartup.AddRoutes(IApplicationBuilder app, IConfiguration configuration, Settings settings)
                {
                }

                Task IStartup.Start(IServiceProvider di, Settings settings)
                {
                    return Task.CompletedTask;
                }
            }
        }
    }
}
