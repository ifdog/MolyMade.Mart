using System;
using System.Collections.Generic;
using System.Globalization;
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
            this.LastConnected = DateTime.MinValue;
            this.LastRead = DateTime.MinValue;
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
        public abstract bool IsConnected { get; protected set; }
        public abstract DateTime LastConnected { get; protected set; }
        public abstract DateTime LastRead { get; protected set; }
        public abstract int Failures { get; protected set; }
        public abstract MachineTypes Type { get; protected set; }
        public abstract Dictionary<string,string> Tags { get; protected set; }
        public abstract Dictionary<string,string> Buffer { get; protected set; }
        public abstract List<string> Logs { get; protected set; }
        public abstract string LastMessage { get; protected set; }
        public abstract int ReadErrors { get; protected set; }
        protected int Weighting = 0;


        public static Machine CreateInstance(string name, int id, string path, MachineTypes type, Dictionary<string, string> tags)
        {
            switch (type)
            {
                case MachineTypes.Testing:
                    return new TestMachine(name, id, path, type, tags);
                case MachineTypes.Siunmerik:
                    return new SinumerikMachine(name,id,path,type,tags);
                case MachineTypes.Moxa:
                    return new MoxaMachine(name,id,path,type,tags);
                default:
                    throw new NotImplementedException($"Type of machine {type.ToString()} not implemented");
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

        public virtual void Addtags()
        {
            Buffer["_TimeStamp"] = Utilities.GetUnixTimeStamp().ToString();
            Buffer["_Name"] = this.Name;
            Buffer["_Id"] = this.Id.ToString();
            Buffer["_Path"] = this.Path;
            Buffer["_LastConnected"] = this.LastConnected.ToString(CultureInfo.InvariantCulture);
            Buffer["_LastRead"] = this.LastRead.ToString(CultureInfo.InvariantCulture);
            Buffer["_Failures"] = this.Failures.ToString();
            Buffer["_LastMessage"] = this.LastMessage;
        }

        public virtual bool Check()
        {
            if (Failures > 0)
            {
                if (Weighting == 0)
                {
                    Weighting = Failures;
                    return true;
                }
                Weighting--;
                return false;
            }
            return true;
        }

        public virtual string[] ReadLog()
        {
            string[] logArray = Logs.ToArray();
            Logs.Clear();
            return logArray;
        }

        public virtual void Dispose()
        {
            this.Disconnect();
            this.Tags = null;
            this.Buffer = null;
            this.Logs = null;
        }
    }
}
