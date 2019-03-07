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
        const int Server_Port = 5001;
        const string SERVER_IP = "69.6.36.79";

        const int CLIENT_PORT_NO = 5002;
        const string CLIENT_SERVER_IP = "84.255.45.106";


        static void Main(string[] args)
        {
            UdpHandler.UDPWrapper secClientWrapper = new UdpHandler.UDPWrapper(SERVER_IP, Server_Port);
            UdpHandler.UDPWrapper clientWrapper = new UdpHandler.UDPWrapper(CLIENT_SERVER_IP, CLIENT_PORT_NO);

            secClientWrapper.ReceiveMessage();
            clientWrapper.ReceiveMessage();

            while (true)
            {
            }

        }
    }

}
