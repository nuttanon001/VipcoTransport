using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

using VipcoTransport.Models;
namespace VipcoTransport.ViewModels
{
    public class TrailerViewModel:TblTrailerTruck
    {
        public string ImagesString { get; set; }
        public string BrandNameString => this.TrailerBrandNavigation?.BrandName ?? "-";
        public string TrailerTypeString => this.CarType?.CarTypeDesc ?? "-";
        public int? CompanyID { get; set; }
    }
}
