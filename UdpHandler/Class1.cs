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

        public UDPWrapper(string desintationIp, int port)
        {
            this.DesintationIp = desintationIp;
            this.Port = port;
        }

        public async void SendMessage(string message)
        {
           
            var client = new UdpSocketClient();

            // convert our greeting message into a byte array
            var msgBytes = Encoding.UTF8.GetBytes(message);

            // send to address:port, 
            // no guarantee that anyone is there 
            // or that the message is delivered.
            await client.SendToAsync(msgBytes, this.DesintationIp, this.Port);
        }

        public async void  ReceiveMessage()
        {
            
            var receiver = new UdpSocketReceiver();

            receiver.MessageReceived += (sender, args) =>
            {
                // get the remote endpoint details and convert the received data into a string
                var from = String.Format("{0}:{1}", args.RemoteAddress, args.RemotePort);
                var data = Encoding.UTF8.GetString(args.ByteData, 0, args.ByteData.Length);

                string returnMsg =String.Format("{0} - {1}", from, data);
                Console.WriteLine(returnMsg);
                //SendMessage(returnMsg);
            };

            // listen for udp traffic on listenPort
            await receiver.StartListeningAsync(this.Port);
        }


    }
}
