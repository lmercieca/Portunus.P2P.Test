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
        static ConfigManager config = new ConfigManager();

        /*
        const int Server_Port = 5001;
        const string SERVER_IP = "69.6.36.79";

        const int CLIENT_PORT_NO = 5001;
        const string CLIENT_SERVER_IP = "84.255.45.106";

        const int Source_PORT_NO = 5001;
        const string Source_SERVER_IP = "18.223.23.99";
        */


        static void StartTCPClient()
        {

            TCPSocket socket = new TCPSocket();

            socket.StartListening(config.Servers[0].Port);
        }

        static async void StartUDPClient()
        {
            //AsynchronousClient socket = new AsynchronousClient();

            //await socket.StartListener(config.Servers[1].Port, config.Client.Address, config.Client.Port);
            //await socket.StartListener(config.Servers[1].Port, config.Host.Address, config.Host.Port);
        }




        static void Main(string[] args)
        {
            try
            {
                StartUDPClient();

                //StartTCPClient();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            while (true)
            {
                System.Threading.Thread.Sleep(500);
            }


        }


    }

}
