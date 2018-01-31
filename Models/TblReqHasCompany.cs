using System;
using System.Collections.Generic;

namespace VipcoTransport.Models
{
    public partial class TblReqHasCompany
    {
        public int ReqHasCompanyId { get; set; }
        public int? TransportRequestId { get; set; }
        public int? CompanyId { get; set; }
        public string Creator { get; set; }
        public DateTime? CreateDate { get; set; }
        public string Modifyer { get; set; }
        public DateTime? ModifyDate { get; set; }

        public virtual TblCompany Company { get; set; }
        public virtual TblTransportRequestion TransportRequest { get; set; }
    }
}
