using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services
           .AddHostedService<StockScheduler>()
           .AddSingleton<IStockRepository, StockRepository>()
           .AddTransient<IStockSource, JsonFileProvider>()
           .AddTransient<IStockSource, JsonFileProvider>()
           .AddTransient<IStockSource, WebProvider>();
    })
    .Build();

await host.RunAsync();
