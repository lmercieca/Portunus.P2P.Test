using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portunus.Core.Client
{
    public class ProcessWorker
    {
        //   static Comm.Tcp.Client tcpClient = new Comm.Tcp.Client();
        static Comm.Udp.Client udpClient = new Comm.Udp.Client();
        static Comm.Udp.Listener udpListener = new Comm.Udp.Listener();

        public static void Main(string[] args)
        {
            bool isTcp = args[0].ToLower() == "tcp";
            string ip = args[1];
            int port = int.Parse(args[2]);

            Process(isTcp, ip, port);



            udpListener.MessageReceived += UdpListener_MessageReceived; ;
            udpListener.Listen(5002);


            while (true)
            {
                System.Threading.Thread.Sleep(500);
            }

        }

        private static void UdpListener_MessageReceived(string ip, int port, string message)
        {
            Console.WriteLine(message + " from " + ip + ":" + port);
        }

        private static void Process(bool isTcp, string ip, int port)
        {

            udpClient.MessageReceived += UdpClient_MessageReceived;
            udpClient.SendMessage(ip, port, "hello").Wait();
         
        }

        private static void UdpClient_MessageReceived(string message)
        {
            Console.WriteLine(message);

        }

    }
}
