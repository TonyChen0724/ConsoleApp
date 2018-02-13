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
  

    public partial class FormVideoLib : Form
    {

        public FormVideoLib()
        {
            InitializeComponent();
            Timer timer = new Timer();
            timer.Interval = (300);
            timer.Tick += new EventHandler(Tick);
            timer.Start();
            Program.g_MsgSender.SendMsg((int)MsgTypeCS.ReqVideoList, Program.g_MsgSender.GetReqVideoList());
            Program.g_ClientData.download_images_progress = 0;
        }
        bool isCopyAction = false;
        string ftp_video_path = "/Video";
        private void Tick(object sender, EventArgs e)
        {
            if(Program.g_videoInfoList.isUpdated)
            {
                Program.g_videoInfoList.UpdateList(list_fvi_videolist);
                Program.g_videoInfoList.isUpdated = false;
            }
                

            if (Program.g_ClientData.isTransferAction())
            {
                btn_fvi_copy_video.Enabled = false;
                btn_fvi_copy_images.Enabled = false;
                btn_fvi_import_ue4.Enabled = false;
                if (!Program.g_ftpServer.IsTask())
                {
                    isCopyAction = false;
                    label_process_info.Text = "Download files done.";
                    if (Program.g_ClientData.download_images_progress > 0)
                        Program.g_ClientData.download_images_progress = 0;
                    if (Program.g_ClientData.download_video_progress > 0)
                        Program.g_ClientData.download_video_progress = 0;
                }
            }
            else
            {
                btn_fvi_import_ue4.Enabled = true;
                btn_fvi_copy_video.Enabled = true;
                btn_fvi_copy_images.Enabled = true;
            }
            if(Program.g_ClientData.download_images_progress > 0
                || Program.g_ClientData.download_video_progress > 0)
            {
                progress_fvi_action.Maximum = Program.g_ftpServer.dest_download;
                progress_fvi_action.Value = Program.g_ftpServer.count_download;
            }
            else
            {

            }
            return;
        }
        private void btn_fvi_copy_images_Click(object sender, EventArgs e)
        {
            if (isCopyAction)
                return;
            string videoName = Program.g_videoInfoList.GetVideoFilename(list_fvi_videolist.SelectedIndex);
            string videoNameWithoutExt = Path.GetFileNameWithoutExtension(videoName);
            string destFolder = Properties.Settings.Default.WorkFolder;
            destFolder = Path.Combine(destFolder, videoNameWithoutExt);
            if (Directory.Exists(destFolder))
            {
                string[] filesInFolder = Directory.GetFiles(destFolder);
                if (filesInFolder.Length > 1)
                {
                    label_process_info.Text = "The specified video folder has been exists in work folder.";
                    return;
                }
            }
            else
            {
                Directory.CreateDirectory(destFolder);
            }
            Program.g_ClientData.downloadImages_DestFolder = destFolder;
            Program.g_ClientData.downloadImages_VideoName = videoName;

            isCopyAction = true;
            RequestCopyImages();

           
        }
        private void RequestCopyImages()
        {
            if(Program.g_ClientData.downloadImages_VideoName.Length < 1)
            {
                return;
            }
            string msgData = Program.g_ClientData.downloadImages_VideoName;
            string sendData = TransferProtocol.Msg_RequestDownloadImages_CS(msgData);
            Program.g_console_client.Send(sendData);
            Program.g_ClientData.download_images_progress = 1;
        }
        public void RequestCopyImagesResult(int result)
        {
            if (result != 0)
            {
                isCopyAction = false;
                Program.g_ClientData.download_images_progress = 0;
                label_process_info.Text = "There is no image file on remote computer.";
                return;
            }
            Program.g_ClientData.download_images_progress = 2;
            Program.g_ftpServer.LinkFTPSite(0);
            string videoNameWithoutExt = Path.GetFileNameWithoutExtension(Program.g_ClientData.downloadImages_VideoName);
            string[] fileList = Program.g_ftpServer.GetFiles(videoNameWithoutExt);
            if (fileList.Length < 1)
            {
                isCopyAction = false;
                Program.g_ClientData.download_images_progress = 0;
                Program.g_ftpServer.Disconnect();
                label_process_info.Text = "There is no image file on ftp server.";
                return;
            }

            label_process_info.Text = "Download image files...";
            if (!Directory.Exists(Program.g_ClientData.downloadImages_DestFolder))
            {
                Directory.CreateDirectory(Program.g_ClientData.downloadImages_DestFolder);
            }
            Program.g_ftpServer.DownloadFiles(fileList, Program.g_ClientData.downloadImages_DestFolder);
            return;
        }

        private void btn_fvi_copy_video_Click(object sender, EventArgs e)
        {
            if (isCopyAction)
                return;
            string videoName = Program.g_videoInfoList.GetVideoFilename(list_fvi_videolist.SelectedIndex);

            Program.g_ClientData.downloadVideoFiles[0] = ftp_video_path + "/" + videoName;
            string txtFile = Path.ChangeExtension(videoName, "txt");
            Program.g_ClientData.downloadVideoFiles[1] = ftp_video_path + "/" + txtFile;
            string destFolder = Properties.Settings.Default.WorkFolder;
            string destFilename = Path.Combine(destFolder, videoName);
            if(File.Exists(destFilename))
            {
                label_process_info.Text = "The specified file has been exists in work folder.";
                return;
            }

            isCopyAction = true;
            RequestCopyVideo();
            //Program.g_ftpServer.LinkFTPSite(0);
            //label_process_info.Text = "Copy video file...";
            //Program.g_ftpServer.DownloadFiles(downloadFiles , destFolder);
        }
        private void RequestCopyVideo()
        {
            string msgData = Program.g_ClientData.downloadVideoFiles[0];
            string sendData = TransferProtocol.Msg_RequestDownloadVideo_CS(msgData);
            Program.g_console_client.Send(sendData);
            Program.g_ClientData.download_video_progress = 1;
        }
        public void RequestCopyVideoResult(int result)
        {
            if(result != 0)
            {
                isCopyAction = false;
                Program.g_ClientData.download_video_progress = 0;
                return;
            }
            Program.g_ClientData.download_video_progress = 2;
            Program.g_ftpServer.LinkFTPSite(0);
            label_process_info.Text = "Copy video file...";
            Program.g_ftpServer.DownloadFiles(Program.g_ClientData.downloadVideoFiles, Program.g_ClientData.downloadVideoDestFolder);
            return;
        }


        private void text_input_search_TextChanged(object sender, EventArgs e)
        {
            string searchKeyword = text_input_search.Text;
            Program.g_videoInfoList.UpdateListWithKeyword(list_fvi_videolist, searchKeyword);
        }

        private void list_fvi_videolist_SelectedIndexChanged(object sender, EventArgs e)
        {
            Program.g_videoInfoList.ShowUpVideoInfo(list_fvi_videolist.SelectedIndex, richText_videoInfo);
        }

        private void btn_fvi_update_list_Click(object sender, EventArgs e)
        {
            Program.g_videoInfoList.UpdateList(list_fvi_videolist);
        }

        private void btn_fvi_import_ue4_Click(object sender, EventArgs e)
        {

        }

        private void btn_fvi_copy_mesh_Click(object sender, EventArgs e)
        {

        }
    }
}
