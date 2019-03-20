using Sockets.Plugin;
using Sockets.Plugin.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace UdpHandler
{
    public class UDPSocket
    {
        UdpSocketReceiver receiver = new UdpSocketReceiver();
        UdpSocketClient client = new UdpSocketClient();
        UdpClient udpClient;
        int listenPort;

        public UDPSocket()
        {
            udpClient = new UdpClient(9001);
            udpClient.AllowNatTraversal(true);
            udpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            
        }

        public async Task StartListening(int listenPort)
        {
            /*
            Console.WriteLine("Starting listening to UDP on {0}", listenPort);
            receiver.MessageReceived += OnMessageReceived;
            
            // listen for udp traffic on listenPort
            await receiver.StartListeningAsync(listenPort);
            */
            this.listenPort = listenPort;
            
            udpClient.BeginReceive(new AsyncCallback(recv), null);

        }


        private void recv(IAsyncResult res)
        {

            udpClient.BeginReceive(new AsyncCallback(recv), null);

            IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, listenPort);
            byte[] received = udpClient.EndReceive(res, ref RemoteIpEndPoint);

            //Process codes

            

            Console.WriteLine("Received from UDP " + Encoding.UTF8.GetString(received));
        }

        public void OnMessageReceived(object sneder, UdpSocketMessageReceivedEventArgs args)
        {
            // get the remote endpoint details and convert the received data into a string
            var from = String.Format("{0}:{1}", args.RemoteAddress, args.RemotePort);
            var data = Encoding.UTF8.GetString(args.ByteData, 0, args.ByteData.Length);

            Console.WriteLine("Received UDP {0} - {1}", from, data);

        }
        public async Task SendTo(string address, int port, string msg)
        {
             var msgBytes = Encoding.UTF8.GetBytes(msg);

            Console.WriteLine("Sending UDP {0} - {1}: {2}", address,port, msg);



            // send to address:port, 
            // no guarantee that anyone is there 
            // or that the message is delivered.
            //await client.SendToAsync(msgBytes, address, port);
            //await client.SendToAsync(msgBytes, address, 9001);
            udpClient.Connect(address, port);
            
             udpClient.Send(msgBytes, msgBytes.Length,address,port);


        }


    }
}
