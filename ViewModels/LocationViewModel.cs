using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

using VipcoTransport.Models;

namespace VipcoTransport.ViewModels
{
    public class LocationViewModel:TblLocation
    {
        public int RowNumber { get; private set; }
        public LocationViewModel(int rowNumber)
        {
            this.RowNumber = rowNumber;
        }
    }
}
