using System;
using System.Collections.Generic;

namespace VipcoTransport.Models
{
    public partial class TblUserHasCompany
    {
        public int UserHasCompanyId { get; set; }
        public int? UserId { get; set; }
        public int? CompanyId { get; set; }
        public bool? EmailAlert { get; set; }
        public string Creator { get; set; }
        public DateTime? CreateDate { get; set; }
        public string Modifyer { get; set; }
        public DateTime? ModifyDate { get; set; }

        public virtual TblCompany Company { get; set; }
        public virtual TblUser User { get; set; }
    }
}
