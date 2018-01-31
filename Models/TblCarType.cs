using System;
using System.Collections.Generic;

namespace VipcoTransport.Models
{
    public partial class TblCarType
    {
        public TblCarType()
        {
            TblCarData = new HashSet<TblCarData>();
            TblTrailerTruck = new HashSet<TblTrailerTruck>();
            TblTransportData = new HashSet<TblTransportData>();
            TblTransportRequestion = new HashSet<TblTransportRequestion>();
        }

        public int CarTypeId { get; set; }
        public string CarTypeNo { get; set; }
        public string CarTypeDesc { get; set; }
        public byte? Type { get; set; }
        public string Remark { get; set; }
        public string Creator { get; set; }
        public DateTime? CreateDate { get; set; }
        public string Modifyer { get; set; }
        public DateTime? ModifyDate { get; set; }

        public virtual ICollection<TblCarData> TblCarData { get; set; }
        public virtual ICollection<TblTrailerTruck> TblTrailerTruck { get; set; }
        public virtual ICollection<TblTransportData> TblTransportData { get; set; }
        public virtual ICollection<TblTransportRequestion> TblTransportRequestion { get; set; }
    }
}
