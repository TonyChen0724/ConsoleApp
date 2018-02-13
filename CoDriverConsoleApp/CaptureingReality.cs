using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Diagnostics;
using System.IO;

namespace CoDriverConsoleApp
{
    class CaptureingReality
    {
        public class CRProjInfo
        {
            public Dictionary<int,string> imageList = new Dictionary<int, string>();
            public Dictionary<int, string> leftList = new Dictionary<int, string>();
            public Dictionary<int, string> rightList = new Dictionary<int, string>();
            public Dictionary<int, string> backList = new Dictionary<int, string>();
            public int startIdx;
            public int endIdx;
            public string proj_name;
        }
        public string savePrjCommand = "-save";
        public string loadPrjCommand = "-load";
        public string quitPrjCommand = "-quit";

        public string newScenePrjCommand = "-newScene";
        public string addFolderPrjCommand = "-addFolder";
        public string addImageCommand = "-add";


        public string alignmentAlignCommand  = "-align";
        public string draftAlignCommand = "-draft";
        public string updateAlignCommand = "-update";

        
        public string mvsRecCommand = "-mvs";
        public string regionAutoRecCommand = "-setReconstructionRegionAuto";
        
        public string simplifyModelRecCommand = "-simplify";
        public string exportModelRecCommand = "-exportModel";

        public string setParamCommand = "-set";
        public string disableOnlineCommand = "-disableOnlineCommunication";

        public string selectMaximalCompCommand = "-selectMaximalComponent";
        public string minCompSizeCommand = "-minComponentSize";
        public string exportComponentCommand = "-exportComponent";

        public string importGlobalSettingsCommand = "-importGlobalSettings";


        public string app_name = "RealityCapture.exe";
        public string project_folder = "";
        public string working_folder = "";
        
        public void Init()
        {
            app_name = Settings1.Default.CapturingReality + "RealityCapture.exe";
            project_folder = Settings1.Default.CRProject;
            return;
        }
        void RunBuildProject(string[] args)
        {
            app_name = @"C:\Program Files\Capturing Reality\RealityCapture\RealityCapture.exe";
            //RealityCapture.exe -load C:\MyFolder\PlainProject.rcproj - addFolder C:\MyFolder\Images\ -save C:\MyFolder\MyProject.rcproj - quit
            Process cmd = new Process();
            cmd.StartInfo.FileName = app_name;// Directory.GetCurrentDirectory() + "\\exiftool.exe";

            cmd.StartInfo.RedirectStandardInput = true;
            cmd.StartInfo.RedirectStandardOutput = true;
            cmd.StartInfo.CreateNoWindow = true;
            cmd.StartInfo.UseShellExecute = false;

            string tm_folder = string.Format("{0}_{1:HHmmss}", DateTime.Now.ToString("yyyyMMdd"), DateTime.Now);

            string output_folder = Path.Combine(project_folder, tm_folder);
            if (!Directory.Exists(output_folder))
                Directory.CreateDirectory(output_folder);

            string imageFolder = args[2];
            string prjName = Path.GetFileName(imageFolder) + ".rcproj";
            string output_project = Path.Combine(output_folder, prjName);

            string outputPlace = @"D:\testGroundVU\Front\proj";

            string arg= newScenePrjCommand;// = newScenePrjCommand;
            arg += " ";
            arg += addFolderPrjCommand;
            arg += " ";
            arg += imageFolder;
            arg += "\\ ";
            arg += savePrjCommand;
            arg += " ";
            arg += outputPlace;
            arg += " ";
            arg += quitPrjCommand;

            cmd.StartInfo.Arguments = arg;
            cmd.Start();
            cmd.StandardInput.Flush();
            cmd.StandardInput.Close();
            cmd.WaitForExit();
            Program.AddLog("CR-RunBuildProject " + arg);
            Program.AddLog(cmd.StandardOutput.ReadToEnd());
            return;
        }

        void BuildNewProject(string proj_filename,string sampleImage)
        {
            //RealityCapture.exe -load C:\MyFolder\PlainProject.rcproj - addFolder C:\MyFolder\Images\ -save C:\MyFolder\MyProject.rcproj - quit
            Process cmd = new Process();
            cmd.StartInfo.FileName = app_name;// Directory.GetCurrentDirectory() + "\\exiftool.exe";

            cmd.StartInfo.RedirectStandardInput = true;
            cmd.StartInfo.RedirectStandardOutput = true;
            cmd.StartInfo.CreateNoWindow = true;
            cmd.StartInfo.UseShellExecute = false;


            string output_project = proj_filename;// Path.Combine(output_folder, prjName);

            string arg = "";// = newScenePrjCommand;
            //arg += " ";
            arg += addImageCommand;
            arg += " \"";
            arg += sampleImage;
            arg += "\" ";
            arg += savePrjCommand;
            arg += " ";
            arg += output_project;
            arg += " ";
            arg += quitPrjCommand;

            cmd.StartInfo.Arguments = arg;
            cmd.Start();
            cmd.StandardInput.Flush();
            cmd.StandardInput.Close();
            cmd.WaitForExit();
            Program.AddLog("CR-RunBuildProject " + arg);
            Program.AddLog(cmd.StandardOutput.ReadToEnd());
            return;
        }


       

        void RunBuildProjectAndAlign(string[] args, int alignMode)
        {
            //RealityCapture.exe -load C:\MyFolder\PlainProject.rcproj - addFolder C:\MyFolder\Images\ -save C:\MyFolder\MyProject.rcproj - quit
            Process cmd = new Process();
            cmd.StartInfo.FileName = app_name;// Directory.GetCurrentDirectory() + "\\exiftool.exe";

            cmd.StartInfo.RedirectStandardInput = true;
            cmd.StartInfo.RedirectStandardOutput = true;
            cmd.StartInfo.CreateNoWindow = true;
            cmd.StartInfo.UseShellExecute = false;

            string tm_folder = string.Format("{0}_{1:HHmmss}", DateTime.Now.ToString("yyyyMMdd"), DateTime.Now);

            string output_folder = Path.Combine(project_folder, tm_folder);
            if (!Directory.Exists(output_folder))
                Directory.CreateDirectory(output_folder);

            string imageFolder = args[2];
            string prjName = Path.GetFileName(imageFolder) + ".rcproj";
            string output_project = Path.Combine(output_folder, prjName);

            string arg = newScenePrjCommand;
            arg += " ";
            arg += addFolderPrjCommand;
            arg += " ";
            arg += imageFolder;
            arg += "\\ ";
            if(alignMode == 1)
            {
                arg += alignmentAlignCommand;
                arg += " ";
            }
            if (alignMode == 2)
            {
                arg += draftAlignCommand;
                arg += " ";
            }

            arg += savePrjCommand;
            arg += " ";
            arg += output_project;
            arg += " ";
            arg += quitPrjCommand;

            cmd.StartInfo.Arguments = arg;
            cmd.Start();
            cmd.StandardInput.Flush();
            cmd.StandardInput.Close();
            cmd.WaitForExit();
            Program.AddLog("CR-RunBuildProject-Alignment " + arg);
            Program.AddLog(cmd.StandardOutput.ReadToEnd());
            return;
        }


        void createMesh(string[] args)
        {
            //RealityCapture.exe -load C:\MyFolder\PlainProject.rcproj - addFolder C:\MyFolder\Images\ -save C:\MyFolder\MyProject.rcproj - quit
            Process cmd = new Process();
            cmd.StartInfo.FileName = app_name;// Directory.GetCurrentDirectory() + "\\exiftool.exe";

            cmd.StartInfo.RedirectStandardInput = true;
            cmd.StartInfo.RedirectStandardOutput = true;
            cmd.StartInfo.CreateNoWindow = true;
            cmd.StartInfo.UseShellExecute = false;

            string tm_folder = string.Format("{0}_{1:HHmmss}", DateTime.Now.ToString("yyyyMMdd"), DateTime.Now);

            string output_folder = Path.Combine(project_folder, tm_folder);
            if (!Directory.Exists(output_folder))
                Directory.CreateDirectory(output_folder);

            string projFile = args[2];
            string globalSettingFile = args[1];
            int OptionNumber = Int32.Parse(args[3]);
            string triangleCount = "";

            if (OptionNumber == 3)
            {
                triangleCount = args[4];

            }

            string arg = loadPrjCommand;
            arg += " ";
            arg += projFile;
            arg += " ";

            arg += importGlobalSettingsCommand;
            arg += " ";
            arg += globalSettingFile;
            arg += " ";
            

            if (OptionNumber == 0)
            {
                arg += mvsRecCommand;
                arg += " ";
            }

            if (OptionNumber == 1)
            {
                arg += alignmentAlignCommand;
                arg += " ";
            }

            if (OptionNumber == 2)
            {
                arg += alignmentAlignCommand;
                arg += " ";
                arg += mvsRecCommand;
                arg += " ";
            }



            if (OptionNumber == 3)
            {
                arg += alignmentAlignCommand;
                arg += " ";
                arg += mvsRecCommand;
                arg += " ";
                arg += simplifyModelRecCommand;
                arg += " ";
                arg += triangleCount;
            }


            
            


           

            



            arg += savePrjCommand;
            arg += " ";
            //arg += output_project;
            arg += " ";
            arg += quitPrjCommand;

            cmd.StartInfo.Arguments = arg;
            cmd.Start();
            cmd.StandardInput.Flush();
            cmd.StandardInput.Close();
            cmd.WaitForExit();
            Program.AddLog("CR-RunBuildProject-Alignment " + arg);
            Program.AddLog(cmd.StandardOutput.ReadToEnd());
            return;
        }

        void ExportComponent(string[] args)
        {
            Process cmd = new Process();
            cmd.StartInfo.FileName = app_name;// Directory.GetCurrentDirectory() + "\\exiftool.exe";

            cmd.StartInfo.RedirectStandardInput = true;
            cmd.StartInfo.RedirectStandardOutput = true;
            cmd.StartInfo.CreateNoWindow = true;
            cmd.StartInfo.UseShellExecute = false;

            string project_name = args[2];

            string export_folder = Path.GetDirectoryName(project_name);
            export_folder = Path.Combine(export_folder, "Components");
            if (!Directory.Exists(export_folder))
                Directory.CreateDirectory(export_folder);
            //export_folder += "\\";
            string arg = loadPrjCommand;
            arg += " ";
            arg += project_name;
            arg += " ";
            //arg += minCompSizeCommand;
            //arg += " 5 ";
            arg += selectMaximalCompCommand;
            arg += " ";
            arg += exportComponentCommand;
            arg += " ";
            arg += export_folder;
            arg += " ";
            arg += quitPrjCommand;

            cmd.StartInfo.Arguments = arg;
            cmd.Start();
            cmd.StandardInput.Flush();
            cmd.StandardInput.Close();
            cmd.WaitForExit();
            Program.AddLog("CR-ExportComponent-Min100 " + arg);
            Program.AddLog(cmd.StandardOutput.ReadToEnd());
        }

        public void RunCommandLine(string[] args)
        {
            if (args[1] == "1")
                RunBuildProject(args);
            if (args[1] == "11")
                RunBuildProjectAndAlign(args,1);
            if (args[1] == "12")
                RunBuildProjectAndAlign(args, 2);
            if (args[1] == "2")
                ExportComponent(args);
            return;
        }
        int[] section_info = {1,0, 50, 2500, 0, 0, 0,
                              2,0, 50, 2500, 0, 0, 0,
                              1,1, 50, 625,  625, 625, 625,
                              1,2, 50, 1000, 500, 500, 500,
                              1,3, 50, 1200, 400, 400, 400,
                              1,4, 50, 1428, 357, 357, 357,
                              2,2, 50, 625,  625, 625, 625,
                              2,4, 50, 1000, 500, 500, 500,
                              2,6, 50, 1200, 400, 400, 400,
                              2,8, 50, 1428, 357, 357, 357 };

        public void RunCRProjCreation(string[] args)
        {
            string[] imgFolder = new string[4];
            if (!Directory.Exists(args[1]))
            {
                Program.AddLog("RCProjFile: No Image Path." + args[1]);
                return;
            }
            imgFolder[0] = args[1];

            string scene_name = args[6];
            string output_folder = Path.Combine(project_folder, scene_name);
            if (!Directory.Exists(output_folder))
                Directory.CreateDirectory(output_folder);

            int nbSection = Convert.ToInt32(args[5]);
            int nbStep    = section_info[nbSection * 7];
            int nbStep2   = section_info[nbSection * 7 + 1];
            int nbOverlap = section_info[nbSection * 7 + 2];
            int nbFront = section_info[nbSection * 7 + 3];
            int nbLeft  = section_info[nbSection * 7 + 4];
            int nbRight = section_info[nbSection * 7 + 5];
            int nbBack  = section_info[nbSection * 7 + 6];


            ImagesManager imgMgr = new ImagesManager();
            imgMgr.SelectFolder(0, imgFolder[0]);
            if (args[2] == "NULL")
            {
                imgFolder[1] = "";
            }
            else
            {
                if (Directory.Exists(args[2]))
                {
                    imgFolder[1] = args[2];
                    imgMgr.SelectFolder(1, imgFolder[1]);
                }
            }
            if (args[3] == "NULL")
            {
                imgFolder[2] = "";
            }
            else
            {
                if (Directory.Exists(args[3]))
                {
                    imgFolder[2] = args[3];
                    imgMgr.SelectFolder(2, imgFolder[2]);
                }
            }
            if (args[4] == "NULL")
            {
                imgFolder[3] = "";
            }
            else
            {
                if (Directory.Exists(args[4]))
                {
                    imgFolder[3] = args[4];
                    imgMgr.SelectFolder(3, imgFolder[3]);
                }
            }
            

            ImagesInfo imgInfo = imgMgr.folderList[0];
            int nbImgTotal = imgInfo.m_images_list.Count;
            int nbTempImgTotal = nbImgTotal - nbFront * nbStep;
            int nbCRPrj = 1;
            int nbTempCRPrj = (nbTempImgTotal + (nbFront - nbOverlap) * nbStep - 1) / ((nbFront - nbOverlap) * nbStep);
            nbCRPrj += nbTempCRPrj;
            int[] nSec = new int[nbCRPrj];
            CRProjInfo[] projInfo = new CRProjInfo[nbCRPrj];
            for (int a = 0; a < nbCRPrj; a++)
                projInfo[a] = new CRProjInfo();

            string first_img_filename = "";
            //Front
            int maxImg = imgInfo.max_count;
            var result = imgInfo.m_images_list.OrderBy(i =>i.Key);
            int count = 0;
            int countPrj = 0;
            foreach(var node in result)
            {
                if (count >= nbFront)
                {
                    countPrj++;
                    count = nbOverlap;
                }                
                int idx = node.Key;
                int mod = idx % nbStep;
                if (mod != 0)
                    continue;
                if (first_img_filename.Length < 3)
                    first_img_filename = node.Value;
                projInfo[countPrj].imageList.Add(idx,node.Value);
                if (count >= nbFront - nbOverlap
                    && countPrj < nbCRPrj - 1)
                {
                    projInfo[countPrj + 1].imageList.Add(idx,node.Value);
                }
                count++;
            }

            //Left
            if(imgMgr.folderList.ContainsKey(1) && nbLeft > 1)
            {
                ImagesInfo imgInfo1 = imgMgr.folderList[1];
                for (int a=0;a<nbCRPrj;a++)
                {
                    foreach (var node in projInfo[a].imageList)
                    {
                        int frontidx = node.Key;
                        int mod = frontidx % nbStep2;
                        if (mod != 0)
                            continue;
                        if (!imgInfo1.m_images_list.ContainsKey(frontidx))
                            continue;
                        string filename = imgInfo1.m_images_list[frontidx];
                        projInfo[a].leftList.Add(frontidx,filename);
                    }
                }
            }

            //Right
            if (imgMgr.folderList.ContainsKey(2) && nbRight > 1)
            {
                ImagesInfo imgInfo1 = imgMgr.folderList[2];
                for (int a = 0; a < nbCRPrj; a++)
                {
                    foreach (var node in projInfo[a].imageList)
                    {
                        int frontidx = node.Key;
                        int mod = frontidx % nbStep2;
                        if (mod != 0)
                            continue;
                        if (!imgInfo1.m_images_list.ContainsKey(frontidx))
                            continue;
                        string filename = imgInfo1.m_images_list[frontidx];
                        projInfo[a].rightList.Add(frontidx, filename);
                    }
                }
            }
            //Back
            if (imgMgr.folderList.ContainsKey(3) && nbBack > 1)
            {
                ImagesInfo imgInfo1 = imgMgr.folderList[3];
                for (int a = 0; a < nbCRPrj; a++)
                {
                    foreach (var node in projInfo[a].imageList)
                    {
                        int frontidx = node.Key;
                        int mod = frontidx % nbStep2;
                        if (mod != 0)
                            continue;
                        if (!imgInfo1.m_images_list.ContainsKey(frontidx))
                            continue;
                        string filename = imgInfo1.m_images_list[frontidx];
                        projInfo[a].backList.Add(frontidx, filename);
                    }
                }
            }
            
            //
            for (int a = 0; a < nbCRPrj; a++)
            {
                string prjName = string.Format("{0}_{1}.rcproj", scene_name, a);
                string output_project = Path.Combine(output_folder, prjName);
                projInfo[a].proj_name = output_project;
            }

            if (first_img_filename.Length < 3)
                return;
            //send command
            for (int a = 0; a < nbCRPrj; a++)
            {
                BuildNewProject(projInfo[a].proj_name, first_img_filename);
            }
            
            //
            for (int a = 0; a < nbCRPrj; a++)
            {
                RCProjFile projFile = new RCProjFile();
                projFile.LoadXML(projInfo[a].proj_name);
                int nTotal = projInfo[a].imageList.Count;
                nTotal += projInfo[a].leftList.Count;
                nTotal += projInfo[a].rightList.Count;
                nTotal += projInfo[a].backList.Count;
                string[] tempList = new string[nTotal];
                int temp_count = 0;
                foreach (var node in projInfo[a].imageList)
                {
                    tempList[temp_count] = node.Value;
                    temp_count++;
                }
                foreach (var node in projInfo[a].leftList)
                {
                    tempList[temp_count] = node.Value;
                    temp_count++;
                }
                foreach (var node in projInfo[a].rightList)
                {
                    tempList[temp_count] = node.Value;
                    temp_count++;
                }
                foreach (var node in projInfo[a].backList)
                {
                    tempList[temp_count] = node.Value;
                    temp_count++;
                }
                projFile.RemoveImages();
                projFile.AddImages(tempList);

                projFile.SaveXML(projInfo[a].proj_name);
            }
            return;
        }
    }
}
