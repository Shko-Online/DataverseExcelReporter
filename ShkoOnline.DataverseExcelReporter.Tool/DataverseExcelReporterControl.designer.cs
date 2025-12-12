/*
   Copyright 2025 Shko Online LLC <sales@shko.online>

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
 */

using ShkoOnline.DataverseExcelReporter.Tool.CustomControls;
using XrmToolBox.Extensibility;

namespace ShkoOnline.DataverseExcelReporter.Tool
{
    public partial class DataverseExcelReporterControl
    {
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DataverseExcelReporterControl));
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.ComboBoxView = new ShkoOnline.DataverseExcelReporter.Tool.CustomControls.TableViewFilteringCombobox();
            this.LabelView = new System.Windows.Forms.Label();
            this.ComboBoxDocument = new ShkoOnline.DataverseExcelReporter.Tool.CustomControls.DocumentTemplateFilteringCombobox();
            this.LabelTemplate = new System.Windows.Forms.Label();
            this.ComboBoxTable = new ShkoOnline.DataverseExcelReporter.Tool.CustomControls.DataverseTableFilteringCombobox();
            this.LabelTable = new System.Windows.Forms.Label();
            this.panelLogo = new System.Windows.Forms.Panel();
            this.LinkLabelContactUs = new System.Windows.Forms.LinkLabel();
            this.ToolStripCommands = new System.Windows.Forms.ToolStrip();
            this.ButtonCancelReportGeneration = new System.Windows.Forms.ToolStripButton();
            this.ButtonGenerateReport = new System.Windows.Forms.ToolStripButton();
            this.ButtonRefreshMetadata = new System.Windows.Forms.ToolStripButton();
            this.StripSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.LabelPageSize = new System.Windows.Forms.ToolStripLabel();
            this.TextPageSize = new System.Windows.Forms.ToolStripTextBox();
            this.tableLayoutPanel1.SuspendLayout();
            this.ToolStripCommands.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.ComboBoxView, 1, 4);
            this.tableLayoutPanel1.Controls.Add(this.LabelView, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.ComboBoxDocument, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.LabelTemplate, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.ComboBoxTable, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.LabelTable, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.panelLogo, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.LinkLabelContactUs, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.ToolStripCommands, 0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // ComboBoxView
            // 
            this.ComboBoxView.DisplayMember = "DisplayName";
            resources.ApplyResources(this.ComboBoxView, "ComboBoxView");
            this.ComboBoxView.Name = "ComboBoxView";
            this.ComboBoxView.ValueMember = "ViewId";
            this.ComboBoxView.SelectedValueChanged += new System.EventHandler(this.ComboBoxView_SelectedValueChanged);
            // 
            // LabelView
            // 
            resources.ApplyResources(this.LabelView, "LabelView");
            this.LabelView.AutoEllipsis = true;
            this.LabelView.Name = "LabelView";
            // 
            // ComboBoxDocument
            // 
            this.ComboBoxDocument.DisplayMember = "DisplayName";
            resources.ApplyResources(this.ComboBoxDocument, "ComboBoxDocument");
            this.ComboBoxDocument.Name = "ComboBoxDocument";
            this.ComboBoxDocument.ValueMember = "TemplateId";
            this.ComboBoxDocument.SelectedValueChanged += new System.EventHandler(this.ComboBoxDocument_SelectedValueChanged);
            // 
            // LabelTemplate
            // 
            resources.ApplyResources(this.LabelTemplate, "LabelTemplate");
            this.LabelTemplate.AutoEllipsis = true;
            this.LabelTemplate.Name = "LabelTemplate";
            // 
            // ComboBoxTable
            // 
            this.ComboBoxTable.DisplayMember = "DisplayName";
            resources.ApplyResources(this.ComboBoxTable, "ComboBoxTable");
            this.ComboBoxTable.Name = "ComboBoxTable";
            this.ComboBoxTable.ValueMember = "LogicalName";
            this.ComboBoxTable.SelectedValueChanged += new System.EventHandler(this.ComboBoxTable_SelectedValueChanged);
            // 
            // LabelTable
            // 
            resources.ApplyResources(this.LabelTable, "LabelTable");
            this.LabelTable.AutoEllipsis = true;
            this.LabelTable.Name = "LabelTable";
            // 
            // panelLogo
            // 
            this.panelLogo.BackgroundImage = global::ShkoOnline.DataverseExcelReporter.Tool.Properties.Resources.ShkoOnline;
            resources.ApplyResources(this.panelLogo, "panelLogo");
            this.panelLogo.Name = "panelLogo";
            // 
            // LinkLabelContactUs
            // 
            resources.ApplyResources(this.LinkLabelContactUs, "LinkLabelContactUs");
            this.LinkLabelContactUs.AutoEllipsis = true;
            this.LinkLabelContactUs.LinkColor = System.Drawing.Color.Black;
            this.LinkLabelContactUs.Name = "LinkLabelContactUs";
            this.LinkLabelContactUs.TabStop = true;
            this.LinkLabelContactUs.UseCompatibleTextRendering = true;
            this.LinkLabelContactUs.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.LinkLabelContactUs_LinkClicked);
            // 
            // ToolStripCommands
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.ToolStripCommands, 2);
            resources.ApplyResources(this.ToolStripCommands, "ToolStripCommands");
            this.ToolStripCommands.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.ToolStripCommands.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.ToolStripCommands.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ButtonCancelReportGeneration,
            this.ButtonGenerateReport,
            this.ButtonRefreshMetadata,
            this.StripSeparator,
            this.LabelPageSize,
            this.TextPageSize});
            this.ToolStripCommands.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.HorizontalStackWithOverflow;
            this.ToolStripCommands.Name = "ToolStripCommands";
            this.ToolStripCommands.Stretch = true;
            // 
            // ButtonCancelReportGeneration
            // 
            this.ButtonCancelReportGeneration.Image = global::ShkoOnline.DataverseExcelReporter.Tool.Properties.Resources.ButtonCancel31x31;
            resources.ApplyResources(this.ButtonCancelReportGeneration, "ButtonCancelReportGeneration");
            this.ButtonCancelReportGeneration.Name = "ButtonCancelReportGeneration";
            this.ButtonCancelReportGeneration.Click += new System.EventHandler(this.ButtonCancelReportGeneration_Click);
            // 
            // ButtonGenerateReport
            // 
            this.ButtonGenerateReport.Image = global::ShkoOnline.DataverseExcelReporter.Tool.Properties.Resources.ButtonExportReport31x31;
            resources.ApplyResources(this.ButtonGenerateReport, "ButtonGenerateReport");
            this.ButtonGenerateReport.Name = "ButtonGenerateReport";
            this.ButtonGenerateReport.Click += new System.EventHandler(this.ButtonGenerateReport_Click);
            // 
            // ButtonRefreshMetadata
            // 
            this.ButtonRefreshMetadata.Image = global::ShkoOnline.DataverseExcelReporter.Tool.Properties.Resources.ButtonRefresh31x31;
            resources.ApplyResources(this.ButtonRefreshMetadata, "ButtonRefreshMetadata");
            this.ButtonRefreshMetadata.Name = "ButtonRefreshMetadata";
            this.ButtonRefreshMetadata.Click += new System.EventHandler(this.ButtonRefreshMetadata_Click);
            // 
            // StripSeparator
            // 
            this.StripSeparator.Name = "StripSeparator";
            resources.ApplyResources(this.StripSeparator, "StripSeparator");
            // 
            // LabelPageSize
            // 
            this.LabelPageSize.Name = "LabelPageSize";
            resources.ApplyResources(this.LabelPageSize, "LabelPageSize");
            // 
            // TextPageSize
            // 
            resources.ApplyResources(this.TextPageSize, "TextPageSize");
            this.TextPageSize.Name = "TextPageSize";
            this.TextPageSize.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TextPageSize_KeyPress);
            // 
            // DataverseExcelReporterControl
            // 
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "DataverseExcelReporterControl";
            this.PluginIcon = ((System.Drawing.Icon)(resources.GetObject("$this.PluginIcon")));
            this.TabIcon = global::ShkoOnline.DataverseExcelReporter.Tool.Properties.Resources.ShkoOnlineIcon;
            this.OnCloseTool += new System.EventHandler(this.DataverseExcelReporterControl_OnCloseTool);
            this.ConnectionUpdated += new XrmToolBox.Extensibility.PluginControlBase.ConnectionUpdatedHandler(this.DataverseExcelReporterControl_ConnectionUpdated);
            this.Load += new System.EventHandler(this.DataverseExcelReporterControl_Load);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ToolStripCommands.ResumeLayout(false);
            this.ToolStripCommands.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label LabelTable;
        private System.Windows.Forms.Label LabelTemplate;
        private DocumentTemplateFilteringCombobox ComboBoxDocument;
        private System.Windows.Forms.Panel panelLogo;
        private System.Windows.Forms.Label LabelView;
        private TableViewFilteringCombobox ComboBoxView;
        private System.Windows.Forms.LinkLabel LinkLabelContactUs;
        private System.Windows.Forms.ToolStrip ToolStripCommands;
        private System.Windows.Forms.ToolStripButton ButtonRefreshMetadata;
        private System.Windows.Forms.ToolStripButton ButtonGenerateReport;
        private System.Windows.Forms.ToolStripButton ButtonCancelReportGeneration;
        private DataverseTableFilteringCombobox ComboBoxTable;
        private System.Windows.Forms.ToolStripSeparator StripSeparator;
        private System.Windows.Forms.ToolStripLabel LabelPageSize;
        private System.Windows.Forms.ToolStripTextBox TextPageSize;
    }
}
