using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

using VipcoTransport.Models;
namespace VipcoTransport.ViewModels
{
    public class ContactViewModel:TblContact
    {
        public string LocationName => this.LocationNavigation?.LocationName ?? "";
        public int RowNumber { get; private set; }
        public ContactViewModel(int rowNumber)
        {
            this.RowNumber = rowNumber;
        }
    }
}
