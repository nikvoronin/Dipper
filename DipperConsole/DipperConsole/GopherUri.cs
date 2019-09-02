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
            guri.uri = new Uri(raw);

            if (guri.uri.Segments.Length < 2 && guri.uri.Segments[1].Length <= 2) {
                guri.Path = guri.uri.AbsolutePath;
                guri.ItemType = GopherItemType.SubmenuDir;
            }
            else {
                int gtype = guri.uri.Segments[1][0];
                if (Enum.IsDefined(typeof(GopherItemType), gtype)) {
                    guri.ItemType = (GopherItemType)gtype;
                    guri.Path = "/" + string.Join("", guri.uri.Segments, 2, guri.uri.Segments.Length - 2);
                }
                else
                    guri.ItemType = GopherItemType.SubmenuDir;
            }

            return guri;
        }
    }
}
