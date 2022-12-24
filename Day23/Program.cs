using System.Linq;

internal class Program
{

    public static List<((int, int), bool)> directions = new List<((int, int), bool)>()
    {
        ((0,-1), true),
        ((0,1), true),
        ((-1,0), false),
        ((1,0), false),
    };
    private static void Main(string[] args)
    {

        HashSet<(int x, int y)> map = File.ReadLines("input.txt")
            .SelectMany((line, y) => line.AsEnumerable().Select((c, x) => (c, x, y)))
            .Where(x => (x.c == '#'))
            .Select(x => (x.x, x.y))
            .ToHashSet();
        //print(map);
        for (int round = 0; ; round++)
        {
            SortedDictionary<(int x, int y), (int x, int y)?> elfMoves = new();
            HashSet<(int x, int y)> proposedMoves = new();
            HashSet<(int x, int y)> duplicateMoves = new();
            var someoneMoves = false;
            foreach (var elf in map)
            {
                (int x, int y)? chosenOne = null;
                var anyNeighbour = false;
                foreach (((int x, int y), bool vertical) in Enumerable.Repeat(directions, round + 1).SelectMany(x => x).Skip(round).Take(4))
                {
                    var fuckedUp = false;
                    for (int off = -1; off <= 1 && !fuckedUp; off++)
                    {
                        var attempt = (x: x + off * (vertical ? 1 : 0), y: y + off * (vertical ? 0 : 1));
                        if (map.Contains((elf.x + attempt.x, elf.y + attempt.y))) fuckedUp = true;
                    }
                    anyNeighbour |= fuckedUp;
                    if (!fuckedUp && !chosenOne.HasValue)
                        chosenOne = (x, y);
                }
                if (chosenOne.HasValue && anyNeighbour)
                {
                    var move = (elf.x + chosenOne.Value.x, elf.y + chosenOne.Value.y);
                    elfMoves.Add(elf, move);
                    if (proposedMoves.Contains(move)) duplicateMoves.Add(move);
                    else proposedMoves.Add(move);
                }
                else
                {
                    elfMoves.Add(elf, null);
                }
            }
            HashSet<(int x, int y)> newMap = new();
            foreach ((var elf, var move) in elfMoves)
            {
                if (!move.HasValue) newMap.Add(elf);
                else if (duplicateMoves.Contains(move.Value)) newMap.Add(elf);
                else { newMap.Add(move.Value); someoneMoves = true; }
            }
            if (!someoneMoves) { Console.WriteLine($"part2; {round + 1}"); break; }
            map = newMap;
            //print(map);
        }
        var bounds = getBounds(map);
        var count = (bounds.x.max - bounds.x.min + 1) * (bounds.y.max - bounds.y.min + 1) - map.Count;
        //Console.WriteLine($"part1: {count}");
    }

    private static void print(HashSet<(int x, int y)> map)
    {
        var bound = getBounds(map);
        for (int y = bound.y.min; y <= bound.y.max; y++)
        {
            for (int x = bound.x.min; x <= bound.x.max; x++)
            {
                Console.Write(map.Contains((x, y)) ? '#' : '.');
            }
            Console.WriteLine();
        }
        Console.WriteLine("--------------------------------------------------");
    }

    private static ((int min, int max) x, (int min, int max) y) getBounds(HashSet<(int x, int y)> map)
    {
        return map.Aggregate((x: (min: int.MaxValue, max: int.MinValue), y: (min: int.MaxValue, max: int.MinValue)),
                            (agg, elf) => (
                                (Math.Min(elf.x, agg.x.min), Math.Max(elf.x, agg.x.max)),
                                (Math.Min(elf.y, agg.y.min), Math.Max(elf.y, agg.y.max))
                                ));
    }
}