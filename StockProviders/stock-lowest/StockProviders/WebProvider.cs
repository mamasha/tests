using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

public class WebProvider : StockSource
{
    private readonly ILogger _log;
    private readonly string _url;
    private readonly IStockRepository _stockRepository;

    public WebProvider(
        ILogger<WebProvider> log,
        IStockRepository stockRepository)
    {
        var config = new Config();

        _log = log;
        _url = config.WebUrl;
        _stockRepository = stockRepository;
    }

    protected override async Task ProvideStocks()
    {
        using var client = new HttpClient();
        var content = await client.GetStringAsync(_url);

        var stocks = JsonConvert.DeserializeObject<List<Stock>>(content);
        Debug.Assert(stocks is not null);

        _log.LogInformation($"Got {stocks.Count} stocks from {_url}");

        _stockRepository.AddStocks(stocks);
    }
}
