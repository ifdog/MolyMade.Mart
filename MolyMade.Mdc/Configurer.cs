using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IniParser;
using IniParser.Model;

namespace MolyMade.Mdc
{
    public class Configurer:IDisposable
    {
        internal IniData IniData;
        public string iniPath { get; internal set; }
        public string[] ServerStrings { get; internal set; }
        public Configurer(string iniPath)
        {
            this.iniPath= iniPath;
        }

        public void Load()
        {
            FileIniDataParser parser = new FileIniDataParser();
            IniData = parser.ReadFile(this.iniPath);
        }


        public void Dispose()
        {
            throw  new NotImplementedException();
        }
    }
}
