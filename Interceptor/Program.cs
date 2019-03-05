using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Interceptor
{
    class Program
    {
        const int PORT_NO_CLIENT = 5001;
        const int PORT_NO_SERVER = 5002;
        const string SERVER_IP = "127.0.0.1";

        static void Main(string[] args)
        {
            //---listen at the specified IP and port no.---
            IPAddress localAdd = IPAddress.Parse(SERVER_IP);

            TcpListener listenerFromClient = new TcpListener(localAdd, PORT_NO_CLIENT);
            TcpListener listenerFromServer = new TcpListener(localAdd, PORT_NO_SERVER);

            Console.WriteLine("Interceptor Listening...");
            listenerFromClient.Start();
            listenerFromServer.Start();

            //---incoming client connected---
            TcpClient client = listenerFromClient.AcceptTcpClient();
            TcpClient server = listenerFromServer.AcceptTcpClient();


            //---get the incoming data through a network stream---
            NetworkStream nwStreamClient = client.GetStream();
            NetworkStream nwStreamServer = server.GetStream();


            byte[] bufferClient = new byte[client.ReceiveBufferSize];
            byte[] bufferServer = new byte[server.ReceiveBufferSize];

            //---read incoming stream---
            int bytesReadClient = nwStreamClient.Read(bufferClient, 0, client.ReceiveBufferSize);
            int bytesReadServer = nwStreamServer.Read(bufferServer, 0, server.ReceiveBufferSize);

            //---convert the data received into a string---
            string dataReceivedFromClient = Encoding.ASCII.GetString(bufferClient, 0, bytesReadClient) + " from client";
            string dataReceivedFromServer = Encoding.ASCII.GetString(bufferServer, 0, bytesReadServer) + " from server";

            Console.WriteLine("Received : " + dataReceivedFromClient);
            Console.WriteLine("Received : " + dataReceivedFromServer);

            //---write back the text to the client---
            Console.WriteLine("Sending back to server: " + dataReceivedFromClient);
            Console.WriteLine("Sending back to client: " + dataReceivedFromServer);

            nwStreamClient.Write(bufferServer, 0, bytesReadServer);
            nwStreamServer.Write(bufferClient, 0, bytesReadClient);

            client.Close();
            server.Close();

            listenerFromClient.Stop();
            listenerFromServer.Stop();

            Console.ReadLine();
        }
    }
}
