using Sockets.Plugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portunus.Comm.Udp
{
    public class Client
    {
        public delegate void MulticastMessageReceivedHandler(string message);
        public delegate void MessageReceivedHandler(string message);

        // Define an Event based on the above Delegate
        public event MulticastMessageReceivedHandler MulticastMessageReceived = delegate { };
        public event MessageReceivedHandler MessageReceived = delegate { };


        public void InvokeMulticastMessageReceived(string message)
        {
            MulticastMessageReceived.Invoke(message);
        }

        public void InvokeMessageReceived(string message)
        {
           MessageReceived.Invoke(message);
        }

        public async Task SendMessage(string address, int port, string message)
        {           
            var client = new UdpSocketClient();

            var msgBytes = Encoding.UTF8.GetBytes(message);

            await client.SendToAsync(msgBytes, address, port);

            client.MessageReceived += Client_MessageReceived;
        }

        private void Client_MessageReceived(object sender, Sockets.Plugin.Abstractions.UdpSocketMessageReceivedEventArgs e)
        {
            InvokeMessageReceived("Received " + Encoding.ASCII.GetString(e.ByteData));
            Console.WriteLine("Received " + Encoding.ASCII.GetString(e.ByteData));
        }

        public async void SendMulticastMessage(string address,int port, string message)
        {
            // typical instantiation
            var receiver = new UdpSocketMulticastClient();
            receiver.TTL = 5;

            receiver.MessageReceived += (sender, args) =>
            {
                var from = String.Format("{0}:{1}", args.RemoteAddress, args.RemotePort);
                var data = Encoding.UTF8.GetString(args.ByteData, 0, args.ByteData.Length);

                string messageReceived = string.Format("{0} - {1}", from, data);
                Console.WriteLine(messageReceived);

                InvokeMulticastMessageReceived(messageReceived);

                
            };

            // join the multicast address:port
            await receiver.JoinMulticastGroupAsync(address, port);

            var msgBytes = Encoding.UTF8.GetBytes(message);

            // send a message that will be received by all listening in
            // the same multicast group. 
            await receiver.SendMulticastAsync(msgBytes);
        }
    }
}
