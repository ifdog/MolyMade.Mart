using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MolyMade.FieldCommunication
{
    public class Controller
    {
        private Hashtable OpcMachines;
        private Configurer.ConfigurationData configurationData;
        private BlockingCollection<MessageItem>_messageQueue = 
            new BlockingCollection<MessageItem>(new ConcurrentQueue<MessageItem>(),byte.MaxValue);

        private BlockingCollection<Dictionary<string, string>> _valuesQueue;
        private Producer _producer;
        private RunningTag _runningtag;
        private Collector _collector;
        private CollectorCallback _collectorCallback;

        public Controller(CollectorCallback callback,RunningTag running)
        {
            _collectorCallback = callback;
            _runningtag = running;
            _valuesQueue = new BlockingCollection<Dictionary<string, string>>(new ConcurrentQueue<Dictionary<string, string>>(),byte.MaxValue);
        }

        public void init()
        {
            Configurer c = new Configurer();
            configurationData = c.Load();
            _producer = new Producer(configurationData.Machines,_valuesQueue,_messageQueue, _runningtag);
        }

        public void Start()
        {
            init();
            StartProducer();
            StartCollector();
        }

        public void StartProducer()
        {
            _producer.Start();
        }

        private void StartCollector(int collectorThreads = 1)
        {
            bool[] threads = new bool[collectorThreads];
            foreach (var b in threads)
            {
                var collectorThread = new Thread(() =>
                {
                    _collector = new Collector(_valuesQueue,_collectorCallback, _runningtag);
                    _collector.start();
                })
                { IsBackground = true };
                collectorThread.Start();
            }
        }

    }
}
