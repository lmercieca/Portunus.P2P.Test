using Sockets.Plugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portunus.Comm.Tcp
{
    public class Client
    {

        TcpSocketClient client = new TcpSocketClient();


        public async Task SendMessage(string address, int port, string message)
        {
            Console.WriteLine("Sending " + message + " to " + address + ":" + port);

            await client.ConnectAsync(address, port);

            var messageBytes = Encoding.Default.GetBytes(message);
            client.WriteStream.Write(messageBytes, 0, messageBytes.Length);
            await client.WriteStream.FlushAsync();

            // wait a little before sending the next bit of data
            await Task.Delay(TimeSpan.FromMilliseconds(500));

           // await client.DisconnectAsync();
        }
    }
}
