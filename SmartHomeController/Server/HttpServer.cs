using Sonos.Client;
using Sonos.Client.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking.Sockets;
using Windows.Storage;
using Windows.Storage.Streams;

namespace SmartHomeController.Server
{
    public class HttpServer : IDisposable
    {
        private const uint BufferSize = 18192;
        private static readonly StorageFolder LocalFolder
                     = Windows.ApplicationModel.Package.Current.InstalledLocation;

        private readonly StreamSocketListener listener;

        private SonosClient sonosClient;

        public HttpServer(int port, SonosClient sonosClient)
        {
            this.sonosClient = sonosClient;

            this.listener = new StreamSocketListener();
            this.listener.ConnectionReceived += (s, e) => ProcessRequestAsync(e.Socket);
            this.listener.BindServiceNameAsync(port.ToString());
        }

        public void Dispose()
        {
            this.listener.Dispose();
        }

        private async void ProcessRequestAsync(StreamSocket socket)
        {
            // this works for text only
            StringBuilder request = new StringBuilder();
            using (IInputStream input = socket.InputStream)
            {
                byte[] data = new byte[BufferSize];
                IBuffer buffer = data.AsBuffer();
                uint dataRead = BufferSize;
                while (dataRead == BufferSize)
                {
                    await input.ReadAsync(buffer, BufferSize, InputStreamOptions.Partial);
                    request.Append(Encoding.UTF8.GetString(data, 0, data.Length));
                    dataRead = buffer.Length;
                }
            }

            using (IOutputStream output = socket.OutputStream)
            {
                string requestString = request.ToString();
                string requestMethod = requestString.Split('\n')[0];
                string[] requestParts = requestMethod.Split(' ');
                string requestMethodShort = requestParts[0];
                int index = requestString.LastIndexOf("\r\n\r\n");
                string body = requestString.Substring(index + 4, requestString.Length - index - 4);
                body = body.Replace("\0", "");


                if (requestMethodShort == "NOTIFY")
                {
                    Event notification = await sonosClient.ParseNotification(body);
                    //Propertyset set = await sonosClient.ParseZoneGroupTopologyNotification(body);

                    await WriteResponseAsync(requestParts[1], output);
                }
                else
                    throw new InvalidDataException("HTTP method not supported: "
                                                   + requestParts[0]);
            }
        }

        private async Task WriteResponseAsync(string path, IOutputStream os)
        {
            using (Stream resp = os.AsStreamForWrite())
            {
                bool exists = true;
                try
                {
                    // Look in the Data subdirectory of the app package
                    string filePath = "Data" + path.Replace('/', '\\');
                    string result = "test";
                    var s = new StringReader(result);
                    using (Stream fs = new MemoryStream(Encoding.UTF8.GetBytes(result)))
                    {
                        string header = String.Format("HTTP/1.1 200 OK\r\n" +
                                        "Content-Length: {0}\r\n" +
                                        "Connection: close\r\n\r\n",
                                        fs.Length);
                        byte[] headerArray = Encoding.UTF8.GetBytes(header);
                        await resp.WriteAsync(headerArray, 0, headerArray.Length);
                        await fs.CopyToAsync(resp);
                    }
                }
                catch (FileNotFoundException)
                {
                    exists = false;
                }

                if (!exists)
                {
                    byte[] headerArray = Encoding.UTF8.GetBytes(
                                          "HTTP/1.1 404 Not Found\r\n" +
                                          "Content-Length:0\r\n" +
                                          "Connection: close\r\n\r\n");
                    await resp.WriteAsync(headerArray, 0, headerArray.Length);
                }

                await resp.FlushAsync();
            }
        }
    }

}
