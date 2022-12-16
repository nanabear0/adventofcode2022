using System.Text.RegularExpressions;

internal class Program
{
    public class DistanceKey : IComparable<DistanceKey>
    {
        public string[] points = new string[2];

        public DistanceKey(string p1, string p2)
        {
            points[0] = p1;
            points[1] = p2;
        }

        int IComparable<DistanceKey>.CompareTo(DistanceKey? other)
        {
            return this.points.Intersect(other.points).Count() == 2 ? 0 : -1;
        }

    }
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
        SortedDictionary<string, Valve> valves = new();
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
        SortedDictionary<(string, string), int> distances = new();
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
        var best = bestPosibility(valves, distances, new HashSet<string>(), valves.Values.Where(v => v.flow > 0).Select(v => v.name).ToHashSet(), "AA", 0, 30);
        Console.WriteLine($"part1: {best}");
    }

    public static int bestPosibility(
        SortedDictionary<string, Valve> valves,
        SortedDictionary<(string, string), int> distances,
        HashSet<string> openValves,
        HashSet<string> closedNonZeroValves,
        string currentValve,
        int currentFlow,
        int timeLeft)
    {
        if (closedNonZeroValves.Count == 0)
            return currentFlow + spendRemainingTime(valves, openValves, timeLeft);
        var valvesInRange = valves.Values.Where(n => closedNonZeroValves.Contains(n.name))
            .Where(n => distances[(currentValve, n.name)] + 1 < timeLeft).ToList();
        if (valvesInRange.Count == 0)
            return currentFlow + spendRemainingTime(valves, openValves, timeLeft);
        return valvesInRange.Select(neighbour =>
        {
            var distance = distances[(neighbour.name, currentValve)];
            var newOpenVales = openValves.ToHashSet();
            newOpenVales.Add(neighbour.name);
            var newClosedNonZeroValves = closedNonZeroValves.ToHashSet();
            newClosedNonZeroValves.Remove(neighbour.name);
            return bestPosibility(
                valves,
                distances,
                newOpenVales,
                newClosedNonZeroValves,
                neighbour.name,
                currentFlow + spendRemainingTime(valves, openValves, distance + 1),
                timeLeft - distance - 1);
        }).Max();
    }

    private static int spendRemainingTime(SortedDictionary<string, Valve> valves, HashSet<string> openValves, int timeLeft)
    {
        return valves.Values
                        .Where(v => openValves.Contains(v.name))
                        .Select(v => v.flow)
                        .Sum() * timeLeft;
    }

    public static int findDistance(SortedDictionary<string, Valve> valves, string p1, string p2)
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