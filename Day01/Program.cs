using System.Diagnostics.Metrics;

internal class Program
{
    private static void Main(string[] args)
    {
        var elves = new List<List<int>>();
        elves.Add(new List<int>());
        foreach (string line in System.IO.File.ReadLines(@"input.txt"))
        {
            if (string.IsNullOrEmpty(line))
            {
                elves.Add(new List<int>());
            }
            else
            {
                elves.Last().Add(int.Parse(line));
            }
        }
        var maxCal = elves
            .Select(x => x.Sum())
            .Max();
        Console.WriteLine("p1:" + maxCal);

        var maxCal3 = elves
            .Select(x => x.Sum())
            .OrderByDescending(x => x)
            .Take(3)
            .Sum();
        Console.WriteLine("p2:" + maxCal3);
    }
}