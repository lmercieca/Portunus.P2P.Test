using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ClientV2
{
    class Program
    {
        const int PORT_NO = 5002;
        const string SERVER_IP = "127.0.0.1";

        const int CLIENT_PORT_NO = 5001;
        const string CLIENT_SERVER_IP = "127.0.0.1";


        static void Main(string[] args)
        {
            //---data to send to the server---
            string textToSend = DateTime.Now.ToString();

            // Receive
            UdpClient listener = new UdpClient(PORT_NO);
            IPEndPoint groupEP = new IPEndPoint(IPAddress.Any, PORT_NO);
            string received_data;
            byte[] receive_byte_array;
            receive_byte_array = listener.Receive(ref groupEP);
            Console.WriteLine("Received a broadcast from {0}", groupEP.ToString());
            received_data = Encoding.ASCII.GetString(receive_byte_array, 0, receive_byte_array.Length);
            Console.WriteLine("data follows \n{0}\n\n", received_data);


            //Send
            Socket server = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            IPAddress send_to_server_address = IPAddress.Parse(CLIENT_SERVER_IP);
            IPEndPoint sending_server_end_point = new IPEndPoint(send_to_server_address, CLIENT_PORT_NO);

            byte[] send_server_buffer = Encoding.ASCII.GetBytes("Hello from client 2");
            server.SendTo(send_server_buffer, sending_server_end_point);


            /*        
            // Send to client
            Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            IPAddress send_to_address = IPAddress.Parse(CLIENT_SERVER_IP);
            IPEndPoint sending_end_point = new IPEndPoint(send_to_address, CLIENT_PORT_NO);

            byte[] send_buffer = Encoding.ASCII.GetBytes("Hello from client 2 Just To you");
            client.SendTo(send_buffer, sending_end_point);
            */

        }
    }
}
