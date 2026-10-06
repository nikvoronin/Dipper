namespace DipperConsole;

public sealed class Parser
{
    public static void DrawMenu(string raw)
    {
        var lines = raw.Split('\n');

        foreach(var rawLine in lines) {
            // servers may end lines with CRLF, so the terminator is ".\r"
            var line = rawLine.TrimEnd('\r');

            if (line.Length == 0
                || (line.Length == 1
                    && line[0] == '.'))
            {
                break;
            }

            var t = ToGopherItem(line[0]);

            var tabbed = line.Split('\t');
            var textLine = tabbed[0].Substring(1);

            var stdForeColor = Console.ForegroundColor;
            switch (t) {
                case GopherItemType.TextDocument:
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.Write($"[DOC] ");
                    Console.ForegroundColor = stdForeColor;
                    Console.Write($"{textLine}\t");
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine(ToAddress(tabbed, '0'));
                    Console.ForegroundColor = stdForeColor;
                    break;

                case GopherItemType.SubmenuDir:
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.Write($"[DIR] ");
                    Console.ForegroundColor = stdForeColor;
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine(ToAddress(tabbed, '1'));
                    Console.ForegroundColor = stdForeColor;
                    break;

                case GopherItemType.GifImage:
                case GopherItemType.ImageFile:
                case GopherItemType.PngFile:
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.Write($"[IMG] ");
                    Console.ForegroundColor = stdForeColor;
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine(ToAddress(tabbed, line[0]));
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

    public static GopherItemType ToGopherItem(int ch) =>
        Enum.IsDefined((GopherItemType)ch)
            ? (GopherItemType)ch
            : GopherItemType.None;

    // host[:port]/<type><selector>; the port is omitted when it is the default one
    private static string ToAddress(string[] tabbed, char type)
    {
        var host = tabbed[2];
        if (tabbed.Length > 3
            && int.TryParse(tabbed[3].Trim(), out var port)
            && port != DEFAULT_GOPHER_PORT)
        {
            host += $":{port}";
        }

        return $"{host}/{type}{tabbed[1]}";
    }

    const int DEFAULT_GOPHER_PORT = 70;
}
