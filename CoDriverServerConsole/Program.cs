using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using EpServerEngine.cs;
namespace CoDriverServerConsole
{
    class Program
    {
        //static private Thread BackGroundThread;
        static System.IO.StreamWriter m_log_file;
        static public void AddCommandLog(string str1, string str2, string str3)
        {
            string str = "Command:";
            str += str1;
            str += " ";
            str += str2;
            str += "-";
            str += str3;
            AddLog(str);
        }
        static public void AddLog(string v)
        {
            Console.WriteLine(v);
            string timeStr = DateTime.Now.ToString("u");
            string lines = timeStr + " " + v + "\n";
            m_log_file.WriteLine(lines);

        }
        static int ParseInput(string inStr)
        {
            if (inStr == "q")
                return 0;
            if (inStr == "Quit")
                return 0;
            if (inStr == "list")
            {
                listClients();
                return 5;
            }
            char[] charSeparators = new char[] { '=', ';', ',',' ' };
            var values = inStr.Split(charSeparators, StringSplitOptions.RemoveEmptyEntries);
            if (values.Length < 2)
                return 100;
            if(values[0] == "Rev" && values.Length == 3)
            {
                int v;
                bool isInt = Int32.TryParse(values[1], out v);
                if(isInt)
                {
                    INetworkSocket socket =Program.g_console_server.m_socket_manager.GetSocket(v);
                    string data = Program.g_console_server.GetSimulatedData(values[2]);
                    Program.g_console_server.m_socket_manager.MsgReceived(socket,data);
                }
            }


            return 100;
        }
        static public MsgReceiver g_MsgReceiver = new MsgReceiver();
        static public MsgSender g_MsgSender = new MsgSender();
        static public ConsoleServer g_console_server;
        static public VideoCollection videoCollectionFunc = new VideoCollection();
        static public TaskManager g_TaskManager = new TaskManager();
        static public AutomationDispatcher g_AutoDispatcher = new AutomationDispatcher();
        static void Main(string[] args)
        {
            string filename = string.Format("Log_{0}_{1:HHmmss}.txt", DateTime.Now.ToString("yyyyMMdd"), DateTime.Now);
            m_log_file = new System.IO.StreamWriter(filename);
            m_log_file.AutoFlush = true;
            AddLog("Start");

            //using (var timer = new System.Threading.Timer(TimerCallback,null,0,500))
            //{
            //    // Rest of code here...
            //}
            Thread BackGroundThread = new Thread(new ThreadStart(BackgroundThreadCallback));
           // BackGroundThread.
            BackGroundThread.Start(); //Start the Thead with the parameter...
            while (!BackGroundThread.IsAlive) ;
            System.Timers.Timer t = new System.Timers.Timer();
            t.Elapsed += new System.Timers.ElapsedEventHandler(TimerCallback);
            t.Interval = 200;
            t.Enabled = true;
            FTPService ftpServer = new FTPService();
            ftpServer.Init();
            g_console_server = new ConsoleServer();
            g_MsgReceiver.Init();
            g_MsgSender.Init();

            videoCollectionFunc.Init();
            videoCollectionFunc.CheckVideoPath();
            videoCollectionFunc.SetWatcher();

            g_console_server.Start_ConsoleServer();
            g_console_server.LoadSimulatedData();

            g_AutoDispatcher.Load();
            if (args.Length < 1)
            {
                string inStr;
                int inValue = 1;
                do
                {
                    Thread.Sleep(1);
                    Console.WriteLine("Input: <Quit/q>");
                    inStr = Console.ReadLine();
                    try
                    {
                        inValue = ParseInput(inStr);
                    }
                    catch (OverflowException e)
                    {
                        Console.WriteLine("Error:{0}", e.Message);
                    }
                } while (inValue != 0);

                BackGroundThread.Abort();
                m_log_file.Close();
                return;
            }

        }
        private static void listClients()
        {
            int nbSocket = Program.g_console_server.m_socket_manager.GetSocketCount();
            for(int a=0;a<nbSocket;a++)
            {
                string socketDesc = Program.g_console_server.m_socket_manager.GetSocketDesc(a);
                Console.WriteLine("Socket Desc: {0}", socketDesc);
            }
        }
        private static void BackgroundThreadCallback()
        {
            int inValue = 1;
            do
            {
                Thread.Sleep(1);

                g_TaskManager.Run();
                g_AutoDispatcher.Tick();

            } while (inValue != 0);
            //GC.Collect();
        }
        private static void TimerCallback(Object o, System.Timers.ElapsedEventArgs e)
        {
            Thread.Sleep(1);
            //g_TaskManager.Run();
            //g_AutoDispatcher.Tick();
            //GC.Collect();
        }
    }
}
