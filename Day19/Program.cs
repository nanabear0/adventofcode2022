﻿using System.Reflection.Metadata;
using System.Text.RegularExpressions;

internal class Program
{
    private static void Main(string[] args)
    {
        var minutes = 24;
        var reggy = new Regex(@"Blueprint ([0-9]+):\s+Each ore robot costs ([0-9]+) ore\.\s+Each clay robot costs ([0-9]+) ore\.\s+Each obsidian robot costs ([0-9]+) ore and ([0-9]+) clay\.\s+Each geode robot costs ([0-9]+) ore and ([0-9]+) obsidian\.");
        var sum = 0;
        foreach (Match match in reggy.Matches(File.ReadAllText("input.txt")))
        {
            Dictionary<int, int[]> robotCosts = new();
            var matches = match.Groups.Values.Skip(1).Select(m => int.Parse(m.Value)).ToList();
            robotCosts[0] = new int[] { matches[1], 0, 0, 0 };
            robotCosts[1] = new int[] { matches[2], 0, 0, 0 };
            robotCosts[2] = new int[] { matches[3], matches[4], 0, 0 };
            robotCosts[3] = new int[] { matches[5], 0, matches[6], 0 };
            var currentResources = new int[] { 0, 0, 0, 0 };
            var currentRobots = new int[] { 1, 0, 0, 0 };
            var b = bestResult(robotCosts, currentResources, currentRobots, 23);
            sum += b * matches[0];
        }
        Console.WriteLine($"part1: {sum}");
    }
    public static int bestResult(
        Dictionary<int, int[]> robotCosts,
        int[] currentResources,
        int[] currentRobots,
        int timeRemaining
        )
    {
        //Console.WriteLine($"[{string.Join(",", currentResources)}] [{string.Join(",", currentRobots)}] : {timeRemaining}");
        if (timeRemaining <= 0)
            return currentResources[3];
        var nextBuy = robotCosts
            .Select(rc => (
                key: rc.Key,
                additionalResources: rc.Value.Zip(currentResources).Select((t) => t.First - t.Second).ToArray()
                ))
            .Where(ar => currentRobots[ar.key] < 4)
            .Where(ar => ar.additionalResources.Zip(currentRobots).All((t) => t.Second != 0 || t.First == 0))
            .Select(ar => (
                key: ar.key,
                timeNeeded: Math.Max(ar.additionalResources.Zip(currentRobots).Select((t) => t.Second == 0 ? 0 : t.First / t.Second).Max(), 0) + 1
            ))
            .Where(tn => tn.timeNeeded < timeRemaining)
            .ToList();
        if (nextBuy.Count() == 0)
            return currentRobots[3] * timeRemaining + currentResources[3];

        return nextBuy.Select(nb =>
        {
            var nextResources = currentRobots
                .Zip(currentResources).Select(v => v.First * nb.timeNeeded + v.Second)
                .Zip(robotCosts[nb.key]).Select(v => v.First - v.Second).ToArray();
            var nextRobots = currentRobots.ToArray();
            nextRobots[nb.key]++;
            var nextTime = timeRemaining - nb.timeNeeded;
            return bestResult(robotCosts, nextResources, nextRobots, nextTime);
        }).Max();
    }
}