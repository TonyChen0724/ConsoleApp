using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CoDriverWinApp
{
    class VideoInfoList
    {
        public class VideoInfo
        {
            static char[] charInfoSeparators = new char[] { ';' };
            public VideoInfo(string strData)
            {
                var values = strData.Split(charInfoSeparators, StringSplitOptions.None);
                sceneName = values[0];
                uploader = values[1];
                uploadTime = values[2];
                filename = values[3];
                camName = values[4];
                address = values[5];
            }
            public string sceneName;
            public string uploader;
            public string uploadTime;
            public string filename;
            public string camName;
            public string address;
            //public string focalLen;
            //public string ppx;
            //public string ppy;
            //public string p1;
            //public string p2;
            //public string p3;
        }

        public bool isUpdated = false;
        private Dictionary<int, VideoInfo> videoDictionary = new Dictionary<int, VideoInfo>();
        private Dictionary<string, int> videoIndexDictionary = new Dictionary<string, int>();
        private Dictionary<int, string> videoItemIndexDictionary = new Dictionary<int, string>();

        public void AddVideoListFromMsg(string[] msgStr)
        {
            int idx = videoDictionary.Count;
            foreach (var node in msgStr)
            {
                if (node.Length < 2)
                    continue;
                idx++;
                VideoInfo vi = new VideoInfo(node);
                if (videoIndexDictionary.ContainsKey(vi.filename))
                    continue;
                videoDictionary.Add(idx, vi);
                videoIndexDictionary.Add(vi.filename, idx);
                isUpdated = true;
            }
        }

        public void UpdateListWithKeyword(ListBox videoListBox,string keyword)
        {
            videoListBox.Items.Clear();
            videoItemIndexDictionary.Clear();

            foreach (var node in videoIndexDictionary)
            {
                string filename = node.Key;
                if (!filename.Contains(keyword))
                    continue;
                int itemIdx = videoListBox.Items.Add(node.Key);
                videoItemIndexDictionary.Add(itemIdx, node.Key);
            }
            return;
        }

        public void UpdateList(ListBox videoListBox)
        {
            videoListBox.Items.Clear();
            videoItemIndexDictionary.Clear();
            foreach (var node in videoIndexDictionary)
            {
                int itemIdx = videoListBox.Items.Add(node.Key);
                videoItemIndexDictionary.Add(itemIdx,node.Key);
            }
            return;
        }
        public string GetVideoFilename(int selectItemIdx)
        {
            if (!videoItemIndexDictionary.ContainsKey(selectItemIdx))
                return "";
            string selectStr = videoItemIndexDictionary[selectItemIdx];
            return selectStr;
        }
        public void ShowUpVideoInfo(int selectItemIdx,RichTextBox infoBox)
        {
            infoBox.Text = "";
            if (!videoItemIndexDictionary.ContainsKey(selectItemIdx))
                return;
            string selectStr = videoItemIndexDictionary[selectItemIdx];
            if (!videoIndexDictionary.ContainsKey(selectStr))
                return;
            int idx = videoIndexDictionary[selectStr];
            VideoInfo vi = videoDictionary[idx];
            infoBox.Text = vi.filename + Environment.NewLine + vi.sceneName + Environment.NewLine + vi.uploader;
        }
    }
}
