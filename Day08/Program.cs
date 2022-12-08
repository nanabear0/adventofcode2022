internal class Program
{
    private static void Main(string[] args)
    {
        part1();
        part2();
    }
    private static void part2()
    {
        List<List<int>> trees = File.ReadLines("input.txt").Select(line => line.Select(x => x - '0').ToList()).ToList();
        int maxScore = trees.SelectMany((treeRow, i) => treeRow.Select((tree, j) =>
        {
            var upCount = trees.Select(v => v[j]).Take(i).Reverse().TakeWhile(v => v < tree).Count();
            if (upCount != i) upCount++;
            var downCount = trees.Select(v => v[j]).Skip(i + 1).TakeWhile(v => v < tree).Count();
            if (downCount != (trees.Count - i - 1)) downCount++;
            var leftCount = treeRow.Take(j).Reverse().TakeWhile(v => v < tree).Count();
            if (leftCount != j) leftCount++;
            var rightCount = treeRow.Skip(j + 1).TakeWhile(v => v < tree).Count();
            if (rightCount != (treeRow.Count - j - 1)) rightCount++;
            return upCount * downCount * leftCount * rightCount;
        })).Max();
        Console.WriteLine($"part2: {maxScore}");
    }
    private static void part1()
    {
        List<List<int>> trees = File.ReadLines("input.txt").Select(line => line.Select(x => x - '0').ToList()).ToList();
        int count = trees.Select((treeRow, i) => treeRow.Where((tree, j) =>
                        trees.Select(v => v[j]).Take(i).All(v => v < tree) ||
                        trees.Select(v => v[j]).Skip(i + 1).All(v => v < tree) ||
                        trees[i].Take(j).All(v => v < tree) ||
                        trees[i].Skip(j + 1).All(v => v < tree)).Count()).Sum();
        Console.WriteLine($"part1: {count}");
    }
}