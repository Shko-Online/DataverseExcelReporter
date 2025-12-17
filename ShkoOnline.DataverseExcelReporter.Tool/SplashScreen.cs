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
using ShkoOnline.DataverseExcelReporter.Tool.BusinessLogic;
using ShkoOnline.DataverseExcelReporter.Tool.Properties;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ShkoOnline.DataverseExcelReporter.Tool
{
    public partial class SplashScreen : Form
    {
        System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SplashScreen));
        private readonly ToolViewModel viewModel = new ToolViewModel();
        private readonly AlBackgroundWorkHandler backgroundWorkHandler = new AlBackgroundWorkHandler();
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

        public SplashScreen()
        {
            InitializeComponent();
            ButtonStartUsingTool.Text = string.Format(this.resources.GetString("ButtonStartUsingTool.Text"), 10);
            this.LabelVersion.Text = $"v {typeof(SplashScreen).Assembly.GetName().Version}";
            backgroundWorkHandler.EnqueueBackgroundWork(
                AlBackgroundWorkerFactory.NewAsyncWorker<bool, int>(CountToTen, SplashProgress, SplashShownFor10Seconds)
            );
        }

        private async Task<bool> CountToTen(Reporter<int> reporter)
        {
            for (int i = 1; i <= 10; i++)
            {
                await Task.Delay(1000);
                if (cancellationTokenSource.IsCancellationRequested) return true;
                reporter.ReportProgress(i, 10 - i);
            }

            return false;
        }

        private void SplashProgress(int progress, int message)
        {
            ButtonStartUsingTool.Text = string.Format(this.resources.GetString("ButtonStartUsingTool.Text"),message);
            if (progress == 3)
            {
                ButtonStartUsingTool.Enabled = true;
            }
        }

        private void SplashShownFor10Seconds(bool cancelled, Exception exception)
        {
            if (cancelled) return;
            Close();
            viewModel.SplashShowing = false;
        }

        internal SplashScreen(ToolViewModel viewModel) : this()
        {
            this.viewModel = viewModel;
        }

        private void ButtonStartUsingTool_Click(object sender, EventArgs e)
        {
            cancellationTokenSource.Cancel();
            Close();
            viewModel.SplashShowing = false;
        }

        private void LinkLabelContactUs_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            MailTo.PrepareSendEmail(Resources.EMAIL_SUBJECT, Resources.EMAIL_BODY);
        }
    }
}
