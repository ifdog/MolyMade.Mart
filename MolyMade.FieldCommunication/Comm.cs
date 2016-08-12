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
    public class Comm:Ilog
    {
        private Configurer.ConfigurationData _configurationData;
        private readonly BlockingCollection<MessageItem>_messageQueue = 
            new BlockingCollection<MessageItem>(new ConcurrentQueue<MessageItem>(),byte.MaxValue);
        private readonly BlockingCollection<Dictionary<string, string>> _valuesQueue = 
            new BlockingCollection<Dictionary<string, string>>(new ConcurrentQueue<Dictionary<string, string>>(), byte.MaxValue);
        private Producer _producer;
        private readonly RunningTag _runningtag;
        private Collector _collector;
        public  BlockingCollection<MessageItem> MessageQueue => _messageQueue;
        private readonly int _warp;
        private readonly string _sysIniPath;
        private readonly string _serverIniPath;
        public event DataMountHandler DataMount;

        public Comm(int warp = 1000, string sysIniPath = "Mart.ini", string serverIniPath = "Machines.ini")
        {
            _runningtag = new RunningTag()
            {
                Value = true
            };
            Tools.Log(this,"Created");
            _sysIniPath = sysIniPath;
            _serverIniPath = serverIniPath;
            _warp = warp;
        }

        private void Init()
        {
            _runningtag.Value = true;
            Configurer c = new Configurer(_sysIniPath,_serverIniPath);
            _configurationData = c.Load();
            _producer = new Producer(_configurationData.Machines,_valuesQueue,_messageQueue, _runningtag);
            Tools.Log(this,"initlized");
        }

        /// <summary>
        /// Start communication.
        /// </summary>
        public void Start()
        {
            if (_producer != null&&_collector!=null)
            {
                return;
            }
            if(_producer==null ^_collector==null)
            {
                this.Stop();
            }
            Init();
            StartProducer();
            StartCollector();
        }

        /// <summary>
        /// Stop communication
        /// </summary>
        public void Stop()
        {
            if (_producer == null && _collector == null)
            {
                return;
            }
            _runningtag.Value = false; //todo:Test.
            _producer = null;
            _collector = null;
        }

        private void StartProducer()
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
                    _collector = new Collector(_valuesQueue,_messageQueue,_runningtag,_warp);
                    _collector.DataMount += DataMount;
                    _collector.Start();
                })
                { IsBackground = true };
                collectorThread.Start();
            }
        }
    }
}
