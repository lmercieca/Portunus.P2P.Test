using Sockets.Plugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace UdpHandler
{
    // State object for reading client data asynchronously  
    public class StateObject
    {
        // Client  socket.  
        public Socket workSocket = null;
        // Size of receive buffer.  
        public const int BufferSize = 1024;
        // Receive buffer.  
        public byte[] buffer = new byte[BufferSize];
        // Received data string.  
        public StringBuilder sb = new StringBuilder();
    }

    public class UDPWrapper
    {
        string sendsTo;
        int sendsOn;

        string receivesFrom;
        int receivesOn;

        string localIp;

        UdpSocketClient client = new UdpSocketClient();
        UdpSocketClient clientUnconnected = new UdpSocketClient();
        UdpSocketReceiver receiver = new UdpSocketReceiver();

        List<Socket> bindedSockets = new List<Socket>();


        List<UdpSocketClient> tempSockets = new List<UdpSocketClient>();
        List<int> foundPorts = new List<int>();

        //private Socket _socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

        // Create a TCP/IP socket.  
        Socket _socket; //= new Socket(ipAddress.AddressFamily,SocketType.Stream, ProtocolType.Tcp);


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
                catch { }
                finally
                {
                    tempSockets.Add(tClient);
                }
                port++;
                System.Threading.Thread.Sleep(100);
            }

            if (!found) return -1;
            return port;
        }

        public void BindAddress(int port)
        {
            Socket tmp = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            tmp.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.ReuseAddress, true);
            //tmp.Bind(new IPEndPoint(IPAddress.Parse(this.local), port));
            //bindedSockets.Add(tmp);

            // Get host name
            String strHostName = Dns.GetHostName();

            // Find host by name
            IPHostEntry iphostentry = Dns.GetHostByName(strHostName);

            // Enumerate IP addresses
            foreach (IPAddress ipaddress in iphostentry.AddressList)
            {
                Socket tmp2 = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                tmp2.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.ReuseAddress, true);
                tmp2.Bind(new IPEndPoint(ipaddress, port));
                
                SocketAsyncEventArgs e = new SocketAsyncEventArgs();
                byte[] buffer = new byte[1024];
                e.SetBuffer(buffer, 0, buffer.Length);

                e.Completed += (sender, e2) =>
                {     
                    Console.WriteLine(" Received" + e2.ReceiveMessageFromPacketInfo.Address + " from " + ipaddress.ToString());
                };

                tmp2.ReceiveAsync(e);

                bindedSockets.Add(tmp2);
            }

            bindedSockets.Add(tmp);
        }
        public static ManualResetEvent allDone = new ManualResetEvent(false);

        public UDPWrapper(string receivesFrom, int receivesOn, string sendsTo, int sendsOn)
        {
            this.receivesFrom = receivesFrom;
            this.receivesOn = receivesOn;

            this.sendsTo = sendsTo;
            this.sendsOn = sendsOn;

            Console.WriteLine("Binding to " + this.localIp + " on " + this.receivesOn);

            client.ConnectAsync(receivesFrom, receivesOn);

            //_socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.ReuseAddress, true);
            //_socket.Bind(new IPEndPoint(IPAddress.Any, 0));

            IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress ipAddress = ipHostInfo.AddressList[0];
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, receivesOn);

            // Create a TCP/IP socket.  
            _socket = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            _socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.ReuseAddress, true);
            _socket.ReceiveTimeout = int.MaxValue;
            _socket.SendTimeout = int.MaxValue;

            _socket.Bind(new IPEndPoint(IPAddress.Any, 0));
            //_socket.Bind(localEndPoint);
            _socket.Listen(100);
            
            allDone.Reset();
        }

        public UDPWrapper(string receivesFrom, int receivesOn, string sendsTo, int sendsOn, string localIp)
        {
            this.receivesFrom = receivesFrom;
            this.receivesOn = receivesOn;

            this.sendsTo = sendsTo;
            this.sendsOn = sendsOn;

            this.localIp = localIp;

            client.ConnectAsync(this.sendsTo, this.sendsOn);

            //IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress ipAddress = IPAddress.Parse(localIp);
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, receivesOn);


            _socket = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            _socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.ReuseAddress, true);
            _socket.ReceiveTimeout = int.MaxValue;
            _socket.SendTimeout = int.MaxValue;

            Console.WriteLine("Binding to " + this.localIp + " on " + this.receivesOn);
            _socket.Bind(new IPEndPoint(IPAddress.Any, 0));
           // _socket.Bind(localEndPoint);
            _socket.Listen(100);
            allDone.Reset();
        }

        public static void AcceptCallback(IAsyncResult ar)
        {
            // Signal the main thread to continue.  
            allDone.Set();

            // Get the socket that handles the client request.  
            Socket listener = (Socket)ar.AsyncState;
            Socket handler = listener.EndAccept(ar);

            // Create the state object.  
            StateObject state = new StateObject();
            state.workSocket = handler;
            handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                new AsyncCallback(ReadCallback), state);
        }

        public static void ReadCallback(IAsyncResult ar)
        {
            String content = String.Empty;

            // Retrieve the state object and the handler socket  
            // from the asynchronous state object.  
            StateObject state = (StateObject)ar.AsyncState;
            Socket handler = state.workSocket;

            // Read data from the client socket.   
            int bytesRead = handler.EndReceive(ar);

            if (bytesRead > 0)
            {
                // There  might be more data, so store the data received so far.  
                state.sb.Append(Encoding.ASCII.GetString(
                    state.buffer, 0, bytesRead));

                // Check for end-of-file tag. If it is not there, read   
                // more data.  
                content = state.sb.ToString();
                if (content.IndexOf("<EOF>") > -1)
                {
                    // All the data has been read from the   
                    // client. Display it on the console.  
                    Console.WriteLine("Read {0} bytes from socket. \n Data : {1}",
                        content.Length, content);
                    // Echo the data back to the client.  
                    Send(handler, content);
                }
                else
                {
                    // Not all data received. Get more.  
                    handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                    new AsyncCallback(ReadCallback), state);
                }
            }
        }

        private static void Send(Socket handler, String data)
        {
            // Convert the string data to byte data using ASCII encoding.  
            byte[] byteData = Encoding.ASCII.GetBytes(data);

            // Begin sending the data to the remote device.  
            handler.BeginSend(byteData, 0, byteData.Length, 0,
                new AsyncCallback(SendCallback), handler);
        }

        private static void SendCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.  
                Socket handler = (Socket)ar.AsyncState;

                // Complete sending the data to the remote device.  
                int bytesSent = handler.EndSend(ar);
                Console.WriteLine("Sent {0} bytes to client.", bytesSent);

                handler.Shutdown(SocketShutdown.Both);
                handler.Close();

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }


        public async void SendMessageTcp(string message)
        {

            if (message.IndexOf("Server received") > -1)
                return;

            // convert our greeting message into a byte array
            var msgBytes = Encoding.UTF8.GetBytes(message);

            Console.WriteLine("sending: " + message + " to " + this.sendsTo + " at " + this.sendsOn);

            // send to address:port, 
            // no guarantee that anyone is there 
            // or that the message is delivered.

            SocketAsyncEventArgs e = new SocketAsyncEventArgs();
            byte[] buffer = msgBytes;
            e.SetBuffer(buffer, 0, buffer.Length);
            e.RemoteEndPoint = new IPEndPoint(IPAddress.Parse(this.sendsTo), this.sendsOn);
            

            _socket.SendToAsync(e);
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


        public async void SendHandshake()
        {
            string message = "HELLO " + localIp + ":" + this.receivesOn;


            // convert our greeting message into a byte array
            var msgBytes = Encoding.UTF8.GetBytes(message);

            Console.WriteLine("sending: " + message + " to " + this.sendsTo + " at " + this.sendsOn);

            // send to address:port, 
            // no guarantee that anyone is there 
            // or that the message is delivered.
            await client.SendAsync(msgBytes);
        }


        public async Task ReceiveMessage()
        {
            try
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
                    var msgBytes = Encoding.UTF8.GetBytes(returnMsg);

                    await client.SendAsync(msgBytes);
                    Console.WriteLine("Sending: " + returnMsg);

                    EndPoint remoteEnd = new IPEndPoint(IPAddress.Parse(this.sendsTo), this.sendsOn);
                    _socket.SendTo(msgBytes, remoteEnd);
                    Console.WriteLine("Sent using clean socket: " + returnMsg);

                    await clientUnconnected.SendToAsync(msgBytes, this.sendsTo, int.Parse(args.RemotePort));

                    EndPoint remoteEndLocal = new IPEndPoint(IPAddress.Parse(this.sendsTo), int.Parse(args.RemotePort));
                    _socket.SendTo(msgBytes, remoteEndLocal);
                    Console.WriteLine("Sent using clean socket (local): " + returnMsg);

                    EndPoint remoteEndLocal2 = new IPEndPoint(IPAddress.Parse(args.RemoteAddress), int.Parse(args.RemotePort));
                    _socket.SendTo(msgBytes, remoteEndLocal2);
                    Console.WriteLine("Sent using clean socket (local): " + returnMsg);


                };

                Console.WriteLine("Listening on " + this.receivesOn);

                await receiver.StartListeningAsync(0);

                SocketAsyncEventArgs e = new SocketAsyncEventArgs();
                byte[] buffer = new byte[1024];
                e.SetBuffer(buffer, 0, buffer.Length);
                
                e.Completed += new EventHandler<SocketAsyncEventArgs>(IO_Completed);

                _socket.ReceiveAsync(e);

            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }


        void IO_Completed(object sender, SocketAsyncEventArgs e)
        {

            byte[] buffer = new byte[1024];
            e.SetBuffer(buffer, 0, buffer.Length);

            Console.WriteLine(" Received" + e.ReceiveMessageFromPacketInfo.Address);

        }


        private void E_Completed(object sender, SocketAsyncEventArgs e)
        {
            byte[] buffer = new byte[1024];
            e.SetBuffer(buffer, 0, buffer.Length);

            Console.WriteLine(" Received" + e.ReceiveMessageFromPacketInfo.Address);

        }
    }
}
