using System;
using System.Threading.Tasks;
using MassTransit.ExtensionsDependencyInjectionIntegration;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Quali.Colony.Services.Common.devX_hub
{
    partial class DevXHub
    {
        partial class Startup
        {
            class Gateway : IStartup
            {
                void IStartup.AddDependencies(IServiceCollection services, IConfiguration configuration,
                    Settings settings)
                { }

                void IStartup.AddOne2OneEndpoints(IServiceCollectionBusConfigurator bus, Settings settings)
                { }

                void IStartup.AddOne2ManyEndpoints(IServiceCollection services, Settings settings)
                { }

                void IStartup.AddMiddlewares(IApplicationBuilder app, IConfiguration configuration, Settings settings)
                {
                    app.UseMiddleware<HttpHeaders>();
                }

                void IStartup.AddRoutes(IApplicationBuilder app, IConfiguration configuration, Settings settings)
                { }

                Task IStartup.Start(IServiceProvider di, Settings settings)
                {
                    return Task.CompletedTask;
                }
            }
        }
    }
}
