using Sockets.Plugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace UdpHandler
{
    public class UDPWrapper
    {
        string sendsTo;
        int sendsOn;

        string receivesFrom;
        int receivesOn;


        UdpSocketClient client = new UdpSocketClient();
        UdpSocketReceiver receiver = new UdpSocketReceiver();

        List<UdpSocketClient> tempSockets = new List<UdpSocketClient>();
        List<int> foundPorts = new List<int>();


        public int FindOpenPort(int start, int end)
        {
            int port = start;
            bool found = false;

       
            while (port <= end)
            {
                UdpSocketClient tClient = new UdpSocketClient();

                byte[] portData = Encoding.ASCII.GetBytes(port.ToString());

                client.SendToAsync(portData, this.sendsTo, this.sendsOn);
                Console.WriteLine("Sending " + port);

                try
                {
                    // convert our greeting message into a byte array
                    var msgBytes = Encoding.UTF8.GetBytes(port.ToString());

                    System.Net.IPEndPoint RemoteIpEndPoint = new System.Net.IPEndPoint(System.Net.IPAddress.Any, port);
                    UdpClient receivingUdpClient = new UdpClient(RemoteIpEndPoint);

                    try
                    {

                        // Blocks until a message returns on this socket from a remote host.
                        Byte[] receiveBytes = receivingUdpClient.Receive(ref RemoteIpEndPoint);

                        string returnData = Encoding.ASCII.GetString(receiveBytes);

                        string messageOut = String.Format("[{0},{1}]@[{2}]: {3}",
                            RemoteIpEndPoint.Address.ToString(),
                            RemoteIpEndPoint.Port.ToString(),
                            DateTime.Now,
                            returnData.ToString());

                        Console.WriteLine(messageOut);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                    }


                }
                catch  { }
                finally {
                    tempSockets.Add(tClient);
                }
                port++;
                System.Threading.Thread.Sleep(100);
            }

            if (!found) return -1;
            return port;
        }


        public UDPWrapper(string receivesFrom, int receivesOn, string sendsTo, int sendsOn)
        {
            this.receivesFrom = receivesFrom;
            this.receivesOn = receivesOn;

            this.sendsTo = sendsTo;
            this.sendsOn = sendsOn;
        }

        public async void SendMessage(string message, int port)
        {

            if (message.IndexOf("Server received") > -1)
                return;

            // convert our greeting message into a byte array
            var msgBytes = Encoding.UTF8.GetBytes(message);

            Console.WriteLine("sending: " + message + " to " + this.sendsTo + " at " + port);

            // send to address:port, 
            // no guarantee that anyone is there 
            // or that the message is delivered.
            await client.SendToAsync(msgBytes, this.sendsTo, port);
        }

        public async void SendMessage(string message)
        {

            if (message.IndexOf("Server received") > -1)
                return;

            // convert our greeting message into a byte array
            var msgBytes = Encoding.UTF8.GetBytes(message);

            Console.WriteLine("sending: " + message + " to " + this.sendsTo + " at " + this.sendsOn);

            // send to address:port, 
            // no guarantee that anyone is there 
            // or that the message is delivered.
            await client.SendToAsync(msgBytes, this.sendsTo, 5002);
        }

        public async Task ReceiveMessage(int port)
        {
            receiver.MessageReceived += async (sender, args) =>
            {
                // get the remote endpoint details and convert the received data into a string
                var from = String.Format("{0}:{1}", args.RemoteAddress, args.RemotePort);
                var data = Encoding.UTF8.GetString(args.ByteData, 0, args.ByteData.Length);

                Console.WriteLine("Received " + data);

                if (data.IndexOf("Server received") > -1)
                {
                    Console.WriteLine("Received echo " + data);

                    return;
                }



                string returnMsg = String.Format("Server received {0} - {1}", from, data);

                Console.WriteLine("Sending back " + returnMsg);

                var msgBytes = Encoding.UTF8.GetBytes(data);

                int retPort = int.Parse(data);

                //while (true)
                //{
                    await client.SendToAsync(msgBytes, this.sendsTo, this.sendsOn);
                   Console.WriteLine("Sending to " + this.sendsTo + " at " + this.sendsOn);
                    System.Threading.Thread.Sleep(1000);

                //    await client.SendToAsync(msgBytes, args.RemoteAddress, this.sendsOn);
                //    Console.WriteLine("Sending to " + args.RemoteAddress + " at " + this.sendsOn);
                //    System.Threading.Thread.Sleep(1000);


                //    await client.SendToAsync(msgBytes, args.RemoteAddress, retPort);
                //    Console.WriteLine("Sending to " + args.RemoteAddress + " at " + retPort);
                //    System.Threading.Thread.Sleep(1000);


                    await client.SendToAsync(msgBytes, args.RemoteAddress, int.Parse(args.RemotePort));
                    Console.WriteLine("Sending to " + args.RemoteAddress + " at " + int.Parse(args.RemotePort));
                    System.Threading.Thread.Sleep(1000);
                //}
            };

            // listen for udp traffic on listenPort

            Console.WriteLine("Listening on " + port);

            await receiver.StartListeningAsync(port);
          
          

        }
    }
}
