using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portunus.Core.Rendezvous
{
    public class ProcessWorker
    {
        static Comm.Udp.Client udpClient = new Comm.Udp.Client();

        static Comm.Tcp.Listener tcpListener = new Comm.Tcp.Listener();
        static Comm.Udp.Listener udpListener = new Comm.Udp.Listener();

        public static void Main(string[] args)
        {
            bool isTcp = args[0].ToLower() == "tcp";
            string ip = args[1];
            int port = int.Parse(args[2]);

            udpListener.MessageReceived += UdpListener_MessageReceived;
            udpListener.Listen(port);


            while (true)
            {
                System.Threading.Thread.Sleep(500);
            }
        }

        private static async void UdpListener_MessageReceived(string ip, int port, string message)
        {
            Console.WriteLine(message);
            await udpClient.SendMessage(ip, port, "Received (UDP): " + message);
        }

    }
}
