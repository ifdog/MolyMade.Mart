﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MolyMade.FieldCommunication
{
    public static class Utilities
    {
        public static long GetUnixTimeStamp()
        {
            long epoch = (DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000000;
            return epoch;
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
                if (_mem > 9999) { _mem = 0;}
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
            Ilog s = sender as Ilog;
            if (s == null) { return;}
            MessageItem item = new MessageItem()
            {
                Time = DateTime.Now.ToLongTimeString(),
                Message =  message,
                Owner =  sender.GetType().Name,
                ThreadId = Thread.CurrentThread.ManagedThreadId.ToString()
            };
            s.MessageQueue.TryAdd(item, timeout);
        }

    }
}
