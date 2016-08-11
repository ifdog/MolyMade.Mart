using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MolyMade.FieldCommunication
{
    public class ModbusTcpMachine:Machine
    {
        public override string Name { get; protected set; }
        public override int Id { get; protected set; }
        public override string Path { get; protected set; }
        public override bool IsConnected { get { return this.socketWrapper.IsConnected; } }
        public override DateTime LastConnected { get; protected set; }
        public override DateTime LastRead { get; protected set; }
        public override int Failures { get; protected set; }
        public override MachineTypes Type { get; protected set; }
        public override Dictionary<string, string> Tags { get; protected set; }
        public override Dictionary<string, string> Buffer { get; protected set; }
        public override List<string> Logs { get; protected set; }
        public override string _lastMessage { get; protected set; }
        private SocketWrapper socketWrapper;

        public ModbusTcpMachine(string name, int id, string path, MachineTypes type, Dictionary<string, string> tags) : base(name, id, path, type, tags)
        {
            this.Name = name;
            this.Id = id;
            this.Path = path;//"192.168.1.10:502
            this.Type = MachineTypes.Modbus;
            this.Tags = tags;
            string[] pathStrings = this.Path.Split(':');
            socketWrapper = new SocketWrapper(pathStrings[0].Trim(),int.Parse(pathStrings[1].Trim()),1000);
        }

        public override void Connect()
        {
            if (!socketWrapper.IsConnected)
            {
                socketWrapper.Connect();
            }
        }

        public override void Disconnect()
        {
            if (socketWrapper.IsConnected)
            {
                socketWrapper.Disconnect();
            }
        }

        public override Dictionary<string, string> Read()
        {
            throw new NotImplementedException();
        }

        public override void Dispose()
        {
            this.socketWrapper.Dispose();
        }
    }
}
