using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MolyMade.FieldCommunication;

namespace MolyMade.TestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            Controller c = new Controller(cb,new RunningTag());
            c.Start();
        }

        public static void cb(List<Dictionary<string, string>> a)
        {
            Console.WriteLine("aaa");
        }

    }
}
