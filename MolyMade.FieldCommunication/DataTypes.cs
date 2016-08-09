using System.Collections.Concurrent;
using System.Collections.Generic;

namespace MolyMade.FieldCommunication
{
    public delegate void CollectorCallback(List<Dictionary<string, string>> valuesList);

    public class RunningTag
    {
        public bool Value;
    }

    public struct MessageItem
    {
        public string owner;
        public string threadId;
        public string message;
    }

    public enum MachineTypes
    {
        Opc = 1,
        Modbus = 2,
        Other = 3,
        Other2 = 4
    }

    public enum MachineState
    {
        NewlyCreated = 1,
        Connected = 2,
        FailToConnect =4,
        SuccessfullyRead = 8,
        FailToRead =16,
        Disconnected = 32
    }

    public interface ILog
    {
        BlockingCollection<MessageItem> MessageQueue { get; }
    }
}
