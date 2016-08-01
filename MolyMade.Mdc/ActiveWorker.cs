using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace MolyMade.Mdc
{
    class ActiveWorker
    {
        private readonly BlockingCollection<Machine> _blockingQuietQueue;
        private readonly BlockingCollection<Machine> _blockingActiveQueue;
        private readonly BlockingCollection<Dictionary<string,string>> _blockingServerValuesQueue;
        private readonly BlockingCollection<MessageItem> _messageQueue;
        private bool _runningtag;
        public ActiveWorker(BlockingCollection<Machine> blockingQuietQueue, BlockingCollection<Machine> blockingActiveQueue, BlockingCollection<Dictionary<string,string>> blockingServerValuesQueue, BlockingCollection<MessageItem> messageQueue,ref bool Running)
        {
            _blockingQuietQueue = blockingQuietQueue;
            _blockingActiveQueue = blockingActiveQueue;
            _blockingServerValuesQueue = blockingServerValuesQueue;
            _messageQueue = messageQueue;
            _runningtag = Running;
        }

        public Dictionary<string, string> MachineRead(Machine machine)
        {
            machine.Read();
            return machine.Buffer;
        }

        public void Start()
        {
            while (_runningtag)
            {
                Machine machine = _blockingActiveQueue.Take();
                if (machine.IsConnected)
                {
                    _blockingServerValuesQueue.Add(MachineRead(machine));
                    _blockingActiveQueue.Add(machine);
                }
                else
                {
                    _blockingQuietQueue.Add(machine);
                }
                Thread.Sleep(1000);
            }
        }
    }
}
