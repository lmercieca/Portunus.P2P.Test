using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace UdpHandler
{
    public class TestEcho
    {
        UdpClient socket;

        public void SendPing(int Port)
        {
            Socket socket = new Socket(AddressFamily.InterNetwork,
                    SocketType.Dgram, ProtocolType.Udp);
            socket.EnableBroadcast = false;
            socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            socket.Bind(new IPEndPoint(0,0));

            EndPoint ipEndPoint = new IPEndPoint(0,0);
            //byte[] data = Encoding.ASCII.GetBytes(message);
            //socket.SendTo(data, data.Length, SocketFlags.None, ipEndPoint);
            while (true)
            {
                byte[] data = new byte[1024];
                int received = socket.ReceiveFrom(data, ref ipEndPoint);

                Console.WriteLine("Received: " + Encoding.ASCII.GetString(data, 0, received));
            }
        }



        public void OnUdpData(IAsyncResult result)
        {
            // this is what had been passed into BeginReceive as the second parameter:
            socket = result.AsyncState as UdpClient;
            // points towards whoever had sent the message:
            IPEndPoint source = new IPEndPoint(0, 0);
            // get the actual message and fill out the source:
            byte[] message = socket.EndReceive(result, ref source);
            // do what you'd like with `message` here:
            Console.WriteLine("Got " + message.Length + " bytes from " + source);
            // schedule the next receive operation once reading is done:
            socket.BeginReceive(new AsyncCallback(OnUdpData), socket);
        }

        public void GetMessage(int port)
        {
            socket = new UdpClient(port); // `new UdpClient()` to auto-pick port
                                                    // schedule the first receive operation:
            socket.BeginReceive(new AsyncCallback(OnUdpData), socket);
            // sending data (for the sake of simplicity, back to ourselves):
            IPEndPoint target = new IPEndPoint(0, port);
        
        }


    }
}


