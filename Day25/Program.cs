internal class Program
{
    private static void Main(string[] args)
    {
        var part1 = parseNormal(File.ReadLines("input.txt").Select(line => parseSnafu(line)).Sum());
        Console.WriteLine($"part1: {part1}");

    }

    private static long parseSnafu(string s)
    {
        return s.Select(c => c switch
        {
            '0' => 0,
            '1' => 1,
            '2' => 2,
            '=' => -2,
            '-' => -1,
            _ => throw new NotImplementedException()
        }).Aggregate((long)0, (agg, d) => agg * 5 + d);
    }

    private static string parseNormal(long l)
    {
        string n = "";
        while (l > 0)
        {
            var r = l % 5;
            switch (r)
            {
                case 0:
                case 1:
                case 2:
                    n = r + n;
                    break;
                case 3:
                    l += r;
                    n = '=' + n;
                    break;
                case 4:
                    l += r;
                    n = '-' + n;
                    break;
            }
            l /= 5;
        }
        return n;
    }
}