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
        protected Machine(string name, int id, string path, MachineTypes type, Dictionary<string, string> tags)
        {
            this.Name = name;
            this.Id = id;
            this.IsConnected = false;
            this.LastConnected = -1;
            this.LastRead = -1;
            this.Path = path;
            this.Failures = 0;
            this.Type = type;
            this.Tags = tags;
            this.Buffer = new Dictionary<string, string>();
        }

        public string Name { get; protected set; }
        public int Id { get; protected set; }
        public string Path { get; protected set; }
        public bool IsConnected { get; protected set; }
        public long LastConnected { get; protected set; }
        public long LastRead { get; protected set; }
        public int Failures { get; protected set; }
        public MachineTypes Type { get; protected set; }
        public Dictionary<string,string> Tags { get; protected set; }
        public Dictionary<string,string> Buffer { get; protected set; }
        public MachineState State { get; protected set; }
        public List<string> Logs { get; protected set; }

        public virtual void Connect()
        {
            throw new NotImplementedException();
        }

        public virtual void Disconnect()
        {
            throw new NotImplementedException();
        }

        public virtual void Read()
        {
            throw new NotImplementedException();
        }

        protected virtual void Log(string s)
        {
            if (Logs.Count > 100)
            {
                Logs.RemoveAt(0);
            }
            Logs.Add(s);
        }

        public virtual string[] ReadLog()
        {
            string[] _logArray = Logs.ToArray();
            Logs.Clear();
            return _logArray;
        }

        public virtual void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
