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

                var t = ToGopherItem(line[0]);

                string[] tabbed = line.Split('\t');
                string textLine = tabbed[0].Substring(1);

                var stdForeColor = Console.ForegroundColor;
                switch (t) {
                    case GopherItemType.TextDocument:
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                        Console.Write($"[DOC] ");
                        Console.ForegroundColor = stdForeColor;
                        Console.Write($"{textLine}\t");
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.WriteLine($"{tabbed[2]}/0{tabbed[1]}");
                        Console.ForegroundColor = stdForeColor;
                        break;
                    case GopherItemType.SubmenuDir:
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                        Console.Write($"[DIR] ");
                        Console.ForegroundColor = stdForeColor;
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.WriteLine($"{tabbed[2]}/1{tabbed[1]}");
                        Console.ForegroundColor = stdForeColor;
                        break;
                    case GopherItemType.GifImage:
                    case GopherItemType.ImageFile:
                    case GopherItemType.PngFile:
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                        Console.Write($"[IMG] ");
                        Console.ForegroundColor = stdForeColor;
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine($"{tabbed[2]}/0{tabbed[1]}");
                        Console.ForegroundColor = stdForeColor;
                        break;

                    case GopherItemType.HtmlFile:
                        string htmlurl = tabbed[1].Replace("URL:", "");
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                        Console.Write($"[URL] ");
                        Console.ForegroundColor = stdForeColor;
                        Console.ForegroundColor = ConsoleColor.Blue;
                        Console.WriteLine($"{htmlurl}");
                        Console.ForegroundColor = stdForeColor;
                        break;

                    default:
                        Console.WriteLine($"      {textLine}");
                        break;
                }
            }
        }
        public static GopherItemType ToGopherItem(int ch)
        {
            return Enum.IsDefined(typeof(GopherItemType), ch) ?
                (GopherItemType)ch :
                GopherItemType.None;
        }

    }
}
