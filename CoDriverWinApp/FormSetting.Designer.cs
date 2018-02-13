namespace CoDriverWinApp
{
    partial class FormSetting
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btn_fs_select_prj = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.text_fs_ue4prj = new System.Windows.Forms.TextBox();
            this.text_fs_engine = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btn_fs_select_engine = new System.Windows.Forms.Button();
            this.btn_fs_done = new System.Windows.Forms.Button();
            this.text_fs_capturereality = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.btn_fs_capturereality = new System.Windows.Forms.Button();
            this.text_fs_crproj = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.btn_fs_crproj = new System.Windows.Forms.Button();
            this.text_fs_workfolder = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.btn_fs_select_wf = new System.Windows.Forms.Button();
            this.text_fs_hostip = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label_groundVU = new System.Windows.Forms.Label();
            this.textbox_GroundVU = new System.Windows.Forms.TextBox();
            this.button_selectGroundVU = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btn_fs_select_prj
            // 
            this.btn_fs_select_prj.Location = new System.Drawing.Point(387, 10);
            this.btn_fs_select_prj.Name = "btn_fs_select_prj";
            this.btn_fs_select_prj.Size = new System.Drawing.Size(75, 21);
            this.btn_fs_select_prj.TabIndex = 0;
            this.btn_fs_select_prj.Text = "Select";
            this.btn_fs_select_prj.UseVisualStyleBackColor = true;
            this.btn_fs_select_prj.Click += new System.EventHandler(this.btn_fs_select_prj_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(14, 14);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(71, 12);
            this.label1.TabIndex = 1;
            this.label1.Text = "UE4Project:";
            // 
            // text_fs_ue4prj
            // 
            this.text_fs_ue4prj.Location = new System.Drawing.Point(78, 12);
            this.text_fs_ue4prj.Name = "text_fs_ue4prj";
            this.text_fs_ue4prj.Size = new System.Drawing.Size(305, 21);
            this.text_fs_ue4prj.TabIndex = 2;
            this.text_fs_ue4prj.TextChanged += new System.EventHandler(this.text_fs_ue4prj_TextChanged);
            // 
            // text_fs_engine
            // 
            this.text_fs_engine.Location = new System.Drawing.Point(76, 43);
            this.text_fs_engine.Name = "text_fs_engine";
            this.text_fs_engine.Size = new System.Drawing.Size(305, 21);
            this.text_fs_engine.TabIndex = 5;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 45);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 12);
            this.label2.TabIndex = 4;
            this.label2.Text = "UE4Engine:";
            // 
            // btn_fs_select_engine
            // 
            this.btn_fs_select_engine.Location = new System.Drawing.Point(385, 42);
            this.btn_fs_select_engine.Name = "btn_fs_select_engine";
            this.btn_fs_select_engine.Size = new System.Drawing.Size(75, 21);
            this.btn_fs_select_engine.TabIndex = 3;
            this.btn_fs_select_engine.Text = "Select";
            this.btn_fs_select_engine.UseVisualStyleBackColor = true;
            this.btn_fs_select_engine.Click += new System.EventHandler(this.btn_fs_select_engine_Click);
            // 
            // btn_fs_done
            // 
            this.btn_fs_done.Location = new System.Drawing.Point(375, 267);
            this.btn_fs_done.Name = "btn_fs_done";
            this.btn_fs_done.Size = new System.Drawing.Size(89, 29);
            this.btn_fs_done.TabIndex = 6;
            this.btn_fs_done.Text = "Done";
            this.btn_fs_done.UseVisualStyleBackColor = true;
            this.btn_fs_done.Click += new System.EventHandler(this.btn_fs_done_Click);
            // 
            // text_fs_capturereality
            // 
            this.text_fs_capturereality.Location = new System.Drawing.Point(96, 79);
            this.text_fs_capturereality.Name = "text_fs_capturereality";
            this.text_fs_capturereality.Size = new System.Drawing.Size(284, 21);
            this.text_fs_capturereality.TabIndex = 9;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(11, 81);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(95, 12);
            this.label3.TabIndex = 8;
            this.label3.Text = "CaptureReality:";
            // 
            // btn_fs_capturereality
            // 
            this.btn_fs_capturereality.Location = new System.Drawing.Point(384, 78);
            this.btn_fs_capturereality.Name = "btn_fs_capturereality";
            this.btn_fs_capturereality.Size = new System.Drawing.Size(75, 21);
            this.btn_fs_capturereality.TabIndex = 7;
            this.btn_fs_capturereality.Text = "Select";
            this.btn_fs_capturereality.UseVisualStyleBackColor = true;
            this.btn_fs_capturereality.Click += new System.EventHandler(this.btn_fs_capturereality_Click);
            // 
            // text_fs_crproj
            // 
            this.text_fs_crproj.Location = new System.Drawing.Point(97, 113);
            this.text_fs_crproj.Name = "text_fs_crproj";
            this.text_fs_crproj.Size = new System.Drawing.Size(284, 21);
            this.text_fs_crproj.TabIndex = 12;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 114);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(89, 12);
            this.label4.TabIndex = 11;
            this.label4.Text = "CRProj Folder:";
            // 
            // btn_fs_crproj
            // 
            this.btn_fs_crproj.Location = new System.Drawing.Point(385, 111);
            this.btn_fs_crproj.Name = "btn_fs_crproj";
            this.btn_fs_crproj.Size = new System.Drawing.Size(75, 21);
            this.btn_fs_crproj.TabIndex = 10;
            this.btn_fs_crproj.Text = "Select";
            this.btn_fs_crproj.UseVisualStyleBackColor = true;
            this.btn_fs_crproj.Click += new System.EventHandler(this.btn_fs_crproj_Click);
            // 
            // text_fs_workfolder
            // 
            this.text_fs_workfolder.Location = new System.Drawing.Point(96, 192);
            this.text_fs_workfolder.Name = "text_fs_workfolder";
            this.text_fs_workfolder.Size = new System.Drawing.Size(284, 21);
            this.text_fs_workfolder.TabIndex = 15;
            this.text_fs_workfolder.TextChanged += new System.EventHandler(this.text_fs_workfolder_TextChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(11, 194);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(77, 12);
            this.label5.TabIndex = 14;
            this.label5.Text = "Work Folder:";
            // 
            // btn_fs_select_wf
            // 
            this.btn_fs_select_wf.Location = new System.Drawing.Point(384, 190);
            this.btn_fs_select_wf.Name = "btn_fs_select_wf";
            this.btn_fs_select_wf.Size = new System.Drawing.Size(75, 21);
            this.btn_fs_select_wf.TabIndex = 13;
            this.btn_fs_select_wf.Text = "Select";
            this.btn_fs_select_wf.UseVisualStyleBackColor = true;
            this.btn_fs_select_wf.Click += new System.EventHandler(this.btn_fs_select_wf_Click);
            // 
            // text_fs_hostip
            // 
            this.text_fs_hostip.Location = new System.Drawing.Point(98, 233);
            this.text_fs_hostip.Name = "text_fs_hostip";
            this.text_fs_hostip.Size = new System.Drawing.Size(100, 21);
            this.text_fs_hostip.TabIndex = 17;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(15, 234);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(53, 12);
            this.label6.TabIndex = 16;
            this.label6.Text = "Host IP:";
            // 
            // label_groundVU
            // 
            this.label_groundVU.AutoSize = true;
            this.label_groundVU.Location = new System.Drawing.Point(11, 151);
            this.label_groundVU.Name = "label_groundVU";
            this.label_groundVU.Size = new System.Drawing.Size(59, 12);
            this.label_groundVU.TabIndex = 18;
            this.label_groundVU.Text = "GroundVU:";
            // 
            // textbox_GroundVU
            // 
            this.textbox_GroundVU.Location = new System.Drawing.Point(98, 151);
            this.textbox_GroundVU.Name = "textbox_GroundVU";
            this.textbox_GroundVU.Size = new System.Drawing.Size(282, 21);
            this.textbox_GroundVU.TabIndex = 19;
            this.textbox_GroundVU.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // button_selectGroundVU
            // 
            this.button_selectGroundVU.Location = new System.Drawing.Point(387, 151);
            this.button_selectGroundVU.Name = "button_selectGroundVU";
            this.button_selectGroundVU.Size = new System.Drawing.Size(75, 23);
            this.button_selectGroundVU.TabIndex = 20;
            this.button_selectGroundVU.Text = "Select";
            this.button_selectGroundVU.UseVisualStyleBackColor = true;
            this.button_selectGroundVU.Click += new System.EventHandler(this.button_selectGroundVU_Click);
            // 
            // FormSetting
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(472, 302);
            this.Controls.Add(this.button_selectGroundVU);
            this.Controls.Add(this.textbox_GroundVU);
            this.Controls.Add(this.label_groundVU);
            this.Controls.Add(this.text_fs_hostip);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.text_fs_workfolder);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.btn_fs_select_wf);
            this.Controls.Add(this.text_fs_crproj);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.btn_fs_crproj);
            this.Controls.Add(this.text_fs_capturereality);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.btn_fs_capturereality);
            this.Controls.Add(this.btn_fs_done);
            this.Controls.Add(this.text_fs_engine);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btn_fs_select_engine);
            this.Controls.Add(this.text_fs_ue4prj);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btn_fs_select_prj);
            this.Name = "FormSetting";
            this.Text = "FormSetting";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btn_fs_select_prj;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox text_fs_ue4prj;
        private System.Windows.Forms.TextBox text_fs_engine;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btn_fs_select_engine;
        private System.Windows.Forms.Button btn_fs_done;
        private System.Windows.Forms.TextBox text_fs_capturereality;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btn_fs_capturereality;
        private System.Windows.Forms.TextBox text_fs_crproj;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btn_fs_crproj;
        private System.Windows.Forms.TextBox text_fs_workfolder;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button btn_fs_select_wf;
        private System.Windows.Forms.TextBox text_fs_hostip;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label_groundVU;
        private System.Windows.Forms.TextBox textbox_GroundVU;
        private System.Windows.Forms.Button button_selectGroundVU;
    }
}