using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VipcoTransport.ViewModels
{
    public class TransportLazyViewModel
    {
        public int RowNumber { get; set; }
        public int TransportId { get; set; }
        public string TransportNo { get; set; }
        public string DestinationString { get; set; }
        public string TransportDateTime { get; set; }
        public string CarNo { get; set; }
    }
}
