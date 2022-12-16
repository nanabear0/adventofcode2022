using System.Security.Cryptography.X509Certificates;
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
        //part1();
        part2();
    }

    private static void part2()
    {
        SortedDictionary<string, Valve> valves;
        SortedDictionary<(string, string), int> distances;
        parse(out valves, out distances);
        var best = bestPosibility2(valves, distances, new HashSet<string>(), valves.Values.Where(v => v.flow > 0).Select(v => v.name).ToHashSet(), "AA", "AA", "", 0, "", 0, 0, 26);
        Console.WriteLine($"part1: {best}");
    }

    public static int bestPosibility2(
        SortedDictionary<string, Valve> valves,
        SortedDictionary<(string, string), int> distances,
        HashSet<string> openValves,
        HashSet<string> closedNonZeroValves,
        string currentValve1,
        string currentValve2,
        string moving1,
        int leftDistance1,
        string moving2,
        int leftDistance2,
        int currentFlow,
        int timeLeft)
    {
        if (closedNonZeroValves.Count == 0)
            return currentFlow + spendRemainingTime(valves, openValves, timeLeft);
        List<(Valve n, int d)> valvesInRange1;
        if (moving1 == "")
            valvesInRange1 = valves.Values.Where(n => closedNonZeroValves.Contains(n.name))
                .Where(n => n.name != moving2)
                .Select(n => (n, d: distances[(currentValve1, n.name)]))
                .Where(x => x.d + 1 < timeLeft).ToList();
        else
            valvesInRange1 = new() { (n: valves[moving1], d: leftDistance1) };

        List<(Valve n, int d)> valvesInRange2;
        if (moving2 == "")
            valvesInRange2 = valves.Values.Where(n => closedNonZeroValves.Contains(n.name))
                .Where(n => n.name != moving1)
                .Select(n => (n, d: distances[(currentValve2, n.name)]))
                .Where(x => x.d + 1 < timeLeft).ToList();
        else
            valvesInRange2 = new() { (n: valves[moving2], d: leftDistance2) };

        if (valvesInRange1.Count == 0 && valvesInRange2.Count == 0)
            return currentFlow + spendRemainingTime(valves, openValves, timeLeft);
        else if (valvesInRange1.Count == 0)
        {
            currentFlow += spendRemainingTime(valves, openValves, leftDistance2 + 1);
            timeLeft -= leftDistance2 + 1;
            openValves.Add(moving2);
            currentFlow += spendRemainingTime(valves, openValves, timeLeft);
            return currentFlow;
        }
        else if (valvesInRange2.Count == 0)
        {
            currentFlow += spendRemainingTime(valves, openValves, leftDistance1 + 1);
            timeLeft -= leftDistance1 + 1;
            openValves.Add(moving1);
            currentFlow += spendRemainingTime(valves, openValves, timeLeft);
            return currentFlow;
        }
        else
        {
            if (valvesInRange1.Count == valvesInRange2.Count && valvesInRange1.Count == 1 && valvesInRange1[0].n.name == valvesInRange2[0].n.name)
            {
                if (valvesInRange1[0].d > valvesInRange2[0].d)
                {
                    currentFlow += spendRemainingTime(valves, openValves, valvesInRange2[0].d + 1);
                    timeLeft -= valvesInRange2[0].d + 1;
                    openValves.Add(valvesInRange2[0].n.name);
                    currentFlow += spendRemainingTime(valves, openValves, timeLeft);
                    return currentFlow;
                }
                else
                {
                    currentFlow += spendRemainingTime(valves, openValves, valvesInRange1[0].d + 1);
                    timeLeft -= valvesInRange1[0].d + 1;
                    openValves.Add(valvesInRange1[0].n.name);
                    currentFlow += spendRemainingTime(valves, openValves, timeLeft);
                    return currentFlow;
                }
            }
            return valvesInRange1.SelectMany(v1 => valvesInRange2.Where(v2 => v2.n.name != v1.n.name).Select(v2 => (v1, v2)))
                .Select(neighbour =>
                {
                    var neighbour1 = neighbour.Item1.n;
                    var neighbour2 = neighbour.Item2.n;
                    var distance1 = neighbour.Item1.d;
                    var distance2 = neighbour.Item2.d;
                    if (distance1 == distance2)
                    {
                        var newOpenVales = openValves.ToHashSet();
                        newOpenVales.Add(neighbour1.name);
                        newOpenVales.Add(neighbour2.name);
                        var newClosedNonZeroValves = closedNonZeroValves.ToHashSet();
                        newClosedNonZeroValves.Remove(neighbour1.name);
                        newClosedNonZeroValves.Remove(neighbour2.name);
                        return bestPosibility2(
                            valves,
                            distances,
                            newOpenVales,
                            newClosedNonZeroValves,
                            neighbour1.name,
                            neighbour2.name,
                            "",
                            0,
                            "",
                            0,
                            currentFlow + spendRemainingTime(valves, openValves, distance1 + 1),
                            timeLeft - distance1 - 1);
                    }
                    else if (distance1 > distance2)
                    {
                        var newOpenVales = openValves.ToHashSet();
                        newOpenVales.Add(neighbour2.name);
                        var newClosedNonZeroValves = closedNonZeroValves.ToHashSet();
                        newClosedNonZeroValves.Remove(neighbour2.name);
                        return bestPosibility2(
                            valves,
                            distances,
                            newOpenVales,
                            newClosedNonZeroValves,
                            "",
                            neighbour2.name,
                            neighbour1.name,
                            distance1 - distance2 - 1,
                            "",
                            0,
                            currentFlow + spendRemainingTime(valves, openValves, distance2 + 1),
                            timeLeft - distance2 - 1);
                    }
                    else
                    {
                        var newOpenVales = openValves.ToHashSet();
                        newOpenVales.Add(neighbour1.name);
                        var newClosedNonZeroValves = closedNonZeroValves.ToHashSet();
                        newClosedNonZeroValves.Remove(neighbour1.name);
                        return bestPosibility2(
                            valves,
                            distances,
                            newOpenVales,
                            newClosedNonZeroValves,
                            neighbour1.name,
                            "",
                            "",
                            0,
                            neighbour2.name,
                            distance2 - distance1 - 1,
                            currentFlow + spendRemainingTime(valves, openValves, distance1 + 1),
                            timeLeft - distance1 - 1);
                    }
                }).Max();
        }
    }

    private static void part1()
    {
        SortedDictionary<string, Valve> valves;
        SortedDictionary<(string, string), int> distances;
        parse(out valves, out distances);
        var best = bestPosibility(valves, distances, new HashSet<string>(), valves.Values.Where(v => v.flow > 0).Select(v => v.name).ToHashSet(), "AA", 0, 30);
        Console.WriteLine($"part1: {best}");
    }

    private static void parse(out SortedDictionary<string, Valve> valves, out SortedDictionary<(string, string), int> distances)
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