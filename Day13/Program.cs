using System.Text.RegularExpressions;

internal class Program
{

    public interface IPacket : IComparable
    {
        public bool isDivider();
        public IEnumerable<IPacket> GetEnumerable();

        public int GetSize();

        public static IPacket Parse(string s, bool divider)
        {
            if (char.IsDigit(s[0])) return new PacketSingle(int.Parse(s));
            List<IPacket> subpackets = new();
            string soFar = "";
            for (int i = 1; i < s.Length; i++)
            {
                switch (s[i])
                {
                    case ']':
                    case ',':
                        if (soFar != "")
                        {
                            subpackets.Add(new PacketSingle(int.Parse(soFar)));
                            soFar = "";
                        }
                        break;
                    case '[':
                        var matchingBracket = FindMatchingBracket(s, i);
                        subpackets.Add(Parse(s.Substring(i, matchingBracket), divider));
                        i += matchingBracket;
                        break;
                    default:
                        soFar += s[i];
                        break;
                }
            }
            return new PacketList(subpackets, divider);
        }
        private static int FindMatchingBracket(string s, int start)
        {
            var count = 0;
            foreach ((char c, int i) in s.Skip(start).Select((v, i) => (v, i)))
            {
                if (c == '[') count++;
                if (c == ']')
                {
                    count--;
                    if (count == 0) return i + 1;
                }
            }
            throw new Exception();
        }
    }

    public class PacketList : IPacket
    {
        private bool divider = false;

        protected List<IPacket> subpackets = new();

        public PacketList(List<IPacket> subpackets) { this.subpackets = subpackets; }
        public PacketList(List<IPacket> subpackets, bool divider) { this.subpackets = subpackets; this.divider = divider; }

        public int GetSize() => subpackets.Count;

        public IEnumerable<IPacket> GetEnumerable() => subpackets.AsEnumerable<IPacket>();

        public int CompareTo(object? otherObj)
        {
            IPacket other = (IPacket)otherObj;
            foreach (var (first, second) in this.GetEnumerable()
                .Zip(other.GetEnumerable()))
            {
                var comparison = first.CompareTo(second);
                if (comparison != 0) return comparison;
            }
            return GetSize().CompareTo(other.GetSize());
        }

        public bool isDivider() => divider;
    }

    public class PacketSingle : IPacket
    {
        protected readonly int value;
        public PacketSingle(int value) { this.value = value; }

        public int GetSize() => 1;

        public int GetValue() => value;

        public IEnumerable<IPacket> GetEnumerable() => Enumerable.Repeat(this, 1);

        public int CompareTo(object? otherObj)
        {
            return otherObj switch
            {
                PacketSingle end => value.CompareTo(end.GetValue()),
                PacketList other => -other.CompareTo(this),
                _ => throw new Exception(),
            };
        }
        public bool isDivider() => false;
    }

    private static void Main()
    {
        //pqrt1();

        List<IPacket> packets = new();
        foreach (string line in File.ReadLines("input.txt").Where(s => !string.IsNullOrWhiteSpace(s)))
            packets.Add(IPacket.Parse(line, false));
        packets.Add(IPacket.Parse("[[2]]", true));
        packets.Add(IPacket.Parse("[[6]]", true));
        packets.Sort();
        var key = packets.Select((v, i) => (v, i)).Where(v => v.v.isDivider()).Select(tuple => tuple.i).Aggregate((a, b) => a * b);
        Console.WriteLine($"part2: {key}");
    }

    private static void pqrt1()
    {
        List<(IPacket, IPacket)> pairs = new();
        foreach (string[] lines in File.ReadLines("input.txt").Chunk(3))
            pairs.Add((IPacket.Parse(lines[0], false), IPacket.Parse(lines[1], false)));
        var sum = 0;
        foreach (var ((p1, p2), i) in pairs.Select((v, i) => (v, i)))
        {
            if (p1.CompareTo(p2) < 0) sum += i + 1;
            Console.WriteLine($"pair {i + 1}: {p1.CompareTo(p2) < 0}");
        }
        Console.WriteLine($"part1: {sum}");
    }
}