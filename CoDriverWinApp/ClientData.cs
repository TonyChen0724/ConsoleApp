using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoDriverWinApp
{
    class ClientData
    {
        public string videoname_upload = "";
        public string infoFilename_upload = "";

        //1 -- Request upload file.
        //2 -- Got response from remote center, start upload files to ftp server.
        //3 -- upload to ftp server done, send msg back to remote center.
        public int upload_progress = 0;

        //
        private int error_code_i;
        public int error_code
        {
            get
            {
                return error_code_i;
            }
            set
            {
                error_code_i = value;
                Program.AddLog("ErrorCode: " + error_code_i);
            }
        }

        public bool isTransferAction()
        {
            if (upload_progress != 0)
                return true;
            if (download_images_progress != 0)
                return true;
            if (download_video_progress != 0)
                return true;
            return false;
        }

        public string downloadImages_DestFolder = "";
        public string downloadImages_VideoName = "";
        public int download_images_progress = 0;

        public int download_video_progress = 0;
        public string[] downloadVideoFiles = new string[2];
        public string downloadVideoDestFolder = "";
    }
}
