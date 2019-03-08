using Sockets.Plugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace UdpHandler
{
    public class UDPWrapper
    {
        string DesintationIp;
        int Port;

        string SourceIp;
        int SourcePort;


        UdpSocketClient client = new UdpSocketClient();

        public UDPWrapper(string sourceIp, int sourceport, string desintationIp, int port)
        {
            this.SourceIp = sourceIp;
            this.SourcePort = sourceport;

            this.DesintationIp = desintationIp;
            this.Port = port;
        }

        public async void SendMessage(string message)
        {

            if (message.IndexOf("Server received") > -1)
                return;

            // convert our greeting message into a byte array
            var msgBytes = Encoding.UTF8.GetBytes(message);

            Console.WriteLine("sending: " + message);

            // send to address:port, 
            // no guarantee that anyone is there 
            // or that the message is delivered.
            await client.SendToAsync(msgBytes, this.DesintationIp, this.Port);
        }

        public async void ReceiveMessage()
        {

            var receiver = new UdpSocketReceiver();


            receiver.MessageReceived += async(sender, args) =>
            {
                // get the remote endpoint details and convert the received data into a string
                var from = String.Format("{0}:{1}", args.RemoteAddress, args.RemotePort);
                var data = Encoding.UTF8.GetString(args.ByteData, 0, args.ByteData.Length);

                if (data.IndexOf("Server received") > -1)
                {
                    Console.WriteLine("Received echo " + data);

                    return;
                }


                string returnMsg = String.Format("Server received {0} - {1}", from, data);
                Console.WriteLine(returnMsg);

              

                Console.WriteLine("sending: " + returnMsg + " to " + args.RemoteAddress + ":" + SourcePort);


                var msgBytes = Encoding.UTF8.GetBytes(returnMsg);
                await client.SendToAsync(msgBytes, args.RemoteAddress, SourcePort);
            };

            // listen for udp traffic on listenPort
            await receiver.StartListeningAsync(this.Port);
        }


    }
}
