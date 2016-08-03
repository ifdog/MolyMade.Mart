using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;   

namespace MolyMade.FieldCommunication
{
    class QuietWorker
    {
        private readonly BlockingCollection<Machine> _blockqingQuietQueue;
        private readonly BlockingCollection<Machine> _blockingActiveQueue;
        private readonly BlockingCollection<MessageItem> _blockingMesageQueue;
        private RunningTag _runningtag;

        public QuietWorker(BlockingCollection<Machine> QuietQueue,BlockingCollection<Machine> ActiveQueue,BlockingCollection<MessageItem> MessageQueue,RunningTag Running)
        {
            _blockqingQuietQueue = QuietQueue;
            _blockingActiveQueue = ActiveQueue;
            _blockingMesageQueue = MessageQueue;
            _runningtag = Running;
        }

        public void MachineConnect(Machine machine)
        {
            if (!machine.IsConnected)
            {
                machine.Connect();
            }
        }

        public void Start()
            {
                {
                    while (_runningtag.Value)
                    {
                    Machine machine = _blockqingQuietQueue.Take();
                        MachineConnect(machine);
                        if (machine.IsConnected)
                        {
                            _blockingActiveQueue.Add(machine);
                        }
                        else
                        {
                            _blockqingQuietQueue.Add(machine);
                        }
                    }
                Thread.Sleep(100);
                }
        }
    }
}
