using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DipperConsole
{
    class Parser
    {
        public static void DrawMenu(string raw)
        {
            string[] lines = raw.Split('\n');
            foreach(var line in lines) {
                if (line.Length == 0 || (line.Length == 1 && line[0] == '.'))
                    break;

                char t = line[0];

                string[] tabbed = line.Split('\t');
                string textLine = tabbed[0].Substring(1);

                var stdForeColor = Console.ForegroundColor;
                switch (t) {
                    case '0':
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                        Console.Write($"[DOC] ");
                        Console.ForegroundColor = stdForeColor;
                        Console.Write($"{textLine}\t");
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.WriteLine($"{tabbed[2]}/0{tabbed[1]}");
                        Console.ForegroundColor = stdForeColor;
                        break;
                    case '1':
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                        Console.Write($"[DIR] ");
                        Console.ForegroundColor = stdForeColor;
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.WriteLine($"{tabbed[2]}{tabbed[1]}");
                        Console.ForegroundColor = stdForeColor;
                        break;
                    default:
                        Console.WriteLine($"      {textLine}");
                        break;
                }
            }
        }
    }
}
