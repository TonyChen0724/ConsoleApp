using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace CoDriverWinApp
{
    public partial class FormUploadVideo : Form
    {
        string[] infoData = new string[9];

        public FormUploadVideo()
        {
            InitializeComponent();
            Timer timer = new Timer();
            timer.Interval = (500);
            timer.Tick += new EventHandler(Tick);
            timer.Start();
            
            
            Program.g_camConfigMgr.UpdateListBox(list_f2_list_camera);
            if (check_use_undistort.Checked)
            {
                list_f2_list_camera.Enabled = true;
                list_f2_list_camera.SelectedIndex = 0;
            }
            else
            {
                list_f2_list_camera.Enabled = false;
                list_f2_list_camera.SelectedIndex = -1;
            }
            Program.g_ClientData.upload_progress = 0;
        }
        private void RequestUpload()
        {
            //string msgData = uploadFiles[0];
            //msgData += "#";
            //msgData += uploadFiles[1];
            string[] uploadFiles = new string[2];
            uploadFiles[0] = Program.g_ClientData.videoname_upload;
            uploadFiles[1] = Program.g_ClientData.infoFilename_upload;
            Program.g_MsgSender.SendMsg((int)MsgTypeCS.ReqUploadVideo,uploadFiles);
            Program.g_ClientData.upload_progress = 1;
        }
        public void RequestUploadResult()
        {
            Program.g_ftpServer.LinkFTPSite(0);
            string[] files = new string[2];
            files[0] = Program.g_ClientData.videoname_upload;
            files[1] = Program.g_ClientData.infoFilename_upload;
            Program.g_ftpServer.SendFiles(files, "/Video");
            return;
        }
        private void Tick(object sender, EventArgs e)
        {
            if(Program.g_ftpServer.IsUploadCompleted())
            {                
                if(Program.g_ClientData.upload_progress == 3)
                {
                    if (Program.g_ClientData.infoFilename_upload.Length > 3)
                    {
                        File.Delete(Program.g_ClientData.infoFilename_upload);
                        Program.g_ClientData.infoFilename_upload = "";
                    }
                    Program.g_MsgSender.SendMsg((int)MsgTypeCS.ReqUploadVideo, Program.g_MsgSender.GetReqUploadVideoEnd());
                    Program.g_ClientData.upload_progress = 0;
                }
            }
            if (Program.g_ClientData.upload_progress == 2)
            {
                RequestUploadResult();
                Program.g_ClientData.upload_progress = 3;
            }
            if (Program.g_ClientData.upload_progress == 0)
            {
                btn_f2_cancel.Enabled = true;
                btn_f2_select.Enabled = true;
                btn_f2_upload.Enabled = true;                
            }
            else
            {
                btn_f2_cancel.Enabled = false;
                btn_f2_select.Enabled = false;
                btn_f2_upload.Enabled = false;
            }

            if(Program.g_ClientData.error_code == 30001)
            {
                label_fuv_info.Text = "There is already exists a file with the same name on storage server!";
            }
            return;
        }
        private void SaveInfoFile(string filename)
        {
            using (StreamWriter writer = File.AppendText(filename))
            {
                string fileData = "";
                for (int a = 0; a < 9; a++)
                {
                    fileData += infoData[a];
                    fileData += ";";
                }
                writer.Write(fileData);
                writer.Close();
            }
            //writer.Dispose();
        }
        private void LoadInfoFile(string filename)
        {
            using (StreamReader reader = new StreamReader(filename))
            {
                string fileData = reader.ReadToEnd();
                char[] charSeparators = new char[] { '=', ';', ',' };
                var values = fileData.Split(charSeparators, StringSplitOptions.RemoveEmptyEntries);
                for (int a = 0; a < 9; a++)
                {
                    infoData[a] = values[a];
                }
                reader.Close();
            }
                
            return;
        }
        private void btn_f2_upload_Click(object sender, EventArgs e)
        {
            for (int a = 0; a < 9; a++)
            {
                infoData[a] = "";
            }
            infoData[0] = text_scene_name.Text;
            infoData[1] = text_uploader_name.Text;
            if (check_use_undistort.Checked)
            {
                if (!Program.g_camConfigMgr.check_select_cam(list_f2_list_camera))
                {
                    int camIdx = list_f2_list_camera.SelectedIndex;
                    CamConfig cam_config = Program.g_camConfigMgr.GetCamConfig(camIdx);
                    infoData[2] = cam_config.name;
                    infoData[3] = cam_config.r1;
                    infoData[4] = cam_config.r2;
                    infoData[5] = cam_config.r3;
                    infoData[6] = cam_config.focallen;
                    infoData[7] = cam_config.ppx;
                    infoData[8] = cam_config.ppy;
                }
                else
                    return;
            }
            string filename = label_video_path.Text;
            if (!File.Exists(filename))
                return;
            string infoFile = Path.ChangeExtension(label_video_path.Text, "txt");
            if (File.Exists(infoFile))
            {
                //File.Delete(infoFile);
            }
            SaveInfoFile(infoFile);
            //infoFilename = infoFile;
            Program.g_ClientData.videoname_upload = filename;
            Program.g_ClientData.infoFilename_upload = infoFile;
            RequestUpload();
           // File.Delete(infoFile);
           // panel_video_list.Enabled = false;
        }

        private void btn_f2_select_Click(object sender, EventArgs e)
        {
            string filter = "Movie files (*.mp4)|*.mp4|All files (*.*)|*.*";
            label_video_path.Text = ConsoleFunction.select_file(filter);
            string infoFile = Path.ChangeExtension(label_video_path.Text, "txt");
            if(File.Exists(infoFile))
            {
                File.Delete(infoFile);
                //LoadInfoFile(infoFile);
                //text_scene_name.Text = infoData[0];
                //text_uploader_name.Text = infoData[1];
                //check_use_undistort.Checked = false;
            }
        }

        private void btn_f2_cancel_Click(object sender, EventArgs e)
        {
            Program.g_ftpServer.Disconnect();
            this.Close();
        }

        private void check_use_undistort_CheckedChanged(object sender, EventArgs e)
        {
            if(check_use_undistort.Checked)
            {
                list_f2_list_camera.Enabled = true;
                list_f2_list_camera.SelectedIndex = 0;
            }
            else
            {
                list_f2_list_camera.Enabled = false;
                list_f2_list_camera.SelectedIndex = -1;
            }
        }
    }
}
