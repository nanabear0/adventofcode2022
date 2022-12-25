using System.Linq;

internal class Program
{
    static Dictionary<char, (int x, int y)> windDirections = new()
    {
        {'>',(1,0) },
        {'<',(-1,0) },
        {'v',(0,1) },
        {'^',(0,-1) },
    };

    static List<(int x, int y)> directions = new()
    {
        (0,0),
        (1,0),
        (0,1),
        (-1,0),
        (0,-1),
    };

    private static void Main(string[] args)
    {
        Dictionary<(int x, int y), List<char>> winds = new();
        ((int min, int max) x, (int min, int max) y) movableAreaLimit = ((int.MaxValue, int.MinValue), (int.MaxValue, int.MinValue));
        foreach ((var lines, int y) in File.ReadLines("input.txt").Select((x, i) => (x, i)).Skip(1).SkipLast(1))
        {
            foreach ((var c, int x) in lines.Select((x, i) => (x, i)))
            {
                switch (c)
                {
                    case '>':
                    case '<':
                    case 'v':
                    case '^':
                        winds.Add((x, y), new List<char>() { c });
                        break;
                }
                if (c != '#')
                    movableAreaLimit = (
                        (Math.Min(movableAreaLimit.x.min, x), Math.Max(movableAreaLimit.x.max, x)),
                        (Math.Min(movableAreaLimit.y.min, y), Math.Max(movableAreaLimit.y.max, y))
                        );
            }
        }
        (int x, int y) start = (1, 0);
        (int x, int y) end = (movableAreaLimit.x.max, movableAreaLimit.y.max + 1);
        Console.WriteLine($"part1: {tryToReachEndAtLowestSteps(winds, start, end, movableAreaLimit, 0)}");
    }

    public static int tryToReachEndAtLowestSteps(
        Dictionary<(int x, int y), List<char>> winds,
        (int x, int y) start,
        (int x, int y) end,
        ((int min, int max) x, (int min, int max) y) movableAreaLimit,
        int times)
    {
        HashSet<(int x, int y)> possiblePositions = new HashSet<(int x, int y)> { (start) };
        Dictionary<(int x, int y), List<char>> currentWinds = winds;
        int time = 0;
        do
        {
            time++;
            currentWinds = moveWinds(currentWinds, movableAreaLimit);
            possiblePositions = directions
                .SelectMany(d => possiblePositions.Select(pm => (x: d.x + pm.x, y: d.y + pm.y)))
                .Where(p =>
                    (p.x >= movableAreaLimit.x.min &&
                    p.x <= movableAreaLimit.x.max &&
                    p.y >= movableAreaLimit.y.min &&
                    p.y <= movableAreaLimit.y.max) ||
                    p == start ||
                    p == end)
                .Where(p => !currentWinds.ContainsKey(p)).ToHashSet();
            if (possiblePositions.Contains(end))
                return time +
                    (times < 2 ?
                        tryToReachEndAtLowestSteps(currentWinds, end, start, movableAreaLimit, times + 1) :
                        0);
        } while (currentWinds.Count > 0);
        return -1;
    }

    private static Dictionary<(int x, int y), List<char>> moveWinds(
        Dictionary<(int x, int y), List<char>> winds,
        ((int min, int max) x, (int min, int max) y) movableAreaLimit)
    {
        var actualMapSize = (
            x: movableAreaLimit.x.max - movableAreaLimit.x.min + 1,
            y: movableAreaLimit.y.max - movableAreaLimit.y.min + 1
            );
        Dictionary<(int x, int y), List<char>> newWinds;
        newWinds = new();
        foreach (((int x, int y), var wind) in winds
            .SelectMany(winds => winds.Value.Select(wind => (winds.Key, wind)))
            .Select(w => (
                    (
                        x: w.Key.x + windDirections[w.wind].x,
                        y: w.Key.y + windDirections[w.wind].y
                    ),
                    w.wind))
            .Select(w => (
                            (
                                (w.Item1.x - movableAreaLimit.x.min + actualMapSize.x) % actualMapSize.x + movableAreaLimit.x.min,
                                (w.Item1.y - movableAreaLimit.y.min + actualMapSize.y) % actualMapSize.y + movableAreaLimit.y.min
                            ),
                            w.wind
                          )
                    )
        )
        {
            List<char> list;
            if (!newWinds.ContainsKey((x, y))) newWinds[(x, y)] = new();
            list = newWinds[(x, y)];
            list.Add(wind);
        }
        return newWinds;
    }
}