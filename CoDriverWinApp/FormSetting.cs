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

namespace CoDriverWinApp
{
    public partial class FormSetting : Form
    {
        public FormSetting()
        {
            InitializeComponent();
            Timer timer = new Timer();
            timer.Interval = (300);
            timer.Tick += new EventHandler(Tick);
            timer.Start();
            string prjName = Path.Combine(Properties.Settings.Default.ProjectFolder, Properties.Settings.Default.ProjectName);
            prjName = Path.ChangeExtension(prjName, "uproject");
            text_fs_ue4prj.Text = prjName;
            text_fs_capturereality.Text = Properties.Settings.Default.CapturingReality;
            text_fs_crproj.Text = Properties.Settings.Default.CRProjectFolder;
            text_fs_engine.Text = Properties.Settings.Default.UE4Engine;
            text_fs_workfolder.Text = Properties.Settings.Default.WorkFolder;
            text_fs_hostip.Text = Properties.Settings.Default.HostIP;
            // text_fs_ue4prj.Text = Properties.Settings.Default.;
        }
        private void Tick(object sender, EventArgs e)
        {
            ConsoleFunction.check_file(text_fs_ue4prj);
            ConsoleFunction.check_folder(text_fs_engine, "");


            ConsoleFunction.check_folder(text_fs_workfolder, "");

        }

        private void btn_fs_select_prj_Click(object sender, EventArgs e)
        {
            string filter = "UProject files (*.uproject)|*.uproject|All files (*.*)|*.*";
            text_fs_ue4prj.Text = ConsoleFunction.select_file(filter);
            
        }

        private void btn_fs_select_engine_Click(object sender, EventArgs e)
        {
            text_fs_engine.Text = ConsoleFunction.select_folder();
        }

        private void btn_fs_done_Click(object sender, EventArgs e)
        {
            string prjFolder = Path.GetDirectoryName(text_fs_ue4prj.Text);
            string prjName = Path.GetFileNameWithoutExtension(text_fs_ue4prj.Text);
            //prjName = Path.GetFileNameWithoutExtension(prjName)
            Properties.Settings.Default.ProjectFolder = prjFolder;
            Properties.Settings.Default.ProjectName = prjName;
            string ue4Engine = text_fs_engine.Text;
            Properties.Settings.Default.UE4Engine = ue4Engine;
            string CRFolder = text_fs_capturereality.Text;
            Properties.Settings.Default.CapturingReality = CRFolder;
            string CRPrjFolder = text_fs_crproj.Text;
            Properties.Settings.Default.CRProjectFolder = CRPrjFolder;


            string GroundVUFile = textbox_GroundVU.Text;
            string workFolder = text_fs_workfolder.Text;
            Properties.Settings.Default.WorkFolder = workFolder;
            Properties.Settings.Default.HostIP = text_fs_hostip.Text;

            Properties.Settings.Default.Save();

            string command_arguments = "0 ";
            command_arguments += BuildCommandArguments.MakePathForCommandLine(prjFolder);
            command_arguments += BuildCommandArguments.MakePathForCommandLine(prjName);
            command_arguments += BuildCommandArguments.MakePathForCommandLine(ue4Engine);
            command_arguments += BuildCommandArguments.MakePathForCommandLine(CRFolder);
            command_arguments += BuildCommandArguments.MakePathForCommandLine(CRPrjFolder);
            command_arguments += BuildCommandArguments.MakePathForCommandLine(GroundVUFile);
            command_arguments += BuildCommandArguments.MakePathForCommandLine(workFolder);

            Form1.task_manager.RunCommandLine_Task("Setting", "Console\\CoDriverConsoleApp.exe", command_arguments);

            this.Close();
        }

        private void btn_fs_capturereality_Click(object sender, EventArgs e)
        {
            text_fs_capturereality.Text = ConsoleFunction.select_folder();
        }

        private void btn_fs_crproj_Click(object sender, EventArgs e)
        {
            text_fs_crproj.Text = ConsoleFunction.select_folder();
        }

        private void btn_fs_select_wf_Click(object sender, EventArgs e)
        {
            text_fs_workfolder.Text = ConsoleFunction.select_folder();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button_selectGroundVU_Click(object sender, EventArgs e)
        {
            textbox_GroundVU.Text = ConsoleFunction.select_file("");
        }

        private void text_fs_ue4prj_TextChanged(object sender, EventArgs e)
        {

        }

        private void text_fs_workfolder_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
