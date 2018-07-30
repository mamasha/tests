using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static System.String;

namespace CsvParsers
{
    public class CsvConfig
    {
        public string Header { get; set; }
    }

    public interface ICsvParser : IEnumerable<ICsvRow>
    {
        int Count { get; }
        bool IsEmpty();
        DynamicCsvRow this[int index] { get; }
        IEnumerable<ICsvRow> WhereEqualsI(string name, string value);
        IEnumerable<ICsvRow> WhereEqualsI(string name, int value);
        IEnumerable<ICsvRow> WhereEqualsI(string name, double value);
        IEnumerable<ICsvRow> WhereGreaterThanI(string name, int value);
        IEnumerable<ICsvRow> WhereGreaterThanI(string name, double value);
        IEnumerable<ICsvRow> WhereLessThanI(string name, int value);
        IEnumerable<ICsvRow> WhereLessThanI(string name, double value);
    }

    public class CsvParser : ICsvParser
    {
        #region members

        private readonly string _path;
        private readonly bool _isEmpty;
        private readonly Dictionary<string, int> _columns;

        private int _count = -1;
        private IRandomAccessor _randomAccessor;

        #endregion

        #region construction

        public static ICsvParser New(string path, CsvConfig config = null)
        {
            return
                new CsvParser(path, config ?? new CsvConfig());
        }

        private CsvParser(string path, CsvConfig config)
        {
            var firstLine = CsvHelpers.ReadFirstNotBlankLine(path);    // will throw IoException if file does not exist

            _path = path;
            _isEmpty = firstLine == null;

            var header = config.Header ?? firstLine;
            _columns = CsvHelpers.ParseHeader(_path, header);
        }

        #endregion

        #region private

        private IEnumerable<ICsvRow> where_clause<T>(string name, T value, Func<string, T, bool> match)
        {
            int columnIndex = CsvHelpers.NameToColumnIndex(_columns, name);

            foreach (var line in File.ReadLines(_path))
            {
                if (IsNullOrWhiteSpace(line))
                    continue;

                var field = CsvHelpers.ParseField(line, columnIndex);

                if (IsNullOrWhiteSpace(field))
                    continue;

                if (!match(field, value))
                    continue;

                yield return 
                    CsvRow.New(_columns, line);
            }
        }

        #endregion

        #region interface

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotSupportedException();
        }

        IEnumerator<ICsvRow> IEnumerable<ICsvRow>.GetEnumerator()
        {
            var query = File.ReadLines(_path)
                .Where(line => !IsNullOrWhiteSpace(line))
                .Select(line => CsvRow.New(_columns, line));

            return 
                query.GetEnumerator();
        }

        int ICsvParser.Count
        {
            get
            {
                if (_count < 0)
                    _count = File.ReadLines(_path).Count();

                return _count;
            }
        }

        bool ICsvParser.IsEmpty()
        {
            return _isEmpty;
        }

        DynamicCsvRow ICsvParser.this[int index]
        {
            get
            {
                if (_randomAccessor == null || _randomAccessor.NextLineNo != index)
                    _randomAccessor = RandomAccessor.New(_path, index);

                _randomAccessor.MoveNext();
                var row = CsvRow.New(_columns, _randomAccessor.Current);

                return
                    new DynamicCsvRow(row);
            }
        }

        IEnumerable<ICsvRow> ICsvParser.WhereEqualsI(string name, string value)
        {
            return
                where_clause(name, value, Operand.StringEqual);
        }

        IEnumerable<ICsvRow> ICsvParser.WhereEqualsI(string name, int value)
        {
            return
                where_clause(name, value, Operand.IntEqual);
        }

        IEnumerable<ICsvRow> ICsvParser.WhereEqualsI(string name, double value)
        {
            return
                where_clause(name, value, Operand.DoubleEqual);
        }

        IEnumerable<ICsvRow> ICsvParser.WhereGreaterThanI(string name, int value)
        {
            return
                where_clause(name, value, Operand.IntGreaterThan);
        }

        IEnumerable<ICsvRow> ICsvParser.WhereGreaterThanI(string name, double value)
        {
            return
                where_clause(name, value, Operand.DoubleGreaterThan);
        }

        IEnumerable<ICsvRow> ICsvParser.WhereLessThanI(string name, int value)
        {
            return
                where_clause(name, value, Operand.IntLessThan);
        }

        IEnumerable<ICsvRow> ICsvParser.WhereLessThanI(string name, double value)
        {
            return
                where_clause(name, value, Operand.DoubleLessThan);
        }

        #endregion
    }
}
