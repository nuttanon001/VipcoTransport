using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

using VipcoTransport.Models;

namespace VipcoTransport.ViewModels
{
    public class DriverViewModel:TblDriver
    {
        public string EmployeeName => this?.EmployeeDriveCodeNavigation?.NameThai ?? "-";
        public string CarNo => (this?.Car?.CarNo ?? "") + $" {"-" + this?.Car?.CarBrandNavigation?.BrandName ?? ""}";
        public string TrailerNo => (this?.Trailer?.TrailerNo ?? "") + ("-" + this?.Trailer?.TrailerBrandNavigation?.BrandName ?? "");
        public int RowNumber { get; private set; }

        public DriverViewModel(int rowNumber)
        {
            this.RowNumber = rowNumber;
        }
    }

    public class DriverViewModel2 : TblDriver
    {
        public string EmployeeName { get; set; }
        public string CarNo { get; set; }
        public string TrailerNo { get; set; }
        public int? CompanyID { get; set; }
    }
}
