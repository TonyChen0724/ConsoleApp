using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;

namespace CoDriverConsoleApp
{
    class ExifDataWriter
    {
        Dictionary<string, GPS5DataFile.GPS5Data> m_images_gps5_list = new Dictionary<string, GPS5DataFile.GPS5Data>();

        Dictionary<int, string> m_images_list = new Dictionary<int, string>();
        GPS5DataFile gps_data_file = new GPS5DataFile();
        int max_count_image_idx = 0;
        float focal_length;
        bool is_focal_len = false;
        string videoName = "";
        string kmlFilename = "";
        public void SetFocalLen(float f)
        {
            focal_length = f;
            is_focal_len = true;
            return;
        }
        public void SetFolder(string folder)
        {
            folder += "\\";
            char[] charSeparators = new char[] { '_', '(',')', ' ' };
            string[] fileEntries = Directory.GetFiles(folder);
            string folderName = Path.GetFileName(Path.GetDirectoryName(folder));
            videoName = folderName;
            kmlFilename = Path.Combine(folder, videoName + ".kml");
            string gps5file = folderName + "_gps5";
            foreach (string fileName in fileEntries)
            {
                string ext = Path.GetExtension(fileName);
                if (ext == ".jpg")
                {
                    string file = Path.GetFileNameWithoutExtension(fileName);
                    var values = file.Split(charSeparators, StringSplitOptions.RemoveEmptyEntries);
                    if (values.Length < 2)
                        continue;
                    int idx = Convert.ToInt32(values[1]);
                    if (max_count_image_idx < idx) 
                        max_count_image_idx = idx;
                    m_images_list[idx] = fileName;
                }
                if (fileName.Contains(gps5file))
                {
                    gps_data_file.LoadGPS5Data(fileName);
                }
            }
            return;
        }

        public void Process()
        {
            //if (movie_data.payload_data.Count < 1)
            //    return;
            //float baseIn = movie_data.payload_data[0].inTime;
            //float baseOut = movie_data.payload_data[0].outTime;
            int total_count_image = max_count_image_idx;// m_images_list.Count;
            float total_time = gps_data_file.movie_data.metadata_length;
            float step_time = total_time / total_count_image;

            foreach (var node in m_images_list)
            {
                int imageIdx = node.Key - 1;
                string imageName = node.Value;
                if (!File.Exists(imageName))
                    continue;
                float imageTM = imageIdx * step_time;
                foreach (var payload in gps_data_file.movie_data.payload_data)
                {
                    if (imageTM < payload.inTime  || imageTM > payload.outTime )
                    {
                        continue;
                    }
                    float step_sample_len = (payload.outTime - payload.inTime) / payload.Samples;
                    int idxSample = (int)((imageTM - payload.inTime) / step_sample_len);
                    if (idxSample < 0 || idxSample >= payload.Samples)
                        continue;
                    GPS5DataFile.GPS5Data gpsData = payload.gps5_data[idxSample];
                    m_images_gps5_list[imageName] = gpsData;
                    break;
                }
            }

            return;
        }

        

        //double GD_semiMajorAxis = 6378137.000000;
        //double GD_TranMercB = 6356752.314245;
        //double GD_geocentF = 0.003352810664;
        //void geodeticOffsetInv(double refLat, double refLon,
        //                        double lat, double lon,
        //                       out double xOffset, out double yOffset )
        //{
        //    double a = GD_semiMajorAxis;
        //    double b = GD_TranMercB;
        //    double f = GD_geocentF;

        //    double L = lon - refLon;
        //    double U1 = Math.Atan((1 - f) * Math.Tan(refLat));
        //    double U2 = Math.Atan((1 - f) * Math.Tan(lat));
        //    double sinU1 = Math.Sin(U1);
        //    double cosU1 = Math.Cos(U1);
        //    double sinU2 = Math.Sin(U2);
        //    double cosU2 = Math.Cos(U2);

        //    double lambda = L;
        //    double lambdaP;
        //    double sinSigma;
        //    double sigma;
        //    double cosSigma;
        //    double cosSqAlpha;
        //    double cos2SigmaM;
        //    double sinLambda;
        //    double cosLambda;
        //    double sinAlpha;
        //    int iterLimit = 100;
        //    do
        //    {
        //        sinLambda = Math.Sin(lambda);
        //        cosLambda = Math.Cos(lambda);
        //        sinSigma = Math.Sqrt((cosU2 * sinLambda) * (cosU2 * sinLambda) +
        //                        (cosU1 * sinU2 - sinU1 * cosU2 * cosLambda) *
        //                        (cosU1 * sinU2 - sinU1 * cosU2 * cosLambda));
        //        if (sinSigma == 0)
        //        {
        //            xOffset = 0.0;
        //            yOffset = 0.0;
        //            return;  // co-incident points
        //        }
        //        cosSigma = sinU1 * sinU2 + cosU1 * cosU2 * cosLambda;
        //        sigma = Math.Atan2(sinSigma, cosSigma);
        //        sinAlpha = cosU1 * cosU2 * sinLambda / sinSigma;
        //        cosSqAlpha = 1 - sinAlpha * sinAlpha;
        //        if(cosSqAlpha == 0)
        //            cos2SigmaM = 0;
        //        else
        //            cos2SigmaM = cosSigma - 2 * sinU1 * sinU2 / cosSqAlpha;
                
        //        double C = f / 16 * cosSqAlpha * (4 + f * (4 - 3 * cosSqAlpha));
        //        lambdaP = lambda;
        //        lambda = L + (1 - C) * f * sinAlpha * (sigma + C * sinSigma * (cos2SigmaM + C * cosSigma * (-1 + 2 * cos2SigmaM * cos2SigmaM)));

        //    } while (Math.Abs(lambda - lambdaP) > 1e-12 && --iterLimit > 0);

        //    if (iterLimit == 0)
        //    {
        //        xOffset = 0.0;
        //        yOffset = 0.0;
        //        return;  // formula failed to converge
        //    }

        //    double uSq = cosSqAlpha * (a * a - b * b) / (b * b);
        //    double A = 1 + uSq / 16384 * (4096 + uSq * (-768 + uSq * (320 - 175 * uSq)));
        //    double B = uSq / 1024 * (256 + uSq * (-128 + uSq * (74 - 47 * uSq)));
        //    double deltaSigma = B * sinSigma * (cos2SigmaM + B / 4 * (cosSigma * (-1 + 2 * cos2SigmaM * cos2SigmaM) -
        //        B / 6 * cos2SigmaM * (-3 + 4 * sinSigma * sinSigma) * (-3 + 4 * cos2SigmaM * cos2SigmaM)));
        //    double s = b * A * (sigma - deltaSigma);

        //    double bearing = Math.Atan2(cosU2 * sinLambda, cosU1 * sinU2 - sinU1 * cosU2 * cosLambda);
        //    xOffset = Math.Sin(bearing) * s;
        //    yOffset = Math.Cos(bearing) * s;       

        //    return;
        //}

        public void BuildKMLFile()
        {
            KMLFile kmlFile = new KMLFile();
            kmlFile.LoadXML("SamplePath.kml");
            kmlFile.SetName(videoName);
            string coordinatesStr = "";
            bool isFirst = true;
            int count = 0;
            int gpsCount = 0;
            foreach (var node in m_images_gps5_list)
            {
                gpsCount++;
                GPS5DataFile.GPS5Data gpsData = node.Value;
                if (isFirst)
                {
                    coordinatesStr += Convert.ToString(gpsData.longitude);
                    coordinatesStr += ",";
                    coordinatesStr += Convert.ToString(gpsData.latitude);
                    coordinatesStr += ",";
                    coordinatesStr += Convert.ToString(gpsData.altitude);
                    coordinatesStr += "\n ";
                    isFirst = false;
                    count++;
                    continue;
                }
                if (gpsData.speed_2d < 1.0)
                    continue;
                if (gpsCount % 30 == 0)
                {
                    coordinatesStr += Convert.ToString(gpsData.longitude);
                    coordinatesStr += ",";
                    coordinatesStr += Convert.ToString(gpsData.latitude);
                    coordinatesStr += ",";
                    coordinatesStr += Convert.ToString(gpsData.altitude);
                    coordinatesStr += "\n ";
                    count++;
                    continue;
                }
                if(gpsData.frame == max_count_image_idx)
                {
                    coordinatesStr += Convert.ToString(gpsData.longitude);
                    coordinatesStr += ",";
                    coordinatesStr += Convert.ToString(gpsData.latitude);
                    coordinatesStr += ",";
                    coordinatesStr += Convert.ToString(gpsData.altitude);
                    coordinatesStr += "\n ";
                    count++;
                    continue;
                }
                //string filename = node.Key;
            }

            kmlFile.SetCoordinates(coordinatesStr);
            kmlFile.SaveXML(kmlFilename);
        }
        public void BuildXMPFiles()
        {
            if (m_images_gps5_list.Count < 1)
                return;

            BuildKMLFile();
            var nodeFirst = m_images_gps5_list.First();
            GPS5DataFile.GPS5Data gpsDataFirst = nodeFirst.Value;
            double latRef = gpsDataFirst.latitude;
            double lonRef = gpsDataFirst.longitude;
            foreach (var node in m_images_gps5_list)
            {
                GPS5DataFile.GPS5Data gpsData = node.Value;
                string filename = node.Key;
                string path = Path.GetDirectoryName(filename);
                string output_filename = Path.GetFileNameWithoutExtension(filename);
                output_filename += ".xmp";
                output_filename = Path.Combine(path,output_filename);

                XMPFile xmp_file = new XMPFile();
                xmp_file.LoadXML("Sample.xmp");
                xmp_file.RemoveNode(100);
                xmp_file.RemoveNode(102);
                xmp_file.RemoveAttribute(1);
               // xmp_file.SetGroup();
              //  xmp_file.SetGPSData(gpsData.latitude, gpsData.longitude, gpsData.altitude);
                double posX;
                double posY;
                //geodeticOffsetInv(latRef, lonRef, gpsData.latitude, gpsData.longitude,out posX,out posY);

                posX = KMLFile.distanceEarth(latRef, lonRef, latRef, gpsData.longitude);
                if (lonRef > gpsData.longitude)
                    posX *= -1.0;
                posY = KMLFile.distanceEarth(latRef, lonRef, gpsData.latitude, lonRef);
                if (latRef > gpsData.latitude)
                    posY *= -1.0;

                posX *= 1000.0;
                posY *= 1000.0;
                xmp_file.SetPosition(posX, posY, gpsData.altitude);
                xmp_file.SaveXML(output_filename);
            }
            return;
        }

        public void RunExifCommand()
        {
            Process cmd = new Process();
            cmd.StartInfo.FileName = Directory.GetCurrentDirectory() + "\\exiftool.exe";

            cmd.StartInfo.RedirectStandardInput = true;
            cmd.StartInfo.RedirectStandardOutput = true;
            cmd.StartInfo.CreateNoWindow = true;
            cmd.StartInfo.UseShellExecute = false;
   
            foreach (var node in m_images_gps5_list)
            {
                GPS5DataFile.GPS5Data gpsData = node.Value;
                string filename = node.Key;
                string arg;
                string latRef = "N";
                string longRef = "E";
                //gpsData.longitude *= -1.0;
                if (gpsData.latitude < 0)
                    latRef = "S";
                if (gpsData.longitude < 0)
                    longRef = "W";
                if (is_focal_len)
                {
                    arg = string.Format("-GPSLatitude={0} -GPSLongitude={1} -GPSAltitude={2} -GPSLatitudeRef={3} -GPSLongitudeRef={4} -FocalLength={5} \"{6}\"", gpsData.latitude, gpsData.longitude, gpsData.altitude, latRef,longRef,focal_length, filename);
                }
                else
                {
                    arg = string.Format("-GPSLatitude={0} -GPSLongitude={1} -GPSAltitude={2} -GPSLatitudeRef={3} -GPSLongitudeRef={4} \"{5}\"", gpsData.latitude, gpsData.longitude, gpsData.altitude, latRef, longRef, filename);
                }
                
                cmd.StartInfo.Arguments = arg;
                cmd.Start();
                cmd.StandardInput.Flush();
                cmd.StandardInput.Close();
                cmd.WaitForExit();
                Program.AddLog("exiftool " + arg);
                Program.AddLog(cmd.StandardOutput.ReadToEnd());
            }
        }

        public void CleanCache(string folder)
        {
            folder += "\\";
            string[] fileEntries = Directory.GetFiles(folder);
            foreach(string fileName in fileEntries)
            {
                string ext = Path.GetExtension(fileName);
                if (ext.Contains("_original"))
                {
                    File.Delete(fileName);
                }
            }
        }

        public void LoadDataFile(string filename)
        {
            StreamReader reader = new StreamReader(filename);
            int count = 0;
            char[] charSeparators = new char[] { '=', ',', ' ' };
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                count++;
                if (line.Length <= 4)
                    continue;
                if (line[0] == '#')
                    continue;
                var values = line.Split(charSeparators, StringSplitOptions.RemoveEmptyEntries);
                if (values.Length < 2)
                    continue;
                if (values[0].Contains("MaxIndex"))
                {
                    max_count_image_idx = Convert.ToInt32(values[1]);
                    continue;
                }
                if (values[0].Contains("FocalLen"))
                {
                    focal_length = (float)Convert.ToDouble(values[1]);
                    continue;
                }
                if (values[0].Contains("F"))
                {
                    string file = values[1];
                    GPS5DataFile.GPS5Data gpsData = new GPS5DataFile.GPS5Data();
                    gpsData.latitude = Convert.ToDouble(values[2]);
                    gpsData.longitude = Convert.ToDouble(values[3]);
                    gpsData.altitude = Convert.ToDouble(values[4]);
                    gpsData.speed_3d = Convert.ToDouble(values[5]);
                    m_images_gps5_list[file] = gpsData;
                    continue;
                }
            }
            reader.Close();
            return;
        }
        public void SaveDataFile(string filename)
        {
            StreamWriter writer = new StreamWriter(filename);
            writer.WriteLine("MaxIndex=" + max_count_image_idx);
            writer.WriteLine("FocalLen=" + focal_length);

            foreach (var node in m_images_gps5_list)
            {
                GPS5DataFile.GPS5Data gpsData = node.Value;
                string file = node.Key;
                string line = "F=" + file + "," + gpsData.latitude + "," + gpsData.longitude + "," + gpsData.altitude + "," + gpsData.speed_3d;
                writer.WriteLine(line);
            }
            writer.Close();
        }

       
    }
}
