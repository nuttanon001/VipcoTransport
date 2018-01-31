using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

using VipcoTransport.Models;

namespace VipcoTransport.ViewModels
{
    public class CarViewModel:TblCarData
    {
        public string ImagesString { get; set; }
        public string BrandNameString => this.CarBrandNavigation?.BrandName ?? "-";
        public string CarTypeString => this.CarType?.CarTypeDesc ?? "-";
        public int? CompanyID { get; set; }
    }
}
