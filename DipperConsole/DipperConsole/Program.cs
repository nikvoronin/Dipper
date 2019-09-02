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
            Console.WriteLine("Dipper. The sharp# Gopher browser.");

            bool run = true;
            while(run) {
                Console.Write("> ");
                string address = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(address))
                    break;

                GopherUri guri;

                try {
                    guri = GopherUri.Parse(address);
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
                            Parser.BuildMenu(response) :
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
