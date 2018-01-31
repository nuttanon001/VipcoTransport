using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace VipcoTransport.ViewModels
{
    public class UserEditionViewModel
    {
        public int UserId { get; set; }
        public string EmployeeName { get; set; }
        public string UserName { get; set; }
        public string MailAddress { get; set; }
        public string PasswordOld { get; set; }
        public string PasswordNew { get; set; }
        public string PasswordConfirm { get; set; }
        public string CompanyName { get; set; }
        public int UserHasCompanyId { get; set; }
        public bool EmailAlert { get; set; }
        public int CompanyID { get; set; }
    }
}
