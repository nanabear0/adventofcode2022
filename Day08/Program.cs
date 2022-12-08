internal class Program
{
    private static void Main(string[] args)
    {
        part1();
        part2();
    }
    private static void part2()
    {
        List<List<int>> trees = new List<List<int>>();
        foreach (string line in File.ReadLines("input.txt"))
            trees.Add(line.Select(x => x - '0').ToList());

        int maxScore = -1;

        for (int i = 0; i < trees.Count; i++)
        {
            for (int j = 0; j < trees[i].Count; j++)
            {
                var upCount = 0;
                for (int u = i - 1; u >= 0; u--)
                {
                    upCount++;
                    if (trees[i][j] <= trees[u][j]) break;
                }
                var downCount = 0;
                for (int d = i + 1; d < trees.Count; d++)
                {
                    downCount++;
                    if (trees[i][j] <= trees[d][j]) break;
                }
                var leftCount = 0;
                for (int l = j - 1; l >= 0; l--)
                {
                    leftCount++;
                    if (trees[i][j] <= trees[i][l]) break;
                }
                var rightCount = 0;
                for (int r = j + 1; r < trees[i].Count; r++)
                {
                    rightCount++;
                    if (trees[i][j] <= trees[i][r]) break;
                }
                var score = upCount * downCount * leftCount * rightCount;
                maxScore = maxScore < score ? score : maxScore;
            }
        }
        Console.WriteLine($"part2: {maxScore}");
    }
    private static void part1()
    {
        List<List<int>> trees = new List<List<int>>();
        foreach (string line in File.ReadLines("input.txt"))
            trees.Add(line.Select(x => x - '0').ToList());

        int count = 0;
        for (int i = 0; i < trees.Count; i++)
        {
            for (int j = 0; j < trees.Count; j++)
            {
                // Up
                if (trees.Select(v => v[j]).Take(i).All(v => v < trees[i][j])) { count++; continue; }
                // Down
                if (trees.Select(v => v[j]).Skip(i + 1).All(v => v < trees[i][j])) { count++; continue; }
                // Left
                if (trees[i].Take(j).All(v => v < trees[i][j])) { count++; continue; }
                // Right
                if (trees[i].Skip(j + 1).All(v => v < trees[i][j])) { count++; continue; }
            }
        }
        Console.WriteLine($"part1: {count}");
    }
}