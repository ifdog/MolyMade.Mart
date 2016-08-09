using System;
using System.Net;
using System.Net.Sockets;

namespace MolyMade.FieldCommunication
{
    class SocketWrapper:IDisposable
    {

        private readonly string _ip ;
        private readonly int _port ;
        private readonly int _timeOut;

        private Socket _socket;
        public bool IsConnected => _socket.Connected;

        public SocketWrapper(string ip, int port, int timeout)
        {
            _ip = ip;
            _port = port;
            _timeOut = timeout;
        }

        public void Connect()
        {
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendTimeout, _timeOut);
            IPEndPoint ip = new IPEndPoint(IPAddress.Parse(_ip), _port);
            _socket.Connect(ip);
        }

        public void Disconnect()
        {
            _socket.Disconnect(true);
        }

        public byte[] Read(int length)
        {
            byte[] data = new byte[length];
            _socket.Receive(data);
            return data;
        }

        public void Write(byte[] data)
        {
            _socket.Send(data);
        }


        public void Dispose()
        {
            _socket?.Close();
        }
    }
}
