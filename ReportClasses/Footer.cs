﻿using System;
using System.Collections.Generic;
using DocumentFormat.OpenXml.Spreadsheet;

namespace ReportClasses
{
    public class Footer //it's rows with cells for labels
    {
        public Row _Row { get; private set; }
        public List<CellForFooter> Cells { get; private set; }

        public Footer(Row row, Cell cell, String cellValue)
        {
            _Row = new Row((Row)row.Clone()) { RowIndex = row.RowIndex };
            var _Cell = (Cell)cell.Clone();
            _Cell.CellReference = cell.CellReference;
            Cells = new List<CellForFooter> { new CellForFooter(_Cell, cellValue) };
        }

        public void AddMoreCell(Cell cell, String cellValue)
        {
            var _Cell = (Cell)cell.Clone();
            _Cell.CellReference = cell.CellReference;
            Cells.Add(new CellForFooter(_Cell, cellValue));
        }
    }
}
