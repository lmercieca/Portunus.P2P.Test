using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;
using System.Threading.Tasks;

// State object for receiving data from remote device.  
public class StateObject
{
    // Client socket.  
    public Socket workSocket = null;
    // Size of receive buffer.  
    public const int BufferSize = 256;
    // Receive buffer.  
    public byte[] buffer = new byte[BufferSize];
    // Received data string.  
    public StringBuilder sb = new StringBuilder();
}

public class AsynchronousClient
{
    UdpClient listener = new UdpClient(9001);

        Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

    public void Send(string ip, int port, String data)
    {
        
        Console.WriteLine("Message sent to the address " + ip + " on " + port);
        s.Ttl = 255;

        s.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
        s.SetIPProtectionLevel(System.Net.Sockets.IPProtectionLevel.Unrestricted);

        IPAddress broadcast = IPAddress.Parse(ip);

        byte[] sendbuf = Encoding.ASCII.GetBytes(data);
        IPEndPoint ep = new IPEndPoint(broadcast, port);

        s.SendTo(sendbuf, ep);
        

        /*
        Console.WriteLine("Message sent to the address " + ip + " on " + port);

        byte[] sendbuf = Encoding.ASCII.GetBytes(data);
        listener.Send(sendbuf, sendbuf.Length, ip, port);
        */
    }

    public void OpenNat(string ip, int port)
    {
            while (true)
            {
                try
                {
                    s.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                    s.SetIPProtectionLevel(System.Net.Sockets.IPProtectionLevel.Unrestricted);

                    s.Ttl = 4;

                    IPAddress broadcast = IPAddress.Parse(ip);

                    byte[] sendbuf = Encoding.ASCII.GetBytes("SYN");
                    IPEndPoint ep = new IPEndPoint(broadcast, port);

                    s.SendTo(sendbuf, ep);

                    System.Threading.Thread.Sleep(500);

                    Console.WriteLine("Sending ping to " + ip + " on " + port);
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        
    }



    public async Task StartListener(int listenPort, string ip, int port)
    {
       listener.AllowNatTraversal(true);

        IPEndPoint groupEP = new IPEndPoint(IPAddress.Any, listenPort);

        try
        {
            while (true)
            {
                Console.WriteLine("Listening on " + listenPort);
               UdpReceiveResult res = await listener.ReceiveAsync();

                

                Console.WriteLine($"Received from {groupEP} :");
                Console.WriteLine($" {Encoding.ASCII.GetString(res.Buffer, 0, res.Buffer.Length)}");
                Console.WriteLine($"On {res.RemoteEndPoint}");
                Send(ip, port, "Got you boy");
                Send(res.RemoteEndPoint.Address.ToString(), res.RemoteEndPoint.Port, "Got you boy");

            }
        }
        catch (SocketException e)
        {
            Console.WriteLine(e);
        }
        finally
        {
            listener.Close();
        }
    }


}