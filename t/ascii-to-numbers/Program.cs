using System;

namespace ascii_to_numbers
{
    class Program
    {
        private static void mainImpl(string[] args)
        {
            Console.WriteLine("ascii-to-numbers");

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
