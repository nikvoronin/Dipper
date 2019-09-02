using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DipperConsole
{
    class Downloader
    {
        const int CLIENT_TIMEOUT_MS = 10000;
        const int SLEEP_BETWEEN_REQ_RESP_MS = 100;
        const int RECEIVE_BUFFER_BYTESIZE = 2048;

        public static (int, string) OpenUrl(GopherUri guri)
        {
            string response = "";
            int status = -1;

            TcpClient client;
            try {
                client = new TcpClient(guri.Host, guri.Port);
            }
            catch (Exception ex) {
                response = ex.Message;
                status = 100;
                return (status, response);
            }

            client.ReceiveTimeout = client.SendTimeout = CLIENT_TIMEOUT_MS;

            using (NetworkStream stream = client.GetStream()) {
                try {
                    string request = guri.Path;
                    if (!request.Contains("\n"))
                        request += "\n";

                    byte[] requestBytes = Encoding.ASCII.GetBytes(request);
                    stream.Write(requestBytes, 0, requestBytes.Length);
                }
                catch (Exception ex) {
                    response = ex.Message;
                    status = 200;
                    return (status, response);
                }

                Thread.Sleep(SLEEP_BETWEEN_REQ_RESP_MS);

                try {
                    MemoryStream mem = ReadResponse(stream);
                    if (mem.Length > 0) {
                        response = Encoding.ASCII.GetString(mem.ToArray());
                        status = 0;
                    }
                }
                catch (Exception ex) {
                    response = ex.Message;
                    status = 300;
                    return (status, response);
                }
            }

            return (status, response);
        }

        private static MemoryStream ReadResponse(NetworkStream stream)
        {
            MemoryStream mem = new MemoryStream();
            byte[] buffer = new byte[RECEIVE_BUFFER_BYTESIZE];

            try {
                while(true) {
                    int received = stream.Read(buffer, 0, buffer.Length);

                    if (received <= 0)
                        break;

                    mem.Write(buffer, 0, received);
                } 
            }
            catch { }

            return mem;
        }
    } // class
}
