static class Helpers
{
    public static int[] ToArray(this Range range) =>
        Enumerable.Range(range.Start.Value, range.End.Value).ToArray();

    public static IEnumerable<int> Exclusive(this Range range)
    {
        for (var n = range.Start.Value; n < range.End.Value; n++)
            yield return n;
    }

    public static void FillWith(this int[] target, int[] nums)
    {
        for (int i = 0; i < target.Length; i++)
            target[i] = nums[i % nums.Length];
    }

    public static void Shuffle(this int[] nums, Random rnd)
    {
        int n = nums.Length;
        while (n > 1)
        {
            var k = rnd.Next(n--);
            (nums[n], nums[k]) = (nums[k], nums[n]);
        }
    }

    public static bool UniqueInLines(this int[] nums, int noOfLines)
    {
        var indx = 0;
        for (var l = 0; l < noOfLines; l++)
        {
            var set = new HashSet<int>();
            foreach (var n in nums[indx..(indx+6)])
            {
                if (set.Contains(n))
                    return false;
                set.Add(n);
                indx++;
            }
        }
        return true;
    }

        public static int EverHash(this string str)
    {
        unchecked
        {
            int hash1 = (5381 << 16) + 5381;
            int hash2 = hash1;

            for (int i = 0; i < str.Length; i += 2)
            {
                hash1 = ((hash1 << 5) + hash1) ^ str[i];
                if (i == str.Length - 1)
                    break;
                hash2 = ((hash2 << 5) + hash2) ^ str[i + 1];
            }

            return hash1 + (hash2 * 1566083941);
        }
    }

    public static void ToConsole(this int n, bool red = false)
    {
        if (red)
            Console.ForegroundColor = ConsoleColor.Red;
        Console.Write($"{n,4}");
        Console.ResetColor();
    }

}

