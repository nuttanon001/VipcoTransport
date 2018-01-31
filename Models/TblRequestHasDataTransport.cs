using System;
using System.Collections.Generic;

namespace VipcoTransport.Models
{
    public partial class TblRequestHasDataTransport
    {
        public int RequestHasDataTransportId { get; set; }
        public int TransportRequestId { get; set; }
        public int TransportId { get; set; }
        public string Creator { get; set; }
        public DateTime? CreateDate { get; set; }
        public string Modifyer { get; set; }
        public DateTime? ModifyDate { get; set; }

        public virtual TblTransportData Transport { get; set; }
        public virtual TblTransportRequestion TransportRequest { get; set; }
    }
}
