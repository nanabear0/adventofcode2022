internal class Program
{
    private static void Main(string[] args)
    {
        part01();
        part02();
    }

    private static void part02()
    {
        var sum = File.ReadLines("input.txt")
            .Chunk(3)
            .Select(elves => elves[0].Intersect(elves[1]).Intersect(elves[2]).First())
            .Select(charValue)
            .Sum();
        Console.WriteLine($"part2:{sum}");
    }

    private static void part01()
    {
        var sum = File.ReadLines("input.txt")
            .Select(line => line.Chunk(line.Length / 2))
            .Select(chunks => chunks.First().Intersect(chunks.Last()).First())
            .Select(charValue)
            .Sum();
        Console.WriteLine($"part1:{sum}");
    }

    private static int charValue(char c) => Char.IsLower(c) ? c - 'a' + 1 : c - 'A' + 27;
}
