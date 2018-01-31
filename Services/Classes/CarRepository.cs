using System;
using System.Linq;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Collections.Generic;

using Microsoft.EntityFrameworkCore;

using VipcoTransport.Models;
using VipcoTransport.ViewModels;
using VipcoTransport.Services.Interfaces;
namespace VipcoTransport.Services.Classes
{
    public class CarRepository:ICarRepository
    {
        #region PrivateMembers

        private ApplicationContext Context;
        private DateTime DateOfServer
        {
            get
            {
                var connection = this.Context.Database.GetDbConnection();
                var command = connection.CreateCommand();
                command.CommandText = "SELECT SYSDATETIME()";
                connection.Open();
                var servertime = (DateTime)command.ExecuteScalar();
                connection.Close();

                return servertime;
            }
        }
        #endregion

        #region Constructor

        public CarRepository(ApplicationContext ctx)
        {
            this.Context = ctx;
        }

        #endregion

        #region TblCarData

        public IQueryable<TblCarData> GetTblCarDatas => this.Context.TblCarData.Include(x => x.CarBrandNavigation)
                                                                               .Include(x => x.CarType)
                                                                               .Include(x => x.TblCarHasCompany)
                                                                               .AsNoTracking()
                                                                               .OrderBy(x => x.CarNo);
        public DataViewModel<CarDataViewModel> GetCarDatasViewModelWithLazyLoad(LazyViewModel lazyData)
        {
            try
            {
                var Query = this.GetTblCarDatas;

                if (!string.IsNullOrEmpty(lazyData.filter))
                {
                    foreach (var keyword in lazyData.filter.Trim().ToLower().Split(null))
                    {
                        Query = Query.Where(x => x.CarNo.ToLower().Contains(keyword) ||
                                                 x.RegisterNo.ToLower().Contains(keyword) ||
                                                 x.Insurance.ToLower().Contains(keyword) ||
                                                 x.ActInsurance.ToLower().Contains(keyword) ||
                                                 x.Remark.ToLower().Contains(keyword) ||
                                                 x.CarType.CarTypeDesc.ToLower().Contains(keyword) ||
                                                 x.CarBrandNavigation.BrandDesc.ToLower().Contains(keyword));
                    }
                }

                if (lazyData.option.HasValue)
                {
                    if (lazyData.option.Value != -1)
                    {
                        int CompanyID = lazyData.option.Value;
                        Query = Query.Where(x => x.TblCarHasCompany.Any(z => z.CompanyId == CompanyID));
                    }
                }

                switch (lazyData.sortField)
                {
                    case "CarTypeDesc":
                        if (lazyData.sortOrder == -1)
                            Query = Query.OrderByDescending(x => x.CarType.CarTypeDesc);
                        else
                            Query = Query.OrderBy(x => x.CarType.CarTypeDesc);
                        break;

                    case "CarBrandDesc":
                        if (lazyData.sortOrder == -1)
                            Query = Query.OrderByDescending(x => x.CarBrandNavigation.BrandDesc);
                        else
                            Query = Query.OrderBy(x => x.CarBrandNavigation.BrandDesc);
                        break;

                    default:
                        if (lazyData.sortOrder == -1)
                            Query = Query.OrderByDescending(x => x.CarNo);
                        else
                            Query = Query.OrderBy(x => x.CarNo);
                        break;
                }

                //this.TotalRow = this.MyCountAsync(Query).Result;
                var totalRow = Query.Count();
                // select with rownumber
                Query = Query.Skip(lazyData.first ?? 0).Take(lazyData.rows ?? 50);

                var hasData = Query.AsEnumerable<TblCarData>().Select((x, i) => new CarDataViewModel
                {
                    RowNumber = (i + 1) + lazyData.first ?? 0,
                    CarId = x.CarId,
                    CarTypeDesc = x.CarType.CarTypeDesc,
                    CarNo = x.CarNo,
                    CarBrandDesc = x.CarBrandNavigation.BrandDesc,
                });
                return new DataViewModel<CarDataViewModel>(hasData, totalRow); ;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Has error {ex.ToString()}");
            }
            return null;
        }
        public TblCarData GetTblCarDataByKey(int CarID)
        {
            try
            {
                if (CarID > 0)
                {
                    return this.GetTblCarDatas.Where(x => x.CarId == CarID).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Has error {ex.ToString()}");
            }
            return null;
        }
        public TblCarData InsertTblCarDataToDataBase(TblCarData nCarData)
        {
            try
            {
                if (nCarData != null)
                {
                    nCarData.CreateDate = this.DateOfServer;
                    nCarData.Creator = nCarData.Creator ?? "someone";

                    this.Context.TblCarData.Add(nCarData);
                    return this.Context.SaveChanges() > 0 ? this.GetTblCarDataByKey(nCarData.CarId) : null;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Has error {ex.ToString()}");
            }

            return null;
        }
        public TblCarData UpdateTblCarDataToDataBase(int CarID,TblCarData uCarData)
        {
            try
            {
                var dbCarData = this.Context.TblCarData.Find(CarID);
                if (dbCarData != null)
                {
                    uCarData.CreateDate = dbCarData.CreateDate;
                    uCarData.ModifyDate = DateTime.Now;
                    uCarData.Modifyer = uCarData.Modifyer ?? "someone";

                    this.Context.Entry(dbCarData).CurrentValues.SetValues(uCarData);
                    return Context.SaveChanges() > 0 ? this.GetTblCarDataByKey(CarID) : null;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Has error {ex.ToString()}");
            }
            return null;
        }

        #endregion

        #region TblCarType

        public IQueryable<TblCarType> GetTblCarTypes => this.Context.TblCarType.AsNoTracking().OrderBy(x => x.CarTypeNo);
        public DataViewModel<TblCarType> GetTblCarTypeWithFilter(string filter)
        {
            try
            {
                var Query = this.GetTblCarTypes;
                if (!string.IsNullOrEmpty(filter))
                {
                    foreach (var keyword in filter.Trim().ToLower().Split(null))
                    {
                        Query = Query.Where(x => x.CarTypeDesc.ToLower().Contains(keyword) ||
                                                 x.CarTypeNo.ToLower().Contains(keyword) ||
                                                 x.Remark.ToLower().Contains(keyword));
                    }
                }
                return new DataViewModel<TblCarType>(Query.AsEnumerable<TblCarType>(), Query.Count());
            }
            catch(Exception ex)
            {
                Debug.WriteLine($"Has error {ex.ToString()}");
            }
            return null;
        }
        public TblCarType GetTblCarTypeWithKey(int CarTypeID)
        {
            try
            {
                if (CarTypeID > 0)
                {
                    return this.GetTblCarTypes.Where(x => x.CarTypeId == CarTypeID).FirstOrDefault();
                }
            }
            catch(Exception ex)
            {
                Debug.WriteLine($"Has error {ex.ToString()}");
            }
            return null;
        }
        public TblCarType InsertTblCarTypeToDataBase(TblCarType nCarType)
        {
            try
            {
                if (nCarType != null)
                {
                    nCarType.CreateDate = this.DateOfServer;
                    nCarType.Creator = nCarType.Creator ?? "someone";

                    this.Context.TblCarType.Add(nCarType);
                    return this.Context.SaveChanges() > 0 ? this.GetTblCarTypeWithKey(nCarType.CarTypeId) : null;
                }
            }
            catch(Exception ex)
            {
                Debug.WriteLine($"Has error {ex.ToString()}");
            }
            return null;
        }
        public TblCarType UpdateTblCarTypeToDataBase(int CarTypeID ,TblCarType uCarType)
        {
            try
            {
                var dbCarType = this.Context.TblCarType.Find(CarTypeID);
                if (dbCarType != null)
                {
                    uCarType.CreateDate = dbCarType.CreateDate;
                    uCarType.ModifyDate = this.DateOfServer;
                    uCarType.Modifyer = uCarType.Modifyer ?? "someone";

                    this.Context.Entry(dbCarType).CurrentValues.SetValues(uCarType);
                    return this.Context.SaveChanges() > 0 ? this.GetTblCarTypeWithKey(CarTypeID) : null;
                }
            }
            catch(Exception ex)
            {
                Debug.WriteLine($"Has error {ex.ToString()}");
            }
            return null;
        }

        #endregion

        #region TblCarBrand
        public IQueryable<TblCarBrand> GetTblCarBrands => this.Context.TblCarBrand.AsNoTracking().OrderBy(x => x.BrandDesc);
        public DataViewModel<TblCarBrand> GetTblCarBrandWithFilter(string filter = "")
        {
            try
            {
                var Query = this.GetTblCarBrands;
                if (!string.IsNullOrEmpty(filter))
                {
                    foreach (var keyword in filter.Trim().ToLower().Split(null))
                    {
                        Query = Query.Where(x => x.BrandDesc.ToLower().Contains(keyword) ||
                                                 x.BrandName.ToLower().Contains(keyword));
                    }
                }
                return new DataViewModel<TblCarBrand>(Query.AsEnumerable<TblCarBrand>(), Query.Count());
            }
            catch(Exception ex)
            {
                Debug.WriteLine($"Has error {ex.ToString()}");
            }
            return null;
        }
        public TblCarBrand GetTblCarBrandWithKey(int BrandID)
        {
            try
            {
                if (BrandID > 0)
                {
                    return this.GetTblCarBrands.Where(x => x.BrandId == BrandID).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Has error {ex.ToString()}");
            }
            return null;
        }
        public TblCarBrand InsertTblCarBrandToDataBase(TblCarBrand nCarBrand)
        {
            try
            {
                if (nCarBrand != null)
                {
                    nCarBrand.CreateDate = this.DateOfServer;
                    nCarBrand.Creator = nCarBrand.Creator ?? "someone";

                    this.Context.TblCarBrand.Add(nCarBrand);
                    return this.Context.SaveChanges() > 0 ? this.GetTblCarBrandWithKey(nCarBrand.BrandId) : null;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Has error " + ex.ToString());
            }
            return null;
        }
        public TblCarBrand UpdateTblCarBrandToDataBase(int BrandID , TblCarBrand uCarBrand)
        {
            try
            {
                var dbCarBrand = this.Context.TblCarBrand.Find(BrandID);
                if (dbCarBrand != null)
                {
                    uCarBrand.CreateDate = dbCarBrand.CreateDate;
                    uCarBrand.ModifyDate = this.DateOfServer;
                    uCarBrand.Modifyer = uCarBrand.Modifyer ?? "someone";

                    this.Context.Entry(dbCarBrand).CurrentValues.SetValues(uCarBrand);
                    return this.Context.SaveChanges() > 0 ? this.GetTblCarBrandWithKey(BrandID) : null;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Has error " + ex.ToString());
            }
            return null;

        }
        #endregion

        #region TblTrailerTruck
        public IQueryable<TblTrailerTruck> GetTblTrailerTrucks => this.Context.TblTrailerTruck.Include(x => x.CarType)
                                                                                              .Include(x => x.TrailerBrandNavigation)
                                                                                              .Include(x => x.TblTruckHasCompany)
                                                                                              .AsNoTracking()
                                                                                              .OrderBy(x => x.TrailerNo);
        public DataViewModel<TrailerTruckViewModel> GetTrailerTruckViewModelWithLazyLoad(LazyViewModel lazyData)
        {
            try
            {
                var Query = this.GetTblTrailerTrucks;

                if (!string.IsNullOrEmpty(lazyData.filter))
                {
                    foreach (var keyword in lazyData.filter.Trim().ToLower().Split(null))
                    {
                        Query = Query.Where(x => x.TrailerNo.ToLower().Contains(keyword) ||
                                              x.TrailerDesc.ToLower().Contains(keyword) ||
                                              x.RegisterNo.ToLower().Contains(keyword) ||
                                              x.Insurance.ToLower().Contains(keyword) ||
                                              x.Remark.ToLower().Contains(keyword) ||
                                              x.CarType.CarTypeDesc.ToLower().Contains(keyword) ||
                                              x.TrailerBrandNavigation.BrandDesc.ToLower().Contains(keyword));
                    }
                }

                if (lazyData.option.HasValue)
                {
                    if (lazyData.option.Value != -1)
                    {
                        int CompanyID = lazyData.option.Value;
                        Query = Query.Where(x => x.TblTruckHasCompany.Any(z => z.CompanyId == CompanyID));
                    }
                }

                switch (lazyData.sortField)
                {
                    case "TrailerTypeDesc":
                        if (lazyData.sortOrder == -1)
                            Query = Query.OrderByDescending(x => x.CarType.CarTypeDesc);
                        else
                            Query = Query.OrderBy(x => x.CarType.CarTypeDesc);
                        break;

                    case "TrailerBrandDesc":
                        if (lazyData.sortOrder == -1)
                            Query = Query.OrderByDescending(x => x.TrailerBrandNavigation.BrandDesc);
                        else
                            Query = Query.OrderBy(x => x.TrailerBrandNavigation.BrandDesc);
                        break;

                    default:
                        if (lazyData.sortOrder == -1)
                            Query = Query.OrderByDescending(x => x.TrailerNo);
                        else
                            Query = Query.OrderBy(x => x.TrailerNo);
                        break;
                }
                var totalRow = Query.Count();
                // select with rownumber
                Query = Query.Skip(lazyData.first ?? 0).Take(lazyData.rows ?? 50);

                var hasData = Query.AsEnumerable<TblTrailerTruck>().Select((x, i) => new TrailerTruckViewModel
                {
                    RowNumber = (i + 1) + lazyData.first ?? 0,
                    TrailerId = x.TrailerId,
                    TrailerBrandDesc = x?.TrailerBrandNavigation?.BrandDesc ?? "-",
                    TrailerNo = x.TrailerNo,
                    TrailerTypeDesc = x?.CarType?.CarTypeDesc ?? "-",
                });

                return new DataViewModel<TrailerTruckViewModel>(hasData, totalRow);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Has error {ex.ToString()}");
            }
            return null;
        }
        public TblTrailerTruck GetTblTrailerTruckWithKey(int TrailerID)
        {

            try
            {
                if (TrailerID > 0)
                {
                    return this.GetTblTrailerTrucks.Where(x => x.TrailerId == TrailerID).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Has error " + ex.ToString());
            }

            return null;
        }
        public TblTrailerTruck InsertTblTrailerTruckToDataBase(TblTrailerTruck nTrailer)
        {
            try
            {
                if (nTrailer != null)
                {
                    nTrailer.CreateDate = this.DateOfServer;
                    nTrailer.Creator = nTrailer.Creator ?? "someone";

                    this.Context.TblTrailerTruck.Add(nTrailer);
                    return this.Context.SaveChanges() > 0 ? this.GetTblTrailerTruckWithKey(nTrailer.TrailerId) : null;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Has error " + ex.ToString());
            }
            return null;

        }
        public TblTrailerTruck UpdateTblTrailerTruckToDataBase(int TrailerID, TblTrailerTruck uTrailer)
        {
            try
            {
                var dbTrailer = this.Context.TblTrailerTruck.Find(TrailerID);
                if (dbTrailer != null)
                {
                    uTrailer.CreateDate = dbTrailer.CreateDate;
                    uTrailer.ModifyDate = this.DateOfServer;
                    uTrailer.Modifyer = uTrailer.Modifyer ?? "someone";

                    this.Context.Entry(dbTrailer).CurrentValues.SetValues(uTrailer);
                    return this.Context.SaveChanges() > 0 ? this.GetTblTrailerTruckWithKey(TrailerID) : null;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Has error " + ex.ToString());
            }
            return null;
        }
        #endregion
    }
}
