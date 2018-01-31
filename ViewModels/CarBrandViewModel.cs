using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

using VipcoTransport.Models;
namespace VipcoTransport.ViewModels
{
    public class CarBrandViewModel:TblCarBrand
    {
        public int RowNumber { get; set; }

        public CarBrandViewModel(int rowNumber)
        {
            this.RowNumber = rowNumber;
        }
    }
}
