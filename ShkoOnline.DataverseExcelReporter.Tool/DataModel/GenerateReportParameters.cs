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

using Microsoft.Xrm.Sdk;
using System.Threading;

namespace ShkoOnline.DataverseExcelReporter.Tool.DataModel
{
    internal class GenerateReportParameters
    {
        public IOrganizationService Service { get; set; }
        
        public DocumentTemplate DocumentTemplate { get; set; }

        public TableView TableView { get; set; }

        public int PageSize { get; set; }

        public CancellationTokenSource CTS { get; set; }
    }
}
