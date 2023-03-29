using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

public class JsonFileProvider : StockSource
{
    private readonly ILogger _log;
    private readonly string _path;
    private readonly IStockRepository _stockRepository;

    public JsonFileProvider(
        ILogger<JsonFileProvider> log,
        IStockRepository stockRepository)
    {
        var config = new Config();

        _log = log;
        _path = config.JsonFilePath;
        _stockRepository = stockRepository;
    }

    protected override async Task ProvideStocks()
    {
        var json = await File.ReadAllTextAsync(_path);

        var stocks = JsonConvert.DeserializeObject<List<Stock>>(json);
        Debug.Assert(stocks != null);

        _log.LogInformation($"Got {stocks.Count} stocks from {_path}");

        _stockRepository.AddStocks(stocks);
    }
}
