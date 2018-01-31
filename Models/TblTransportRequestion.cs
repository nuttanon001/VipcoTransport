using System;
using System.Collections.Generic;

namespace VipcoTransport.Models
{
    public partial class TblTransportRequestion
    {
        public TblTransportRequestion()
        {
            TblReqHasCompany = new HashSet<TblReqHasCompany>();
            TblRequestHasDataTransport = new HashSet<TblRequestHasDataTransport>();
        }

        public int TransportRequestId { get; set; }
        public int? CarTypeId { get; set; }
        public string EmployeeRequestCode { get; set; }
        public string DepartmentCode { get; set; }
        public int? TransportTypeId { get; set; }
        public string TransportType { get; set; }
        public byte? TransportStatus { get; set; }
        public string TransportReqNo { get; set; }
        public string EmailResponse { get; set; }
        public DateTime? TransportReqDate { get; set; }
        public DateTime? TransportDate { get; set; }
        public string TransportTime { get; set; }
        public int? ContactSource { get; set; }
        public int? ContactDestination { get; set; }
        public string ProblemName { get; set; }
        public string ProblemPhone { get; set; }
        public string TransportInformation { get; set; }
        public float? WeightLoad { get; set; }
        public double? Width { get; set; }
        public double? Length { get; set; }
        public double? Passenger { get; set; }
        public float? Cost { get; set; }
        public string JobInfo { get; set; }
        public string Creator { get; set; }
        public DateTime? CreateDate { get; set; }
        public string Modifyer { get; set; }
        public DateTime? ModifyDate { get; set; }

        public virtual ICollection<TblReqHasCompany> TblReqHasCompany { get; set; }
        public virtual ICollection<TblRequestHasDataTransport> TblRequestHasDataTransport { get; set; }
        public virtual TblCarType CarType { get; set; }
        public virtual TblContact ContactDestinationNavigation { get; set; }
        public virtual TblContact ContactSourceNavigation { get; set; }
        public virtual VEmployee EmployeeRequestCodeNavigation { get; set; }
        public virtual TblTransportType TransportTypeNavigation { get; set; }
    }
}
