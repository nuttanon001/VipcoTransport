using System;
using System.Collections.Generic;

namespace VipcoTransport.Models
{
    public partial class TblTransportRequestHasAttach
    {
        public int TransportRequestHasAttachId { get; set; }
        public int TransportRequestId { get; set; }
        public int AttachId { get; set; }
        public string Creator { get; set; }
        public DateTime? CreateDate { get; set; }
        public string Modifyer { get; set; }
        public DateTime? ModifyDate { get; set; }
    }
}
