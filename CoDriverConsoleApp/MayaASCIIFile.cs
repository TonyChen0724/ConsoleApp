using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace CoDriverConsoleApp
{
    class MayaASCIIFile
    {
        public struct TransformInfo
        {
            public string[] translate;
            public string[] rotate;
        }
        public struct CameraInfo
        {
            public string imagefile;
            public string focalLength;
            public string imageSizeX;
            public string imageSizeY;
            public TransformInfo transform;
        }
        public struct ImageInfo
        {
            public string keyword;
            public int count;
        }
        public Dictionary<string, CameraInfo> m_maya_data = new Dictionary<string, CameraInfo>();
        public Dictionary<string, ImageInfo> image_info_map = new Dictionary<string, ImageInfo>();
        public int number;
        string src_filename;

        public int CheckImagesFolder(string imagesFolder,string backupFolder)
        {
            string now_time = string.Format("{0}_{1:HHmmss}\\", DateTime.Now.ToString("yyyyMMdd"), DateTime.Now);
            backupFolder += now_time;
            if (!Directory.Exists(backupFolder))
            {
                Directory.CreateDirectory(backupFolder);
            }
            int count = 0;
            string[] fileEntries = Directory.GetFiles(imagesFolder);
            foreach (string fileName in fileEntries)
            {
                string img_name = Path.GetFileName(fileName);
                if (!m_maya_data.ContainsKey(img_name))
                {
                    string backup_filename = backupFolder + img_name;
                    File.Move(fileName, backup_filename);
                    //File.Move();
                }
                else
                {
                    count++;
                }
            }
            return count;
        }
        public bool Load(string filename)
        {
            src_filename = filename;
            StreamReader reader = new StreamReader(filename);
            int count = 0;
            int current_segment = 0;
            TransformInfo transformInfo = new TransformInfo();
            transformInfo.translate = new string[3];
            transformInfo.rotate = new string[3];
            CameraInfo cameraInfo = new CameraInfo();
            char[] charSeparators = new char[] { ' ',';' };
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();                
                if (line.Length <= 3)
                    continue;
                if (line[0] == '#')
                    continue;
                if (line[0] == '/' && line[1] == '/')
                    continue;

                var values = line.Split(charSeparators, StringSplitOptions.RemoveEmptyEntries);
                if (values.Length < 2)
                    continue;
                var string_values = line.Split('"');
                if (values[0] == "createNode")
                {
                    current_segment = 0;
                    if (string_values.Length < 2)
                        continue;
                    if (values[1] == "transform"
                        && string_values[1].Contains("Camera"))
                    {
                        current_segment = 1;
                    }
                    if (values[1] == "camera")
                    {
                        current_segment = 2;
                    }
                    if (values[1] == "imagePlane")
                    {
                        current_segment = 3;
                    }
                }
                if (values[0].Contains("setAttr"))
                {
                    if (string_values.Length < 2)
                        continue;
                    if (current_segment == 1)
                    {
                        if (values.Length < 7)
                            continue;
                        if (string_values[1].Contains("translate"))
                        {
                            transformInfo.translate[0] = values[4];
                            transformInfo.translate[1] = values[5];
                            transformInfo.translate[2] = values[6];
                        }
                        if (string_values[1].Contains("rotate"))
                        {
                            transformInfo.rotate[0] = values[4];
                            transformInfo.rotate[1] = values[5];
                            transformInfo.rotate[2] = values[6];
                        }
                    }
                    if (current_segment == 2)
                    {
                        if (string_values[1].Contains("focalLength"))
                        {
                            if (values.Length < 2)
                                continue;
                            cameraInfo.focalLength = values[2];
                        }
                        if (string_values[1].Contains("imageFileName"))
                        {
                            if (string_values.Length < 6)
                                continue;
                            cameraInfo.imagefile = string_values[5];
                        }
                    }
                    if (current_segment == 3)
                    {
                        if (string_values[1].Contains(".cov"))
                        {
                            cameraInfo.imageSizeX = values[4];
                            cameraInfo.imageSizeY = values[5];
                            cameraInfo.transform.rotate = new string[3];
                            cameraInfo.transform.rotate[0] = transformInfo.rotate[0];
                            cameraInfo.transform.rotate[1] = transformInfo.rotate[1];
                            cameraInfo.transform.rotate[2] = transformInfo.rotate[2];
                            cameraInfo.transform.translate = new string[3];
                            cameraInfo.transform.translate[0] = transformInfo.translate[0];
                            cameraInfo.transform.translate[1] = transformInfo.translate[1];
                            cameraInfo.transform.translate[2] = transformInfo.translate[2];
                            m_maya_data.Add(cameraInfo.imagefile, cameraInfo);
                        }
                    }
                }

                //CameraInfo camera_data = new CameraInfo();
                //line_data.values = values;
                //line_data.data = line;
                //if (count == 0)
                //    keywords = values;

                //m_csv_data.Add(count, line_data);
                count++;
            }
            number = m_maya_data.Count;
            reader.Close();

            foreach(var node in m_maya_data)
            {
                string rawImageName = node.Value.imagefile;
                string image_name = ImageList.convert_string(rawImageName);
                char[] charSep = new char[] { '_' };
                var values = image_name.Split(charSep, StringSplitOptions.RemoveEmptyEntries);
                if (values.Length < 2)
                    continue;
                bool isContainKey = image_info_map.ContainsKey(values[0]);
                if (!isContainKey)
                {
                    ImageInfo imgInfo = new ImageInfo();
                    imgInfo.keyword = values[0];
                    imgInfo.count = 0;
                    image_info_map[values[0]] = imgInfo;
                }
                int cur_index = Convert.ToInt32(values[1]);
                ImageInfo img_info = image_info_map[values[0]];
                img_info.count++;
                image_info_map[values[0]] = img_info;
            }
            return true;
        }
    }
}
