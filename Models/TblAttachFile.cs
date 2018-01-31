using System;
using System.Collections.Generic;

namespace VipcoTransport.Models
{
    public partial class TblAttachFile
    {
        public int AttachId { get; set; }
        public string AttachFileName { get; set; }
        public string AttachAddress { get; set; }
        public string Creator { get; set; }
        public DateTime? CreateDate { get; set; }
        public string Modifyer { get; set; }
        public DateTime? ModifyDate { get; set; }
    }
}
