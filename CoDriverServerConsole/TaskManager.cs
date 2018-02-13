using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using System.Diagnostics;

namespace CoDriverServerConsole
{
    class TaskManager
    {
        public struct process_info
        {
            public int idx;
            public int itemIdx;
            public string desc;
            public DateTime startTime;
            public TimeSpan stayTime;
            public Process m_process;
            public int state { get; set; }
            public Queue<process_task> task_queue;// = new Queue<process_task>();
            public void UpdateStayTime()
            {
                stayTime = DateTime.Now - startTime;
            }
            public void SetState(int s)
            {
                state = s;
            }

            public string ToDescString()
            {
                stayTime = DateTime.Now - startTime;
                return string.Format("{0} {1}. {2} {3:hh\\:mm\\:ss}", idx, desc, state, stayTime);
            }

        }
        Dictionary<int, ProcessTask> m_process_task = new Dictionary<int, ProcessTask>();

        Dictionary<int, process_info> m_processList = new Dictionary<int, process_info>();

       // public ListBox listBox_Process;
        static int count = 0;
        static public int processState;

        static void process_Exited(object sender, EventArgs e)
        {
            processState = 0;
        }
        void process_ReceiveOutput(object sender, DataReceivedEventArgs e)
        {
            //processState = 0;
            //string aa;
            Program.AddLog(e.Data);
            //logList.Items.Add(e.Data);
        }
        public void RunCommandLine_Task(string typeStr, string app_name, string command_arguments)
        {
            ProcessTask pt = new ProcessTask();
            count++;
            pt.idx = count;
            pt.desc = typeStr;
            pt.startTime = DateTime.Now;
            pt.AddTask(typeStr, app_name, command_arguments);
            pt.itemIdx = 0;// listBox_Process.Items.Add(pt.ToDescString());
            m_process_task.Add(pt.idx, pt);

            Program.AddCommandLog(typeStr, app_name, command_arguments);
        }

        public int RunCommandLine_MultiTask(process_task[] tasks)
        {
            ProcessTask pt = new ProcessTask();
            count++;
            pt.idx = count;
            pt.startTime = DateTime.Now;
            foreach (var t in tasks)
            {
                pt.AddTask(t.task_desc, t.app_name, t.app_args);
                string typeStr = "MultiTask-";
                typeStr += t.task_desc;
                Program.AddCommandLog(typeStr, t.app_name, t.app_args);
            }
            pt.itemIdx = 0;// listBox_Process.Items.Add(pt.ToDescString());
            m_process_task.Add(pt.idx, pt);
            return pt.idx;
        }

        public void RunCommandLine(string typeStr, string app_name, string command_arguments)
        {
            process_info pi = new process_info();
            count++;
            pi.idx = count;
            pi.desc = typeStr;
            pi.startTime = DateTime.Now;
            pi.m_process = new Process();
            pi.m_process.StartInfo.FileName = app_name;// "ipconfig.exe";
            pi.m_process.StartInfo.UseShellExecute = false;
            pi.m_process.StartInfo.RedirectStandardOutput = true;
            pi.m_process.StartInfo.Arguments = command_arguments;
            //m_ue_process = Process.Start(app_name, command_arguments);
            pi.m_process.Exited += process_Exited;
            pi.m_process.OutputDataReceived += process_ReceiveOutput;
            pi.m_process.Start();
            pi.m_process.BeginOutputReadLine();
            pi.SetState(1);
            pi.UpdateStayTime();
            pi.itemIdx = 0;// listBox_Process.Items.Add(pi.ToDescString());
            m_processList.Add(pi.idx, pi);
            //m_process.WaitForExit();
            //m_process.Close();
            //pi.desc = string.Format("{0} Deploy Scene. {1} {2:hh\\:mm\\:ss}", pi.idx, pi.state,pi.stayTime);

            Program.AddCommandLog(typeStr, app_name, command_arguments);
        }

        public void Run()
        {
            foreach (var p in m_process_task.Keys.ToList())
            {
                ProcessTask pt = m_process_task[p];
                pt.Run();
                m_process_task[p] = pt;
                //listBox_Process.Items[p.Value.itemIdx] = p.Value.ToDescString();
            }
            bool isTaskBusy = false;
            foreach (var p in m_process_task)
            {
                if (!p.Value.isDone)
                    isTaskBusy = true;
            }
            if (!isTaskBusy)
                m_process_task.Clear();


            foreach (var p in m_processList.Keys.ToList())
            {
                process_info pi = m_processList[p];
                if (pi.state > 0)
                {
                    pi.m_process.Refresh();
                    if (pi.m_process.HasExited)
                    {
                        pi.SetState(0);
                    }
                    //p.Value.UpdateStayTime();
                    //listBox_Process.Items[pi.itemIdx] = pi.ToDescString();
                    m_processList[p] = pi;
                }
            }
            bool isIdle = true;
            foreach (var p in m_processList)
            {
                if (p.Value.state != 0)
                    isIdle = false;
            }
            if (isIdle)
                m_processList.Clear();

            //while (processState > 0)
            //{
            //    //Program.AddLog("..Refresh.." + DateTime.Now.ToString());
            //    m_process.Refresh();

            //    //m_process.CancelOutputRead();
            //    //m_process.BeginOutputReadLine();
            //    if (m_process.HasExited)
            //    {
            //        processState = 0;
            //    }
            //}
            //Program.AddLog("Command Line Process has exited.");
        }
    }
}
