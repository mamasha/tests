[TestClass]
public class StockRepositoryTests
{
    [TestMethod]
    public void It_should_return_min_price()
    {
        IStockRepository repo = new StockRepository();

        repo.AddStocks(new []
        {
            new Stock("AAA", 1.23m),
            new Stock("Bb", 1.23m),
            new Stock("AAA", 1.3m),
            new Stock("Bb", 1.1m),
        });

        var aaa = repo.GetLowestPrice("AAA");
        Assert.IsNotNull(aaa);
        Assert.Equals(aaa.Value.price, 1.23d);

        var bb = repo.GetLowestPrice("Bb");
        Assert.IsNotNull(bb);
        Assert.Equals(bb.Value.price, 1.1d);
    }
}
