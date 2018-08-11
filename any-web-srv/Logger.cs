using System.IO;
using System.Threading;

namespace ThumbnailSrv
{
    interface ILogger
    {
        void Trace(string msg);
    }

    class Logger : ILogger
    {
        private readonly string _path;

        #region construction

        public static ILogger New(string path)
        {
            return 
                new Logger(path);
        }

        private Logger(string path)
        {
            _path = path;
        }

        #endregion

        #region interface

        void ILogger.Trace(string msg)
        {
            for (int retry = 5; retry-- > 0;)
            {
                try
                {
                    File.AppendAllLines(_path, new[] {msg});
                    break;
                }
                catch
                {
                    Thread.Sleep(0);
                }
            }
        }

        #endregion
    }
}