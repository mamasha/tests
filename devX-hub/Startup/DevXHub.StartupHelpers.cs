using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using MassTransit.ExtensionsDependencyInjectionIntegration;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Quali.Colony.Services.Common.devX_hub
{
    public static class DevXHubStartupHelpers
    {
        private static readonly DevXHub.IStartup _devXHub = new DevXHub.Startup();

        private static DevXHub.Settings makeSettings(
            IConfiguration configuration,
            Action<DevXHub.Settings> configMe)
        {
            var settings =
                configuration.GetSection("AppSettings:DevXHub").Get<DevXHub.Settings>() ??
                new DevXHub.Settings();

            configMe?.Invoke(settings);

            return settings;
        }

        public static IServiceCollection AddDevXHubDependencies(
            this IServiceCollection services,
            IConfiguration configuration,
            Action<DevXHub.Settings> configMe = null)
        {
            var settings = makeSettings(configuration, configMe);
            _devXHub.AddDependencies(services, configuration, settings);
            _devXHub.AddOne2ManyEndpoints(services, settings);
            return services;
        }

        public static void AddDevXHubRabbitConsumers(
            this IServiceCollectionBusConfigurator bus,
            IConfiguration configuration,
            Action<DevXHub.Settings> configMe = null)
        {
            var settings = makeSettings(configuration, configMe);
            _devXHub.AddOne2OneEndpoints(bus, settings);
        }

        public static IApplicationBuilder AddDevXHubMiddlewares(
            this IApplicationBuilder app,
            IConfiguration configuration)
        {
            var settings = app.ApplicationServices.GetService<IOptions<DevXHub.Settings>>();
            _devXHub.AddMiddlewares(app, configuration, settings.Value);
            return app;
        }

        public static IApplicationBuilder AddDevXHubRoutes(
            this IApplicationBuilder app,
            IConfiguration configuration)
        {
            var settings = app.ApplicationServices.GetService<IOptions<DevXHub.Settings>>();
            _devXHub.AddRoutes(app, configuration, settings.Value);
            return app;
        }

        public static void StartDevXHub(IServiceProvider serviceProvider)
        {
            var services = serviceProvider.GetRequiredService<IEnumerable<IHostedService>>();
            var devXHub = services.First(x => x.GetType() == typeof(DevXHub.AsService));

            devXHub.StartAsync(CancellationToken.None);
        }

    }
}
