using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MolyMade.FieldCommunication
{
    public class ModbusTcpMachine:Machine
    {
        public ModbusTcpMachine(string name, int id, string path, MachineTypes type, Dictionary<string, string> tags) : base(name, id, path, type, tags)
        {
            //TODO
        }

        public override string Name { get; protected set; }
        public override int Id { get; protected set; }
        public override string Path { get; protected set; }
        public override bool IsConnected { get; }
        public override long LastConnected { get; protected set; }
        public override long LastRead { get; protected set; }
        public override int Failures { get; protected set; }
        public override MachineTypes Type { get; protected set; }
        public override Dictionary<string, string> Tags { get; protected set; }
        public override Dictionary<string, string> Buffer { get; protected set; }
        public override MachineState State { get; protected set; }
        public override List<string> Logs { get; protected set; }
    }
}
