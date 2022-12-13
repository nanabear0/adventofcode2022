internal class Program
{
    public class Square
    {
        public bool start = false;
        public bool end = false;
        public int elevation = -1;
        public int dist = int.MaxValue;
        public int i;
        public int j;
        public Square(bool start, bool end, int i, int j)
        {
            if (start) elevation = 'a' - 'a';
            else if (end) elevation = 'z' - 'a';
            this.start = start;
            this.end = end;
            this.i = i;
            this.j = j;
        }
        public Square(int elevation, int i, int j)
        {
            this.elevation = elevation;
            this.i = i;
            this.j = j;
            start = elevation == 0;
        }
    }

    public static (int, int)[] directions = new (int, int)[] { (0, 1), (0, -1), (1, 0), (-1, 0) };
    private static void Main()
    {
        Dictionary<(int, int), Square> dic = new();
        foreach (var (line, i) in File.ReadLines("input.txt").Select((v, i) => (v, i)))
        {
            foreach (var (c, j) in line.Select((c, j) => (c, j)))
            {
                switch (c)
                {
                    case 'S':
                        dic.Add((i, j), new Square(true, false, i, j));
                        break;
                    case 'E':
                        dic.Add((i, j), new Square(false, true, i, j));
                        break;
                    default:
                        dic.Add((i, j), new Square(c - 'a', i, j));
                        break;
                }
            }
        }
        int shortestPath = int.MaxValue;
        foreach (var start in dic.Values.Where(s => s.start))
        {
            start.dist = 0;
            List<Square> next = new() { start };
            var distance = 0;
            bool end = false;
            while (!end && next.Count != 0)
            {
                distance++;
                List<Square> temp = new();
                foreach (var target in next.SelectMany(source =>
                        directions
                            .Select(dir => (dir.Item1 + source.i, dir.Item2 + source.j))
                            .Where(p => dic.ContainsKey(p))
                            .Select(p => dic[p])
                            .Where(p => source.elevation >= p.elevation - 1)))
                {
                    if (target.dist == int.MaxValue) temp.Add(target);
                    target.dist = Math.Min(target.dist, distance);
                    if (target.end) { end = true; break; }
                }
                next = temp;
            }
            if (end) shortestPath = Math.Min(shortestPath, distance);
            foreach (var sq in dic.Values) sq.dist = int.MaxValue;
        }
        Console.WriteLine($"part2: {shortestPath}");
    }
}