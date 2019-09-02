using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DipperConsole
{
    class Parser
    {
        public static string BuildMenu(string raw)
        {
            StringBuilder b = new StringBuilder();

            string[] lines = raw.Split('\n');
            foreach(var line in lines) {
                if (line.Length == 0 || (line.Length == 1 && line[0] == '.'))
                    break;

                char t = line[0];

                string[] tabbed = line.Split('\t');
                string textLine = tabbed[0].Substring(1);

                switch (t) {
                    case '0':
                        b.Append($"[DOC] {textLine}\t{tabbed[2]}{tabbed[1]}\n");
                        break;
                    case '1':
                        b.Append($"[DIR] {textLine}\t{tabbed[2]}{tabbed[1]}\n");
                        break;
                    default:
                        b.Append($"      {textLine}\n");
                        break;
                }
            }

            return b.ToString();
        }
    }
}
