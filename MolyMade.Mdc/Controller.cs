using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MolyMade.Mdc
{
    public class Controller
    {
        private Hashtable OpcMachines;
        public Controller()
        {
            
        }

        public void ReadConfig()
        {
            var x = System.Configuration.ConfigurationManager.AppSettings["NewKey0"];
            int a = 1;
        }
    }
}
