namespace CoDriverWinApp
{
    partial class FormUploadVideo
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
            this.btn_f2_upload = new System.Windows.Forms.Button();
            this.btn_f2_cancel = new System.Windows.Forms.Button();
            this.label_video_path = new System.Windows.Forms.Label();
            this.list_f2_list_camera = new System.Windows.Forms.ListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.check_use_undistort = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.text_scene_name = new System.Windows.Forms.TextBox();
            this.text_uploader_name = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.btn_f2_select = new System.Windows.Forms.Button();
            this.label_fuv_info = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btn_f2_upload
            // 
            this.btn_f2_upload.Location = new System.Drawing.Point(253, 334);
            this.btn_f2_upload.Name = "btn_f2_upload";
            this.btn_f2_upload.Size = new System.Drawing.Size(105, 37);
            this.btn_f2_upload.TabIndex = 0;
            this.btn_f2_upload.Text = "Upload";
            this.btn_f2_upload.UseVisualStyleBackColor = true;
            this.btn_f2_upload.Click += new System.EventHandler(this.btn_f2_upload_Click);
            // 
            // btn_f2_cancel
            // 
            this.btn_f2_cancel.Location = new System.Drawing.Point(397, 334);
            this.btn_f2_cancel.Name = "btn_f2_cancel";
            this.btn_f2_cancel.Size = new System.Drawing.Size(105, 37);
            this.btn_f2_cancel.TabIndex = 1;
            this.btn_f2_cancel.Text = "Close";
            this.btn_f2_cancel.UseVisualStyleBackColor = true;
            this.btn_f2_cancel.Click += new System.EventHandler(this.btn_f2_cancel_Click);
            // 
            // label_video_path
            // 
            this.label_video_path.AutoSize = true;
            this.label_video_path.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.label_video_path.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_video_path.Location = new System.Drawing.Point(22, 24);
            this.label_video_path.Name = "label_video_path";
            this.label_video_path.Size = new System.Drawing.Size(45, 16);
            this.label_video_path.TabIndex = 2;
            this.label_video_path.Text = "label1";
            // 
            // list_f2_list_camera
            // 
            this.list_f2_list_camera.FormattingEnabled = true;
            this.list_f2_list_camera.Location = new System.Drawing.Point(89, 179);
            this.list_f2_list_camera.Name = "list_f2_list_camera";
            this.list_f2_list_camera.Size = new System.Drawing.Size(137, 121);
            this.list_f2_list_camera.TabIndex = 4;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(83, 159);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(79, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Select Camera:";
            // 
            // check_use_undistort
            // 
            this.check_use_undistort.AutoSize = true;
            this.check_use_undistort.Location = new System.Drawing.Point(83, 139);
            this.check_use_undistort.Name = "check_use_undistort";
            this.check_use_undistort.Size = new System.Drawing.Size(101, 17);
            this.check_use_undistort.TabIndex = 6;
            this.check_use_undistort.Text = "Undistort Param";
            this.check_use_undistort.UseVisualStyleBackColor = true;
            this.check_use_undistort.CheckedChanged += new System.EventHandler(this.check_use_undistort_CheckedChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(60, 77);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(38, 13);
            this.label2.TabIndex = 7;
            this.label2.Text = "Name:";
            // 
            // text_scene_name
            // 
            this.text_scene_name.Location = new System.Drawing.Point(101, 74);
            this.text_scene_name.Name = "text_scene_name";
            this.text_scene_name.Size = new System.Drawing.Size(128, 20);
            this.text_scene_name.TabIndex = 8;
            // 
            // text_uploader_name
            // 
            this.text_uploader_name.Location = new System.Drawing.Point(102, 101);
            this.text_uploader_name.Name = "text_uploader_name";
            this.text_uploader_name.Size = new System.Drawing.Size(128, 20);
            this.text_uploader_name.TabIndex = 10;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(49, 104);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 13);
            this.label3.TabIndex = 9;
            this.label3.Text = "Uploader:";
            // 
            // btn_f2_select
            // 
            this.btn_f2_select.Location = new System.Drawing.Point(105, 334);
            this.btn_f2_select.Name = "btn_f2_select";
            this.btn_f2_select.Size = new System.Drawing.Size(105, 37);
            this.btn_f2_select.TabIndex = 11;
            this.btn_f2_select.Text = "Select Video";
            this.btn_f2_select.UseVisualStyleBackColor = true;
            this.btn_f2_select.Click += new System.EventHandler(this.btn_f2_select_Click);
            // 
            // label_fuv_info
            // 
            this.label_fuv_info.AutoSize = true;
            this.label_fuv_info.ForeColor = System.Drawing.Color.Red;
            this.label_fuv_info.Location = new System.Drawing.Point(49, 311);
            this.label_fuv_info.Name = "label_fuv_info";
            this.label_fuv_info.Size = new System.Drawing.Size(35, 13);
            this.label_fuv_info.TabIndex = 12;
            this.label_fuv_info.Text = "label4";
            // 
            // FormUploadVideo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(529, 385);
            this.Controls.Add(this.label_fuv_info);
            this.Controls.Add(this.btn_f2_select);
            this.Controls.Add(this.text_uploader_name);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.text_scene_name);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.check_use_undistort);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.list_f2_list_camera);
            this.Controls.Add(this.label_video_path);
            this.Controls.Add(this.btn_f2_cancel);
            this.Controls.Add(this.btn_f2_upload);
            this.Name = "FormUploadVideo";
            this.Text = "FormUploadVideo";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btn_f2_upload;
        private System.Windows.Forms.Button btn_f2_cancel;
        private System.Windows.Forms.Label label_video_path;
        private System.Windows.Forms.ListBox list_f2_list_camera;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox check_use_undistort;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox text_scene_name;
        private System.Windows.Forms.TextBox text_uploader_name;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btn_f2_select;
        private System.Windows.Forms.Label label_fuv_info;
    }
}