using System;
using DocumentFormat.OpenXml.Spreadsheet;

namespace ReportClasses
{
    public class CellForFooter// here i will be keep cells for my labels. self cell and value
    {
        public Cell _Cell { get; private set; }
        public String Value { get; private set; }

        public CellForFooter(Cell cell, String value)
        {
            _Cell = cell;
            Value = value;
        }
    }
}
