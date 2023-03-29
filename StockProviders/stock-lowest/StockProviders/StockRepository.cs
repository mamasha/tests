using System.Collections.Concurrent;

public interface IStockRepository
{
    void AddStocks(IEnumerable<Stock> stocks);
    Stock? GetLowestPrice(string stockName);
    Stock[] GetAllLowestPrices();
}

public class StockRepository : IStockRepository
{
    class StockBox
    {
        private Stock _value;

        public StockBox(Stock value)
        {
            _value = value;
        }

        public Stock Value
        {
            get
            {
                lock (this)
                    return _value;
            }

            set
            {
                lock (this)
                {
                    if (_value.price <= value.price)
                        return;

                    _value = value;
                }
            }
        }
    }

    private readonly ConcurrentDictionary<string, StockBox> _stocks;

    public StockRepository()
    {
        _stocks = new();
    }

    void IStockRepository.AddStocks(IEnumerable<Stock> stocks)
    {
        foreach (var stock in stocks)
        {
            var box = _stocks.GetOrAdd(stock.name, _ => new StockBox(stock));
            box.Value = stock;
        }
    }

    Stock? IStockRepository.GetLowestPrice(string stockName)
    {
        if (!_stocks.TryGetValue(stockName, out var box))
            return null;

        return box.Value;
    }

    Stock[] IStockRepository.GetAllLowestPrices()
    {
        var query =
            _stocks.Select(box => box.Value.Value);

        return query.ToArray();
    }
}
