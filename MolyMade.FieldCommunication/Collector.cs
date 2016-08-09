﻿using System.Collections.Concurrent;
using System.Collections.Generic;

namespace MolyMade.FieldCommunication
{
    class Collector:ILog
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
            int valueswarp)
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
                while (_buffer.Count < _valuesWarp && _runningtag.Value)
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
