using System;
using System.IO;

namespace CsvParsers._nunit
{
    class TmpFile : IDisposable
    {
        private readonly string _path;

        public TmpFile(string text)
        {
            _path = Path.GetTempFileName();
            File.WriteAllText(_path, text);
        }

        public string FullPath => _path;

        public void Dispose()
        {
            try
            {
                File.Delete(_path);
            }
            catch { }
        }
    }
}