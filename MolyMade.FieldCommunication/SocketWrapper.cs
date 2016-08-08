using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Common.Logging;

namespace MolyMade.FieldCommunication
{
    class SocketWrapper:IDisposable
    {

        private  string IP ;
        private  int Port ;
        private  int TimeOut;

        public ILog Logger { get; set; }
        private Socket socket = null;
        public bool IsConnected { get { return socket.Connected; } }

        public SocketWrapper(string ip, int port, int timeout)
        {
            this.IP = ip;
            this.Port = port;
            this.TimeOut = timeout;
        }

        public void Connect()
        {
            this.socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            this.socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendTimeout, TimeOut);

            IPEndPoint ip = new IPEndPoint(IPAddress.Parse(IP), Port);
            this.socket.Connect(ip);
        }

        public void Disconnect()
        {
            this.socket.Disconnect(true);
        }

        public byte[] Read(int length)
        {
            byte[] data = new byte[length];
            this.socket.Receive(data);
            return data;
        }

        public void Write(byte[] data)
        {
            this.socket.Send(data);
        }


        public void Dispose()
        {
            if (this.socket != null)
            {
                this.socket.Close();
            }
        }
    }
}
