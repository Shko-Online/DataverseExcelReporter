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

using AlbanianXrm.XrmToolBox.Shared;
using ShkoOnline.DataverseExcelReporter.Tool.DataModel;
using System.Collections.Generic;
using System.Threading;

namespace ShkoOnline.DataverseExcelReporter.Tool
{
    internal class ToolViewModel : ToolViewModelBase
    {
        private bool _ActiveConnection = false;
        public bool ActiveConnection
        {
            get { return _ActiveConnection; }
            set
            {
                if (_ActiveConnection == value) return;
                _ActiveConnection = value;

                NotifyPropertyChanged(nameof(ActiveConnection));
            }
        }

        private bool _SplashShowing = false;
        public bool SplashShowing
        {
            get { return _SplashShowing; }
            set
            {
                if (_SplashShowing == value) return;
                _SplashShowing = value;

                NotifyPropertyChanged(nameof(SplashShowing));
            }
        }

        private bool _ShareAnonymousTelemetry = true;
        public bool ShareAnonymousTelemetry
        {
            get { return _ShareAnonymousTelemetry; }
            set
            {
                if (_ShareAnonymousTelemetry == value) return;
                _ShareAnonymousTelemetry = value;

                NotifyPropertyChanged(nameof(ShareAnonymousTelemetry));
            }
        }

        private List<DataverseTable> _Tables = new List<DataverseTable>();
        public List<DataverseTable> Tables
        {
            get { return _Tables; }
            set
            {
                _Tables = value;
                SelectedTable = null;
                NotifyPropertyChanged(nameof(Tables));
            }
        }

        private DataverseTable _SelectedTable;
        public DataverseTable SelectedTable
        {
            get { return _SelectedTable; }
            set
            {
                if (_SelectedTable == value) return;
                _SelectedTable = value;
                SelectedDocumentTemplate = null;
                FilteredDocumentTemplates = _SelectedTable == null ?
                    new List<DocumentTemplate>() :
                    _DocumentTemplates.FindAll(dt => dt.Table.Equals(_SelectedTable));
                FilteredTableViews = _SelectedTable == null ?
                    new List<TableView>() :
                    _TableViews.FindAll(dt => dt.Table.Equals(_SelectedTable));
                NotifyPropertyChanged(nameof(SelectedTable));
            }
        }

        private List<DocumentTemplate> _DocumentTemplates = new List<DocumentTemplate>();
        public List<DocumentTemplate> DocumentTemplates
        {
            get { return _DocumentTemplates; }
            set
            {
                _DocumentTemplates = value;
                SelectedDocumentTemplate = null;
                NotifyPropertyChanged(nameof(DocumentTemplates));
            }
        }

        private List<DocumentTemplate> _FilteredDocumentTemplates = new List<DocumentTemplate>();
        public List<DocumentTemplate> FilteredDocumentTemplates
        {
            get { return _FilteredDocumentTemplates; }
            set
            {
                _FilteredDocumentTemplates = value;
                NotifyPropertyChanged(nameof(FilteredDocumentTemplates));
            }
        }

        private DocumentTemplate _SelectedDocumentTemplate;
        public DocumentTemplate SelectedDocumentTemplate
        {
            get { return _SelectedDocumentTemplate; }
            set
            {
                if (_SelectedDocumentTemplate == value) return;
                _SelectedDocumentTemplate = value;
                NotifyPropertyChanged(nameof(SelectedDocumentTemplate));
            }
        }

        private List<TableView> _TableViews = new List<TableView>();
        public List<TableView> TableViews
        {
            get { return _TableViews; }
            set
            {
                _TableViews = value;
                SelectedTableView = null;
                NotifyPropertyChanged(nameof(TableViews));
            }
        }

        private List<TableView> _FilteredTableViews = new List<TableView>();
        public List<TableView> FilteredTableViews
        {
            get { return _FilteredTableViews; }
            set
            {
                _FilteredTableViews = value;
                NotifyPropertyChanged(nameof(FilteredTableViews));
            }
        }

        private TableView _SelectedTableView;
        public TableView SelectedTableView
        {
            get { return _SelectedTableView; }
            set
            {
                if (_SelectedTableView == value) return;
                _SelectedTableView = value;
                NotifyPropertyChanged(nameof(SelectedTableView));
            }
        }

        private CancellationTokenSource _PendingOperationCTS = new CancellationTokenSource();
        public CancellationTokenSource PendingOperationCTS
        {
            get { return _PendingOperationCTS; }
            set
            {
                _PendingOperationCTS = value;
                NotifyPropertyChanged(nameof(PendingOperationCTS));
            }
        }
    }
}
