class Set
{
    public int Strong { get; set; }
    public HashSet<int> Nums { get; set; } = new();
    public Queue<int> Queue { get; set; } = new();

    public int Add(int num)
    {
        if (Nums.Contains(num))
            return num;

        Nums.Add(num);
        Queue.Enqueue(num);

        if (Nums.Count <= 6)
            return 0;

        var rem = Queue.Dequeue();
        Nums.Remove(rem);

        return rem;
    }
}

class Loto
{
    private readonly Set[] _sets;

    public Loto(
        string phrase,
        int numOfSets,
        int[]? balls = null,
        int[]? singles = null)
    {
        var rnd = new Random(phrase.EverHash());

        var nums = new int[numOfSets * 6];
        nums.FillWith(balls ?? (1..37).ToArray());
        nums.Shuffle(rnd);

        var strongs = new int[numOfSets];
        strongs.FillWith(singles ?? new int[] { 7, 6, 5, 4, 3, 2, 1 });
        strongs.Shuffle(rnd);

        _sets = new Set[numOfSets];
        for (var i = numOfSets; i-- > 0;)
            _sets[i] = new() { Strong = strongs[i] };

        foreach (var num in nums)
            Add(num);
    }

    public IEnumerable<(int, int[])> Sets =>
        _sets.Select(set => (
            set.Strong,
            set.Nums.OrderBy(n => n).ToArray()
        ));


    private void Add(int num)
    {
        var line = 0;

        for (var maxRetries = 999; ;)
        {
            num = _sets[line++ % _sets.Length].Add(num);

            if (num == 0)
                return;

            if (maxRetries-- == 0)
                throw new InvalidOperationException();
        }
    }
}