using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace MolyMade.FieldCommunication
{
    public abstract class Machine:IDisposable
    {
        protected Machine(string name, int id, string path, MachineTypes type, Dictionary<string, string> tags)
        {
            this.Name = name;
            this.Id = id;
            this.LastConnected = -1;
            this.LastRead = -1;
            this.Path = path;
            this.Failures = 0;
            this.Type = type;
            this.Tags = tags;
            this.Buffer = new Dictionary<string, string>();
            this.Logs = new List<string>();
        }

        public abstract string Name { get; protected set; }
        public abstract int Id { get; protected set; }
        public abstract string Path { get; protected set; }
        public abstract bool IsConnected { get;}
        public abstract long LastConnected { get; protected set; }
        public abstract long LastRead { get; protected set; }
        public abstract int Failures { get; protected set; }
        public abstract MachineTypes Type { get; protected set; }
        public abstract Dictionary<string,string> Tags { get; protected set; }
        public abstract Dictionary<string,string> Buffer { get; protected set; }
        public abstract MachineState State { get; protected set; }
        public abstract List<string> Logs { get; protected set; }

        public static Machine CreateInstance(string name, int id, string path, MachineTypes type, Dictionary<string, string> tags)
        {
            switch (type)
            {
                case MachineTypes.Opc:
                    return new OpcMachine(name,id,path,type,tags);
                case MachineTypes.Modbus:
                    return null;
                case MachineTypes.Other:
                    return null;
                case MachineTypes.Other2:
                    return null;
                default:
                    return null;
            }
        }

        public virtual void Connect(){}

        public virtual void Disconnect(){}

        public virtual Dictionary<string, string> Read()
        {
            return new Dictionary<string, string>();
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
            string[] logArray = Logs.ToArray();
            Logs.Clear();
            return logArray;
        }

        public virtual void Dispose(){}
    }
}
