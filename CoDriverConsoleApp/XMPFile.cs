using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Xml;
using MathNet.Numerics.LinearAlgebra;


//FORCEINLINE FRotationTranslationMatrix::FRotationTranslationMatrix(const FRotator& Rot, const FVector& Origin)
//{
//#if PLATFORM_ENABLE_VECTORINTRINSICS

//	const VectorRegister Angles = MakeVectorRegister(Rot.Pitch, Rot.Yaw, Rot.Roll, 0.0f);
//	const VectorRegister HalfAngles = VectorMultiply(Angles, GlobalVectorConstants::DEG_TO_RAD);

//	union { VectorRegister v; float f[4]; } SinAngles, CosAngles;
//	VectorSinCos(&SinAngles.v, &CosAngles.v, &HalfAngles);

//	const float	SP	= SinAngles.f[0];
//	const float	SY	= SinAngles.f[1];
//	const float	SR	= SinAngles.f[2];
//	const float	CP	= CosAngles.f[0];
//	const float	CY	= CosAngles.f[1];
//	const float	CR	= CosAngles.f[2];

//#else

//    float SP, SY, SR;
//    float CP, CY, CR;
//    FMath::SinCos(&SP, &CP, FMath::DegreesToRadians(Rot.Pitch));
//    FMath::SinCos(&SY, &CY, FMath::DegreesToRadians(Rot.Yaw));
//    FMath::SinCos(&SR, &CR, FMath::DegreesToRadians(Rot.Roll));

//#endif // PLATFORM_ENABLE_VECTORINTRINSICS

//    M[0][0] = CP * CY;
//    M[0][1] = CP * SY;
//    M[0][2] = SP;
//    M[0][3] = 0.f;

//    M[1][0] = SR * SP * CY - CR * SY;
//    M[1][1] = SR * SP * SY + CR * CY;
//    M[1][2] = -SR * CP;
//    M[1][3] = 0.f;

//    M[2][0] = -(CR * SP * CY + SR * SY);
//    M[2][1] = CY * SR - CR * SP * SY;
//    M[2][2] = CR * CP;
//    M[2][3] = 0.f;

//    M[3][0] = Origin.X;
//    M[3][1] = Origin.Y;
//    M[3][2] = Origin.Z;
//    M[3][3] = 1.f;
//}





namespace CoDriverConsoleApp
{
    class XMPFile
    {
        public double[] rotation = new double[9];
        public double[] position = new double[3];
        public string src_filename;
        public Dictionary<int, string> m_xmp_data = new Dictionary<int, string>();
        int rotation_idx;
        int position_idx;

        public void Load(string filename)
        {
            src_filename = filename;
            StreamReader reader = new StreamReader(filename);
            int count = 0;
            char[] charSeparators = new char[] { '<', '>',' ' };
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                m_xmp_data[count] = line;
                count++;
                if (line.Length <= 3)
                    continue;
                if (line[0] == '#')
                    continue;
                var values = line.Split(charSeparators, StringSplitOptions.RemoveEmptyEntries);
                if (values.Length < 2)
                    continue;
                if(values[0].Contains("Rotation"))
                {
                    rotation_idx = count - 1;
                    rotation[0] = Convert.ToDouble(values[1]);
                    rotation[1] = Convert.ToDouble(values[2]);
                    rotation[2] = Convert.ToDouble(values[3]);
                    rotation[3] = Convert.ToDouble(values[4]);
                    rotation[4] = Convert.ToDouble(values[5]);
                    rotation[5] = Convert.ToDouble(values[6]);
                    rotation[6] = Convert.ToDouble(values[7]);
                    rotation[7] = Convert.ToDouble(values[8]);
                    rotation[8] = Convert.ToDouble(values[9]);
                }
                if (values[0].Contains("Position"))
                {
                    position_idx = count - 1;
                    position[0] = Convert.ToDouble(values[1]);
                    position[1] = Convert.ToDouble(values[2]);
                    position[2] = Convert.ToDouble(values[3]);
                }
                //CSVLine line_data = new CSVLine();
                //line_data.values = values;
                //line_data.data = line;
                //if (count == 0)
                //    keywords = values;

                //m_csv_data.Add(count, line_data);
                //count++;
            }
            //number = m_csv_data.Count - 1;
            reader.Close();
        }
        

        public void WriteRotation(double[] rotation)
        {
            string new_line = "      <xcr:Rotation>";
            new_line += string.Format("{0} {1} {2} {3} {4} {5} {6} {7} {8}",rotation[0], rotation[1], rotation[2], rotation[3], rotation[4], rotation[5], rotation[6], rotation[7], rotation[8]);
            new_line += "</xcr:Rotation>";
            m_xmp_data[rotation_idx] = new_line;
        }
        public void Save()
        {
            string folder = Path.GetDirectoryName(src_filename);
            string filename = Path.GetFileName(src_filename);
            folder += "\\Convert\\";
            string save_filename = folder + filename;
            //string folder = Path.GetDirectoryName(src_filename);
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);
            if (File.Exists(save_filename))
                File.Delete(save_filename);
            StreamWriter writer = new StreamWriter(save_filename);
            foreach (var line in m_xmp_data)
            {
                writer.WriteLine(line.Value);
            }
            writer.Close();
        }



        public string nodename_focallength = "";
        public string nodename_ppx = "";
        public string nodename_ppy = "";
        public string nodename_rotation = "x:xmpmeta/rdf:RDF/rdf:Description/xcr:Rotation/";
        public string nodename_position = "x:xmpmeta/rdf:RDF/rdf:Description/xcr:Position";
        public string nodename_distortion = "x:xmpmeta/rdf:RDF/rdf:Description/xcr:DistortionCoeficients";

        XmlDocument document = new XmlDocument();
        public void LoadXML(string filename)
        {
            XmlReader reader = XmlReader.Create(filename);
            //XmlNamespaceManager namespaces = new XmlNamespaceManager(document.NameTable);
            //namespaces.AddNamespace("x", "urn:hl7-org:v3");
            document.Load(reader);
            CheckPositionNode();
            reader.Close();
            //XmlNode xmlNode = document.ChildNodes[0].ChildNodes[0].ChildNodes[0];
            //XmlNode xmlNode = document.SelectSingleNode(nodename_rotation);
            //XmlNode xmlNode2 = document.SelectSingleNode(nodename_position);
            //XmlNode xmlNode3 = document.SelectSingleNode(nodename_distortion);
            //XmlNode xmlNode = document.SelectSingleNode("x:xmpmeta/rdf:RDF/rdf:Description");
            //XmlNodeList xmlNodeList = document.SelectNodes("/x:xmpmeta/rdf:RDF/rdf:Description");
            //foreach (XmlNode node in xmlNodeList)
            //{
            //    XmlAttribute att = node.Attributes[0];
            //}
            return;
        }
        public void AddNode(string nodeName)
        {
            XmlNode xmlNode = document.ChildNodes[0].ChildNodes[0].ChildNodes[0];
            XmlNode newNode = document.CreateElement(nodeName, "http://www.capturingreality.com/ns/xcr/1.1#");
            newNode.InnerText = "";
            xmlNode.AppendChild(newNode);
            return;
        }
        public void CheckPositionNode()
        {
            XmlNode xmlNode = document.ChildNodes[0].ChildNodes[0].ChildNodes[0];
            XmlNodeList childNodes = xmlNode.ChildNodes;
            foreach (XmlNode node in childNodes)
            {
                if (node.Name == "xcr:Position")
                {
                    return;
                }
            }
            string dataPos = "";
            XmlAttributeCollection attCollection = xmlNode.Attributes;
            foreach(XmlAttribute att in attCollection)
            {
                if (att.Name == "xcr:Position")
                    dataPos = att.InnerText;
            }
            if (dataPos.Length < 2)
                return;
            XmlNode newNode = document.CreateElement("xcr:Position", "http://www.capturingreality.com/ns/xcr/1.1#");
            newNode.InnerText = dataPos;
            xmlNode.AppendChild(newNode);

            RemoveAttribute("xcr:Position");
            return;
        }
        public void SetAttribute(string attName,string v)
        {
            XmlNode xmlNode = document.ChildNodes[0].ChildNodes[0].ChildNodes[0];
            XmlAttributeCollection attCollection = xmlNode.Attributes;
            int countAtt = attCollection.Count;
            for (int a = 0; a < countAtt; a++)
            {
                if (attCollection[a].Name == attName)
                {
                    attCollection[a].Value = v;
                    return;
                }
            }
            XmlAttribute newAtt = document.CreateAttribute(attName, "http://www.capturingreality.com/ns/xcr/1.1#");
            newAtt.InnerText = v;
            xmlNode.Attributes.Append(newAtt);
        }
        public void RemoveAttribute(string attName)
        {
            XmlNode xmlNode = document.ChildNodes[0].ChildNodes[0].ChildNodes[0];
            XmlAttributeCollection attCollection = xmlNode.Attributes;
            int countAtt = attCollection.Count;
            for (int a = 0; a < countAtt; a++)
            {
                if (attCollection[a].Name == attName)
                {
                    attCollection.Remove(attCollection[a]);
                    return;
                }
            }
            return;
        }
        public void RemoveAttribute(int nGroup)
        {
            if (nGroup == 1)
            {
                RemoveAttribute("xcr:FocalLength35mm");
                RemoveAttribute("xcr:PrincipalPointU");
                RemoveAttribute("xcr:PrincipalPointV");
                RemoveAttribute("xcr:Skew");
                RemoveAttribute("xcr:AspectRatio");


                RemoveAttribute("xcr:ComponentId");
                RemoveAttribute("xcr:DistortionModel");
                RemoveAttribute("xcr:InColoring");
                RemoveAttribute("xcr:InMeshing");
                RemoveAttribute("xcr:InTexturing");
                
                RemoveAttribute("xcr:latitude");
                RemoveAttribute("xcr:longitude");
                RemoveAttribute("xcr:altitude");

                RemoveAttribute("xcr:PosePrior");
                RemoveAttribute("xcr:CalibrationPrior");
                RemoveAttribute("xcr:CalibrationGroup");
                RemoveAttribute("xcr:DistortionGroup");
                
            }
            if (nGroup == 2)
            {

            }
            if (nGroup == 2)
            {

            }
        }
        public void RemoveNode(int type)
        {
            string nodeName = " ";
            if(type == 100)
            {
                nodeName = "xcr:DistortionCoeficients";
            }
            if (type == 101)
            {
                nodeName = "xcr:Position";
            }
            if (type == 102)
            {
                nodeName = "xcr:Rotation";
            }
            XmlNode xmlNode = document.ChildNodes[0].ChildNodes[0].ChildNodes[0];
            XmlNodeList childNodes = xmlNode.ChildNodes;
            foreach (XmlNode node in childNodes)
            {
                if (node.Name == nodeName)
                {
                    xmlNode.RemoveChild(node);
                    return;
                }
            }
        }
        public void SetDistortion(double p1, double p2, double p3,double k1,double k2)
        {
            XmlNode xmlNode = document.ChildNodes[0].ChildNodes[0].ChildNodes[0];
            XmlNodeList childNodes = xmlNode.ChildNodes;
            foreach(XmlNode node in childNodes)
            {
                if(node.Name == "xcr:DistortionCoeficients")
                {
                    node.InnerText = string.Format("{0} {1} {2} {3} {4}", p1, p2, p3, k1, k2);
                }                
            }
        }
        public void SetPosition(string posStr)
        {
            if(!IsPositionData())
            {
                AddNode("xcr:Position");
            }
            XmlNode xmlNode = document.ChildNodes[0].ChildNodes[0].ChildNodes[0];
            XmlNodeList childNodes = xmlNode.ChildNodes;
            foreach (XmlNode node in childNodes)
            {
                if (node.Name == "xcr:Position")
                {
                    node.InnerText = posStr;
                }
            }
        }
        public void SetPosition(double x, double y, double z)
        {
            string posStr = string.Format("{0} {1} {2}", x, y, z);
            SetPosition(posStr);
        }
        public string GetPosition()
        {
            XmlNode xmlNode = document.ChildNodes[0].ChildNodes[0].ChildNodes[0];
            XmlNodeList childNodes = xmlNode.ChildNodes;
            foreach (XmlNode node in childNodes)
            {
                if (node.Name == "xcr:Position")
                {
                    return node.InnerText;
                }
            }
            return "";
        }
        public void SetPosePrior(string v)
        {
            SetAttribute("xcr:PosePrior", v);            
        }
        public void SetGroup()
        {
            XmlNode xmlNode = document.ChildNodes[0].ChildNodes[0].ChildNodes[0];
            XmlAttributeCollection attCollection = xmlNode.Attributes;
            SetAttribute("xcr:CalibrationGroup", "0");
            SetAttribute("xcr:DistortionGroup", "0");
            SetAttribute("xcr:PosePrior", "initial");
            SetAttribute("xcr:CalibrationPrior", "Unknown");            
        }

        public void SetGPSData(double latitude, double longitude, double altitude)
        {
            XmlNode xmlNode = document.ChildNodes[0].ChildNodes[0].ChildNodes[0];
            XmlAttributeCollection attCollection = xmlNode.Attributes;
            string lat = Convert.ToString(Math.Abs(latitude));
            if (latitude < 0)
                lat += "S";
            else
                lat += "N";
            string lon = Convert.ToString(Math.Abs(longitude));
            if (longitude < 0)
                lon += "W";
            else
                lon += "E";
            SetAttribute("xcr:latitude", lat);
            SetAttribute("xcr:longitude", lon);
            SetAttribute("xcr:altitude", Rational.DecimalToFraction(altitude));
            return;
        }
        public void SetParameters(double focalLen,double ppx,double ppy)
        {
            XmlNode xmlNode = document.ChildNodes[0].ChildNodes[0].ChildNodes[0];
            XmlAttributeCollection attCollection = xmlNode.Attributes;
            SetAttribute("xcr:FocalLength35mm", Convert.ToString(focalLen));
            SetAttribute("xcr:PrincipalPointU", Convert.ToString(ppx));
            SetAttribute("xcr:PrincipalPointV", Convert.ToString(ppy));
            return;
        }
        public string GetRotation()
        {
            XmlNode xmlNode = document.ChildNodes[0].ChildNodes[0].ChildNodes[0];
            XmlNodeList childNodes = xmlNode.ChildNodes;
            foreach (XmlNode node in childNodes)
            {
                if (node.Name == "xcr:Rotation")
                {
                    return node.InnerText;
                }
            }
            return "";
        }
        public void SetRotation(double[] mt)
        {
            XmlNode xmlNode = document.ChildNodes[0].ChildNodes[0].ChildNodes[0];
            XmlNodeList childNodes = xmlNode.ChildNodes;
            foreach (XmlNode node in childNodes)
            {
                if (node.Name == "xcr:Rotation")
                {
                    string innerText = string.Format("{0} {1} {2} {3} {4} {5}", mt[0], mt[1], mt[2], mt[3], mt[4], mt[5]);
                    node.InnerText = string.Format("{0} {1} {2} {3}", innerText,mt[6], mt[7], mt[8]);
                }
            }
        }
        public bool IsPositionData()
        {
            XmlNode xmlNode = document.ChildNodes[0].ChildNodes[0].ChildNodes[0];
            XmlNodeList childNodes = xmlNode.ChildNodes;
            foreach (XmlNode node in childNodes)
            {
                if (node.Name == "xcr:Position")
                {
                    return true;
                }
            }
            return false;
        }
        public bool IsHaveNode(string nodeName)
        {
            XmlNode xmlNode = document.ChildNodes[0].ChildNodes[0].ChildNodes[0];
            XmlNodeList childNodes = xmlNode.ChildNodes;
            foreach (XmlNode node in childNodes)
            {
                if (node.Name == nodeName)
                {
                    return true;
                }
            }
            return false;
        }
        public bool isHaveAttribute(string attName)
        {
            XmlNode xmlNode = document.ChildNodes[0].ChildNodes[0].ChildNodes[0];
            XmlAttributeCollection attCollection = xmlNode.Attributes;
            for (int a = 0; a < attCollection.Count; a++)
            {
                if (attCollection[a].Name == attName)
                {
                    return true;
                }
            }
            return false;
        }
        public bool CheckAttribute(string attName,string v)
        {
            XmlNode xmlNode = document.ChildNodes[0].ChildNodes[0].ChildNodes[0];
            XmlAttributeCollection attCollection = xmlNode.Attributes;
            for (int a = 0; a < attCollection.Count; a++)
            {
                if (attCollection[a].Name == attName)
                {
                    if (attCollection[a].InnerText == v)
                        return true;
                }
            }
            return false;
        }

        public bool IsLockedData()
        {
            bool isPos = false;
            bool isRot = false;
            bool isDistorted = false;
            bool isLockedPos = false;
            XmlNode xmlNode = document.ChildNodes[0].ChildNodes[0].ChildNodes[0];
            XmlNodeList childNodes = xmlNode.ChildNodes;
            foreach (XmlNode node in childNodes)
            {
                if (node.Name == "xcr:Position")
                {
                    isPos = true;
                }
                if (node.Name == "xcr:Rotation")
                {
                    isRot = true;
                }
                if (node.Name == "xcr:DistortionCoeficients")
                {
                    isDistorted = true;
                }
            }
            XmlAttributeCollection attCollection = xmlNode.Attributes;
            for(int a=0;a< attCollection.Count;a++)
            {
                if(attCollection[a].Name == "xcr:PosePrior")
                {
                    if (attCollection[a].InnerText == "locked")
                        isLockedPos = true;
                }
            }
            if (isLockedPos && isPos && isRot && isDistorted)
                return true;

            return false;
        }
        public bool IsCalibrationData()
        {
            XmlNode xmlNode = document.ChildNodes[0].ChildNodes[0].ChildNodes[0];
            XmlNodeList childNodes = xmlNode.ChildNodes;
            foreach (XmlNode node in childNodes)
            {
                if (node.Name == "xcr:DistortionCoeficients")
                {
                    return true;
                }
            }
            return false;
        }
        public void SaveXML(string output_filename)
        {
            //XmlWriter writer = XmlWriter.Create("TestSave.xmp");
            document.Save(output_filename);
        }
    }
}
