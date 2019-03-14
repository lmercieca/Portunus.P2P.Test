using Sockets.Plugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UdpHandler
{
    public class TcpManager
    {
        public async Task Server(string listeningAddress, int listeningPort, string sendingAddress, int sendingPort)
        {

            var listener = new TcpSocketListener();
            Console.WriteLine("Listening on " + listeningAddress + " at " + listeningPort);

            // when we get connections, read byte-by-byte from the socket's read stream
            listener.ConnectionReceived += async (sender, args) =>
            {
                var client = args.SocketClient;

                var bytesRead = -1;
                var buf = new byte[1024];
                List<byte> data = new List<byte>();

                while (bytesRead != 0)
                {
                    bytesRead = await args.SocketClient.ReadStream.ReadAsync(buf, 0, 1024);
                    if (bytesRead > 0)
                        data.AddRange(buf.ToList());
                }

                string result = Encoding.ASCII.GetString(data.Where(x => x != 0).ToArray()).Trim();

                Console.WriteLine("Received " + result + " from " + args.SocketClient.RemoteAddress + " at " + args.SocketClient.RemotePort);
                
               Console.WriteLine("Sending " + result + " to " + args.SocketClient.RemoteAddress + " at " + sendingPort);
                await SendData(args.SocketClient.RemoteAddress, sendingPort, result);


                //Console.WriteLine("Sending " + result + " to " + args.SocketClient.RemoteAddress + " at " + args.SocketClient.RemotePort);
                //await SendData(args.SocketClient.RemoteAddress, args.SocketClient.RemotePort, result);


            };

            // bind to the listen port across all interfaces
            await listener.StartListeningAsync(listeningPort);
        }

        public async Task SendData(string address, int port, string message)
        {
            Console.WriteLine("Sending " + message + " to " + address + " at " + port);


            var client = new TcpSocketClient();
            await client.ConnectAsync(address, port);

            byte[] data = Encoding.ASCII.GetBytes(message);

            client.WriteStream.Write(data, 0, data.Length);

            
            await client.WriteStream.FlushAsync();

            // wait a little before sending the next bit of data
            await Task.Delay(TimeSpan.FromMilliseconds(500));


            await client.DisconnectAsync();
        }
    }
}
