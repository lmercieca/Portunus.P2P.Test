using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    class Program
    {
        const int Server_Port = 5001;
        const string SERVER_IP = "51.136.23.107";
        //const string SERVER_IP = "127.0.0.1";

        const int CLIENT_PORT_NO = 5002;
        const string CLIENT_SERVER_IP = "84.255.45.106";


        static void Main(string[] args)
        {
            UdpHandler.UDPWrapper serverWrapper = new UdpHandler.UDPWrapper(SERVER_IP, Server_Port);
            UdpHandler.UDPWrapper clientWrapper = new UdpHandler.UDPWrapper(CLIENT_SERVER_IP, CLIENT_PORT_NO);

            serverWrapper.ReceiveMessage();
            clientWrapper.ReceiveMessage();

            serverWrapper.SendMessage("Hello from client 1");

            System.Threading.Thread.Sleep(1000);

            clientWrapper.SendMessage("Hello from client 1, just for you.");

            while (true)
            {
            }
        }
    }
}
