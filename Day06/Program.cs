internal class Program
{
    private static void Main(string[] args)
    {
        part(1, 4);
        part(2, 14);
    }

    private static void part(int p, int c)
    {
        var stream = File.ReadAllText("input.txt").ToList();
        var count = Enumerable.Range(0, stream.Count - (c-1)).TakeWhile(i => stream.GetRange(i, c).ToHashSet().Count != c).Count();
        Console.WriteLine($"part{p}: {count + c}");
    }
}