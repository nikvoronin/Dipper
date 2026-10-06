using System.Net.Sockets;
using System.Text;

namespace DipperConsole;

public static class Downloader
{
    public static async Task<(int, string)> OpenUrlAsync(GopherUri guri)
    {
        var (status, data, error) = await FetchAsync(guri);

        return status == 0
            ? (status, Encoding.ASCII.GetString(data))
            : (status, error);
    }

    /// <summary>
    /// Downloads the raw bytes of the response. Error is filled when status is not 0.
    /// </summary>
    public static async Task<(int Status, byte[] Data, string Error)> FetchAsync(GopherUri guri)
    {
        using var client = new TcpClient();

        try
        {
            using var connectCts = new CancellationTokenSource(CLIENT_TIMEOUT_MS);
            await client.ConnectAsync(guri.Host, guri.Port, connectCts.Token);
        }
        catch (Exception ex)
        {
            return (100, [], ex.Message);
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
            return (200, [], ex.Message);
        }

        try
        {
            var mem = await ReadResponseAsync(stream);

            return mem.Length > 0
                ? (0, mem.ToArray(), "")
                : (-1, [], "");
        }
        catch (Exception ex)
        {
            return (300, [], ex.Message);
        }
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
