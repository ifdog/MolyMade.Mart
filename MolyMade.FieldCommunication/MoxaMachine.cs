using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Modbus;
using Modbus.Device;

namespace MolyMade.FieldCommunication
{
    public class MoxaMachine:Machine
    {
        public override string Name { get; protected set; }
        public override int Id { get; protected set; }
        public override string Path { get; protected set; }
        public override bool IsConnected { get; protected set; }
        public override DateTime LastConnected { get; protected set; }
        public override DateTime LastRead { get; protected set; }
        public override int Failures { get; protected set; }
        public override MachineTypes Type { get; protected set; }
        public override Dictionary<string, string> Tags { get; protected set; }
        public override Dictionary<string, string> Buffer { get; protected set; }
        public override List<string> Logs { get; protected set; }
        public override string _lastMessage { get; protected set; }
        private TcpClient _tcpClient;
        private ModbusIpMaster _modbusIpMaster;
        private Dictionary<string, string> _innerBuffer =new Dictionary<string, string>(); 
        private ManualResetEvent _manualResetEvent = new ManualResetEvent(false);
        private int _tcpclienttimeout = 1000;
        private string[] _td = {"DI0","DI1","DI2","DI3","DIO0","DIO1","DIO2","DIO3"};
        private string[] _ta = {"AI0","AI1","AI2","AI3"};
      

        public MoxaMachine(string name, int id, string path, MachineTypes type, Dictionary<string, string> tags) : base(name, id, path, type, tags)
        {
            this.Name = name;
            this.Id = id;
            this.Path = path;//"192.168.1.10
            this.Type = MachineTypes.Moxa;
            this.Tags = tags;


        }

        public override void Connect()
        {
            if (!IsConnected)
            {
                try
                {
                    _tcpClient = new TcpClient();
                    _tcpClient.ReceiveTimeout = 500;
                    _tcpClient.SendTimeout = 500;
                    ConnectWithTimeout(_tcpclienttimeout);
                    _modbusIpMaster = ModbusIpMaster.CreateIp(_tcpClient);
                    LastConnected = DateTime.Now;
                    Failures = 0;
                }
                catch (Exception)
                {
                    Failures = Failures > 100 ? 99 : Failures + 1;
                    throw;
                }
            }
        }

        public void ConnectWithTimeout(int timeout)
        {
            _manualResetEvent.Reset();
            _tcpClient.BeginConnect(Path, 502, ConnectCallback, _tcpClient);
            if (_manualResetEvent.WaitOne(3000, false))
            {
                IsConnected = true;
            }
            else
            {
                _tcpClient.Close();
                IsConnected = false;
                throw new TimeoutException();
            }
        }

        public void ConnectCallback(IAsyncResult iAsyncResult)
        {
            try
            {
                TcpClient tcpClient = iAsyncResult as TcpClient;
                tcpClient?.EndConnect(iAsyncResult);
            }
            finally
            {
                _manualResetEvent.Set();
            }
        }

        public override void Disconnect()
        {
            if (IsConnected)
            {
                _modbusIpMaster.Dispose();
                _tcpClient.Close();
                IsConnected = false;
            }
        }

        public override Dictionary<string, string> Read()
        {
            if (!IsConnected)
            {
                throw new Exception("disconnected!");
            }
            ushort[] dis;
            ushort[] ais;
            List<byte> bytes = new List<byte>();
            try
            {
                dis = _modbusIpMaster.ReadInputRegisters(48, 1);
                ais = _modbusIpMaster.ReadInputRegisters(520, 8);
            }
            catch (Exception e)
            {
                IsConnected = false;
                _tcpClient.Client.Close();
                Utilities.Log(this, e.Message);
                throw;
            }
            if (dis?.Length == 1)
            {
                for (int i = 0; i < 8; i++)
                {
                    _innerBuffer[_td[i]]= (1 & (dis[0] >> i)).ToString();
                }
            }
            if (ais?.Length==8)
            {
                foreach (ushort v in ais)
                {
                    bytes.Add(Convert.ToByte(v&byte.MaxValue));
                    bytes.Add(Convert.ToByte((v>>8)&byte.MaxValue));
                }
                var x = bytes.ToArray();
                for (int i = 0; i < 4; i++)
                {
                    _innerBuffer[_ta[i]] = BitConverter.ToSingle(x, 4*i).ToString();
                }
            }
            foreach (var key in Tags.Keys)
            {
                if (_innerBuffer.ContainsKey(key))
                {
                    Buffer[Tags[key]] = _innerBuffer[key];
                }
            }
            Addtags();
            LastRead = DateTime.Now;
            return Buffer;
        }

        public override void Dispose()
        {
            _modbusIpMaster.Dispose();
            _tcpClient.Close();
            _tcpClient.Dispose();
        }
    }
}
