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
        public new bool IsConnected => _server.IsConnected;

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
            this.State = MachineState.NewlyCreated;
        }

        public override void Connect()
        {
            try
            {
                _server.Connect();
                _group = _server.AddGroup("MolyMadeGroup");
                _group.IsActive = true;
                _group.AddItems(_itemDefinitions);
                this.State = MachineState.Connected;
                this.LastConnected = Tools.GetUnixTimeStamp();
                Failures = 0;
            }
            catch (Exception e)
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
                this.State = MachineState.SuccessfullyRead;
                this.LastRead = Tools.GetUnixTimeStamp();
                return this.Buffer;
            }
            catch (Exception e)
            {
                Log(e.Message);
                this.State = MachineState.FailToRead;
                throw;
            }
        }

        public override void Disconnect()
        {
            try
            {
                _server.Disconnect();
                this.State = MachineState.Disconnected;
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
