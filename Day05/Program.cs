using System.Text.RegularExpressions;

internal class Program
{
    private static void Main(string[] args)
    {
        var fileReader = File.ReadLines("input.txt").GetEnumerator();
        fileReader.MoveNext();
        var columnCount = (fileReader.Current.Length + 1) / 4;
        var columns = new List<List<string>>();
        for (int i = 0; i < columnCount; i++)
        {
            columns.Add(new List<string>());
        }
        do
        {
            var regex = new Regex(@"(?:\[([A-Z])\]|   ) ?");
            var matches = regex.Matches(fileReader.Current);
            if (matches.Count == 0) break;
            for (int i = 0; i < matches.Count; i++)
            {
                if (matches[i].Value.Trim().Length == 0) continue;
                columns[i].Insert(0, matches[i].Groups[1].Value);
            }
        } while (fileReader.MoveNext());

        printStack(columns);
        while (fileReader.MoveNext())
        {
            Console.WriteLine(fileReader.Current);
            var regex = new Regex(@"[0-9]+");
            var matches = regex.Matches(fileReader.Current).Select(v => int.Parse(v.Value)).ToList();
            var source = columns[matches[1] - 1];
            var receiver = columns[matches[2] - 1];
            var tempList = new List<String>();
            for (int i = 0; i < matches[0] && source.Count > 0; i++)
            {
                tempList.Add(source.Last());
                source.RemoveAt(source.Count - 1);
            }
            // remove this line for p1
            tempList.Reverse();
            tempList.ForEach(v => receiver.Add(v));
            printStack(columns);
        }
        Console.WriteLine($"part2: {string.Join("", columns.Select(v => v.Last()))}");
    }

    private static void printStack(List<List<string>> columns)
    {
        foreach (var column in columns)
        {
            foreach (var elem in column)
            {
                Console.Write($"{elem}, ");
            }
            Console.WriteLine();
        }
    }
}