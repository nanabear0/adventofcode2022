using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using static Program;

internal class Program
{
    static long lcm(long a, long b)
    {
        return Math.Abs(a * b) / gcd(a, b);
    }
    static long gcd(long a, long b)
    {
        return b == 0 ? a : gcd(b, a % b);
    }

    public class Monke
    {
        protected static long monkeFactor = 1;
        public Queue<long> items = new();

        private int index;
        private int trueTarget;
        private int falseTarget;
        private char op;
        private long opValue;
        private long testValue;
        public long monkeyBusiness;
        public int getIndex()
        {
            return index;
        }

        protected Monke(int index, string itemsStr, char op, long opValue, long testValue, int trueTarget, int falseTarget)
        {
            this.index = index;
            Queue<long> items = new();
            foreach (long item in itemsStr.Split(", ").Select(long.Parse))
                items.Enqueue(item);
            this.items = items;
            this.trueTarget = trueTarget;
            this.falseTarget = falseTarget;
            this.op = op;
            this.opValue = opValue;
            this.testValue = testValue;
            Monke.monkeFactor = lcm(Monke.monkeFactor, testValue);
        }

        public (int, long)? ooga()
        {
            if (items.Count == 0) return null;
            monkeyBusiness++;
            var newValue = items.Dequeue();
            switch (op)
            {
                case '+':
                    newValue += opValue == -1 ? newValue : opValue;
                    break;
                case '*':
                    newValue *= opValue == -1 ? newValue : opValue;
                    break;
            }
            newValue %= monkeFactor;
            var returnValue = ((newValue % testValue == 0) ? trueTarget : falseTarget, newValue);
            return returnValue;
        }

        public void booga(long value)
        {
            items.Enqueue(value);
        }

        public static Monke Parse(string line)
        {
            Regex reggy = new Regex(@"Monkey ([0-9]+):\s+Starting items: ([0-9, ]+)\s+Operation: new = old ([+*]{1}) ([0-9]+|old)\s+Test: divisible by ([0-9]+)\s+If true: throw to monkey ([0-9]+)\s+If false: throw to monkey ([0-9]+)");
            var groups = reggy.Match(line).Groups;
            Monke monke = new(
                int.Parse(groups[1].Value),
                groups[2].Value,
                groups[3].Value[0],
                groups[4].Value == "old" ? -1 : long.Parse(groups[4].Value),
                long.Parse(groups[5].Value),
                int.Parse(groups[6].Value),
                int.Parse(groups[7].Value));
            return /*to*/ monke;
        }
    }

    private static void Main(string[] args)
    {
        SortedDictionary<int, Monke> monkes = new();
        foreach (Monke monke in File.ReadAllText("input.txt").Split("\r\n\r\n").Select(Monke.Parse))
            monkes[monke.getIndex()] = monke;
        foreach (var i in Enumerable.Range(1, 10000))
        {
            foreach (Monke monke in monkes.Values)
            {
                while (true)
                {
                    (int, long)? item = monke.ooga();
                    if (item.HasValue)
                        monkes[item.Value.Item1].booga(item.Value.Item2);
                    else break;
                }
            }
        }
        var levelOfMonkeyBusiness = monkes.Values.Select(m => m.monkeyBusiness).OrderByDescending(v => v).Take(2).Aggregate((m1, m2) => m1 * m2);
        Console.WriteLine($"part2: {levelOfMonkeyBusiness}");
    }
}