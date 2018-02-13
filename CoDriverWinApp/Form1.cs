using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.IO;
using System.Diagnostics;

namespace CoDriverWinApp
{

    public struct VideoFile
    {
        public string cubemapOutput;
        public string proecessFolder;
        public int cubemapSize;
        public int taskidx;

        public VideoFile (string cubemapOutputs, string processFolders, int cubemapSizes, int idx)
        {
            cubemapOutput = cubemapOutputs;
            proecessFolder = processFolders;
            cubemapSize = cubemapSizes;
            taskidx = idx;

        }


    }


    public partial class Form1 : Form
    {
        
        public static List<string> filesnames = new List<string>();
        public static List<string> checkedItems = new List<string>();
        public Form1()
        {
            InitializeComponent();
            Timer timer = new Timer();
            timer.Interval = (500); 
            timer.Tick += new EventHandler(Tick);
            timer.Start();

            isClient = Properties.Settings.Default.IsClient;
            default_forecolor = ImportImgTextBox.ForeColor;
            default_backcolor = ImportImgTextBox.BackColor;
            ConsoleFunction.Init(19);
            ConsoleFunction.default_forecolor = ImportImgTextBox.ForeColor;
            ConsoleFunction.default_backcolor = ImportImgTextBox.BackColor;
            CamConfigManager.default_forecolor = ImportImgTextBox.ForeColor;
            CamConfigManager.default_backcolor = ImportImgTextBox.BackColor;

            NewConsoleFunction(0, "Import Images", panel_importimages);
            NewConsoleFunction(1, "Build Derived Data", panel_ddc);
            NewConsoleFunction(2, "Deploy Scene", panel_deployscene);
            NewConsoleFunction(3, "DLC Package", panel_dlcpackage);
            NewConsoleFunction(4, "Extract JPEG&GPS", panel_exact_image);
            NewConsoleFunction(5, "Undistort images", panel_undistort_image);
            NewConsoleFunction(6, "Build GPS metaData", panel_build_metadata);
            NewConsoleFunction(7, "Change Resolution", panel_change_resolution);
            NewConsoleFunction(8, "Convert CC XML", panel_convertcc);
            NewConsoleFunction(9, "CapturingReality", panel_cr);
            NewConsoleFunction(10, "Generate XMP Files", panel_xmp_generator);
            NewConsoleFunction(11, "Extract Multi-Video", panel_multi_video_extract);
            NewConsoleFunction(12, "Select Images", panel_build_section_images);
            NewConsoleFunction(13, "Extend CR Prj Images", panel_cr_proj);
            NewConsoleFunction(14, "Pack Scene", scenepack);
            NewConsoleFunction(15, "Extract cubemaps", cubemapImage);
            NewConsoleFunction(16, "GroundVU", GroundVU);
            NewConsoleFunction(17, "CR Proj", panel_cr_creation);
            NewConsoleFunction(18, "Mesh Creation", panel_MeshCreation);



            FunctionlistBox.SelectedIndexChanged += UpdateSelectFunction;
            if(isClient)
                ConsoleFunction.UpdateSelectFunc(FunctionlistBox.SelectedIndex);
            //else
            //{
            //    videoManager = new VideoCollection();
            //    videoManager.Init();
            //    videoManager.CheckVideoPath();

            //    ConsoleFunction.UpdateSelectFunc(-1);
            //    FunctionlistBox.Visible = false;
            //    list_video.MultiColumn = true;
            //    string[] data = { "1111", "2222" };
            //    list_video.Items.AddRange(data);
            //}
            ConsoleFunction.InitKeywordCombo(combo_multi_keywords);
            combo_nb_sec_image.Items.Add("2400");
            combo_nb_sec_image.Items.Add("3000");
            combo_nb_sec_image.SelectedIndex = 0;
            combo_select_image_step.Items.Add("1/1");
            combo_select_image_step.Items.Add("1/2");
            combo_select_image_step.Items.Add("1/3");
            combo_select_image_step.Items.Add("1/4");
            combo_select_image_step.Items.Add("1/5");
            combo_select_image_step.SelectedIndex = 1;
            combo_crc_section.Items.Add("2500-Overlap(50)-Front(1)");
            combo_crc_section.Items.Add("2500-Overlap(50)-Front(1/2)");
            combo_crc_section.Items.Add("2500-Overlap(50)-Front(1)-Left(1)-Right(1)-Back(1)");
            combo_crc_section.Items.Add("2500-Overlap(50)-Front(1)-Left(1/2)-Right(1/2)-Back(1/2)");
            combo_crc_section.Items.Add("2500-Overlap(50)-Front(1)-Left(1/3)-Right(1/3)-Back(1/3)");
            combo_crc_section.Items.Add("2500-Overlap(50)-Front(1)-Left(1/4)-Right(1/4)-Back(1/4)");
            combo_crc_section.Items.Add("2500-Overlap(50)-Front(1/2)-Left(1/2)-Right(1/2)-Back(1/2)");
            combo_crc_section.Items.Add("2500-Overlap(50)-Front(1/2)-Left(1/4)-Right(1/4)-Back(1/4)");
            combo_crc_section.Items.Add("2500-Overlap(50)-Front(1/2)-Left(1/6)-Right(1/6)-Back(1/6)");
            combo_crc_section.Items.Add("2500-Overlap(50)-Front(1/2)-Left(1/8)-Right(1/8)-Back(1/8)");
            combo_crc_section.SelectedIndex = 3;
            //combo_crc_overlap.Items.Add("50");
            //combo_crc_overlap.SelectedIndex = 0;
            //ConsoleFunction.InitStepGapCombo(combo_crc_front,0);
            //ConsoleFunction.InitStepGapCombo(combo_crc_left, 0);
            //ConsoleFunction.InitStepGapCombo(combo_crc_right, 0);
            //ConsoleFunction.InitStepGapCombo(combo_crc_back, 0);

            comboBox_CRactionsNeeded.Items.Add("reconstructing");
            comboBox_CRactionsNeeded.Items.Add("alignment");
            comboBox_CRactionsNeeded.Items.Add("alignment + reconstructing");
            comboBox_CRactionsNeeded.Items.Add("alignment + reconstructing + simplify");
            comboBox_CRactionsNeeded.SelectedIndex = 3;

            BuildCommandArguments.InitFFMpegCommandLine();
            button_start_importImg.Enabled = false;
            button_start_deploy.Enabled = false;
            btn_package.Enabled = false;
            ErrorInfo_Deploy.Text = "";
            checkBox_ddc.Checked = true;
            check_add_gps_to_image.Checked = false;
            check_extract_gps.Checked = true;
            panel_video_list.Enabled = false;
            panel_video_list.Visible = false;
           // task_manager = new TaskManager();
            task_manager.listBox_Process = listBox_Process;
            list_multi_video.Items.Clear();

            isClient = Properties.Settings.Default.IsClient;
            if(isClient)
            {
                label_socket.Text = "Client";
                //console_client = new ConsoleClient();
              }
            else
            {
                //label_socket.Text = "Server";
                //console_server = new ConsoleServer();
                //console_server.SocketList = list_client;
                //console_server.Start_ConsoleServer();
            }

           


        }

        
        static Color default_backcolor;
        static Color default_forecolor;
        static string openCV_app_name = "OpenCV\\OpenCVConsoleApp.exe";
        static string console_app_name = "Console\\CoDriverConsoleApp.exe";

        static public System.IO.StreamWriter m_log_file;
        public static int currentidx;
        public static bool extractedcubemap = false;
        static public TaskManager task_manager = new TaskManager();
        public static bool cubemapstarted = false;
        public static string processFolder;
        public static string cubemapoutput;

        public static Dictionary<int, VideoFile> extractingTasks = new Dictionary<int, VideoFile>();
       
        
       



        public static int cubemapsize;
        public static int startid;
        public static int endid;
        public static int cubemapcalled = 0;

        VideoCollection videoManager;


        //ConsoleServer console_server;

        bool isClient = false;

        private void Tick(object sender, EventArgs e)
        {

            
            int funcIdx = FunctionlistBox.SelectedIndex;
            if (funcIdx == 0 && CheckCondition_ImportImg())
                button_start_importImg.Enabled = true;
            else
                button_start_importImg.Enabled = false;
            if (funcIdx == 2 && CheckCondition_DeployScene())
                button_start_deploy.Enabled = true;
            else
                button_start_deploy.Enabled = false;
            if (funcIdx == 3 && CheckCondition_Package())
                btn_package.Enabled = true;
            else
                btn_package.Enabled = false;

            if (funcIdx == 4 && CheckCondition_ExtractImages())
            {
                btn_exactimage_start.Enabled = true;
            }
            else
                btn_exactimage_start.Enabled = false;
            if (funcIdx == 5 && CheckCondition_UndistortImages())
            {
                btn_undistort.Enabled = true;
            }
            else
                btn_undistort.Enabled = false;
            if (funcIdx == 6)
            {

            }
            if (funcIdx == 7 && CheckCondition_ChangeResolution())
            {
                btn_resolution_start.Enabled = true;
            }
            else
                btn_resolution_start.Enabled = false;
            task_manager.Run();

            if(funcIdx == 10 && CheckCondition_GenerateXMPFiles())
            {
                btn_xg_start.Enabled = true;
            }
            else
                btn_xg_start.Enabled = false;

            if (funcIdx == 10 && CheckCondition_GenerateXMPFiles_2())
            {
                btn_xg_start_2.Enabled = true;
            }
            else
                btn_xg_start_2.Enabled = false;
            if (funcIdx == 11 && CheckCondition_ExtractMultiVideo())
            {
                btn_multi_video_start.Enabled = true;
            }
            else
                btn_multi_video_start.Enabled = false;

            if (funcIdx == 12 && CheckCondition_SelectImagesSection())
            {
                btn_build_sec_image.Enabled = true;
            }
            else
                btn_build_sec_image.Enabled = false;
            if (funcIdx == 13 && CheckCondition_ExtendCRImages())
            {
                btn_cr_build_start.Enabled = true;
            }

         
            if (funcIdx == 14 && CheckShippingVersion())
            {
                
                button_start_packing.Enabled = true;
            }

            

            else
                btn_cr_build_start.Enabled = false;

            if (funcIdx == 16 && CheckForGroundVU())
            {
                button_startGroundVU.Enabled = true;
            }
            else
                button_startGroundVU.Enabled = false;

            if(funcIdx == 17 && CheckCRProjCreation())
            {
                btn_crc_creation_start.Enabled = true;
            }
            else
            {
                btn_crc_creation_start.Enabled = false;
            }

            if (funcIdx == 18 && CheckMeshCreation())
            {
                button_StartCreateMesh.Enabled = true;
            }
            else
            {
                button_StartCreateMesh.Enabled = false;
            }

            if (isClient)
                Program.g_console_client.Update();

            if (Program.g_ftpServer.IsUploadCompleted())
                panel_video_list.Enabled = true;
            else
                panel_video_list.Enabled = false;

            
            if (extractedcubemap) {
                cubemapExtracting();
            }

        }
        

        






        public void cubemapExtracting()
        {
            for (int j = 0; j < extractingTasks.Count; j++)
            {
                if (checkifexited(extractingTasks[j].taskidx))
                {
                    Program.AddLog("image extraction finished");
                    cubemapstarted = true;
                    int processNumber = Int32.Parse(processNumberInfo.Text);

                    int fCount = Directory.GetFiles(extractingTasks[j].proecessFolder, "*.jpg", SearchOption.TopDirectoryOnly).Length;
                    int average = fCount / processNumber;
                    
                    for (int i = 0; i < processNumber; i++)
                    {
                        startid = average * i + 1;
                        endid = average * (i + 1);
                        if (i == processNumber - 1)
                        {
                            cubemapGenerator(extractingTasks[j].proecessFolder, extractingTasks[j].cubemapOutput, extractingTasks[j].cubemapSize, startid, fCount);
                        }
                        cubemapGenerator(extractingTasks[j].proecessFolder, extractingTasks[j].cubemapOutput, extractingTasks[j].cubemapSize, startid, endid);
                    }
              
                    if (extractingTasks.Count <= 0)
                        extractedcubemap = false;
                    extractingTasks.Remove(j);
                    break;
                }
            }
        
       }


        public bool checkifexited(int id)
        {
            return task_manager.CheckTask(id);
        }

        void NewConsoleFunction(int idx,string name,Panel p)
        {
            ConsoleFunction func = new ConsoleFunction();
            func.name = name;
            func.panel_form = p;
            func.itemIdx = FunctionlistBox.Items.Add(name);
            func.panel_form.Visible = false;

            if(idx == 0)
            {
                FunctionlistBox.SelectedIndex = func.itemIdx;
            }
            ConsoleFunction.gs_func[idx] = func;
        }

        void UpdateSelectFunction(object sender, EventArgs e)
        {
            int funcIdx = FunctionlistBox.SelectedIndex;
            ConsoleFunction.UpdateSelectFunc(funcIdx);            
            if (funcIdx == 0)
            {
                checkBox_ddc.Checked = true;
            }
            if(funcIdx == 3)
            {
                ConsoleFunction.ListUE4Scenes(listBox_Scenes);
            }
            if (funcIdx == 4)
            {
                check_extract_gps.Checked = true;
                check_add_gps_to_image.Checked = false;
                Program.g_camConfigMgr.UpdateListBox(list_movie_cam_config);
            }
            if (funcIdx == 5)
            {
                Program.g_camConfigMgr.UpdateListBox(list_camera_config);
            }
            if(funcIdx == 6)
            {
            }
            if(funcIdx == 7)
            {
                list_resolution.Items.Clear();
                list_resolution.Items.Add("1024X1024");
                list_resolution.Items.Add("2048X2048");
                list_resolution.Items.Add("4096X2048");
                list_resolution.SelectedIndex = 0;
            }
            return;
        }
        public void InitLog()
        {
            string filename = string.Format("ConsoleLog_{0}_{1:HHmmss}.txt", DateTime.Now.ToString("yyyyMMdd"), DateTime.Now);
            m_log_file = new System.IO.StreamWriter(filename);
            m_log_file.AutoFlush = true;
            AddLog("Start");
        }
        public void CloseLog()
        {
            m_log_file.Close();
        }
        delegate void AddLog_Involk(string message);
        public void AddLog(string v)
        {
            try
            {
                if (LogListBox.InvokeRequired)
                {
                    AddLog_Involk CI = new AddLog_Involk(AddLog);
                    LogListBox.Invoke(CI, v);
                }
                else
                {
                    string timeStr = DateTime.Now.ToString("u");
                    string lines = timeStr + " " + v + "\n";
                    m_log_file.WriteLine(lines);
                    LogListBox.Items.Add(lines);
                    LogListBox.SelectedIndex = LogListBox.Items.Count - 1;
                }
            } catch (Exception e)
            {
                
            }
        }

        static string MakePathForCommandLine(string inPath)
        {
            return "\"" + inPath + "\" ";
        }
        public bool CheckCondition_ExtendCRImages()
        {
            bool isError = ConsoleFunction.check_folder(text_cr_img_folder, "");
            if (ConsoleFunction.check_file(text_cr_proj_file))
            {
                isError = true;
            }
            return !isError;
        }


        public bool CheckShippingVersion()
        {
            
            bool isError = ConsoleFunction.check_folder(text_sections_shippingVersion, "");
            if (ConsoleFunction.check_folder(text_sections_shippingVersion, ""))
            {
                isError = true;
            }
            

            return !isError;




        }

        public bool CheckCRProjCreation()
        {
            bool isError = ConsoleFunction.check_folder(text_crc_frontimages, "");


            if (ConsoleFunction.check_scene_name(text_crc_scenename))
            {
                isError = true;
            }
            //if (ConsoleFunction.check_folder(text_crc_leftimages, ""))
            //    isError = true;
            //if (ConsoleFunction.check_folder(text_crc_rightimages, ""))
            //    isError = true;
            //if (ConsoleFunction.check_folder(text_crc_backimages, ""))
            //    isError = true;

            return !isError;
        }

        public bool CheckMeshCreation()
        {
            bool isError = ConsoleFunction.check_file(textBox_ProjectFileLocation);


            if (ConsoleFunction.check_file(textBox_GlobalSettingLocation))
            {
                isError = true;
            }

            if (comboBox_CRactionsNeeded.SelectedIndex == 3)
            {
                text_targetTriangleCount.Enabled = true;
            } else
            {
                text_targetTriangleCount.Enabled = false;
            }
        

            return !isError;
        }

        public bool CheckForGroundVU()
        {
            bool isErrorOne = ConsoleFunction.check_folder(text_ImageDir, "");
            bool isErrorTwo = ConsoleFunction.check_file(text_csvFile);
            bool isErrorThree = false;
            
            if (text_sceneName.Text.Equals("") || !(System.Text.RegularExpressions.Regex.IsMatch(text_sceneName.Text, "^[a-zA-Z0-9\x20]+$")) || text_sceneName.Text.Contains(" "))
            {
                isErrorThree = true;
            }

            bool isError = isErrorOne || isErrorTwo || isErrorThree;

            return !isError;
        }


      

        public bool CheckCondition_SelectImagesSection()
        {
            bool isError = ConsoleFunction.check_folder(text_sections_images_folder, "");

            if (combo_nb_sec_image.SelectedIndex < 0)
                isError = true;

            return !isError;
        }
        public bool CheckCondition_ExtractMultiVideo()
        {
            bool isError = ConsoleFunction.check_file(text_first_video);
            if (ConsoleFunction.check_folder(text_multi_video_output,""))
            {
                isError = true;
            }
            //if (list_multi_video.Items.Count < 1)
            //    isError = true;

            return !isError;
        }
        public bool CheckCondition_GenerateXMPFiles_2()
        {
            bool isError = ConsoleFunction.check_folder(text_xg_image_folder, "");
            if (ConsoleFunction.check_file(text_xg_calib_xmp))
            {
                isError = true;
            }
            return !isError;
        }
        public bool CheckCondition_GenerateXMPFiles()
        {
            bool isError = ConsoleFunction.check_folder(text_master_folder, "");
            if(ConsoleFunction.check_folder(text_slave_folder0, ""))
            {
                isError = true;
            }
            bool isFixed = checkBox_xg_fixed_90.Checked;
            if (ConsoleFunction.check_file(text_xmp_master))
            {
                isError = true;
            }
            if (!isFixed && ConsoleFunction.check_file(text_xmp_slave0))
            {
                isError = true;
            }
            if(text_slave_folder1.Text.Length > 3)
            {
                if (ConsoleFunction.check_folder(text_slave_folder1, ""))
                {
                    isError = true;
                }
                if (!isFixed && ConsoleFunction.check_file(text_xmp_slave1))
                {
                    isError = true;
                }
            }
            if (text_slave_folder2.Text.Length > 3)
            {
                if (ConsoleFunction.check_folder(text_slave_folder2, ""))
                {
                    isError = true;
                }
                if (!isFixed && ConsoleFunction.check_file(text_xmp_slave2))
                {
                    isError = true;
                }
            }
            return !isError;
        }
        public bool CheckCondition_ChangeResolution()
        {
            bool isError = ConsoleFunction.check_folder(text_resolution_folder,"");
            if (check_resolution_virb360.Checked)
                list_resolution.Enabled = false;
            else
                list_resolution.Enabled = true;
            return !isError;
        }

        public bool CheckCondition_UndistortImages()
        {         
            bool isError = false;
            isError = ConsoleFunction.check_folder(text_undistort_image_folder, "");
            if (Program.g_camConfigMgr.check_select_cam(list_camera_config))
            {
                isError = true;
            }
            if(ConsoleFunction.check_double_value(text_output_focallen))
            {
                isError = true;
            }
            return !isError;
        }
        public bool CheckCondition_ExtractImages()
        {
            bool isError = false;
            isError = ConsoleFunction.check_file(text_movie_file);
            if(Program.g_camConfigMgr.check_select_cam(list_movie_cam_config))
            {
                isError = true;
            }
            bool isNameError = ConsoleFunction.check_keyword(text_exactImage_keyword);
            if (isNameError)
                isError = true;

            if(check_add_gps_to_image.Checked == true)
            {
                check_extract_gps.Checked = true;
            }
            if (check_movie_undistort.Checked)
                check_movie_backup.Checked = true;

            return !isError;
        }
        public bool CheckCondition_ImportImg()
        {
            //string append = "Images";
            bool isError = ConsoleFunction.check_folder(ImportImgTextBox, "");
           
            bool isNameError = ConsoleFunction.check_keyword(ImportImgSceneName);
            if (isNameError)
                isError = true;
            return !isError;
        }
        public bool CheckCondition_DeployScene()
        {
            bool isError = false;
            string images_folder = DeploySelectFolder_Text.Text + "\\Images\\";
            if (!Directory.Exists(images_folder))
            {
                isError = true;
                DeploySelectFolder_Text.ForeColor = Color.Red;
                DeploySelectFolder_Text.BackColor = Color.Yellow;
            }
            else
            {
                string imagelist_filename = DeploySelectFolder_Text.Text + "\\imagelist.csv";
                if (!File.Exists(imagelist_filename))
                {
                    ErrorInfo_Deploy.Text = string.Format("Missing Image List CSV File! {0}", imagelist_filename);
                    isError = true;
                }
                string maya_filename = DeploySelectFolder_Text.Text + "\\maya_ascii_data.ma";
                if (!File.Exists(maya_filename))
                {
                    ErrorInfo_Deploy.Text = string.Format("Missing Maya ASCII Data File! {0}", maya_filename);
                    isError = true;
                }
                string sceneInfo_xml_filename = DeploySelectFolder_Text.Text + "\\SceneInfo.xml";
                if (!File.Exists(sceneInfo_xml_filename))
                {
                    ErrorInfo_Deploy.Text = string.Format("Missing Scene Info XML File! {0}", sceneInfo_xml_filename);
                    isError = true;
                }
                if(isError)
                {
                    DeploySelectFolder_Text.ForeColor = Color.Red;
                    DeploySelectFolder_Text.BackColor = Color.Yellow;
                }
                else
                {
                    DeploySelectFolder_Text.ForeColor = default_forecolor;
                    DeploySelectFolder_Text.BackColor = default_backcolor;
                }                
            }
            bool isNameError = ConsoleFunction.check_keyword(SceneNameText);
            if (isNameError)
                isError = true;
 
            char[] charSeparators = new char[] { '_', ' ', '(', ')' };
            //string images_folder = DeploySelectFolder_Text.Text + "Images\\";
            string keyword = "";
            int count = 0;
            if (Directory.Exists(images_folder))
            {
                string[] files = Directory.GetFiles(images_folder);
                foreach (var file in files)
                {
                    string filename = Path.GetFileName(file);
                    var values = filename.Split(charSeparators, StringSplitOptions.RemoveEmptyEntries);
                    if (count != 0)
                        keyword += "-";
                    keyword = values[0];
                    count++;
                }
                // System.Windows.Forms.MessageBox.Show("Files found: " + files.Length.ToString(), "Message");
            }
            ImgKeywordTextBox.Text = keyword;

            return !isError;
        }
        public bool CheckCondition_Package()
        {
            bool isError = false;
            bool isFolderError = false;
            string engine_folder = ShippingVersionFolder.Text + "\\WindowsNoEditor\\" + Properties.Settings.Default.ProjectName + "\\Binaries\\";
            if (!Directory.Exists(engine_folder))
            {
                isFolderError = true;
            }
            string dest_pak_file = ShippingVersionFolder.Text + "\\WindowsNoEditor\\" + Properties.Settings.Default.ProjectName + "\\Content\\Paks\\";
            if (!Directory.Exists(dest_pak_file))
            {
                isFolderError = true;
            }
            string app_filename = ShippingVersionFolder.Text + "\\WindowsNoEditor\\" + Properties.Settings.Default.ProjectName + ".exe";
            if (!File.Exists(app_filename))
                isFolderError = true;
            if (isFolderError)
            {
                isError = true;
                ShippingVersionFolder.ForeColor = Color.Red;
                ShippingVersionFolder.BackColor = Color.Yellow;
            }
            else
            {
                ShippingVersionFolder.ForeColor = default_forecolor;
                ShippingVersionFolder.BackColor = default_backcolor;
            }

            bool isSelectSceneError = false;
            string selectScene = listBox_Scenes.GetItemText(listBox_Scenes.SelectedItem);            
            if(selectScene.Length < 3)
            {
                isSelectSceneError = true;
            }
            if(isSelectSceneError)
            {
                listBox_Scenes.ForeColor = Color.Red;
                listBox_Scenes.BackColor = Color.Yellow;
                isError = true;
            }
            else
            {
                listBox_Scenes.ForeColor = default_forecolor;
                listBox_Scenes.BackColor = default_backcolor;
            }
            return !isError;
        }       

        private void button_start_deploy_Click(object sender, EventArgs e)
        {
            string app_name = console_app_name;
            string command_arguments = BuildCommandArguments.build_args_deploy_scene(SceneNameText.Text, DeploySelectFolder_Text.Text);
            task_manager.RunCommandLine_Task("DeployScene",app_name, command_arguments);
            return;
        }

        private void button_SelectFolder_Click(object sender, EventArgs e)
        {
            DeploySelectFolder_Text.Text = ConsoleFunction.select_folder();
            //DeploySelectFolder_Text.Text += "\\";
        }
        private void button_selectImportImgFolder_Click(object sender, EventArgs e)
        {
            ImportImgTextBox.Text = ConsoleFunction.select_folder();
            //ImportImgTextBox.Text += "\\";
        }
        private void btn_selectFodler_package_Click(object sender, EventArgs e)
        {
            ShippingVersionFolder.Text = ConsoleFunction.select_folder();
            //ShippingVersionFolder.Text += "\\";
        }
        private void btn_exact_selectfile_Click(object sender, EventArgs e)
        {
            string filter = "Movie files (*.mp4)|*.mp4|All files (*.*)|*.*";
            text_movie_file.Text = ConsoleFunction.select_file(filter);
        }
        private void btn_select_images_Click(object sender, EventArgs e)
        {
            text_undistort_image_folder.Text = ConsoleFunction.select_folder();
        }

        private void button_start_importImg_Click(object sender, EventArgs e)
        {
            if (checkBox_ddc.Checked)
            {
                string app_name1 = console_app_name;
                process_task[] tasks = new process_task[2];

                tasks[0].task_desc = "ImportImages";
                tasks[0].app_name = app_name1;
                tasks[0].app_args = BuildCommandArguments.build_args_import_images(ImportImgSceneName.Text, ImportImgTextBox.Text); 

                tasks[1].task_desc = "BuildDerivedData";
                tasks[1].app_name = app_name1;
                tasks[1].app_args = BuildCommandArguments.build_args_build_derived();

                task_manager.RunCommandLine_MultiTask(tasks);
                return;
            }
            string app_name = console_app_name;
            string command_arguments = BuildCommandArguments.build_args_import_images(ImportImgSceneName.Text, ImportImgTextBox.Text); 
            task_manager.RunCommandLine_Task("ImportImages", app_name, command_arguments);
            return;
        }

        private void btn_package_Click(object sender, EventArgs e)
        {
            string scene_name = listBox_Scenes.GetItemText(listBox_Scenes.SelectedItem);
            string app_name = console_app_name;
            string command_arguments = BuildCommandArguments.build_args_package(scene_name, ShippingVersionFolder.Text); 
            task_manager.RunCommandLine_Task("Package", app_name, command_arguments);
            return;
        }

        private void btn_start_ddc_Click(object sender, EventArgs e)
        {
            string command_arguments = BuildCommandArguments.build_args_build_derived();
            task_manager.RunCommandLine_Task("BuildDerivedData", console_app_name, command_arguments);
        }
        private void btn_exactimage_start_Click(object sender, EventArgs e)
        {
            string keyword = text_exactImage_keyword.Text;
            string output_folder = Path.GetDirectoryName(text_movie_file.Text);
            string filename = Path.GetFileNameWithoutExtension(text_movie_file.Text);
            output_folder = Path.Combine(output_folder, filename);
           
            string app_name = "FFmpeg\\ffmpeg.exe";
            

            if (!Directory.Exists(output_folder))
            {
                Directory.CreateDirectory(output_folder);
            }

            if (check_extract_gps.Checked)
            {                
                process_task[] tasks;
                int nbTask = 2;
                if (check_movie_undistort.Checked)
                    nbTask++;
                if (check_movie_backup.Checked)
                    nbTask++;
                if (check_add_gps_to_image.Checked || check_movie_xmp_file.Checked)
                    nbTask++;
                tasks = new process_task[nbTask];
              
                tasks[0].task_desc = "ExtractImages";
                tasks[0].app_name = app_name;
                tasks[0].app_args = BuildCommandArguments.build_args_ffmpeg(text_movie_file.Text, output_folder, keyword);

                tasks[1].task_desc = "ExtractGPSData";
                tasks[1].app_name = openCV_app_name;
                tasks[1].app_args = BuildCommandArguments.build_args_extract_gps_data(text_movie_file.Text,output_folder,filename);

                string destFocalLen = text_movie_focallen.Text;
                int idxTask = 2;
                if (check_movie_backup.Checked)
                {
                    tasks[idxTask].task_desc = "BackupFolder";
                    tasks[idxTask].app_name = console_app_name;
                    tasks[idxTask].app_args = BuildCommandArguments.build_args_backup_folder(output_folder);
                    idxTask++;
                }
                if (check_movie_undistort.Checked)
                {
                    int camIdx = list_movie_cam_config.SelectedIndex;
                    CamConfig cam_config = Program.g_camConfigMgr.GetCamConfig(camIdx);
                  
                    tasks[idxTask].task_desc = "UndistortImage";
                    tasks[idxTask].app_name = openCV_app_name;
                    tasks[idxTask].app_args = BuildCommandArguments.build_args_undistort(output_folder,cam_config,destFocalLen,false);
                    idxTask++;
                }
                if(check_movie_xmp_file.Checked)
                {
                    tasks[idxTask].task_desc = "AddGPSToImage";
                    tasks[idxTask].app_name = console_app_name;
                    tasks[idxTask].app_args = BuildCommandArguments.build_args_build_metadata(output_folder, true);
                }
                else
                {
                    if (check_add_gps_to_image.Checked)
                    {
                        tasks[idxTask].task_desc = "AddGPSToImage";
                        tasks[idxTask].app_name = console_app_name;
                        string command_arguments2 = "7 ";
                        command_arguments2 += MakePathForCommandLine(output_folder);
                        if (check_movie_undistort.Checked)
                            command_arguments2 += destFocalLen;
                        tasks[idxTask].app_args = command_arguments2;
                    }
                }

                task_manager.RunCommandLine_MultiTask(tasks);
                return;
            }

            task_manager.RunCommandLine_Task("ExtractImages", app_name, BuildCommandArguments.build_args_ffmpeg(text_movie_file.Text, output_folder, keyword));
        }

        private void btn_undistort_Click(object sender, EventArgs e)
        {
            int camIdx = list_camera_config.SelectedIndex;
            if (camIdx < 0)
                return;
            if (camIdx >= Program.g_camConfigMgr.GetNbCamConfig())
                return;

            string destFocalLen = text_output_focallen.Text;
            CamConfig  cam_config = Program.g_camConfigMgr.GetCamConfig(camIdx);
            string app_name = "OpenCV\\OpenCVConsoleApp.exe";
            string command_arguments = BuildCommandArguments.build_args_undistort(text_undistort_image_folder.Text, cam_config, destFocalLen, check_undistort_backup.Checked);
            task_manager.RunCommandLine_Task("UndistortImage", app_name, command_arguments);
            return;
        }

        private void btn_resolution_start_Click(object sender, EventArgs e)
        {
            if(check_resolution_virb360.Checked)
            {
                char[] charSeparators = new char[] { '_', '(', ')', ' ' };
                string[] fileEntries = Directory.GetFiles(text_resolution_folder.Text);
                //string folderName = Path.GetFileName(Path.GetDirectoryName(folder));
                int max_index = 0;
                foreach (string fileName in fileEntries)
                {
                    string ext = Path.GetExtension(fileName);
                    if (ext == ".jpg")
                    {
                        string file = Path.GetFileNameWithoutExtension(fileName);
                        var values = file.Split(charSeparators, StringSplitOptions.RemoveEmptyEntries);
                        if (values.Length < 2)
                            continue;
                        int idx = Convert.ToInt32(values[1]);
                        if (max_index < idx)
                            max_index = idx;
                    }
                }

                if(max_index < 200)
                {
                    string args = "3 ";
                    args += MakePathForCommandLine(text_resolution_folder.Text);
                    task_manager.RunCommandLine_Task("ChangeResolution", "OpenCV\\OpenCVConsoleApp.exe", args);
                    return;
                }
                int nProcess = 10;
                int nStep = max_index / nProcess;
                int nAppend = max_index % nProcess;
                int[] startIdxList = new int[nProcess];
                for (int a=0;a< nProcess; a++)
                {
                    startIdxList[a] = nStep * a + 1;
                }

                int maxCount = startIdxList.Count();
                for (int a = 0; a < maxCount; a++)
                {
                    int i2;
                    int i1 = startIdxList[a];
                    if(a == maxCount - 1)
                    {
                        i2 = max_index;
                    }
                    else
                        i2 = startIdxList[a + 1] - 1;
                    string args = "3 ";
                    args += MakePathForCommandLine(text_resolution_folder.Text);
                    args += i1.ToString();
                    args += " ";
                    args += i2.ToString();
                    task_manager.RunCommandLine_Task("ChangeResolution", "OpenCV\\OpenCVConsoleApp.exe", args);
                }
                


                return;
            }
            int resIdx = list_resolution.SelectedIndex;
            string target_res = "1024";
            if (resIdx == 0)
                target_res = "1024";
            if (resIdx == 1)
                target_res = "2048";
            if (resIdx == 2)
            {
                string args = "2 ";
                args += MakePathForCommandLine(text_resolution_folder.Text);
                args += "4096 2048 ";
                task_manager.RunCommandLine_Task("ChangeResolution", "OpenCV\\OpenCVConsoleApp.exe", args);
                return;
            }

            string app_name = "OpenCV\\OpenCVConsoleApp.exe";
            string command_arguments = "2";
            command_arguments += " \"";
            command_arguments += text_resolution_folder.Text;
            command_arguments += "\" ";
            command_arguments += target_res;
            command_arguments += " ";
            task_manager.RunCommandLine_Task("ChangeResolution", app_name, command_arguments);

        }

        private void btn_resolution_select_Click(object sender, EventArgs e)
        {
            text_resolution_folder.Text = ConsoleFunction.select_folder();
        }
        private void btn_build_metadata_Click(object sender, EventArgs e)
        {
            text_build_metadata.Text = ConsoleFunction.select_folder();
        }
        private void btn_select_cc_xml_Click(object sender, EventArgs e)
        {
            string filter = "XML files (*.xml)|*.xml|All files (*.*)|*.*";
            text_cc_xml_file.Text = ConsoleFunction.select_file(filter);
        }

        private void btn_start_build_metadata_Click(object sender, EventArgs e)
        {
            string command_arguments = BuildCommandArguments.build_args_build_metadata(text_build_metadata.Text, check_build_metadata_xmp.Checked);
            task_manager.RunCommandLine_Task("AddGPSToImage", console_app_name, command_arguments);
        }

        private void btn_start_cc_xml_Click(object sender, EventArgs e)
        {
            string command_arguments = "8 ";
            command_arguments += "\"";
            command_arguments += text_cc_xml_file.Text;
            command_arguments += "\"";
            task_manager.RunCommandLine_Task("ConvertCCXML", console_app_name, command_arguments);

        }

        private void btn_cr_select_folder_Click(object sender, EventArgs e)
        {
            text_cr_image_folder.Text = ConsoleFunction.select_folder();
        }

        private void btn_cr_start_Click(object sender, EventArgs e)
        {
            string command_arguments = "100 ";
            command_arguments += "1 ";
            command_arguments += "\"";
            command_arguments += text_cr_image_folder.Text;
            command_arguments += "\"";
        }

        private void btn_cr_export_comp_Click(object sender, EventArgs e)
        {
            string command_arguments = "100 ";
            command_arguments += "2 ";
            command_arguments += "\"";
            command_arguments += text_cr_project_name.Text;
            command_arguments += "\"";
        }

        private void btn_cr_select_project_Click(object sender, EventArgs e)
        {
            string filter = "Project files (*.rcproj)|*.rcproj|All files (*.*)|*.*";
            text_cr_project_name.Text = ConsoleFunction.select_file(filter);
        }
        
        

        private void btn_upload_form_Click(object sender, EventArgs e)
        {
           // console_client.SendMsg_RequestUploadVideo();
            FormUploadVideo form2_uploadvideo = new FormUploadVideo();
            // Show the settings form
            form2_uploadvideo.Show();

           // Program.g_ftpServer.LinkFTPSite(0);
        }

        private void btn_xg_select_master_img_Click(object sender, EventArgs e)
        {
            text_master_folder.Text = ConsoleFunction.select_folder();
        }

        private void btn_xg_select_slave_img0_Click(object sender, EventArgs e)
        {
            text_slave_folder0.Text = ConsoleFunction.select_folder();
        }

        private void btn_xg_select_slave_img1_Click(object sender, EventArgs e)
        {
            text_slave_folder1.Text = ConsoleFunction.select_folder();
        }

        private void btn_xg_select_slave_img2_Click(object sender, EventArgs e)
        {
            text_slave_folder2.Text = ConsoleFunction.select_folder();
        }

        private void btn_select_xmp_master_Click(object sender, EventArgs e)
        {
            string filter = "XMP files (*.xmp)|*.xmp|All files (*.*)|*.*";
            text_xmp_master.Text = ConsoleFunction.select_file(filter);
        }
        private void btn_select_xmp_slave0_Click(object sender, EventArgs e)
        {
            string filter = "XMP files (*.xmp)|*.xmp|All files (*.*)|*.*";
            text_xmp_slave0.Text = ConsoleFunction.select_file(filter);
        }
        private void btn_select_xmp_slave1_Click(object sender, EventArgs e)
        {
            string filter = "XMP files (*.xmp)|*.xmp|All files (*.*)|*.*";
            text_xmp_slave1.Text = ConsoleFunction.select_file(filter);
        }
        private void btn_select_xmp_slave2_Click(object sender, EventArgs e)
        {
            string filter = "XMP files (*.xmp)|*.xmp|All files (*.*)|*.*";
            text_xmp_slave2.Text = ConsoleFunction.select_file(filter);
        }
        private void btn_xg_start_Click(object sender, EventArgs e)
        {
            if(checkBox_xg_fixed_90.Checked)
            {
                string command_args = "10 2 ";
                command_args += MakePathForCommandLine(text_master_folder.Text);
                command_args += MakePathForCommandLine(text_slave_folder0.Text);
                command_args += MakePathForCommandLine(text_slave_folder1.Text);
                command_args += MakePathForCommandLine(text_slave_folder2.Text);
                command_args += MakePathForCommandLine(text_xmp_master.Text);

                task_manager.RunCommandLine_Task("XmpGenerater", console_app_name, command_args);
                return;
            }
            string command_arguments = "10 ";
            command_arguments += MakePathForCommandLine(text_master_folder.Text);
            command_arguments += MakePathForCommandLine(text_slave_folder0.Text);
            command_arguments += MakePathForCommandLine(text_slave_folder1.Text);
            command_arguments += MakePathForCommandLine(text_slave_folder2.Text);
            command_arguments += MakePathForCommandLine(text_xmp_master.Text);
            command_arguments += MakePathForCommandLine(text_xmp_slave0.Text);
            command_arguments += MakePathForCommandLine(text_xmp_slave1.Text);
            command_arguments += MakePathForCommandLine(text_xmp_slave2.Text);

            task_manager.RunCommandLine_Task("XmpGenerater", console_app_name, command_arguments);
        }

        private void btn_xg_select_img_folder_Click(object sender, EventArgs e)
        {
            text_xg_image_folder.Text = ConsoleFunction.select_folder();
        }

        private void btn_xg_select_calib_xmp_Click(object sender, EventArgs e)
        {
            string filter = "XMP files (*.xmp)|*.xmp|All files (*.*)|*.*";
            text_xg_calib_xmp.Text = ConsoleFunction.select_file(filter);
        }

        private void btn_xg_start_2_Click(object sender, EventArgs e)
        {
            string command_arguments = "10 1 ";
            command_arguments += MakePathForCommandLine(text_xg_image_folder.Text);
            command_arguments += MakePathForCommandLine(text_xg_calib_xmp.Text);

            task_manager.RunCommandLine_Task("XmpGenerater", console_app_name, command_arguments);
        }

        private void btn_setting_form_Click(object sender, EventArgs e)
        {
            FormSetting form3_setting = new FormSetting();
            form3_setting.Show();
        }

        private void btn_video_lib_Click(object sender, EventArgs e)
        {
            FormVideoLib form3_video_lib = new FormVideoLib();
            form3_video_lib.Show();
            
        }
        private static string GetExtractFolder(string videoFile)
        {
            string output_folder = Path.GetDirectoryName(videoFile);
            string filename = Path.GetFileNameWithoutExtension(videoFile);
            output_folder = Path.Combine(output_folder, filename);

            if (!Directory.Exists(output_folder))
            {
                Directory.CreateDirectory(output_folder);
            }
            return output_folder;
        }
        private void btn_multi_video_start_Click(object sender, EventArgs e)
        {
            string app_name = "FFmpeg\\ffmpeg.exe";
            int nbVideo = 1;
            List<string> videoFilenameList = new List<string>();
            for (int a = 0; a < list_multi_video.Items.Count; a++)
            {
                string videoFile = list_multi_video.Items[a].ToString();
                if (videoFile.Length < 3)
                    continue;
                videoFilenameList.Add(videoFile);
                nbVideo++;
            }

                process_task[] tasks;
            int nbTask = 1;
            nbTask += 2 * nbVideo;
            tasks = new process_task[nbTask];

            string outputFolder = GetExtractFolder(text_first_video.Text);
            string filename = Path.GetFileNameWithoutExtension(text_first_video.Text);
            string keyword = combo_multi_keywords.SelectedItem.ToString();

            tasks[0].task_desc = "ExtractImages";
            tasks[0].app_name = app_name;
            tasks[0].app_args = BuildCommandArguments.build_args_ffmpeg(text_first_video.Text, outputFolder, keyword);

            tasks[1].task_desc = "ExtractGPSData";
            tasks[1].app_name = openCV_app_name;
            tasks[1].app_args = BuildCommandArguments.build_args_extract_gps_data(text_first_video.Text, outputFolder, filename);

            for(int a=0;a< videoFilenameList.Count;a++)
            {
                int idxBase = (a + 1) * 2;
                string videoFile = videoFilenameList[a];
                string outputVideoFolder = GetExtractFolder(videoFile);
                string videoFilename = Path.GetFileNameWithoutExtension(videoFile);

                tasks[idxBase].task_desc = "ExtractImages";
                tasks[idxBase].app_name = app_name;
                tasks[idxBase].app_args = BuildCommandArguments.build_args_ffmpeg(videoFile, outputVideoFolder, keyword);

                tasks[idxBase + 1].task_desc = "ExtractGPSData";
                tasks[idxBase + 1].app_name = openCV_app_name;
                tasks[idxBase + 1].app_args = BuildCommandArguments.build_args_extract_gps_data(videoFile, outputVideoFolder, videoFilename);
            }

            tasks[nbTask - 1].task_desc = "TotalImages";
            tasks[nbTask - 1].app_name = console_app_name;

           
            string command_arguments = "12 ";
            command_arguments += nbVideo.ToString();
            command_arguments += " ";
            string gapLimit = text_multi_gap_limit.Text;
            command_arguments += gapLimit;
            command_arguments += " ";
            command_arguments += MakePathForCommandLine(text_multi_video_output.Text);
            command_arguments += MakePathForCommandLine(outputFolder);
            for (int a=0;a< videoFilenameList.Count; a++)
            {
                string videoFile = videoFilenameList[a];
                string outputVideoFolder = GetExtractFolder(videoFile);
                command_arguments += MakePathForCommandLine(outputVideoFolder);
            }


            tasks[nbTask - 1].app_args = command_arguments;

            //"ExtractMultiVideo", console_app_name, command_arguments
            task_manager.RunCommandLine_MultiTask(tasks);
        }

        private void btn_multi_video_select_first_Click(object sender, EventArgs e)
        {
            string filter = "MP4 files (*.mp4)|*.mp4|All files (*.*)|*.*";
            text_first_video.Text = ConsoleFunction.select_file(filter);
        }

        private void btn_multi_video_select_Click(object sender, EventArgs e)
        {
            string filter = "MP4 files (*.mp4)|*.mp4|All files (*.*)|*.*";
            string addFilename = ConsoleFunction.select_file(filter);
            if (addFilename == text_first_video.Text)
                return;
            foreach(var node in list_multi_video.Items)
            {
                if(node.ToString()  == addFilename)
                {
                    return;
                }
            }
            list_multi_video.Items.Add(addFilename);
        }

        private void btn_remove_video_Click(object sender, EventArgs e)
        {
            int idx = list_multi_video.SelectedIndex;
            if (idx < 0)
                return;
            list_multi_video.Items.RemoveAt(idx);
        }

        private void btn_multi_video_dest_folder_Click(object sender, EventArgs e)
        {
            text_multi_video_output.Text = ConsoleFunction.select_folder();
        }

        private void btn_build_sec_select_folder_Click(object sender, EventArgs e)
        {
            text_sections_images_folder.Text = ConsoleFunction.select_folder();
            text_sections_images_folder.ReadOnly = false;
        }

        private void btn_build_sec_image_Click(object sender, EventArgs e)
        {
            string nbImgStr = combo_nb_sec_image.SelectedItem.ToString();
            int nbImg = Convert.ToInt32(nbImgStr);
            int idx = combo_select_image_step.SelectedIndex;
            if (idx < 0)
                return;
            int step = idx + 1;
            string command_arguments = "13 ";
            command_arguments += MakePathForCommandLine(text_sections_images_folder.Text);
            command_arguments += nbImg.ToString();
            command_arguments += " ";
            command_arguments += step.ToString();
            if(check_sections_copy.Checked)
            {
                command_arguments += " 1";
            }
            else
            {
                command_arguments += " 0";
            }

            task_manager.RunCommandLine_Task("SelectImages", console_app_name, command_arguments);
        }

        private void btn_cr_build_start_Click(object sender, EventArgs e)
        {
            string command_arguments = "14 ";
            command_arguments += MakePathForCommandLine(text_cr_proj_file.Text);
            command_arguments += MakePathForCommandLine(text_cr_img_folder.Text);
            if(!ConsoleFunction.check_folder(text_cr_img_folder2, ""))
            {
                command_arguments += MakePathForCommandLine(text_cr_img_folder2.Text);
            }
            if (!ConsoleFunction.check_folder(text_cr_img_folder3, ""))
            {
                command_arguments += MakePathForCommandLine(text_cr_img_folder3.Text);
            }
            if (!ConsoleFunction.check_folder(text_cr_img_folder4, ""))
            {
                command_arguments += MakePathForCommandLine(text_cr_img_folder4.Text);
            }
            task_manager.RunCommandLine_Task("ExtendCRImages", console_app_name, command_arguments);
        }

        private void btn_cr_sel_cr_prj_Click(object sender, EventArgs e)
        {
            string filter = "rc proj (*.rcproj)|*.rcproj|All files (*.*)|*.*";
            text_cr_proj_file.Text = ConsoleFunction.select_file(filter);
        }

        private void btn_cr_sel_img_folder_Click(object sender, EventArgs e)
        {
            text_cr_img_folder.Text = ConsoleFunction.select_folder();
        }

        private void btn_cr_sel_img_folder4_Click(object sender, EventArgs e)
        {
            text_cr_img_folder4.Text = ConsoleFunction.select_folder();
        }

        private void btn_cr_sel_img_folder3_Click(object sender, EventArgs e)
        {
            text_cr_img_folder3.Text = ConsoleFunction.select_folder();
        }

        private void btn_cr_sel_img_folder2_Click(object sender, EventArgs e)
        {
            text_cr_img_folder2.Text = ConsoleFunction.select_folder();
        }

       

      

        

        

       

        // Tony
        public List<string> checkPaksFile(string dir)
        {
            try
            {
                DirectoryInfo d = new DirectoryInfo(Path.Combine(dir, "WindowsNoEditor", "RallyTemplate", "Content", "Paks"));
                //FileInfo[] pakFiles = d.GetFiles("*.pak");
                FileInfo[] binFiles = d.GetFiles("*.bin");

                foreach (FileInfo file in binFiles)
                {
                    filesnames.Add(file.Name.Split('_')[0]);
                }
                Program.AddLog("Files in paks folder checked");

                return filesnames;
            } catch (Exception e)
            {
                Program.AddLog("Please enter the right directory");
                return null;
            }
        }


        // Tony
        private void selectShippingVersion_Click(object sender, EventArgs e)
        {
            text_sections_shippingVersion.Text = ConsoleFunction.select_folder();
            textBox1.ReadOnly = false;
            checkPaksFile(text_sections_shippingVersion.Text);
            foreach (string name in filesnames)
            {
               
                listbox_sceneNames.Items.Add(name);
            }
        }


        


        // Tony
        private void button_start_packing_Click(object sender, EventArgs e)
        {
            string command_arguments = "15 ";
            // todo: add command arguments in this series: shipping version, packdir, scenename 
            command_arguments += text_sections_shippingVersion.Text;
            command_arguments += " ";
            ListBox.SelectedIndexCollection index = listbox_sceneNames.SelectedIndices;

            // still need a scene name
            /*for (int i = 0; i < listbox_sceneNames.Items.Count; i++)
            {
                if (index.Contains(i))
                {
                    command_arguments += listbox_sceneNames.Items[i];
                    command_arguments += " ";
                }
            }*/
            
            command_arguments += (listbox_sceneNames.GetItemText(listbox_sceneNames.SelectedItem) + " ");

            //TaskManager tasks = new TaskManager();
            task_manager.RunCommandLine_Task("ScenePack", "Console\\CoDriverConsoleApp.exe", command_arguments);
            
        }


      

        private void checkBox_xg_fixed_90_CheckedChanged(object sender, EventArgs e)
        {
            if(checkBox_xg_fixed_90.Checked)
            {
                text_xmp_slave0.Enabled = false;
                text_xmp_slave1.Enabled = false;
                text_xmp_slave2.Enabled = false;
                btn_select_xmp_slave0.Enabled = false;
                btn_select_xmp_slave1.Enabled = false;
                btn_select_xmp_slave2.Enabled = false;
            }
            else
            {
                text_xmp_slave0.Enabled = true;
                text_xmp_slave1.Enabled = true;
                text_xmp_slave2.Enabled = true;
                btn_select_xmp_slave0.Enabled = true;
                btn_select_xmp_slave1.Enabled = true;
                btn_select_xmp_slave2.Enabled = true;
            }
        }

        private void btn_xg_start_gps_Click(object sender, EventArgs e)
        {
            string command_arguments = "10 3 ";
            command_arguments += MakePathForCommandLine(text_xg_image_folder_gps.Text);
            command_arguments += MakePathForCommandLine(text_xg_gps_file.Text);

            task_manager.RunCommandLine_Task("XmpGenerater", console_app_name, command_arguments);
        }

        private void btn_xg_select_img_folder_gps_Click(object sender, EventArgs e)
        {
            text_xg_image_folder_gps.Text = ConsoleFunction.select_folder();
        }

        private void btn_xg_select_gps_file_Click(object sender, EventArgs e)
        {
            string filter = "CSV (*.csv)|*.csv|All files (*.*)|*.*";
            text_xg_gps_file.Text = ConsoleFunction.select_file(filter);
        }

        private void button_selectVideo_Click(object sender, EventArgs e)
        {
            text_sections_video.Text = ConsoleFunction.select_file("");
            text_sections_video.ReadOnly = false;
            listbox_cubemapSize.Items.Clear();
            listbox_cubemapSize.Items.Add("480x480");
            listbox_cubemapSize.Items.Add("512x512");
            listbox_cubemapSize.Items.Add("1024x1024");
            listbox_cubemapSize.Items.Add("1440x1440");

        }

      

  



        // Note: the videos should be selected together before the stage of extracting cubemap, or there could be some unexpected behaviour
        private void extractStarted_Click(object sender, EventArgs e)
        {
            cubemapcalled++;
            extractedcubemap = true;

            string output_folder = Path.Combine(Directory.GetParent(text_sections_video.Text).ToString(), Path.GetFileNameWithoutExtension(text_sections_video.Text));
            Directory.CreateDirectory(output_folder);

            string cubemap_output = Path.Combine(Directory.GetParent(text_sections_video.Text).ToString(), "OutputCubeMap");
            string app_name = "FFmpeg\\ffmpeg.exe";

            if (!Directory.Exists(output_folder))
            {
                Directory.CreateDirectory(output_folder);
            }

            if (true)
            {
                process_task[] tasks;
                int nbTask = 1;
                if (checkbox_cubemap.Checked) nbTask++;
                if (check_movie_undistort.Checked)
                    nbTask++;
                if (check_movie_backup.Checked)
                    nbTask++;
                if (check_add_gps_to_image.Checked || check_movie_xmp_file.Checked)
                    nbTask++;
                tasks = new process_task[nbTask];
                //task_manager.RunCommandLine_MultiTask(tasks);
                currentidx = task_manager.RunCommandLine_Task("ExtractImages", app_name, BuildCommandArguments.build_args_ffmpeg(text_sections_video.Text, output_folder, "Front"));
                string cubemapoutputer = cubemap_output + "-" + Path.GetFileNameWithoutExtension(text_sections_video.Text);
                int cubemapSizer = Int32.Parse(listbox_cubemapSize.GetItemText(listbox_cubemapSize.SelectedItem).Split('x')[0]);



                
                extractingTasks.Add(cubemapcalled - 1, new VideoFile(cubemapoutputer, output_folder,  cubemapSizer, currentidx));



                /*ListBox.SelectedIndexCollection index = listbox_cubemapSize.SelectedIndices;
                for (int i = 0; i < listbox_cubemapSize.Items.Count; i++)
                {
                    if (index.Contains(i))
                    {
                        cubemapsize = Int32.Parse(listbox_cubemapSize.Items[i].ToString().Split('x')[0]);
                    }
                }*/

                
             
               






            }


        }

        public void cubemapGenerator(string output_folder, string cubemap_output, int cubemapsize, int startid, int endid) {
            if (checkbox_cubemap.Checked)
            {
                
                
                    string command_arguments = "11 ";
                    command_arguments += MakePathForCommandLine(output_folder + "\\" + "\\");
                    command_arguments += " ";



                    /*string frontFolder = Path.Combine(cubemap_output, "Front");
                    string topFolder = Path.Combine(cubemap_output, "Top");
                    string downFolder = Path.Combine(cubemap_output, "Down");
                    string leftFolder = Path.Combine(cubemap_output, "Left");
                    string rightFolder = Path.Combine(cubemap_output, "Right");
                    string backFolder = Path.Combine(cubemap_output, "Back");*/



                    command_arguments += MakePathForCommandLine(cubemap_output);
                    command_arguments += " ";
                    // ListBox.SelectedIndexCollection index = listbox_cubemapSize.SelectedIndices;

                        
                        
                    command_arguments += cubemapsize;
                    command_arguments += " ";

                    command_arguments += startid;
                    command_arguments += " ";
                    command_arguments += endid;
                    command_arguments += " ";

               

                    task_manager.RunCommandLine_Task("extractCubemap", "OpenCV\\OpenCVConsoleApp.exe", command_arguments);
                        
                

                
            }
        }

    

   
   


        

        

        

        

        private void button_selectImageDir_Click(object sender, EventArgs e)
        {
            text_ImageDir.Text = ConsoleFunction.select_folder();
        }

        private void button_selectCSVFile_Click(object sender, EventArgs e)
        {
            text_csvFile.Text = ConsoleFunction.select_file("");
        }

        private void button_selectSceneName_Click(object sender, EventArgs e)
        {
            text_sceneName.Text = ConsoleFunction.select_file("");
        }

        private void button_startGroundVU_Click(object sender, EventArgs e)
        {
            string command_arguments = "16 ";
            
            command_arguments += text_ImageDir.Text;
            command_arguments += " ";
            command_arguments += text_csvFile.Text;
            command_arguments += " ";
            command_arguments += text_sceneName.Text;

            task_manager.RunCommandLine_Task("GroundVU", "Console\\CoDriverConsoleApp.exe", command_arguments);
        }

        private void GroundVU_Paint(object sender, PaintEventArgs e)
        {

        }

        private void btn_crc_creation_start_Click(object sender, EventArgs e)
        {
            string command_arguments = "17 ";
            command_arguments += MakePathForCommandLine(text_crc_frontimages.Text);

            if (text_crc_leftimages.Text.Length < 3)
                command_arguments += MakePathForCommandLine(text_crc_leftimages.Text);
            else
                command_arguments += MakePathForCommandLine("NULL");

            if (text_crc_rightimages.Text.Length < 3)
                command_arguments += MakePathForCommandLine(text_crc_rightimages.Text);
            else
                command_arguments += MakePathForCommandLine("NULL");

            if (text_crc_backimages.Text.Length < 3)
                command_arguments += MakePathForCommandLine(text_crc_backimages.Text);
            else
                command_arguments += MakePathForCommandLine("NULL");
            
            command_arguments += combo_crc_section.SelectedIndex;
            command_arguments += " ";
            command_arguments += text_crc_scenename.Text;
            //command_arguments += " ";
            //command_arguments += ConsoleFunction.GetCombo_StepGap(combo_crc_front);
            //command_arguments += " ";
            //command_arguments += ConsoleFunction.GetCombo_StepGap(combo_crc_left);
            //command_arguments += " ";
            //command_arguments += ConsoleFunction.GetCombo_StepGap(combo_crc_right);
            //command_arguments += " ";
            //command_arguments += ConsoleFunction.GetCombo_StepGap(combo_crc_back);

            task_manager.RunCommandLine_Task("CRProjCreation", console_app_name, command_arguments);
        }

        private void btn_crc_backimages_Click(object sender, EventArgs e)
        {
            text_crc_backimages.Text = ConsoleFunction.select_folder();
        }

        private void btn_crc_rightimages_Click(object sender, EventArgs e)
        {
            text_crc_rightimages.Text = ConsoleFunction.select_folder();
        }

        private void btn_crc_leftimages_Click(object sender, EventArgs e)
        {
            text_crc_leftimages.Text = ConsoleFunction.select_folder();
        }

        private void btn_crc_frontimages_Click(object sender, EventArgs e)
        {
            text_crc_frontimages.Text = ConsoleFunction.select_folder();
        }

        private void FunctionlistBox_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        

        

        private void button_selectProjectFile_Click(object sender, EventArgs e)
        {
            textBox_ProjectFileLocation.Text = ConsoleFunction.select_file("");
        }

        private void button_selectGlobalSettings_Click(object sender, EventArgs e)
        {
            textBox_GlobalSettingLocation.Text = ConsoleFunction.select_file("");
        }

        private void button_StartCreateMesh_Click(object sender, EventArgs e)
        {
            string command_arguments = "18 ";
            command_arguments += MakePathForCommandLine(textBox_GlobalSettingLocation.Text);
            command_arguments += " ";
            command_arguments += MakePathForCommandLine(textBox_ProjectFileLocation.Text);
            command_arguments += " ";

            int ActionSelected = comboBox_CRactionsNeeded.SelectedIndex;

            command_arguments += ActionSelected;
            command_arguments += " ";

            if (ActionSelected == 3)
            {
                command_arguments += text_targetTriangleCount.Text;
            }

            

            task_manager.RunCommandLine_Task("CRMeshCreation", console_app_name, command_arguments);
        }
    }
}
