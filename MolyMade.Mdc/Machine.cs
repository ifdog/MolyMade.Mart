using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MolyMade.Mdc
{
    public abstract class Machine:IDisposable
    {
        protected Machine(string name, int id, string path, int failures, MachineTypes type, Dictionary<string, string> tags)
        {
            this.Name = name;
            this.Id = id;
            this.IsConnected = false;
            this.LastConnected = -1;
            this.LastRead = -1;
            this.Path = path;
            this.Failures = failures;
            this.Type = type;
            this.Tags = tags;
            this.Buffer = new Dictionary<string, string>();
        }

        public string Name { get; }
        public int Id { get; }
        public string Path { get; }
        public bool IsConnected { get; }
        public long LastConnected { get; }
        public long LastRead { get; }
        public int Failures { get; }
        public MachineTypes Type { get; }
        public Dictionary<string,string> Tags { get; }
        public Dictionary<string,string> Buffer { get; }

        public virtual bool Connect()
        {
            throw new NotImplementedException();
        }

        public virtual bool Disconnect()
        {
            throw new NotImplementedException();
        }

        public virtual Dictionary<string, string> Read()
        {
            throw new NotImplementedException();
        }

        public virtual void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
