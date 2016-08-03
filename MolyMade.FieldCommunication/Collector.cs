using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;

namespace MolyMade.FieldCommunication
{
    class Collector
    {
        BlockingCollection<Dictionary<string, string>> _valuesQueue;
        private RunningTag _runningtag;
        List<Dictionary<string,string>> _buffer = new List<Dictionary<string, string>>();
        private CollectorCallback _callback;

        public Collector(BlockingCollection<Dictionary<string,string>> valuesQueue,CollectorCallback callback, RunningTag running )
        {
            _valuesQueue = valuesQueue;
            _runningtag = running;
            _callback = callback;
        }

        public void start()
        {
            while (_runningtag.Value)
            {
                while (_buffer.Count < 10 && _runningtag.Value)
                {
                    _buffer.Add(_valuesQueue.Take());
                }
                _callback.Invoke(_buffer);
                _buffer.Clear();
            }

        }

    }
}
