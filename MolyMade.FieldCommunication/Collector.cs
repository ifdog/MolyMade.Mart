using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;

namespace MolyMade.FieldCommunication
{
    class Collector:Ilog
    {
        readonly BlockingCollection<Dictionary<string, string>> _valuesQueue;
        private readonly RunningTag _runningtag;
        readonly List<Dictionary<string,string>> _buffer = new List<Dictionary<string, string>>();
        public event DataMountHandler DataMount ;
        private readonly int _valuesWarp;
        public BlockingCollection<MessageItem> MessageQueue { get; }
        public Collector(BlockingCollection<Dictionary<string,string>> valuesQueue,
            BlockingCollection<MessageItem> messageQueue,
            RunningTag running,
            int valueswarp)
        {
            _valuesQueue = valuesQueue;
            _runningtag = running;
            MessageQueue = messageQueue;
            _valuesWarp = valueswarp;
        }

        public void Start()
        {
            while (_runningtag.Value)
            {
                while (_buffer.Count < _valuesWarp&&_runningtag.Value)
                {
                    _buffer.Add(_valuesQueue.Take());
                    Utilities.Log(this,$"Took {_valuesWarp} from ValuesQueue");
                }
                DataMount?.Invoke(this,new DataMountEventArgs()
                {
                    Tags = _buffer

                });
                _buffer.Clear();
            }
            Utilities.Log(this,"Exit");
        }
    }
}
