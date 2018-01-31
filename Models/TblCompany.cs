using System;
using System.Collections.Generic;

namespace VipcoTransport.Models
{
    public partial class TblCompany
    {
        public TblCompany()
        {
            TblCarHasCompany = new HashSet<TblCarHasCompany>();
            TblDriverHasCompany = new HashSet<TblDriverHasCompany>();
            TblReqHasCompany = new HashSet<TblReqHasCompany>();
            TblTranHasCompany = new HashSet<TblTranHasCompany>();
            TblTruckHasCompany = new HashSet<TblTruckHasCompany>();
            TblUserHasCompany = new HashSet<TblUserHasCompany>();
        }

        public int CompanyId { get; set; }
        public string CompanyCode { get; set; }
        public string CompanyName { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Telephone { get; set; }
        public string Creator { get; set; }
        public DateTime? CreateDate { get; set; }
        public string Modifyer { get; set; }
        public DateTime? ModifyDate { get; set; }

        public virtual ICollection<TblCarHasCompany> TblCarHasCompany { get; set; }
        public virtual ICollection<TblDriverHasCompany> TblDriverHasCompany { get; set; }
        public virtual ICollection<TblReqHasCompany> TblReqHasCompany { get; set; }
        public virtual ICollection<TblTranHasCompany> TblTranHasCompany { get; set; }
        public virtual ICollection<TblTruckHasCompany> TblTruckHasCompany { get; set; }
        public virtual ICollection<TblUserHasCompany> TblUserHasCompany { get; set; }
    }
}
