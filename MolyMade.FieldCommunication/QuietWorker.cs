using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;   

namespace MolyMade.FieldCommunication
{
    class QuietWorker:Ilog
    {
        private readonly BlockingCollection<Machine> _blockqingQuietQueue;
        private readonly BlockingCollection<Machine> _blockingActiveQueue;
        private RunningTag _runningtag;
        public BlockingCollection<MessageItem> MessageQueue { get; }

        public QuietWorker(BlockingCollection<Machine> QuietQueue,BlockingCollection<Machine> ActiveQueue,BlockingCollection<MessageItem> MessageQueue,RunningTag Running)
        {
            _blockqingQuietQueue = QuietQueue;
            _blockingActiveQueue = ActiveQueue;
            this.MessageQueue = MessageQueue;
            _runningtag = Running;
            Tools.Log(this,"created");
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
                            Tools.Log(this,$"{machine.Name} is quiet");
                            _blockqingQuietQueue.Add(machine);
                        }
                    }
                Thread.Sleep(100);
                }
            Tools.Log(this,"Exit");
        }


    }
}
