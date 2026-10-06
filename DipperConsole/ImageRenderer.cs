using Asceils;
using System.Text;

namespace DipperConsole;

static class ImageRenderer
{
    const float FONT_ASPECT = 8f / 12f; // symbol width divided by height in pixels
    const int DEFAULT_CONSOLE_W = 80;   // console width in chars when it is unknown

    public static bool IsImage(GopherItemType type) =>
        type is GopherItemType.ImageFile
            or GopherItemType.PngFile
            or GopherItemType.GifImage;

    public static void Draw(byte[] data)
    {
        var options = new PicToAsciiOptions {
            FixedDimension = PicToAsciiOptions.Fix.Horizontal,
            FixedSize = ConsoleWidth(),
            SymbolAspectRatio = FONT_ASPECT
        };

        // block symbols (░▒▓█) are not available in every OEM codepage
        Console.OutputEncoding = Encoding.UTF8;

        using var stream = new MemoryStream(data);
        var tapes = new PicToAscii(options).Convert(stream);

        foreach (var tape in tapes) {
            Console.ForegroundColor = tape.ForeColor;
            Console.Write(tape.Chunk);
        }

        Console.ResetColor();
    }

    private static int ConsoleWidth()
    {
        if (Console.IsOutputRedirected)
            return DEFAULT_CONSOLE_W;

        try {
            return Math.Max(1, Console.WindowWidth - 1);
        }
        catch (IOException) {
            return DEFAULT_CONSOLE_W;
        }
    }
}
