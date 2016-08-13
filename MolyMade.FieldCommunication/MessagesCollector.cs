using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MolyMade.FieldCommunication
{
    class MessagesCollector
    {
        private readonly RunningTag _runningtag;

        public event MessageArriveHandler MessageArrive;
        private readonly int _messageWarp;
        public BlockingCollection<MessageItem> MessageQueue { get; }
        public MessagesCollector(BlockingCollection<MessageItem> messageQueue, RunningTag running, int messagewarp=10)
        {
            _runningtag = running;
            MessageQueue = messageQueue;
            _messageWarp = messagewarp;
        }
        public void Start()
        {
            while (_runningtag.Value)
            {
                List<MessageItem> _buffer = new List<MessageItem>();
                while (_buffer.Count < _messageWarp && _runningtag.Value)
                {
                    _buffer.Add(MessageQueue.Take());
                    Utilities.Log(this, $"Took {_messageWarp} from MessageQueue");
                }
                MessageArrive?.Invoke(this, new MessageArriveArgs()
                {
                    Messages  = _buffer
                });
            }
            Utilities.Log(this, "Exit");
        }
    }
}
