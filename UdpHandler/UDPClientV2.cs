using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace UdpHandler
{
    public class UDPClientV2
    {
     

        public string GetMessage(int port)
        {
            //Creates a UdpClient for reading incoming data.
            UdpClient receivingUdpClient = new UdpClient(port);
            while (true)
            {
                //Creates an IPEndPoint to record the IP Address and port number of the sender.
                // The IPEndPoint will allow you to read datagrams sent from any source.
                System.Net.IPEndPoint RemoteIpEndPoint = new System.Net.IPEndPoint(System.Net.IPAddress.Any,0);
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

                    //SendMessage("69.6.36.79", 8080, "I got " + messageOut);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
        }

        public void SendMessage(string ip, int port, string message)
        {
            UdpClient udpClient = new UdpClient(ip,port);
            Byte[] sendBytes = Encoding.ASCII.GetBytes(message);
            try
            {
                udpClient.Send(sendBytes, sendBytes.Length);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

        }
    }
}
