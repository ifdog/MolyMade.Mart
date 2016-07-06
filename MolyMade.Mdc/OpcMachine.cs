using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MolyMade.Mdc
{
    class OpcMachine:Machine
    {
        public OpcMachine(string name, int id, string path, int failures, MachineTypes type,
            Dictionary<string, string> tags) : base(name, id, path, failures, type, tags)
        {
            
        }
        
    }
}
