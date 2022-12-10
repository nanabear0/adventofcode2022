internal class Program
{
    private static void Main(string[] args)
    {
        parto1();
        parto2();
    }
    private static void parto2()
    {
        int cycle = 0;
        int value = 1;
        List<char> crt = new List<char>();
        render(cycle, value, crt);
        foreach (string line in File.ReadLines("input.txt"))
        {
            var splitted = line.Split(' ');
            if (splitted.Length == 2)
            {
                cycle += 1;
                render(cycle, value, crt);
                value += int.Parse(splitted[1]);
            }
            cycle += 1;
            render(cycle, value, crt);
        }
        Console.WriteLine($"part2:");
        Console.WriteLine(String.Join('\n', crt.Chunk(40).Select(v => new String(v))));
    }

    private static void render(int cycle, int value, List<char> crt)
    {
        if (Math.Abs(cycle % 40 - value) < 2) crt.Add('#'); else crt.Add('.');
    }

    private static void parto1()
    {
        int cycle = 1;
        int value = 1;
        int solution = 0;
        foreach (string line in File.ReadLines("input.txt"))
        {
            var splitted = line.Split(' ');
            if (splitted.Length == 1)
            {
                cycle++;
            }
            else
            {
                if ((cycle + 1 - 20) % 40 == 0)
                    solution += value * (cycle + 1);

                value += int.Parse(splitted[1]);
                cycle += 2;
            }

            if ((cycle - 20) % 40 == 0)
                solution += value * cycle;
        }
        Console.WriteLine($"part1: {solution}");
    }
}