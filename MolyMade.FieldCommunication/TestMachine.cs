﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MolyMade.FieldCommunication
{
    public class TestMachine:Machine
    {
        private bool _isconnected = false;
        public override string Name { get; protected set; }
        public override int Id { get; protected set; }
        public override string Path { get; protected set; }
        public override bool IsConnected { get { return _isconnected; } }
        public override DateTime LastConnected { get; protected set; }
        public override DateTime LastRead { get; protected set; }
        public override int Failures { get; protected set; }
        public override MachineTypes Type { get; protected set; }
        public override Dictionary<string, string> Tags { get; protected set; }
        public override Dictionary<string, string> Buffer { get; protected set; }
        public override List<string> Logs { get; protected set; }
        public override string _lastMessage { get; protected set; }

        public TestMachine(
            string name, 
            int id, string path, 
            MachineTypes type, 
            Dictionary<string, string> tags) : base(name, id, path, type, tags)
        {
            Name = name;
            Id = id;
            Path = path;
            Type = type;
            Tags = tags;
        }

        public override void Connect()
        {
            if (_isconnected)
            {
                throw new Exception("Already connected");
            }
            _isconnected = true;
            LastConnected = DateTime.Now;
        }

        public override void Disconnect()
        {
            if (!_isconnected)
            {
                throw new Exception("Already disconnected");
            }
            _isconnected = false;
        }

        public override Dictionary<string, string> Read()
        {
            if (!_isconnected) { throw new Exception("Not connected");}
            Random r = new Random();
            Thread.Sleep(88);
            Buffer = Tags.Keys
                .Select(k => new KeyValuePair<string, string>(Tags[k], $"{r.NextDouble()}"))
                .ToDictionary(kv => kv.Key, kv => kv.Value);
            Addtags();
            LastRead = DateTime.Now;
            return Buffer;
        }

        public override string ToString()
        {
            return $"A Testing Server named {Name} with{Tags.Keys.Count} values";
        }
    }
}
