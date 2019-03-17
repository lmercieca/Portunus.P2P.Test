using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace UdpHandler
{
    public class UDPSocket
    {
        private Socket _socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        private const int bufSize = 8 * 1024;
        private State state = new State();
        private EndPoint epFrom = new IPEndPoint(IPAddress.Any, 0);
        private AsyncCallback recv = null;

        public UDPSocket()
        {
            _socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.ReuseAddress, true);
            _socket.SetIPProtectionLevel(System.Net.Sockets.IPProtectionLevel.Unrestricted);
        }

        public class State
        {
            public byte[] buffer = new byte[bufSize];
        }

        public void Server(string address, int port)
        {
            IPEndPoint remoteEndpoint = new IPEndPoint(IPAddress.Parse(address), port);
            Console.WriteLine("Listening to " + remoteEndpoint.ToString());
            _socket.Bind(remoteEndpoint);
            Receive();
        }

        public void Client(string address, int port)
        {
            _socket.Connect(IPAddress.Parse(address), port);
            Receive();
        }

        public void Send(string text, string ip, int port)
        {
            byte[] data = Encoding.ASCII.GetBytes(text);
            EndPoint remoteEnd = new IPEndPoint(IPAddress.Parse(ip), port);

            _socket.BeginSendTo(data, 0, data.Length, SocketFlags.None,remoteEnd, (ar) =>
            {
                State so = (State)ar.AsyncState;
                int bytes = _socket.EndSend(ar);
                Console.WriteLine("SEND: {0}, {1}", bytes, text);
            }, state);
        }

        private void Receive()
        {
            _socket.BeginReceiveFrom(state.buffer, 0, bufSize, SocketFlags.None, ref epFrom, recv = (ar) =>
            {
               
                State so = (State)ar.AsyncState;

                IPPacketInformation packetInfo;
                EndPoint remoteEnd = new IPEndPoint(IPAddress.Any, 0);
                SocketFlags flags = SocketFlags.None;

                int received = _socket.EndReceiveMessageFrom(ar, ref flags, ref remoteEnd, out packetInfo);
                Console.WriteLine(
                    "{0} bytes received from {1} to {2}",
                    received,
                    remoteEnd,
                    packetInfo.Address
                );


                int bytes = _socket.EndReceiveFrom(ar, ref epFrom);
                
                _socket.BeginReceiveFrom(so.buffer, 0, bufSize, SocketFlags.None, ref epFrom, recv, so);
                Console.WriteLine("RECV: {0}: {1}, {2}", epFrom.ToString(), bytes, Encoding.ASCII.GetString(so.buffer, 0, bytes));
            }, state);
        }
    }
}
