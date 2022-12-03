using System.Runtime.CompilerServices;

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
            .Select(e => e.Select(s => s.ToHashSet()).ToList())
            .Select(elves => elves[0].Intersect(elves[1]).Intersect(elves[2])
                .Select(charValue).First())
            .Sum();
        Console.WriteLine($"part2:{sum}");
    }
    private static void part01()
    {
        var sum = File.ReadLines("input.txt")
            .Select(line => line.Substring(0, line.Length / 2).ToHashSet()
                .Intersect(line.Substring(line.Length / 2).ToHashSet())
                .Select(charValue).First())
            .Sum();
        Console.WriteLine($"part1:{sum}");
    }

    private static int charValue(char c)
    {
        if (c >= 'a' && c <= 'z')
        {
            return (int)c - (int)'a' + 1;
        }
        else if (c >= 'A' && c <= 'Z')
        {
            return (int)c - (int)'A' + 27;
        }
        return 0;
    }
}