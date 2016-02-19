using FileIO.XMLReader;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLDatabase
{
	public class DataBaseConfig
	{
		public DataBaseConfig(string configFile, string tableFile)
		{
            DirectoryInfo dir = new DirectoryInfo(configFile);
            this.Path = dir.Parent.FullName;
            List<Node> nodes = XMLReader.ReadXMLFile(configFile);
            IterateNodes(nodes[0].Childs);
			Structure = new DataBaseStructure(tableFile);
		}

        private void IterateNodes(List<Node> nodes)
        {
            foreach(Node node in nodes)
            {
                switch (node.Name)
                {
                    case "DataBase":
                        string dataBase = node.Attributes["Value"];
                        this.DataBaseFile = dataBase + ".db";
                        this.LogFile = dataBase + "_log.txt";
                        this.SetupFile = dataBase + "_setup.xml";
                        this.TestValuesFile = dataBase + "_Testvalues.xml";
                        break;
                    case "Mappings":
                        this.Mapping = new Dictionary<string, string>();
                        foreach (Node n in node.Childs)
                        {
                            if(n.Name == "Mapping")
                                this.Mapping.Add(n.Attributes["type"], n.Attributes["table"]);
                        }
                        break;
                }
            }
        }

		public string Path { get; set; }
		public string DataBaseFile { get; set; }
		public DataBaseStructure Structure { get; set; }
		public string LogFile { get; set; }
		public string SetupFile { get; set; }
		public Dictionary<string, string> Mapping { get; set; }
		public string TestValuesFile { get; set; }
	}
}
