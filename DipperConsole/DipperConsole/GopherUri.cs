using System;
using System.Linq;

namespace DipperConsole
{
    class GopherUri
    {
        private Uri uri;
        public string Host
        {
            get {
                return uri.Host;
            }
        }

        public string Path { get; internal set; }

        public int Port
        {
            get {
                return uri.Port;
            }
        }

        public GopherItemType ItemType = GopherItemType.SubmenuDir;

        public static GopherUri Parse(string raw)
        {
            GopherUri guri = new GopherUri();
            var uri = new Uri(raw);
            guri.uri = uri;

            if (guri.uri.Segments.Length < 2) {
                guri.Path = uri.AbsolutePath;
                guri.ItemType = GopherItemType.SubmenuDir;
            }
            else {
                int gtype = uri.Segments[1][0];
                if (Enum.IsDefined(typeof(GopherItemType), gtype)) {
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
    }
}
