using System;
using System.Collections.Generic;

namespace MolyMade.FieldCommunication
{
    public class ModbusTcpMachine:Machine
    {
        public sealed override string Name { get; protected set; }
        public sealed override int Id { get; protected set; }
        public sealed override string Path { get; protected set; }
        public override bool IsConnected { get { return socketWrapper.IsConnected; } }
        public override DateTime LastConnected { get; protected set; }
        public override DateTime LastRead { get; protected set; }
        public override int Failures { get; protected set; }
        public sealed override MachineTypes Type { get; protected set; }
        public sealed override Dictionary<string, string> Tags { get; protected set; }
        public override Dictionary<string, string> Buffer { get; protected set; }
        public override MachineState State { get; protected set; }
        public override List<string> Logs { get; protected set; }
        private SocketWrapper socketWrapper;

        public ModbusTcpMachine(string name, int id, string path, MachineTypes type, Dictionary<string, string> tags) : base(name, id, path, type, tags)
        {
            Name = name;
            Id = id;
            Path = path;//"192.168.1.10:502
            Type = MachineTypes.Modbus;
            Tags = tags;
            string[] pathStrings = Path.Split(':');
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
            socketWrapper.Dispose();
        }
    }
}
