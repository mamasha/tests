using System;
using System.Threading.Tasks;
using MassTransit.ExtensionsDependencyInjectionIntegration;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Quali.Colony.DataAccess;

namespace Quali.Colony.Services.Common.devX_hub
{
    partial class DevXHub
    {
        partial class Startup
        {
            class Authenticator : IStartup
            {
                class One2One :
                    MassTransit.IConsumer<ClientIsHereMsg>
                {
                    private readonly ILog _log;
                    private readonly IAuth _auth;

                    public One2One(ILog log, IAuth auth)
                    {
                        _log = log;
                        _auth = auth;
                    }

                    public Task Consume(MassTransit.ConsumeContext<ClientIsHereMsg> context) => Try.Catch.Handle(
                        () => _auth.ClientIsHere(context.Message),
                        ex => _log.Warn(ex));
                }

                void IStartup.AddDependencies(IServiceCollection services, IConfiguration configuration,
                    Settings settings)
                {
                    services
                        .Configure<PostgresqlSettings>(configuration.GetSection("AppSettings:Postgresql"))
                        .AddTransient<IDbConnectionFactory<PostgresqlSettings>,
                            PostgreSqlConnectionFactory<PostgresqlSettings>>()
                        .AddTransient<IAuth, Auth>();
                }

                void IStartup.AddOne2OneEndpoints(IServiceCollectionBusConfigurator bus, Settings settings)
                {
                    bus
                        .AddConsumer<One2One>();
                }

                void IStartup.AddOne2ManyEndpoints(IServiceCollection services, Settings settings)
                {
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
