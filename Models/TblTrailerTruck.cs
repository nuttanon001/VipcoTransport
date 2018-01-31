using System;
using System.Collections.Generic;

namespace VipcoTransport.Models
{
    public partial class TblTrailerTruck
    {
        public TblTrailerTruck()
        {
            TblDriver = new HashSet<TblDriver>();
            TblTransportData = new HashSet<TblTransportData>();
            TblTruckHasCompany = new HashSet<TblTruckHasCompany>();
        }

        public int TrailerId { get; set; }
        public string TrailerNo { get; set; }
        public int? CarTypeId { get; set; }
        public string Insurance { get; set; }
        public DateTime? InsurDate { get; set; }
        public int? TrailerBrand { get; set; }
        public string RegisterNo { get; set; }
        public byte[] Images { get; set; }
        public string TrailerDesc { get; set; }
        public float? TrailerWeight { get; set; }
        public float? TrailerLoad { get; set; }
        public string Remark { get; set; }
        public string Creator { get; set; }
        public DateTime? CreateDate { get; set; }
        public string Modifyer { get; set; }
        public DateTime? ModifyDate { get; set; }

        public virtual ICollection<TblDriver> TblDriver { get; set; }
        public virtual ICollection<TblTransportData> TblTransportData { get; set; }
        public virtual ICollection<TblTruckHasCompany> TblTruckHasCompany { get; set; }
        public virtual TblCarType CarType { get; set; }
        public virtual TblCarBrand TrailerBrandNavigation { get; set; }
    }
}
