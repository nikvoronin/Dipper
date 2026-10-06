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

            if (ImageRenderer.IsImage(guri.ItemType)) {
                await ShowImageAsync(guri);
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

    private static async Task ShowImageAsync(GopherUri guri)
    {
        var (status, data, error) = await Downloader.FetchAsync(guri);

        if (status != 0) {
            Console.WriteLine($">>> ERROR {status}!");
            Console.WriteLine(error);
            return;
        }

        try {
            ImageRenderer.Draw(data);
        }
        catch (Exception ex) {
            Console.ResetColor();
            Console.WriteLine($">>> ERROR {IMAGE_ERROR_CODE}!");
            Console.WriteLine(ex.Message);
        }
    }

    const int IMAGE_ERROR_CODE = 400;
}
