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

using AlbanianXrm.BackgroundWorker;
using McTools.Xrm.Connection;
using Microsoft.Xrm.Sdk;
using ShkoOnline.DataverseExcelReporter.Tool.BusinessLogic;
using ShkoOnline.DataverseExcelReporter.Tool.DataModel;
using ShkoOnline.DataverseExcelReporter.Tool.Properties;
using System;
using System.Windows.Forms;
using XrmToolBox.Extensibility;
using XrmToolBox.Extensibility.Args;
using XrmToolBox.Extensibility.Interfaces;

namespace ShkoOnline.DataverseExcelReporter.Tool
{
    public partial class DataverseExcelReporterControl : PluginControlBase, IGitHubPlugin, IStatusBarMessenger
    {
        #region IGitHubPlugin Members
        public string UserName => "Shko-Online";

        public string RepositoryName => "DataverseExcelReporter";
        #endregion

        private readonly AlBackgroundWorkHandler backgroundWorkHandler = new AlBackgroundWorkHandler();
        private readonly ToolViewModel viewModel = new ToolViewModel();
        private readonly MetadataHandler metadataHandler;
        private readonly ExcelReportGenerator excelReportGenerator;
        private bool shouldShowSplash = true;

        public event EventHandler<StatusBarMessageEventArgs> SendMessageToStatusBar;

        public void UpdateStatusBarMessage(string message)
        {
            SendMessageToStatusBar?.Invoke(this, new StatusBarMessageEventArgs(message));
        }

        public DataverseExcelReporterControl()
        {
            InitializeComponent();
            metadataHandler = new MetadataHandler(this, viewModel);
            excelReportGenerator = new ExcelReportGenerator(this, viewModel);
            ComboBoxTable.FilterPredicate = (t, filter) => t.DisplayName.IndexOf(filter, StringComparison.OrdinalIgnoreCase) >= 0;
            ComboBoxView.FilterPredicate = (t, filter) => t.DisplayName.IndexOf(filter, StringComparison.OrdinalIgnoreCase) >= 0;
            ComboBoxDocument.FilterPredicate = (t, filter) => t.DisplayName.IndexOf(filter, StringComparison.OrdinalIgnoreCase) >= 0;
            viewModel.PropertyChanged += ViewModel_PropertyChanged;
        }

        private void ViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(viewModel.GenerateReport_Enabled):
                    ButtonGenerateReport.Enabled = viewModel.GenerateReport_Enabled;
                    break;
                case nameof(viewModel.PendingOperationCTS):
               
                    ButtonCancelReportGeneration.Enabled = viewModel.PendingOperationCTS != null;
                    ButtonCancelReportGeneration.Visible = viewModel.PendingOperationCTS != null;
                    break;
                case nameof(viewModel.Tables):
                    ComboBoxTable.BindingValues = viewModel.Tables;
                    ComboBoxTable.DataSource = viewModel.Tables;
                    ComboBoxTable.SelectedIndex = -1;
                    ComboBoxTable.Text = "";
                    break;
                case nameof(viewModel.SelectedTable):
                    ComboBoxDocument.Enabled = viewModel.SelectedTable != null && viewModel.AllowRequests;
                    ComboBoxView.Enabled = viewModel.SelectedTable != null && viewModel.AllowRequests;
                    break;
                case nameof(viewModel.FilteredDocumentTemplates):
                    ComboBoxDocument.BindingValues = viewModel.FilteredDocumentTemplates;
                    ComboBoxDocument.DataSource = viewModel.FilteredDocumentTemplates;
                    ComboBoxDocument.SelectedIndex = -1;
                    ComboBoxDocument.Text = "";
                    break;
                case nameof(viewModel.SelectedDocumentTemplate):

                    break;
                case nameof(viewModel.FilteredTableViews):
                    ComboBoxView.BindingValues = viewModel.FilteredTableViews;
                    ComboBoxView.DataSource = viewModel.FilteredTableViews;
                    ComboBoxView.SelectedIndex = -1;
                    ComboBoxView.Text = "";
                    break;
                case nameof(viewModel.SelectedTableView):

                    break;
                case nameof(viewModel.AllowRequests):
                    ComboBoxTable.Enabled = viewModel.AllowRequests;
                    ButtonRefreshMetadata.Enabled = viewModel.AllowRequests;
                    ComboBoxDocument.Enabled = viewModel.SelectedTable != null && viewModel.AllowRequests;
                    ComboBoxView.Enabled = viewModel.SelectedTable != null && viewModel.AllowRequests;
                    break;
            }
        }

        private void DataverseExcelReporterControl_Load(object sender, EventArgs e)
        {
            if (!shouldShowSplash)
            {
                return;
            }
            viewModel.ActiveConnection = ConnectionDetail != null;
            if (viewModel.ActiveConnection)
            {
                metadataHandler.LoadTables(Service);
            }
            var splash = new SplashScreen(viewModel);
            splash.Show(this);
            viewModel.SplashShowing = true;
            shouldShowSplash = false;
        }

        public override void UpdateConnection(IOrganizationService newService, ConnectionDetail detail, string actionName, object parameter)
        {
            base.UpdateConnection(newService, detail, actionName, parameter);
            viewModel.ActiveConnection = detail != null;
        }

        private void ComboBoxTable_SelectedValueChanged(object sender, EventArgs e)
        {
            viewModel.SelectedTable = ComboBoxTable.SelectedItem as DataverseTable;
        }

        private void ComboBoxDocument_SelectedValueChanged(object sender, EventArgs e)
        {
            viewModel.SelectedDocumentTemplate = ComboBoxDocument.SelectedItem as DocumentTemplate;
        }

        private void ComboBoxView_SelectedValueChanged(object sender, EventArgs e)
        {
            viewModel.SelectedTableView = ComboBoxView.SelectedItem as TableView;
        }

        private void LinkLabelContactUs_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            MailTo.PrepareSendEmail(Resources.EMAIL_SUBJECT, Resources.EMAIL_BODY);
        }

        private void ButtonRefreshMetadata_Click(object sender, EventArgs e)
        {
            if (viewModel.ActiveConnection)
            {
                metadataHandler.LoadTables(Service);
            }
        }

        private void ButtonGenerateReport_Click(object sender, EventArgs e)
        {
            excelReportGenerator.GenerateReport(Service);
        }

        private void ButtonCancelReportGeneration_Click(object sender, EventArgs e)
        {
            if (viewModel.PendingOperationCTS != null)
            {
                viewModel.PendingOperationCTS.Cancel();
                viewModel.PendingOperationCTS = null;
            }
        }

        private void DataverseExcelReporterControl_ConnectionUpdated(object sender, ConnectionUpdatedEventArgs e)
        {
            if(viewModel.PendingOperationCTS!= null)    
            {
                viewModel.PendingOperationCTS.Cancel();
                viewModel.PendingOperationCTS = null;
            }
        }

        private void DataverseExcelReporterControl_OnCloseTool(object sender, EventArgs e)
        {
            if (viewModel.PendingOperationCTS != null)
            {
                viewModel.PendingOperationCTS.Cancel();
                viewModel.PendingOperationCTS = null;
            }
        }

        public override void ClosingPlugin(PluginCloseInfo info)
        {
            base.ClosingPlugin(info);
            if (viewModel.PendingOperationCTS != null)
            {
                viewModel.PendingOperationCTS.Cancel();
                viewModel.PendingOperationCTS = null;
            }
        }
    }
}
