using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using UdpHandler;

namespace ClientV2
{
    class Program
    {
        /*
        //updated
        const int Server_Port = 5001;
        const string SERVER_IP = "23.97.244.188";
        //const string SERVER_IP = "127.0.0.1";

        const int CLIENT_PORT_NO = 5002;
        const string CLIENT_SERVER_IP = "69.6.36.79";


        const int Source_PORT_NO = 5001;
        const string Source_SERVER_IP = "84.255.45.106";
        */

        static void Main(string[] args)
        {
            int fromPort = ConfigManager.GetFromPort(ConfigManager.Mode.client_two);
            string fromIp = ConfigManager.GetFromIP(ConfigManager.Mode.client_two);
            int toPort = ConfigManager.GetToPort(ConfigManager.Mode.client_two); ;
            string toIp = ConfigManager.GetToIP(ConfigManager.Mode.client_two);


            UdpHandler.UDPWrapper serverWrapper = new UdpHandler.UDPWrapper(fromIp, fromPort, toIp, toPort);

            serverWrapper.ReceiveMessage(5001);
            serverWrapper.SendMessage("Hello from client 2");

            while (true)
            {
                System.Threading.Thread.Sleep(1000);

            }
        }
    }
}
