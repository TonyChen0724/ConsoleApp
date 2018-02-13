namespace CoDriverWinApp
{
    partial class FormVideoLib
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
            this.list_fvi_videolist = new System.Windows.Forms.ListBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.richText_videoInfo = new System.Windows.Forms.RichTextBox();
            this.text_input_search = new System.Windows.Forms.TextBox();
            this.btn_fvi_copy_images = new System.Windows.Forms.Button();
            this.btn_fvi_copy_video = new System.Windows.Forms.Button();
            this.btn_fvi_update_list = new System.Windows.Forms.Button();
            this.label_process_info = new System.Windows.Forms.Label();
            this.btn_fvi_import_ue4 = new System.Windows.Forms.Button();
            this.progress_fvi_action = new System.Windows.Forms.ProgressBar();
            this.btn_fvi_copy_mesh = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // list_fvi_videolist
            // 
            this.list_fvi_videolist.FormattingEnabled = true;
            this.list_fvi_videolist.Location = new System.Drawing.Point(0, -1);
            this.list_fvi_videolist.Name = "list_fvi_videolist";
            this.list_fvi_videolist.Size = new System.Drawing.Size(350, 485);
            this.list_fvi_videolist.TabIndex = 0;
            this.list_fvi_videolist.SelectedIndexChanged += new System.EventHandler(this.list_fvi_videolist_SelectedIndexChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.richText_videoInfo);
            this.groupBox1.Location = new System.Drawing.Point(357, -1);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(574, 286);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Video Info";
            // 
            // richText_videoInfo
            // 
            this.richText_videoInfo.BackColor = System.Drawing.SystemColors.Menu;
            this.richText_videoInfo.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.richText_videoInfo.Enabled = false;
            this.richText_videoInfo.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.richText_videoInfo.Location = new System.Drawing.Point(21, 19);
            this.richText_videoInfo.Name = "richText_videoInfo";
            this.richText_videoInfo.Size = new System.Drawing.Size(287, 253);
            this.richText_videoInfo.TabIndex = 0;
            this.richText_videoInfo.Text = "Filename:\nSceneName:\nUploader:";
            // 
            // text_input_search
            // 
            this.text_input_search.Location = new System.Drawing.Point(0, 496);
            this.text_input_search.Name = "text_input_search";
            this.text_input_search.Size = new System.Drawing.Size(350, 20);
            this.text_input_search.TabIndex = 2;
            this.text_input_search.TextChanged += new System.EventHandler(this.text_input_search_TextChanged);
            // 
            // btn_fvi_copy_images
            // 
            this.btn_fvi_copy_images.Location = new System.Drawing.Point(398, 295);
            this.btn_fvi_copy_images.Name = "btn_fvi_copy_images";
            this.btn_fvi_copy_images.Size = new System.Drawing.Size(213, 27);
            this.btn_fvi_copy_images.TabIndex = 4;
            this.btn_fvi_copy_images.Text = "Copy Images (XMP) to WorkFolder";
            this.btn_fvi_copy_images.UseVisualStyleBackColor = true;
            this.btn_fvi_copy_images.Click += new System.EventHandler(this.btn_fvi_copy_images_Click);
            // 
            // btn_fvi_copy_video
            // 
            this.btn_fvi_copy_video.Location = new System.Drawing.Point(398, 328);
            this.btn_fvi_copy_video.Name = "btn_fvi_copy_video";
            this.btn_fvi_copy_video.Size = new System.Drawing.Size(164, 27);
            this.btn_fvi_copy_video.TabIndex = 5;
            this.btn_fvi_copy_video.TabStop = false;
            this.btn_fvi_copy_video.Text = "Copy Video to WorkFolder";
            this.btn_fvi_copy_video.UseVisualStyleBackColor = true;
            this.btn_fvi_copy_video.Click += new System.EventHandler(this.btn_fvi_copy_video_Click);
            // 
            // btn_fvi_update_list
            // 
            this.btn_fvi_update_list.Location = new System.Drawing.Point(354, 495);
            this.btn_fvi_update_list.Name = "btn_fvi_update_list";
            this.btn_fvi_update_list.Size = new System.Drawing.Size(75, 23);
            this.btn_fvi_update_list.TabIndex = 6;
            this.btn_fvi_update_list.Text = "button1";
            this.btn_fvi_update_list.UseVisualStyleBackColor = true;
            this.btn_fvi_update_list.Click += new System.EventHandler(this.btn_fvi_update_list_Click);
            // 
            // label_process_info
            // 
            this.label_process_info.AutoSize = true;
            this.label_process_info.Location = new System.Drawing.Point(398, 451);
            this.label_process_info.Name = "label_process_info";
            this.label_process_info.Size = new System.Drawing.Size(85, 13);
            this.label_process_info.TabIndex = 7;
            this.label_process_info.Text = "aaaaaaaaaaaaa";
            // 
            // btn_fvi_import_ue4
            // 
            this.btn_fvi_import_ue4.Location = new System.Drawing.Point(401, 390);
            this.btn_fvi_import_ue4.Name = "btn_fvi_import_ue4";
            this.btn_fvi_import_ue4.Size = new System.Drawing.Size(210, 34);
            this.btn_fvi_import_ue4.TabIndex = 8;
            this.btn_fvi_import_ue4.Text = "Import to UE4";
            this.btn_fvi_import_ue4.UseVisualStyleBackColor = true;
            this.btn_fvi_import_ue4.Click += new System.EventHandler(this.btn_fvi_import_ue4_Click);
            // 
            // progress_fvi_action
            // 
            this.progress_fvi_action.Location = new System.Drawing.Point(481, 492);
            this.progress_fvi_action.Name = "progress_fvi_action";
            this.progress_fvi_action.Size = new System.Drawing.Size(437, 23);
            this.progress_fvi_action.TabIndex = 9;
            // 
            // btn_fvi_copy_mesh
            // 
            this.btn_fvi_copy_mesh.Location = new System.Drawing.Point(400, 360);
            this.btn_fvi_copy_mesh.Name = "btn_fvi_copy_mesh";
            this.btn_fvi_copy_mesh.Size = new System.Drawing.Size(211, 27);
            this.btn_fvi_copy_mesh.TabIndex = 10;
            this.btn_fvi_copy_mesh.TabStop = false;
            this.btn_fvi_copy_mesh.Text = "Copy Mesh&&Images(cr) to WorkFolder";
            this.btn_fvi_copy_mesh.UseVisualStyleBackColor = true;
            this.btn_fvi_copy_mesh.Click += new System.EventHandler(this.btn_fvi_copy_mesh_Click);
            // 
            // FormVideoLib
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(943, 521);
            this.Controls.Add(this.btn_fvi_copy_mesh);
            this.Controls.Add(this.progress_fvi_action);
            this.Controls.Add(this.btn_fvi_import_ue4);
            this.Controls.Add(this.label_process_info);
            this.Controls.Add(this.btn_fvi_update_list);
            this.Controls.Add(this.btn_fvi_copy_video);
            this.Controls.Add(this.btn_fvi_copy_images);
            this.Controls.Add(this.text_input_search);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.list_fvi_videolist);
            this.Name = "FormVideoLib";
            this.Text = "FormVideoLib";
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox list_fvi_videolist;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox text_input_search;
        private System.Windows.Forms.Button btn_fvi_copy_images;
        private System.Windows.Forms.Button btn_fvi_copy_video;
        private System.Windows.Forms.RichTextBox richText_videoInfo;
        private System.Windows.Forms.Button btn_fvi_update_list;
        private System.Windows.Forms.Label label_process_info;
        private System.Windows.Forms.Button btn_fvi_import_ue4;
        private System.Windows.Forms.ProgressBar progress_fvi_action;
        private System.Windows.Forms.Button btn_fvi_copy_mesh;
    }
}