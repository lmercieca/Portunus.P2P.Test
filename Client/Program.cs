//using Open.Nat;
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


        //static void SetPortForward(int port)
        //{
        //    try
        //    {
        //        var discoverer = new NatDiscoverer();
        //        var cts = new CancellationTokenSource(10000);
        //        var device = discoverer.DiscoverDeviceAsync(PortMapper.Upnp, cts).Result;


        //        device.CreatePortMapAsync(new Mapping(Protocol.Udp, 5001, port, "Portunus")).Wait();
        //    }
        //    catch
        //    {
        //        Console.Write(' ');
        //    }
        //}

        static async void StartTCPClient()
        {

            TCPSocket socket = new TCPSocket();


            new Thread(() =>
            {
                socket.StartListening(config.Client.Port);
            });
             
            socket.Send(config.Servers[0].Address, config.Servers[0].Port, "Hello Server 1, this is the host");


            while (true)
            {
                try
                {
                    socket.Send(config.Client.Address, config.Client.Port, "Hello host, this is the client");
                    Thread.Sleep(500);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        static async void StartServer()
        {
            AsynchronousSocketListener socket = new AsynchronousSocketListener();
            socket.StartListening(config.Client.Address, config.Client.Port);

        }

        static async void StartUDPClient()
        {
            AsynchronousClient clientSocket = new AsynchronousClient();
            clientSocket.StartClient(config.Host.Port, config.Servers[1].Address, config.Servers[1].Port);

            //UDPSocket sock = new UDPSocket();


            //await sock.StartListening(config.Client.Port);
            //await sock.SendTo(config.Servers[1].Address, config.Servers[1].Port, "Hello Server 1, this is the client");


            //new Thread(async () =>
            //{
            //   socket.OpenNat(config.Servers[0].Address, config.Servers[0].Port);
            //});

            //new Thread(async () =>
            //{
            //    socket.OpenNat(config.Servers[1].Address, config.Servers[0].Port);
            //});

            //new Thread(async () =>
            //{
            //    await socket.StartListener(config.Client.Port, config.Servers[0].Address, config.Servers[0].Port);
            //    await socket.StartListener(config.Client.Port, config.Servers[1].Address, config.Servers[1].Port);
            //});


            //Thread.Sleep(5000);

            ////new Thread(() =>
            ////{
            //socket.Send(config.Servers[0].Address, config.Servers[0].Port, "Hello server 1, this is the client");
            //socket.Send(config.Servers[1].Address, config.Servers[1].Port, "Hello server 2, this is the client");

            //Thread.Sleep(5000);



            while (true)
            {



                //new Thread(() =>
                //{
                //    socket.OpenNat(config.Servers[1].Address, newPort);
                //});

                //new Thread(() =>
                //{
                //    socket.OpenNat(config.Servers[1].Address, newPort);
                //});

                //socket.Send(config.Servers[0].Address, newPort, "Hello server 1, this is the client on new port" + newPort);
                //socket.Send(config.Servers[1].Address, newPort, "Hello server 2, this is the client on new port " + newPort);

                Thread.Sleep(5000);
                //new Thread(async () =>
                //{
                //    await socket.StartListener(newPort, config.Servers[0].Address, config.Servers[0].Port);
                //    await socket.StartListener(newPort, config.Servers[1].Address, config.Servers[1].Port);
                //});



            }

        }

        static void Main(string[] args)
        {

            Thread threadBG = new Thread(StartServer);
            threadBG.IsBackground = true;
            threadBG.Start();


            //StartTCPClient();
            StartUDPClient();

            while (true)
            {
                System.Threading.Thread.Sleep(500);
            }
        }
    }
}
