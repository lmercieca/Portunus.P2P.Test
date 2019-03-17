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
        static ConfigManager config = new ConfigManager();


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

        static void StartTCPClient()
        {

            TCPSocket socket = new TCPSocket();

            new Thread(() =>
            {
                socket.StartListening(config.Client.Port);
            });

            socket.Send(config.Servers[0].Address, config.Servers[0].Port, "Hello Server 1, this is the client");
            socket.Send(config.Servers[1].Address, config.Servers[1].Port, "Hello Server 2, this is the client");

        }

        static async void StartUDPClient()
        {
            AsynchronousClient socket = new AsynchronousClient();

            new Thread(async () =>
            {
               socket.OpenNat(config.Servers[0].Address, config.Servers[0].Port);
            });

            new Thread(async () =>
            {
                socket.OpenNat(config.Servers[1].Address, config.Servers[0].Port);
            });

            new Thread(async () =>
            {
                await socket.StartListener(config.Client.Port, config.Servers[0].Address, config.Servers[0].Port);
                await socket.StartListener(config.Client.Port, config.Servers[1].Address, config.Servers[1].Port);
            });


            Thread.Sleep(5000);

            //new Thread(() =>
            //{
            socket.Send(config.Servers[0].Address, config.Servers[0].Port, "Hello server 1, this is the client");
            socket.Send(config.Servers[1].Address, config.Servers[1].Port, "Hello server 2, this is the client");

            Thread.Sleep(5000);



            while (true)
            {
                Console.WriteLine("Enter new port");
                int newPort = int.Parse(Console.ReadLine());


                new Thread(() =>
                {
                    socket.OpenNat(config.Servers[1].Address, newPort);
                });

                new Thread(() =>
                {
                    socket.OpenNat(config.Servers[1].Address, newPort);
                });

                socket.Send(config.Servers[0].Address, newPort, "Hello server 1, this is the client on new port" + newPort);
                socket.Send(config.Servers[1].Address, newPort, "Hello server 2, this is the client on new port " + newPort);

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
