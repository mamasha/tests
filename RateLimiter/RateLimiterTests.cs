
using Microsoft.Extensions.Logging.Abstractions;
using System.Linq.Expressions;

static class RateLimiterTests
{
    private static readonly TimeSpan sec = TimeSpan.FromSeconds(1);
    private static readonly TimeSpan m10 = TimeSpan.FromMilliseconds(10);
    private static readonly TimeSpan m500 = TimeSpan.FromMilliseconds(500);

    static void Assert(Expression<Func<bool>> func, bool shouldBe)
    {
        if (func.Compile()() != shouldBe)
            throw new ApplicationException($"Assertion failed: {func} => {shouldBe}");
    }

    public static void RunUnitTests()
    {
        IRateLimiter limiter = new RateLimiter(new NullLogger<RateLimiter>(), new RateLimiter.Config { 
            Threshold = 3,
            Ttl = TimeSpan.FromMilliseconds(1000)
        });

        var now = DateTime.UtcNow;

        Assert(() => limiter.LimitUrl(now, "url-1"), false);
        Assert(() => limiter.LimitUrl(now + m10, "url-1"), false);
        Assert(() => limiter.LimitUrl(now + m500, "url-1"), false);
        Assert(() => limiter.LimitUrl(now + m500 + m10, "url-1"), true);
        Assert(() => limiter.LimitUrl(now + m500, "url-2"), false);
        Assert(() => limiter.LimitUrl(now + sec + m10, "url-1"), false);
    }
}


