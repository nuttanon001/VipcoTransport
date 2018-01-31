using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VipcoTransport.Models;
namespace VipcoTransport.ViewModels
{
    public class UserViewModel: TblUser
    {
        public string FirstName { get; private set; }
        public string LastName { get; private set; }

        public string FullName => $"{this.FirstName} {this.LastName}";
        public string RoleString => this.Role == 1 ? "Requirement" :
            (this.Role == 2 ? "Standerd" :
            (this.Role == 3 ? "Administrator" :
            (this.Role == 4 ? "TopLevel" : "Anonymous")));

        public UserViewModel(TblUser dbUser)
        {
            if (dbUser != null) {
                var FullName = dbUser?.EmployeeCodeNavigation.NameThai.Trim().Split(null);
                if (FullName.Length > 1)
                {
                    this.FirstName = $"คุณ{FullName[0]}";
                    this.LastName = FullName[FullName.Length-1];
                }
            }
        }
    }
}
