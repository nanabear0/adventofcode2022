internal class Program
{
    public class RingNode
    {
        public RingNode next;
        public RingNode previous;
        public long value;
        public RingNode(long value)
        {
            this.next = this;
            this.previous = this;
            this.value = value;
        }
        public RingNode(long value, RingNode previous)
        {
            previous.next.previous = this;
            this.next = previous.next;
            previous.next = this;
            this.previous = previous;

            this.value = value;
        }
        public void shift(int size)
        {
            if (value > 0) forward(this.value % (size - 1));
            else if (value < 0) backward(this.value % (size - 1));
        }

        public void forward(long shiftme)
        {
            if (shiftme == 0) return;
            this.next.previous = this.previous;
            this.previous.next = this.next;
            var after = this.next;
            after.next.previous = this;
            this.next = after.next;
            after.next = this;
            this.previous = after;

            forward(shiftme - 1);
        }
        public void backward(long shiftme)
        {
            if (shiftme == 0) return;
            this.previous.next = this.next;
            this.next.previous = this.previous;
            var before = this.previous;
            before.previous.next = this;
            this.previous = before.previous;
            before.previous = this;
            this.next = before;

            backward(shiftme + 1);
        }
    }
    private static void Main(string[] args)
    {
        long decriptionKey = 811589153;
        RingNode current = null;
        SortedDictionary<int, RingNode> nodePointers = new();
        foreach ((long n, int i) in File.ReadLines("input.txt").Select(line => long.Parse(line)).Select((v, i) => (v * decriptionKey, i + 1)))
        {
            if (current == null) current = new(n);
            else current = new(n, current);
            nodePointers[i] = current;
        }
        for (int i = 0; i < 10; i++)
            foreach (var node in nodePointers.Values)
                node.shift(nodePointers.Count);

        var ordered = new List<RingNode>();
        var size = nodePointers.Count;
        var temp = nodePointers.Values.Where(x => x.value == 0).First();
        foreach (var _ in nodePointers.Values)
        {
            ordered.Add(temp);
            temp = temp.next;
        }
        Console.WriteLine($"part1: {ordered[1000 % size].value + ordered[2000 % size].value + ordered[3000 % size].value}");
    }

    private static void print(SortedDictionary<int, RingNode> nodePointers)
    {
        var temp = nodePointers.Values.Where(x => x.value == 0).First();
        for (int i = 0; i < nodePointers.Values.Count; i++)
        {
            Console.Write($"{temp.value},");
            temp = temp.next;
        }
        Console.WriteLine();
    }
}