using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Quali.Colony.Services.Common.devX_hub
{
    partial class DevXHub
    {
        class HttpHeaders
        {
            private readonly RequestDelegate _next;
            private readonly ILog _log;
            private readonly Settings _settings;
            private readonly IServiceProvider _di;

            public HttpHeaders(
                RequestDelegate next,
                ILog log,
                IOptions<Settings> settings,
                IServiceProvider di)
            {
                _next = next;
                _log = log;
                _settings = settings.Value;
                _di = di;
            }

            private async Task startSession(HttpContext http)
            {
                var headers = http.Request.Headers;

                var sessionIsHere =
                    headers.TryGetValue(_settings.SessionIdHeader, out var sessionIds);

                if (!sessionIsHere)
                    return;

                var sessionId = sessionIds.First();

                using var di = _di.CreateScope();
                var devXHub = di.ServiceProvider.GetService<IDevXHub>();

                await devXHub.MapMySession(sessionId);
            }

            public async Task Invoke(HttpContext http)
            {
                await Try.Catch.Handle(() => startSession(http),
                    ex => _log.Warn(ex));

                await _next.Invoke(http);
            }
        }
    }
}
