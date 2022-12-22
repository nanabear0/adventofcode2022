using System.Text.RegularExpressions;

internal class Program
{
    public static (int x, int y)[] directions = new[]
    {
        (1,0),
        (0,1),
        (-1,0),
        (0,-1),
    };
    private static void Main(string[] args)
    {
        var fileReader = File.ReadLines("input.txt").GetEnumerator();
        SortedDictionary<(int x, int y), char> map = new();
        int y = 0;
        (int x, int y) bounds = (int.MaxValue, int.MinValue);
        while (fileReader.MoveNext())
        {
            if (string.IsNullOrWhiteSpace(fileReader.Current)) break;
            int x = 0;
            foreach (char c in fileReader.Current)
            {
                if (c != ' ')
                    map.Add((x, y), c);
                x++;
            }
            bounds.x = x;
            bounds.y = ++y;
        }
        fileReader.MoveNext();
        List<string> instructions = new Regex(@"[0-9]+|[LR]").Matches(fileReader.Current).Select(m => m.Value).ToList();
        var direction = 0;
        (int x, int y) position = (
            map.Where(k => k.Key.y == 0).Where(k => k.Value == '.').Select(k => k.Key.x).Min(),
            0
        );
        Console.WriteLine($"{position}, {direction}");
        foreach (var instruction in instructions)
        {
            if (instruction == "L") direction = (direction + 3) % 4;
            else if (instruction == "R") direction = (direction + 1) % 4;
            else
            {
                var distance = int.Parse(instruction);
                var ppp = map.Where(m => (direction % 2 == 0 && m.Key.y == position.y) || (direction % 2 == 1 && m.Key.x == position.x));
                if (direction > 1) ppp = ppp.Reverse();
                position = Enumerable.Repeat(ppp, distance).SelectMany(x => x)
                    .SkipWhile(x => x.Key != position)
                    .Skip(1)
                    .Take(distance)
                    .TakeWhile(x => x.Value == '.')
                    .Select(x => x.Key)
                    .LastOrDefault(position);
            }
        }
        Console.WriteLine($"{1000 * (position.y + 1) + 4 * (position.x + 1) + direction}");
    }
    public static (int x, int y) addPointers((int x, int y) p1, (int x, int y) p2, (int x, int y) bounds) => ((p1.x + p2.x) % bounds.x, (p1.y + p2.y) % bounds.y);
}