using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VipcoTransport.ViewModels
{
    public class DataViewModel<T>
    {
        public List<T> Data { get; private set; }
        public long TotalRecordCount { get; private set; }

        public DataViewModel(IEnumerable<T> items, long totalRecordCount)
        {
            Data = new List<T>(items);
            TotalRecordCount = totalRecordCount;
        }
    }
}
