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
    public class Controller:Ilog
    {
        private Configurer.ConfigurationData _configurationData;
        private readonly BlockingCollection<MessageItem>_messageQueue = 
            new BlockingCollection<MessageItem>(new ConcurrentQueue<MessageItem>(),byte.MaxValue);
        private readonly BlockingCollection<Dictionary<string, string>> _valuesQueue = 
            new BlockingCollection<Dictionary<string, string>>(new ConcurrentQueue<Dictionary<string, string>>(), byte.MaxValue);
        private Producer _producer;
        private RunningTag _runningtag;
        private Collector _collector;
        private CollectorCallback _collectorCallback;
        public  BlockingCollection<MessageItem> MessageQueue => _messageQueue;

        public Controller(CollectorCallback callback,RunningTag running)
        {
            _collectorCallback = callback;
            _runningtag = running;
            Tools.Log(this,"Created");
        }

        public void Init()
        {
            Configurer c = new Configurer();
            _configurationData = c.Load();
            _producer = new Producer(_configurationData.Machines,_valuesQueue,_messageQueue, _runningtag);
            Tools.Log(this,"initlized");
        }

        public void Start()
        {
            Init();
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
                    _collector = new Collector(_valuesQueue,_messageQueue,_collectorCallback, _runningtag,10);
                    _collector.start();
                })
                { IsBackground = true };
                collectorThread.Start();
            }
        }


    }
}
