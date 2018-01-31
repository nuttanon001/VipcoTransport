using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VipcoTransport.ViewModels
{
    public class TransportRequestionLazyViewModel
    {
        public int RowNumber { get; set; }
        public int TransportRequestId { get; set; }
        public string TransportReqNo { get; set; }
        public string DestinationString { get; set; }
        public string TransportDateTime { get; set; }
        public string RequestDateTime { get; set; }
        public string EmployeeRequest { get; set; }
        public string Color { get; set; }
        public string Creator { get; set; }
        public byte TransportStatus { get; set; }
    }
}
