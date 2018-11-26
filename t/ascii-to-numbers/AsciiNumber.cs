using System;
using System.Collections.Generic;
using System.Text;

namespace ascii_to_numbers
{
    interface IAsciiNumber
    {
        bool IsValid { get; }
        string Number { get; }
        void PushLine(string line);
    }

    class AsciiNumber : IAsciiNumber
    {
        #region config

        public class Config
        {
            public int NoOfDigits { get; set; }
            public char[][] ValidChars { get; set; }
            public int[] DigitCodes { get; set; }
        }

        #endregion

        #region members

        private readonly Config _config;
        private readonly int _lineNo;
        private readonly IAsciiPattern[] _patterns;

        #endregion

        #region construction

        public static IAsciiNumber New(Config config, int lineNo)
        {
            return
                new AsciiNumber(config, lineNo);
        }

        private AsciiNumber(Config config, int lineNo)
        {
            _config = config;
            _lineNo = lineNo;
            _patterns = new IAsciiPattern[config.NoOfDigits];

            for (int i = 0; i < config.NoOfDigits; i++)
            {
                _patterns[i] = AsciiPattern.New(config.ValidChars);
            }
        }

        #endregion

        #region private

        private string recognize(int code)
        {
            for (int digit = 0; digit < 10; digit++)
            {
                if (_config.DigitCodes[digit] == code)
                    return digit.ToString();
            }

            return "?";
        }

        #endregion

        #region interface

        bool IAsciiNumber.IsValid
        {
            get
            {
                foreach (var pattern in _patterns)
                {
                    if (!pattern.IsValid)
                        return false;

                    var digit = recognize(pattern.Code);

                    if (digit == "?")
                        return false;
                }

                return true;
            }
        }

        string IAsciiNumber.Number
        {
            get
            {
                var sb = new StringBuilder();

                foreach (var pattern in _patterns)
                {
                    var digit = recognize(pattern.Code);
                    sb.Append(digit);
                }

                return sb.ToString();
            }
        }

        void IAsciiNumber.PushLine(string line)
        {
            var charCount = 3 * _config.NoOfDigits;

            if (line.Length < charCount)
                throw new ApplicationException($"Not enough characters ({line.Length}) in line {_lineNo}; should be at least {charCount} characters");

            var next = 0;

            foreach (var pattern in _patterns)
            {
                pattern.PushChar(line[next++]);       // a line has groups of three characters each per digit
                pattern.PushChar(line[next++]);
                pattern.PushChar(line[next++]);
            }
        }

        #endregion
    }
}