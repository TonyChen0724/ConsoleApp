using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;
using System.Windows.Forms;

namespace CoDriverWinApp
{
    class ConsoleFunction
    {
        static public ConsoleFunction[] gs_func; 
        static public Color default_backcolor;
        static public Color default_forecolor;
        static string lastSelectedFolder = @"c:\";
        public int idx;
        public string name;
        public Panel panel_form;
        public int itemIdx;
        public ConsoleFunction()
        {

        }


        static Point panelPos = new Point(200, 3);
        public void UpdateSelect(bool isSelect)
        {
            if(isSelect)
            {
                panel_form.Location = panelPos;
                panel_form.Visible = true;
            }
            else
            {
                panel_form.Visible = false;
            }
        }
        
        static public void Init(int size)
        {
            gs_func = new ConsoleFunction[size];
        }

        static public void UpdateSelectFunc(int nSelect)
        {
            foreach(var n in gs_func)
            {
                    if (n.itemIdx == nSelect)
                        n.UpdateSelect(true);
                    else
                        n.UpdateSelect(false);
            }
        }
        static public string select_folder()
        {
            using (var fbd = new FolderBrowserDialog())
            {
                fbd.SelectedPath = lastSelectedFolder;
                DialogResult result = fbd.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    lastSelectedFolder = fbd.SelectedPath;
                    return fbd.SelectedPath;
                }
            }
            return "";
        }
        static public string select_file(string filter)
        {
            using (var ofd = new OpenFileDialog())
            {
                DialogResult result = ofd.ShowDialog();
                ofd.InitialDirectory = lastSelectedFolder;
                ofd.Filter = filter;// "XML files (*.xml)|*.xml|All files (*.*)|*.*";
                ofd.FilterIndex = 1;
                ofd.RestoreDirectory = true;

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(ofd.FileName))
                {
                    lastSelectedFolder = Path.GetFullPath(ofd.FileName);
                    return ofd.FileName;
                }
            }
            return "";
        }
        static public int GetCombo_StepGap(System.Windows.Forms.ComboBox combo_control)
        {
            int n = combo_control.SelectedIndex;
            if (n == 0)
                return 1;
            if (n == 1)
                return 2;
            if (n == 2)
                return 4;
            if (n == 3)
                return 5;
            return 0;
        }
        static public void InitStepGapCombo(System.Windows.Forms.ComboBox combo_control,int defaultIdx)
        {
            combo_control.Items.Add("1/1");
            combo_control.Items.Add("1/2");
           // combo_control.Items.Add("1/3");
            combo_control.Items.Add("1/4");
            combo_control.Items.Add("1/5");
            combo_control.SelectedIndex = defaultIdx;
            return;
        }
        static public void InitKeywordCombo(System.Windows.Forms.ComboBox combo_control)
        {
            combo_control.Items.Add("Front");
            combo_control.Items.Add("Left");
            combo_control.Items.Add("Right");
            combo_control.Items.Add("Rear");
            combo_control.SelectedIndex = 0;
            return;
        }
        static public bool check_folder(System.Windows.Forms.TextBox text_control, string append)
        {
            bool isError = false;
            string folder = text_control.Text;
            if(append.Length > 1)
                folder = Path.Combine(folder, append);
            if (!Directory.Exists(text_control.Text))
            {
                text_control.ForeColor = Color.Red;
                text_control.BackColor = Color.Yellow;
                isError = true;
            }
            else
            {
                text_control.ForeColor = default_forecolor;
                text_control.BackColor = default_backcolor;
            }

            return isError;
        }

        static public bool check_scene_name(System.Windows.Forms.TextBox text_control)
        {
            bool isError = false;
            if (text_control.Text.Equals("") || !(System.Text.RegularExpressions.Regex.IsMatch(text_control.Text, "^[a-zA-Z0-9\x20]+$")) || text_control.Text.Contains(" "))
            {
                text_control.ForeColor = Color.Red;
                text_control.BackColor = Color.Yellow;
                isError = true;
            }
            else
            {
                text_control.ForeColor = default_forecolor;
                text_control.BackColor = default_backcolor;
            }
            
            return isError;
        }

        static public bool check_double_value(System.Windows.Forms.TextBox text_control)
        {
            string destFocalLen = text_control.Text;
            double j;
            bool isError = false;
            if (!Double.TryParse(destFocalLen, out j))
            {
                text_control.ForeColor = Color.Red;
                text_control.BackColor = Color.Yellow;
                isError = true;
            }
            else
            {
                text_control.ForeColor = default_forecolor;
                text_control.BackColor = default_backcolor;
            }
            return isError;
        }

        static public bool check_keyword(System.Windows.Forms.TextBox text_control)
        {
            bool invalidKeyword = false;
            string keyword = text_control.Text;
            char[] invalidChar = Path.GetInvalidPathChars();
            foreach (var c in invalidChar)
            {
                if (keyword.Contains(c))
                {
                    invalidKeyword = true;
                }
            }
            if (keyword.Contains(' '))
                invalidKeyword = true;
            if (keyword.Length < 3)
                invalidKeyword = true;

            if (invalidKeyword)
            {
                text_control.ForeColor = Color.Red;
                text_control.BackColor = Color.Yellow;
            }
            else
            {
                text_control.ForeColor = default_forecolor;
                text_control.BackColor = default_backcolor;
            }
            return invalidKeyword;
        }

        static public bool check_file(System.Windows.Forms.TextBox text_control)
        {
            bool isError = false;
            string filename = text_control.Text;
            if (!File.Exists(filename))
            {
                isError = true;
            }
            if (isError)
            {
                text_control.ForeColor = Color.Red;
                text_control.BackColor = Color.Yellow;
            }
            else
            {
                text_control.ForeColor = default_forecolor;
                text_control.BackColor = default_backcolor;
            }
            return isError;
        }
        static public void ListUE4Scenes(ListBox list)
        {
            list.Items.Clear();

            string project_folder = Properties.Settings.Default.ProjectFolder;
            if (Directory.Exists(project_folder))
            {
                string project_scene_folder = project_folder + "\\Content\\Scene\\";
                if (Directory.Exists(project_scene_folder))
                {
                    string[] scenes_folder = Directory.GetDirectories(project_scene_folder);
                    foreach (var s in scenes_folder)
                    {
                        string str = Path.GetFileName(s);
                        list.Items.Add(str);
                    }
                }
            }
        }
    }
}
