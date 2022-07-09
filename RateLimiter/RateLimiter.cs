public interface IRateLimiter
{
    bool LimitUrl(DateTime now, string url);
}

public class RateLimiter : IRateLimiter
{
    public class Config
    {
        public int Threshold { get; set; }
        public TimeSpan Ttl { get; set; }
    }

    private readonly ILogger _logger;
    private readonly Config _config;

    public RateLimiter(ILogger<RateLimiter> logger, Config config)
    {
        _logger = logger;
        _config = config;
    }

    bool IRateLimiter.LimitUrl(DateTime now, string url)
    {
        _logger.LogInformation("Limitting '{url}'", url);
        return false;
    }
}


