using System.Collections.Generic;
using System.Linq;

namespace CsvParsers
{
    public interface ICsvRow
    {
        string this[string name] { get; }
        string this[int index] { get; }
    }

    class CsvRow : ICsvRow
    {
        #region members

        private readonly Dictionary<string, int> _columns;
        private readonly string[] _fields;

        #endregion

        #region construction

        public static ICsvRow New(Dictionary<string, int> columns, string line)
        {
            return
                new CsvRow(columns, line);
        }

        private CsvRow(Dictionary<string, int> columns, string line)
        {
            _columns = columns;

            var split =
                from field in line.Split(',')
                select field.Trim();

            _fields = split.ToArray();
        }

        #endregion

        #region interface

        string ICsvRow.this[string name]
        {
            get
            {
                int index = CsvHelpers.NameToColumnIndex(_columns, name);

                if (index >= _fields.Length)
                    return null;

                return _fields[index];
            }
        }

        string ICsvRow.this[int index]
        {
            get
            {
                if (index >= _fields.Length)
                    return null;

                return _fields[index];
            }
        }

        #endregion
    }
}