
using Microsoft.Extensions.Logging.Abstractions;
using System.Linq.Expressions;

static class RateLimiterTests
{
    private static readonly TimeSpan sec = TimeSpan.FromSeconds(1);
    private static readonly TimeSpan m10 = TimeSpan.FromMilliseconds(10);
    private static readonly TimeSpan m500 = TimeSpan.FromMilliseconds(500);

    static void Assert(bool shouldBe, Expression<Func<bool>> func)
    {
        if (func.Compile()() != shouldBe)
            throw new ApplicationException($"Assertion failed: {func} => {shouldBe}");
    }

    public static void RunUnitTests()
    {
        var config = new RateLimiter.Config {
            Threshold = 3,
            Ttl = TimeSpan.FromMilliseconds(1000)
        };

        IRateLimiter limiter = new RateLimiter(new NullLogger<RateLimiter>(), config, new StringHasher());

        var now = DateTime.UtcNow;

        Assert(false, () => limiter.LimitUrl(now, "url-1"));
        Assert(false, () => limiter.LimitUrl(now + m10, "url-1"));
        Assert(false, () => limiter.LimitUrl(now + m500, "url-1"));
        Assert(true,  () => limiter.LimitUrl(now + m500 + m10, "url-1"));
        Assert(false, () => limiter.LimitUrl(now + m500, "url-2"));
        Assert(false, () => limiter.LimitUrl(now + sec + m10, "url-1"));
    }
}


