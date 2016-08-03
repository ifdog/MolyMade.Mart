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
        BlockingCollection<Dictionary<string, string>> _valuesQueue;
        private RunningTag _runningtag;
        List<Dictionary<string,string>> _buffer = new List<Dictionary<string, string>>();
        private CollectorCallback _callback;
        private int _valuesWarp;
        public BlockingCollection<MessageItem> MessageQueue { get; }

        public Collector(BlockingCollection<Dictionary<string,string>> valuesQueue,
            BlockingCollection<MessageItem> messageQueue,
            CollectorCallback callback, 
            RunningTag running,
            int valueswarp = 1000)
        {
            _valuesQueue = valuesQueue;
            _runningtag = running;
            _callback = callback;
            MessageQueue = messageQueue;
            _valuesWarp = valueswarp;
        }

        public void start()
        {
            while (_runningtag.Value)
            {
                while (_buffer.Count < 10 && _runningtag.Value)
                {
                    _buffer.Add(_valuesQueue.Take());
                    Tools.Log(this,$"Took {_valuesWarp} from ValuesQueue");
                }
                _callback.Invoke(_buffer);
                _buffer.Clear();
            }

        }


    }
}
