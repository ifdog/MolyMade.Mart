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
        internal IniData SysIni;
        internal IniData ServersIni;
        public string SysIniPath { get; internal set; }
        public string ServerIniPath { get; internal set; }
        
        public Configurer(string sysIniPath,string ServerIniPath)
        {
            this.SysIniPath = sysIniPath;
            this.ServerIniPath = ServerIniPath;
        }

        public void Load()
        {
            FileIniDataParser parser = new FileIniDataParser();
            SysIni = parser.ReadFile(this.SysIniPath);
            ServersIni = parser.ReadFile(this.ServerIniPath);
            
            int i = 0;
        }


        public void Dispose()
        {
            throw  new NotImplementedException();
        }
    }
}
