internal class Program
{
    private static void Main(string[] args)
    {
        day01();
        day02();
    }
    public static void day01()
    {
        Console.WriteLine($"day01: {readLines().Select(x => x.e2 + 1 + ((x.e2 - x.e1 + 4) % 3) * 3).Sum()}");
    }

    public static void day02()
    {
        Console.WriteLine($"day02: {readLines().Select(x => x.e2 * 3 + (x.e1 + x.e2 + 2) % 3 + 1).Sum()}");
    }

    private static IEnumerable<(int e1, int e2)> readLines()
    {
        return File.ReadLines("input.txt").Select(line => (e1: (int)line[0] - 65, e2: (int)line[2] - 88));
    }
}