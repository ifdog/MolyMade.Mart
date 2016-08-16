using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Modbus;
using Modbus.Device;

namespace MolyMade.FieldCommunication
{
    public class ModbusTcpMachine:Machine
    {
        public override string Name { get; protected set; }
        public override int Id { get; protected set; }
        public override string Path { get; protected set; }
        public override bool IsConnected {
            get { return _tcpClient.Connected; } 
        }
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
      

        public ModbusTcpMachine(string name, int id, string path, MachineTypes type, Dictionary<string, string> tags) : base(name, id, path, type, tags)
        {
            this.Name = name;
            this.Id = id;
            this.Path = path;//"192.168.1.10
            this.Type = MachineTypes.Modbus;
            this.Tags = tags;
            _tcpClient = new TcpClient();
            _tcpClient.ReceiveTimeout = 1000;
            _tcpClient.SendTimeout = 1000;

        }

        public override void Connect()
        {
            if (!IsConnected)
            {
                try
                {
                    _tcpClient.Connect(Path, 502);
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

        public override void Disconnect()
        {
            if (IsConnected)
            {
                _modbusIpMaster.Dispose();
                _tcpClient.Close();
            }
        }

        public override Dictionary<string, string> Read()
        {
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
                Utilities.Log(this, e.Message);
                throw;
            }
            if (dis?.Length == 1)
            {
                _innerBuffer["DI0"] = (1 & dis[0]).ToString();
                _innerBuffer["DI1"] = (1 & (dis[0]>>1)).ToString();
                _innerBuffer["DI2"] = (1 & (dis[0]>>2)).ToString();
                _innerBuffer["DI3"] = (1 & (dis[0]>>3)).ToString();
                _innerBuffer["DIO0"] = (1 & (dis[0]>>4)).ToString();
                _innerBuffer["DIO1"] = (1 & (dis[0]>>5)).ToString();
                _innerBuffer["DIO2"] = (1 & (dis[0]>>6)).ToString();
                _innerBuffer["DIO3"] = (1 & (dis[0]>>7)).ToString();
            }
            if (ais?.Length==8)
            {
                foreach (ushort v in ais)
                {
                    bytes.Add(Convert.ToByte(v&byte.MaxValue));
                    bytes.Add(Convert.ToByte((v>>8)&byte.MaxValue));
                }
                var x = bytes.ToArray();
                
                _innerBuffer["AI0"] = BitConverter.ToSingle(x, 0).ToString();
                _innerBuffer["AI1"] = BitConverter.ToSingle(x, 4).ToString();
                _innerBuffer["AI2"] = BitConverter.ToSingle(x, 8).ToString();
                _innerBuffer["AI3"] = BitConverter.ToSingle(x, 12).ToString();
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
