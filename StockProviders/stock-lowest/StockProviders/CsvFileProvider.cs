using System.Diagnostics;
using Microsoft.Extensions.Logging;

public class CsvFileProvider : StockSource
{
    private readonly ILogger _log;
    private readonly string _path;
    private readonly IStockRepository _stockRepository;

    public CsvFileProvider(
        ILogger<CsvFileProvider> log,
        IStockRepository stockRepository)
    {
        var config = new Config();

        _log = log;
        _path = config.CsvFilePath;
        _stockRepository = stockRepository;
    }

    private Stock? ParseLine(string line, int lineNo)
    {
        try
        {
            var parts = line.Split(",");

            var name = parts[0];
            var price = decimal.Parse(parts[2]);

            return new Stock(name, price);
        }
        catch (Exception ex)
        {
            _log.LogError($"Failed to parse stock at line {lineNo} ({ex.Message})");
            return null;
        }
    }

    protected override async Task ProvideStocks()
    {
        var lines = await File.ReadAllLinesAsync(_path);

        var transform = lines
           .Skip(1)                                     // skip header line
           .Where(line => !string.IsNullOrWhiteSpace(line))
           .Select(ParseLine)
           .Where(stock => stock.HasValue)
           .Select(x => x!.Value);

        var stocks = transform.ToArray();
        Debug.Assert(stocks is not null);

        _log.LogInformation($"Got {stocks.Length} stocks from {_path}");

        _stockRepository.AddStocks(stocks);
    }
}
