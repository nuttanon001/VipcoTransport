
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Globalization;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System.Reflection;

namespace ReportClasses
{
    public class OnePage
    {
        public string TemplateFolder { get; set; }
        public string Directory => Path.GetTempPath();
        private Stream CreateFile(string templateName)
        {
            var fullpath = TemplateFolder + templateName + ".xlsx";

            if (!File.Exists(fullpath))
                throw new Exception(String.Format("File not found \"{0}\"!", fullpath));

            var filePath = Directory + templateName + "_" + Regex.Replace((DateTime.Now.ToString(CultureInfo.InvariantCulture)), @"[^a-z0-9]+", "") + ".xlsx";

            File.Copy(fullpath, filePath, true);
            Stream stream = File.Open(filePath, FileMode.Open);

            return stream;
        }
        public Stream Export<T>(T dataExcel, String templateName)
        {
            var stream = CreateFile(templateName);
            return OpenForRewriteFile(stream, dataExcel);
        }
        private Stream OpenForRewriteFile<T>(Stream stream, T dataExcel)
        {
            var footer = new List<Footer>();
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

                Type type = typeof(T);
                var properties = type.GetProperties();

                var worksheetPart = (WorksheetPart)document.WorkbookPart.GetPartById(sheet.Id.Value);
                var sheetData = worksheetPart.Worksheet.GetFirstChild<SheetData>();

                var rowsForRemove = new List<Row>();
                var fields = new List<Field>();
                foreach (var row in worksheetPart.Worksheet.GetFirstChild<SheetData>().Elements<Row>())
                {
                    var celsForRemove = new List<Cell>();
                    foreach (var cell in row.Descendants<Cell>())
                    {
                        // if cell is null continue to next cell
                        if (cell == null)
                            continue;
                        // get value from cell
                        var value = GetCellValue(cell, document.WorkbookPart);
                        if (value.IndexOf("DataField:", StringComparison.Ordinal) != -1)
                        {
                            string Parameter = value.Replace("DataField:", "");
                            var PropertyValue = properties.FirstOrDefault(x => x.Name == Parameter).GetValue(dataExcel);
                            if (PropertyValue != null)
                            {
                                //cell.CellValue = new CellValue(PropertyValue.ToString());
                                //cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);

                                cell.DataType = CellValues.InlineString;
                                cell.InlineString = new InlineString() { Text = new Text(PropertyValue.ToString()) };
                            }
                        }
                    }
                }
                document.Save();
            }

            return stream;
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
