using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace UdpHandler
{
    public static class ConfigManager
    {
        public enum Mode { client_one, client_two, server};
        public static string GetFromIP(Mode mode)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load("config.xml");
            return doc.DocumentElement.SelectSingleNode("./" + mode.ToString()).Attributes["fromIp"].Value;            
        }

        public static int GetFromPort(Mode mode)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load("config.xml");
            return int.Parse(doc.DocumentElement.SelectSingleNode("./" + mode.ToString()).Attributes["fromPort"].Value);
        }

        public static string GetToIP(Mode mode)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load("config.xml");
            return doc.DocumentElement.SelectSingleNode("./" + mode.ToString()).Attributes["toIp"].Value;
        }

        public static int GetToPort(Mode mode)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load("config.xml");
            return int.Parse(doc.DocumentElement.SelectSingleNode("./" + mode.ToString()).Attributes["toPort"].Value);
        }
    }
}
