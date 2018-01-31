using System;
using System.Collections.Generic;

namespace VipcoTransport.Models
{
    public partial class TblLocation
    {
        public TblLocation()
        {
            TblContact = new HashSet<TblContact>();
        }

        public int LocationId { get; set; }
        public string LocationCode { get; set; }
        public string LocationName { get; set; }
        public string LocationAddress { get; set; }
        public string Creator { get; set; }
        public DateTime? CreateDate { get; set; }
        public string Modifyer { get; set; }
        public DateTime? ModifyDate { get; set; }

        public virtual ICollection<TblContact> TblContact { get; set; }
    }
}
