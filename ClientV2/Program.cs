using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ClientV2
{
    class Program
    {
        const int PORT_NO = 5002;
        const string SERVER_IP = "69.6.36.79";

        const int CLIENT_PORT_NO = 5001;
        const string CLIENT_SERVER_IP = "23.97.216.14";

        static void Main(string[] args)
        {
            UdpHandler.UDPWrapper serverWrapper = new UdpHandler.UDPWrapper(SERVER_IP, PORT_NO);
            UdpHandler.UDPWrapper clientWrapper = new UdpHandler.UDPWrapper(CLIENT_SERVER_IP, CLIENT_PORT_NO);

            serverWrapper.SendMessage("Hello from client 2");

            System.Threading.Thread.Sleep(1000);

            clientWrapper.SendMessage("Hello from client 2, just for you.");
        }
    }
}
