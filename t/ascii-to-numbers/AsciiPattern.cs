using System;
using System.Linq;

namespace ascii_to_numbers
{
    interface IAsciiPattern
    {
        bool IsValid { get; }
        int Code { get; }
        void PushChar(char ch);
    }

    class AsciiPattern : IAsciiPattern
    {
        private readonly char[][] _validChars;

        private bool _hasWrongChars;
        private int _index;
        private int _code;

        #region construction

        public static IAsciiPattern New(char[][] validChars)
        {
            return
                new AsciiPattern(validChars);
        }

        private AsciiPattern(char[][] validChars)
        {
            _validChars = validChars;
        }

        #endregion

        #region private

        private void validateChar(char ch)
        {
            do
            {
                if (_index >= _validChars.Length)
                    break;

                if (ch == ' ')
                    return;

                var chars = _validChars[_index];

                if (chars.Contains(ch))
                    return;
            }
            while (false);

            _hasWrongChars = true;
        }

        #endregion

        #region interface

        bool IAsciiPattern.IsValid => !_hasWrongChars;

        int IAsciiPattern.Code => _code;

        void IAsciiPattern.PushChar(char ch)
        {
            validateChar(ch);

            var bit =
                ch != ' ' ? 1 : 0;

            _code = 2 * _code + bit;
            _index++;
        }

        #endregion
    }
}