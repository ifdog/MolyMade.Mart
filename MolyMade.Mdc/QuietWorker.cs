using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;   

namespace MolyMade.Mdc
{
    class QuietWorker
    {
        private readonly BlockingCollection<Machine> _blockqingQuietQueue;
        private readonly BlockingCollection<Machine> _blockingActiveQueue;
        private readonly BlockingCollection<MessageItem> _blockingMesageQueue;
        private bool _runningtag;

        public QuietWorker(BlockingCollection<Machine> QuietQueue,BlockingCollection<Machine> ActiveQueue,BlockingCollection<MessageItem> MessageQueue,ref bool Running)
        {
            _blockqingQuietQueue = QuietQueue;
            _blockingActiveQueue = ActiveQueue;
            _blockingMesageQueue = MessageQueue;
            _runningtag = Running;
        }

        public Machine MachineConnect(Machine machine)
        {
            if (!machine.IsConnected)
            {
                machine.Connect();
            }
            return machine;
        }

        public void Start()
            {
                {
                    while (_runningtag)
                    {
                    Machine machine = _blockqingQuietQueue.Take(); 
                    _blockingActiveQueue.Add(MachineConnect(machine));
                    }
                Thread.Sleep(1000);
                }
        }
    }
}
