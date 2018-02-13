using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Net.Security;
using FluentFTP;
using System.Security.Authentication;
namespace CoDriverServerConsole
{
    class FTPService
    {
        //public string ftpSite = "169.254.100.99";
        //public string ftpUserName = "anonymous";
        //public string ftpPassword = "anonymous@domain.com";
        public int count_upload = 0;
        public int dest_upload = 0;
        FtpClient ftpSocket;

        public struct FTPSiteInfo
        {
            public string site;
            public string userName;
            public string password;
        }
        FTPSiteInfo[] ftpSites = new FTPSiteInfo[1];
        public void Init()
        {
            StreamReader reader = new StreamReader("FTPInfo.ini");
            int count = 0;
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

                if (values[0] == "MainServer")
                    ftpSites[0].site = values[1];
                if (values[0] == "MainUsername")
                    ftpSites[0].userName = values[1];
                if (values[0] == "MainPassword")
                    ftpSites[0].password = values[1];
                count++;
            }
            reader.Close();
        }
        public void LinkFTPSite(int nSiteIdx)
        {
            string site;
            string username;
            string password;
            if (nSiteIdx == 0)
            {
                site = ftpSites[0].site;
                username = ftpSites[0].userName;
                password = ftpSites[0].password;
            }
            else
                return;
            //ftpSocket = new FtpClient("169.254.100.99");
            //ftpSocket.Credentials = new NetworkCredential(ftpUserName, ftpPassword);

            ftpSocket = new FtpClient(site, username, password); // or set Host & Credentials
            ftpSocket.EncryptionMode = FtpEncryptionMode.Explicit;
            ftpSocket.SslProtocols = SslProtocols.Tls;
            ftpSocket.ValidateCertificate += new FtpSslValidation(OnValidateCertificate);

            ftpSocket.Connect();

            string workDirectory = ftpSocket.GetWorkingDirectory();
            FtpListItem[] items = ftpSocket.GetListing("/");
            // get a list of files and directories in the "/htdocs" folder
            foreach (FtpListItem item in items)
            {
                if (item.Type == FtpFileSystemObjectType.Directory)
                {

                }
                    // if this is a file
                    if (item.Type == FtpFileSystemObjectType.File)
                {

                    // get the file size
                    long size = ftpSocket.GetFileSize(item.FullName);

                }

                // get modified date/time of the file or folder
                //DateTime time = client.GetModifiedTime(item.FullName);

                // calculate a hash for the file on the server side (default algorithm)
                //FtpHash hash = client.GetHash(item.FullName);
            }
            count_upload = 0;
            dest_upload = 0;
        }

        public void Disconnect()
        {
            if (ftpSocket == null)
                return;
            ftpSocket.Disconnect();
        }
        private void OnValidateCertificate(FtpClient control, FtpSslValidationEventArgs e)
        {
            // add logic to test if certificate is valid here
            e.Accept = true;
        }
        public void BuildTask()
        {

        }
        public bool IsUploadCompleted()
        {
            if (dest_upload == count_upload)
                return true;
            else
                return false;
        }
        public async void SendFiles(string[] filenames,string destDir)
        {
            dest_upload = filenames.Length;
            int n = await ftpSocket.UploadFilesAsync(filenames, destDir);
            count_upload = n;
            return;
        }
    }
}
