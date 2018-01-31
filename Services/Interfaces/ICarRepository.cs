using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VipcoTransport.Models;
using VipcoTransport.ViewModels;

namespace VipcoTransport.Services.Interfaces
{
    public interface ICarRepository
    {
        //TblCarData
        IQueryable<TblCarData> GetTblCarDatas { get; }
        DataViewModel<CarDataViewModel> GetCarDatasViewModelWithLazyLoad(LazyViewModel lazyData);
        TblCarData GetTblCarDataByKey(int CarID);
        TblCarData InsertTblCarDataToDataBase(TblCarData nCarData);
        TblCarData UpdateTblCarDataToDataBase(int CarID, TblCarData uCarData);

        //TblCarType
        IQueryable<TblCarType> GetTblCarTypes { get; }
        DataViewModel<TblCarType> GetTblCarTypeWithFilter(string filter);
        TblCarType GetTblCarTypeWithKey(int CarTypeID);
        TblCarType InsertTblCarTypeToDataBase(TblCarType nCarType);
        TblCarType UpdateTblCarTypeToDataBase(int CarTypeID, TblCarType uCarType);

        //TblCarBrand
        IQueryable<TblCarBrand> GetTblCarBrands { get; }
        DataViewModel<TblCarBrand> GetTblCarBrandWithFilter(string filter);
        TblCarBrand GetTblCarBrandWithKey(int BrandID);
        TblCarBrand InsertTblCarBrandToDataBase(TblCarBrand nCarBrand);
        TblCarBrand UpdateTblCarBrandToDataBase(int BrandID, TblCarBrand uCarBrand);

        //TblTrailerTruck
        IQueryable<TblTrailerTruck> GetTblTrailerTrucks { get; }
        DataViewModel<TrailerTruckViewModel> GetTrailerTruckViewModelWithLazyLoad(LazyViewModel lazyData);
        TblTrailerTruck GetTblTrailerTruckWithKey(int TrailerID);
        TblTrailerTruck InsertTblTrailerTruckToDataBase(TblTrailerTruck nTrailer);
        TblTrailerTruck UpdateTblTrailerTruckToDataBase(int TrailerID, TblTrailerTruck uTrailer);
    }
}
