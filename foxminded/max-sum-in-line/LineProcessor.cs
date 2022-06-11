using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;

namespace max_sum_in_line
{
    interface ILineProcessor
    {
        int MaxLineNo { get; }
        int[] InvalidLineIndexes { get; }
    }

    class LineProcessor : ILineProcessor
    {
        private readonly int _maxLineNo;
        private readonly int[] _invalidLineIndexes;

        public static ILineProcessor NewFromFile(string path)
        {
            var lines = File.ReadLines(path);

            return
                new LineProcessor(lines);
        }

        public static ILineProcessor New(IEnumerable<string> lines)
        {
            return
                new LineProcessor(lines);
        }

        private static int sumLines(IEnumerable<string> lines, out int[] invalidLines)
        {
            var max = decimal.MinValue;
            var maxLineNo = -1;
            var index = -1;
            var invalidLinesList = new List<int>();
            var hasValidLines = false;

            foreach (var line in lines)
            {
                index++;

                if (string.IsNullOrWhiteSpace(line))
                    continue;

                var split = line.Split(',');
                var sum = 0m;
                var hasInvalidPart = false;

                for (int n = 0; n < split.Length; n++)
                {
                    var part = split[n];
                    var lastPart = n == split.Length - 1;
                    var partIsEmpty = string.IsNullOrWhiteSpace(part);

                    if (partIsEmpty)
                    {
                        hasInvalidPart = !lastPart;
                        break;
                    }

                    if (!decimal.TryParse(part, out var num))
                    {
                        hasInvalidPart = true;
                        break;
                    }

                    sum += num;
                }

                if (hasInvalidPart)
                {
                    invalidLinesList.Add(index);
                    continue;
                }

                hasValidLines = true;

                if (sum > max)
                {
                    maxLineNo = index;
                    max = sum;
                }
            }

            if (hasValidLines == false)
            {
                throw index == -1 ?
                    new ApplicationException("Input is empty") :
                    throw new ApplicationException("No valid lines are found in input");
            }

            invalidLines = invalidLinesList.ToArray();
            return maxLineNo;
        }

        private LineProcessor(IEnumerable<string> lines)
        {
            if (lines == null)
                throw new ArgumentNullException(nameof(lines));

            _maxLineNo = sumLines(lines, out _invalidLineIndexes);
        }

        int ILineProcessor.MaxLineNo => _maxLineNo;
        int[] ILineProcessor.InvalidLineIndexes => _invalidLineIndexes;
    }
}