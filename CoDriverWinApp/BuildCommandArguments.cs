using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;

namespace CoDriverWinApp
{
    public class BuildCommandArguments
    {
        static public void InitFFMpegCommandLine()
        {
            string ffmpeg_command_file = Directory.GetCurrentDirectory() + "\\FFmpeg\\sample.txt";
            StreamReader reader = new StreamReader(ffmpeg_command_file);
            ffmpeg_command = reader.ReadToEnd();
            reader.Close();
        }
        static string ffmpeg_command;
        static public string MakePathForCommandLine(string inPath)
        {
            return "\"" + inPath + "\" ";
        }

        static public string build_args_deploy_scene(string sceneName,string folder)
        {
            string command_arguments = "2 ";
            command_arguments += sceneName;
            command_arguments += " ";
            command_arguments += MakePathForCommandLine(folder);
            return command_arguments;
        }
        static public string build_args_import_images(string sceneName, string folder)
        {
            string command_arguments = "3 ";
            command_arguments += sceneName;
            command_arguments += " ";
            command_arguments += MakePathForCommandLine(folder);
            return command_arguments;
        }

        static public string build_args_package(string sceneName, string folder)
        {
            string command_arguments = "4 ";
            command_arguments += sceneName;
            command_arguments += " ";
            command_arguments += MakePathForCommandLine(folder);
            return command_arguments;
        }

        static public string build_args_build_derived()
        {
            string command_arguments = "6 ";
            return command_arguments;
        }
        static public string build_args_build_metadata(string folder,bool isXMP)
        {
            string command_arguments = "7 ";
            command_arguments += "\"";
            command_arguments += folder;
            command_arguments += "\"";
            if (isXMP)
            {
                command_arguments += " 1 0";
            }
            return command_arguments;
        }
        static public string build_args_backup_folder(string folder)
        {
            string command_arguments = "9 " + MakePathForCommandLine(folder);
            return command_arguments;
        }
        static public string build_args_undistort(string folder, CamConfig camConfig,string focalLen,bool isBackup)
        {
            string command_arguments = MakePathForCommandLine(folder);
            command_arguments += camConfig.focallen;
            command_arguments += " ";
            command_arguments += camConfig.r1;
            command_arguments += " ";
            command_arguments += camConfig.r2;
            command_arguments += " ";
            command_arguments += camConfig.r3;
            command_arguments += " ";
            command_arguments += camConfig.ppx;
            command_arguments += " ";
            command_arguments += camConfig.ppy;
            command_arguments += " ";
            command_arguments += focalLen;
            if(isBackup)
                command_arguments += " 1";
            else
                command_arguments += " 0";
            return command_arguments;
        }
        static public string build_args_extract_gps_data(string srcFile, string outputFolder, string filename)
        {
            string command_arguments = "10 ";
            command_arguments += MakePathForCommandLine(srcFile);
            command_arguments += MakePathForCommandLine(outputFolder + "\\" + filename + "_gps5.txt"); ;
            return command_arguments;
        }
        static public string build_args_ffmpeg(string file,string outputFolder,string keyword)
        {
            string command_arguments = "-i ";
            command_arguments += MakePathForCommandLine(file);
            command_arguments += " ";
            command_arguments += ffmpeg_command;// " -r 30 -q:v 1 -qscale:v 2 -qmin 1 -qmax 1 -qcomp 0 -qblur 0 ";
            command_arguments += " ";
            command_arguments += MakePathForCommandLine(outputFolder + "\\" + keyword + "_%06d.jpg");
            return command_arguments;
        }

        static public string build_args_cubemap(string file, string outputFolder, string keyword)
        {
            string command_arguments = "11 ";
            command_arguments += MakePathForCommandLine(file);
            command_arguments += " ";
            command_arguments += ffmpeg_command;// " -r 30 -q:v 1 -qscale:v 2 -qmin 1 -qmax 1 -qcomp 0 -qblur 0 ";
            command_arguments += " ";
            command_arguments += MakePathForCommandLine(outputFolder + "\\" + keyword + "_%06d.jpg");
            return command_arguments;
        }
    }
}
