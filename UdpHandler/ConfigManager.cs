using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace UdpHandler
{
    public class Node
    {
        public string Address { get; set; }
        public int Port { get; set; }        


        public Node(XmlNode node)
        {
            this.Address =  node.Attributes["address"].Value;
            this.Port = int.Parse(node.Attributes["port"].Value);            
        }
    }

    public class ConfigManager
    {
        public Node Host { get; set; }
        public Node Client { get; set; }
        public List<Node> Servers { get; set; }

        public enum Mode { client_one, client_two, server};
        
        public ConfigManager()
        {
            Servers = new List<Node>();

            XmlDocument doc = new XmlDocument();
            doc.Load("config.xml");

            Host = new Node(doc.DocumentElement.SelectSingleNode("./clients/host"));
            Client = new Node(doc.DocumentElement.SelectSingleNode("./clients/client"));

            foreach (XmlNode node in doc.DocumentElement.SelectNodes("./servers/server"))
            {
                Servers.Add(new Node(node));
            }

        }
    }
}
