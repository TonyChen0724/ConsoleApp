using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace CoDriverConsoleApp
{
    public class ImagesInfo
    {
        public string videoName;
        public string kmlFilename;
        public string keyword;
        public string gps5filename;
        public int min_count = 0;
        public int max_count = 0;
        public Dictionary<int, string> m_images_list = new Dictionary<int, string>();
        public Dictionary<string, GPS5DataFile.GPS5Data> m_images_gps5_list = new Dictionary<string, GPS5DataFile.GPS5Data>();
        public GPS5DataFile gps_data_file = new GPS5DataFile();
        public ImagesInfo()
        {
        }
    }

    class ImagesManager
    {
        public string sceneName = "GoogleEarth";
        public string destFolder;
        Dictionary<int, string> m_total_images = new Dictionary<int, string>();
        Dictionary<string, string> m_total_new_image_name = new Dictionary<string, string>();
        Dictionary<string, GPS5DataFile.GPS5Data> m_total_gps_data_list = new Dictionary<string, GPS5DataFile.GPS5Data>();

        public Dictionary<int,ImagesInfo> folderList = new Dictionary<int,ImagesInfo>();
        public void SelectFolder(int idxFolder,string folder)
        {
            ImagesInfo imgInfo = new ImagesInfo();
            
            folder += "\\";
            char[] charSeparators = new char[] { '_', '(', ')', ' ' };
            string[] fileEntries = Directory.GetFiles(folder);
            string folderName = Path.GetFileName(Path.GetDirectoryName(folder));
            imgInfo.videoName = folderName;
            imgInfo.kmlFilename = Path.Combine(folder, imgInfo.videoName + ".kml");
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
                    imgInfo.keyword = values[0];
                    int idx = Convert.ToInt32(values[1]);
                    if (imgInfo.max_count < idx)
                        imgInfo.max_count = idx;
                    imgInfo.m_images_list[idx] = fileName;
                }
                if (fileName.Contains(gps5file))
                {
                    imgInfo.gps_data_file.LoadGPS5Data(fileName);
                }
            }
            folderList.Add(idxFolder, imgInfo);
            Program.AddLog("Select folder: " + folder + " " + idxFolder.ToString() + " " + imgInfo.max_count);
            return;
        }

        public void Process()
        {
            foreach (var imgFolder in folderList)
            {
                ImagesInfo imgInf = imgFolder.Value;
                int total_count_image = imgInf.max_count;

                float total_time = imgInf.gps_data_file.movie_data.metadata_length;
                float step_time = total_time / total_count_image;

                foreach (var node in imgInf.m_images_list)
                {
                    int imageIdx = node.Key - 1;
                    string imageName = node.Value;
                    if (!File.Exists(imageName))
                        continue;
                    float imageTM = imageIdx * step_time;
                    foreach (var payload in imgInf.gps_data_file.movie_data.payload_data)
                    {
                        if (imageTM < payload.inTime || imageTM > payload.outTime)
                        {
                            continue;
                        }
                        float step_sample_len = (payload.outTime - payload.inTime) / payload.Samples;
                        int idxSample = (int)((imageTM - payload.inTime) / step_sample_len);
                        if (idxSample < 0 || idxSample >= payload.Samples)
                            continue;
                        GPS5DataFile.GPS5Data gpsData = payload.gps5_data[idxSample];
                        imgInf.m_images_gps5_list[imageName] = gpsData;
                        break;
                    }
                }
            }
 

            int nBaseIndex = 0;
            bool zeroSpdCheck = false;
            int nbFolder = folderList.Count;
            for(int a=0; a < nbFolder; a++)
            {
                int idx = a + 1;
                if(!folderList.ContainsKey(idx))
                {
                    continue;
                }
                ImagesInfo imgInf = folderList[idx];
                int total_count_image = imgInf.max_count;

                foreach (var node in imgInf.m_images_list)
                {
                    int imageIdx = node.Key;
                    string imageName = node.Value;
                    if (!imgInf.m_images_gps5_list.ContainsKey(imageName))
                        continue;
                    GPS5DataFile.GPS5Data gpsData = imgInf.m_images_gps5_list[imageName];
                    if (gpsData.speed_2d < 0.1)
                    {
                        if (zeroSpdCheck)
                        {
                            continue;
                        }
                        zeroSpdCheck = true;
                    }
                    else
                        zeroSpdCheck = false;
                    int newIdx = imageIdx + nBaseIndex;
                    m_total_images.Add(newIdx, imageName);
                    m_total_gps_data_list.Add(imageName, gpsData);
                    
                    string newFilename = string.Format("{0}_{1}.jpg",imgInf.keyword, newIdx.ToString("D6"));
                    m_total_new_image_name.Add(imageName, newFilename);

                }
                nBaseIndex += imgInf.max_count;
                zeroSpdCheck = false;
            }

            return;
        }
        public void BuildGPSZones()
        {
            var gpsFirstNode = m_total_gps_data_list.First();
            GPS5DataFile.GPS5Data gpsDataFirst = gpsFirstNode.Value;
            double latRef = gpsDataFirst.latitude;
            double lonRef = gpsDataFirst.longitude;

            //m_total_images.OrderBy();
            foreach (var node in m_total_images)
            {
                int idx = node.Key;
                string imageName = node.Value;
                //Caculate XYZ;
                GPS5DataFile.GPS5Data gpsData = m_total_gps_data_list[imageName];
                double posX;
                double posY;
                posX = KMLFile.distanceEarth(latRef, lonRef, latRef, gpsData.longitude);
                if (lonRef > gpsData.longitude)
                    posX *= -1.0;
                posY = KMLFile.distanceEarth(latRef, lonRef, gpsData.latitude, lonRef);
                if (latRef > gpsData.latitude)
                    posY *= -1.0;
                // km -> m
                posX *= 1000.0;
                posY *= 1000.0;
                gpsData.posX = posX;
                gpsData.posY = posY;
                gpsData.idxZone = GPS5DataFile.PushGPSDataToZone(gpsData);
                m_total_gps_data_list[imageName] = gpsData;
            }
        }

        public void UpdateGPSPos()
        {
            var gpsFirstNode = m_total_gps_data_list.First();
            GPS5DataFile.GPS5Data gpsDataFirst = gpsFirstNode.Value;
            double posX = gpsDataFirst.posX;
            double posY = gpsDataFirst.posY;
            double posZ = gpsDataFirst.altitude;

            int countInZone = 0;
            int lastZoneIdx = -1;
            foreach (var node in m_total_images)
            {
                int idx = node.Key;
                string imageName = node.Value;
                GPS5DataFile.GPS5Data gpsData = m_total_gps_data_list[imageName];
                if(gpsData.idxZone < 0)
                    continue;
                GPS5DataFile.GPS5Zone gpsZone = GPS5DataFile.GetGPSZone(gpsData.idxZone);



                countInZone++;
            }
        }
        public void BuildTotalImagesFolder(double gapLimit)
        {
            if (!Directory.Exists(destFolder))
            {
                Directory.CreateDirectory(destFolder);
            }
            //string removeImgFolder = Path.Combine(destFolder, "Remove");
            //if (!Directory.Exists(removeImgFolder))
            //{
            //    Directory.CreateDirectory(removeImgFolder);
            //}
            var gpsFirstNode = m_total_gps_data_list.First();
            GPS5DataFile.GPS5Data gpsDataFirst = gpsFirstNode.Value;
            double latRef = gpsDataFirst.latitude;
            double lonRef = gpsDataFirst.longitude;
            double lastPosX = 0;
            double lastPosY = 0;
            double lastPosZ = 0;
            bool gapCheck = false;
            foreach (var node in m_total_images)
            {
                int idx = node.Key;
                string imageName = node.Value;
                //Caculate XYZ;
                GPS5DataFile.GPS5Data gpsData = m_total_gps_data_list[imageName];
                double posX;
                double posY;
                posX = KMLFile.distanceEarth(latRef, lonRef, latRef, gpsData.longitude);
                if (lonRef > gpsData.longitude)
                    posX *= -1.0;
                posY = KMLFile.distanceEarth(latRef, lonRef, gpsData.latitude, lonRef);
                if (latRef > gpsData.latitude)
                    posY *= -1.0;
                // km -> m
                posX *= 1000.0;
                posY *= 1000.0;

                //Check gap limit
                if (!gapCheck)
                {
                    lastPosX = posX;
                    lastPosY = posY;
                    lastPosZ = gpsData.altitude;
                    gapCheck = true;
                }
                else
                {
                    double dist = (posX - lastPosX) * (posX - lastPosX) + (posY - lastPosY) * (posY - lastPosY) + (gpsData.altitude - lastPosZ) * (gpsData.altitude - lastPosZ);
                    dist = Math.Sqrt(dist);
                    if (dist < gapLimit)
                    {
                        continue;
                    }
                    else
                    {
                        lastPosX = posX;
                        lastPosY = posY;
                        lastPosZ = gpsData.altitude;
                    }
                }
                
                //string filename = Path.GetFileName(imageName);
                string newFilename = m_total_new_image_name[imageName];
                string destImgFilename = Path.Combine(destFolder, newFilename);
                File.Move(imageName, destImgFilename);

                Program.AddLog("Move: " + imageName + " To " + destImgFilename);

                string xmpFilename = Path.GetFileNameWithoutExtension(newFilename);
                xmpFilename += ".xmp";
                xmpFilename = Path.Combine(destFolder, xmpFilename);
                XMPFile xmp_file = new XMPFile();
                xmp_file.LoadXML("Sample.xmp");
                xmp_file.RemoveNode(100);
                xmp_file.RemoveNode(102);
                xmp_file.RemoveAttribute(1);                
                xmp_file.SetPosition(posX, posY, gpsData.altitude);
                xmp_file.SaveXML(xmpFilename);

               // Program.AddLog("Save XMP file: " + xmpFilename);

                 
            }
        }
        public void BuildKMLFile()
        {
            string kmlFilename = Path.Combine(destFolder,sceneName + ".kml");
            KMLFile kmlFile = new KMLFile();
            kmlFile.LoadXML("SamplePath.kml");
            kmlFile.SetName(sceneName);
            string coordinatesStr = "";
            bool isFirst = true;
            int count = 0;
            int gpsCount = 0;
            foreach (var node in m_total_gps_data_list)
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
                //if (gpsData.frame == max_count_image_idx)
                //{
                //    coordinatesStr += Convert.ToString(gpsData.longitude);
                //    coordinatesStr += ",";
                //    coordinatesStr += Convert.ToString(gpsData.latitude);
                //    coordinatesStr += ",";
                //    coordinatesStr += Convert.ToString(gpsData.altitude);
                //    coordinatesStr += "\n ";
                //    count++;
                //    continue;
                //}
                //string filename = node.Key;
            }

            kmlFile.SetCoordinates(coordinatesStr);
            kmlFile.SaveXML(kmlFilename);
            Program.AddLog("Save KML: " + kmlFilename);
            return;
        }

        //100km / 30kmh = 3.333h * 60m * 60s * 30frame = 360,000
        //
        static public void SetupSectionsFolder(string folder,int nbSectionImg, int nStep,int nCopy)
        {
            Dictionary<int, string> imagesList = new Dictionary<int, string>();
            
            //Search the images folder
            char[] charSeparators = new char[] { '_', '(', ')', ' ' };
            string[] fileEntries = Directory.GetFiles(folder);
            string folderName = Path.GetFileName(Path.GetDirectoryName(folder));
            int max_index = 0;
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
                    if (max_index < idx)
                        max_index = idx;
                    imagesList[idx] = fileName;
                }
            }

            //Create sections folder;
            int nbTotalImg = imagesList.Count;
            int nbSection = (nbTotalImg / nStep + nbSectionImg - 1) / nbSectionImg;
            string[] sectionPath = new string[nbSection];
            for(int a=0;a<nbSection;a++)
            {
                int idxSec = a + 1;
                string sectionName = string.Format("Section{0}", idxSec.ToString("D3"));
                sectionPath[a] = Path.Combine(folder, sectionName);
                if(!Directory.Exists(sectionPath[a]))
                {
                    Directory.CreateDirectory(sectionPath[a]);
                }
            }

            //Select images
            int count = 0;
            int count_step = 0;
            int secCount = 0;
            foreach (var node in imagesList)
            {
                if (count_step % nStep != 0)
                {
                    count_step++;
                    continue;
                }
                count_step++;

                int idx = node.Key;
                string imageName = node.Value;
                string filename = Path.GetFileName(imageName);
                string destImgFilename = Path.Combine(folder, sectionPath[secCount], filename);
                if(nCopy == 0)
                {
                    File.Move(imageName, destImgFilename);
                }
                else
                    File.Copy(imageName, destImgFilename, true);
                Program.AddLog("Copy JPG: " + imageName + " To " + destImgFilename);

                string xmpFilename = Path.ChangeExtension(imageName,"xmp");
                if(File.Exists(xmpFilename))
                {
                    string xmpFile = Path.ChangeExtension(filename, "xmp");
                    string xmpDestFilename = Path.Combine(folder, sectionPath[secCount], xmpFile);
                    if (nCopy == 0)
                    {
                        File.Move(xmpFilename, xmpDestFilename);
                    }
                    else
                        File.Copy(xmpFilename, xmpDestFilename, true);
                    Program.AddLog("Copy XMP: " + imageName + " To " + destImgFilename);
                }
                count++;
                if(count >= nbSectionImg)
                {
                    secCount++;
                    count = 0;
                }
            }
            return;
        }
    }
}
