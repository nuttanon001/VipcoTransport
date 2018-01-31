
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using VipcoTransport.Models;
using VipcoTransport.ViewModels;

using AutoMapper;

namespace VipcoTransport.Classes
{
    public class MappingProfile: Profile
    {
        public MappingProfile()
        {
            //TblCarData
            CreateMap<TblCarData, CarViewModel>()
                 .ForMember(x => x.CompanyID,
                     obt => obt.MapFrom(src => src.TblCarHasCompany.Any() ?
                     src.TblCarHasCompany.FirstOrDefault().CompanyId : null));
            CreateMap<CarViewModel, TblCarData>();
            //TblCarData2
            CreateMap<TblCarData, Car2ViewModel>()
                .ForMember(x => x.BrandNameString, obt => obt.MapFrom(
                     src => src.CarBrandNavigation != null ? src.CarBrandNavigation.BrandName : "-"))
                .ForMember(x => x.CarTypeString, obt => obt.MapFrom(
                    src => src.CarType != null ? src.CarType.CarTypeDesc : "-"));


            //TblTrailerTruck
            CreateMap<TblTrailerTruck, TrailerViewModel>()
                 .ForMember(x => x.CompanyID,
                     obt => obt.MapFrom(src => src.TblTruckHasCompany.Any() ?
                     src.TblTruckHasCompany.FirstOrDefault().CompanyId : null));
            CreateMap<TrailerViewModel, TblTrailerTruck>();
            //TblTrailerTruck
            CreateMap<TblTrailerTruck,Truck2ViewModel>()
               .ForMember(x => x.BrandNameString, obt => obt.MapFrom(
                     src => src.TrailerBrandNavigation != null ? src.TrailerBrandNavigation.BrandName : "-"))
                .ForMember(x => x.TrailerTypeString, obt => obt.MapFrom(
                    src => src.CarType != null ? src.CarType.CarTypeDesc : "-"));


            //TblCarBrand
            CreateMap<TblCarBrand, CarBrandViewModel>();
            //TblCarType
            CreateMap<TblCarType, CarTypeViewModel>();
            //TblDriver
            CreateMap<TblDriver, DriverViewModel>();
            //TblDriver2
            CreateMap<TblDriver, DriverViewModel2>()
                .ForMember(x => x.EmployeeName, obt => obt.MapFrom(
                    src => src.EmployeeDriveCodeNavigation != null ? src.EmployeeDriveCodeNavigation.NameThai : ""))
                .ForMember(x => x.EmployeeDriveCodeNavigation,obt => obt.Ignore())

                .ForMember(x => x.CarNo, obt => obt.MapFrom(
                    src => src.Car != null ? $"{src.Car.CarNo}-{src.Car.CarBrandNavigation.BrandName}" : ""))
                .ForMember(x => x.Car, obt => obt.Ignore())

                .ForMember(x => x.TrailerNo, obt => obt.MapFrom(
                    src => src.Trailer != null ? $"{src.Trailer.TrailerNo}-{src.Trailer.TrailerBrandNavigation.BrandName}": ""))
                .ForMember(x => x.Trailer, obt => obt.Ignore())

                .ForMember(x => x.CompanyID,
                    obt => obt.MapFrom(src => src.TblDriverHasCompany.Any() ?
                    src.TblDriverHasCompany.FirstOrDefault().CompanyId : null))
                .ForMember(x => x.TblDriverHasCompany, obt => obt.Ignore());

            CreateMap<DriverViewModel2, TblDriver>();
            //TblContact
            CreateMap<TblContact, ContactViewModel>();
            //TblLocation
            CreateMap<TblLocation, LocationViewModel>();
            //TblTransportRequestion
            CreateMap<TblTransportRequestion, TransportRequestionViewModel>();
            CreateMap<TransportRequestionViewModel, TblTransportRequestion>();
            //TblTransportRequestion2
            CreateMap<TblTransportRequestion, TransportRequestion2ViewModel>()
                // CarType
                .ForMember(x => x.VehicleType, obt => obt.MapFrom(s => s.CarType.CarTypeDesc))
                .ForMember(x => x.CarType, o => o.Ignore())
                // EmployeeRequest
                .ForMember(x => x.EmployeeRequestString, o => o.MapFrom(s => $"{s.EmployeeRequestCode} - {s.EmployeeRequestCodeNavigation.NameThai}"))
                .ForMember(x => x.EmployeeRequestCodeNavigation, o => o.Ignore())
                // Source
                .ForMember(x => x.SoureString, o => o.MapFrom(s => s.ContactSourceNavigation.LocationNavigation.LocationName))
                .ForMember(x => x.ContactSourceNavigation, o => o.Ignore())
                // Destination
                .ForMember(x => x.DestinationString, o => o.MapFrom(s => s.ContactDestinationNavigation.LocationNavigation.LocationName))
                .ForMember(x => x.ContactDestinationNavigation, o => o.Ignore())
                // Status
                .ForMember(x => x.StringStaus, o => o.MapFrom(s => s.TransportStatus == 1 ? "Waiting" : (s.TransportStatus == 2 ? "Change Date-Time" : (s.TransportStatus == 3 ? "Completed" : "Cancelled"))))
                // Vehcile
                .ForMember(x => x.TypeOfVehcile, o => o.MapFrom(s => s.CarType.Type))
                // Request
                .ForMember(x => x.RequestTime, o => o.MapFrom(s => s.TransportReqDate.Value.ToString("HH:mm")))
                // Company
                .ForMember(x => x.CompanyID,
                    obt => obt.MapFrom(src => src.TblReqHasCompany.Any() ?
                    src.TblReqHasCompany.FirstOrDefault().CompanyId : null))
                .ForMember(x => x.BranchName,
                    obt => obt.MapFrom(s => s.TblReqHasCompany.Any() ?
                    s.TblReqHasCompany.FirstOrDefault().Company.CompanyName : ""))
                .ForMember(x => x.TblReqHasCompany, obt => obt.Ignore());
            CreateMap<TransportRequestion2ViewModel, TblTransportRequestion>();

            //TblTransportData
            CreateMap<TblTransportData, TransportDataViewModel>();
            CreateMap<TransportDataViewModel,TblTransportData>();
            //TblTransportData2
            CreateMap<TblTransportData, TransportData2ViewModel>()
                // CarType
                .ForMember(x => x.VehicleType, obt => obt.MapFrom(s => s.CarType.CarTypeDesc))
                .ForMember(x => x.CarType, o => o.Ignore())
                // TransportType
                .ForMember(x => x.TransportTypeString, obt => obt.MapFrom(s => s.TransportType.TransportTypeDesc))
                .ForMember(x => x.TransportType, o => o.Ignore())
                // Car Or Trailer
                .ForMember(x => x.CarInfo, o => o.MapFrom(s => s.Car != null ?
                    $"{s.Car.CarNo} - {s.Car.CarBrandNavigation.BrandName}" :
                    $"{s.Trailer.TrailerNo} - {s.Trailer.TrailerBrandNavigation.BrandName}"))
                .ForMember(x => x.Car, o => o.Ignore())
                .ForMember(x => x.Trailer, o => o.Ignore())
                // EmployeeRequest
                .ForMember(x => x.EmployeeRequestString, o => o.MapFrom(s => $"{s.EmployeeRequestCode} - {s.EmployeeRequestCodeNavigation.NameThai}"))
                .ForMember(x => x.EmployeeRequestCodeNavigation, o => o.Ignore())
                // EmployeeeDriver
                .ForMember(x => x.EmployeeeDriverString, o => o.MapFrom(s => $"{s.EmployeeDriveCode} - {s.EmployeeDriveNavigation.EmployeeDriveCodeNavigation.NameThai}"))
                .ForMember(x => x.EmployeeDriveNavigation, o => o.Ignore())
                // Soure
                .ForMember(x => x.SoureString, o => o.MapFrom(s => $"{s.ContactSourceNavigation.LocationNavigation.LocationName}"))
                .ForMember(x => x.ContactSourceNavigation, o => o.Ignore())
                // Destination
                .ForMember(x => x.DestinationString, o => o.MapFrom(s => $"{s.ContactDestinationNavigation.LocationNavigation.LocationName}"))
                .ForMember(x => x.ContactDestinationNavigation, o => o.Ignore())
                // StartDate
                .ForMember(x => x.StartDate, o => o.MapFrom(s => s.TransDate != null ? s.TransDate.Value.ToString("dd-MM-yy เวลา HH:mm") : "No-Data"))
                // ReturnDate
                .ForMember(x => x.ReturnDate, o => o.MapFrom(s => s.FinalDate != null ? s.FinalDate.Value.ToString("dd-MM-yy เวลา HH:mm") : "No-Data"))
                // Company
                .ForMember(x => x.BranchName, o => o.MapFrom(s => s.TblTranHasCompany.Any()
                        ? s.TblTranHasCompany.FirstOrDefault().Company.CompanyName : ""))
                .ForMember(x => x.CompanyID, o => o.MapFrom(s => s.TblTranHasCompany.Any()
                        ? s.TblTranHasCompany.FirstOrDefault().CompanyId : null))
                .ForMember(x => x.TblTranHasCompany, o => o.Ignore());

            CreateMap<TransportData2ViewModel, TblTransportData>();
            //TblUser
            CreateMap<TblUser, UserViewModel>()
                .ForMember(item => item.Password,obt => obt.Ignore())
                .ForMember(item => item.MailPassword,obt => obt.Ignore());
            CreateMap<UserViewModel, TblUser>();

        }
    }
}
