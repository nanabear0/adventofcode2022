﻿using System.Text.RegularExpressions;

internal class Program
{
    readonly static (int, int) noMatch = (-1, -1);
    readonly static (int, int)[] directionHierarchy = new (int, int)[] { (0, 1), (-1, 1), (1, 1) };
    private static void Main()
    {
        Dictionary<(int, int), char> cavern = new();
        var maxDistance = 0;
        foreach (string path in File.ReadLines("input.txt"))
        {
            Regex reggy = new(@"([0-9]+),([0-9]+)");
            var points = reggy.Matches(path)
                .Select(match => match.Groups.Values.Skip(1).Take(2).Select(v => v.Value).ToList())
                .Select(pointParts => (int.Parse(pointParts[0]), int.Parse(pointParts[1])))
                .ToList();
            cavern[points[0]] = '#';
            for (int i = 1; i < points.Count; i++)
            {
                var vector = (points[i].Item1 - points[i - 1].Item1, points[i].Item2 - points[i - 1].Item2);
                var normalizedVector = (vector.Item1 / Math.Max(Math.Abs(vector.Item1), 1), vector.Item2 / Math.Max(Math.Abs(vector.Item2), 1));
                var nextPoint = (points[i - 1].Item1, points[i - 1].Item2);
                do
                {
                    nextPoint = (nextPoint.Item1 + normalizedVector.Item1, nextPoint.Item2 + normalizedVector.Item2);
                    cavern[nextPoint] = '#';
                } while (nextPoint != points[i]);
                maxDistance = Math.Max(points[i].Item2, maxDistance);
            }
        }
        // ---- //
        bool sandStopped = false;
        int sandDrops = 0;
        do
        {
            var sandPos = (500, 0);
            do
            {
                var nextPos = directionHierarchy.AsEnumerable()
                    .Select(d => (sandPos.Item1 + d.Item1, sandPos.Item2 + d.Item2))
                    .Where(x => !cavern.ContainsKey(x))
                    .Where(x => x.Item2 < maxDistance + 2) // p2
                    .FirstOrDefault(noMatch);
                sandStopped = nextPos == noMatch;
                if (sandStopped)
                {
                    //Console.WriteLine($"stopped at: {sandPos.Item1},{sandPos.Item2}");
                    cavern[sandPos] = 'o';
                    sandDrops++;
                    if (sandPos == (500, 0)) sandStopped = false; // p2
                    break;
                }
                sandPos = nextPos;
            } while (true); // p2
        } while (sandStopped);

        Console.WriteLine($"part2:{sandDrops}");
    }
}