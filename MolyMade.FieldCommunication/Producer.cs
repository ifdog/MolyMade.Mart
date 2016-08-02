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
        private Dictionary<string, Dictionary<string, string>> _MachinesDefinition;
        private BlockingCollection<Machine> _quietQueue = new BlockingCollection<Machine>(new ConcurrentQueue<Machine>(),byte.MaxValue);
        private BlockingCollection<Machine> _activeQueue = new BlockingCollection<Machine>(new ConcurrentQueue<Machine>(),byte.MaxValue));
        private BlockingCollection<MessageItem> _messageQueue;
        private BlockingCollection<Dictionary<string,string>> _valuesQueue;
        private readonly int _quietThreads;
        private readonly int _activeThreads;
        private bool runningtag;
        public Producer(
            Dictionary<string,Dictionary<string,string>> machines,
            BlockingCollection<Dictionary<string,string>> valuesQueue, 
            BlockingCollection<MessageItem> messageQueue,
            ref bool running,
            int QuietThreads = 1, 
            int ActiveThreads = 1
          )
        {
            _MachinesDefinition = machines;
            _valuesQueue = valuesQueue;
            _messageQueue = messageQueue;
            _quietThreads = QuietThreads;
            _activeThreads = ActiveThreads;
            runningtag = running;
        }

        public void Start()
        {
            List<Machine> machines = new List<Machine>();
            MachinesMake(_MachinesDefinition,machines);
            QueuesInit(machines,out _quietQueue,out _activeQueue);

            QuietWorker qw = new QuietWorker(_quietQueue,_activeQueue,_messageQueue,ref runningtag);
            ActiveWorker aw = new ActiveWorker(_quietQueue,_activeQueue,_valuesQueue,_messageQueue,ref runningtag);
   
            bool[] QuietThreads = new bool[_quietThreads];
            bool[] ActiveThreads = new bool[_activeThreads];
            foreach (bool b in QuietThreads)
            {
                Thread serverThread = new Thread(qw.Start) { IsBackground = true };
                serverThread.Start();
            }
            foreach (bool b in ActiveThreads)
            {
                Thread valuesThread = new Thread(aw.Start) { IsBackground = true };
                valuesThread.Start();
            }
        }


        public void MachinesMake(Dictionary<string, Dictionary<string, string>> definitions, List<Machine> machines)
        {
            foreach (string key in definitions.Keys)
            {
                Machine aMachine = Machine.CreateInstance(
                    key,
                    Tools.MachineId.Create(),
                    definitions[key]["Path"],
                    (MachineTypes) (Convert.ToInt32(definitions[key]["Type"])),
                    definitions[key]
                        .Where(kv => kv.Key.StartsWith("_"))
                        .Select(kv => new KeyValuePair<string, string>(kv.Key.Remove(0, 1), kv.Value))
                        .ToDictionary(kv => kv.Key, kv => kv.Value));
                machines.Add(aMachine);
            }
        }

        private void QueuesInit(List<Machine> MachineList, 
            out BlockingCollection<Machine> quietQueue, 
            out BlockingCollection<Machine> activeQueue)
        {
            quietQueue = new BlockingCollection<Machine>(new ConcurrentQueue<Machine>(MachineList), byte.MaxValue);
            activeQueue = new BlockingCollection<Machine>(new ConcurrentQueue<Machine>(), byte.MaxValue);
        }
    }
}
