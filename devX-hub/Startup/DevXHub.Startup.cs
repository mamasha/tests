using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MassTransit.ExtensionsDependencyInjectionIntegration;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Quali.Colony.Services.Common.devX_hub
{
    partial class DevXHub
    {
        internal interface IStartup
        {
            void AddDependencies(IServiceCollection services, IConfiguration configuration, Settings settings);
            void AddOne2OneEndpoints(IServiceCollectionBusConfigurator bus, Settings settings);
            void AddOne2ManyEndpoints(IServiceCollection services, Settings settings);
            void AddMiddlewares(IApplicationBuilder app, IConfiguration configuration, Settings settings);
            void AddRoutes(IApplicationBuilder app, IConfiguration configuration, Settings settings);
            Task Start(IServiceProvider di, Settings settings);
        }

        internal partial class Startup : IStartup
        {
            private readonly Dictionary<As, IStartup> _hosts = new Dictionary<As, IStartup> {
                { As.Api, new Null() },
                { As.Connector, new Connector() },
                { As.Auth, new Authenticator() },
                { As.Hub, new Hub() },
                { As.Gateway, new Gateway() }
            };

            void IStartup.AddDependencies(IServiceCollection services, IConfiguration configuration, Settings settings)
            {
                ConfigureAutoMapper();

                services
                    .AddHostedService<AsService>()
                    .AddSingleton<IOptions<Settings>>(new OptionsWrapper<Settings>(settings))
                    .AddSingleton<IIdFactory, IdFactory>()
                    .AddSingleton<IRelayRepo, RelayRepo.Null>()
                    .AddSingleton<IIoRepo, IoRepo.Null>()
                    .AddTransient<IDevXHub, DevXHub>()
                    .AddTransient<ILog, Log>()
                    .AddTransient<IBus, Bus>()
                    .AddTransient<IRelay, Relay.Null>()
                    .AddTransient<IIoOut, IoOut.Null>()
                    .AddTransient<IAuth, Auth.Null>();

                _hosts[settings.StartAs].AddDependencies(services, configuration, settings);
            }

            void IStartup.AddOne2OneEndpoints(
                IServiceCollectionBusConfigurator bus,
                Settings settings)
            {
                _hosts[settings.StartAs].AddOne2OneEndpoints(bus, settings);
            }

            void IStartup.AddOne2ManyEndpoints(IServiceCollection services, Settings settings)
            {
                _hosts[settings.StartAs].AddOne2ManyEndpoints(services, settings);
            }

            void IStartup.AddMiddlewares(IApplicationBuilder app, IConfiguration configuration, Settings settings)
            {
                _hosts[settings.StartAs].AddMiddlewares(app, configuration, settings);
            }

            void IStartup.AddRoutes(IApplicationBuilder app, IConfiguration configuration, Settings settings)
            {
                // NOTE: We assume here that
                //      app.UseRouting() is already called
                //      SignalR is configured with Redis backplane

                _hosts[settings.StartAs].AddRoutes(app, configuration, settings);
            }

            async Task IStartup.Start(IServiceProvider di, Settings settings)
            {
                ILog log = new Log(di.GetService<ILoggerFactory>());

                await _hosts[settings.StartAs].Start(di, settings);

                log.Info($"Started", settings);
            }

            class Null : IStartup
            {
                void IStartup.AddDependencies(IServiceCollection services, IConfiguration configuration, Settings settings) { }
                void IStartup.AddOne2OneEndpoints(IServiceCollectionBusConfigurator bus, Settings settings) { }
                void IStartup.AddOne2ManyEndpoints(IServiceCollection services, Settings settings) { }
                void IStartup.AddMiddlewares(IApplicationBuilder app, IConfiguration configuration, Settings settings) { }
                void IStartup.AddRoutes(IApplicationBuilder app, IConfiguration configuration, Settings settings) { }
                Task IStartup.Start(IServiceProvider di, Settings settings) { return Task.CompletedTask; }
            }

        }
    }
}
