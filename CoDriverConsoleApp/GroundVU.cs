
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
    class GroundVU
    {
        string imgLocation;
        List<string> csvfileList; // for now
        string workingDir;
        string ImgDir;
        string csvFile;
        string sceneName;
        string projectFolder;
        string path;

        public void init(string[] args)
        {
            if (args.Length < 4)
            {
                Program.AddLog("GroundVU: You don't have enough arguments");
                return;
            }

            workingDir = Settings1.Default.GVworkingDir;
            ImgDir = args[1];
            csvFile = args[2];
            sceneName = args[3];

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

            projectFolder = Path.Combine(workingDir, sceneName + "_project");
            path = Path.Combine(workingDir, sceneName + ".bat");
        }

        public void startRunGroundVU()
        {









            csvfileList = extractCSVFileList(csvFile);



            List<string> imgfileList = extractImgFileList(ImgDir);


            if (!verifyImgFolder(csvfileList, imgfileList))
            {
                Program.AddLog("GroundVU: csv files doesn't match with image files");
                return;
            }

            writeBatch(path, csvFile, ImgDir, projectFolder);


            imgLocation = Path.Combine(ImgDir, sceneName + "List.csv");
            runProcess(workingDir, path, ImgDir);
            generateResultList();







        }

 
        private void runProcess(string workingDir, string path, string ImgDir)
        {

            Process myProcess = new Process();
            myProcess.StartInfo.WorkingDirectory = workingDir;
            myProcess.StartInfo.FileName = path;
            myProcess.EnableRaisingEvents = true;
            myProcess.Exited += new EventHandler(myProcess_Exited);
            myProcess.Start();
            myProcess.WaitForExit();



        }


        private List<string> extractCSVFileList(string csvFile)
        {
            List<string> csvList = new List<string>();
            using (var rd = new StreamReader(csvFile))
            {
                while (!rd.EndOfStream)
                {

                    var splits = rd.ReadLine().Split(',');
                    if (splits[0].Contains("jpg"))
                        csvList.Add(splits[0].Split('.')[0]);
                }
            }
            return csvList;
        }


        private List<string> extractImgFileList(string ImgDir)
        {
            List<string> imgFiles = new List<string>();
            string[] fileArray = Directory.GetFiles(ImgDir);

            for (int i = 0; i < fileArray.Length; i++)
            {
                imgFiles.Add(Path.GetFileNameWithoutExtension(fileArray[i]));

            }
            return imgFiles;
        }


        private bool verifyImgFolder(List<string> csvList, List<string> imgList)
        {
            foreach (string str in csvList)
            {
                if (!imgList.Contains(str))
                {
                    Program.AddLog("GroundVU: csv files doesn't match with image files");
                    return false;
                }
            }
            return true;

        }

        private void writeBatch(string path, string csvFile, string ImgDir, string projectFolder)
        {
            using (StreamWriter w = new StreamWriter(path))
              {
                w.WriteLine(@"set GV_PATH =..\GV - 1.2.92");
                w.WriteLine(@"rem copy Y:\GroundVu\GV\gv.exe c:\code\Gv\release\Gv.exe");
                string settingGroundVULocation = Settings1.Default.GroundVULocation;
                settingGroundVULocation.Trim('"');


                w.WriteLine(settingGroundVULocation + @" -params garminParams.json -testRoad garminCalibration.json " + csvFile + " " + ImgDir + " " + projectFolder);


                w.WriteLine("quit");
                w.Close();
            }
        }

        private void myProcess_Exited(object sender, EventArgs e)
        {


            /*using (StreamWriter resultFile = new StreamWriter(imgLocation))
            {


                foreach (string str in csvfileList)
                {
                    resultFile.WriteLine(str + ",");
                }
            }
            Program.AddLog("GroundVU: finished");*/
            generateResultList();

        }

        private void generateResultList()
        {
            //string format_string_images = "{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12}";
            string format_string_images = "{0},{1},{2},{3},{4},{5},{6}";
            //string path = Path.Combine(workingDir, sceneName + "_project", "undistorted");
            string path = Path.Combine(@"D:\testGroundVU", "xmp");
            string resultListFilePath = Path.Combine(ImgDir, sceneName + "List.csv"); 

            string[] filePaths = Directory.GetFiles(path, "*.xmp");

            CSVFile csv = new CSVFile();
            csv.BuildFile(resultListFilePath, "ImageName,X,Y,Z,Yaw,Pitch,Roll");
            for (int i = 0; i < filePaths.Length; i++)
            {
                /*XMPFile xmp = new XMPFile();
                xmp.LoadXML(filePaths[i]);
                XMPGenerator xmpGen = new XMPGenerator();
               

                

                //string statement = Path.GetFileNameWithoutExtension(filePaths[2]) + " " + xmp.GetPosition() + " " + xmp.GetRotation();

                string xmpPostions = xmp.GetPosition();
                string xmpRotations = xmp.GetRotation();

                XMPGenerator.FRotator xmpYawPitchAndRoll = xmpGen.GroundVUYawPitchRow(xmpRotations);
                
                string[] Positions = xmpPostions.Split(' ');
                string[] Rotations = xmpRotations.Split(' ');


                //string statement = string.Format(format_string_images, Path.GetFileNameWithoutExtension(filePaths[i]), Positions[0], Positions[1], Positions[2], Rotations[0], Rotations[1], Rotations[2], Rotations[3], Rotations[4], Rotations[5], Rotations[6], Rotations[7], Rotations[8]);
                string statement = string.Format(format_string_images, Path.GetFileNameWithoutExtension(filePaths[i]), Positions[0], Positions[1], Positions[2], xmpYawPitchAndRoll.Roll, xmpYawPitchAndRoll.Pitch, xmpYawPitchAndRoll.Yaw);

                csv.AddLineWithoutIdx(statement);*/
                makeCSVFromXmpWithCertainFormat(filePaths[i], format_string_images, csv);
                Program.AddLog(filePaths[i]);
                
            }
            csv.Save();
        }
        private void makeCSVFromXmpWithCertainFormat(string xmpPath, string statementFormat, CSVFile csv)
        {
            XMPFile xmp = new XMPFile();
            xmp.LoadXML(xmpPath);
            XMPGenerator xmpGen = new XMPGenerator();
            /*
            Program.AddLog(xmp.GetPosition());
            Program.AddLog("strat add rotation");
            Program.AddLog(xmp.GetRotation());
            Console.ReadLine();*/



            //string statement = Path.GetFileNameWithoutExtension(filePaths[2]) + " " + xmp.GetPosition() + " " + xmp.GetRotation();

            string xmpPostions = xmp.GetPosition();
            string xmpRotations = xmp.GetRotation();

            XMPGenerator.FRotator xmpYawPitchAndRoll = xmpGen.GroundVUYawPitchRow(xmpRotations);

            string[] Positions = xmpPostions.Split(' ');
            string[] Rotations = xmpRotations.Split(' ');


            //string statement = string.Format(format_string_images, Path.GetFileNameWithoutExtension(filePaths[i]), Positions[0], Positions[1], Positions[2], Rotations[0], Rotations[1], Rotations[2], Rotations[3], Rotations[4], Rotations[5], Rotations[6], Rotations[7], Rotations[8]);
            string statement = string.Format(statementFormat, Path.GetFileNameWithoutExtension(xmpPath), Positions[0], Positions[1], Positions[2], xmpYawPitchAndRoll.Roll, xmpYawPitchAndRoll.Pitch, xmpYawPitchAndRoll.Yaw);

            csv.AddLineWithoutIdx(statement);
        }

    }


    
}
