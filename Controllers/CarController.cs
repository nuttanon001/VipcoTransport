using System;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.Collections.Generic;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;

using AutoMapper;
using Newtonsoft.Json;

using VipcoTransport.Models;
using VipcoTransport.ViewModels;
using VipcoTransport.Services.Interfaces;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace VipcoTransport.Controllers
{
    [Route("api/[controller]")]
    public class CarController : Controller
    {
        #region PrivateMenbers

        readonly ICarRepository repository;
        readonly IRepository<TblCarData> repositoryCar;
        readonly IRepository<TblTrailerTruck> repositoryTruck;
        readonly IRepository<TblCarHasCompany> repositoryCarCompany;
        readonly IRepository<TblTruckHasCompany> repositoryTruckCompany;
        readonly IMapper mapper;
        readonly IHostingEnvironment appEnvironment;
        private JsonSerializerSettings DefaultJsonSettings =>
            new JsonSerializerSettings()
            {
                Formatting = Formatting.Indented,
                PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
        private List<CarBrandViewModel> ToCarBrandViewModelList(IEnumerable<TblCarBrand> carBrands)
        {
            var listData = new List<CarBrandViewModel>();
            int RowNumber = 1;
            foreach (var carBrand in carBrands)
            {
                listData.Add(this.mapper.Map<TblCarBrand, CarBrandViewModel>(carBrand, new CarBrandViewModel(RowNumber)));
                RowNumber++;
            }
            return listData;
        }
        private List<CarTypeViewModel> ToCarTypeViewModelList(IEnumerable<TblCarType> carTypes)
        {
            var listData = new List<CarTypeViewModel>();
            int RowNumber = 1;
            foreach(var item in carTypes)
            {
                listData.Add(this.mapper.Map<TblCarType, CarTypeViewModel>(item, new CarTypeViewModel(RowNumber)));
                RowNumber++;
            }
            return listData;
        }
        private List<MapType> ConverterMap<MapType, TableType>(ICollection<TableType> tables)
        {
            var listData = new List<MapType>();
            foreach (var item in tables)
                listData.Add(this.mapper.Map<TableType, MapType>(item));
            return listData;
        }
        #endregion

        #region Constructor
        public CarController(
            ICarRepository repo, IHostingEnvironment appEnv,
            IMapper map, IRepository<TblCarData> repo2nd,
            IRepository<TblCarHasCompany> repo3rd,
            IRepository<TblTrailerTruck> repo4th,IRepository<TblTruckHasCompany> repo5th)
        {
            this.repository = repo;
            this.repositoryCar = repo2nd;
            this.repositoryCarCompany = repo3rd;
            this.repositoryTruck = repo4th;
            this.repositoryTruckCompany = repo5th;
            this.appEnvironment = appEnv;
            this.mapper = map;
        }
        #endregion

        #region TblCarData

        [HttpGet]
        [HttpGet("{key}")]
        public IActionResult GetTblCarDataFromDataBase(int key = 0)
        {
            if (key < 1)
                return new JsonResult(this.repository.GetTblCarDatas, this.DefaultJsonSettings);
            else
            {
                var hasData = this.repository.GetTblCarDataByKey(key);
                if (hasData != null)
                {
                    var hasDataVM = this.mapper.Map<TblCarData, CarViewModel>(hasData);

                    if (hasDataVM.Images.Length > 0)
                    {
                        hasDataVM.ImagesString = "data:image/png;base64," + System.Convert.ToBase64String(hasDataVM.Images);
                        hasDataVM.Images = null;
                    }

                    return new JsonResult(hasDataVM, this.DefaultJsonSettings);
                }
                else
                    return NotFound(new { Error = String.Format($"CarData ID {key} has not been found") });
            }
        }

        [HttpGet("GetCars")]
        public IActionResult GetCarViewModelFromDataBase()
        {
            var hasData = new List<CarViewModel>();
            foreach (var item in this.repository.GetTblCarDatas)
                hasData.Add(this.mapper.Map<TblCarData,CarViewModel>(item));
            return new JsonResult(hasData, this.DefaultJsonSettings);
        }

        [HttpGet("GetCarsByCompany/{CompanyId}")]
        public IActionResult GetCarsByCompany(int CompanyId)
        {
            //relates
            Expression<Func<TblCarData, object>> brand = a => a.CarBrandNavigation;
            Expression<Func<TblCarData, object>> type = s => s.CarType;

            var relates = new List<Expression<Func<TblCarData, object>>>
                { brand, type};
            // condition
            Expression<Func<TblCarData, bool>> condition = c => c.TblCarHasCompany.Any(x => x.CompanyId == CompanyId);
            // Json
            return new JsonResult(this.ConverterMap<Car2ViewModel,TblCarData>
                (this.repositoryCar.FindAllWithIncludeAsync(condition,relates).Result), this.DefaultJsonSettings);
        }

        [HttpPost("GetCarDataWithLazyLoad")]
        public IActionResult GetCarDataWithLazyLoadFromDataBase([FromBody] LazyViewModel lazyData)
        {
            if (lazyData != null)
                return new JsonResult(this.repository.GetCarDatasViewModelWithLazyLoad(lazyData), this.DefaultJsonSettings);
            return NotFound(new { Error = "lazy data has not been found" });
        }

        [HttpPost]
        public IActionResult PostTblCarDataToDataBase([FromBody] CarViewModel nCarData)
        {
            if (nCarData != null)
            {
                if (!string.IsNullOrEmpty(nCarData.ImagesString))
                {
                    var splitstring = nCarData.ImagesString.Split(',');
                    nCarData.Images = Convert.FromBase64String(splitstring[1]);
                }
                // Update to company
                int? CompanyId = null;
                if (nCarData.CompanyID.HasValue)
                    CompanyId = nCarData.CompanyID.Value;

                var hasData = this.repository.InsertTblCarDataToDataBase(this.mapper.Map<CarViewModel, TblCarData>(nCarData));
                if (hasData != null)
                {
                    if (CompanyId.HasValue)
                    {
                        this.repositoryCarCompany.Add(new TblCarHasCompany()
                        {
                            CarId = hasData.CarId,
                            CompanyId = CompanyId,
                            CreateDate = DateTime.Now,
                            Creator = hasData.Creator,
                        });
                    }
                    return new JsonResult(hasData, this.DefaultJsonSettings);
                }
            }
            return new StatusCodeResult(500);
        }

        [HttpPut("{key}")]
        public IActionResult PutTblCarDataToDataBase(int key, [FromBody] CarViewModel uCarData)
        {
            if (uCarData != null && key > 0)
            {
                if (!string.IsNullOrEmpty(uCarData.ImagesString))
                {
                    var splitstring = uCarData.ImagesString.Split(',');
                    uCarData.Images = Convert.FromBase64String(splitstring[1]);
                }

                // Update to company
                int? CompanyId = null;
                if (uCarData.CompanyID.HasValue)
                    CompanyId = uCarData.CompanyID.Value;

                var hasData = this.repository.UpdateTblCarDataToDataBase(key, uCarData);

                if (hasData != null)
                {
                    if (CompanyId.HasValue)
                    {
                        Expression<Func<TblCarHasCompany, bool>> condition = c => c.CarId == hasData.CarId;
                        var hasCarHasCompany = this.repositoryCarCompany.Find(condition);
                        if (hasCarHasCompany != null)
                        {
                            hasCarHasCompany.CompanyId = CompanyId;
                            hasCarHasCompany.Modifyer = hasData.Modifyer;
                            hasCarHasCompany.ModifyDate = hasData.ModifyDate;
                            // update to table car has company
                            this.repositoryCarCompany.Update(hasCarHasCompany, hasCarHasCompany.CarHasCompanyId);
                        }
                        else
                        {
                            this.repositoryCarCompany.Add(new TblCarHasCompany()
                            {
                                CarId = hasData.CarId,
                                CompanyId = CompanyId,
                                CreateDate = DateTime.Now,
                                Creator = hasData.Creator,
                            });
                        }
                    }
                    return new JsonResult(hasData, this.DefaultJsonSettings);
                }
                else
                    return new StatusCodeResult(500);
            }
            else
                return NotFound(new { Error = String.Format($"Car ID {key} has not been found") });
        }

        #endregion

        #region TblTrailerTruck

        [HttpGet("GetTrailer")]
        [HttpGet("GetTrailer/{key}")]
        public IActionResult GetTblTrailerTruckFromDataBase(int key = 0)
        {
            if (key < 1)
                return new JsonResult(this.repository.GetTblTrailerTrucks, this.DefaultJsonSettings);
            else
            {
                var hasData = this.repository.GetTblTrailerTruckWithKey(key);
                if (hasData != null)
                {
                    var hasDataVM = this.mapper.Map<TblTrailerTruck, TrailerViewModel>(hasData);

                    if (hasDataVM.Images.Length > 0)
                    {
                        hasDataVM.ImagesString = "data:image/png;base64," + System.Convert.ToBase64String(hasDataVM.Images);
                        hasDataVM.Images = null;
                    }

                    return new JsonResult(hasDataVM, this.DefaultJsonSettings);
                }
                else
                    return NotFound(new { Error = String.Format($"Trailer ID {key} has not been found") });
            }
        }

        [HttpGet("GetTrailers")]
        public IActionResult GetTrailersViewModelFromDataBase()
        {
            var hasData = new List<TrailerViewModel>();
            foreach (var item in this.repository.GetTblTrailerTrucks)
                hasData.Add(this.mapper.Map<TblTrailerTruck, TrailerViewModel>(item));

            return new JsonResult(hasData, this.DefaultJsonSettings);
        }

        [HttpGet("GetTrucksByCompany/{CompanyId}")]
        public IActionResult GetTrucksByCompany(int CompanyId)
        {
            //relates
            Expression<Func<TblTrailerTruck, object>> brand = a => a.TrailerBrandNavigation;
            Expression<Func<TblTrailerTruck, object>> type = s => s.CarType;

            var relates = new List<Expression<Func<TblTrailerTruck, object>>>
                { brand, type};
            // condition
            Expression<Func<TblTrailerTruck, bool>> condition = c => c.TblTruckHasCompany.Any(x => x.CompanyId == CompanyId);
            // Json
            return new JsonResult(this.ConverterMap<Truck2ViewModel,TblTrailerTruck>
                (this.repositoryTruck.FindAllWithIncludeAsync(condition, relates).Result)
                , this.DefaultJsonSettings);
        }

        [HttpPost("GetTrailerWithLazyData")]
        public IActionResult GetTrailerWithLazyLOadFromDataBase([FromBody] LazyViewModel lazyData)
        {
            if (lazyData != null)
            {
                var hasData = this.repository.GetTrailerTruckViewModelWithLazyLoad(lazyData);
                if (hasData != null)
                    return new JsonResult(hasData, this.DefaultJsonSettings);
            }
            return NotFound(new { Error = "lazy data has not been found" });
        }

        [HttpPost("PostTrailer")]
        public IActionResult PostTblTrailerTruckToDataBase([FromBody] TrailerViewModel nTrailer)
        {
            if (nTrailer != null)
            {
                if (!string.IsNullOrEmpty(nTrailer.ImagesString))
                {
                    var splitstring = nTrailer.ImagesString.Split(',');
                    nTrailer.Images = Convert.FromBase64String(splitstring[1]);
                }

                // Update to company
                int? CompanyId = null;
                if (nTrailer.CompanyID.HasValue)
                    CompanyId = nTrailer.CompanyID.Value;

                var hasData = this.repository.InsertTblTrailerTruckToDataBase(this.mapper.Map<TrailerViewModel, TblTrailerTruck>(nTrailer));
                if (hasData != null)
                {
                    if (CompanyId.HasValue)
                    {
                        this.repositoryTruckCompany.Add(new TblTruckHasCompany()
                        {
                            CompanyId = CompanyId,
                            CreateDate = DateTime.Now,
                            Creator = hasData.Creator,
                            TrailerId = hasData.TrailerId
                        });
                    }
                    return new JsonResult(hasData, this.DefaultJsonSettings);
                }
            }
            return new StatusCodeResult(500);
        }

        [HttpPut("PutTrailer/{key}")]
        public IActionResult PutTblTrailerTruckToDataBase(int key, [FromBody] TrailerViewModel uTrailer)
        {
            if (uTrailer != null && key > 0)
            {
                if (!string.IsNullOrEmpty(uTrailer.ImagesString))
                {
                    var splitstring = uTrailer.ImagesString.Split(',');
                    uTrailer.Images = Convert.FromBase64String(splitstring[1]);
                }

                // Update to company
                int? CompanyId = null;
                if (uTrailer.CompanyID.HasValue)
                    CompanyId = uTrailer.CompanyID.Value;

                var hasData = this.repository.UpdateTblTrailerTruckToDataBase(key, uTrailer);
                if (hasData != null)
                {
                    if (CompanyId.HasValue)
                    {
                        Expression<Func<TblTruckHasCompany, bool>> condition = c => c.TrailerId == hasData.TrailerId;
                        var hasCarHasCompany = this.repositoryTruckCompany.Find(condition);
                        if (hasCarHasCompany != null)
                        {
                            hasCarHasCompany.CompanyId = CompanyId;
                            hasCarHasCompany.Modifyer = hasData.Modifyer;
                            hasCarHasCompany.ModifyDate = hasData.ModifyDate;
                            // update to table car has company
                            this.repositoryTruckCompany.Update(hasCarHasCompany, hasCarHasCompany.TruckHasCompanyId);
                        }
                        else
                        {
                            this.repositoryTruckCompany.Add(new TblTruckHasCompany()
                            {
                                CompanyId = CompanyId,
                                CreateDate = DateTime.Now,
                                Creator = hasData.Creator,
                                TrailerId = hasData.TrailerId,
                            });
                        }
                    }
                    return new JsonResult(hasData, this.DefaultJsonSettings);
                }
                else
                    return new StatusCodeResult(500);
            }
            else
                return NotFound(new { Error = String.Format($"Customer ID {key} has not been found") });
        }

        #endregion

        #region TblCarBrand

        [HttpGet("GetCarBrand")]
        [HttpGet("GetCarBrand/{key}")]
        public IActionResult GetTblCarBrandFromDataBase(int key = 0)
        {
            if (key < 1)
                return new JsonResult(this.repository.GetTblCarBrands, this.DefaultJsonSettings);
            else
                return new JsonResult(this.repository.GetTblCarBrandWithKey(key), this.DefaultJsonSettings);
        }

        [HttpGet("GetCarBrandFilter")]
        [HttpGet("GetCarBrandFilter/{filter}")]
        public IActionResult GetTblCarBrandFromDataBase(string filter = "")
        {
            var hasData = this.repository.GetTblCarBrandWithFilter(filter);
            return new JsonResult(new DataViewModel<CarBrandViewModel>(this.ToCarBrandViewModelList(hasData.Data),hasData.TotalRecordCount), this.DefaultJsonSettings);
        }

        [HttpPost("PostCarBarnd")]
        public IActionResult PostTblCarBrandToDataBase([FromBody] TblCarBrand nCarBrand)
        {
            if (nCarBrand != null)
            {
                var hasData = this.repository.InsertTblCarBrandToDataBase(nCarBrand);
                if (hasData != null)
                    return new JsonResult(hasData, DefaultJsonSettings);
            }
            return new StatusCodeResult(500);
        }

        [HttpPut("PutCarBarnd/{key}")]
        public IActionResult PutTblCarBrandToDataBase(int key, [FromBody] TblCarBrand uCarBrand)
        {
            if (uCarBrand != null)
            {
                var hasData = this.repository.UpdateTblCarBrandToDataBase(key, uCarBrand);
                if (hasData != null)
                    return new JsonResult(hasData, DefaultJsonSettings);
            }
            return new StatusCodeResult(500);
        }

        #endregion

        #region TblCarType

        [HttpGet("GetCarType")]
        [HttpGet("GetCarType/{key}")]
        public IActionResult GetTblCarTypeFromDataBase(int key = 0)
        {
            if (key < 1)
                return new JsonResult(this.repository.GetTblCarTypes, this.DefaultJsonSettings);
            else
                return new JsonResult(this.repository.GetTblCarTypeWithKey(key), this.DefaultJsonSettings);
        }

        [HttpGet("GetCarTypeFilter")]
        [HttpGet("GetCarTypeFilter/{filter}")]
        public IActionResult GetTblCarTypeFromDataBase(string filter = "")
        {
            var hasData = this.repository.GetTblCarTypeWithFilter(filter);
            return new JsonResult(new DataViewModel<CarTypeViewModel>(this.ToCarTypeViewModelList(hasData.Data),hasData.TotalRecordCount), this.DefaultJsonSettings);
        }

        [HttpPost("PostCarType")]
        public IActionResult PostTblCarTypeToDataBase([FromBody] TblCarType nCarType)
        {
            if (nCarType != null)
            {
                var hasData = this.repository.InsertTblCarTypeToDataBase(nCarType);
                if (hasData != null)
                    return new JsonResult(hasData, DefaultJsonSettings);
            }
            return new StatusCodeResult(500);
        }

        [HttpPut("PutCarType/{key}")]
        public IActionResult PutTblCarTypeToDataBase(int key, [FromBody] TblCarType uCarType)
        {
            if (uCarType != null)
            {
                var hasData = this.repository.UpdateTblCarTypeToDataBase(key, uCarType);
                if (hasData != null)
                    return new JsonResult(hasData, DefaultJsonSettings);
            }
            return new StatusCodeResult(500);
        }
        #endregion
    }
}
