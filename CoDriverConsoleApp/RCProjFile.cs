using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.IO;


namespace CoDriverConsoleApp
{
    class RCProjFile
    {
        XmlDocument document = new XmlDocument();
        public void LoadXML(string filename)
        {
            XmlReader reader = XmlReader.Create(filename);
            document.Load(reader);

            reader.Close();
        }
        public void AddImages(string folder)
        {
            if (!Directory.Exists(folder))
            {
                Program.AddLog("RCProjFile: No Path."  + folder);
                return;
            }
            string[] files = Directory.GetFiles(folder);         

            XmlNode xmlNode = document.ChildNodes[0];
            XmlAttributeCollection rootAtt = xmlNode.Attributes;
            XmlNodeList childNodes = xmlNode.ChildNodes;
            foreach (XmlNode node in childNodes)
            {
                if (node.Name == "source")
                {
                    foreach (var file in files)
                    {
                        string ext = Path.GetExtension(file);
                        ext = ext.ToLower();
                        if (ext == ".jpg")
                        {
                            //XmlNodeList childNodes2 = node.ChildNodes;
                            XmlNode newNode = document.CreateElement("input");
                            XmlAttribute newAtt = document.CreateAttribute("fileName");
                            newAtt.InnerText = file;
                            newNode.Attributes.Append(newAtt);
                            node.AppendChild(newNode);
                        }
                    }
                    continue;
                }
            }
            return;
        }
        public void RemoveImages()
        {
            XmlNode xmlNode = document.ChildNodes[0];
            XmlAttributeCollection rootAtt = xmlNode.Attributes;
            XmlNodeList childNodes = xmlNode.ChildNodes;
            foreach (XmlNode node in childNodes)
            {
                if (node.Name == "source")
                {
                    node.RemoveAll();
                    continue;
                }
            }
        }
        public void AddImages(string[] imgList)
        {

            XmlNode xmlNode = document.ChildNodes[0];
            XmlAttributeCollection rootAtt = xmlNode.Attributes;
            XmlNodeList childNodes = xmlNode.ChildNodes;
            foreach (XmlNode node in childNodes)
            {
                if (node.Name == "source")
                {
                    foreach (var file in imgList)
                    {
                        XmlNode newNode = document.CreateElement("input");
                        XmlAttribute newAtt = document.CreateAttribute("fileName");
                        newAtt.InnerText = file;
                        newNode.Attributes.Append(newAtt);
                        node.AppendChild(newNode);
                    }
                    continue;
                }
            }
            return;
        }
        public void SetGroup()
        {
            XmlNode xmlNode = document.ChildNodes[0];
            XmlAttributeCollection rootAtt = xmlNode.Attributes;
            //rootAtt.Remove(rootAtt[])
            XmlNodeList childNodes = xmlNode.ChildNodes;
            foreach (XmlNode node in childNodes)
            {
                if (node.Name == "coordinates")
                {
                    XmlNodeList childNodes2 = node.ChildNodes;
                    foreach (XmlNode node2 in childNodes2)
                    {
                        if (node2.Name == "projectCoordinates")
                        {
                            XmlAttributeCollection attCollection = node2.Attributes;
                            attCollection["index"].Value = "-1";
                            //attCollection["calibrationGroup"].Value = "0";
                            //attCollection["distortionGroup"].Value = "0";
                            continue;
                        }
                    }
                    continue;
                }
                if (node.Name == "source")
                {
                    XmlNodeList childNodes2 = node.ChildNodes;
                    foreach (XmlNode node2 in childNodes2)
                    {
                        if (node2.Name == "input")
                        {
                            XmlAttributeCollection attCollection = node2.Attributes;
                            attCollection["relGroup"].Value = "1";
                            attCollection["calibrationGroup"].Value = "1";
                            attCollection["distortionGroup"].Value = "0";
                            continue;
                        }
                    }
                    continue;
                }
                if (node.Name == "reconstructions")
                {
                    continue;
                }
                if (node.Name == "controlpoints")
                {
                    continue;
                }
            }
        }

        public void SaveXML(string output_filename)
        {
            //XmlWriter writer = XmlWriter.Create("TestSave.xmp");
            document.Save(output_filename);
        }

    }
}
