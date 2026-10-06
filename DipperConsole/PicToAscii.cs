// Source: https://github.com/nikvoronin/Asceils (MIT, Copyright (c) 2019 Nikolai Voronin)
// Image decoding is done by StbImageSharp, resizing is custom (box averaging).
using StbImageSharp;
using System.Text;

namespace Asceils;

public class PicToAscii
{
    public readonly PicToAsciiOptions Options = new();

    private PicToAscii() { }
    public static PicToAscii CreateDefault => new();

    public PicToAscii(PicToAsciiOptions options)
    {
        Options = options;
    }

    public IReadOnlyList<ColorTape> Convert(Stream stream)
    {
        ImageResult source = ImageResult.FromStream(stream, ColorComponents.RedGreenBlue);

        int width, height;
        var sourceAspect = (float)source.Width / source.Height;

        switch (Options.FixedDimension)
        {
            case PicToAsciiOptions.Fix.Vertical:
                height = Options.FixedSize;
                width = (int)Math.Round(Options.FixedSize * sourceAspect / Options.SymbolAspectRatio);
                break;

            default:
            case PicToAsciiOptions.Fix.Horizontal:
                width = Options.FixedSize;
                height = (int)Math.Round(Options.FixedSize / sourceAspect * Options.SymbolAspectRatio);
                break;
        }

        return ConvertInternal(source, Math.Max(1, width), Math.Max(1, height));
    }

    /// <summary>
    /// Returns a list of the colored string chunks
    /// </summary>
    /// <param name="source">Bitmap source image</param>
    /// <param name="width">Result width in symbols</param>
    /// <param name="height">Result height in symbols</param>
    /// <returns>Colored tapes ready to print to the console</returns>
    private IReadOnlyList<ColorTape> ConvertInternal(ImageResult source, int width, int height)
    {
        var reduced = Resize(source, width, height);

        var chunks = new List<ColorTape>();

        var chunkBuilder = new StringBuilder();
        var lastColor = ConsoleColor.Black;

        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
            {
                var rgb = reduced[y * width + x];
                var cc = ToConsoleColor(rgb);

                if (lastColor != cc)
                {
                    if (chunkBuilder.Length > 0)
                    {
                        var tape = new ColorTape(chunkBuilder.ToString(), lastColor);
                        chunks.Add(tape);

                        chunkBuilder.Clear();
                    }

                    lastColor = cc;
                }

                var bright = Lightness(rgb);
                var symbol = BrightnessToChar(bright, Options.AsciiTable);
                chunkBuilder.Append(symbol);
            }

            chunkBuilder.Append(Environment.NewLine);
        }

        if (chunkBuilder.Length > 0)
            chunks.Add(new ColorTape(chunkBuilder.ToString(), lastColor));

        return chunks;
    }

    /// <summary>
    /// Box resize: every result pixel is the average of the source rectangle it covers.
    /// When upscaling the rectangle is at least one pixel (nearest neighbour).
    /// </summary>
    private static Rgb[] Resize(ImageResult src, int width, int height)
    {
        const int COMPONENTS = 3; // RGB
        var result = new Rgb[width * height];

        for (var y = 0; y < height; y++)
        {
            var y0 = (int)((long)y * src.Height / height);
            var y1 = Math.Max(y0 + 1, (int)((long)(y + 1) * src.Height / height));

            for (var x = 0; x < width; x++)
            {
                var x0 = (int)((long)x * src.Width / width);
                var x1 = Math.Max(x0 + 1, (int)((long)(x + 1) * src.Width / width));

                long r = 0, g = 0, b = 0;
                for (var sy = y0; sy < y1; sy++)
                {
                    var i = (sy * src.Width + x0) * COMPONENTS;

                    for (var sx = x0; sx < x1; sx++)
                    {
                        r += src.Data[i++];
                        g += src.Data[i++];
                        b += src.Data[i++];
                    }
                }

                var count = (y1 - y0) * (x1 - x0);
                result[y * width + x] =
                    new Rgb((byte)(r / count), (byte)(g / count), (byte)(b / count));
            }
        }

        return result;
    }

    // HSL lightness: (max + min) / 2, normalized to 0..1
    private static float Lightness(Rgb c)
    {
        var max = Math.Max(c.R, Math.Max(c.G, c.B));
        var min = Math.Min(c.R, Math.Min(c.G, c.B));

        return (max + min) / 510f;
    }

    private static char BrightnessToChar(float bright, string symbols)
    {
        var charIndex = (int)(bright * (symbols.Length - 1));

        return symbols[charIndex];
    }

    private ConsoleColor ToConsoleColor(Rgb c)
    {
        // bright bit
        var index = (
              c.R > Options.Threshold_RedBright
            | c.G > Options.Threshold_GreenBright
            | c.B > Options.Threshold_BlueBright
            ) ? 8 : 0;

        // color bits
        var t = Options.Threshold_ValuableColor;
        var max = Math.Max(c.R, Math.Max(c.G, c.B));
        index |= (c.R / max > t) ? 4 : 0;
        index |= (c.G / max > t) ? 2 : 0;
        index |= (c.B / max > t) ? 1 : 0;

        return (ConsoleColor)index;
    }
}

public class PicToAsciiOptions
{
    // sorted ascending by brightness: darker --> lighter
    public const string ASCIITABLE_SOLID = " ░▒▓█";
    public const string ASCIITABLE_SYMBOLIC = " `'.,:;i+o*%&$#@";
    public const string ASCIITABLE_SYMBOLIC_LIGHT = " `'.,:;i+o*wW%&$#@▒▓█";

    public float Threshold_ValuableColor = .8f;
    public int Threshold_RedBright = 200;
    public int Threshold_GreenBright = 170;
    public int Threshold_BlueBright = 220;

    public enum Fix { Horizontal = 0, Vertical }
    public Fix FixedDimension = Fix.Horizontal;
    public int FixedSize = 80;

    public string AsciiTable = ASCIITABLE_SOLID;
    public float SymbolAspectRatio = .5f;
}

internal readonly record struct Rgb(byte R, byte G, byte B);

public class ColorTape(string chunk, ConsoleColor color)
{
    public ConsoleColor ForeColor = color;
    public string Chunk = chunk;
}
