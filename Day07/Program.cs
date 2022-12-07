using System.Text.RegularExpressions;

internal class Program
{
    public interface Entry
    {
        public int getSize();
        public string getName();
    }

    public class Directory : Entry
    {
        public string _name;

        public Directory parent;

        public Directory(string name, Directory parent)
        {
            _name = name;
            this.parent = parent;
        }

        public int getSize()
        {
            return entries.Select(e => e.getSize()).Sum();
        }

        public string getName()
        {
            return _name;
        }

        public List<Entry> entries = new List<Entry>();
    }

    public class File : Entry
    {
        public int _size;
        public string _name;

        public File(string name, int size)
        {
            _name = name;
            _size = size;
        }

        public int getSize()
        {
            return _size;
        }

        public string getName()
        {
            return _name;
        }
    }
    private static void Main(string[] args)
    {
        Directory root = new Directory("/", null);
        parseInput(root);
        List<int> output = new();
        traverseDirectories(root, output);
        Console.WriteLine($"part1: {output.Where(v => v <= 100000).Sum()}");
        int spaceNeeded = 30000000 - (70000000 - root.getSize());
        output.Sort();
        Console.WriteLine($"part2: {output.First(v => v > spaceNeeded)}");
    }

    public static void traverseDirectories(Directory cd, List<int> output)
    {
        output.Add(cd.getSize());
        foreach (Directory d in cd.entries.Where(v => v is Directory)) traverseDirectories(d, output);
    }

    private static void parseInput(Directory root)
    {
        Directory currentDir = root;
        foreach (string line in System.IO.File.ReadLines("input.txt"))
        {
            Regex reggy = new(@"\$ cd ([a-zA-Z]+|\.\.|/)|\$ (ls)|dir ([a-zA-Z]+)|([0-9]+) (.+)");
            var match = reggy.Match(line);
            if (!String.IsNullOrWhiteSpace(match.Groups[1].Value))
            {
                string dirName = match.Groups[1].Value;
                switch (dirName)
                {
                    case "..":
                        currentDir = currentDir.parent;
                        break;
                    case "/":
                        currentDir = root;
                        break;
                    default:
                        currentDir = (Directory)currentDir.entries.First(x => x.getName().Equals(dirName));
                        break;
                }
            }
            else if (!String.IsNullOrWhiteSpace(match.Groups[3].Value))
            {
                string dirName = match.Groups[3].Value;
                currentDir.entries.Add(new Directory(dirName, currentDir));
            }
            else if (!String.IsNullOrWhiteSpace(match.Groups[4].Value))
            {
                int size = int.Parse(match.Groups[4].Value);
                string fileName = match.Groups[5].Value;
                currentDir.entries.Add(new File(fileName, size));
            }
        }
    }
}