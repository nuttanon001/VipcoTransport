using System;
using VipcoTransport.Models;

namespace VipcoTransport.ViewModels
{
    public class TransportRequestionViewModel : TblTransportRequestion
    {
        public TransportRequestionViewModel()
        {
        }

        private string stringTransportDate;
        public string StringTransportDate
        {
            get { return this.stringTransportDate; }
            set
            {
                if (this.stringTransportDate != value)
                {
                    this.stringTransportDate = value;
                    //if (!string.IsNullOrEmpty(value))
                    //{
                    //    DateTime dt = new DateTime();
                    //    if (DateTime.TryParseExact(value,
                    //                        "d/M/yyyy",
                    //                        System.Globalization.CultureInfo.InvariantCulture,
                    //                        System.Globalization.DateTimeStyles.None,
                    //                        out dt))
                    //    {
                    //        this.TransportDate = dt;
                    //    }
                    //}
                }
            }
        }
        public string VehicleType => $"{this?.CarType?.CarTypeDesc ?? ""}";
        public string EmployeeRequestString => $"{this?.EmployeeRequestCode ?? ""} - {this?.EmployeeRequestCodeNavigation?.NameThai ?? ""}";
        public string SoureString => $"{this?.ContactSourceNavigation?.LocationNavigation?.LocationName}"; // {(" - " + this?.ContactSourceNavigation?.ContactName) ?? ""}
        public string DestinationString => $"{this?.ContactDestinationNavigation?.LocationNavigation?.LocationName}"; // {(" - " + this?.ContactDestinationNavigation?.ContactName) ?? ""}
        public string StringStaus => $"{(this.TransportStatus == 1 ? "Waiting" : (this.TransportStatus == 2 ? "Change Date-Time" : (this.TransportStatus == 3 ? "Completed" : "Cancelled")))}";
        public byte TypeOfVehcile => this?.CarType?.Type ?? 0;
        public string RequestTime => this.TransportReqDate?.ToString("HH:mm") ?? "-";
    }

    public class TransportRequestion2ViewModel : TblTransportRequestion
    {
        public string VehicleType { get; set; }
        public string EmployeeRequestString { get; set; }
        public string SoureString { get; set; }
        public string DestinationString { get; set; }
        public string StringStaus { get; set; }
        public byte TypeOfVehcile { get; set; }
        public string RequestTime { get; set; }
        public int? CompanyID { get; set; }
        public string BranchName { get; set; }
    }
}