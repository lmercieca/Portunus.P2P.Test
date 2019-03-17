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
        static ConfigManager config = new ConfigManager();


        static void StartTCPClient()
        {

            TCPSocket socket = new TCPSocket();

            new Thread(() =>
            {
                socket.StartListening(config.Host.Port);
            });

            socket.Send(config.Servers[0].Address, config.Servers[0].Port, "Hello Server 1, this is the client");
            socket.Send(config.Servers[1].Address, config.Servers[1].Port, "Hello Server 2, this is the client");

        }

        static async void StartUDPClient()
        {
            AsynchronousClient socket = new AsynchronousClient();


            //new Thread(() =>
            //{
            socket.Send(config.Servers[0].Address, config.Servers[0].Port, "Hello server 1, this is the client");
            socket.Send(config.Servers[1].Address, config.Servers[1].Port, "Hello server 2, this is the client");

            new Thread(async () =>
            {
                Thread.Sleep(5000);
                await socket.StartListener(config.Host.Port, config.Servers[0].Address, config.Servers[0].Port);
                await socket.StartListener(config.Host.Port, config.Servers[1].Address, config.Servers[1].Port);
            });


            //});

            while (true)
            {
                Console.WriteLine("Enter new port");
                int newPort = int.Parse(Console.ReadLine());

                socket.Send(config.Servers[0].Address, newPort, "Hello server 1, this is the client");
                socket.Send(config.Servers[1].Address, newPort, "Hello server 2, this is the client");

                Thread.Sleep(5000);
                new Thread(async () =>
                {
                    await socket.StartListener(newPort, config.Servers[0].Address, config.Servers[0].Port);
                    await socket.StartListener(newPort, config.Servers[1].Address, config.Servers[1].Port);
                });


            }

        }

        static void Main(string[] args)
        {
            //StartTCPClient();
            StartUDPClient();

            while (true)
            {
                System.Threading.Thread.Sleep(500);
            }
        }
    }
}
