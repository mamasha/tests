using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace CsvParsers
{
    interface IRandomAccessor : IEnumerator<string>
    {
        int NextLineNo { get; }
    }

    class RandomAccessor : IRandomAccessor
    {
        private readonly string _path;
        private readonly IEnumerator<string> _itLines;

        private string _current;
        private int _nextLineNo;

        #region construction

        public static IRandomAccessor New(string path, int startFrom)
        {
            return
                new RandomAccessor(path, startFrom);
        }

        private RandomAccessor(string path, int startFrom)
        {
            _path = path;
            _itLines = File.ReadLines(path).GetEnumerator();

            for (int lineNo = 0; lineNo < startFrom; lineNo++)
            {
                if (!_itLines.MoveNext())
                    throw new IndexOutOfRangeException($"File '{path}' has only {lineNo} lines, while line number {startFrom} is being accessed");

                _nextLineNo++;
            }
        }

        #endregion

        #region interface

        void IDisposable.Dispose()
        {
        }

        bool IEnumerator.MoveNext()
        {
            if (!_itLines.MoveNext())
                throw new IndexOutOfRangeException($"File '{_path}' has only {_nextLineNo} lines, while line number {_nextLineNo+1} is being accessed");

            _current = _itLines.Current;
            _nextLineNo++;

            return true;
        }

        void IEnumerator.Reset()
        {
            throw new NotSupportedException();
        }

        string IEnumerator<string>.Current => _current;
        object IEnumerator.Current => _current;
        int IRandomAccessor.NextLineNo => _nextLineNo;

        #endregion
    }
}