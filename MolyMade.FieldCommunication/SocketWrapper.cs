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

        private readonly string _ip ;
        private readonly int _port ;
        private readonly int _timeOut;

        private Socket _socket;
        public bool IsConnected => _socket.Connected;

        public SocketWrapper(string ip, int port, int timeout)
        {
            this._ip = ip;
            this._port = port;
            this._timeOut = timeout;
        }

        public void Connect()
        {
            this._socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            this._socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendTimeout, _timeOut);
            IPEndPoint ip = new IPEndPoint(IPAddress.Parse(_ip), _port);
            this._socket.Connect(ip);
        }

        public void Disconnect()
        {
            this._socket.Disconnect(true);
        }

        public byte[] Read(int length)
        {
            byte[] data = new byte[length];
            this._socket.Receive(data);
            return data;
        }

        public void Write(byte[] data)
        {
            this._socket.Send(data);
        }


        public void Dispose()
        {
            this._socket?.Close();
        }
    }
}
