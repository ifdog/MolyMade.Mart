using System;
using System.Collections.Generic;
using System.Linq;
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
        private SendOrPostCallback _callback;

        public Controller(SynchronizationContext context,SendOrPostCallback callback)
        {
            _comm = new Communication(2);
            _comm.DataMount+= CommOnDataMount;
            _uicontext = context;
            _callback = callback;
        }

        private void CommOnDataMount(object sender, DataMountEventArgs args)
        {
            _uicontext.Send(_callback, args.Tags);
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
