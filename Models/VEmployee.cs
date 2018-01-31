using System;
using System.Collections.Generic;

namespace VipcoTransport.Models
{
    public partial class VEmployee
    {
        public VEmployee()
        {
            TblDriver = new HashSet<TblDriver>();
            TblTransportData = new HashSet<TblTransportData>();
            TblTransportRequestion = new HashSet<TblTransportRequestion>();
            TblUser = new HashSet<TblUser>();
        }

        public string EmpCode { get; set; }
        public string NameThai { get; set; }
        public string NameEng { get; set; }
        public string SectionCode { get; set; }
        public string SectionName { get; set; }

        public virtual ICollection<TblDriver> TblDriver { get; set; }
        public virtual ICollection<TblTransportData> TblTransportData { get; set; }
        public virtual ICollection<TblTransportRequestion> TblTransportRequestion { get; set; }
        public virtual ICollection<TblUser> TblUser { get; set; }
    }
}
