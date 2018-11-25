using System;
using System.Collections.Generic;
using System.Text;

namespace ascii_to_numbers
{
    interface IAsciiNumber
    {
        bool IsValid { get; }
        string ParsedNumber { get; }
        void PushLine(string line);
    }

    class AsciiNumber : IAsciiNumber
    {
        #region members

        private readonly int _noOfDigits;
        private readonly int _lineNo;
        private readonly IAsiiDigit[] _digits;

        #endregion

        #region construction

        public static IAsciiNumber New(int noOfDigits, int lineNo)
        {
            return
                new AsciiNumber(noOfDigits, lineNo);
        }

        private AsciiNumber(int noOfDigits, int lineNo)
        {
            _noOfDigits = noOfDigits;
            _lineNo = lineNo;
            _digits = new IAsiiDigit[noOfDigits];

            for (int i = 0; i < noOfDigits; i++)
            {
                _digits[i] = AsciiDigit.New();
            }
        }

        #endregion

        #region interface

        bool IAsciiNumber.IsValid
        {
            get
            {
                foreach (var digit in _digits)
                {
                    if (!digit.IsValid)
                        return false;
                }

                return true;
            }
        }

        string IAsciiNumber.ParsedNumber
        {
            get
            {
                var sb = new StringBuilder();

                foreach (var digit in _digits)
                {
                    sb.Append(digit.ParsedDigit);
                }

                return sb.ToString();
            }
        }

        void IAsciiNumber.PushLine(string line)
        {
            var charCount = 3 * _noOfDigits;

            if (line.Length < charCount)
                throw new ApplicationException($"Not enough characters ({line.Length}) in line {_lineNo}; should be at least {charCount} characters");

            var next = 0;

            foreach (var digit in _digits)
            {
                digit.PushChar(line[next++]);       // a line has groups of three characters each per digit
                digit.PushChar(line[next++]);
                digit.PushChar(line[next++]);
            }
        }

        #endregion
    }
}