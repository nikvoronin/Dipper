namespace DipperConsole;

public class Program
{
    public static async Task Main(string[] args)
    {
        Console.WriteLine("Dipper. The sharp# Gopher browser.");

        var run = true;
        while(run) {
            Console.Write("> ");
            var address = Console.ReadLine();
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

            (int status, string response) = await Downloader.OpenUrlAsync(guri);

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
