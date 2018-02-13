using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;

namespace CoDriverServerConsole
{
    class VideoCollection
    {

        private List<FileSystemWatcher> watcherList = new List<FileSystemWatcher>();
        private List<PathInfo> pathInfoList = new List<PathInfo>();
        public struct PathInfo
        {
            public int index;
            public string path;
            public int count { get; set; }

        }
        public struct VideoInfo
        {
            public string sceneName;
            public string uploader;
            public string uploadTime;
            public string filename;
            public string fullpath;
            public string camName;
            public string address;
            public string focalLen;
            public string ppx;
            public string ppy;
            public string p1;
            public string p2;
            public string p3;
            public int imageState;
            public int xmpState;
            public int gpsState;
            public string videoDesc;

            public string ToDescString()
            {
                string desc = string.Format("{0};{1};{2};{3};{4};", sceneName, uploader, uploadTime, filename, camName);
                desc = string.Format("{0};{1};{2};{3};{4};", desc, address, videoDesc, imageState,gpsState);

                return desc;
            }
            public void CheckImages()
            {
                string folder = Path.GetDirectoryName(fullpath);
                string filenameNoExt = Path.GetFileNameWithoutExtension(filename);
                folder = Path.Combine(folder, filenameNoExt);
                imageState = 1;
                if (!Directory.Exists(folder))
                {
                    imageState = 0;
                    return;
                }
                string[] files = Directory.GetFiles(folder);
                if(files.Length < 10)
                {
                    imageState = 0;
                }
                return;
            }

            public void RunExtractImages()
            {
                Program.g_AutoDispatcher.AddTask(AutoTaskStep.ATS_extract_images, fullpath);
            }
        }
        private Dictionary<int, VideoInfo> videoDictionary = new Dictionary<int, VideoInfo>();
        private Dictionary<string, int> videoIndexDictionary = new Dictionary<string, int>();
        //private Dictionary<int, int> videoFileAccessDictionary = new Dictionary<int, int>();

        public string[] GetVideoInfoList()
        {
            int patchCount = 20;
            int totalNb = videoDictionary.Count;
            int nbStr = totalNb / patchCount;
            if (totalNb % patchCount != 0)
                nbStr++;
            string[] infoList = new string[nbStr];
            int count = 0;
            int patch = 0;
            foreach(var node in videoDictionary)
            {
                string nodeDesc = node.Value.ToDescString();
                infoList[patch] += "#";
                infoList[patch] += nodeDesc;
                if (count < patchCount)
                {
                    count++;
                }
                else
                {
                    patch++;
                    count = 0;
                }
            }


            return infoList;
        }

        public int RequestUploadVideo(string videoFilename,string sceneName)
        {
            if (videoIndexDictionary.ContainsKey(videoFilename))
                return 1;
            foreach(var node in videoDictionary)
            {
                VideoInfo vInfo = node.Value;
                if (vInfo.sceneName == sceneName)
                    return 11;
                if (vInfo.filename == videoFilename)
                    return 2;
            }
            return 0;
        }

        //0 -- OK
        //1 -- Same video name;
        public int CheckIsPossibleUpload(string filename)
        {
            string file = Path.GetFileName(filename);
            if (videoIndexDictionary.ContainsKey(file))
                return 1;
            return 0;
        }

        public int CheckIsPossibleDownload(string filename)
        {
            string file = Path.GetFileName(filename);
            if (videoIndexDictionary.ContainsKey(file))
                return 1;
            return 0;
        }
        public int CheckIsPossibleDownloadImages(string filename)
        {
            string file = Path.GetFileName(filename);
            if (videoIndexDictionary.ContainsKey(file))
                return 1;


            return 0;
        }


        private int GetNewVideoIndex(string srcPath)
        {
            for(int a=0;a<pathInfoList.Count;a++)
            {
                if(pathInfoList[a].path == srcPath)
                {
                    PathInfo pi = pathInfoList[a];
                    pi.count++;
                    pathInfoList[a] = pi;
                    return pi.index * 1000 + pi.count;
                }
            }
            return 0;
        }
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
                count++;
                PathInfo pi = new PathInfo();
                pi.count = 0;
                pi.path = line;
                pi.index = count;
                pathInfoList.Add(pi);
                
            }
            reader.Close();
        }

        public void SetWatcher()
        {
            foreach(var node in pathInfoList)
            {
                if(node.path.Length > 2 && Directory.Exists(node.path))
                    watchFolder(node.path);
            }
        }

        private void watchFolder(string path)
        {
            //FileSystemWatcher watcher = new FileSystemWatcher();
            //watcher.Path = path;
            //watcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName;
            //watcher.Filter = "*.*";
            //watcher.Changed += new FileSystemEventHandler(OnChanged);
            //watcher.Deleted += new FileSystemEventHandler(OnDeleted);
            //watcher.EnableRaisingEvents = true;
            //watcherList.Add(watcher);

            Program.AddLog("Create Watcher: " + path);
        }
      

        public void CheckVideoPath()
        {
            for (int a = 0; a < pathInfoList.Count; a++)
            {
                if (pathInfoList[a].path.Length<1)
                    continue;
                if (!Directory.Exists(pathInfoList[a].path))
                    continue;
                string[] files = Directory.GetFiles(pathInfoList[a].path);
                int count = 0;
                foreach (var filename in files)
                {
                    string filename2;// = filename.ToLower();
                    filename2 = Path.GetFileName(filename);
                    string ext = Path.GetExtension(filename);
                    ext = ext.ToLower();
                    if(ext.Contains("mp4"))
                    {
                        if (videoIndexDictionary.ContainsKey(filename2))
                        {
                            int sIdx = videoIndexDictionary[filename2];
                            Program.AddLog("Watcher:CheckVideoPath - Same video file. " + filename + " And: " + videoDictionary[sIdx].fullpath);
                            continue;
                        }

                        PathInfo pi = pathInfoList[a];
                        pi.count++;
                        pathInfoList[a] = pi;

                        int idx = pathInfoList[a].index * 10000 + pathInfoList[a].count;
                        string infoFile = Path.ChangeExtension(filename, "txt");

                        VideoInfo vi = LoadInfoFile(infoFile);
                        vi.filename = filename2;
                        vi.fullpath = filename;
                        vi.CheckImages();
                        
                        videoDictionary.Add(idx, vi);
                        videoIndexDictionary.Add(filename2, idx);
                        //videoFileAccessDictionary.Add(idx, 22);
                        count++;
                    }
                }
                Program.AddLog("Watcher:CheckVideoPath - " + pathInfoList[a].path + " Count:" + count);
            }
            return;
        }
        public void CheckVideoPath(int PathIdx)
        {
            if (PathIdx >= pathInfoList.Count)
                return;
            if (pathInfoList[PathIdx].path.Length < 1)
                return;
            if (!Directory.Exists(pathInfoList[PathIdx].path))
                return;

            string[] files = Directory.GetFiles(pathInfoList[PathIdx].path);
            int count = 0;
            foreach (var filename in files)
            {
                string filename2 = Path.GetFileName(filename);
                string ext = Path.GetExtension(filename);
                ext = ext.ToLower();
                if (ext.Contains("mp4"))
                {
                    if (videoIndexDictionary.ContainsKey(filename2))
                    {
                        //int sIdx = videoIndexDictionary[filename2];
                        //Program.AddLog("Watcher:CheckVideoPath - Same video file. " + filename + " And: " + videoDictionary[sIdx].fullpath);
                        continue;
                    }
                    PathInfo pi = pathInfoList[PathIdx];
                    pi.count++;
                    pathInfoList[PathIdx] = pi;
                    int idx = pathInfoList[PathIdx].index * 10000 + pathInfoList[PathIdx].count;
                    string infoFile = Path.ChangeExtension(filename, "txt");
                    VideoInfo vi = LoadInfoFile(infoFile);
                    vi.filename = filename2;
                    vi.fullpath = filename;
                    vi.CheckImages();
                    if(vi.imageState == 0)
                    {
                        vi.RunExtractImages();
                    }
                    videoDictionary.Add(idx, vi);
                    videoIndexDictionary.Add(filename2, idx);
                    //videoFileAccessDictionary.Add(idx, 22);
                    count++;
                }
            }
            Program.AddLog("Watcher:CheckVideoPath - " + pathInfoList[PathIdx].path + " Count:" + count);            
            return;
        }
        public void CheckVideoFile(string videoFile)
        {
            string filename = Path.GetFileNameWithoutExtension(videoFile);
            if (!videoIndexDictionary.ContainsKey(filename))
            {
                return;
            }
            int idx = videoIndexDictionary[filename];
            if(!videoDictionary.ContainsKey(idx))
            {
                return;
            }
            VideoInfo vi = videoDictionary[idx];
            vi.CheckImages();
            videoDictionary[idx] = vi;
            return;
        }
        private VideoInfo LoadInfoFile(string filename)
        {
            VideoInfo vi = new VideoInfo();
            if(!File.Exists(filename))
            {
                Program.AddLog("Watcher:LoadInfoFile - not exist!" + filename);
                return vi;
            }
                
            using (StreamReader reader = new StreamReader(filename))
            {

                string fileData = reader.ReadToEnd();
                char[] charSeparators = new char[] { '=', ';', ',' };
                var values = fileData.Split(charSeparators, StringSplitOptions.None);
                if (values.Length >= 9)
                {
                    vi.sceneName = values[0];
                    vi.uploader = values[1];
                    vi.camName = values[2];
                    vi.p1 = values[3];
                    vi.p2 = values[4];
                    vi.p3 = values[5];
                    vi.focalLen = values[6];
                    vi.ppx = values[7];
                    vi.ppy = values[8];
                }
                else
                    Program.AddLog("Watcher:LoadInfoFile - wrong format!" + filename);
                reader.Close();
            }
                
            return vi;
        }
        //public VideoInfo loadVideoInfo(string infoFilenanme)
        //{
        //    VideoInfo vi = new VideoInfo();
        //    if (!File.Exists(infoFilenanme))
        //        return vi;
        //    StreamReader reader = new StreamReader(infoFilenanme);
        //    char[] charSeparators = new char[] { '=', ';', ',' };
        //    while (!reader.EndOfStream)
        //    {
        //        var line = reader.ReadLine();
        //        if (line.Length <= 1)
        //            continue;
        //        if (line[0] == '#')
        //            continue;
        //        var values = line.Split(charSeparators, StringSplitOptions.RemoveEmptyEntries);
        //        if (values.Length < 2)
        //            continue;

        //        if (values[0] == "Address")
        //            vi.address = values[1];
        //    }
        //    return vi;
        //}
    }
}
