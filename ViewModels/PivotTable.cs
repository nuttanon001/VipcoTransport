using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace VipcoTransport.ViewModels
{
    public class PivotTable
    {
        public MyTable CreateMyDataTable<T>(IEnumerable<T> list)
        {
            Type type = typeof(T);
            var properties = type.GetProperties();
            MyTable dataTable = new MyTable();

            foreach (T entity in list)
            {
                MyRow dataRow = new MyRow();
                object[] values = new object[properties.Length];
                for (int i = 0; i < properties.Length; i++)
                {
                    string name = properties[i].Name;
                    Type typeCol = Nullable.GetUnderlyingType(properties[i].PropertyType) ?? properties[i].PropertyType;
                    object valueCol = properties[i].GetValue(entity);

                    MyColumn dataColumn = new MyColumn(name, typeCol, valueCol);
                    dataRow.AddMyColumn(dataColumn);
                }
                dataTable.AddMyRow(dataRow);
            }
            return dataTable;
        }
    }

    public class MyColumn
    {
        public string name { get; private set; }
        public object value { get; private set; }
        public Type type { get; private set; }

        public MyColumn(string name, Type type, object value)
        {
            this.name = name;
            this.type = type;
            this.value = value;
        }
    }
    public class MyRow
    {
        public List<MyColumn> Columns { get; private set; }
        public MyRow()
        {
            this.Columns = new List<MyColumn>();
        }

        public void AddMyColumn(MyColumn column)
        {
            this.Columns.Add(column);
        }
    }

    public class MyTable
    {
        public List<MyRow> Rows { get; private set; }

        public MyTable()
        {
            this.Rows = new List<MyRow>();
        }

        public void AddMyRow(MyRow row)
        {
            this.Rows.Add(row);
        }
    }
}
