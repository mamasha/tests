using System;
using System.IO;

namespace max_sum_in_line
{
    class Program
    {
        private static void processFile(string path)
        {

        }

        private static void mainImpl(string[] args)
        {
            var path = args.Length > 0 ?  args[0] : null;

            if (string.IsNullOrEmpty(path))
            {
                Console.Write("Please enter file to sum: ");
                path = Console.ReadLine();
            }

            if (!File.Exists(path))
            {
                Console.WriteLine($"File '{path}' is not found");
                return;
            }

            Console.WriteLine($"Processing '{path}'...");

            var processor = LineProcessor.NewFromFile(path);

            var maxLineNo = processor.MaxLineNo;
            var invalidIndexes = string.Join(',', processor.InvalidLineIndexes);

            Console.WriteLine($"Index of line with max sum: {maxLineNo}");
            Console.WriteLine($"Invalid line indexes: [{invalidIndexes}]");
        }

        static void Main(string[] args)
        {
            try
            {
                mainImpl(args);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}
