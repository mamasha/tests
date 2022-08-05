var phrases = new[] {
    "We got it",
    "I feel lucky",
    "Mnoga deneg!"
};

var phrase = phrases[2];
var numOfSets = 14;

var winStrongs = new int[] { 
7 
};

var winNums = new int[] {
6,
8,
9,
20,
23,
28
};


var loto = new Loto(phrase, numOfSets);

Console.WriteLine(phrase);
Console.WriteLine();

int no = 0;

foreach (var set in loto.Sets)
{
    Console.Write($"{++no,-4}");
    var (strong, nums) = set;

    foreach (var num in nums.Reverse())
    {
        num.ToConsole(winNums.Contains(num));
    }

    Console.Write("  ");
    strong.ToConsole(winStrongs.Contains(strong));

    Console.WriteLine();
//    Console.ReadLine();
}

