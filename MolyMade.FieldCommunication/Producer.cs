using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace MolyMade.FieldCommunication
{
    class Producer
    {
        private readonly Dictionary<string, Dictionary<string, string>> _machinesDefinition;
        private BlockingCollection<Machine> _quietQueue = new BlockingCollection<Machine>(new ConcurrentQueue<Machine>(),byte.MaxValue);
        private BlockingCollection<Machine> _activeQueue = new BlockingCollection<Machine>(new ConcurrentQueue<Machine>(),byte.MaxValue);
        private readonly BlockingCollection<MessageItem> _messageQueue;
        private readonly BlockingCollection<Dictionary<string,string>> _valuesQueue;
        private readonly int _quietThreads;
        private readonly int _activeThreads;
        private readonly RunningTag _runningtag;
        private int _quietThreadsInterval;
        private int _activeThreadsInterval;
        public Producer(
            Dictionary<string,Dictionary<string,string>> machines,
            BlockingCollection<Dictionary<string,string>> valuesQueue, 
            BlockingCollection<MessageItem> messageQueue,
            RunningTag running, 
            int quietThreadsInterval = 100, 
            int activeThreadsInterval= 100, 
            int quietThreads = 1, 
            int activeThreads = 1
          )
        {
            _machinesDefinition = machines;
            _valuesQueue = valuesQueue;
            _messageQueue = messageQueue;
            _quietThreads = quietThreads;
            _activeThreads = activeThreads;
            _runningtag = running;
            _quietThreadsInterval = quietThreadsInterval;
            _activeThreadsInterval = activeThreadsInterval;
        }

        public void Start()
        {
            List<Machine> machines = new List<Machine>();
            MachinesMake(_machinesDefinition,machines);
            QueuesInit(machines,out _quietQueue,out _activeQueue);
            QuietWorker qw = new QuietWorker(_quietQueue,_activeQueue,_messageQueue, _runningtag,_quietThreadsInterval);
            ActiveWorker aw = new ActiveWorker(_quietQueue,_activeQueue,_valuesQueue,_messageQueue, _runningtag,_activeThreadsInterval);
            bool[] quietThreads = new bool[_quietThreads];
            bool[] activeThreads = new bool[_activeThreads];
            foreach (bool b in quietThreads)
            {
                Thread serverThread = new Thread(qw.Start) { IsBackground = true };
                serverThread.Start();
                Thread.Sleep(_quietThreadsInterval / _quietThreads);
            }
            foreach (bool b in activeThreads)
            {
                Thread valuesThread = new Thread(aw.Start) { IsBackground = true };
                valuesThread.Start();
                Thread.Sleep(_activeThreadsInterval/_activeThreads);
            }
        }


        public void MachinesMake(Dictionary<string, Dictionary<string, string>> definitions, List<Machine> machines)
        {
            foreach (string key in definitions.Keys)
            {
                if (!Convert.ToBoolean(definitions[key]["Enable"])) {continue;}
                machines.Add(Machine.CreateInstance(
                    key,
                    Utilities.MachineId.Create(),
                    definitions[key]["Path"],
                    (MachineTypes)(Convert.ToInt32(definitions[key]["Type"])),
                    definitions[key]
                        .Where(kv => kv.Key.StartsWith("_"))
                        .Select(kv => new KeyValuePair<string, string>(kv.Key.Remove(0, 1), kv.Value))
                        .ToDictionary(kv => kv.Key, kv => kv.Value)));
            }
        }

        private void QueuesInit(List<Machine> machineList, 
            out BlockingCollection<Machine> quietQueue, 
            out BlockingCollection<Machine> activeQueue)
        {
            quietQueue = new BlockingCollection<Machine>(new ConcurrentQueue<Machine>(machineList), byte.MaxValue);
            activeQueue = new BlockingCollection<Machine>(new ConcurrentQueue<Machine>(), byte.MaxValue);
        }
    }
}
