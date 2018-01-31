using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

using VipcoTransport.Models;
namespace VipcoTransport.ViewModels
{
    public class TransportDataViewModel:TblTransportData
    {
        private string stringTransDate;
        public string StringTransDate
        {
            get { return this.stringTransDate; }
            set
            {
                if (this.stringTransDate != value)
                {
                    this.stringTransDate = value;
                }
            }
        }
        public int? RoutineCount { get; set; }
        public int? RoutineDay { get; set; }
        public bool? RoutineTransport { get; set; }
        public string VehicleType => $"{this?.CarType?.CarTypeDesc ?? ""}";
        public string TransportTypeString => this?.TransportType?.TransportTypeDesc ?? "-";
        public string CarInfo => this.Car != null ? this.CarString : this.TrailerString ;
        public string CarString => $"{this?.Car?.CarNo ?? ""} - {this?.Car?.CarBrandNavigation?.BrandName}";
        public string TrailerString => $"{this?.Trailer?.TrailerNo ?? ""} - {this?.Trailer?.TrailerBrandNavigation?.BrandName}";
        public string EmployeeRequestString => $"{this?.EmployeeRequestCode ?? ""}{(" - " + this?.EmployeeRequestCodeNavigation?.NameThai) ?? ""}";
        public string EmployeeeDriverString => $"{this?.EmployeeDriveCode ?? ""}{(" - " + this?.EmployeeDriveNavigation?.EmployeeDriveCodeNavigation?.NameThai) ?? ""}";
        public string SoureString => $"{this?.ContactSourceNavigation?.LocationNavigation?.LocationName}"; //  - {this?.ContactSourceNavigation?.ContactName ?? ""}
        public string DestinationString => $"{this?.ContactDestinationNavigation?.LocationNavigation?.LocationName}"; //  - {this?.ContactDestinationNavigation?.ContactName ?? ""}
        public string StartDate => this.TransDate != null ? this.TransDate.Value.ToString("dd-MM-yy เวลา HH:mm") : "No-Data";
        public string ReturnDate => this.FinalDate != null ? this.FinalDate.Value.ToString("dd-MM-yy เวลา HH:mm") : "No-Data";
    }

    public class TransportData2ViewModel:TblTransportData
    {
        public int? RoutineCount { get; set; }
        public int? RoutineDay { get; set; }
        public bool? RoutineTransport { get; set; }
        public string VehicleType { get; set; }
        public string TransportTypeString { get; set; }
        public string CarInfo { get; set; }
        public string EmployeeRequestString { get; set; }
        public string EmployeeeDriverString { get; set; }
        public string SoureString { get; set; }
        public string DestinationString { get; set; }
        public string StartDate { get; set; }
        public string ReturnDate { get; set; }
        public int? CompanyID { get; set; }

        public string BranchName { get; set; }
    }
}
