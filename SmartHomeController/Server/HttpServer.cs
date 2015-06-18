using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;

namespace SmartHomeController.Server
{
    public sealed class HttpServer : IDisposable
    {
        public HttpServer()
        {
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public void StartServer(int port)
        {
            // Create and bind our StreamSocket to a port and process
            // requests as they arrive
            StreamSocketListener listener = new StreamSocketListener();
            listener.BindServiceNameAsync(port.ToString());
            listener.ConnectionReceived += OnConnection;
        }

        //When a connection appears, this function his called
        private async void OnConnection(StreamSocketListener sender, StreamSocketListenerConnectionReceivedEventArgs args)
        {
            DataReader reader = new DataReader(args.Socket.InputStream);
            try
            {
                while (true)
                {
                    reader.InputStreamOptions = InputStreamOptions.Partial;
                    // Read first 4 bytes (length of the subsequent string).
                    uint sizeFieldCount = await reader.LoadAsync(sizeof(uint));
                    if (sizeFieldCount != sizeof(uint))
                    {
                        return;
                    }

                    // Read the string.
                    uint stringLength = reader.ReadUInt32();
                    uint actualStringLength = await reader.LoadAsync(stringLength);
                    if (stringLength != actualStringLength)
                    {
                        return;
                    }
                    String message = reader.ReadString(actualStringLength);
                }
            }
            catch (Exception exception)
            {
                // If this is an unknown status it means that the error is fatal and retry will likely fail.
                if (SocketError.GetStatus(exception.HResult) == SocketErrorStatus.Unknown)
                {
                    throw;
                }
            }
        }
    }

}
