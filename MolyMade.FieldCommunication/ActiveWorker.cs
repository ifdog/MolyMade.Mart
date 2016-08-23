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
        private readonly RunningTag _runningtag;
        public BlockingCollection<MessageItem> MessageQueue { get; }
        private readonly int _interval;
        private readonly int _readRetry;

        public ActiveWorker(BlockingCollection<Machine> blockingQuietQueue, 
            BlockingCollection<Machine> blockingActiveQueue, 
            BlockingCollection<Dictionary<string,string>> blockingServerValuesQueue, 
            BlockingCollection<MessageItem> messageQueue,
            RunningTag running, int readRetry, int interval=100)
        {
            _blockingQuietQueue = blockingQuietQueue;
            _blockingActiveQueue = blockingActiveQueue;
            _blockingValuesQueue = blockingServerValuesQueue;
            MessageQueue = messageQueue;
            _runningtag = running;
            _readRetry = readRetry;
            _interval = interval;
            Utilities.Log(this,"Created");
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
                        Utilities.Log(this, $"{machine.Name} fails to read:{e.Message}");
                    }
                    finally
                    {
                        if (machine.ReadErrors >= _readRetry)
                        {
                            machine.Disconnect();
                            _blockingQuietQueue.Add(machine);
                        }
                        else
                        {
                            _blockingActiveQueue.Add(machine);
                        }
                    }
                }
                else
                {
                    Utilities.Log(this,$"{machine.Name} is quiet");
                    _blockingQuietQueue.Add(machine);
                }
                Thread.Sleep(_interval);
            }
            
            Utilities.Log(this,"Exit");
        }

    }
}
