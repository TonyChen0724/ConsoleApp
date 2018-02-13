 using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using Ionic.Zip;
using System.Threading;

namespace CoDriverConsoleApp
{
    class Program
    {
        static Process myProcess = new Process();
        static string xmp_sample_head;
        static string xmp_sample_end;
        //static string section_rotation = "<xcr:Rotation>";
        //static string section_rotation_end = "</xcr:Rotation>";
        //static string section_position = "<xcr:Position>";
        //static string section_position_end = "</xcr:Position>";
        //static string section_distortionCoeficients = "<xcr:DistortionCoeficients>";
        //static string section_distortionCoeficients_end = "</xcr:DistortionCoeficients>";
        static CSVGPSData csv_data = new CSVGPSData();
        static CoConfig configFile = new CoConfig();
        static string select_folder;
        static string select_images_folder;
        static string gps_data_filename = "GoProGPSData.csv";
        static double device_frame_rate;
        static System.IO.StreamWriter m_log_file;
        static string groundVUPlace;
        static string imgLocation;
        static List<string> csvfileNames;
        

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
            if (inStr == "BuildXMP")
                return 1;
            if (inStr == "DeployScene")
                return 2;
            if (inStr == "ImportImages")
                return 3;
            if (inStr == "Cook")
                return 4;

            return 100;
        }
        static void SaveSetting(string[] args)
        {
            if (args.Length < 6)
                return;
            Settings1.Default.ProjectFolder = args[1];
            Settings1.Default.ProjectName = args[2];
            Settings1.Default.UE4Folder = args[3];
            Settings1.Default.CapturingReality = args[4];
            Settings1.Default.CRProject = args[5];
            Settings1.Default.GroundVULocation = args[6];
            Settings1.Default.GVworkingDir = args[7];
           
            Settings1.Default.Save();
            return;
        }

        static public string MakePathForCommandLine(string inPath)
        {
            return "\"" + inPath + "\" ";
        }

        static void RunCapturingReality(string[] args)
        {
            if (args.Length < 2)
                return;
            CaptureingReality cr = new CaptureingReality();
            cr.Init();
            cr.RunCommandLine(args);
            return;
        }

        static void RunCRProjCreation(string[] args)
        {
            if (args.Length < 2)
                return;
            CaptureingReality cr = new CaptureingReality();
            cr.Init();
            cr.RunCRProjCreation(args);
            return;
        }

        static void RunCRMeshCreation(string[] args)
        {
            if (args.Length < 4) 
                return;
            CaptureingReality cr = new CaptureingReality();
            cr.Init();
            cr.RunCRProjCreation(args);
            return;
        }

        static void RunGroundVU(string[] args)
        {
            GroundVU g = new GroundVU();
            g.init(args);
            g.startRunGroundVU();
            
            
        }

        static void ExtendCRProjImages(string[] args)
        {
            string prjFile = args[1];
            string imgFolder = args[2];
            RCProjFile crPrj = new RCProjFile();
            crPrj.LoadXML(prjFile);
            crPrj.AddImages(imgFolder);
            if(args.Length > 3)
            {
                for(int a=3;a<args.Length;a++)
                {
                    string folder = args[a];
                    crPrj.AddImages(folder);
                }
            }
            crPrj.SaveXML(prjFile);
            return;
        }

        static void SelectImageForSections(string[] args)
        {
            string folder = args[1];
            int nbSectinImg = Convert.ToInt32(args[2]);
            int nStep = Convert.ToInt32(args[3]);
            int nCopy = Convert.ToInt32(args[4]);
            ImagesManager.SetupSectionsFolder(folder, nbSectinImg, nStep,nCopy);
            return;
        }
        static void ExtractMultiVideo(string[] args)
        {
            int nbFolder = Convert.ToInt32(args[1]);
            double gapLimit = Convert.ToDouble(args[2]);
            ImagesManager imgMgr = new ImagesManager();
            imgMgr.destFolder = args[3];
            for(int a=0;a<nbFolder;a++)
            {
                imgMgr.SelectFolder(a + 1, args[4 + a]);
            }
            imgMgr.Process();
            imgMgr.BuildTotalImagesFolder(gapLimit);
            imgMgr.BuildKMLFile();
            for (int a = 0; a < nbFolder; a++)
            {
                if (Directory.Exists(args[4 + a]))
                    Directory.Delete(args[4 + a],true);
            }
            return;
        }

        // Tony
        static void ScenePack(string[] args)
        {
            
            if (args.Length < 3)
            {
                Program.AddLog("ScenePack: You don't have enough arguments");
                return;
            }
            string shippingversion = args[1];
            string scenename = args[2];

            if (!Directory.Exists(shippingversion))
            {
                Program.AddLog("ScenePack: shippingversion do not exist");
                return;
            }

            string packDir = Directory.GetParent(shippingversion).ToString();
            string[] filepaths = Directory.GetFiles(shippingversion, "*.*", SearchOption.AllDirectories);
            string[] directories = Directory.GetDirectories(shippingversion, "*", SearchOption.AllDirectories);

            List<string> pakfiles = new List<string>();
            List<string> binfiles = new List<string>();

            int scene_number = args.Length - 2;
            string windowsNoEditor = Path.Combine(shippingversion, "WindowsNoEditor");
            string[] projNameFiles = Directory.GetFiles(windowsNoEditor, "*.exe", SearchOption.TopDirectoryOnly);
            string projName = Path.GetFileName(projNameFiles[0]).Split('.')[0];
            string pakDir = Path.Combine(windowsNoEditor, projName, "Content", "Paks");


            for (int i = 0; i < scene_number; i++)
            {
                pakfiles.Add(Path.Combine(pakDir, args[i + 2] + ".pak"));
                binfiles.Add(Path.Combine(pakDir, args[i + 2] + "_AssetRegistry.bin"));
            }

            using (ZipFile zip = new ZipFile())
            {
                zip.UseZip64WhenSaving = Zip64Option.Always;
                zip.CodecBufferSize = 100000;
                foreach (string dir in directories)
                {
                    zip.AddDirectoryByName(dir);
                }


                
                string template = Path.Combine(pakDir, projName + "-WindowsNoEditor.pak");

                foreach (string file in filepaths)
                {
                   
                    if (!file.Contains("Paks"))
                    {
                        zip.AddFile(file);
                    }
                }

                zip.AddFile(template);

                foreach (string pakfile in pakfiles)
                {
                    zip.AddFile(pakfile);
                }

                foreach (string binfile in binfiles)
                {
                   zip.AddFile(binfile);
                }


               


                string destFile = string.Format("shippingversion_{0}_{1:HHmmss}", DateTime.Now.ToString("yyyyMMdd"), DateTime.Now);
                string destDir = Path.Combine(packDir, destFile);
                zip.Save(destDir);
                
                Program.AddLog("ScenePack: Finished");
                
            }
        }


        /*static void groundVU(string[] args)
           
        {
            if (args.Length < 4)
            {
                Program.AddLog("GroundVU: You don't have enough arguments");
                return;
            }

            string workingDir = Settings1.Default.GVworkingDir;
            string ImgDir = args[1];
            string csvFile = args[2];
            string sceneName = args[3];
            
            if (!Directory.Exists(workingDir))
            {
                Program.AddLog("GroundVU: Working Dir does not exist");
                return;
            }

            if (!Directory.Exists(ImgDir))
            {
                Program.AddLog("GroundVU: Image Dir does not exist");
                return;
            }

            if (!File.Exists(csvFile))
            {
                Program.AddLog("GroundVU: CSV file doesn't exist");
                return;
            }

            if (sceneName.Equals("") || !(System.Text.RegularExpressions.Regex.IsMatch(sceneName, "^[a-zA-Z0-9\x20]+$")) || sceneName.Contains(" "))
            {
                Program.AddLog("GroundVU: sceneName is not right");
                return;
            }

            string projectFolder = Path.Combine(workingDir, sceneName + "_project");

            // create a batch file in workingDir
            string path = Path.Combine(workingDir, sceneName + ".bat");

            csvfileNames = new List<string>();
            var imgfileNames = new List<string>();


           // private string[] pdfFiles = Directory.GetFiles("C:\\Documents", "*.pdf").Select(Path.GetFileName).ToArray();

            using (var rd = new StreamReader(csvFile))
            {
                while (!rd.EndOfStream)
                {
                    var splits = rd.ReadLine().Split(',');
                    if (splits[0].Contains("jpg")) 
                        csvfileNames.Add(splits[0].Split('.')[0]);
                }
            }

            string[] fileArray = Directory.GetFiles(ImgDir);

            for (int i = 0; i < fileArray.Length; i++)
            {
                imgfileNames.Add(Path.GetFileNameWithoutExtension(fileArray[i]));
                
            }

            

            foreach (string str in csvfileNames)
            {
                if (!imgfileNames.Contains(str))
                {
                    Program.AddLog("GroundVU: csv files doesn't match with image files");
                    return;
                }
            }


            using (StreamWriter w = new StreamWriter(path))
            {
                w.WriteLine(@"set GV_PATH =..\GV - 1.2.92");
                w.WriteLine(@"rem copy Y:\GroundVu\GV\gv.exe c:\code\Gv\release\Gv.exe");
                string settingGroundVULocation = Settings1.Default.GroundVULocation;
                settingGroundVULocation.Trim('"');


                w.WriteLine(settingGroundVULocation + @" -params garminParams.json -testRoad garminCalibration.json " + csvFile + " " + ImgDir + " " + projectFolder);

                //w.WriteLine(@"C:\\temp\\framepkg.exe /install=agent /silent");
                w.WriteLine("quit");
                w.Close();
          }
            
            myProcess.StartInfo.WorkingDirectory = workingDir;
            myProcess.StartInfo.FileName = path;
            myProcess.EnableRaisingEvents = true;
            myProcess.Exited += new EventHandler(myProcess_Exited);
            //myProcess.StartInfo.UseShellExecute = true;
            myProcess.Start();

            imgLocation = Path.Combine(ImgDir, sceneName + "List.csv");
           


            Console.ReadLine();




            
      }


        private static void myProcess_Exited(object sender, EventArgs e)
        {


            using (StreamWriter resultFile = new StreamWriter(imgLocation))
            {


                foreach (string str in csvfileNames)
                {
                    resultFile.WriteLine(str + ",");
                }
            }
            Program.AddLog("GroundVU: finished");
        }*/


        static void SendNotification(string[] args)
        {
            string strData = args[2];

            return;
        }
        static void XMPFileGenerator(string[] args)
        {
            if(args[1] == "1")
            {
                XMPGenerator xmpGen = new XMPGenerator();
                xmpGen.BuildXMPDirectory(4, args[2]);
                xmpGen.SetRelativeXMPFile(4, args[3]);
                xmpGen.BuildCalibXMPFile();
                return;
            }
            if (args[1] == "2")
            {
                XMPGenerator xmpGen2 = new XMPGenerator();
                xmpGen2.BuildXMPDirectory(0, args[2]);
                xmpGen2.BuildXMPDirectory(1, args[3]);
                xmpGen2.BuildXMPDirectory(2, args[4]);
                xmpGen2.BuildXMPDirectory(3, args[5]);
                xmpGen2.BuildFixedRelXMPFile(args[6]);
                return;
            }
            if (args[1] == "3")
            {
                Virb360 v360 = new Virb360();
                v360.LoadGPSFile_csv(args[3]);
                //v360.BuildXMPFile_LerpGPSData(args[2]);
                v360.BuildCSVFile_Lerp(args[2]);
                return;
            }
            XMPGenerator xmpGenerator = new XMPGenerator();
            xmpGenerator.BuildXMPDirectory(0, args[1]);
            xmpGenerator.BuildXMPDirectory(1, args[2]);
            xmpGenerator.BuildXMPDirectory(2, args[3]);
            xmpGenerator.BuildXMPDirectory(3, args[4]);
            xmpGenerator.SetRelativeXMPFile(0, args[5]);
            xmpGenerator.SetRelativeXMPFile(1, args[6]);
            xmpGenerator.SetRelativeXMPFile(2, args[7]);
            xmpGenerator.SetRelativeXMPFile(3, args[8]);
            xmpGenerator.BuildRelXMPFile();
            return;
        }
        static void BackupFolder(string[] args)
        {
            if (args.Length < 2)
                return;

            string src_folder = args[1];
            src_folder += "\\";
            string backup_folder = src_folder + "Backup\\";
            if(!Directory.Exists(backup_folder))
            {
                Directory.CreateDirectory(backup_folder);
                Program.AddLog("Create Folder: " + backup_folder);
            }

            string[] fileEntries = Directory.GetFiles(src_folder);
            foreach (string fileName in fileEntries)
            {
                string dest_filename = Path.GetFileName(fileName);
                dest_filename = backup_folder + dest_filename;
                File.Copy(fileName, dest_filename);
                Program.AddLog("Copy To: " + dest_filename);
            }
            //    Program.AddLog("exiftool " + arg);
            //    Program.AddLog(cmd.StandardOutput.ReadToEnd());
            //}
            if (args.Length < 4)
                return;
            int step = Convert.ToInt32(args[2]);
            int isCheckSpd = Convert.ToInt32(args[3]);
            ExifDataWriter exifWriter = new ExifDataWriter();
            AddLog("....Set Folder....");

            exifWriter.SetFolder(args[1]);
            AddLog("....Process....");
            exifWriter.Process();

           // exifWriter.SelectImages(step,Convert.ToBoolean(isCheckSpd));
            return;
        }
        static void ConvertCCXMLFile(string[] args)
        {
            if (args.Length < 2)
                return;
            CCXML cc_xml = new CCXML();
            cc_xml.Load(args[1]);
            cc_xml.Process();
            AddLog("....Convert CC XML File....");
            return;
        }
        static void AddGPSDataToImage(string[] args)
        {
            if (args.Length < 2)
                return;

            //RCProjFile rcProj = new RCProjFile();
            //rcProj.LoadXML("E:\\CRSrcData\\Proj\\testWithXMPPos.rcproj");
            //rcProj.SetGroup();
            //rcProj.SaveXML("E:\\CRSrcData\\Proj\\testWithXMPPos.rcproj");

            ExifDataWriter exifWriter = new ExifDataWriter();
            AddLog("....Set Folder....");

            exifWriter.SetFolder(args[1]);
            if(args.Length >= 3)
            {
                AddLog("....Set Focal Length....");
                float f = (float)Convert.ToDouble(args[2]);
                exifWriter.SetFocalLen(f);
            }
            AddLog("....Process....");
            exifWriter.Process();

            if (args.Length >= 4)
            {
                if(args[3] == "1")
                {
                    AddLog("....Build XMP Files....");
                    exifWriter.BuildXMPFiles();
                    AddLog("....Run Exif Command....");
                    exifWriter.RunExifCommand();
                }
                else
                {
                    AddLog("....Build XMP Files....");
                    exifWriter.BuildXMPFiles();
                }
            }
            else
            {
                AddLog("....Run Exif Command....");
                exifWriter.RunExifCommand();
            }

            AddLog("....Clean Cache....");
            exifWriter.CleanCache(args[1]);
            //UE4Directory ue4Dir = new UE4Directory();
            //ue4Dir.Init();
            //ue4Dir.Command_BuildDerivedData();
            //UE4Directory.Run();
            AddLog("....Add GPS Data To Image....");
            return;
        }
        static void BuildDerivedData()
        {
            UE4Directory ue4Dir = new UE4Directory();
            ue4Dir.Init();
            ue4Dir.Command_BuildDerivedData();
            UE4Directory.Run();
            AddLog("....Build Derived Data Completed....");
            return;
        }
        static void BuildXMPForRotationBias(string[] args)
        {
            string[] fileEntries = Directory.GetFiles(args[1]);
            foreach (string fileName in fileEntries)
            {
                string ext = Path.GetExtension(fileName);
                if (ext == ".xmp")
                {
                    XMPFile xmpFile = new XMPFile();
                    xmpFile.Load(fileName);
                    double[] rot = xmpFile.rotation;
                    double[] new_rot = new double[9];

                    if(args[2] == "0")
                    {
                        //Right hand - 90 right turn
                        //Left hand - 90 left turn
                        new_rot[0] = -1.0 * rot[3];
                        new_rot[1] = -1.0 * rot[4];
                        new_rot[2] = -1.0 * rot[5];

                        new_rot[3] = rot[0];
                        new_rot[4] = rot[1];
                        new_rot[5] = rot[2];

                        new_rot[6] = rot[6];
                        new_rot[7] = rot[7];
                        new_rot[8] = rot[8];
                    }
                    if (args[2] == "1")
                    {
                        //Left hand - 90 right turn
                        //Right hand - 90 left turn
                        new_rot[0] = rot[3];
                        new_rot[1] = rot[4];
                        new_rot[2] = rot[5];

                        new_rot[3] = -1.0 * rot[0];
                        new_rot[4] = -1.0 * rot[1];
                        new_rot[5] = -1.0 * rot[2];

                        new_rot[6] = rot[6];
                        new_rot[7] = rot[7];
                        new_rot[8] = rot[8];
                    }

                    xmpFile.WriteRotation(new_rot);
                    xmpFile.Save();
                }
            }
        }
        static void Cooking(string[] args)
        {
            if (args.Length < 2)
            {
                //4 FlorenceHill012017 D:\TestShipping
                AddLog(string.Format("Invalid arguments! 4 <Scene Name> <Dest Shipping Folder>."));
                return;
            }
            UE4Directory ue4Dir = new UE4Directory();
            ue4Dir.Init();
            string scene_name = args[1];
            string shippingFolder = args[2];
            string dest_pak_file = shippingFolder + "\\WindowsNoEditor\\" + CoDriverConsoleApp.Settings1.Default.ProjectName + "\\Content\\Paks\\";
            if(!Directory.Exists(dest_pak_file))
            {
                AddLog("....Wrong shipping folder....");
                return;
            }
            dest_pak_file += scene_name + ".pak";
            //string destFolder = Path.GetDirectoryName(dest_pak_file);

            AddLog("....Cook Command Started....");
            ue4Dir.Command_CookDirectory(scene_name);
            UE4Directory.Run();
            AddLog("....Cook Command Completed....");

            UE4Directory.CreatePakResponseFile();
            AddLog("....Create Stating Manifest....");


            AddLog("....Package Command Completed....");
            string pakListFile = UE4Directory.pak_response_file;
            ue4Dir.Command_Pak(dest_pak_file, pakListFile);
            UE4Directory.Run();
            UE4Directory.CopyFiles(dest_pak_file);
            AddLog("....Package Command Completed....");
            return;
        }

        static void ImportImages(string[] args)
        {
            if (args.Length < 3)
            {
                AddLog(string.Format("Invalid arguments! 3 <Scene' Name> <Src Folder>."));
                return;
            }
            string scene_name = args[1];
            if (scene_name.Length < 3)
            {
                AddLog(string.Format("Scenes name is wrong! {0}", scene_name));
                return;
            }
            AddLog("-Scene Name: " + scene_name);
            select_folder = args[2];
            if (!Directory.Exists(select_folder))
            {
                AddLog(string.Format("Directory doesn't exist! {0}", select_folder));
                return;
            }
            AddLog("-Source Folder: " + select_folder);
            select_images_folder = select_folder/* + "\\Images\\"*/;
            //if (!Directory.Exists(select_images_folder))
            //{
            //    AddLog(string.Format("Images directory doesn't exist! {0}", select_images_folder));
            //    return;
            //}

            UE4Directory ue4Dir = new UE4Directory();
            ue4Dir.Init();
            string destFolder = UE4Directory.project_content_folder;
            destFolder += "\\Scene\\";
            destFolder += scene_name;
            destFolder += "\\";

            AddLog("....Start import images....");
            AddLog("-Image Folder: " + select_images_folder);
            AddLog("-Dest Folder: " + destFolder);
            ue4Dir.Command_ImportImages(select_images_folder, destFolder);
            UE4Directory.Run();
            AddLog("....End import images....");
            return;
        }

        static void DeployScene(string[] args)
        {
            if (args.Length < 3)
            {
                //2 TestDeploy Front D:\TestGoProGPSData
                AddLog(string.Format("Invalid arguments! 2 <New Scene' Name> <Src Folder>."));
                return;
            }
            AddLog(string.Format("....Check...."));
            string scene_name = args[1];
            if (scene_name.Length < 3)
            {
                AddLog(string.Format("Scenes name is wrong! {0}", scene_name));
                return;
            }
            AddLog("-Scene Name: " + scene_name);
            //string keyword = args[2];
            //AddLog("-Image Keyword: " + keyword);
            select_folder = args[2];
            if (!Directory.Exists(select_folder))
            {
                AddLog(string.Format("Directory doesn't exist! {0}", select_folder));
                return;
            }
            AddLog("-Source Folder: " + select_folder);

            select_images_folder = select_folder + "\\Images\\";// Path.Combine(select_folder,"Images");
            if (!Directory.Exists(select_images_folder))
            {
                AddLog(string.Format("Images directory doesn't exist! {0}", select_images_folder));
                return;
            }
            AddLog("-Source Images: " + select_images_folder);
            string imagelist_filename = select_folder + "\\imagelist.csv";
            if(!File.Exists(imagelist_filename))
            {
                AddLog(string.Format("Image list doesn't exist! {0}", imagelist_filename));
                return;
            }
            AddLog("-Images List: " + imagelist_filename);
            string maya_filename = select_folder + "\\maya_ascii_data.ma";
            if (!File.Exists(maya_filename))
            {
                AddLog(string.Format("Maya ASCII data doesn't exist! {0}", maya_filename));
                return;
            }
            AddLog("-Maya ASCII Data: " + maya_filename);
            string sceneInfo_xml_filename = select_folder + "\\SceneInfo.xml";
            if (!File.Exists(sceneInfo_xml_filename))
            {
                AddLog(string.Format("Scene info xml file doesn't exist! {0}", sceneInfo_xml_filename));
                return;
            }
            AddLog("-Maya ASCII Data: " + sceneInfo_xml_filename);




            select_folder = Path.GetDirectoryName(select_folder);            
            UE4Directory ue4Dir = new UE4Directory();
            ue4Dir.Init();
            if(!ue4Dir.CheckSceneName(scene_name))
            {
                AddLog(string.Format("Scene's content directory alreay exist! {0}", select_folder));
                return;
            }

            SceneInfoData sceneInfoData = new SceneInfoData();
            if(!sceneInfoData.Load(sceneInfo_xml_filename))
            {
                AddLog(string.Format("Failed, load scene info xml file! {0}", sceneInfo_xml_filename));
                return;
            }
            AddLog("-Open SceneInfo: " + sceneInfo_xml_filename);

            ImageList imgList = new ImageList();
            if(!imgList.Load(imagelist_filename))
            {
                AddLog(string.Format("Failed, load images list file! {0}", imagelist_filename));
                return;
            }
            string[] keywordArr = sceneInfoData.keywords;
            foreach(var str in keywordArr)
            {
                if (!imgList.image_info_map.ContainsKey(str))
                {
                    AddLog(string.Format("Failed, Can't find out Keyword in images list file! {0}", imagelist_filename));
                    return;
                }

                ImageList.ImageInfo imgInfo = imgList.image_info_map[str];
                AddLog(string.Format("-Images List: {0} nb:{1} {2}-{3}", imgInfo.keyword, imgInfo.count, imgInfo.idxMin, imgInfo.idxMax));
            }            

            MayaASCIIFile mayaFile = new MayaASCIIFile();
            if(!mayaFile.Load(maya_filename))
            {
                AddLog(string.Format("Failed, load maya ascii data file! {0}", maya_filename));
                return;
            }
            foreach (var str in keywordArr)
            {
                if (!mayaFile.image_info_map.ContainsKey(str))
                {
                    AddLog(string.Format("Failed, Can't find Keyword in maya ascii data! {0}", imagelist_filename));
                    return;
                }
                MayaASCIIFile.ImageInfo mayaInfo = mayaFile.image_info_map[str];
                AddLog(string.Format("-Maya Data: {0} nb:{1}", mayaInfo.keyword, mayaInfo.count));
            }
            string backupFolder = select_folder + "\\Backup\\";
            int matchFile = mayaFile.CheckImagesFolder(select_images_folder, backupFolder);
            if (matchFile <= 1)
            {
                AddLog(string.Format("Failed, Maya ASCII Data doesn't match the images! {0}", imagelist_filename));
                return;
            }
            AddLog(string.Format("-Check Images: {0}", matchFile));


            AddLog(string.Format("....Start Deploy...."));
            string destFolder = string.Format("{0}\\Scene\\{1}\\", UE4Directory.project_content_folder, scene_name);
            
            RoadInfoFile roadinfo_file = new RoadInfoFile();
            string roadinfo_filename = string.Format("{0}\\Scene\\{1}\\{1}.csv", UE4Directory.project_content_folder, scene_name);
            roadinfo_file.BuildRoadInfoFromImageList(imgList, keywordArr, roadinfo_filename);
            AddLog("-Create Road Info File: " + roadinfo_filename);

            string copy_imagelist_file = string.Format("{0}\\Scene\\{1}\\exported.csv", UE4Directory.project_content_folder, scene_name);
            File.Copy(imagelist_filename, copy_imagelist_file);
            AddLog("-Copy Image List File: " + copy_imagelist_file);

            string copy_maya_file = string.Format("{0}\\Scene\\{1}\\maya_ascii_data.ma", UE4Directory.project_content_folder, scene_name);
            File.Copy(maya_filename, copy_maya_file);
            AddLog("-Copy Maya ASCII Data File: " + copy_imagelist_file);

            string copy_sceneinfo_xml_file = string.Format("{0}\\Scene\\{1}\\SceneInfo.xml", UE4Directory.project_content_folder, scene_name);
            File.Copy(sceneInfo_xml_filename, copy_sceneinfo_xml_file);
            AddLog("-Copy Scene Info XML File: " + copy_sceneinfo_xml_file);


            AddLog("....Start import images....");
            AddLog("-Image Folder: " + select_images_folder);
            AddLog("-Dest Folder: " + destFolder);
            ue4Dir.Command_ImportImages(select_images_folder, destFolder);
            UE4Directory.Run();
            AddLog("....End import images....");

            string select_mesh_folder = select_folder + "\\Mesh\\";
            if (Directory.Exists(select_mesh_folder))
            {
                AddLog("....Start import mesh....");
                ue4Dir.Command_ImportMesh(select_mesh_folder, destFolder);
                UE4Directory.Run();
                AddLog("....End import mesh....");
            }
            else
            {
                AddLog(string.Format("No mesh folder! {0}", select_mesh_folder));
            }

            AddLog("....End....");
            
            
        }

        static void BuildXMP(string[] args)
        {
            if(args.Length < 3)
            {
                AddLog(string.Format("Invalid arguments! 1 <Folder> <FrameRate>."));
                return;
            }
            select_folder = args[1];
            device_frame_rate = Convert.ToDouble(args[2]);

            bool is_filter = false;
            for (int a = 2; a < args.Length; a++)
            {
                if (args[a] == "-f")
                    is_filter = true;
            }

            if (!Directory.Exists(select_folder))
            {
                AddLog(string.Format("Directory doesn't exist! {0}", select_folder));
            }
            select_images_folder = select_folder + "\\Images\\";
            if (!Directory.Exists(select_images_folder))
            {
                AddLog(string.Format("Images directory doesn't exist! {0}", select_images_folder));
            }

            //XMPFile xmpFile = new XMPFile();
            //xmpFile.LoadXML("Sample.xmp");
            //xmpFile.SaveXML();

            //select_folder = select_folder;
            AddLog("Initial parameters...");
            StreamReader xmp_head = new StreamReader("xmp_sample_start.txt");
            StreamReader xmp_body = new StreamReader("xmp_sample_end.txt");
            xmp_sample_head = xmp_head.ReadToEnd();
            xmp_sample_end = xmp_body.ReadToEnd();

            AddLog("Load gps data...Start.");
            csv_data.Load(select_folder + gps_data_filename);
            AddLog(string.Format("Load gps data...End. {0} Estimate time:{1}", csv_data.number, csv_data.total_tm));


            string[] fileEntries = Directory.GetFiles(select_images_folder);
            foreach (string fileName in fileEntries)
            {
                string ext = Path.GetExtension(fileName);
                if (ext == ".xmp")
                {
                    File.Delete(fileName);
                }
            }

            string[] imagesFileEntries = Directory.GetFiles(select_images_folder);
            AddLog(string.Format("Prepare Images...{0}", imagesFileEntries.Length));
            Array.Sort(imagesFileEntries);


            AddLog("Build xmp files...Start.");
            foreach (string fileName in imagesFileEntries)
            {
                string tempStr = Path.GetFileName(fileName);
                int idx = GetIndexFromFilename(tempStr);
                if (idx == -1)
                {
                    AddLog(string.Format("Wrong file name...{0}.", tempStr));
                    continue;
                }
                double tm = 1.0000 / device_frame_rate;
                tm *= idx;
                AddLog(string.Format("Estimate...{0}  {1}  {2}.", idx, tm, fileName));
                int dataIdx = csv_data.FindData(tm);
                string tempStr2 = Path.GetFileNameWithoutExtension(tempStr);
                tempStr2 = select_images_folder + tempStr2 + ".xmp";

                BuildXMPFile(tempStr2, dataIdx);

            }
            AddLog("Build xmp files...End.");

        }
        static void Main(string[] args)
        {
            configFile.Load();
            string filename = string.Format("Log_{0}_{1:HHmmss}.txt", DateTime.Now.ToString("yyyyMMdd"), DateTime.Now);
            m_log_file = new System.IO.StreamWriter(filename);
            m_log_file.AutoFlush = true;
            AddLog("Started");
            

            if (args.Length < 1)
            {

                m_log_file.Close();
                return;
            }
            if (args[0] == "0")
            {
                SaveSetting(args);

                m_log_file.Close();
                return;
            }
            if (args[0] == "1")
            {
                BuildXMP(args);

                m_log_file.Close();
                return;
            }
            if (args[0] == "2")
            {
                DeployScene(args);

                m_log_file.Close();
                return;
            }
            if (args[0] == "3")
            {
                ImportImages(args);

                m_log_file.Close();
                return;
            }
            if (args[0] == "4")
            {
                Cooking(args);

                m_log_file.Close();
                return;
            }
            if (args[0] == "5")
            {
                BuildXMPForRotationBias(args);

                m_log_file.Close();
                return;
            }
            if (args[0] == "6")
            {
                BuildDerivedData();
                m_log_file.Close();
                return;
            }
            if (args[0] == "7")
            {
                AddGPSDataToImage(args);
                m_log_file.Close();
                return;
            }
            if (args[0] == "8")
            {
                ConvertCCXMLFile(args);
                m_log_file.Close();
                return;
            }
            if (args[0] == "9")
            {
                BackupFolder(args);
                m_log_file.Close();
                return;
            }

            if (args[0] == "10")
            {
                XMPFileGenerator(args);
                m_log_file.Close();
                return;
            }
            if (args[0] == "11")
            {
                SendNotification(args);
                m_log_file.Close();
                return;
            }
            if (args[0] == "12")
            {
                ExtractMultiVideo(args);
                m_log_file.Close();
                return;
            }
            if (args[0] == "13")
            {
                SelectImageForSections(args);
                m_log_file.Close();
                return;
            }
            if (args[0] == "14")
            {
                ExtendCRProjImages(args);
                m_log_file.Close();
                return;
            }
            if (args[0] == "15")
            {
                ScenePack(args);
                m_log_file.Close();
                return;
            }

            if (args[0] == "16")
            {
                //test
                RunGroundVU(args);
                m_log_file.Close();
                return;

            }
            if(args[0] == "17")
            {
                RunCRProjCreation(args);
                m_log_file.Close();
                return;
            }
            if (args[0] == "100")
            {
                RunCapturingReality(args);
                m_log_file.Close();
                return;
            }

            if (args[0] == "-s")
            {
                string inStr;
                int inValue = 1;
                do
                {
                    Console.WriteLine("Input:");
                    Console.WriteLine("<1 - BuildXMP>");
                    Console.WriteLine("<2 - DeployScene>");
                    Console.WriteLine("<3 - ImportImages>");
                    Console.WriteLine("<4 - Cooking>");
                    //Console.WriteLine("<5 - >");
                    Console.WriteLine("<Quit/q>");
                    inStr = Console.ReadLine();
                    try
                    {
                        inValue = ParseInput(inStr);
                        if (inValue == 1)
                            BuildXMP(args);
                        if (inValue == 2)
                            DeployScene(args);
                        if (inValue == 3)
                            ImportImages(args);
                    }
                    catch (OverflowException e)
                    {
                        Console.WriteLine("Error:{0}", e.Message);
                    }
                } while (inValue != 0);

                m_log_file.Close();
                return;
            }

            m_log_file.Close();
            return;

        }



        static string BuildSectionPosition(double v1, double v2, double v3)
        {
            return string.Format("      <xcr:Position>{0} {1} {2}</xcr:Position>\r\n", v1,v2,v3);
        }
        static string BuildSectionRotation()
        {
            return string.Format("      <xcr:Rotation>{0} {1} {2} {3} {4} {5} {6} {7} {8}</xcr:Rotation>\r\n", 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0);
        }
        static string BuildSectionDistortionCoeficients()
        {
            return string.Format("      <xcr:DistortionCoeficients>{0} {1} {2} {3} {4} {5}</xcr:DistortionCoeficients>\r\n", 0.0, 0.0, 0.0, 0.0, 0.0, 0.0);
        }

        void LoadGPSData()
        {
            
        }

        static int GetIndexFromFilename(string filename)
        {
            string str_data = filename;
            string ext = Path.GetExtension(str_data);
            string raw_filename = Path.GetFileNameWithoutExtension(str_data);
            if (ext != ".jpg")
                return -1;

            char[] charSeparators = new char[] { '_',' ','(',')' };
            var values = raw_filename.Split(charSeparators, StringSplitOptions.RemoveEmptyEntries);
            if (values.Length < 2)
                return -1;
            return Convert.ToInt32(values[values.Length - 1]);
        }
        static void BuildXMPFile(string filename, int idx)
        {
            CSVGPSData.CSVData data = csv_data.GetData(idx);
            StreamWriter report_file = new StreamWriter(filename);
            string str_data;
            str_data = xmp_sample_head;
            //str_data += BuildSectionRotation();
            str_data += BuildSectionPosition(data.v1, data.v2, data.v3);
            str_data += BuildSectionDistortionCoeficients();
            str_data += xmp_sample_end;
            report_file.WriteLine(str_data);
            report_file.Close();

            AddLog(string.Format("Build xmp file...{0} - {1} - {2}.", idx,data.v1, data.v2));
            AddLog(filename);
        }
    }
}
