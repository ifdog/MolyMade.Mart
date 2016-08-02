using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MolyMade.FieldCommunication
{
    public class Controller
    {
        private Hashtable OpcMachines;
        private Configurer.ConfigurationData configurationData;
        public Controller()
        {
            
        }

        public void init()
        {
            Configurer c = new Configurer();
            configurationData = c.Load();
        }
        
    }
}
