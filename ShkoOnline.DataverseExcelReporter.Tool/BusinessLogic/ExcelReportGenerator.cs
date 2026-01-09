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
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Query;
using Newtonsoft.Json.Linq;
using ShkoOnline.DataverseExcelReporter.Tool.DataModel;
using ShkoOnline.DataverseExcelReporter.Tool.Properties;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Windows.Forms;
using System.Xml.Linq;
using System.Xml.XPath;

namespace ShkoOnline.DataverseExcelReporter.Tool.BusinessLogic
{
    internal class ExcelReportGenerator
    {
        private readonly DataverseExcelReporterControl dataverseExcelReporterControl;
        private readonly ToolViewModel viewModel;
        private readonly AlBackgroundWorkHandler backgroundWorkHandler = new AlBackgroundWorkHandler();

        public ExcelReportGenerator(DataverseExcelReporterControl dataverseExcelReporterControl, ToolViewModel viewModel)
        {
            this.dataverseExcelReporterControl = dataverseExcelReporterControl;
            this.viewModel = viewModel;
        }

        public void GenerateReport(IOrganizationService service, int pageSize)
        {
            viewModel.PendingOperationCTS = new CancellationTokenSource();
            backgroundWorkHandler.EnqueueBackgroundWork(
                                    AlBackgroundWorkerFactory.NewWorker<GenerateReportParameters, FileInfo, int>(GenerateReportWorker, new GenerateReportParameters()
                                    {
                                        Service = service,
                                        DocumentTemplate = viewModel.SelectedDocumentTemplate,
                                        TableView = viewModel.SelectedTableView,
                                        PageSize = pageSize,
                                        CTS = viewModel.PendingOperationCTS
                                    }, ReportGenerating, ReportGenerated)
                                 .WithViewModel(viewModel)
                                 .WithMessage(dataverseExcelReporterControl, Resources.Generating_Report)
            );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// This is executed on the main thread
        /// </remarks>
        /// <param name="percentage"></param>
        /// <param name="progress"></param>
        private void ReportGenerating(int percentage, int progress)
        {
            this.dataverseExcelReporterControl.UpdateStatusBarMessage(string.Format(Resources.ExportedNRecords, progress));
        }

        private FileInfo GenerateReportWorker(GenerateReportParameters parameters, Reporter<int> reporter)
        {
            if (parameters.CTS.Token.IsCancellationRequested)
            {
                return null;
            }

            var result = GetTemplateAsTempFile(parameters);

            var viewColumns = JObject.Parse(HttpUtility.HtmlDecode(parameters.DocumentTemplate.ClientData))
                                     .ToObject<ClientData>();

            var query = XDocument.Parse(parameters.TableView.FetchXml);
            query.XPathSelectElements("/fetch/entity/attribute").Remove(); //remove existing attributes
            query.Root.SetAttributeValue("count", parameters.PageSize);
            var entityElement = query.XPathSelectElement("/fetch/entity");
            var table = entityElement.Attribute("name").Value;
            var relationshipMetadata = GetRelationshipMetadataForRelatedColumns(table, viewColumns, parameters.Service);
            var relationshipMapping = RewriteQueryColumnsAndGetRelationshipMapping(relationshipMetadata, viewColumns, entityElement);

            int page = 1;
            uint rowIndex = 2; // The first row is for headers  
            var retrieveResult = new EntityCollection();
            retrieveResult.MoreRecords = true;

            using (var spreadsheet = SpreadsheetDocument.Open(result.FullName, true, new OpenSettings() { AutoSave = false }))
            {
                var dateFormatIndex = AddStylesheetPart(spreadsheet.WorkbookPart);
                foreach (var pivotTableDefinition in spreadsheet.WorkbookPart.PivotTableCacheDefinitionParts)
                {
                    pivotTableDefinition.PivotCacheDefinition.RefreshOnLoad = true;
                }

                var columnOffset = GetColumnOffset(viewColumns.Range);
                var worksheets = spreadsheet.WorkbookPart.GetPartsOfType<WorksheetPart>();
                foreach (var worksheet in worksheets)
                {
                    var tableParts = worksheet.GetPartsOfType<TableDefinitionPart>();
                    foreach (var tablePart in tableParts)
                    {
                        if (tablePart.Table.Name != "Table1")
                        {
                            continue;
                        }

                        var sheetData = worksheet.Worksheet.GetFirstChild<SheetData>();

                        while (retrieveResult.MoreRecords)
                        {
                            if (!string.IsNullOrEmpty(retrieveResult.PagingCookie))
                            {
                                query.Root.SetAttributeValue("paging-cookie", retrieveResult.PagingCookie);
                            }
                            query.Root.SetAttributeValue("page", page);

                            retrieveResult = parameters.Service.RetrieveMultiple(new FetchExpression(query.ToString()));

                            if (parameters.CTS.Token.IsCancellationRequested)
                            {
                                return null;
                            }

                            // If the worksheet does not contain a row with the specified row index, insert one.
                            var row = sheetData.Elements<Row>().FirstOrDefault(r => r.RowIndex.HasValue && r.RowIndex == rowIndex);
                            if (row == null)
                            {
                                row = new Row() { RowIndex = rowIndex };
                                sheetData.Append(row);
                            }

                            foreach (var entity in retrieveResult.Entities)
                            {
                                foreach (var column in viewColumns.Columns)
                                {
                                    if (column.Value.LogicalName == null)
                                    {
                                        continue;
                                    }
                                    var cellReference = $"{GetColumnLetter((uint)column.Value.ColumnIndex + columnOffset)}{rowIndex}";
                                    var cell = row.Elements<Cell>().FirstOrDefault(c => c.CellReference == cellReference);
                                    if (cell == null && !row.Elements<Cell>().Any())
                                    {
                                        cell = new Cell() { CellReference = cellReference };
                                        row.Append(cell);

                                    }
                                    else if (cell == null)
                                    {
                                        cell = new Cell() { CellReference = cellReference };
                                        Cell lastReference = null;
                                        Cell insertBeforeReference = null;
                                        foreach (Cell currentCell in row.Elements<Cell>())
                                        {
                                            lastReference = currentCell;
                                            if (GetColumnOffset(currentCell.CellReference) > column.Value.ColumnIndex)
                                            {
                                                insertBeforeReference = currentCell;
                                                break;
                                            }
                                        }

                                        if (insertBeforeReference != null)
                                        {
                                            row.InsertBefore(cell, insertBeforeReference);
                                        }
                                        else
                                        {
                                            row.InsertAfter(cell, lastReference);
                                        }
                                    }
                                    var columnLogicalName = column.Value.LogicalName;
                                    if (columnLogicalName.Contains('.'))
                                    {
                                        var splitted = columnLogicalName.Split('.');
                                        columnLogicalName = relationshipMapping[splitted[0]] + "." + splitted[1];
                                    }
                                    SetCellValueFromAttribute(entity, columnLogicalName, dateFormatIndex, ref cell);
                                }
                                rowIndex++;
                                // If the worksheet does not contain a row with the specified row index, insert one.
                                row = sheetData.Elements<Row>().FirstOrDefault(r => r.RowIndex.HasValue && r.RowIndex == rowIndex);
                                if (row == null)
                                {
                                    row = new Row() { RowIndex = rowIndex };
                                    sheetData.Append(row);
                                }
                            }
                            reporter.ReportProgress(0, (int)rowIndex - 2);
                            page++;
                        }
                        if (rowIndex == 2)
                        {
                            rowIndex++; // No data rows were added, keep at least one empty row
                        }
                        tablePart.Table.Reference.Value = new Regex("[A-Za-z]+[0-9]+\\:[A-Za-z]+").Match(tablePart.Table.Reference.Value).Value + (rowIndex - 1);
                    }
                }
                spreadsheet.Save();
            }

            return result;
        }

        private FileInfo GetTemplateAsTempFile(GenerateReportParameters parameters)
        {
            var documentContent = parameters.Service.Retrieve(
                                    parameters.DocumentTemplate.IsPersonal ?
                                        "personaldocumenttemplate" :
                                        "documenttemplate",
                                    parameters.DocumentTemplate.TemplateId,
                                    new ColumnSet("content")).GetAttributeValue<string>("content");
            var result = new FileInfo(Path.GetTempFileName());
            var base64Transform = new FromBase64Transform();
            using (var writer = new StreamWriter(new CryptoStream(result.OpenWrite(), base64Transform, CryptoStreamMode.Write)))
            {
                writer.Write(documentContent);
            }
            return result;
        }

        private Dictionary<string, OneToManyRelationshipMetadata> GetRelationshipMetadataForRelatedColumns(string table, ClientData viewColumns, IOrganizationService service)
        {
            var result = new Dictionary<string, OneToManyRelationshipMetadata>();
            var retrieveMetadataResponse = (RetrieveEntityResponse)service.Execute(new RetrieveEntityRequest()
            {
                EntityFilters = EntityFilters.Entity | EntityFilters.Attributes | EntityFilters.Relationships,
                LogicalName = table
            });
            var metadata = retrieveMetadataResponse.EntityMetadata;
            foreach (var relatedEntity in viewColumns.Columns.Values.Where(x => x.LogicalName?.Contains(".") == true).Select(x => x.LogicalName.Split('.')[0]).Distinct())
            {
                result.Add(relatedEntity, null);
            }
            if (!result.Any())
            {
                return result;
            }
            var relationshipQuery = new QueryExpression("relationship")
            {
                ColumnSet = new ColumnSet("relationshipid", "name")
            };
            relationshipQuery.Criteria.AddCondition("relationshipid", ConditionOperator.In, result.Keys.Cast<object>().ToArray());
            var relationshipResults = service.RetrieveMultiple(relationshipQuery);
            foreach (var relationshipResult in relationshipResults.Entities)
            {
                result[relationshipResult.Id.ToString()] = metadata.ManyToOneRelationships
                                                                   .FirstOrDefault(x =>
                                                                      x.SchemaName == relationshipResult.GetAttributeValue<string>("name"));
            }

            return result;
        }

        private Dictionary<string, string> RewriteQueryColumnsAndGetRelationshipMapping(Dictionary<string, OneToManyRelationshipMetadata> relationshipMetadata, ClientData viewColumns, XElement entityElement)
        {
            var result = new Dictionary<string, string>();
            foreach (var column in viewColumns.Columns)
            {
                if (column.Value.LogicalName == "checksumLogicalName" || column.Value.LogicalName == null)
                {                     // Skip related columns for now
                    continue;
                }

                if (column.Value.LogicalName.Contains("."))
                {
                    var parts = column.Value.LogicalName.Split('.');
                    var relationshipId = parts[0];
                    var attributeName = parts[1];
                    var relationship = relationshipMetadata[relationshipId];
                    if (relationship == null)
                    {
                        continue;
                    }

                    var linkEntityElement = entityElement.XPathSelectElement($"link-entity[@name='{relationship.ReferencedEntity}' and @from='{relationship.ReferencedAttribute}' and @to='{relationship.ReferencingAttribute}']");

                    if (linkEntityElement == null)
                    {
                        linkEntityElement = new XElement("link-entity",
                                                new XAttribute("name", relationship.ReferencedEntity),
                                                new XAttribute("from", relationship.ReferencedAttribute),
                                                new XAttribute("to", relationship.ReferencingAttribute),
                                                new XAttribute("link-type", "outer"),
                                                new XAttribute("alias", relationship.ReferencedEntity + relationshipId.Replace("-", "_"))
                       );
                        entityElement.Add(linkEntityElement);
                    }
                    if (linkEntityElement.Attribute("alias") == null)
                    {
                        linkEntityElement.SetAttributeValue("alias", relationship.ReferencedEntity + relationshipId.Replace("-", "_"));
                    }
                    if (!result.ContainsKey(relationshipId))
                    {
                        result.Add(relationshipId, linkEntityElement.Attribute("alias").Value);
                    }
                    linkEntityElement.Add(new XElement("attribute", new XAttribute("name", attributeName)));
                    continue;
                }
                entityElement.Add(new XElement("attribute", new XAttribute("name", column.Value.LogicalName)));
            }

            return result;
        }

        private void SetCellValueFromAttribute(Entity entity, string attributeName, uint dateFormatIndex, ref Cell cell)
        {
            if (attributeName == "checksumLogicalName")
            {
                cell.DataType = new EnumValue<CellValues>(CellValues.String);
                cell.CellValue = new CellValue(entity.RowVersion);
                return;
            }
            if (!entity.Contains(attributeName))
            {
                cell.DataType = new EnumValue<CellValues>(CellValues.String);
                cell.CellValue = new CellValue(string.Empty);
                return;
            }
            var value = attributeName.Contains('.') ? entity.GetAttributeValue<AliasedValue>(attributeName)?.Value : entity[attributeName];
            var typeOfValue = value.GetType();

            if (typeof(decimal) == value.GetType() || typeof(double) == value.GetType() || typeof(float) == value.GetType())
            {
                cell.DataType = new EnumValue<CellValues>(CellValues.Number);
                cell.CellValue = new CellValue(decimal.Parse(value.ToString()));
                return;
            }
            if (typeof(Money) == value.GetType())
            {
                cell.DataType = new EnumValue<CellValues>(CellValues.Number);
                cell.CellValue = new CellValue(((Money)value).Value);
                return;
            }
            if (typeof(int) == value.GetType() || typeof(long) == value.GetType() || typeof(short) == value.GetType())
            {
                cell.DataType = new EnumValue<CellValues>(CellValues.Number);
                cell.CellValue = new CellValue(Convert.ToInt32(value));
                return;
            }
            if (typeof(DateTime) == value.GetType())
            {
                cell.DataType = new EnumValue<CellValues>(CellValues.Date);
                cell.StyleIndex = dateFormatIndex; // DateTime format
                cell.CellValue = new CellValue(((DateTime)value));
                return;
            }
            if (typeof(EntityCollection) == value.GetType()) // Activity Parties
            {
                cell.DataType = new EnumValue<CellValues>(CellValues.String);
                var activityParties = value as EntityCollection;
                cell.CellValue = new CellValue(
                    string.Join(
                        ";",
                        activityParties.Entities.Select(e =>
                            string.IsNullOrEmpty(e.GetAttributeValue<string>("addressused")) ?
                                e.GetAttributeValue<EntityReference>("partyid").Name :
                                e.GetAttributeValue<string>("addressused"))));
                return;
            }
            if (entity.FormattedValues.Contains(attributeName))
            {
                cell.DataType = new EnumValue<CellValues>(CellValues.String);
                cell.CellValue = new CellValue(entity.FormattedValues[attributeName]);
                return;
            }
            cell.DataType = new EnumValue<CellValues>(CellValues.String);
            cell.CellValue = new CellValue(value.ToString());
        }

        private static uint AddStylesheetPart(WorkbookPart workbookPart)
        {
            WorkbookStylesPart stylesPart = workbookPart.GetPartsOfType<WorkbookStylesPart>().FirstOrDefault() ??
                workbookPart.AddNewPart<WorkbookStylesPart>();
            Stylesheet stylesheet = stylesPart.Stylesheet ?? new Stylesheet();
            if (stylesheet.NumberingFormats == null)
            {
                stylesheet.NumberingFormats = new NumberingFormats() { Count = 0U };
            }
            var numberingFormats = stylesheet.NumberingFormats;
            var dateFormat = new NumberingFormat()
            {
                NumberFormatId = ((uint)Math.Max((numberingFormats.ChildElements.OfType<NumberingFormat>().Select(x => x.NumberFormatId).Max() ?? 0) + 1, 164)),
                FormatCode = "yyyy/mm/dd hh:mm:ss"
            };
            numberingFormats.Append(dateFormat);
            numberingFormats.Count = (uint)numberingFormats.Count();

            if (stylesheet.CellFormats == null)
            {
                stylesheet.CellFormats = new CellFormats() { Count = 0U };
            }
            var cellFormats = stylesheet.CellFormats;
            var dateCellFormat = new CellFormat()
            {
                NumberFormatId = dateFormat.NumberFormatId,
                ApplyNumberFormat = true
            };
            cellFormats.Append(dateCellFormat);


            stylesPart.Stylesheet.Save();
            return (uint)(cellFormats.ToList().IndexOf(dateCellFormat));
        }

        private uint GetColumnOffset(string columnLetter)
        {
            columnLetter = new Regex("[A-Za-z]+").Match(columnLetter).Value; // Remove any row numbers)
            uint columnIndex = 0;
            for (int i = 0; i < columnLetter.Length; i++)
            {
                columnIndex *= 26;
                columnIndex += (uint)(columnLetter[i] - 'A' + 1);
            }
            return columnIndex;
        }

        private string GetColumnLetter(uint columnIndex)
        {
            string columnLetter = string.Empty;
            while (columnIndex > 0)
            {
                uint modulo = (columnIndex - 1) % 26;
                columnLetter = Convert.ToChar(65 + modulo) + columnLetter;
                columnIndex = (columnIndex - modulo) / 26;
            }
            return columnLetter;
        }


        private void ReportGenerated(GenerateReportParameters parameters, FileInfo result, Exception exception)
        {
            if (exception != null)
            {
                dataverseExcelReporterControl.ShowErrorDialog(exception, Resources.GenerateError);
                viewModel.PendingOperationCTS = null;
                return;
            }
            if (result != null)
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog
                {
                    Title = Resources.SaveGeneratedReport,
                    Filter = Resources.SaveFilter,
                    FileName = $"Report_{parameters.DocumentTemplate.Table.Name}_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx",
                    OverwritePrompt = true,
                    CheckPathExists = true,
                    InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
                };

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        File.Copy(result.FullName, saveFileDialog.FileName, true);
                        dataverseExcelReporterControl.UpdateStatusBarMessage(Resources.ReportSaved);
                    }
                    catch (Exception ex)
                    {
                        dataverseExcelReporterControl.ShowErrorDialog(ex, "Error saving the report file.");
                    }
                }
            }
            viewModel.PendingOperationCTS = null;
        }
    }
}
