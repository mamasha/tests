using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace ascii_to_numbers
{
    class Program
    {
        private static void ConvertFile(string rootFolder, string inFile, string outFile)
        {
            var inPath = Path.Combine(rootFolder, inFile);
            var outPath = Path.Combine(rootFolder, outFile);

            var lines = File.ReadAllLines(inPath);      // read them all for now; can be read line line if file is big
            var lineCount = lines.Length;

            Console.WriteLine($"Processing '{inFile}' ({lineCount} lines) into '{outFile}'");

            if (lineCount % 4 != 0)
                throw new ApplicationException($"Wrong line count {lineCount} in '{inFile}'; should be modulo of four");

            var numbers = new List<string>();

            for (int next = 0; next < lineCount; )
            {
                var ascii = AsciiNumber.New(8, next);   // number of 8 digits at next line no 

                ascii.PushLine(lines[next++]);          // fragment of three line
                ascii.PushLine(lines[next++]);
                ascii.PushLine(lines[next++]);
                next++;                                 // skip empty line

                var number = ascii.ParsedNumber;

                if (ascii.IsValid == false)
                    number += " INVALID";

                numbers.Add(number);
            }

            Console.WriteLine($"Parsed {numbers.Count} numbers");

            File.WriteAllLines(outPath, numbers);
        }

        private static void mainImpl()
        {
            var rootFolder = Path.GetFullPath(
                Directory.GetCurrentDirectory() + "/../..");

            Console.WriteLine($"ascii-to-numbers: root folder is '{rootFolder}'");

            ConvertFile(rootFolder, "input_Q1a.txt", "output_Q1a.txt");
            ConvertFile(rootFolder, "input_Q1b.txt", "output_Q1b.txt");
        }

        static void Main(string[] args)
        {
            try
            {
                mainImpl();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
