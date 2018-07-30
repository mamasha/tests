using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static System.String;

namespace CsvParsers
{
    static class CsvHelpers
    {
        public static string ReadFirstNotBlankLine(string path)
        {
            return
                File.ReadLines(path)
                    .FirstOrDefault(line => !IsNullOrWhiteSpace(line));
        }

        public static Dictionary<string, int> ParseHeader(string path, string header)
        {
            if (IsNullOrWhiteSpace(header))
                throw new NotSupportedException($"First line of '{path}' is empty and no header is found in configuration");

            var columns = new Dictionary<string, int>(StringComparer.InvariantCultureIgnoreCase);
            int index = 0;

            foreach (var field in header.Split(','))
            {
                var name = field.Trim();

                if (IsNullOrWhiteSpace(name))
                    throw new NotSupportedException($"An empty column {index} in header ({header})");

                if (columns.ContainsKey(name))
                    throw new NotSupportedException($"A same column '{name}' in header ({header})");

                columns.Add(name, index);
                index++;
            }

            return columns;
        }

        public static int NameToColumnIndex(Dictionary<string, int> columns, string name)
        {
            int index;

            if (columns.TryGetValue(name, out index))
                return index;

            throw new KeyNotFoundException($"Can't find column name '{name}' in header ({columns})");
        }

        public static string ParseField(string line, int columnIndex)
        {
            var fields = line.Split(',');

            if (columnIndex >= fields.Length)
                return null;

            var field = fields[columnIndex].Trim();

            return field;
        }

    }
}