using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MolyMade.FieldCommunication;

namespace MolyMade.FieldUI
{
    class Controller
    {
        private readonly MolyMade.FieldCommunication.Communication _comm;
        public Dictionary<string,Dictionary<string,string>> MachinesValueDictionary = 
            new Dictionary<string, Dictionary<string, string>>();

        private SynchronizationContext _uicontext;
        private SendOrPostCallback _valuesCallback;
        private SendOrPostCallback _messageCallback;

        public Controller(SynchronizationContext context,SendOrPostCallback valuesCallback,SendOrPostCallback messageCallback)
        {
            _comm = new Communication(1);
            _comm.DataMount+= CommOnDataMount;
            _comm.MessageArrive+= CommOnMessageArrive;
            _uicontext = context;
            _valuesCallback = valuesCallback;
            _messageCallback = messageCallback;
        }

        private void CommOnMessageArrive(object sender, MessageArriveArgs args)
        {
            _uicontext.Post(_messageCallback,args.Messages);
        }

        private void CommOnDataMount(object sender, DataMountEventArgs args)
        {
            _uicontext.Post(_valuesCallback, args.Tags);
        }

        public void Start()
        {
            _comm.Start();
        }

        public void Stop()
        {
            _comm.Stop();
        }

    }
}
