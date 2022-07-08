var phrases = new[] {
    "We got it",
    "I feel lucky",
    "Mnoga deneg!"
};

var phrase = phrases[2];
var numOfSets = 14;

var winNums = new int[] { 2, 11, 17, 22, 20, 31 };
var winStrongs = new int[] { 7 };

var loto = new Loto(phrase, numOfSets);

Console.WriteLine(phrase);
Console.WriteLine();

foreach (var set in loto.Sets)
{
    var (strong, nums) = set;

    foreach (var num in nums.Reverse())
        num.ToConsole(winNums.Contains(num));

    Console.Write("  ");
    strong.ToConsole(winStrongs.Contains(strong));

    Console.WriteLine();
//    Console.ReadLine();
}

