using System;
using System.Threading.Tasks;
using MassTransit.ExtensionsDependencyInjectionIntegration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Quali.Colony.Redis;

namespace Quali.Colony.Services.Common.devX_hub
{
    partial class DevXHub
    {
        partial class Startup
        {
            class Connector : IStartup
            {
                class One2Many :
                    IConsumer<ClientIsGoneMsg>,
                    IConsumer<PushMsg>
                {
                    private readonly ILog _log;
                    private readonly IIoOut _ioOut;
                    private readonly IHeartbeat _heartbeat;

                    public One2Many(
                        ILog log,
                        IIoOut ioOut,
                        IHeartbeat heartbeat)
                    {
                        _log = log;
                        _ioOut = ioOut;
                        _heartbeat = heartbeat;
                    }

                    private Task clientIsGone(ClientIsGoneMsg msg)
                    {
                        _heartbeat.ClientIsGone(msg.To<Client>());
                        return Task.CompletedTask;
                    }

                    public Task Consume(ConsumeContext<ClientIsGoneMsg> context) => Try.Catch.Handle(
                        () => clientIsGone(context.Message),
                        ex => _log.Warn(ex));

                    public Task Consume(ConsumeContext<PushMsg> context) => Try.Catch.Handle(
                        () => _ioOut.Push(context.Message),
                        ex => _log.Warn(ex));
                }

                void IStartup.AddDependencies(IServiceCollection services, IConfiguration configuration,
                    Settings settings)
                {
                    services
                        .AddTransient<IIoOut, IoOut>()
                        // stateful singletons
                        .AddSingleton<IIoRepo, IoRepo>()
                        .AddSingleton<IHeartbeat, Heartbeat>(); // state of intervals
                }

                void IStartup.AddOne2OneEndpoints(IServiceCollectionBusConfigurator bus, Settings settings)
                {
                }

                void IStartup.AddOne2ManyEndpoints(IServiceCollection services, Settings settings)
                {
                    services
                        .AddConsumer<One2Many, ClientIsGoneMsg>()
                        .AddConsumer<One2Many, PushMsg>();
                }

                void IStartup.AddMiddlewares(IApplicationBuilder app, IConfiguration configuration, Settings settings)
                {
                }

                void IStartup.AddRoutes(IApplicationBuilder app, IConfiguration configuration, Settings settings)
                {
                    app.UseEndpoints(endpoints =>
                    {
                        endpoints.MapHub<IoIn>(settings.BaseUrl, options =>
                        {
                            options.LongPolling.PollTimeout = settings.PollTimeout;
                            options.Transports = HttpTransports.All;
                        });
                        endpoints.MapControllers();
                    });
                }

                Task IStartup.Start(IServiceProvider di, Settings settings)
                {
                    var heartbeat = di.GetService<IHeartbeat>();

                    heartbeat.Start();

                    return Task.CompletedTask;
                }
            }
        }
    }
}
