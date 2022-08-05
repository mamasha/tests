using System.Linq.Expressions;

Assert(648, () => ParseRomanNumber("DCXLVIII"));
Assert(2549, () => ParseRomanNumber("MMDXLIX"));
Assert(1944, () => ParseRomanNumber("MCMXLIV"));
Assert(1999, () => ParseRomanNumber("MCMXCIX"));

void Assert(int expected, Func<int> expr)
{
    var result = expr();

    if (result == expected)
        return;

    throw new Exception($"Assertion failed: expected {expected}, got {result}");
}


int ParseRomanNumber(string romanNumber)
{
    var romanDigits = new List<(string, int)> {
        // two letters combination first
        ("IV", 4),
        ("IX", 9),
        ("XL", 40),
        ("XC", 90),
        ("CM", 900),

        // one letters
        ("I", 1),
        ("V", 5),
        ("X", 10),
        ("C", 100),
        ("D", 500),
        ("M", 1000)
    };

    (string?, int) parseNextToken(int from)
    {
        var leftLetters = romanNumber.Substring(from);

        foreach (var digit in romanDigits)
        {
            var (token, value) = digit;

            if (leftLetters.StartsWith(token))
                return digit;
        }

        return (null, -1);
    }

    var number = 0;
    var it = 0;

    for (;;)
    {
        var (token, value) = parseNextToken(it);

        if (token is null)
            throw new Exception($"'{romanNumber}': Unknown token at {it}");

        number += value;
        it += token.Length;

        if (it >= romanNumber.Length)
            break;
    }

    return number;
}