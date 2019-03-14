using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UdpHandler;

namespace Server
{
    class Program
    {

        /*
        const int Server_Port = 5001;
        const string SERVER_IP = "69.6.36.79";

        const int CLIENT_PORT_NO = 5001;
        const string CLIENT_SERVER_IP = "84.255.45.106";

        const int Source_PORT_NO = 5001;
        const string Source_SERVER_IP = "18.223.23.99";
        */


        static async void TcpThings()
        {
            int fromPort = ConfigManager.GetFromPort(ConfigManager.Mode.server);
            string fromIp = ConfigManager.GetFromIP(ConfigManager.Mode.server);
            int toPort = ConfigManager.GetToPort(ConfigManager.Mode.server); ;
            string toIp = ConfigManager.GetToIP(ConfigManager.Mode.server);

            TcpManager tcp = new TcpManager();

            tcp.Server(fromIp, fromPort, fromIp, toPort);

            await tcp.SendData(toIp, toPort, "Hello from the server");


        }




        static void Main(string[] args)
        {
            //TcpThings();

            int fromPort = ConfigManager.GetFromPort(ConfigManager.Mode.server);
            string fromIp = ConfigManager.GetFromIP(ConfigManager.Mode.server);
            int toPort = ConfigManager.GetToPort(ConfigManager.Mode.server); ;
            string toIp = ConfigManager.GetToIP(ConfigManager.Mode.server);

            
            UdpHandler.UDPWrapper serverWrapper = new UdpHandler.UDPWrapper(fromIp, fromPort, toIp, toPort);
            serverWrapper.ReceiveMessage(5001);

            while (true) { }
            //serverWrapper.SendMessage("Hello from your Server");

            /*
            TestEcho te = new TestEcho();
            te.SendPing("85.232.199.239","Hello from the server");
            */

       

            /*
                        UDPClientV2 client = new UdpHandler.UDPClientV2();

                        client.SendMessage(SERVER_IP, Server_Port, "Hello client");
                        client.GetMessage(Source_PORT_NO);

                */


        }

        public static void callSecondMethod()
        {

            UDPClientV2 clientV2 = new UDPClientV2();

            clientV2.GetMessage(5002);
        }
    }

}
