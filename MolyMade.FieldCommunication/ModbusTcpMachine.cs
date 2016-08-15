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
      

        public ModbusTcpMachine(string name, int id, string path, MachineTypes type, Dictionary<string, string> tags) : base(name, id, path, type, tags)
        {
            this.Name = name;
            this.Id = id;
            this.Path = path;//"192.168.1.10:502
            this.Type = MachineTypes.Modbus;
            this.Tags = tags;
            _tcpClient = new TcpClient();
          
        }

        public override void Connect()
        {
            if (!IsConnected)
            {
                _tcpClient.Connect(Path,502);
                _modbusIpMaster = ModbusIpMaster.CreateIp(_tcpClient);
            }
        }

        public override void Disconnect()
        {
            if (IsConnected)
            {
                _tcpClient.Close();
            }
        }

        public override Dictionary<string, string> Read()
        {
            //ushort[] imputs = _modbusIpMaster.ReadInputRegisters()
            return null;
        }

        public override void Dispose()
        {
            
        }
    }
}
