using System;
using System.Collections.Generic;


namespace MolyMade.FieldCommunication
{
    public struct MessageItem
    {
        public string owner;
        public string message;
    }

    public enum MachineTypes
    {
        Opc = 1,
        Modbus = 2,
        Other = 4,
        Other2 = 8
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
}
