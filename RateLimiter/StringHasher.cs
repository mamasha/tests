public interface IStringHasher
{
    string GetHashOf(string str);
}

class StringHasher : IStringHasher
{
    public string GetHashOf(string str)
    {
        unchecked
        {
            long hash1 = (5381 << 16) + 5381;
            long hash2 = hash1;

            for (int i = 0; i < str.Length; i += 2)
            {
                hash1 = ((hash1 << 5) + hash1) ^ str[i];
                if (i == str.Length - 1)
                    break;
                hash2 = ((hash2 << 5) + hash2) ^ str[i + 1];
            }

            var hash = hash1 + (hash2 * 1566083941);

            return hash.ToString();
        }
    }
}
