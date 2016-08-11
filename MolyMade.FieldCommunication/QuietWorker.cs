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

        public QuietWorker(
            BlockingCollection<Machine> quietQueue,
            BlockingCollection<Machine> activeQueue,
            BlockingCollection<MessageItem> messageQueue,
            RunningTag running)
        {
            _blockqingQuietQueue = quietQueue;
            _blockingActiveQueue = activeQueue;
            this.MessageQueue = messageQueue;
            _runningtag = running;
            Tools.Log(this,"created");
        }

        public void Start()
        {
            while (_runningtag.Value)
            {
                Machine machine = _blockqingQuietQueue.Take();
                try
                {
                    machine.Connect();
                }
                catch (Exception e)
                {
                    Tools.Log(this, $"{machine.Name} Fails to connect:{e.Message}");
                }
                finally
                {
                    if (machine.IsConnected)
                    {
                        _blockingActiveQueue.Add(machine);
                    }
                    else
                    {
                        Tools.Log(this, $"{machine.Name} is quiet");
                        _blockqingQuietQueue.Add(machine);
                    }
                }
                Thread.Sleep(100);
            }
            Tools.Log(this, "Exit");
        }


    }
}
