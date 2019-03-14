using Open.Nat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UdpHandler;

namespace Client
{
    class Program
    {
        /*
        const int Server_Port = 5001;
        //const string SERVER_IP = "13.93.90.155";
        //const string SERVER_IP = "127.0.0.1";
        const string SERVER_IP = "18.223.23.99";

        const int CLIENT_PORT_NO = 5001;
        const string CLIENT_SERVER_IP = "84.255.45.106";

        const int Source_PORT_NO = 5001;
        const string Source_SERVER_IP = "85.232.199.239";
        */

        static async void TcpThings()
        {
            int fromPort = ConfigManager.GetFromPort(ConfigManager.Mode.client_one);
            string fromIp = ConfigManager.GetFromIP(ConfigManager.Mode.client_one);
            int toPort = ConfigManager.GetToPort(ConfigManager.Mode.client_one); ;
            string toIp = ConfigManager.GetToIP(ConfigManager.Mode.client_one);


            TcpManager tcp = new TcpManager();

            tcp.Server(fromIp, fromPort, toIp, toPort);

            await tcp.SendData(toIp, toPort, "Hello from the client");


        }

        static void SetPortForward(int port)
        {
            try
            {
                var discoverer = new NatDiscoverer();
                var cts = new CancellationTokenSource(10000);
                var device = discoverer.DiscoverDeviceAsync(PortMapper.Upnp, cts).Result;

                device.CreatePortMapAsync(new Mapping(Protocol.Udp, 5001, port, "Portunus")).Wait();
            }
            catch
            {
                Console.Write(' ');
            }
        }

        public static async Task Start()
        {

            int fromPort = ConfigManager.GetFromPort(ConfigManager.Mode.client_one);
            string fromIp = ConfigManager.GetFromIP(ConfigManager.Mode.client_one);
            int toPort = ConfigManager.GetToPort(ConfigManager.Mode.client_one); ;
            string toIp = ConfigManager.GetToIP(ConfigManager.Mode.client_one);



            UdpHandler.UDPWrapper serverWrapper = new UdpHandler.UDPWrapper(fromIp, fromPort, toIp, toPort);
            serverWrapper.SendMessage("1", 5001);

            string port = Console.ReadLine();
            SetPortForward(int.Parse(port));

            await serverWrapper.ReceiveMessage(5001);
            await serverWrapper.ReceiveMessage(int.Parse(port));



            serverWrapper.SendMessage("2", 5001);
            serverWrapper.SendMessage("3", 5001);


        }
        static  void  Main(string[] args)
        {
            // TcpThings();
            // SetPortForward();

            Start();

        


            /*int res = serverWrapper.FindOpenPort(5001, 5999);*/

            /*            
            TestEcho te = new TestEcho();
            te.SendPing("13.93.90.155","Hello from the client.");
            */


            //UdpHandler.UDPWrapper clientWrapper = new UdpHandler.UDPWrapper(Source_SERVER_IP, Source_PORT_NO, CLIENT_SERVER_IP, CLIENT_PORT_NO);

            //serverWrapper.SendMessage("5001");

            //while (true)
            //{
            //    Console.WriteLine("Enter port");

            //    string result = Console.ReadLine();
            //    //serverWrapper.ReceiveMessage(int.Parse(result));

            //    TestEcho te = new TestEcho();
            //    te.GetMessage(int.Parse(result));

            //}

            //    clientWrapper.ReceiveMessage();



            // System.Threading.Thread.Sleep(1000);
            //     clientWrapper.SendMessage("Hello from client 1, just for you.");

            /*
   UDPClientV2 client = new UdpHandler.UDPClientV2();

   client.SendMessage(SERVER_IP, Server_Port, "Hello server");
   client.GetMessage(Source_PORT_NO);
   */
            while (true)
            {
                System.Threading.Thread.Sleep(500);

                

            }
        }
    }
}
