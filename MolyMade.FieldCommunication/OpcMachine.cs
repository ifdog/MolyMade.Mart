using System;
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

        public OpcMachine(string name, int id, string path, MachineTypes type,
            Dictionary<string, string> tags) : base(name, id, path, type, tags)
        {
            //path:"PCU50|OPC.SINUMERIK.Machineswitch"
            string[] urlStrings = path.Split('|');
            if (urlStrings.Length != 2)
            {
                throw new ArgumentException($"Invaild path string :{path}");
            }
            this._url = UrlBuilder.Build(urlStrings[1], urlStrings[0]);
            _server = new OpcDaServer(this._url);
            _itemDefinitions = tags.Keys.Select(i => new OpcDaItemDefinition()
            {
                ItemId = i.Trim(),
                IsActive = true
            }).ToArray();
            this.State = MachineState.NewlyCreated;
        }

        public override void Connect()
        {
            try
            {
                _server.Connect();
                IsConnected = _server.IsConnected;
                _group = _server.AddGroup("MolyMadeGroup");
                _group.IsActive = true;
                _group.AddItems(_itemDefinitions);
                this.State = MachineState.Connected;
                this.IsConnected = true;
                this.LastConnected = Tools.GetUnixTimeStamp();
                Failures = 0;
            }
            catch (Exception e)
            {
                this.State= MachineState.FailToConnect;
                this.IsConnected = false;
                if (Failures < 999)
                {
                    Failures ++;
                }
                Log(e.Message);
            }
        }

        public override void Read()
        {
            try
            {
                _itemValues = _group.Read(_group.Items, OpcDaDataSource.Device);
                foreach (OpcDaItemValue value in _itemValues)
                {
                    Buffer[Tags[value.Item.ItemId]] = value.Value.ToString();
                }
                Buffer["_TimeStamp"] = Tools.GetUnixTimeStamp().ToString();
                this.State = MachineState.SuccessfullyRead;
                this.LastRead = Tools.GetUnixTimeStamp();
            }
            catch (Exception e)
            {
                Log(e.Message);
                this.State = MachineState.FailToRead;
            }
        }

        public override void Disconnect()
        {
            _server.Disconnect();
            this.State = MachineState.Disconnected;
            this.IsConnected = false;
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
