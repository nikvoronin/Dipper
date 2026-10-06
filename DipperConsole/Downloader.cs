using System.Net.Sockets;
using System.Text;

namespace DipperConsole;

public static class Downloader
{
    public static async Task<(int, string)> OpenUrlAsync(GopherUri guri)
    {
        var response = "";
        var status = -1;

        using var client = new TcpClient();

        try
        {
            using var connectCts = new CancellationTokenSource(CLIENT_TIMEOUT_MS);
            await client.ConnectAsync(guri.Host, guri.Port, connectCts.Token);
        }
        catch (Exception ex)
        {
            response = ex.Message;
            status = 100;

            return (status, response);
        }

        using var stream = client.GetStream();

        try
        {
            var request = guri.Path;
            if (!request.Contains('\n'))
                request += "\n";

            var requestBytes = Encoding.ASCII.GetBytes(request);

            using var writeCts = new CancellationTokenSource(CLIENT_TIMEOUT_MS);
            await stream.WriteAsync(requestBytes, writeCts.Token);
        }
        catch (Exception ex)
        {
            response = ex.Message;
            status = 200;

            return (status, response);
        }

        try
        {
            var mem = await ReadResponseAsync(stream);
            if (mem.Length > 0)
            {
                response = Encoding.ASCII.GetString(mem.ToArray());
                status = 0;
            }
        }
        catch (Exception ex)
        {
            response = ex.Message;
            status = 300;

            return (status, response);
        }

        return (status, response);
    }

    private static async Task<MemoryStream> ReadResponseAsync(NetworkStream stream)
    {
        var mem = new MemoryStream();
        var buffer = new byte[RECEIVE_BUFFER_BYTESIZE];

        try
        {
            while (true)
            {
                // timeout applies to each single read, like ReceiveTimeout did
                using var readCts = new CancellationTokenSource(CLIENT_TIMEOUT_MS);
                var received = await stream.ReadAsync(buffer, readCts.Token);

                if (received <= 0)
                    break;

                mem.Write(buffer, 0, received);
            }
        }
        catch { }

        return mem;
    }

    const int CLIENT_TIMEOUT_MS = 10000;
    const int RECEIVE_BUFFER_BYTESIZE = 2048;
}
