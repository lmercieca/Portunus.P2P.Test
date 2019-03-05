using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class Program
    {
        const int Server_Port = 5002;
        const string SERVER_IP = "69.6.36.79";

        const int CLIENT_PORT_NO = 5001;
        const string CLIENT_SERVER_IP = "69.6.36.79";


        static void Main(string[] args)
        {
            UdpHandler.UDPWrapper secClientWrapper = new UdpHandler.UDPWrapper(SERVER_IP, Server_Port);
            UdpHandler.UDPWrapper clientWrapper = new UdpHandler.UDPWrapper(CLIENT_SERVER_IP, CLIENT_PORT_NO);

            while (true)
            {
                Console.WriteLine("Received " + secClientWrapper.ReceiveMessage() + " from " + SERVER_IP);
                Console.WriteLine("Received " + clientWrapper.ReceiveMessage() + " from " + CLIENT_SERVER_IP);
            }

        }
    }

}
