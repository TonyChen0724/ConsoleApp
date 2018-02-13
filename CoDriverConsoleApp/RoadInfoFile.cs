using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace CoDriverConsoleApp
{
    class RoadInfoFile
    {
        public CSVFile csv_file;
        public string src_filename;
        public string head_string = ",Cam1,Cam2,Cam3,Cam4,CentreLineX,CentreLineY,CentreLineZ,RoadWidth,Append,Tag,3DAssets,SpeedZone,RecommendedSpeed";
        public string format_string = ",{0},,,,{1},{2},{3},,,,,,";
        public string format_string_images = ",{0},*,*,*,{1},{2},{3},,,,,,";
        public void Load(string filename)
        {
            src_filename = filename;
            csv_file = new CSVFile();
            csv_file.Load(filename);
        }

        public void BuildRoadInfoFromImageList(ImageList imgListFile,string[] keywords, string src_filename)
        {
            csv_file = new CSVFile();
            csv_file.BuildFile(src_filename, head_string);
            foreach(var node in imgListFile.image_nodes)
            {
                if(node.keyword.Contains(keywords[0]))
                {
                    if(keywords.Length == 4)
                    {
                        string newStr = string.Format(format_string_images, node.values[0], node.values[1], node.values[2], node.values[3]);
                        csv_file.AddLine(newStr);
                    }
                    else
                    {
                        string newStr = FormatLineData(node.values[0], node.values[1], node.values[2], node.values[3]);
                        csv_file.AddLine(newStr);
                    }
                }
            }
            csv_file.Save();
        }

        string FormatLineData(string imageName,string posX,string posY,string posZ)
        {
            string new_str = string.Format(format_string, imageName, posX, posY, posZ);

            return new_str;
        }
    }
}
