using System.Text.RegularExpressions;

internal class Program
{
    private static void Main(string[] args)
    {
        part1();
    }

    private static void part1()
    {
        var targetY = 2000000; // \\
        HashSet<((int, int), (int, int), int)> pairs = new();
        HashSet<(int, int)> beacons = new();
        HashSet<(int, int)> sensors = new();
        HashSet<(int, int)> emptySpots = new();
        foreach (string line in File.ReadLines("input.txt"))
        {
            int[] numbers = new Regex(@"-?[0-9]+").Matches(line).Select(m => int.Parse(m.Value)).ToArray();
            var sensor = (numbers[0], numbers[1]);
            var beacon = (numbers[2], numbers[3]);
            var distance = manhattanDistance(sensor, beacon);
            pairs.Add((sensor, beacon, distance));
            beacons.Add(beacon);
            sensors.Add(sensor);
        }
        foreach (var (sensor, beacon, distance) in pairs)
        {
            var distanceToTargetY = Math.Abs(sensor.Item2 - targetY);
            if (distanceToTargetY > distance) continue;
            var xAllowance = distance - distanceToTargetY;
            for (int x = -xAllowance; x <= xAllowance; x++) emptySpots.Add((x + sensor.Item1, targetY));
        }
        int count = emptySpots.Where(es => !beacons.Contains(es) && !sensors.Contains(es)).Count();
        Console.WriteLine($"part1:{count}");
    }

    public static int manhattanDistance((int, int) p1, (int, int) p2)
    {
        return Math.Abs(p1.Item1 - p2.Item1) + Math.Abs(p1.Item2 - p2.Item2);
    }
}