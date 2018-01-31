using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using VipcoTransport.Models;

namespace VipcoTransport.ViewModels
{
    public static class ApplicationContextExtensions
    {
        public static void EnsureSeedData(this ApplicationContext Context)
        {
            if (!Context.TblCarData.Any())
            {
                Random random_number = new Random();

                var Toyota = Context.TblCarBrand.Add(new TblCarBrand
                {
                    BrandDesc = "โตโยต้า",
                    BrandName = "Toyota",
                    CreateDate = DateTime.Now,
                    Creator = "someone",
                }).Entity;
                var Isuzu = Context.TblCarBrand.Add(new TblCarBrand
                {
                    BrandDesc = "อีซุซุ",
                    BrandName = "Isuzu",
                    CreateDate = DateTime.Now,
                    Creator = "someone",
                }).Entity;
                var Mitsubishi = Context.TblCarBrand.Add(new TblCarBrand
                {
                    BrandDesc = "มิตซูบิชิ",
                    BrandName = "Mitsubishi",
                    CreateDate = DateTime.Now,
                    Creator = "someone",
                }).Entity;
                var Nissan = Context.TblCarBrand.Add(new TblCarBrand
                {
                    BrandDesc = "นิสสัน",
                    BrandName = "Nissan",
                    CreateDate = DateTime.Now,
                    Creator = "someone",
                }).Entity;

                var PickupTruck = Context.TblCarType.Add(new TblCarType
                {
                    CarTypeDesc = "PickupTruck",
                    CarTypeNo = "01",
                    CreateDate = DateTime.Now,
                    Creator = "someone",
                    Remark = "",
                }).Entity;
                var CargoVan = Context.TblCarType.Add(new TblCarType
                {
                    CarTypeDesc = "CargoVan",
                    CarTypeNo = "02",
                    CreateDate = DateTime.Now,
                    Creator = "someone",
                    Remark = "",
                }).Entity;
                var CompactCar = Context.TblCarType.Add(new TblCarType
                {
                    CarTypeDesc = "CompactCar",
                    CarTypeNo = "03",
                    CreateDate = DateTime.Now,
                    Creator = "someone",
                    Remark = "",
                }).Entity;
                var MediumTrucks = Context.TblCarType.Add(new TblCarType
                {
                    CarTypeDesc = "Medium trucks",
                    CarTypeNo = "04",
                    CreateDate = DateTime.Now,
                    Creator = "someone",
                    Remark = "",
                }).Entity;
                var HeavyTrucks = Context.TblCarType.Add(new TblCarType
                {
                    CarTypeDesc = "Heavy trucks",
                    CarTypeNo = "05",
                    CreateDate = DateTime.Now,
                    Creator = "someone",
                    Remark = "",
                }).Entity;

                Context.TblCarData.Add(new TblCarData
                {
                    ActDate = DateTime.Today.AddMonths(random_number.Next(1,12)),
                    ActInsurance = "123456-987654",
                    CarBrandNavigation = Isuzu,
                    CarDate = DateTime.Today.AddYears(random_number.Next(1, 12) * -1),
                    CarNo = "01",
                    CarType = MediumTrucks,
                    CarWeight = (float)13.55,
                    CreateDate = DateTime.Today,
                    Creator = "someone",
                    Insurance = "BB123456-987/60",
                    InsurDate = DateTime.Today.AddMonths(random_number.Next(1, 12)),
                    RegisterNo = "งง-56478",
                    Remark = "",
                    Status = true,
                });
                Context.TblCarData.Add(new TblCarData
                {
                    ActDate = DateTime.Today.AddMonths(random_number.Next(1, 12)),
                    ActInsurance = "123454-345345",
                    CarBrandNavigation = Isuzu,
                    CarDate = DateTime.Today.AddYears(random_number.Next(1, 12) * -1),
                    CarNo = "02",
                    CarType = MediumTrucks,
                    CarWeight = (float)13.55,
                    CreateDate = DateTime.Today,
                    Creator = "someone",
                    Insurance = "ZZ12E123-987/60",
                    InsurDate = DateTime.Today.AddMonths(random_number.Next(1, 12)),
                    RegisterNo = "กด-87642",
                    Remark = "",
                    Status = true,
                });
                Context.TblCarData.Add(new TblCarData
                {
                    ActDate = DateTime.Today.AddMonths(random_number.Next(1, 12)),
                    ActInsurance = "564DAE-4452ZZ",
                    CarBrandNavigation = Toyota,
                    CarDate = DateTime.Today.AddYears(random_number.Next(1, 12) * -1),
                    CarNo = "03",
                    CarType = CargoVan,
                    CarWeight = (float)2.5,
                    CreateDate = DateTime.Today,
                    Creator = "someone",
                    Insurance = "AE895AS-854/60",
                    InsurDate = DateTime.Today.AddMonths(random_number.Next(1, 12)),
                    RegisterNo = "รท-8645",
                    Remark = "",
                    Status = true,
                });
                Context.TblCarData.Add(new TblCarData
                {
                    ActDate = DateTime.Today.AddMonths(random_number.Next(1, 12)),
                    ActInsurance = "123456-987654",
                    CarBrandNavigation = Toyota,
                    CarDate = DateTime.Today.AddYears(random_number.Next(1, 12) * -1),
                    CarNo = "04",
                    CarType = CompactCar,
                    CarWeight = (float)1.9,
                    CreateDate = DateTime.Today,
                    Creator = "someone",
                    Insurance = "AA131435-987/60",
                    InsurDate = DateTime.Today.AddMonths(random_number.Next(1, 12)),
                    RegisterNo = "ธก-5624",
                    Remark = "",
                    Status = true,
                });
                Context.TblCarData.Add(new TblCarData
                {
                    ActDate = DateTime.Today.AddMonths(random_number.Next(1, 12)),
                    ActInsurance = "123456-987654",
                    CarBrandNavigation = Nissan,
                    CarDate = DateTime.Today.AddYears(random_number.Next(1, 12) * -1),
                    CarNo = "05",
                    CarType = HeavyTrucks,
                    CarWeight = (float)23.50,
                    CreateDate = DateTime.Today,
                    Creator = "someone",
                    Insurance = "AA6454-123/60",
                    InsurDate = DateTime.Today.AddMonths(random_number.Next(1, 12)),
                    RegisterNo = "กพ-54-56478",
                    Remark = "",
                    Status = true,
                });
                Context.TblCarData.Add(new TblCarData
                {
                    ActDate = DateTime.Today.AddMonths(random_number.Next(1, 12)),
                    ActInsurance = "578789-987654",
                    CarBrandNavigation = Toyota,
                    CarDate = DateTime.Today.AddYears(random_number.Next(1, 12) * -1),
                    CarNo = "06",
                    CarType = PickupTruck,
                    CarWeight = (float)2.3,
                    CreateDate = DateTime.Today,
                    Creator = "someone",
                    Insurance = "BB123456-987/60",
                    InsurDate = DateTime.Today.AddMonths(random_number.Next(1, 12)),
                    RegisterNo = "4ดห-4547",
                    Remark = "",
                    Status = true,
                });

                Context.SaveChanges();
            }

            if (!Context.TblTrailerTruck.Any())
            {
                Random random_number = new Random();

                Context.TblTrailerTruck.Add(new TblTrailerTruck
                {
                    CarTypeId = 1,
                    CreateDate = DateTime.Now,
                    Creator = "someone",
                    Insurance = "EQ7984-56487",
                    InsurDate = DateTime.Today.AddMonths(random_number.Next(1, 12)),
                    RegisterNo = "ธน45-6588",
                    Remark = "",
                    TrailerBrand = 3,
                    TrailerDesc = "",
                    TrailerLoad = (float)25,
                    TrailerNo = "01",
                    TrailerWeight = (float)5.4,
                });
                Context.TblTrailerTruck.Add(new TblTrailerTruck
                {
                    CarTypeId = 3,
                    CreateDate = DateTime.Now,
                    Creator = "someone",
                    Insurance = "ET245785-56487",
                    InsurDate = DateTime.Today.AddMonths(random_number.Next(1, 12)),
                    RegisterNo = "กก45-75412",
                    Remark = "",
                    TrailerBrand = 2,
                    TrailerDesc = "",
                    TrailerLoad = (float)35,
                    TrailerNo = "02",
                    TrailerWeight = (float)6.5,
                });
                Context.TblTrailerTruck.Add(new TblTrailerTruck
                {
                    CarTypeId = 4,
                    CreateDate = DateTime.Now,
                    Creator = "someone",
                    Insurance = "AC456214-9831",
                    InsurDate = DateTime.Today.AddMonths(random_number.Next(1, 12)),
                    RegisterNo = "ภถ4-48765",
                    Remark = "",
                    TrailerBrand = 4,
                    TrailerDesc = "",
                    TrailerLoad = (float)35,
                    TrailerNo = "03",
                    TrailerWeight = (float)6.5,
                });

                Context.SaveChanges();
            }
        }
    }
}
