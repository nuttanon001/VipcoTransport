using System;
using System.Collections.Generic;

namespace VipcoTransport.Models
{
    public partial class TblCarHasCompany
    {
        public int CarHasCompanyId { get; set; }
        public int? CarId { get; set; }
        public int? CompanyId { get; set; }
        public string Creator { get; set; }
        public DateTime? CreateDate { get; set; }
        public string Modifyer { get; set; }
        public DateTime? ModifyDate { get; set; }

        public virtual TblCarData Car { get; set; }
        public virtual TblCompany Company { get; set; }
    }
}
