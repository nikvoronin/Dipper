using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DipperConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Dipper. Sharp# gopher browser.");

            bool run = true;
            while(run) {
                Console.Write("> ");
                string url = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(url))
                    break;

                GopherUri guri;

                try {
                    guri = GopherUri.Parse(url);
                }
                catch {
                    Console.WriteLine(">>> ERROR!!! Wrong URL!");
                    continue;
                }

                (int status, string response) = Downloader.OpenUrl(guri);

                if (status == 0) {
                    string logFilename = $"log-{guri.Host}-{Environment.TickCount}.txt";
                    File.WriteAllText(logFilename, response);

                    string text =
                        (guri.ItemType == GopherItemType.SubmenuDir) ?
                            Parser.Clarify(response) :
                            response;
                    Console.WriteLine(text);
                }
                else {
                    Console.WriteLine($">>> ERROR!!! #{status}");
                    Console.WriteLine(response);
                }
            }
        }
    }
}
