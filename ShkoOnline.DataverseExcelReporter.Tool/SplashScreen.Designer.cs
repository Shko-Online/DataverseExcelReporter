namespace ShkoOnline.DataverseExcelReporter.Tool
{
    partial class SplashScreen
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SplashScreen));
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.LabelVersion = new System.Windows.Forms.Label();
            this.LinkLabelContactUs = new System.Windows.Forms.LinkLabel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.LabelToolTitle = new System.Windows.Forms.Label();
            this.ButtonStartUsingTool = new System.Windows.Forms.Button();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.LabelVersion, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.LinkLabelContactUs, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.panel1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.LabelToolTitle, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.ButtonStartUsingTool, 1, 3);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // LabelVersion
            // 
            resources.ApplyResources(this.LabelVersion, "LabelVersion");
            this.LabelVersion.Name = "LabelVersion";
            // 
            // LinkLabelContactUs
            // 
            resources.ApplyResources(this.LinkLabelContactUs, "LinkLabelContactUs");
            this.LinkLabelContactUs.AutoEllipsis = true;
            this.tableLayoutPanel1.SetColumnSpan(this.LinkLabelContactUs, 2);
            this.LinkLabelContactUs.LinkColor = System.Drawing.Color.Black;
            this.LinkLabelContactUs.Name = "LinkLabelContactUs";
            this.LinkLabelContactUs.TabStop = true;
            this.LinkLabelContactUs.UseCompatibleTextRendering = true;
            this.LinkLabelContactUs.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.LinkLabelContactUs_LinkClicked);
            // 
            // panel1
            // 
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.BackgroundImage = global::ShkoOnline.DataverseExcelReporter.Tool.Properties.Resources.ShkoOnline490x148;
            this.tableLayoutPanel1.SetColumnSpan(this.panel1, 2);
            this.panel1.Name = "panel1";
            // 
            // LabelToolTitle
            // 
            resources.ApplyResources(this.LabelToolTitle, "LabelToolTitle");
            this.tableLayoutPanel1.SetColumnSpan(this.LabelToolTitle, 2);
            this.LabelToolTitle.Name = "LabelToolTitle";
            // 
            // ButtonStartUsingTool
            // 
            resources.ApplyResources(this.ButtonStartUsingTool, "ButtonStartUsingTool");
            this.ButtonStartUsingTool.Name = "ButtonStartUsingTool";
            this.ButtonStartUsingTool.UseVisualStyleBackColor = true;
            this.ButtonStartUsingTool.Click += new System.EventHandler(this.ButtonStartUsingTool_Click);
            // 
            // SplashScreen
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ControlBox = false;
            this.Controls.Add(this.tableLayoutPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "SplashScreen";
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label LabelToolTitle;
        private System.Windows.Forms.Button ButtonStartUsingTool;
        private System.Windows.Forms.LinkLabel LinkLabelContactUs;
        private System.Windows.Forms.Label LabelVersion;
    }
}