using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

internal class Program
{
    public class Valve
    {
        public string name;
        public int flow = 0;
        public List<Valve> neighbours = new();
        public Valve(string name)
        {
            this.name = name;
        }
    }

    private static void Main(string[] args)
    {
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();
        part2();
        stopwatch.Stop();
        Console.WriteLine(stopwatch.Elapsed.ToString());
    }

    private static Dictionary<string, int> cache = new Dictionary<string, int>();

    private static int TryEverything(
      Dictionary<string, Valve> valves,
      Dictionary<(string, string), int> distances,
      Dictionary<string, Valve> human,
      Dictionary<string, Valve> elephant)
    {
        if (valves.Count == 0)
        {
            if (human.Count > elephant.Count) return 0;
            return BestPossibility(human, distances, new HashSet<string>(), "AA", 26) + BestPossibility(elephant, distances, new HashSet<string>(), "AA", 26);
        }
        var valve = valves.First();
        valves.Remove(valve.Key);

        human.Add(valve.Key, valve.Value);
        int v1 = TryEverything(valves, distances, human, elephant);
        human.Remove(valve.Key);

        elephant.Add(valve.Key, valve.Value);
        int v2 = TryEverything(valves, distances, human, elephant);
        elephant.Remove(valve.Key);

        valves.Add(valve.Key, valve.Value);
        return Math.Max(v1, v2);
    }

    private static void part2()
    {
        Dictionary<string, Valve> valves;
        Dictionary<(string, string), int> distances;
        parse(out valves, out distances);
        int best = TryEverything(valves, distances, new Dictionary<string, Valve>(), new Dictionary<string, Valve>());
        Console.WriteLine($"part2: {best}");
    }

    private static void part1()
    {
        Dictionary<string, Program.Valve> valves;
        Dictionary<(string, string), int> distances;
        parse(out valves, out distances);
        int best = BestPossibility(valves, distances, new HashSet<string>(), "AA", 30);
        Console.WriteLine($"part2: {best}");
    }

    private static int BestPossibility2(
        Dictionary<string, Valve> valves,
        Dictionary<(string, string), int> distances,
        HashSet<string> openValves,
        string current,
        int leftTime)
    {
        string key = string.Join(",", valves);
        if (cache.ContainsKey(key)) return cache[key];
        int best = BestPossibility(valves, distances, openValves, current, leftTime);
        cache[key] = best;
        return best;
    }

    private static int BestPossibility(
        Dictionary<string, Valve> valves,
        Dictionary<(string, string), int> distances,
        HashSet<string> openValves,
        string current,
        int leftTime)
    {
        return valves.Where(v => !openValves.Contains(v.Key)).Select(v =>
        {
            int time = distances[(current, v.Key)] + 1;
            if (time >= leftTime) return 0;
            return v.Value.flow * (leftTime - time) + BestPossibility(valves, distances, openValves.Prepend(v.Key).ToHashSet(), v.Key, leftTime - time);
        }).DefaultIfEmpty(0).Max();
    }

    private static void parse(out Dictionary<string, Valve> valves, out Dictionary<(string, string), int> distances)
    {
        valves = new();
        foreach (string line in File.ReadLines("input.txt"))
        {
            Regex reggy = new(@"Valve ([a-zA-Z]+) has flow rate=([0-9]+); tunnels? leads? to valves? ((?:[A-Z]+,? ?)+)");
            var groups = reggy.Match(line).Groups.Values.Skip(1).Select(g => g.Value).ToList();
            var name = groups[0];
            var flow = int.Parse(groups[1]);
            var neighbours = groups[2].Split(", ").ToList();
            Valve valvy;
            if (valves.ContainsKey(name)) valvy = valves[name];
            else { valvy = new Valve(name); valves[name] = valvy; }
            valvy.flow = flow;
            foreach (string n in neighbours)
            {
                Valve neighbour;
                if (!valves.ContainsKey(n)) { neighbour = new Valve(n); valves[n] = neighbour; }
                else neighbour = valves[n];

                valvy.neighbours.Add(neighbour);
            }
        }
        distances = new();
        foreach (var (p1, i) in valves.Keys.Select((k, i) => (k, i)))
        {
            foreach (var p2 in valves.Keys.Skip(i + 1))
            {
                if (distances.ContainsKey((p1, p2))) continue;
                var distance = findDistance(valves, p1, p2);
                distances[(p1, p2)] = distance;
                distances[(p2, p1)] = distance;
            }
        }
        valves = valves.Where(v => v.Value.flow > 0).ToDictionary(v => v.Key, v => v.Value);
    }

    public static int findDistance(Dictionary<string, Valve> valves, string p1, string p2)
    {
        Dictionary<string, int> distances = new() { { p1, 0 } };
        HashSet<string> outer = new() { p1 };
        var i = 1;
        while (outer.Count > 0)
        {
            HashSet<string> inner = new();
            foreach (var v in outer)
            {
                foreach (var n in valves[v].neighbours)
                {
                    if (distances.ContainsKey(n.name)) continue;
                    if (n.name == p2) return i;
                    inner.Add(n.name);
                    distances.Add(n.name, i);
                }
            }
            outer = inner;
            i++;
        }
        return -1;
    }
}
