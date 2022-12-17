using System.Security.Cryptography.X509Certificates;

internal class Program
{
    public class Rock
    {
        public int height;
        public int width;
        public string pattern;
        public Rock(int height, int width, string pattern)
        {
            this.height = height;
            this.width = width;
            this.pattern = pattern;
        }
    }

    public static List<Rock> rocks = new() {
        new Rock(1, 4, "####"),
        new Rock(3, 3, ".#.###.#."),
        new Rock(3, 3, "###..#..#"),
        new Rock(4, 1, "####"),
        new Rock(2, 2, "####"),
    };
    private static void Main()
    {
        var movements = File.ReadAllText("input.txt").ToList();
        Dictionary<(int, int), char> pattern = new();
        List<(int, int)> blockHistory = new();
        List<int> heightHistory = new();
        var patternTop = -1;
        var currentMovement = 0;
        long currentRock = -1;
        var period = (offset: -1, period: -1, offsetHeight: -1, periodIncrement: -1);
        var matchCount = 0;
        while (matchCount < 10)
        {
            var rockIndex = (int)(++currentRock % (long)rocks.Count);
            var rock = rocks[rockIndex];
            var rockPos = (x: 2, y: patternTop + 4);
            while (true)
            {
                var movement = movements[(currentMovement++) % movements.Count] == '>' ? 1 : -1;
                var newPos = (rockPos.x + movement, rockPos.y + 0);
                if (IsMoveLegit(pattern, rock, newPos))
                    rockPos = newPos;
                newPos = (rockPos.x, rockPos.y - 1);
                if (IsMoveLegit(pattern, rock, newPos))
                    rockPos = newPos;
                else
                    break;
            };
            UpdatePattern(pattern, rock, rockPos);
            patternTop = Math.Max(rockPos.y + rock.height - 1, patternTop);
            blockHistory.Add((rockPos.x, rockIndex));
            heightHistory.Add(patternTop + 1);
            //printPattern(pattern, patternTop);

            for (int i = blockHistory.Count % 2; i <= blockHistory.Count - 2; i += 2)
            {
                var p = (blockHistory.Count - i) / 2;
                if (blockHistory.Skip(i).Take(p)
                    .Zip(blockHistory.Skip(i).Skip(p))
                    .All(h => h.First == h.Second))
                {
                    period = (
                        i,
                        p,
                        heightHistory[i],
                        heightHistory[i + p] - heightHistory[i]
                    );
                    matchCount++;
                }
            }
        }
        // change this for part 1
        long target = 1000000000000 - 1;
        var loops = (target - period.offset) / period.period;
        int loopOffset = (int)((target - period.offset) % period.period);
        var height = period.offsetHeight + // until loop
            period.periodIncrement * loops + // during loops
            (heightHistory[loopOffset + period.offset] - period.offsetHeight); // last loop
        Console.WriteLine($"part2: {height}");
    }

    public static void PrintPattern(Dictionary<(int, int), char> pattern, int patternTop)
    {
        for (int y = patternTop; y >= 0; y--)
        {
            for (int x = 0; x < 7; x++)
            {
                Console.Write(pattern.GetValueOrDefault((x, y), '.'));
            }
            Console.WriteLine();
        }
        Console.WriteLine("-------");
    }

    public static bool IsMoveLegit(Dictionary<(int, int), char> pattern, Rock rock, (int x, int y) newPos)
    {
        if (newPos.x < 0 || newPos.x + rock.width > 7) return false;
        if (newPos.y < 0) return false;
        return !rock.pattern
            .Select((c, i) => (c, rockPos: (x: i % rock.width, y: i / rock.width)))
            .Where(x => x.c == '#')
            .Select(x => x.rockPos)
            .Any(rockPos => pattern.ContainsKey((rockPos.x + newPos.x, newPos.y + rockPos.y)));
    }
    public static void UpdatePattern(Dictionary<(int, int), char> pattern, Rock rock, (int x, int y) newPos)
    {
        foreach (var rockPos in rock.pattern
            .Select((c, i) => (c, rockPos: (x: i % rock.width, y: i / rock.width)))
            .Where(x => x.c == '#')
            .Select(x => x.rockPos))
        {
            pattern[(rockPos.x + newPos.x, newPos.y + rockPos.y)] = '#';
        }
    }
}