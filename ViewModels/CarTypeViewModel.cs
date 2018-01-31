using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

using VipcoTransport.Models;
namespace VipcoTransport.ViewModels
{
    public class CarTypeViewModel:TblCarType
    {
        public int RowNumber { get; private set; }

        public CarTypeViewModel(int rowNumber)
        {
            this.RowNumber = rowNumber;
        }
    }
}
