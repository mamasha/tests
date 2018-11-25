using System;

namespace words_count
{
    class Program
    {
        private static void mainImpl(string[] args)
        {
            Console.WriteLine("words-count");
        }

        static void Main(string[] args)
        {
            try
            {
                mainImpl(args);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
