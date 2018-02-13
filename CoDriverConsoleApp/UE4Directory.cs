using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
namespace CoDriverConsoleApp
{
    class UE4Directory
    {
        static public string project_folder;
        static public string project_content_folder;
        static public string project_name;
        static public string project_save_folder;
        static public string pak_response_file;
        static public string assetRegistryFile;
        string ue4_folder;
        string ue4_engine_folder;
        string[] scenes_folder;
        string[] scenes_name;
        static Process m_ue_process;
        static int processState = 0;
        public void Init()
        {
            project_folder = Settings1.Default.ProjectFolder;
            project_name = Path.Combine(project_folder,Settings1.Default.ProjectName + ".uproject");
            project_content_folder = Path.Combine(project_folder,"Content");
            project_save_folder = Path.Combine(project_folder,"Saved\\Cooked\\WindowsNoEditor");
            ue4_folder = Settings1.Default.UE4Folder;
            ue4_engine_folder = Path.Combine(ue4_folder,"Engine\\Binaries\\Win64");
            string project_scene_folder = Path.Combine(project_content_folder,"Scene");
            scenes_folder = Directory.GetDirectories(project_scene_folder);
            scenes_name = new string[scenes_folder.Length];
            for (int a = 0; a < scenes_folder.Length; a++)
            {
                scenes_name[a] = Path.GetFileName(scenes_folder[a]);
            }

            Program.AddLog("UE4 Folder Init.");
            //var process = System.Diagnostics.Process.GetProcesses();
            //foreach (var p in process)
            //{
            //    Console.WriteLine(p.ToString());
            //}
        }
        public bool CheckSceneName(string str)
        {
            foreach(var node in scenes_name)
            {
                if (node.Contains(str))
                    return false;                    
            }
            return true;
        }
        public bool Command_BuildDerivedData()
        {
            string app_name = Path.Combine(ue4_engine_folder,"UE4Editor-Cmd.exe");
            string command_arguments = project_name + " -run=DerivedDataCache -fill";
            m_ue_process = Process.Start(app_name, command_arguments);
            RegisterProcessExit(m_ue_process);
            Program.AddLog("UE4 Command Started.");
            Program.AddLog(app_name + command_arguments);
            processState = 1;
            return true;
        }

        public bool Command_ImportMesh(string srcFolder, string destFolder)
        {
            string app_name = Path.Combine(ue4_engine_folder,"UE4Editor-Cmd.exe");
            string command_arguments = "-run=ImportAssets -CoDriver -source=";
            command_arguments += srcFolder;
            command_arguments += " -dest=Images -project=";
            command_arguments += destFolder;
            command_arguments += " -importsettings=\"\"";
            
            m_ue_process = Process.Start(app_name, command_arguments);
            RegisterProcessExit(m_ue_process);
            Program.AddLog("UE4 Command Started.");
            Program.AddLog(app_name + command_arguments);
            processState = 1;
            return true;
        }
        public bool Command_ImportImages(string srcFolder,string destFolder)
        {
            string app_name = Path.Combine(ue4_engine_folder,"UE4Editor-Cmd.exe");
            string command_arguments = "-run=ImportAssets -CoDriver -source=";
            command_arguments += srcFolder;
            command_arguments += " -dest=Images -project=";
            command_arguments += destFolder;
            command_arguments += " -importsettings=\"\"";

            m_ue_process = Process.Start(app_name, command_arguments);
            RegisterProcessExit(m_ue_process);
            processState = 1;
            Program.AddLog("UE4 Command Started.");
            Program.AddLog(app_name + command_arguments);
            
            return true;
        }

        public bool Command_CookDirectory(string scene_name)
        {
            string srcFolder = "\\Game\\Scene\\" + scene_name;
            srcFolder += "\\";

            string checkFolder = Path.Combine(project_content_folder,"Scene",scene_name);
            if (!Directory.Exists(checkFolder))
            {
                Program.AddLog("Wrong scene name.");
                return false;
            }
            //D:\DriTest\DriTest.uproject -run=Cook -cookDir=\Game\Scene\RallySample\ -CoDriver -TargetPlatform=WindowsNoEditor -cooksinglepackage -stdout -FORCELOGFLUSH -CrashForUAT -unattended  -UTF8Output
            //UE4Editor-Cmd.exe D:\DriTest\DriTest.uproject -run = Cook -Map = FlorenceHill012017 + Tomahawk + LoginLevel -TargetPlatform=WindowsNoEditor -fileopenlog -unversioned -createreleaseversion=1.000 
            //-abslog="D:\UE Code\UnrealEngine-release-4.14.3\UnrealEngine-release\Engine\Programs\AutomationTool\Saved\Cook-2017.05.30-10.54.33.txt" -stdout -FORCELOGFLUSH -CrashForUAT -unattended -UTF8Output
            string app_name = Path.Combine(ue4_engine_folder, "UE4Editor-Cmd.exe");
            string command_arguments = project_name + " -run=Cook -CoDriver -cookDir=";
            command_arguments += srcFolder;
            command_arguments += " -TargetPlatform=WindowsNoEditor -cooksinglepackage -Compressed -stdout -FORCELOGFLUSH -CrashForUAT -unattended  -UTF8Output";
 
            m_ue_process = Process.Start(app_name, command_arguments);
            RegisterProcessExit(m_ue_process);
            processState = 1;
            Program.AddLog("UE4 Command Started.");
            Program.AddLog(app_name + command_arguments);

            return true;
        }
        public bool Command_Pak(string destPakFile,string pakResponseFile)
        {
            //D:\TestShipping\test.pak -create="D:\DriTest\Saved\Logs\PakList_20170601_101634.txt"  -UTF8Output -multiprocess -patchpaddingalign=2048
            //UnrealPak.exe D:\TestShipping\WindowsNoEditor\DriTest\Content\Paks\DriTest-WindowsNoEditor.pak -create="D:\UE Code\UnrealEngine-release-4.14.3\UnrealEngine-release\Engine\Programs\AutomationTool\Saved\Logs\PakList_DriTest-WindowsNoEditor.txt" 
            //-order=D:\DriTest\Build\WindowsNoEditor\FileOpenOrder\CookerOpenOrder.log -UTF8Output -multiprocess -patchpaddingalign=2048
            string app_name = Path.Combine(ue4_engine_folder,"UnrealPak.exe");
            string command_arguments = destPakFile;
            command_arguments += " -create=\"";
            command_arguments += pakResponseFile;
            command_arguments += "\" -UTF8Output -multiprocess -patchpaddingalign=2048";

            m_ue_process = Process.Start(app_name, command_arguments);
            RegisterProcessExit(m_ue_process);
            processState = 1;
            Program.AddLog("UE4 Command Started.");
            Program.AddLog(app_name + command_arguments);

            return true;
        }

        public static void Run()
        {
            while (processState > 0)
            {
                System.Threading.Thread.Sleep(3000);
                Console.WriteLine("..Refresh.." + DateTime.Now.ToString());
                m_ue_process.Refresh();
                if(m_ue_process.HasExited)
                {
                    processState = 0;
                }
            }
            Program.AddLog("UE4 Process has exited.");
        }
        void RegisterProcessExit(Process process)
        {
            // NOTE there will be a race condition with the caller here
            //   how to fix it is left as an exercise
            process.Exited += process_Exited;
        }

        static void process_Exited(object sender, EventArgs e)
        {
            processState = 0;
        }

        static Dictionary<string, string> UnrealPakResponseFile = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
        private static void WritePakResponseFile(string Filename, Dictionary<string, string> ResponseFile, bool Compressed, bool EncryptIniFiles)
        {
            using (var Writer = new StreamWriter(Filename, false, new System.Text.UTF8Encoding(true)))
            {
                foreach (var Entry in ResponseFile)
                {
                    string Line = string.Format("\"{0}\" \"{1}\"", Entry.Key, Entry.Value);
                    if (Compressed)
                    {
                        Line += " -compress";
                    }

                    if (Path.GetExtension(Entry.Key).Contains(".ini") && EncryptIniFiles)
                    {
                        Line += " -encrypt";
                    }
                    Writer.WriteLine(Line);
                }
                Writer.Close();
            }
        }
        public enum PathSeparator
        {
            Default = 0,
            Slash,
            Backslash,
            Depot,
            Local
        }
        public static char GetPathSeparatorChar(PathSeparator SeparatorType)
        {
            char Separator;
            switch (SeparatorType)
            {
                case PathSeparator.Slash:
                case PathSeparator.Depot:
                    Separator = '/';
                    break;
                case PathSeparator.Backslash:
                    Separator = '\\';
                    break;
                default:
                    Separator = Path.DirectorySeparatorChar;
                    break;
            }
            return Separator;
        }
        public static bool IsPathSeparator(char Character)
        {
            return (Character == '/' || Character == '\\');
        }
        public static string CombinePaths(params string[] Paths)
        {
            return CombinePaths(PathSeparator.Default, Paths);
        }
        public static string CombinePaths(PathSeparator SeparatorType, params string[] Paths)
        {
            // Pick a separator to use.
            var SeparatorToUse = GetPathSeparatorChar(SeparatorType);
            var SeparatorToReplace = SeparatorToUse == '/' ? '\\' : '/';

            // Allocate string builder
            int CombinePathMaxLength = 0;
            foreach (var PathPart in Paths)
            {
                CombinePathMaxLength += (PathPart != null) ? PathPart.Length : 0;
            }
            CombinePathMaxLength += Paths.Length;
            var CombinedPath = new StringBuilder(CombinePathMaxLength);

            // Combine all paths
            CombinedPath.Append(Paths[0]);
            for (int PathIndex = 1; PathIndex < Paths.Length; ++PathIndex)
            {
                var NextPath = Paths[PathIndex];
                if (String.IsNullOrEmpty(NextPath) == false)
                {
                    int NextPathStartIndex = 0;
                    if (CombinedPath.Length != 0)
                    {
                        var LastChar = CombinedPath[CombinedPath.Length - 1];
                        var NextChar = NextPath[0];
                        var IsLastCharPathSeparator = IsPathSeparator(LastChar);
                        var IsNextCharPathSeparator = IsPathSeparator(NextChar);
                        // Check if a separator between paths is required
                        if (!IsLastCharPathSeparator && !IsNextCharPathSeparator)
                        {
                            CombinedPath.Append(SeparatorToUse);
                        }
                        // Check if one of the saprators needs to be skipped.
                        else if (IsLastCharPathSeparator && IsNextCharPathSeparator)
                        {
                            NextPathStartIndex = 1;
                        }
                    }
                    CombinedPath.Append(NextPath, NextPathStartIndex, NextPath.Length - NextPathStartIndex);
                }
            }
            // Make sure there's only one separator type used.
            CombinedPath.Replace(SeparatorToReplace, SeparatorToUse);
            return CombinedPath.ToString();
        }

        //public static void StageLine(string filename)
        //{
        //    filename;
        //    CombinePaths(PathSeparator.Slash, "../../../", Dest);
        //}
        public static void CopyFiles(string pakFile)
        {
            if (assetRegistryFile.Length < 3)
                return;
            string targetFolder = Path.GetDirectoryName(pakFile);
            string pakfilename = Path.GetFileNameWithoutExtension(pakFile);
            string copyAssetRegistryFile = targetFolder + "\\" + pakfilename + "_AssetRegistry.bin";
            File.Copy(assetRegistryFile, copyAssetRegistryFile);
            return;
        }
        public static bool CreatePakResponseFile()
        {
            string filename = string.Format("PakList_{0}_{1:HHmmss}.txt", DateTime.Now.ToString("yyyyMMdd"), DateTime.Now);
            pak_response_file = Path.Combine(project_folder,"Saved\\Logs\\",filename);

            string[] files = Directory.GetFiles(project_save_folder, "*.*", SearchOption.AllDirectories);
            foreach(var file in files)
            {

                if (file.Contains("AssetRegistry.bin"))
                {
                    assetRegistryFile = file;
                    continue;
                }
                var FileToCopy = CombinePaths(file);
                string Dest = FileToCopy;
                //if (!FileToCopy.StartsWith(project_folder))
                //{
                //    //throw new AutomationException("Can't deploy {0}; it was supposed to start with {1}", FileToCopy, InPath);
                //}
                string FileToRemap = FileToCopy;
                if (FileToRemap.StartsWith(project_save_folder, StringComparison.InvariantCultureIgnoreCase))
                {
                    Dest = FileToRemap.Substring(project_save_folder.Length);
                    if (Dest.StartsWith("/") || Dest.StartsWith("\\"))
                    {
                        Dest = Dest.Substring(1);
                    }
                    //Dest = CombinePaths("DriTest", Dest);
                }
                Dest = CombinePaths(PathSeparator.Slash, "../../../", Dest);
                UnrealPakResponseFile[file] = Dest;
            }


            string[] copy_files = Directory.GetFiles(project_save_folder);
            WritePakResponseFile(pak_response_file, UnrealPakResponseFile,false,false);
            return true;
        }
    }
}
