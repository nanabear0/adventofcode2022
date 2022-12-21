using System.Collections;
using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;

internal class Program
{

    class Monke2
    {
        public static Dictionary<string, Monke2> veryManyMonkes = new();
        public string name;
        public char operation = '.';
        public string monke1 = "";
        public string monke2 = "";
        public long number = 0;
        public Monke2(string name, char operation, string monke1, string monke2)
        {
            this.name = name;
            if (this.name == "root")
                this.operation = '=';
            else
                this.operation = operation;
            this.monke1 = monke1;
            this.monke2 = monke2;
        }
        public Monke2(string name, long number)
        {
            this.name = name;
            this.number = number;
            this.operation = '.';
        }

        public long speak()
        {
            if (operation == '.') return number;
            else
            {
                var v1 = veryManyMonkes[monke1].speak();
                var v2 = veryManyMonkes[monke2].speak();
                if (operation == '=') Console.WriteLine($"{Monke2.veryManyMonkes["humn"].number} : {v1 - v2}"); // debug
                return operation switch
                {
                    '.' => number,
                    '+' => v1 + v2,
                    '-' => v1 - v2,
                    '*' => v1 * v2,
                    '/' => v1 / v2,
                    '=' => v1 == v2 ? 1 : 0
                };
            }
        }
    }
    private static void Main(string[] args)
    {
        foreach (string line in File.ReadLines("input.txt"))
        {
            Regex reggy = new(@"([a-z]+): (?:([0-9]+)|([a-z]+) ([\-\*\/\+]) ([a-z]+))");
            var groups = reggy.Match(line).Groups.Values.Skip(1).ToList();
            var name = groups[0].Value;
            if (groups[3].Success)
                Monke2.veryManyMonkes.Add(name, new Monke2(name, groups[3].Value[0], groups[2].Value, groups[4].Value));
            else
                Monke2.veryManyMonkes.Add(name, new Monke2(name, long.Parse(groups[1].Value)));
        }
        //part1();
        part2();
    }
    private static void part2()
    {
        long i = 3349136380000; // magic number dont question.
        for (; ; i++)
        {
            Monke2.veryManyMonkes["humn"].number = i;
            var result = Monke2.veryManyMonkes["root"].speak();
            if (result == 1) break;
        }
        Console.WriteLine($"part2: {i}");
    }

    private static void part1()
    {
        Console.WriteLine($"part1: {Monke2.veryManyMonkes["root"].speak()}");
    }
}