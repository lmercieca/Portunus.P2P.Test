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

        static Comm.Tcp.Listener tcpListener = new Comm.Tcp.Listener();
        static Comm.Udp.Listener udpListener = new Comm.Udp.Listener();

        public static void Main(string[] args)
        {
            bool isTcp = args[0].ToLower() == "tcp";
            string ip = args[1];
            int port = int.Parse(args[2]);

            Process(isTcp, ip, port).Wait();
            
            while (true)
            {
                System.Threading.Thread.Sleep(500);

                string s = Console.ReadLine();
                tcpListener.Listen(int.Parse(s)).Wait();

            }

        }

        private static async Task<String> Process(bool isTcp, string ip, int port)
        {
            if (isTcp)
            {
                tcpListener.MessageReceived += Listener_MessageReceived;
               //  tcpListener.Listen(5001).Wait();
              await tcpListener.Connect("13.93.90.155", 5001);
                await tcpListener.SendMessage("Hello");


            }
            else
            {
                udpListener.MessageReceived += UdpListener_MessageReceived;
                udpListener.Listen(port);
            }

            return await Task.FromResult("");
        }

        private static void UdpListener_MessageReceived(string ip, int port, string message)
        {
            Console.WriteLine(message);

        }

        private static void Listener_MessageReceived(string ip, int port, string message)
        {
            Console.WriteLine(message);

        }
    }
}
