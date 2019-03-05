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
        IPEndPoint endPoint;

        public UDPWrapper(string desintationIp, int port)
        {
            IPAddress destIp = IPAddress.Parse(desintationIp);

            endPoint = new IPEndPoint(destIp, port);
        }

        public string SendMessage(string message)
        {
            string serverResponse = string.Empty;       // The variable which we will use to store the server response

            using (UdpClient client = new UdpClient())
            {
                byte[] data = Encoding.UTF8.GetBytes(message);      // Convert our message to a byte array
                client.Send(data, data.Length, endPoint);      // Send the date to the server

                serverResponse = Encoding.UTF8.GetString(client.Receive(ref endPoint));    // Retrieve the response from server as byte array and convert it to string
            }

            return serverResponse;
        }

        public string ReceiveMessage()
        {
            string serverResponse = string.Empty;       // The variable which we will use to store the server response

            using (UdpClient client = new UdpClient())
            {
                serverResponse = Encoding.UTF8.GetString(client.Receive(ref endPoint));    // Retrieve the response from server as byte array and convert it to string

                byte[] data = Encoding.UTF8.GetBytes("Received " + serverResponse);      // Convert our message to a byte array
                client.Send(data, data.Length, endPoint);      // Send the date to the server

            }

            return serverResponse;
        }


    }
}
