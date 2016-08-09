using System;
using System.Collections.Concurrent;
using System.Threading;

namespace MolyMade.FieldCommunication
{
    internal class QuietWorker:ILog
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
            MessageQueue = messageQueue;
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
                        _blockqingQuietQueue.Add(machine);
                        Tools.Log(this, $"{machine.Name} is quiet");
                    }
                }
            }
            Thread.Sleep(100);
            Tools.Log(this, "Exit");
        }


    }
}
