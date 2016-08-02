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
        bool runningtag = true;
        List<Dictionary<string,string>> _buffer = new List<Dictionary<string, string>>();
        private CollectorCallback _callback;

        public Collector(BlockingCollection<Dictionary<string,string>> valuesQueue,CollectorCallback callback, ref bool running )
        {
            _valuesQueue = valuesQueue;
            runningtag = running;
            _callback = callback;
        }

        public void start()
        {
            while (runningtag)
            {
                while (_buffer.Count < 100 && runningtag)
                {
                    _buffer.Add(_valuesQueue.Take());
                }
                _callback.Invoke(_buffer);
                _buffer.Clear();
            }

        }

    }
}
