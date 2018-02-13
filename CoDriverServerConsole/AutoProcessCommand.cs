using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace CoDriverServerConsole
{
    class AutoProcessCommand
    {
        static string openCV_app_name = "OpenCV\\OpenCVConsoleApp.exe";
        static string console_app_name = "Console\\CoDriverConsoleApp.exe";
        static public int ExtractImages(string videoFilename,string keyword)
        {
            if (videoFilename.Length < 3)
                return -1;
            string output_folder = Path.GetDirectoryName(videoFilename);
            string filename = Path.GetFileNameWithoutExtension(videoFilename);
            output_folder = Path.Combine(output_folder, filename);
            // output_folder += "\\";
            string app_name = "FFmpeg\\ffmpeg.exe";

            if (!Directory.Exists(output_folder))
            {
                Directory.CreateDirectory(output_folder);
            }
            process_task[] tasks = new process_task[3];
            tasks[0].task_desc = "ExtractImages";
            tasks[0].app_name = app_name;
            tasks[0].app_args = BuildCommandArguments.build_args_ffmpeg(videoFilename, output_folder, keyword);

            tasks[1].task_desc = "ExtractGPSData";
            tasks[1].app_name = openCV_app_name;
            tasks[1].app_args = BuildCommandArguments.build_args_extract_gps_data(videoFilename, output_folder, filename);

            tasks[2].task_desc = "AddGPSToImage";
            tasks[2].app_name = console_app_name;
            tasks[2].app_args = BuildCommandArguments.build_args_build_metadata(output_folder, true);


            return Program.g_TaskManager.RunCommandLine_MultiTask(tasks);
        }
    }
}
