using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace CoDriverConsoleApp
{
    class SceneInfoData
    {
        public string[] keywords;
        //public CSVFile csv_file;
        //public string GetNodeAtt
        public bool Load(string filename)
        {
            XmlDocument document = new XmlDocument();
            XmlReader reader = XmlReader.Create(filename);
            document.Load(reader);
            XmlNode xmlNode = document.SelectSingleNode("/Root/Scene");
            XmlNode xmlMeshAssetNode = document.SelectSingleNode("/Root/Scene/MeshAsset");
            XmlNodeList xmlNodeList =  document.SelectNodes("/Root/Scene/property");

            char[] charSeparators = new char[] { '_', ' ', ';', ',' };
            foreach (XmlNode node in xmlNodeList)
            {
                XmlAttribute att = node.Attributes[0];
                if(att.Name == "Keywords")
                {
                    string keywordStr = att.Value;
                    keywords = keywordStr.Split(charSeparators, StringSplitOptions.RemoveEmptyEntries);
                }
            }

            //csv_file = new CSVFile();
            //csv_file.Load(filename);
            ////char[] charSeparators = new char[] { '_' };
            //foreach (var csv_data in csv_file.m_csv_data)
            //{
            //    string[] data = csv_data.Value.values;
            //    if (data.Length < 4)
            //        continue;
               
            //}

            //image_nodes.Sort();
            return true;
        }
    }
}
