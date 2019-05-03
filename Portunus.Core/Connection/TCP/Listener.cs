using Sockets.Plugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portunus.Comm.Tcp
{
    public class Listener
    {

        TcpSocketClient client = new TcpSocketClient();


        public delegate void MessageReceivedHandler(string ip, int port, string message);

        // Define an Event based on the above Delegate
        public event MessageReceivedHandler MessageReceived = delegate { };

        public void InvokeMessageReceived(string ip, int port, string message)
        {
            MessageReceived.Invoke(ip, port, message);
        }


        public async Task Connect(string address, int port)
        {
            client = new TcpSocketClient();

            await client.ConnectAsync(address, port);
        }

        public async Task SendMessage(string message)
        {
            var messageBytes = Encoding.Default.GetBytes(message);
            client.WriteStream.Write(messageBytes, 0, messageBytes.Length);
            await client.WriteStream.FlushAsync();

            // wait a little before sending the next bit of data
            await Task.Delay(TimeSpan.FromMilliseconds(500));

            byte[] buffer = new byte[2048]; // read in chunks of 2KB

            while (true)
            {
                var s = client.ReadStream.Read(buffer, 0, buffer.Length);
                string result = Encoding.Default.GetString(buffer).Trim().Replace("\0", "");

                await Task.Delay(TimeSpan.FromMilliseconds(500));

            }
        }

        public async Task Disconnect()
        {
            await client.DisconnectAsync();
        }

        public async Task Listen(int listenPort)
        {
            StringBuilder sbResult = new StringBuilder();

            var listener = new TcpSocketListener();

            

            // when we get connections, read byte-by-byte from the socket's read stream
            listener.ConnectionReceived += async (sender, args) =>
            {
                var client = args.SocketClient;

                Console.WriteLine("Connection Received");
                Console.WriteLine("Connection Received Details: " + client.RemoteAddress + ":" + client.RemotePort);

                byte[] buffer = new byte[2048]; // read in chunks of 2KB

                int bytesRead = args.SocketClient.ReadStream.Read(buffer, 0, buffer.Length);


                while (bytesRead > 0)
                {

                    string result = Encoding.Default.GetString(buffer).Trim().Replace("\0", "");
                    sbResult.Append(result);
                    Console.WriteLine("Received (Byte): " + result + " - bytesRead " + bytesRead);

                    buffer = new byte[2048];
                    InvokeMessageReceived(client.RemoteAddress, client.RemotePort, sbResult.ToString());


                    bytesRead = args.SocketClient.ReadStream.Read(buffer, 0, buffer.Length);
                    sbResult = new StringBuilder();

                    await SendMessage(sbResult.ToString());

                   
                }
            };

            // bind to the listen port across all interfaces
            await listener.StartListeningAsync(listenPort);

            Console.WriteLine("Listening on " + listenPort);
        }
    }
}
