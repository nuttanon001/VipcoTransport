using System;
using System.Collections.Generic;

namespace VipcoTransport.Models
{
    public partial class TblCarData
    {
        public TblCarData()
        {
            TblCarHasCompany = new HashSet<TblCarHasCompany>();
            TblDriver = new HashSet<TblDriver>();
            TblTransportData = new HashSet<TblTransportData>();
        }

        public int CarId { get; set; }
        public int CarTypeId { get; set; }
        public string CarNo { get; set; }
        public int? CarBrand { get; set; }
        public string RegisterNo { get; set; }
        public DateTime? CarDate { get; set; }
        public float? CarWeight { get; set; }
        public string Insurance { get; set; }
        public DateTime? InsurDate { get; set; }
        public string ActInsurance { get; set; }
        public DateTime? ActDate { get; set; }
        public string Remark { get; set; }
        public byte[] Images { get; set; }
        public bool? Status { get; set; }
        public string Creator { get; set; }
        public DateTime? CreateDate { get; set; }
        public string Modifyer { get; set; }
        public DateTime? ModifyDate { get; set; }

        public virtual ICollection<TblCarHasCompany> TblCarHasCompany { get; set; }
        public virtual ICollection<TblDriver> TblDriver { get; set; }
        public virtual ICollection<TblTransportData> TblTransportData { get; set; }
        public virtual TblCarBrand CarBrandNavigation { get; set; }
        public virtual TblCarType CarType { get; set; }
    }
}
