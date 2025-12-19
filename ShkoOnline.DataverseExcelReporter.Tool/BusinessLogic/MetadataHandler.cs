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
using AlbanianXrm.XrmToolBox.Shared.Extensions;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Query;
using ShkoOnline.DataverseExcelReporter.Tool.DataModel;
using ShkoOnline.DataverseExcelReporter.Tool.Properties;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ShkoOnline.DataverseExcelReporter.Tool.BusinessLogic
{
    internal class MetadataHandler
    {
        private readonly DataverseExcelReporterControl dataverseExcelReporterControl;
        private readonly ToolViewModel viewModel;
        private readonly AlBackgroundWorkHandler backgroundWorkHandler = new AlBackgroundWorkHandler();

        public MetadataHandler(DataverseExcelReporterControl dataverseExcelReporterControl, ToolViewModel viewModel)
        {
            this.dataverseExcelReporterControl = dataverseExcelReporterControl;
            this.viewModel = viewModel;
        }

        internal void LoadTables(IOrganizationService service)
        {
            backgroundWorkHandler.EnqueueBackgroundWork(
                   AlBackgroundWorkerFactory.NewWorker(LoadTablesWorker, service, TablesLoaded)
                                            .WithViewModel(viewModel)
                                            .WithMessage(dataverseExcelReporterControl, Resources.Loading_Document_Templates)
                   );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>This method is executed on a background thread</remarks>
        /// <param name="service"></param>
        /// <returns></returns>
        private Tuple<List<DataverseTable>, List<DocumentTemplate>, List<TableView>> LoadTablesWorker(IOrganizationService service)
        {
            var tables = new List<DataverseTable>();
            var metadata = new Dictionary<string, EntityMetadata>();
            var metadataByCode = new Dictionary<int, EntityMetadata>();

            var results = GetDocumentTemplates(tables, isPersonal: false, service)
                  .Concat(GetDocumentTemplates(tables, isPersonal: true, service))
                  .OrderBy(x => x.DisplayName)
                  .ToList();

            if (tables.Count == 0)
            {
                throw new InvalidOperationException(Resources.NO_TABLES);
            }

            GetTableMetadata(tables, metadata, metadataByCode, service);

            var tableViews = GetTableViews(tables, metadata, isPersonal: false, service)
                     .Concat(GetTableViews(tables, metadata, isPersonal: true, service))
                     .OrderBy(x => x.DisplayName)
                     .ToList();

            tables = tables.OrderBy(x => x.LogicalName).ToList();

            return new Tuple<List<DataverseTable>, List<DocumentTemplate>, List<TableView>>(tables, results, tableViews);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>This method is executed on the main thread</remarks>
        /// <param name="service"></param>
        /// <param name="results"></param>
        /// <param name="exception"></param>
        private void TablesLoaded(IOrganizationService service, Tuple<List<DataverseTable>, List<DocumentTemplate>, List<TableView>> results, Exception exception)
        {
            if (exception != null)
            {
                dataverseExcelReporterControl.ShowErrorDialog(exception, "Error loading tables");
                return;
            }
            viewModel.SelectedTableView = null;
            viewModel.TableViews = results.Item3;
            viewModel.SelectedDocumentTemplate = null;
            viewModel.DocumentTemplates = results.Item2;
            viewModel.SelectedTable = null;
            viewModel.Tables = results.Item1;
        }

        private static IEnumerable<DocumentTemplate> GetDocumentTemplates(List<DataverseTable> tables, bool isPersonal, IOrganizationService service)
        {
            var query = new QueryExpression(isPersonal ? "personaldocumenttemplate" : "documenttemplate")
            {
                ColumnSet = new ColumnSet(
                    "associatedentitytypecode",
                    "clientdata",
                    isPersonal ? "personaldocumenttemplateid" : "documenttemplateid",
                    "languagecode",
                    "name",
                    "description",
                    "status")
            };
            query.Criteria.AddCondition("documenttype", ConditionOperator.Equal, 1);
            return service.RetrieveMultiple(query).Entities
                          .Select(x => new DocumentTemplate()
                          {
                              Table = DataverseTable.GetOrCreate(
                                        tables,
                                        logicalName: x.GetAttributeValue<string>("associatedentitytypecode"),
                                        name: x.FormattedValues["associatedentitytypecode"]),
                              IsPersonal = isPersonal,
                              TemplateId = x.GetAttributeValue<Guid>(isPersonal ? "personaldocumenttemplateid" : "documenttemplateid"),
                              ClientData = x.GetAttributeValue<string>("clientdata"),
                              LanguageCode = x.GetAttributeValue<int>("languagecode"),
                              Name = x.GetAttributeValue<string>("name"),
                              Description = x.GetAttributeValue<string>("description"),
                              Status = x.FormattedValues["status"]
                          });
        }

        private IEnumerable<TableView> GetTableViews(List<DataverseTable> tables, Dictionary<string, EntityMetadata> metadata, bool isPersonal, IOrganizationService service)
        {
            var query = new QueryExpression(isPersonal ? "userquery" : "savedquery")
            {
                ColumnSet = new ColumnSet(
                    "name",
                    "returnedtypecode",
                    "fetchxml",
                    isPersonal ? "userqueryid" : "savedqueryid"),
            };
            query.Criteria.AddCondition("returnedtypecode", ConditionOperator.In, metadata.Values.Select(x => x.ObjectTypeCode).Cast<object>().ToArray());
            return service.RetrieveMultiple(query).Entities
                                 .Select(x => new TableView()
                                 {
                                     Table = DataverseTable.GetOrCreate(
                                                tables,
                                                logicalName: x.GetAttributeValue<string>("returnedtypecode"),
                                                name: x.FormattedValues["returnedtypecode"]),
                                     Name = x.GetAttributeValue<string>("name"),
                                     IsPersonal = isPersonal,
                                     ViewId = x.GetAttributeValue<Guid>(isPersonal ? "userqueryid" : "savedqueryid"),
                                     FetchXml = x.GetAttributeValue<string>("fetchxml"),
                                 }).ToList();
        }

        private void GetTableMetadata(List<DataverseTable> tables, Dictionary<string, EntityMetadata> metadata, Dictionary<int, EntityMetadata> metadataByCode, IOrganizationService service)
        {
            foreach (var table in tables)
            {
                var retrieveMetadataRequest = new RetrieveEntityRequest()
                {
                    EntityFilters = EntityFilters.Entity | EntityFilters.Attributes | EntityFilters.Relationships,
                    LogicalName = table.LogicalName
                };
                var retrieveMetadataResponse = (RetrieveEntityResponse)service.Execute(retrieveMetadataRequest);
                metadata[table.LogicalName] = retrieveMetadataResponse.EntityMetadata;
                metadataByCode[retrieveMetadataResponse.EntityMetadata.ObjectTypeCode.Value] = retrieveMetadataResponse.EntityMetadata;
            }
        }
    }
}
