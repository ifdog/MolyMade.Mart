using System;
using System.Threading;

namespace MolyMade.FieldCommunication
{
    public static class Tools
    {
        public static long GetUnixTimeStamp()
        {
            long epoch = (DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000000;
            return epoch;
        }

        public static string GetCurrentTime()
        {
            return DateTime.Now.ToString();
        }

        public static class MachineId
        {
            private static int _mem;
             static MachineId()
             {
                 _mem = 1000;
             }
            public static int Create()
            {
                if (_mem > 99999) { _mem = 0;}
                _mem++;
                return _mem;
            }
            public static void Reset()
            {
                _mem = 0;
            }
        }
        

        public static void Log(object sender,string message, int timeout = 50)
        {
            ILog s = sender as ILog;
            if (s == null) { return;}
            MessageItem item = new MessageItem
            {
                message =  message,
                owner =  sender.GetType().ToString(),
                threadId = Thread.CurrentThread.ManagedThreadId.ToString()
            };
            s.MessageQueue.TryAdd(item, timeout);
            if (s.MessageQueue.Count > 500)
            {
                s.MessageQueue.Take();
            }
        }
    }
}
