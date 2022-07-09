using Microsoft.AspNetCore.Mvc;

[ApiController]
public class UrlLimiterController : ControllerBase
{ 
    public class ReportRequest
    {
        public string url { get; set; }
    }

    public class ReportResponse
    {
        public bool block { get; set; }
    }

    private readonly IRateLimiter _limiter;

    public UrlLimiterController(IRateLimiter limiter) : base()
    {
        _limiter = limiter;
    }

    [HttpPost("report")]
    public ReportResponse Report([FromBody] ReportRequest request)
    {
        var now = DateTime.UtcNow;
        var blocked = _limiter.LimitUrl(now, request.url);

        return new ReportResponse { 
            block = blocked
        };
    }
}
