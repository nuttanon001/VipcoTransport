using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VipcoTransport.ViewModels
{
    public class ScheduleViewModel<T>
    {
        public List<T> Data { get; private set; }
        public List<string> Columns { get; private set; }

        public ScheduleViewModel(IEnumerable<T> items, IEnumerable<string> columns)
        {
            this.Data = new List<T>(items);
            this.Columns = new List<string>(columns);
        }
    }
}
