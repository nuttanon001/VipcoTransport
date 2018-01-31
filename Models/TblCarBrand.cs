using System;
using System.Collections.Generic;

namespace VipcoTransport.Models
{
    public partial class TblCarBrand
    {
        public TblCarBrand()
        {
            TblCarData = new HashSet<TblCarData>();
            TblTrailerTruck = new HashSet<TblTrailerTruck>();
        }

        public int BrandId { get; set; }
        public string BrandName { get; set; }
        public string BrandDesc { get; set; }
        public string Creator { get; set; }
        public DateTime? CreateDate { get; set; }
        public string Modifyer { get; set; }
        public DateTime? ModifyDate { get; set; }

        public virtual ICollection<TblCarData> TblCarData { get; set; }
        public virtual ICollection<TblTrailerTruck> TblTrailerTruck { get; set; }
    }
}
