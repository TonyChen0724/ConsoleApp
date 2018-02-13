using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;

namespace CoDriverServerConsole
{
    public struct process_task
    {
        public int idx;
        public string task_desc;
        public string app_name;
        public string app_args;
        public string GetDescString()
        {
            return task_desc + "-" + app_name + "-" + app_args;
        }
    }
    
    class ProcessTask
    {
        static int g_process_index = 0;
        public int idx;
        public int itemIdx;
        public string desc;
        public bool isDone;
        public DateTime startTime;
        public TimeSpan stayTime;
        Process m_process;
        public int state { get; set; }
        //public Queue<process_task> task_queue;// = new Queue<process_task>();
        int task_idx = 0;
        Queue<process_task> task_queue = new Queue<process_task>();

        public ProcessTask()
        {
            g_process_index++;
            idx = g_process_index;
            state = 0;
            isDone = false;
            startTime = DateTime.Now;
        }

        public void AddTask(string typeStr, string app_name, string command_arguments)
        {
            process_task pt = new process_task();
            task_idx++;
            pt.idx = task_idx;
            pt.task_desc = typeStr;
            pt.app_name = app_name;
            pt.app_args = command_arguments;
            task_queue.Enqueue(pt);
        }

        bool RequestTask()
        {
            if (task_queue.Count == 0)
                return false;
            process_task newTask = task_queue.Dequeue();
           // pt.idx = count;
           // pt.desc = typeStr;
           // pt.startTime = DateTime.Now;
            desc = newTask.task_desc;
            //m_process.Close();

            m_process = new Process();
            m_process.StartInfo.FileName = newTask.app_name;// "ipconfig.exe";
            m_process.StartInfo.UseShellExecute = false;
            m_process.StartInfo.RedirectStandardOutput = true;
            m_process.StartInfo.Arguments = newTask.app_args;
            //m_ue_process = Process.Start(app_name, command_arguments);
            m_process.Exited += process_Exited;
            m_process.OutputDataReceived += process_ReceiveOutput;
            m_process.Start();
            m_process.BeginOutputReadLine();
            SetState(1);
            UpdateStayTime();
            return true;
        }

        void process_Exited(object sender, EventArgs e)
        {
            SetState(0);
        }
        
        void process_ReceiveOutput(object sender, DataReceivedEventArgs e)
        {
            //processState = 0;
            //string aa;
            Program.AddLog(e.Data);
            //logList.Items.Add(e.Data);
        }

        public void Run()
        {
            if (isDone)
                return;

            if (state == 0 )
            {
                if(!RequestTask())
                {
                    isDone = true;
                }
            }
            if (state > 0)
            {
                m_process.Refresh();
                if (m_process.HasExited)
                {
                    SetState(0);
                }
            }
            UpdateStayTime();
            return;
        }
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
}
