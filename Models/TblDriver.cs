using System;
using System.Collections.Generic;

namespace VipcoTransport.Models
{
    public partial class TblDriver
    {
        public TblDriver()
        {
            TblDriverHasCompany = new HashSet<TblDriverHasCompany>();
            TblTransportData = new HashSet<TblTransportData>();
        }

        public int EmployeeDriveId { get; set; }
        public string EmployeeDriveCode { get; set; }
        public int? CarId { get; set; }
        public int? TrailerId { get; set; }
        public string PhoneNumber { get; set; }
        public string Creator { get; set; }
        public DateTime? CreateDate { get; set; }
        public string Modifyer { get; set; }
        public DateTime? ModifyDate { get; set; }

        public virtual ICollection<TblDriverHasCompany> TblDriverHasCompany { get; set; }
        public virtual ICollection<TblTransportData> TblTransportData { get; set; }
        public virtual TblCarData Car { get; set; }
        public virtual VEmployee EmployeeDriveCodeNavigation { get; set; }
        public virtual TblTrailerTruck Trailer { get; set; }
    }
}
