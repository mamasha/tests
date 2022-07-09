using System.Collections.Concurrent;

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
    private readonly IStringHasher _hasher;
    private readonly ConcurrentDictionary<string, Ring<DateTime>> _rings;

    public RateLimiter(ILogger<RateLimiter> logger, Config config, IStringHasher hasher)
    {
        _logger = logger;
        _config = config;
        _hasher = hasher;
        _rings = new();
    }

    private (int, bool) LimitByRing(DateTime now, Ring<DateTime> ring)
    {
        while (!ring.IsEmpty && ring.Next + _config.Ttl < now)
            ring.Pop();

        var count = ring.Count;

        if (ring.IsFull)
            return (count, true);

        ring.Push(now);

        return (count, false);
    }

    bool IRateLimiter.LimitUrl(DateTime now, string url)
    {
        var hash = _hasher.GetHashOf(url);
        var ring = _rings.GetOrAdd(hash, _ => new(_config.Threshold));

        bool blocked;
        int count;

        lock (ring)
        {
            (count, blocked) = LimitByRing(now, ring);
        }

        _logger.LogInformation("URL {url} is reported ({hash}), count={count}, {blocked}", url, hash, count, blocked ? "blocked" : "not blocked");

        return blocked;
    }
}
