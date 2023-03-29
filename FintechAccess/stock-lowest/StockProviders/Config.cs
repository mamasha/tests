class Config
{
    public TimeSpan TriggerStockSourcesSpan { get; set; } = TimeSpan.FromSeconds(5);
    public string JsonFilePath { get; set; } = "../../../../../stocks.json";
    public string CsvFilePath { get; set; } = "../../../../../stocks.csv";
    public string WebUrl { get; set; } = "https://pastebin.com/raw/C81zsFav";
}
