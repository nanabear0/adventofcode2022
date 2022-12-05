using System.Linq;
using System.Text.RegularExpressions;

internal class Program
{
    private static void Main(string[] args)
    {
        part1();
        part2();
    }

    private static void part1()
    {
        IEnumerator<string> fileReader;
        List<Stack<string>> columns;
        readColumns(out fileReader, out columns);
        while (fileReader.MoveNext())
        {
            List<int> matches;
            Stack<string> source, receiver;
            readLines(fileReader, columns, out matches, out source, out receiver);
            foreach (var elem in Enumerable.Range(0, matches[0]).Select(_ => source.Pop()))
                receiver.Push(elem);
        }
        Console.WriteLine($"part1: {string.Join("", columns.Select(v => v.First()))}");
    }

    private static void part2()
    {
        IEnumerator<string> fileReader;
        List<Stack<string>> columns;
        readColumns(out fileReader, out columns);
        while (fileReader.MoveNext())
        {
            List<int> matches;
            Stack<string> source, receiver;
            readLines(fileReader, columns, out matches, out source, out receiver);
            foreach (var elem in Enumerable.Range(0, matches[0]).Select(_ => source.Pop()).Reverse())
                receiver.Push(elem);
        }
        Console.WriteLine($"part2: {string.Join("", columns.Select(v => v.First()))}");
    }

    private static void readLines(
        IEnumerator<string> fileReader,
        List<Stack<string>> columns,
        out List<int> matches,
        out Stack<string> source,
        out Stack<string> receiver)
    {
        var regex = new Regex(@"[0-9]+");
        matches = regex.Matches(fileReader.Current).Select(v => int.Parse(v.Value)).ToList();
        source = columns[matches[1] - 1];
        receiver = columns[matches[2] - 1];
    }

    private static void readColumns(
        out IEnumerator<string> fileReader,
        out List<Stack<string>> columns)
    {
        fileReader = File.ReadLines("input.txt").GetEnumerator();
        fileReader.MoveNext();
        var columnCount = (fileReader.Current.Length + 1) / 4;
        columns = new(Enumerable.Range(0, columnCount).Select(_ => new Stack<string>()));
        do
        {
            var regex = new Regex(@"(?:\[([A-Z])\]|   ) ?");
            var matches = regex.Matches(fileReader.Current);
            if (matches.Count == 0) break;
            for (int i = 0; i < matches.Count; i++)
            {
                if (matches[i].Value.Trim().Length == 0) continue;
                columns[i].Push(matches[i].Groups[1].Value);
            }
        } while (fileReader.MoveNext());
        columns = columns.Select(s => s).Select(s => new Stack<string>(s)).ToList();
    }
}