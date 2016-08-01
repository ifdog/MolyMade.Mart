using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MolyMade.Mdc
{
    class Tools
    {
        public static long GetUnixTimeStamp()
        {
            long epoch = (DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000000;
            return epoch;
        }

        public static class MachineId
        {
            private static int mem;
             static MachineId()
             {
                 mem = 0;
             }

            public static int Create()
            {
                if (mem > 99999)
                {
                    mem = 0;
                }
                mem++;
                return mem;
            }

            public static void Reset()
            {
                mem = 0;
            }
        }
    }
}
