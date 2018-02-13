using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace CoDriverConsoleApp
{
    public class GPS5DataFile
    {
        public struct MovieData
        {
            public float metadata_length;
            public int payload_max;
            public string movie_filename;
            public float gps5_rate;
            public List<PayloadData> payload_data;
        }
        public struct PayloadData
        {
            public int PayloadId;
            public int Samples;
            public int Elements;
            public float Rate;
            public float inTime;
            public float outTime;
            public List<GPS5Data> gps5_data;
        }
        public struct GPS5Data
        {
            public int frame;
            public double latitude;
            public double longitude;
            public double altitude;
            public double speed_2d;
            public double speed_3d;
            public int idxZone;
            public double posX;
            public double posY;
        }
        public struct GPS5Zone
        {
            public int idx;
            public double posX;
            public double posY;
            public double altitude;
            public int nbImg;
            public double startSpeed;
            public double endSpeed;

        }
        static public double checkDistance(GPS5Data gpsData,double destX,double destY,double destZ)
        {
            double biasX = gpsData.posX - destX;
            double biasY = gpsData.posY - destY;
            double biasZ = gpsData.altitude - destZ;
            double W = biasX * biasX + biasY * biasY + biasZ * biasZ;
            return Math.Sqrt(W);
        }
        public MovieData movie_data;
        //
        static Dictionary<int, GPS5Zone> gpsZoneList = new Dictionary<int, GPS5Zone>();
        static int lastZoneIdx = -1;
        //static GPS5Zone lastZone = new GPS5Zone();
        static public GPS5Zone GetGPSZone(int idx)
        {
            return gpsZoneList[idx];
        }
        static public int PushGPSDataToZone(GPS5Data newData)
        {
            int foundIdx = -1;
            foreach(var node in gpsZoneList)
            {
                int zoneIdx = node.Key;
                GPS5Zone zoneInfo = node.Value;
                double distance = checkDistance(newData, zoneInfo.posX, zoneInfo.posY, zoneInfo.altitude);
                if(distance < 0.01)
                {
                    foundIdx = zoneInfo.idx;   
                }
            }
            if(foundIdx >= 0)
            {
                GPS5Zone zoneInfo = gpsZoneList[foundIdx];
                zoneInfo.nbImg++;
                zoneInfo.endSpeed = newData.speed_3d;
                gpsZoneList[foundIdx] = zoneInfo;
                return foundIdx;
            }

            //new zone
            lastZoneIdx++;
            GPS5Zone newZone = new GPS5Zone();
            newZone.idx = lastZoneIdx;
            newZone.posX = newData.posX;
            newZone.posY = newData.posY;
            newZone.altitude = newData.altitude;
            newZone.nbImg = 1;
            newZone.startSpeed = newData.speed_3d;
            newZone.endSpeed = newData.speed_3d;
            gpsZoneList.Add(newZone.idx, newZone);
            return newZone.idx;
        }
        public void LoadGPS5Data(string filename)
        {
            movie_data = new MovieData();
            StreamReader reader = new StreamReader(filename);
            int count = 0;
            int payload_idx = 0;
            int sample_idx = 0;
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
                if (values[0].Contains("MetadataLength"))
                {
                    double d = Convert.ToDouble(values[1]);
                    int n = Convert.ToInt32(values[3]);
                    string movieFile = values[5];
                    movie_data.payload_max = n;
                    movie_data.payload_data = new List<PayloadData>(n);
                    movie_data.movie_filename = movieFile;
                    movie_data.metadata_length = (float)d;
                    continue;
                }
                if (values[0].Contains("PayloadInTime"))
                {
                    payload_idx++;

                    PayloadData payload_data = new PayloadData();
                    payload_data.PayloadId = payload_idx - 1;
                    double d = Convert.ToDouble(values[1]);
                    payload_data.inTime = (float)d;
                    d = Convert.ToDouble(values[3]);
                    payload_data.outTime = (float)d;
                    movie_data.payload_data.Add(payload_data);
                    sample_idx = 0;
                    continue;
                }
                if (values[0].Contains("GPS5"))
                {
                    if (values[1].Contains("In"))
                    {
                        PayloadData payload_data = movie_data.payload_data[payload_idx - 1];
                        double d = Convert.ToDouble(values[2]);
                        //payload_data.inTime = (float)d;
                        //d = Convert.ToDouble(values[4]);
                        //payload_data.outTime = (float)d;
                        d = Convert.ToDouble(values[6]);
                        payload_data.Rate = (float)d;
                        movie_data.payload_data[payload_idx - 1] = payload_data;
                    }
                    if (values[1].Contains("Samples"))
                    {
                        PayloadData payload_data = movie_data.payload_data[payload_idx - 1];
                        int n = Convert.ToInt32(values[2]);
                        payload_data.Samples = n;
                        payload_data.gps5_data = new List<GPS5Data>(n);
                        sample_idx = 0;
                        movie_data.payload_data[payload_idx - 1] = payload_data;
                    }
                    if (values[1].Contains("Elements"))
                    {
                        PayloadData payload_data = movie_data.payload_data[payload_idx - 1];
                        int n = Convert.ToInt32(values[2]);
                        payload_data.Elements = n;
                        movie_data.payload_data[payload_idx - 1] = payload_data;
                    }
                    if (values[1].Contains("Rate"))
                    {
                        double d = Convert.ToDouble(values[2]);
                        movie_data.gps5_rate = (float)d;
                    }
                    continue;
                }
                if (values[0].Contains("GPSElement"))
                {
                    if (values[0].Contains("GPSElement1"))
                    {
                        PayloadData payload_data = movie_data.payload_data[payload_idx - 1];
                        int baseIdx = payload_idx - 1;
                        baseIdx *= payload_data.Samples;
                        baseIdx += sample_idx;
                        GPS5Data gps5Data = new GPS5Data();
                        gps5Data.frame = baseIdx;
                        double d = Convert.ToDouble(values[1]);
                        gps5Data.latitude = (float)d;
                        payload_data.gps5_data.Add(gps5Data);
                        movie_data.payload_data[payload_idx - 1] = payload_data;
                        sample_idx++;
                    }
                    if (values[0].Contains("GPSElement2"))
                    {
                        double d = Convert.ToDouble(values[1]);
                        PayloadData payload_data = movie_data.payload_data[payload_idx - 1];
                        GPS5Data gps5Data = payload_data.gps5_data[sample_idx - 1];
                        gps5Data.longitude = (float)d;
                        payload_data.gps5_data[sample_idx - 1] = gps5Data;
                        movie_data.payload_data[payload_idx - 1] = payload_data;
                    }
                    if (values[0].Contains("GPSElement3"))
                    {
                        double d = Convert.ToDouble(values[1]);
                        PayloadData payload_data = movie_data.payload_data[payload_idx - 1];
                        GPS5Data gps5Data = payload_data.gps5_data[sample_idx - 1];
                        gps5Data.altitude = (float)d;
                        payload_data.gps5_data[sample_idx - 1] = gps5Data;
                        movie_data.payload_data[payload_idx - 1] = payload_data;
                    }
                    if (values[0].Contains("GPSElement4"))
                    {
                        double d = Convert.ToDouble(values[1]);
                        PayloadData payload_data = movie_data.payload_data[payload_idx - 1];
                        GPS5Data gps5Data = payload_data.gps5_data[sample_idx - 1];
                        gps5Data.speed_2d = (float)d;
                        payload_data.gps5_data[sample_idx - 1] = gps5Data;
                        movie_data.payload_data[payload_idx - 1] = payload_data;
                    }
                    if (values[0].Contains("GPSElement5"))
                    {
                        double d = Convert.ToDouble(values[1]);
                        PayloadData payload_data = movie_data.payload_data[payload_idx - 1];
                        GPS5Data gps5Data = payload_data.gps5_data[sample_idx - 1];
                        gps5Data.speed_3d = (float)d;
                        payload_data.gps5_data[sample_idx - 1] = gps5Data;
                        movie_data.payload_data[payload_idx - 1] = payload_data;
                    }
                    continue;
                }


                //m_csv_data.Add(count, line_data);
                //count++;
            }
            //number = m_csv_data.Count - 1;
            reader.Close();
            return;
        }
    }

}
