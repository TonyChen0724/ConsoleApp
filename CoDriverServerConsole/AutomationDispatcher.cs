using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using EpServerEngine.cs;

namespace CoDriverServerConsole
{
    enum AutoTaskStep
    {
        ATS_Idle = 0,
        ATS_upload_video,
        ATS_extract_images, //gps data && kml && xmp
        ATS_CR_FirstAlignment,
       

    }
    struct AutoTaskDesc
    {
        public string taskName;
        public AutoTaskStep taskStep;
        public string output;
        public string input;
        public double delayTime;
        public int autoStart;

    }
    struct TaskData
    {
        public int index;
        public AutoTaskStep step;
        public string taskName;
        public int taskState;
        public int processState;
        public string param1;
        public string param2;
    }
    class AutomationTermination
    {
        public int type; //0 - self server, 1 - CPU pc, 2 - GPS pc , 3 - client
        INetworkSocket socket;
        AutoTaskStep currentStep;
    }

    class AutomationDispatcher
    {
        Dictionary<int, AutomationTermination> ProcessPCLibs = new Dictionary<int, AutomationTermination>();
        int number_terminations = 0;
        Dictionary<string, AutoTaskDesc> TaskConfiguration = new Dictionary<string, AutoTaskDesc>();
        Queue<TaskData> task_waitting_queue = new Queue<TaskData>();
        List<TaskData> task_active_list = new List<TaskData>();
        int number_task_total = 0;
        public AutomationDispatcher()
        {
            AutomationTermination ter = new AutomationTermination();
            ter.type = 0;
            ProcessPCLibs.Add(0, ter);
            number_terminations = 1;
        }
        public void Load()
        {
            StreamReader reader = new StreamReader("TaskConfig.ini");
            int count = 0;
            string taskName = "";
            char[] charSeparators = new char[] { '=', ';', ',' };
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                if (line.Length <= 3)
                    continue;
                if (line[0] == ';')
                    continue;
                if (line[0] == '/'
                    && line[1] == '/')
                    continue;
                if (line[0] == '['
                    && line[line.Length - 1] == ']')
                {
                    taskName = line.Substring(1, line.Length - 2);
                    AutoTaskDesc taskDesc = new AutoTaskDesc();
                    taskDesc.taskName = taskName;
                    TaskConfiguration.Add(taskName, taskDesc);
                    continue;
                }
                if (taskName.Length < 2)
                    continue;
                var values = line.Split(charSeparators, StringSplitOptions.RemoveEmptyEntries);
                if (values.Length < 2)
                    continue;
                AutoTaskDesc curTaskDesc = TaskConfiguration[taskName];

                if (values[0] == "Output")
                    curTaskDesc.output = values[1];

                if (values[0] == "Input")
                    curTaskDesc.input = values[1];

                if (values[0] == "delayTime")
                    curTaskDesc.delayTime = Convert.ToDouble(values[1]);
                if (values[0] == "AutoStart")
                    curTaskDesc.autoStart = Convert.ToInt32(values[1]);

                TaskConfiguration[taskName] = curTaskDesc;
                count++;
            }
            reader.Close();
            return;
        }
        public void AddTask(AutoTaskStep taskStep ,string srcName)
        {
            number_task_total++;
            TaskData taskData = new TaskData();
            taskData.taskName = Convert.ToString(taskStep);
            taskData.step = taskStep;
            taskData.taskState = 0;
            taskData.processState = 0;
            taskData.index = number_task_total;
            taskData.param1 = srcName;
            taskData.param2 = "";
            task_waitting_queue.Enqueue(taskData);

            string logStr = string.Format("Task Add: {0} {1} {2} {3}", taskData.index, taskData.taskName, taskData.param1, taskData.param2);
            Program.AddLog(logStr);
        }
        private int TriggerTask(TaskData td)
        {
            if(td.step == AutoTaskStep.ATS_extract_images)
            {
                return AutoProcessCommand.ExtractImages(td.param1, td.param2);
            }
            return 1;
        }
        public void Tick()
        {
            foreach(var task in TaskConfiguration)
            {
                if(task.Value.autoStart == 1)
                {

                }
            }
            
            //Trigger task
            if(task_active_list.Count < 2 && task_waitting_queue.Count > 0)
            {
                TaskData td = task_waitting_queue.Dequeue();
                td.index = TriggerTask(td);
                td.processState = 1;
                task_active_list.Add(td);
                string logStr = string.Format("Task Start: {0} {1} {2} {3}", td.index, td.taskName, td.param1, td.param2);
                Program.AddLog(logStr);
            }


            //Remove task from active list
            foreach (var node in task_active_list)
            {
                if (node.processState == 0)
                {
                    if(node.step == AutoTaskStep.ATS_extract_images)
                    {
                        Program.videoCollectionFunc.CheckVideoFile(node.param1);
                    }
                    task_active_list.Remove(node);
                    string logStr = string.Format("Task End: {0} {1} {2} {3}", node.index, node.taskName, node.param1, node.param2);
                    Program.AddLog(logStr);
                    break;
                }
            }
            return;
        }
    }
}
