namespace ascii_to_numbers
{
    interface IAsiiDigit
    {
        bool IsValid { get; }
        string ParsedDigit { get; }
        void PushChar(char ch);
    }

    class AsciiDigit : IAsiiDigit
    {
        private string _prsedDigit;

        private bool _hasWrongChars;

        #region construction

        public static IAsiiDigit New()
        {
            return
                new AsciiDigit();
        }

        private AsciiDigit()
        { }

        #endregion

        #region digit

        bool IAsiiDigit.IsValid => !_hasWrongChars;

        string IAsiiDigit.ParsedDigit => "1";

        void IAsiiDigit.PushChar(char ch)
        {
        }

        #endregion
    }
}