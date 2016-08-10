using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace MolyMade.FieldCommunication
{
    class ActiveWorker:Ilog
    {
        private readonly BlockingCollection<Machine> _blockingQuietQueue;
        private readonly BlockingCollection<Machine> _blockingActiveQueue;
        private readonly BlockingCollection<Dictionary<string,string>> _blockingValuesQueue;
        private readonly BlockingCollection<MessageItem> _messageQueue;
        private RunningTag _runningtag;
        public BlockingCollection<MessageItem> MessageQueue { get { return _messageQueue; }}
        public ActiveWorker(BlockingCollection<Machine> blockingQuietQueue, 
            BlockingCollection<Machine> blockingActiveQueue, 
            BlockingCollection<Dictionary<string,string>> blockingServerValuesQueue, 
            BlockingCollection<MessageItem> messageQueue,
            RunningTag Running)
        {
            _blockingQuietQueue = blockingQuietQueue;
            _blockingActiveQueue = blockingActiveQueue;
            _blockingValuesQueue = blockingServerValuesQueue;
            _messageQueue = messageQueue;
            _runningtag = Running;
            Tools.Log(this,"Created");
        }

        public void Start()
        {
            while (_runningtag.Value)
            {
                Machine machine = _blockingActiveQueue.Take();
                if (machine.IsConnected)
                {
                    try
                    {
                        _blockingValuesQueue.Add(machine.Read());
                    }
                    catch (Exception e)
                    {
                        Tools.Log(this, $"{machine.Name} fails to read:{e.Message}");
                    }
                    finally
                    {
                        _blockingActiveQueue.Add(machine);
                    }
                }
                else
                {
                    Tools.Log(this,$"{machine.Name} is quiet");
                    _blockingQuietQueue.Add(machine);
                }
                Thread.Sleep(100);
            }
            Tools.Log(this,"Exit");
        }


    }
}
