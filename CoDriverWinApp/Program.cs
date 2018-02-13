using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace CoDriverWinApp
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        /// 
        static public Form1 g_Form;
        static public FTPService g_ftpServer;
        static public ConsoleClient g_console_client;
        static public VideoInfoList g_videoInfoList = new VideoInfoList();
        static public MsgReceiver g_MsgReceiver = new MsgReceiver();
        static public MsgSender g_MsgSender = new MsgSender();
        static public ClientData g_ClientData = new ClientData();
        static public CamConfigManager g_camConfigMgr = new CamConfigManager();

        static public void AddCommandLog(string str1,string str2,string str3)
        {
            string str = "Command:";
            str += str1;
            str += " ";
            str += str2;
            str += "-";
            str += str3;
            g_Form.AddLog(str);
        }

        static public void AddLog(string str)
        {
            if (g_Form == null)
                return;
            g_Form.AddLog(str);
        }
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            g_Form = new Form1();
            g_Form.InitLog();
            g_ftpServer = new FTPService();
            g_ftpServer.Init();
            g_console_client = new ConsoleClient();
            g_MsgReceiver.Init();
            g_MsgSender.Init();
            g_camConfigMgr = new CamConfigManager();
            string path = System.IO.Directory.GetCurrentDirectory();
            path += "\\CameraConfig\\";
            g_camConfigMgr.LoadFolder(path);

            Application.Run(g_Form);
            g_Form.CloseLog();
        }
        
    }
}
