using Microsoft.Extensions.Logging;
using Quali.Colony.Logging;

namespace Quali.Colony.Services.Common.devX_hub
{
    partial class DevXHub
    {
        public interface ILog : IJsonLogger
        {
        }

        class Log : JsonLogger, ILog
        {
            public Log(ILoggerFactory loggerFactory) :
                base(loggerFactory.CreateLogger(nameof(DevXHub)))
            {
            }
        }
    }
}
