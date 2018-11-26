using NUnit.Framework;

namespace ascii_to_numbers
{
    [TestFixture]
    class UnitTests
    {
        [Test]
        public void Digit_patterns_are_binary_coded()
        {
            var patterns = Program.DigitPatterns;
            var validChars = Program.ValidChars;

            void assert(string digit, int code)
            {
                var pattern = AsciiPattern.New(validChars);
                foreach (var ch in digit) {
                    pattern.PushChar(ch);
                }
                Assert.That(pattern.Code, Is.EqualTo(code));
            }

            assert(patterns[0], 0b010_101_111);
            assert(patterns[1], 0b000_001_001);
            assert(patterns[2], 0b010_011_110);
            assert(patterns[3], 0b010_011_011);
            assert(patterns[4], 0b000_111_001);
            assert(patterns[5], 0b010_110_011);
            assert(patterns[6], 0b010_110_111);
            assert(patterns[7], 0b010_001_001);
            assert(patterns[8], 0b010_111_111);
            assert(patterns[9], 0b010_111_011);
        }

        [Test]
        public void parse_line_of_all_digits()
        {
            var patterns = Program.DigitPatterns;
            var validChars = Program.ValidChars;

            var digitCodes = Program.BuildDigitCodes(patterns);

            var config = new AsciiNumber.Config {
                NoOfDigits = 10,
                ValidChars = validChars,
                DigitCodes = digitCodes
            };

            var ascii = AsciiNumber.New(config, 0);

            ascii.PushLine("    _  _     _  _  _  _  _  _ ");
            ascii.PushLine("  | _| _||_||_ |_   ||_||_|| |");
            ascii.PushLine("  ||_  _|  | _||_|  ||_| _||_|");

            var number = ascii.Number;
            Assert.That(number, Is.EqualTo("1234567890"));
        }

        [Test]
        public void parse_wrong_patterns()
        {
            var patterns = Program.DigitPatterns;
            var validChars = Program.ValidChars;

            var digitCodes = Program.BuildDigitCodes(patterns);

            var config = new AsciiNumber.Config
            {
                NoOfDigits = 10,
                ValidChars = validChars,
                DigitCodes = digitCodes
            };

            var ascii = AsciiNumber.New(config, 0);

            ascii.PushLine("       _     _  _  _  _  _  _ ");
            ascii.PushLine("  | _| _||_||_ |_   ||_ |_|| |");
            ascii.PushLine("  ||_  _|  | _ |_|  || | _||_|");

            var number = ascii.Number;
            Assert.That(number, Is.EqualTo("1?34?67?90"));
        }
    }
}