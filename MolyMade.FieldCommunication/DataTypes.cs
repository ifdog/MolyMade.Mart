using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;


namespace MolyMade.FieldCommunication
{
    public delegate void DataMountHandler(object sender, DataMountEventArgs args);

    public delegate void MessageArriveHandler(object sender,MessageArriveArgs args);

    public class RunningTag
    {
        public bool Value;
    }

    public struct MessageItem
    {
        public string Time;
        public string Owner;
        public string ThreadId;
        public string Message;
    }

    public enum MachineTypes
    {
        Testing = 0, //a fake server,used for testing.
        Siunmerik = 1,  //Sinumerik server
        Moxa = 2, //Moxa module
    }

    public enum MachineState
    {
        NewlyCreated = 1,
        Connected = 2,
        FailToConnect =4,
        SuccessfullyRead = 8,
        FailToRead =16,
        Disconnected = 32,
    }

    public interface Ilog
    {
        BlockingCollection<MessageItem> MessageQueue { get; }
    }

    public class DataMountEventArgs : EventArgs
    {
        public List<Dictionary<string, string>> Tags;
    }

    public class MessageArriveArgs : EventArgs
    {
        public List<MessageItem> Messages;
    }
}
