﻿using System;
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
                    Console.WriteLine(">>> ERROR! Wrong URL!");
                    continue;
                }

                (int status, string response) = Downloader.OpenUrl(guri);

                if (status == 0) {
#if DEBUG 
                    string logFilename = $"log-{guri.Host}-{Environment.TickCount}.txt";
                    File.WriteAllText(logFilename, response);
#endif
                    if (guri.ItemType == GopherItemType.SubmenuDir)
                        Parser.DrawMenu(response);
                    else
                        Console.WriteLine(response);
                }
                else {
                    Console.WriteLine($">>> ERROR {status}!");
                    Console.WriteLine(response);
                }
            }
        }
    }
}
