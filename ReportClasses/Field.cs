using System;

namespace ReportClasses
{
    public class Field // here i will keep my colums names, rows indexes and column index
    {
        public uint Row { get; private set; }
        public String Column { get; private set; }
        public String _Field { get; private set; }

        public Field(uint row, String column, String field)
        {
            Row = row;
            Column = column;
            _Field = field;
        }
    }
}
