using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;

namespace CoDriverWinApp
{
    class VideoCollection
    {
        public string[] targetPath = new string[20];
        public struct VideoInfo
        {
            public string filename;
            public string address;
            public double focalLen;
            public double ppx;
            public double ppy;
            public double p1;
            public double p2;
            public double p3;
        }
        private Dictionary<int, VideoInfo> videoDictionary = new Dictionary<int, VideoInfo>();

        public int GetNumVideo()
        {
            return videoDictionary.Count;
        }
        public VideoInfo GetVideoInfo(int idx)
        {
            if (!videoDictionary.ContainsKey(idx))
            {
                VideoInfo vi = new VideoInfo();
                return vi;
            }
            return videoDictionary[idx];
        }

        public void Init()
        {
            StreamReader reader = new StreamReader("VideoPath.ini");
            int count = 0;
            char[] charSeparators = new char[] { '=', ';', ',' };
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                if (line.Length <= 1)
                    continue;
                if (line[0] == '#')
                    continue;
                if (!Directory.Exists(line))
                    continue;
                targetPath[count] = line;
                count++;
            }
            reader.Close();
        }
        public void CheckVideoPath()
        {
            int count = 0;
            int countVideo = 0;
            foreach(var node in targetPath)
            {
                count++;
                countVideo = 0;
                int baseIdx = count * 100000;
                if (node == null)
                    continue;
                if (node.Length<1)
                    continue;
                if (!Directory.Exists(node))
                    continue;
                string[] files = Directory.GetFiles(node);
                foreach (var filename in files)
                {
                    string ext = Path.GetExtension(filename);
                    ext = ext.ToLower();
                    if(ext.Contains("mp4"))
                    {
                        countVideo++;
                        int idx = baseIdx + countVideo;

                        string infoFile = Path.ChangeExtension(filename,"txt");
                        VideoInfo vi = loadVideoInfo(infoFile);
                        vi.filename = filename;
                        videoDictionary.Add(idx, vi);
                    }
                }
            }
            return;
        }
        public VideoInfo loadVideoInfo(string infoFilenanme)
        {
            VideoInfo vi = new VideoInfo();
            if (!File.Exists(infoFilenanme))
                return vi;
            StreamReader reader = new StreamReader(infoFilenanme);
            char[] charSeparators = new char[] { '=', ';', ',' };
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                if (line.Length <= 1)
                    continue;
                if (line[0] == '#')
                    continue;
                var values = line.Split(charSeparators, StringSplitOptions.RemoveEmptyEntries);
                if (values.Length < 2)
                    continue;

                if (values[0] == "Address")
                    vi.address = values[1];
            }
            return vi;
        }
    }
}
