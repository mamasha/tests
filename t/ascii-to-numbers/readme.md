### ASCII patterns to numbers

The key idea behind a solution was map an ASCII pattern to a number.
Since each pattern has nine characters (positions) and each position has
just two variations, it can be nicely mapped into binary representation of
a number in range 0-512.

[AsciiPattern.cs](./AsciiPattern.cs) is responsible for the binary mapping.
As characters of a pattern are streamed into it a pattern binary code is calculated.

[AsciiNumber.cs](./AsciiNumber.cs) aggregates patterns (digits) into a number.
As strings of a file are streamed it takes care that ASCII patterns receives relevant characters.

[Program.cs](./Program.cs) orchestrates the whole process. First it builds a library
of binary codes of all digits. Then each four lines of a file are fed into an `AsciiNumber` instance.

[UnitTests.cs](./UnitTests.cs) tests each class independently.