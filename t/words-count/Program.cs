using System;
using System.IO;

namespace words_count
{
    class Program
    {
        private static void mainImpl()
        {
            var rootFolder = Path.GetFullPath(
                Directory.GetCurrentDirectory() + "/../..");

            Console.WriteLine($"words-count: root folder is '{rootFolder}'");

            var wc = WordsCounter.New(rootFolder);
            wc.load("file1.txt", "file2.txt", "file3.txt");

            wc.displayStatus();
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
