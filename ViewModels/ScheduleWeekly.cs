using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VipcoTransport.ViewModels
{
    public class ScheduleWeeklyViewModel
    {
        public DateTime? StartDate2 { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate2 { get; set; }
        public DateTime? EndDate { get; set; }
        public int? CompanyID { get; set; }

        //public DateTime? EndDate
        //{
        //    get
        //    {
        //        if (!string.IsNullOrEmpty(this.EndDateString))
        //        {
        //            DateTime dt = new DateTime();
        //            if (DateTime.TryParseExact(this.EndDateString.Substring(0, 10),
        //                                "yyyy-MM-dd",
        //                                System.Globalization.CultureInfo.InvariantCulture,
        //                                System.Globalization.DateTimeStyles.None,
        //                                out dt))
        //            {
        //                return dt;
        //            }
        //        }

        //        return null;
        //    }
        //}

        //public DateTime? StartDate
        //{
        //    get
        //    {
        //        if (!string.IsNullOrEmpty(this.StartDateString))
        //        {
        //            DateTime dt = new DateTime();
        //            if (DateTime.TryParseExact(this.StartDateString.Substring(0, 10),
        //                                "yyyy-MM-dd",
        //                                System.Globalization.CultureInfo.InvariantCulture,
        //                                System.Globalization.DateTimeStyles.None,
        //                                out dt))
        //            {
        //                return dt;
        //            }
        //        }

        //        return null;
        //    }
        //}
    }
}
