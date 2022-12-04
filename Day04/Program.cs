using System.Text.RegularExpressions;
internal class Program
{
    private static void Main(string[] args)
    {
        part01();
        part02();
    }

    private static void part02()
    {
        var count = File.ReadLines("input.txt")
            .Select(getAssignments)
            .Where(matches => doesOverlap(matches[0], matches[1], matches[2], matches[3])).Count();
        Console.WriteLine($"part2: {count}");
    }

    private static void part01()
    {
        var count = File.ReadLines("input.txt")
            .Select(getAssignments)
            .Where(matches => doesContain(matches[0], matches[1], matches[2], matches[3])).Count();
        Console.WriteLine($"part2: {count}");
    }

    private static List<int> getAssignments(string line) =>
        new Regex(@"[0-9]+")
            .Matches(line)
            .Select(x => int.Parse(x.Value))
            .ToList();

    private static Boolean doesContain(int r1s, int r1e, int r2s, int r2e) =>
        (r1s <= r2s && r1e >= r2e) || (r1s >= r2s && r1e <= r2e);

    private static Boolean doesOverlap(int r1s, int r1e, int r2s, int r2e) =>
        (r1s >= r2s && r1s <= r2e) || (r2s >= r1s && r2s <= r1e);

}