using System;
using System.Collections.Generic;

namespace VipcoTransport.Models
{
    public partial class TblUser
    {
        public TblUser()
        {
            TblUserHasCompany = new HashSet<TblUserHasCompany>();
        }

        public int UserId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string MailAddress { get; set; }
        public string MailPassword { get; set; }
        public byte? Role { get; set; }
        public string EmployeeCode { get; set; }
        public int? CompanyId { get; set; }
        public string Creator { get; set; }
        public DateTime? CreateDate { get; set; }
        public string Modifyer { get; set; }
        public DateTime? ModifyDate { get; set; }

        public virtual ICollection<TblUserHasCompany> TblUserHasCompany { get; set; }
        public virtual VEmployee EmployeeCodeNavigation { get; set; }
    }
}
