using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Quali.Colony.Services.Common.devX_hub
{
    partial class DevXHub
    {
        public class AsService : IHostedService
        {
            private static readonly IStartup _devXHub = new Startup();

            private readonly IServiceProvider _di;
            private readonly Settings _settings;

            public AsService(IServiceProvider di, IOptions<Settings> settings)
            {
                _di = di;
                _settings = settings.Value;
            }

            public Task StartAsync(CancellationToken cancellationToken)
            {
                _devXHub.Start(_di, _settings);
                return Task.CompletedTask;
            }

            public Task StopAsync(CancellationToken cancellationToken)
            {
                return Task.CompletedTask;
            }
        }
    }
}
