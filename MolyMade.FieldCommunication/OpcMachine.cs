﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TitaniumAS.Opc.Client.Da;
using  TitaniumAS.Opc.Client.Common;

namespace MolyMade.FieldCommunication
{
    class OpcMachine:Machine
    {
        private readonly Uri _url;
        private readonly OpcDaServer _server;
        private OpcDaGroup _group;
        private readonly OpcDaItemDefinition[] _itemDefinitions;
        private OpcDaItemValue[] _itemValues;
        public override string Name { get; protected set; }
        public override int Id { get; protected set; }
        public override string Path { get; protected set; }
        public override bool IsConnected => _server.IsConnected;
        public override DateTime LastConnected { get; protected set; }
        public override DateTime LastRead { get; protected set; }
        public override int Failures { get; protected set; }
        public override MachineTypes Type { get; protected set; }
        public override Dictionary<string, string> Tags { get; protected set; }
        public override Dictionary<string, string> Buffer { get; protected set; }
        public override List<string> Logs { get; protected set; }

        public OpcMachine(string name, int id, string path, MachineTypes type,
            Dictionary<string, string> tags) : base(name, id, path, type, tags)
        {
            //path:"PCU50|OPC.SINUMERIK.Machineswitch"
            string[] urlStrings = path.Split('@');
            if (urlStrings.Length != 2)
            {
                throw new ArgumentException($"Invaild path string :{path}");
            }
            this._url = UrlBuilder.Build(urlStrings[0], urlStrings[1]);
            _server = new OpcDaServer(this._url);
            _itemDefinitions = tags.Keys.Select(i => new OpcDaItemDefinition()
            {
                ItemId = i.Trim(),
                IsActive = true
            }).ToArray();
        }

        public override void Connect()
        {
            try
            {
                _server.Connect();
                _group = _server.AddGroup("MolyMadeGroup");
                _group.IsActive = true;
                _group.AddItems(_itemDefinitions);
                this.LastConnected = DateTime.Now;
                Failures = 0;
            }
            catch (Exception)
            {
                Failures++;
                throw;
            }
        }

        public override Dictionary<string,string> Read()
        {
            try
            {
                _itemValues = _group.Read(_group.Items, OpcDaDataSource.Device);
                foreach (OpcDaItemValue value in _itemValues)
                {
                    Buffer[Tags[value.Item.ItemId]] = value.Value.ToString();
                }
                Buffer["_TimeStamp"] = Tools.GetUnixTimeStamp().ToString();
                Buffer["_Name"] = this.Name;
                Buffer["_Id"] = this.Id.ToString();
                Buffer["_Path"] = this.Path;
                Buffer["_LastConnected"] = this.LastConnected.ToString();
                Buffer["_LastRead"] = this.LastRead.ToString();
                Buffer["_Failures"] = this.Failures.ToString();
                this.LastRead = DateTime.Now;
                return this.Buffer;
            }
            catch (Exception e)
            {
                Log(e.Message);

                throw;
            }
        }

        public override void Disconnect()
        {
            try
            {
                _server.Disconnect();
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public override void Dispose()
        {
            _server.Dispose();
        }

        public override string ToString()
        {
            return $"A opc machine ({this.Path}) with {Tags.Keys.Count} tags";
        }
    }
}
