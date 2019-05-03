using Sockets.Plugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portunus.Comm.Udp
{
    public class Listener
    {
        public delegate void MessageReceivedHandler(string ip, int port, string message);

        // Define an Event based on the above Delegate
        public event MessageReceivedHandler MessageReceived = delegate { };

        public void InvokeMessageReceived(string ip, int port, string message)
        {
            MessageReceived.Invoke(ip, port, message);
        }

        public async void Listen(int listenPort)
        {
            var receiver = new UdpSocketReceiver();

            receiver.MessageReceived += (sender, args) =>
            {
                // get the remote endpoint details and convert the received data into a string
                var from = String.Format("{0}:{1}", args.RemoteAddress, args.RemotePort);
                var data = Encoding.UTF8.GetString(args.ByteData, 0, args.ByteData.Length);

                string messageReceived = string.Format("{0} - {1}", from, data);
                Console.WriteLine(messageReceived);

                InvokeMessageReceived(args.RemoteAddress, int.Parse(args.RemotePort), messageReceived);

                //receiver.SendToAsync(args.ByteData, args.RemoteAddress, 5001);
                //receiver.SendToAsync(args.ByteData, args.RemoteAddress, int.Parse(args.RemotePort));
                //receiver.SendToAsync(args.ByteData, args.RemoteAddress, 5002);
            };

            // listen for udp traffic on listenPort
            await receiver.StartListeningAsync(listenPort);
            
            Console.WriteLine("Listening on " + listenPort);
        }
    }
}

