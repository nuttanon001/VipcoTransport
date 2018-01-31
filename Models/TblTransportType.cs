using System;
using System.Collections.Generic;

namespace VipcoTransport.Models
{
    public partial class TblTransportType
    {
        public TblTransportType()
        {
            TblTransportData = new HashSet<TblTransportData>();
            TblTransportRequestion = new HashSet<TblTransportRequestion>();
        }

        public int TransportTypeId { get; set; }
        public string TransportTypeDesc { get; set; }
        public byte? Property { get; set; }
        public string Remark { get; set; }
        public string Creator { get; set; }
        public DateTime? CreateDate { get; set; }
        public string Modifyer { get; set; }
        public DateTime? ModifyDate { get; set; }

        public virtual ICollection<TblTransportData> TblTransportData { get; set; }
        public virtual ICollection<TblTransportRequestion> TblTransportRequestion { get; set; }
    }
}
