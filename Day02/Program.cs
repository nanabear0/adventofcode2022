internal class Program
{
    private static void Main(string[] args)
    {
        day01();
        day02();
    }

    public static void day02()
    {
        Console.WriteLine(
            "day01: " +
            System.IO.File.ReadLines(@"input.txt")
            .Select(x => x.Split(' '))
            .Select(line => (ec: (int)line[0][0] - 65, result: (int)line[1][0] - 88))
            .Select(x => x.result * 3 + (x.ec + x.result + 2) % 3 + 1)
            .Sum());
    }
    public static void day01()
    {
        Console.WriteLine(
            "day02: " +
            System.IO.File.ReadLines(@"input.txt")
            .Select(x => x.Split(' '))
            .Select(line => (ec: (int)line[0][0] - 65, mc: (int)line[1][0] - 88))
            .Select(x => x.mc + 1 + ((x.mc - x.ec + 4) % 3) * 3)
            .Sum());
    }
}