using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MolyMade.FieldCommunication;

namespace MolyMade.FieldUI
{
    class Controller
    {
        private MolyMade.FieldCommunication.Comm _comm;
        public Dictionary<string,Dictionary<string,string>> MachinesValueDictionary = 
            new Dictionary<string, Dictionary<string, string>>();
       

        public Controller()
        {
            _comm = new Comm(10);
            _comm.DataMount+= CommOnDataMount;
        }

        private void CommOnDataMount(object sender, DataMountEventArgs args)
        {
            foreach (Dictionary<string,string> dict in args.Tags)
            {
                string _id = dict["_Id"];
                MachinesValueDictionary[_id] = dict;
            }
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
