namespace DipperConsole;

public sealed class GopherUri
{
    public string Host => uri.Host;

    public string Path { get; internal set; } = "";

    public int Port => uri.Port;

    public GopherItemType ItemType = GopherItemType.SubmenuDir;

    public static GopherUri Parse(string raw)
    {
        if (!raw.StartsWith(GopherScheme))
            raw = GopherScheme + raw;

        var guri = new GopherUri();
        var uri = new Uri(raw);
        guri.uri = uri;

        if (guri.uri.Segments.Length < 2) {
            guri.Path = uri.AbsolutePath;
            guri.ItemType = GopherItemType.SubmenuDir;
        }
        else {
            int gtype = uri.Segments[1][0];
            if (Enum.IsDefined((GopherItemType)gtype)) {
                guri.ItemType = (GopherItemType)gtype;

                string pathBody = string.Join("", guri.uri.Segments, 2, guri.uri.Segments.Length - 2);
                if (uri.Segments[1].Length > 2)
                    guri.Path = $"/{uri.Segments[1].Substring(1)}/{pathBody}";
                else
                    guri.Path = $"/{pathBody}";
            }
            else {
                guri.Path = uri.AbsolutePath;
                guri.ItemType = GopherItemType.SubmenuDir;
            }
        }

        return guri;
    }

    private Uri uri = null!;
    const string GopherScheme = "gopher://";
}
