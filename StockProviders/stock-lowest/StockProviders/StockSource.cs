public interface IStockSource
{
    Task ProvideStocks();
}

public abstract class StockSource : IStockSource
{
    protected abstract Task ProvideStocks();

    Task IStockSource.ProvideStocks() =>
        ProvideStocks();
}
