using System;
using System.Collections.Generic;

namespace VipcoTransport.Models
{
    public partial class TblContact
    {
        public TblContact()
        {
            TblTransportDataContactDestinationNavigation = new HashSet<TblTransportData>();
            TblTransportDataContactSourceNavigation = new HashSet<TblTransportData>();
            TblTransportRequestionContactDestinationNavigation = new HashSet<TblTransportRequestion>();
            TblTransportRequestionContactSourceNavigation = new HashSet<TblTransportRequestion>();
        }

        public int ContactId { get; set; }
        public int Location { get; set; }
        public string ContactName { get; set; }
        public string ContactPhone { get; set; }
        public string Creator { get; set; }
        public DateTime? CreateDate { get; set; }
        public string Modifyer { get; set; }
        public DateTime? ModifyDate { get; set; }

        public virtual ICollection<TblTransportData> TblTransportDataContactDestinationNavigation { get; set; }
        public virtual ICollection<TblTransportData> TblTransportDataContactSourceNavigation { get; set; }
        public virtual ICollection<TblTransportRequestion> TblTransportRequestionContactDestinationNavigation { get; set; }
        public virtual ICollection<TblTransportRequestion> TblTransportRequestionContactSourceNavigation { get; set; }
        public virtual TblLocation LocationNavigation { get; set; }
    }
}
