using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace MolyMade.FieldCommunication
{
    public class Controller:ILog
    {
        private Configurer.ConfigurationData _configurationData;
        private readonly BlockingCollection<MessageItem>_messageQueue = 
            new BlockingCollection<MessageItem>(new ConcurrentQueue<MessageItem>(),byte.MaxValue);
        private readonly BlockingCollection<Dictionary<string, string>> _valuesQueue = 
            new BlockingCollection<Dictionary<string, string>>(new ConcurrentQueue<Dictionary<string, string>>(), byte.MaxValue);
        private Producer _producer;
        private readonly RunningTag _runningtag;
        private Collector _collector;
        private readonly CollectorCallback _collectorCallback;
        private Action<List<Dictionary<string, string>>> _action;
        public  BlockingCollection<MessageItem> MessageQueue => _messageQueue;
        private int _warp;

        public Controller(Action<List<Dictionary<string,string>>> callback ,RunningTag running,int warp=10)
        {
            _action = callback;
            _runningtag = running;
            _warp = warp;
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
                    _collector = new Collector(_valuesQueue,_messageQueue,_action, _runningtag,_warp);
                    _collector.start();
                })
                { IsBackground = true };
                collectorThread.Start();
            }
        }


    }
}
