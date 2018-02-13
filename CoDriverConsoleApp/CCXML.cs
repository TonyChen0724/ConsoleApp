using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Xml;


namespace CoDriverConsoleApp
{
    class CCXML
    {
        struct PhotoInfo
        {
            public int id;
            public string image_path;
           // public double yaw;
          //  public double pitch;
          //  public double roll;
            public double x;
            public double y;
            public double z;
            public double Latitude;
            public double Longitude;
            public double Altitude;
            public double[] mt;
         }

        string output_folder;
        double focallength;
        double ppx;
        double ppy;
        double distorttion_p1;
        double distorttion_p2;
        double distorttion_p3;
        double distorttion_k1;
        double distorttion_k2;
        int width;
        int height;
        List<PhotoInfo> photo_info_list = new List<PhotoInfo>();

        public void Process()
        {
            foreach (PhotoInfo node in photo_info_list)
            {
                XMPFile xmp_file = new XMPFile();
                xmp_file.LoadXML("Sample.xmp");
                double focalLen = focallength;
                double ppx_mm = ppx / width - 0.5;
                double ppy_mm = ppy / height - 0.5;
                xmp_file.SetParameters(focallength, ppx_mm, ppy_mm);
                xmp_file.SetPosition(node.x,node.y,node.z);
                xmp_file.SetRotation(node.mt);
                xmp_file.SetDistortion(distorttion_p1, distorttion_p2, distorttion_p3, distorttion_k1, distorttion_k2);
                string filename = Path.GetFileNameWithoutExtension(node.image_path);
                filename += ".xmp";
                filename = Path.Combine(output_folder, filename);
                xmp_file.SaveXML(filename);
            }
        }
        public void Load(string filename)
        {
            output_folder = Path.GetDirectoryName(filename);
            XmlDocument document = new XmlDocument();
            XmlReader reader = XmlReader.Create(filename);
            document.Load(reader);
            XmlNodeList xmlNodeList = document.SelectNodes("/BlocksExchange/Block/Photogroups/Photogroup");

            char[] charSeparators = new char[] { '_', ' ', ';', ',' };
            foreach (XmlNode node in xmlNodeList)
            {
                XmlNodeList childNodes = node.ChildNodes;
                foreach(XmlNode childnode in childNodes)
                {
                    if(childnode.Name == "FocalLength")
                    {
                        focallength = Double.Parse(childnode.InnerText);
                        continue;
                    }
                    if (childnode.Name == "ImageDimensions")
                    {
                        width = Convert.ToInt32(childnode.ChildNodes[0].InnerText);
                        height = Convert.ToInt32(childnode.ChildNodes[1].InnerText);
                        continue;
                    }
                    if (childnode.Name == "PrincipalPoint")
                    {
                        ppx = Double.Parse(childnode.ChildNodes[0].InnerText);
                        ppy = Double.Parse(childnode.ChildNodes[1].InnerText);
                        continue;
                    }
                    if (childnode.Name == "Distortion")
                    {
                        distorttion_p1 = Double.Parse(childnode.ChildNodes[0].InnerText);
                        distorttion_p2 = Double.Parse(childnode.ChildNodes[1].InnerText);
                        distorttion_p3 = Double.Parse(childnode.ChildNodes[2].InnerText);
                        distorttion_k1 = Double.Parse(childnode.ChildNodes[3].InnerText);
                        distorttion_k2 = Double.Parse(childnode.ChildNodes[4].InnerText);
                        continue;
                    }
                    if (childnode.Name == "Photo")
                    {
                        ProcessNode_Photo(childnode);
                        continue;
                    }
                }

            }
            reader.Close();
        }
        
        void ProcessNode_Photo(XmlNode node)
        {
            PhotoInfo pi = new PhotoInfo();

            pi.mt = new double[9];
            XmlNodeList childNodes = node.ChildNodes;
            foreach (XmlNode childnode in childNodes)
            {
                if (childnode.Name == "Id")
                {
                    pi.id = Convert.ToInt32(childnode.InnerText);
                    continue;
                }
                if (childnode.Name == "ImagePath")
                {
                    pi.image_path = childnode.InnerText;
                    continue;
                }
                if (childnode.Name == "Pose")
                {
                    foreach (XmlNode child in childnode.ChildNodes)
                    {
                        if (child.Name == "Rotation")
                        {
                            if(child.ChildNodes.Count == 3)
                            {
                                pi.mt[0] = Double.Parse(child.ChildNodes[0].InnerText);
                                pi.mt[1] = Double.Parse(child.ChildNodes[1].InnerText);
                                pi.mt[2] = Double.Parse(child.ChildNodes[2].InnerText);
                            }
                            if (child.ChildNodes.Count == 9)
                            {
                                pi.mt[0] = Double.Parse(child.ChildNodes[0].InnerText);
                                pi.mt[1] = Double.Parse(child.ChildNodes[1].InnerText);
                                pi.mt[2] = Double.Parse(child.ChildNodes[2].InnerText);
                                pi.mt[3] = Double.Parse(child.ChildNodes[3].InnerText);
                                pi.mt[4] = Double.Parse(child.ChildNodes[4].InnerText);
                                pi.mt[5] = Double.Parse(child.ChildNodes[5].InnerText);
                                pi.mt[6] = Double.Parse(child.ChildNodes[6].InnerText);
                                pi.mt[7] = Double.Parse(child.ChildNodes[7].InnerText);
                                pi.mt[8] = Double.Parse(child.ChildNodes[8].InnerText);
                            }
                        }
                        if (child.Name == "Center")
                        {
                            pi.x = Double.Parse(child.ChildNodes[0].InnerText);
                            pi.y = Double.Parse(child.ChildNodes[1].InnerText);
                            pi.z = Double.Parse(child.ChildNodes[2].InnerText);
                        }
                    }
                    continue;
                }
                if (childnode.Name == "ExifData")
                {
                    foreach (XmlNode child in childnode.ChildNodes)
                    {
                        if (child.Name == "GPS")
                        {
                            foreach (XmlNode c in child.ChildNodes)
                            {
                                if (c.Name == "Latitude")
                                {
                                    pi.Latitude = Double.Parse(c.InnerText);
                                }
                                if (c.Name == "Longitude")
                                {
                                    pi.Longitude = Double.Parse(c.InnerText);
                                }
                                if (c.Name == "Altitude")
                                {
                                    pi.Altitude = Double.Parse(c.InnerText);
                                }
                            }
                        }
                    }
                    continue;
                }
            }

            photo_info_list.Add(pi);
            return;
        }
    }
}
