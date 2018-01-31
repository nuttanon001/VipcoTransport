using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace ReportClasses
{
    public class Worker
    {
        public string TemplateFolder { get; set; }
        public static String Directory => Path.GetTempPath();
        public Stream Export(MyTable dataTable, Dictionary<string, string> dataLable, String templateName)
        {
            var stream = CreateFile(templateName);
            return OpenForRewriteFile(stream, dataTable, dataLable);
        }
        private Stream CreateFile(String templateName)
        {
            var fullpath = TemplateFolder + templateName + ".xlsx";

            if (!File.Exists(fullpath))
                throw new Exception(String.Format("File not found \"{0}\"!", fullpath));

            var filePath = Directory + templateName + "_" + Regex.Replace((DateTime.Now.ToString(CultureInfo.InvariantCulture)), @"[^a-z0-9]+", "") + ".xlsx";

            File.Copy(fullpath, filePath, true);
            Stream stream = File.Open(filePath, FileMode.Open);

            return stream;
        }
        private Stream OpenForRewriteFile(Stream stream, MyTable dataTable, Dictionary<string, string> dataLable)
        {
            Row rowTemplate = null;
            var footer = new List<Footer>();
            var firsIndexFlag = false;
            using (var document = SpreadsheetDocument.Open(stream, true))
            {
                Sheet sheet;
                try
                {
                    sheet = document.WorkbookPart.Workbook.GetFirstChild<Sheets>().Elements<Sheet>().SingleOrDefault(s => s.Name == "report1");// get my sheet
                }
                catch (Exception ex)
                {
                    throw new Exception("Error found in workbookPart.", ex);
                }

                if (sheet == null)
                {
                    throw new Exception("Sheet not found\n");
                }

                var worksheetPart = (WorksheetPart)document.WorkbookPart.GetPartById(sheet.Id.Value);
                var sheetData = worksheetPart.Worksheet.GetFirstChild<SheetData>();

                var rowsForRemove = new List<Row>();
                var fields = new List<Field>();
                foreach (var row in worksheetPart.Worksheet.GetFirstChild<SheetData>().Elements<Row>())
                {
                    var celsForRemove = new List<Cell>();
                    foreach (var cell in row.Descendants<Cell>())
                    {
                        if (cell == null)
                        {
                            continue;
                        }

                        var value = GetCellValue(cell, document.WorkbookPart);
                        if (value.IndexOf("DataField:", StringComparison.Ordinal) != -1)
                        {
                            if (!firsIndexFlag)
                            {
                                firsIndexFlag = true;
                                rowTemplate = row;
                            }
                            fields.Add(new Field(Convert.ToUInt32(Regex.Replace(cell.CellReference.Value, @"[^\d]+", ""))
                                , new string(cell.CellReference.Value.ToCharArray().Where(p => !char.IsDigit(p)).ToArray())
                                , value.Replace("DataField:", "")));

                        }

                        if (value.IndexOf("Label:", StringComparison.Ordinal) != -1 && rowTemplate == null)
                        {
                            if (!dataLable.ContainsKey(value.Replace("Label:", "").Trim()))
                            {
                                throw new Exception("Error not been found Label name");
                            }
                            cell.CellValue = new CellValue(dataLable[value.Replace("Label:", "").Trim()].ToString());
                            cell.DataType = new EnumValue<CellValues>(CellValues.String);

                        }

                        if (rowTemplate == null || row.RowIndex <= rowTemplate.RowIndex || String.IsNullOrWhiteSpace(value))
                        {
                            continue;
                        }
                        var item = footer.SingleOrDefault(p => p._Row.RowIndex == row.RowIndex);
                        if (item == null)
                        {
                            footer.Add(new Footer(row, cell, value.IndexOf("Label:", StringComparison.Ordinal) != -1 ? dataLable[value.Replace("Label:", "").Trim()].ToString() : value));
                        }
                        else
                        {
                            item.AddMoreCell(cell, value.IndexOf("Label:", StringComparison.Ordinal) != -1 ? dataLable[value.Replace("Label:", "").Trim()].ToString() : value);
                        }
                        celsForRemove.Add(cell);
                    }

                    foreach (var cell in celsForRemove)
                    {
                        cell.Remove();
                    }

                    if (rowTemplate != null && row.RowIndex != rowTemplate.RowIndex)
                    {
                        rowsForRemove.Add(row);
                    }
                }

                if (rowTemplate == null || rowTemplate.RowIndex == null || rowTemplate.RowIndex < 0)
                {
                    throw new Exception("Error not found.");
                }

                foreach (var row in rowsForRemove)
                {
                    row.Remove();
                }

                var index = rowTemplate.RowIndex;

                foreach (var row in from item in dataTable.Rows select CreateRow(rowTemplate, index, item, fields))
                {
                    sheetData.InsertBefore(row, rowTemplate);
                    index++;
                }

                foreach (var newRow in footer.Select(item => CreateLabel(item, (UInt32)dataTable.Rows.Count)))
                {
                    sheetData.InsertBefore(newRow, rowTemplate);
                }

                rowTemplate.Remove();
                document.Save();
            }

            return stream;
        }
        private Row CreateLabel(Footer item, uint count)
        {
            var row = item._Row;
            row.RowIndex = new UInt32Value(item._Row.RowIndex + (count - 1));
            foreach (var cell in item.Cells)
            {
                cell._Cell.CellReference = new StringValue(cell._Cell.CellReference.Value.Replace(Regex.Replace(cell._Cell.CellReference.Value, @"[^\d]+", ""), row.RowIndex.ToString()));
                cell._Cell.CellValue = new CellValue(cell.Value);
                cell._Cell.DataType = new EnumValue<CellValues>(CellValues.String);
                row.Append(cell._Cell);
            }
            return row;
        }
        private Row CreateRow(Row rowTemplate, uint index, MyRow item, List<Field> fields)
        {
            try
            {
                var newRow = (Row)rowTemplate.Clone();
                newRow.RowIndex = new UInt32Value(index);

                foreach (var cell in newRow.Elements<Cell>())
                {
                    cell.CellReference = new StringValue(cell.CellReference.Value.Replace(Regex.Replace(cell.CellReference.Value, @"[^\d]+", ""), index.ToString(CultureInfo.InvariantCulture)));
                    foreach (var fil in fields.Where(fil => cell.CellReference == fil.Column + index))
                    {
                        var hasdata = item.Columns.Where(x => x.Name == fil._Field).FirstOrDefault();
                        if (hasdata != null)
                        {
                            cell.CellValue = new CellValue(hasdata.Value == null ? "-" : hasdata.Value.ToString());
                            cell.DataType = new EnumValue<CellValues>(CellValues.String);
                        }

                    }
                }
                return newRow;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Has error {ex.ToString()}");
            }
            return null;
        }
        private string GetCellValue(Cell cell, WorkbookPart wbPart)
        {
            var value = cell.InnerText;

            if (cell.DataType == null)
            {
                return value;
            }
            switch (cell.DataType.Value)
            {
                case CellValues.SharedString:

                    var stringTable = wbPart.GetPartsOfType<SharedStringTablePart>().FirstOrDefault();

                    if (stringTable != null)
                    {
                        value = stringTable.SharedStringTable.ElementAt(int.Parse(value)).InnerText;
                    }
                    break;
            }

            return value;
        }
    }
}
