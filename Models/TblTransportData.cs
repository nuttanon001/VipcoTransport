using System;
using System.Collections.Generic;

namespace VipcoTransport.Models
{
    public partial class TblTransportData
    {
        public TblTransportData()
        {
            TblRequestHasDataTransport = new HashSet<TblRequestHasDataTransport>();
            TblTranHasCompany = new HashSet<TblTranHasCompany>();
        }

        public int TransportId { get; set; }
        public int? CarId { get; set; }
        public int? TrailerId { get; set; }
        public int? CarTypeId { get; set; }
        public string TransportNo { get; set; }
        public int? TransportTypeId { get; set; }
        public int? ContactSource { get; set; }
        public int? ContactDestination { get; set; }
        public string EmployeeRequestCode { get; set; }
        public int? EmployeeDrive { get; set; }
        public string EmployeeDriveCode { get; set; }
        public DateTime? TransDate { get; set; }
        public string TransTime { get; set; }
        public DateTime? FinalDate { get; set; }
        public string FinalTime { get; set; }
        public string TransportInformation { get; set; }
        public double? Cost { get; set; }
        public float? WeightLoad { get; set; }
        public double? Width { get; set; }
        public double? Length { get; set; }
        public double? Passenger { get; set; }
        public string Remark { get; set; }
        public string StatusNo { get; set; }
        public string JobInfo { get; set; }
        public string Creator { get; set; }
        public DateTime? CreateDate { get; set; }
        public string Modifyer { get; set; }
        public DateTime? ModifyDate { get; set; }

        public virtual ICollection<TblRequestHasDataTransport> TblRequestHasDataTransport { get; set; }
        public virtual ICollection<TblTranHasCompany> TblTranHasCompany { get; set; }
        public virtual TblCarData Car { get; set; }
        public virtual TblCarType CarType { get; set; }
        public virtual TblContact ContactDestinationNavigation { get; set; }
        public virtual TblContact ContactSourceNavigation { get; set; }
        public virtual TblDriver EmployeeDriveNavigation { get; set; }
        public virtual VEmployee EmployeeRequestCodeNavigation { get; set; }
        public virtual TblTrailerTruck Trailer { get; set; }
        public virtual TblTransportType TransportType { get; set; }
    }
}
