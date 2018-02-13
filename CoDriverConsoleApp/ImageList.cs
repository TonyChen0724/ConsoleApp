using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace CoDriverConsoleApp
{
    class ImageListNode : IEquatable<ImageListNode>, IComparable<ImageListNode>
    {
        public string[] values;
        public int index { get; set; }
        public string keyword { get; set; } 
        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            ImageListNode objAsPart = obj as ImageListNode;
            if (objAsPart == null)
                return false;
            else return Equals(objAsPart);
        }
        public override int GetHashCode()
        {
            return index;
        }
        public int CompareTo(ImageListNode comparePart)
        {
            // A null value means that this object is greater.
            if (comparePart == null)
                return 1;

            else
                return this.index.CompareTo(comparePart.index);
        }
        public bool Equals(ImageListNode other)
        {
            if (other == null) return false;
            return (this.index.Equals(other.index));
        }
    }

    class ImageList
    {
        public struct ImageInfo
        {
            public string keyword;
            public int idxMin;
            public int idxMax;
            public int count;
        }
        public CSVFile csv_file;

        public Dictionary<string, ImageInfo> image_info_map = new Dictionary<string, ImageInfo>();
        public List<ImageListNode> image_nodes = new List<ImageListNode>();
        public bool Load(string filename)
        {
            csv_file = new CSVFile();
            csv_file.Load(filename);
            char[] charSeparators = new char[] { '_' };
            foreach (var csv_data in csv_file.m_csv_data)
            {
                string[] data = csv_data.Value.values;
                if (data.Length < 4)
                    continue;
                string image_name = convert_string(data[0]);
                var values = image_name.Split(charSeparators, StringSplitOptions.RemoveEmptyEntries);
                if (values.Length < 2)
                    continue;
                bool isContainKey = image_info_map.ContainsKey(values[0]);
                if(!isContainKey)
                {
                    ImageInfo imgInfo = new ImageInfo();
                    imgInfo.keyword = values[0];
                    imgInfo.idxMin = 9999;
                    imgInfo.idxMax = 0;
                    imgInfo.count = 0;
                    image_info_map[values[0]] = imgInfo;
                }


                int cur_index = Convert.ToInt32(values[1]);
                ImageInfo img_info = image_info_map[values[0]];
                if (img_info.idxMin > cur_index)
                {
                    img_info.idxMin = cur_index;
                }
                if (img_info.idxMax < cur_index)
                    img_info.idxMax = cur_index;
                img_info.count++;
                image_info_map[values[0]] = img_info;


                ImageListNode node = new ImageListNode();
                node.values = data;
                node.index = cur_index;
                node.keyword = values[0];
                image_nodes.Add(node);
            }

            image_nodes.Sort();
            return true;
        }

        static public string convert_string(string data_str)
        {
            string new_str = Path.GetFileNameWithoutExtension(data_str);
            new_str = new_str.Replace(' ', '_');
            new_str = new_str.Replace('(', '_');
            new_str = new_str.Replace(')', '_');
            return new_str;
        }
    }
}
