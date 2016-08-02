using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IniParser;
using IniParser.Model;

namespace MolyMade.FieldCommunication
{
    public class Configurer:IDisposable
    {
        internal IniData SysIni;
        internal IniData ServersIni;
        public string SysIniPath { get; internal set; }
        public string ServerIniPath { get; internal set; }
        public ConfigurationData Configuration;
        public struct ConfigurationData
        {
            public Dictionary<string, Dictionary<string,string>> System;
            public Dictionary<string, Dictionary<string, string>> Servers;
        }
        
        public Configurer(string sysIniPath = "Mart.ini",string serverIniPath="Servers.ini")
        {
            this.SysIniPath = sysIniPath;
            this.ServerIniPath = serverIniPath;
            this.Configuration = new ConfigurationData();
        }

        public ConfigurationData Load()
        {
            FileIniDataParser parser = new FileIniDataParser();
            SysIni = parser.ReadFile(this.SysIniPath);
            ServersIni = parser.ReadFile(this.ServerIniPath);
            Dictionary < string, Dictionary < string,string>> tmpsysDictionary = new Dictionary<string, Dictionary<string, string>>();
            foreach (SectionData section in SysIni.Sections)
            {
                Dictionary<string,string> tmpDictionary = new Dictionary<string, string>();
                foreach (KeyData key in section.Keys)
                {
                    tmpDictionary[key.KeyName] = key.Value;
                }
                tmpsysDictionary[section.SectionName] = tmpDictionary;
            }
            Configuration.System = tmpsysDictionary;
            Dictionary<string, Dictionary<string, string>> tmpserverDictionary = new Dictionary<string, Dictionary<string, string>>();
            foreach (SectionData section in ServersIni.Sections)
            {
                Dictionary<string,string> tmpDictionary = new Dictionary<string, string>();
                foreach (KeyData key in section.Keys)
                {
                    tmpDictionary[key.KeyName] = key.Value;
                }
                tmpserverDictionary[section.SectionName] = tmpDictionary;
            }
            Configuration.Servers = tmpserverDictionary;
            return this.Configuration;
        }


        public void Dispose()
        {
            throw  new NotImplementedException();
        }
    }
}
