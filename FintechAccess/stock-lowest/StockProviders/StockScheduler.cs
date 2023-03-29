using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

public class StockScheduler : IHostedService
{
    private readonly ILogger _log;
    private readonly IServiceProvider _di;
    private readonly IStockRepository _stocks;

    public StockScheduler(
        ILogger<StockScheduler> log,
        IServiceProvider di,
        IStockRepository stocks)
    {
        _log = log;
        _di = di;
        _stocks = stocks;
    }

    Task IHostedService.StartAsync(CancellationToken cancellationToken) =>
        Task.Run(() => Main(cancellationToken), cancellationToken);

    Task IHostedService.StopAsync(CancellationToken cancellationToken) =>
        Task.CompletedTask;

    private async Task Trigger()
    {
        async Task Invoke(IStockSource source)
        {
            try
            {
                await source.ProvideStocks();
            }
            catch (Exception ex)
            {
                _log.LogError(ex.Message);
            }
        }

        var sources = _di.GetServices<IStockSource>();

        foreach (var source in sources)
        {
            await Invoke(source);
        }

        var stocks = _stocks.GetAllLowestPrices();
        _log.LogInformation($"There are {stocks.Length} stocks in repository");
    }

    private async Task Main(CancellationToken cancellationToken)
    {
        var config = new Config();

        for (;;)
        {
            if (cancellationToken.IsCancellationRequested)
                break;

            try
            {
                await Trigger();
                await Task.Delay(config.TriggerStockSourcesSpan, cancellationToken);
            }
            catch (TaskCanceledException)
            {}
        }
    }
}
