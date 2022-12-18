using System.Linq;

internal class Program
{
    private static void Main(string[] args)
    {
        //part1();
        part2();
    }
    public static void part2()
    {
        var sides = new List<(int x, int y, int z)>()
        {
            (1,0,0),
            (-1,0,0),
            (0,1,0),
            (0,-1,0),
            (0,0,1),
            (0,0,-1),
        };
        HashSet<(int x, int y, int z)> droplets = File.ReadLines("input.txt")
            .Select(l => l.Split(',').Select(d => int.Parse(d)).ToArray())
            .Select(ds => (x: ds[0], y: ds[1], z: ds[2]))
            .ToHashSet();
        var outside = droplets.Aggregate(
            (
                x: (min: int.MaxValue, max: int.MinValue),
                y: (min: int.MaxValue, max: int.MinValue),
                z: (min: int.MaxValue, max: int.MinValue)
            ),
            (acc, v) => (
                (Math.Min(acc.x.min, v.x - 1), Math.Max(acc.x.max, v.x + 1)),
                (Math.Min(acc.y.min, v.y - 1), Math.Max(acc.y.max, v.y + 1)),
                (Math.Min(acc.z.min, v.z - 1), Math.Max(acc.z.max, v.z + 1))
            ));
        HashSet<(int x, int y, int z)> airPockets = droplets.SelectMany(droplet => sides
            .Select(side => (droplet.x + side.x, droplet.y + side.y, droplet.z + side.z))
            .Where(pocket => !droplets.Contains(pocket)))
            .ToHashSet();
        var freeAirPockets = airPockets.Where(pocket =>
        {
            HashSet<(int x, int y, int z)> visited = new() { };
            HashSet<(int x, int y, int z)> current = new() { pocket };
            while (current.Count > 0)
            {
                HashSet<(int x, int y, int z)> next = new();
                foreach (var c in current)
                {
                    foreach (var side in sides)
                    {
                        var n = (x: c.x + side.x, y: c.y + side.y, z: c.z + side.z);
                        if (visited.Contains(n) || droplets.Contains(n)) continue;
                        else if (n.x <= outside.x.min ||
                            n.x >= outside.x.max ||
                            n.y <= outside.y.min ||
                            n.y >= outside.y.max ||
                            n.z <= outside.z.min ||
                            n.z >= outside.z.max) return true;
                        else
                            next.Add(n);
                    }
                }
                visited = visited.Union(next).ToHashSet();
                current = next;
            }
            return false;
        }).ToHashSet();
        var surfaceArea = droplets.Select(droplet => sides
            .Select(side => (droplet.x + side.x, droplet.y + side.y, droplet.z + side.z))
            .Where(surface => freeAirPockets.Contains(surface)).Count())
        .Sum();
        Console.WriteLine($"part2: {surfaceArea}");
    }

    private static void part1()
    {
        var sides = new List<(int x, int y, int z)>()
        {
            (1,0,0),
            (-1,0,0),
            (0,1,0),
            (0,-1,0),
            (0,0,1),
            (0,0,-1),
        };
        HashSet<(int x, int y, int z)> droplets = File.ReadLines("input.txt")
            .Select(l => l.Split(',').Select(d => int.Parse(d)).ToArray())
            .Select(ds => (x: ds[0], y: ds[1], z: ds[2]))
            .ToHashSet();
        var surfaceArea = droplets.Select(droplet => sides
            .Where(side =>
                !droplets.Contains((droplet.x + side.x, droplet.y + side.y, droplet.z + side.z))).Count())
        .Sum();
        Console.WriteLine($"part1: {surfaceArea}");
    }
}