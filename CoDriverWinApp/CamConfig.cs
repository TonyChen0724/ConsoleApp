using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace CoDriverWinApp
{
    public struct CamConfig
    {
        public string name;
        public string r1;
        public string r2;
        public string r3;
        public string focallen;
        public string ppx;
        public string ppy;
    }

    class CamConfigManager
    {
        List<CamConfig> camera_list = new List<CamConfig>();
        static public Color default_backcolor;
        static public Color default_forecolor;
        public void LoadFolder(string folder)
        {
            string[] fileEntries = Directory.GetFiles(folder);
            foreach (string fileName in fileEntries)
            {
                string ext = Path.GetExtension(fileName);
                if (ext == ".ini")
                {
                    //File.Delete(fileName);
                    LoadCamConfig(fileName);
                }
            }
        }
        public bool check_select_cam(ListBox list)
        {
            bool isError = false;
            int camIdx = list.SelectedIndex;
            if (camIdx < 0 || camIdx >= camera_list.Count)
            {
                list.ForeColor = Color.Red;
                list.BackColor = Color.Yellow;
                isError = true;
            }
            else
            {
                list.ForeColor = default_forecolor;
                list.BackColor = default_backcolor;
            }
            return isError;
        }
        public void UpdateListBox(ListBox list)
        {
            list.Items.Clear();
            int nb = camera_list.Count;
            for (int a = 0; a < nb; a++)
            {
                CamConfig cam_config = camera_list[a];
                list.Items.Add(cam_config.name);
            }
            list.SelectedIndex = 0;
        }

        public int GetNbCamConfig()
        {
            return camera_list.Count;
        }

        public CamConfig GetCamConfig(int idx)
        {
            return camera_list[idx];
        }

        void LoadCamConfig(string filename)
        {
            CamConfig cam_config = new CamConfig();
            cam_config.name = Path.GetFileNameWithoutExtension(filename);
            StreamReader reader = new StreamReader(filename);
            int count = 0;
            char[] charSeparators = new char[] { '=',';',',' };
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                if (line.Length <= 3)
                    continue;
                if (line[0] == '[')
                    continue;
                var values = line.Split(charSeparators, StringSplitOptions.RemoveEmptyEntries);
                if (values.Length < 2)
                    continue;

                if (values[0] == "FocalLen")
                    cam_config.focallen = values[1];

                if (values[0] == "R1")
                    cam_config.r1 = values[1];

                if (values[0] == "R2")
                    cam_config.r2 = values[1];

                if (values[0] == "R3")
                    cam_config.r3 = values[1];

                if (values[0] == "PPX")
                    cam_config.ppx = values[1];

                if (values[0] == "PPY")
                    cam_config.ppy = values[1];                
                count++;
            }
            camera_list.Add(cam_config);
            reader.Close();

            return;
        }
    }
}
