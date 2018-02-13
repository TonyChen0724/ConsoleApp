using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace CoDriverConsoleApp
{
    class Virb360
    {
        public struct GPSData
        {
            public int idx;
            public Int64 timeStamp;
            public double latitude;
            public double longitude;
            public double altitude;
            public double heading;
            public double posX;
            public double posY;

        }
        string gps_csv_filename;
        public Dictionary<int, GPSData> m_gps_data = new Dictionary<int, GPSData>();

        public void LoadGPSFile_csv(string filename)
        {
            gps_csv_filename = filename;
            StreamReader reader = new StreamReader(filename);
            int count = 0;
            double latRef = 0;
            double lonRef = 0;
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                if (line.Length <= 3)
                    continue;
                if (line[0] == '#')
                    continue;
                var values = line.Split(',');
                if (values.Length < 2)
                    continue;
                
                if (count == 0)
                {
                    count++;
                    continue;
                }

                GPSData line_data = new GPSData();
                line_data.idx = count;                
                line_data.timeStamp = Convert.ToInt64(values[0]);
                line_data.latitude = Convert.ToDouble(values[1]);
                line_data.longitude = Convert.ToDouble(values[2]);
                line_data.altitude = Convert.ToDouble(values[3]);
                line_data.heading = Convert.ToDouble(values[4]);

                if(count == 1)
                {
                    line_data.posX = 0;
                    line_data.posY = 0;
                    latRef = line_data.latitude;
                    lonRef = line_data.longitude;
                }
                else
                {
                    double posX;
                    double posY;
                    posX = KMLFile.distanceEarth(latRef, lonRef, latRef, line_data.longitude);
                    if (lonRef > line_data.longitude)
                        posX *= -1.0;
                    posY = KMLFile.distanceEarth(latRef, lonRef, line_data.latitude, lonRef);
                    if (latRef > line_data.latitude)
                        posY *= -1.0;
                    // km -> m
                    line_data.posX = posX * 1000.0;
                    line_data.posY = posY * 1000.0;

                }


                m_gps_data.Add(count, line_data);
                count++;
            }
            reader.Close();
            return;
        }
        public void BuildCSVFile_Lerp(string imgFolder)
        {
            if (imgFolder.Length < 3)
            {
                Program.AddLog("BuildCSVFile_Lerp: wrong Path." + imgFolder);
                return;
            }
            if (!Directory.Exists(imgFolder))
            {
                Program.AddLog("BuildCSVFile_Lerp: No Path." + imgFolder);
                return;
            }
            if (m_gps_data.Count < 1)
                return;
            string[] files = Directory.GetFiles(imgFolder);
            string srcFilename = Path.GetFileNameWithoutExtension(gps_csv_filename);
            string destPath = Path.GetDirectoryName(gps_csv_filename);
            string destFilename = Path.Combine(destPath, srcFilename + "_lerp.csv");
            StreamWriter writer = new StreamWriter(destFilename);
            writer.WriteLine("imagename,X,Y,Z");
            GPSData firstGPSData = m_gps_data.First().Value;
            foreach (var file in files)
            {
                string ext = Path.GetExtension(file);
                ext = ext.ToLower();
                if (ext == ".jpg")
                {
                    string filename = Path.GetFileNameWithoutExtension(file);
                    string keyword = "";
                    int idx = 0;
                    bool isFilename = XMPGenerator.GetIndexAndKeyword(out keyword, out idx, filename);
                    if (!isFilename)
                        continue;
                    idx -= 1;
                    int gpsIdx = idx / 30;
                    int gpsSecStep = idx % 30;
                    gpsIdx += 1;
                    int gpsIdxNext = gpsIdx + 1;
                    if (!m_gps_data.ContainsKey(gpsIdx))
                        continue;
                    if (!m_gps_data.ContainsKey(gpsIdxNext))
                        continue;
                    GPSData gpsData = m_gps_data[gpsIdx];
                    GPSData gpsData2 = m_gps_data[gpsIdxNext];
                    double p = gpsSecStep / 30.0;
                    double posX = (gpsData2.posX - gpsData.posX) * p + gpsData.posX;
                    double posY = (gpsData2.posY - gpsData.posY) * p + gpsData.posY;
                    double altitude = (gpsData2.altitude - gpsData.altitude) * p + gpsData.altitude;

                    string line_data = string.Format("{0},{1},{2},{3}", Path.GetFileName(file),posX, posY, altitude);
                    writer.WriteLine(line_data);
                }
            }

            writer.Close();
        }
        public void BuildXMPFile_LerpGPSData(string imgFolder)
        {
            if (imgFolder.Length < 3)
            {
                Program.AddLog("BuildXMPFile_LerpGPSData: wrong Path." + imgFolder);
                return;
            }
            if (!Directory.Exists(imgFolder))
            {
                Program.AddLog("BuildXMPFile_LerpGPSData: No Path." + imgFolder);
                return;
            }
            if (m_gps_data.Count < 1)
                return;
            string[] files = Directory.GetFiles(imgFolder);
            
            GPSData firstGPSData = m_gps_data.First().Value;
            foreach (var file in files)
            {
                string ext = Path.GetExtension(file);
                ext = ext.ToLower();
                if (ext == ".jpg")
                {
                    string filename = Path.GetFileNameWithoutExtension(file);
                    string keyword = "";
                    int idx = 0;
                    bool isFilename = XMPGenerator.GetIndexAndKeyword(out keyword, out idx, filename);
                    if (!isFilename)
                        continue;
                    string xmpFileName = Path.ChangeExtension(file, "xmp");
                    if (File.Exists(xmpFileName))
                    {
                        File.Delete(xmpFileName);
                    }
                    idx -= 1;
                    int gpsIdx = idx / 30;
                    int gpsSecStep = idx % 30;
                    gpsIdx += 1;
                    int gpsIdxNext = gpsIdx + 1;
                    if (!m_gps_data.ContainsKey(gpsIdx))
                        continue;
                    if (!m_gps_data.ContainsKey(gpsIdxNext))
                        continue;
                    GPSData gpsData = m_gps_data[gpsIdx];
                    GPSData gpsData2 = m_gps_data[gpsIdxNext];
                    double p = gpsSecStep / 30.0;
                    double posX = (gpsData2.posX - gpsData.posX) * p + gpsData.posX;
                    double posY = (gpsData2.posY - gpsData.posY) * p + gpsData.posY;
                    double altitude = (gpsData2.altitude - gpsData.altitude) * p + gpsData.altitude;

                    XMPFile xmp_file = new XMPFile();
                    xmp_file.LoadXML("Sample.xmp");
                    xmp_file.RemoveNode(100);
                    xmp_file.RemoveNode(102);
                    xmp_file.RemoveAttribute(1);
                    xmp_file.SetPosition(posX, posY, altitude);
                    xmp_file.SaveXML(xmpFileName);
                }
           }
        }
    }
}
