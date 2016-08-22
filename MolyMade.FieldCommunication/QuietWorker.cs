using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
 

namespace MolyMade.FieldCommunication
{
    internal class QuietWorker:Ilog
    {
        private readonly BlockingCollection<Machine> _blockqingQuietQueue;
        private readonly BlockingCollection<Machine> _blockingActiveQueue;
        private readonly RunningTag _runningtag;
        public BlockingCollection<MessageItem> MessageQueue { get; }
        private readonly int _interval;

        public QuietWorker(
            BlockingCollection<Machine> quietQueue,
            BlockingCollection<Machine> activeQueue,
            BlockingCollection<MessageItem> messageQueue,
            RunningTag running, int interval = 100)
        {
            _blockqingQuietQueue = quietQueue;
            _blockingActiveQueue = activeQueue;
            this.MessageQueue = messageQueue;
            _runningtag = running;
            _interval = interval;
            Utilities.Log(this,"created");
        }

        public void Start()
        {
            while (_runningtag.Value)
            {
                Machine machine = _blockqingQuietQueue.Take();
                if (!machine.Check())
                {
                    _blockqingQuietQueue.Add(machine);
                    //Utilities.Log(this,$"passed {machine.Name} wtih {machine.Failures} failures");
                    continue;
                }
                try
                {
                    machine.Connect();
                    Utilities.Log(this, $"{machine.Name} connected");
                }
                catch (Exception e)
                {
                    Utilities.Log(this, $"{machine.Name} Fails to connect:{e.Message}({machine.Failures})");
                }
                finally
                {
                    if (machine.IsConnected)
                    {
                        _blockingActiveQueue.Add(machine);
                    }
                    else
                    {
                        Utilities.Log(this, $"{machine.Name} is quiet");
                        _blockqingQuietQueue.Add(machine);
                    }
                }
                Thread.Sleep(_interval);
            }
            Utilities.Log(this, "Exit");
        }


    }
}
