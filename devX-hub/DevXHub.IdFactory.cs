using System;
using System.IO;
using System.Threading;
using Microsoft.Extensions.Options;

namespace Quali.Colony.Services.Common.devX_hub
{
    partial class DevXHub
    {
        public interface IIdFactory
        {
            string GetNextId<T>();
            string GetNextId(string prefix);
        }

        class IdFactory : IIdFactory
        {
            private readonly string _host;
            private readonly string _process;
            private readonly string _suffix;

            private int _nextId;

            public IdFactory(IOptions<Settings> settings)
            {
                var config = settings.Value;

                _host = Path.GetFileName(Environment.MachineName);
                _process = Path.GetFileName(global::System.Diagnostics.Process.GetCurrentProcess().ProcessName);
                _suffix = Path.GetRandomFileName().Substring(0, config.IdRandomSuffixLength);
            }

            private string getNextId(string prefix)
            {
                var nextId = Interlocked.Increment(ref _nextId);
                return $"{prefix}.{_process}.{_host}.{_suffix}.{nextId}";
            }

            string IIdFactory.GetNextId<T>()
            {
                return getNextId(typeof(T).Name);
            }

            string IIdFactory.GetNextId(string prefix)
            {
                return getNextId(prefix);
            }
        }
    }
}
