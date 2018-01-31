using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VipcoTransport.ViewModels
{
    public class LazyViewModel
    {
        public int? first { get; set; }
        public int? rows { get; set; }
        public string sortField { get; set; }
        public int? sortOrder { get; set; }
        public string filter { get; set; }
        public int? option { get; set; }
    }
}
