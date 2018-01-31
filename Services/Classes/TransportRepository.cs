using MimeKit;
using MailKit.Security;
using MailKit.Net.Smtp;

using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

using System;
using System.Linq;
using System.Dynamic;
using System.Diagnostics;
using System.Collections.Generic;

using VipcoTransport.Models;
using VipcoTransport.ViewModels;
using VipcoTransport.Services.Interfaces;
using System.Threading.Tasks;

namespace VipcoTransport.Services.Classes
{
    public class TransportRepository : ITransportRepository
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

        private List<int> ConveterStringToInt(List<string> listString)
        {
            List<int> listInt = new List<int>();

            foreach (var item in listString)
            {
                if (int.TryParse(item, out int itemInt))
                    listInt.Add(itemInt);
            }

            return listInt;
        }

        #endregion PrivateMembers

        #region Constructor

        public TransportRepository(ApplicationContext ctx)
        {
            this.Context = ctx;
        }

        #endregion Constructor

        #region TblTransportData

        public IQueryable<TblTransportData> GetTblTransportDatas => this.Context.TblTransportData
                                                                      .Include(x => x.ContactSourceNavigation.LocationNavigation)
                                                                      .Include(x => x.ContactDestinationNavigation.LocationNavigation)
                                                                      .Include(x => x.Car.CarBrandNavigation)
                                                                      .Include(x => x.CarType)
                                                                      .Include(x => x.Trailer.TrailerBrandNavigation)
                                                                      .Include(x => x.TransportType)
                                                                      .Include(x => x.EmployeeDriveNavigation.EmployeeDriveCodeNavigation)
                                                                      .Include(x => x.EmployeeRequestCodeNavigation)
                                                                      .Include(x => x.TblTranHasCompany)
                                                                        .ThenInclude(x => x.Company)
                                                                      .OrderByDescending(x => x.TransDate).ThenBy(x => x.TransTime)
                                                                      .AsNoTracking();

        #region TblTransportData of Car

        public IQueryable<TblTransportData> GetTblTransportDatasCar => this.Context.TblTransportData.Where(x => x.Car != null)
                                                                              .Include(x => x.ContactSourceNavigation.LocationNavigation)
                                                                              .Include(x => x.ContactDestinationNavigation.LocationNavigation)
                                                                              .Include(x => x.Car.CarBrandNavigation)
                                                                              .Include(x => x.Trailer.TrailerBrandNavigation)
                                                                              .Include(x => x.TransportType)
                                                                              .Include(x => x.EmployeeDriveNavigation.EmployeeDriveCodeNavigation)
                                                                              .Include(x => x.EmployeeRequestCodeNavigation)
                                                                              .AsNoTracking()
                                                                              .OrderByDescending(x => x.TransDate).ThenBy(x => x.TransTime);

        public DataViewModel<TransportLazyViewModel> GetTransportViewModelWithLazyLoad(LazyViewModel lazyData)
        {
            try
            {
                var Query = this.GetTblTransportDatasCar;

                if (!string.IsNullOrEmpty(lazyData.filter))
                {
                    foreach (var keyword in lazyData.filter.Trim().ToLower().Split(null))
                    {
                        Query = Query.Where(x => x.TransportNo.ToLower().Contains(keyword) ||
                                                 x.EmployeeRequestCode.ToLower().Contains(keyword) ||
                                                 x.TransportInformation.ToLower().Contains(keyword) ||
                                                 x.Remark.ToLower().Contains(keyword) ||
                                                 x.Car.CarNo.ToLower().Contains(keyword) ||
                                                 x.Car.CarBrandNavigation.BrandName.ToLower().Contains(keyword) ||
                                                 x.ContactDestinationNavigation.ContactName.ToLower().Contains(keyword) ||
                                                 x.ContactDestinationNavigation.ContactPhone.ToLower().Contains(keyword) ||
                                                 x.ContactDestinationNavigation.LocationNavigation.LocationCode.ToLower().Contains(keyword) ||
                                                 x.ContactDestinationNavigation.LocationNavigation.LocationName.ToLower().Contains(keyword) ||
                                                 x.ContactSourceNavigation.ContactName.ToLower().Contains(keyword) ||
                                                 x.ContactSourceNavigation.ContactPhone.ToLower().Contains(keyword) ||
                                                 x.ContactSourceNavigation.LocationNavigation.LocationCode.ToLower().Contains(keyword) ||
                                                 x.ContactSourceNavigation.LocationNavigation.LocationName.ToLower().Contains(keyword) ||
                                                 x.EmployeeDriveNavigation.EmployeeDriveCode.ToLower().Contains(keyword) ||
                                                 x.EmployeeDriveNavigation.EmployeeDriveCodeNavigation.NameThai.ToLower().Contains(keyword) ||
                                                 x.EmployeeRequestCodeNavigation.EmpCode.ToLower().Contains(keyword) ||
                                                 x.EmployeeRequestCodeNavigation.NameThai.ToLower().Contains(keyword));
                    }
                }

                if (lazyData.option.HasValue)
                {
                    if (lazyData.option.Value != -1)
                    {
                        int CompanyID = lazyData.option.Value;
                        Query = Query.Where(x => x.TblTranHasCompany.Any(z => z.CompanyId == CompanyID));
                    }
                }

                switch (lazyData.sortField)
                {
                    case "TransportDateTime":
                        if (lazyData.sortOrder == -1)
                            Query = Query.OrderByDescending(x => x.TransDate).ThenByDescending(x => x.TransTime);
                        else
                            Query = Query.OrderBy(x => x.TransDate).ThenBy(x => x.TransTime);
                        break;

                    case "DestinationString":
                        if (lazyData.sortOrder == -1)
                            Query = Query.OrderByDescending(x => x.ContactDestinationNavigation.LocationNavigation.LocationCode);
                        else
                            Query = Query.OrderBy(x => x.ContactDestinationNavigation.LocationNavigation.LocationCode);
                        break;

                    case "CarNo":
                        if (lazyData.sortOrder == -1)
                            Query = Query.OrderByDescending(x => x.Car.CarNo);
                        else
                            Query = Query.OrderBy(x => x.Car.CarNo);
                        break;

                    case null:
                        Query = Query.OrderByDescending(x => x.TransDate);
                        break;

                    case "TransportNo":
                        if (lazyData.sortOrder == -1)
                            Query = Query.OrderByDescending(x => x.Car.CarNo);
                        else
                            Query = Query.OrderBy(x => x.Car.CarNo);
                        break;

                    default:
                        Query = Query.OrderByDescending(x => x.TransportNo);
                        break;
                }

                var totalRow = Query.Count();
                // select with rownumber
                Query = Query.Skip(lazyData.first ?? 0).Take(lazyData.rows ?? 50);

                var hasData = Query.AsEnumerable<TblTransportData>().Select((x, i) => new TransportLazyViewModel
                {
                    RowNumber = (i + 1) + lazyData.first ?? 0,
                    TransportId = x.TransportId,
                    DestinationString = (x?.ContactDestinationNavigation?.LocationNavigation?.LocationName ?? ""),
                    TransportDateTime = (x?.TransDate?.ToString("dd/MM/yy") ?? "") + $" :{x.TransTime ?? ""}",
                    TransportNo = x.TransportNo,
                    CarNo = (x?.Car?.CarNo ?? "") + " - " + (x?.Car?.CarBrandNavigation?.BrandName ?? "")
                });

                return new DataViewModel<TransportLazyViewModel>(hasData, totalRow);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Has error {ex.ToString()}");
            }
            return null;
        }

        #endregion TblTransportData of Car

        #region TblTransportData of Truck

        public IQueryable<TblTransportData> GetTblTransportDatasTruck => this.Context.TblTransportData.Where(x => x.Trailer != null)
                                                                             .Include(x => x.ContactSourceNavigation.LocationNavigation)
                                                                             .Include(x => x.ContactDestinationNavigation.LocationNavigation)
                                                                             .Include(x => x.Car.CarBrandNavigation)
                                                                             .Include(x => x.Trailer.TrailerBrandNavigation)
                                                                             .Include(x => x.TransportType)
                                                                             .Include(x => x.EmployeeDriveNavigation.EmployeeDriveCodeNavigation)
                                                                             .Include(x => x.EmployeeRequestCodeNavigation)
                                                                             .AsNoTracking()
                                                                             .OrderByDescending(x => x.TransDate).ThenBy(x => x.TransTime);

        public DataViewModel<TransportLazyViewModel> GetTransportTruckViewModelWithLazyLoad(LazyViewModel lazyData)
        {
            try
            {
                var Query = this.GetTblTransportDatasTruck;

                if (!string.IsNullOrEmpty(lazyData.filter))
                {
                    foreach (var keyword in lazyData.filter.Trim().ToLower().Split(null))
                    {
                        Query = Query.Where(x => x.TransportNo.ToLower().Contains(keyword) ||
                                                 x.EmployeeRequestCode.ToLower().Contains(keyword) ||
                                                 x.TransportInformation.ToLower().Contains(keyword) ||
                                                 x.Remark.ToLower().Contains(keyword) ||
                                                 x.Trailer.TrailerNo.ToLower().Contains(keyword) ||
                                                 x.Trailer.TrailerBrandNavigation.BrandName.ToLower().Contains(keyword) ||
                                                 x.ContactDestinationNavigation.ContactName.ToLower().Contains(keyword) ||
                                                 x.ContactDestinationNavigation.ContactPhone.ToLower().Contains(keyword) ||
                                                 x.ContactDestinationNavigation.LocationNavigation.LocationCode.ToLower().Contains(keyword) ||
                                                 x.ContactDestinationNavigation.LocationNavigation.LocationName.ToLower().Contains(keyword) ||
                                                 x.ContactSourceNavigation.ContactName.ToLower().Contains(keyword) ||
                                                 x.ContactSourceNavigation.ContactPhone.ToLower().Contains(keyword) ||
                                                 x.ContactSourceNavigation.LocationNavigation.LocationCode.ToLower().Contains(keyword) ||
                                                 x.ContactSourceNavigation.LocationNavigation.LocationName.ToLower().Contains(keyword) ||
                                                 x.EmployeeDriveNavigation.EmployeeDriveCode.ToLower().Contains(keyword) ||
                                                 x.EmployeeDriveNavigation.EmployeeDriveCodeNavigation.NameThai.ToLower().Contains(keyword) ||
                                                 x.EmployeeRequestCodeNavigation.EmpCode.ToLower().Contains(keyword) ||
                                                 x.EmployeeRequestCodeNavigation.NameThai.ToLower().Contains(keyword));
                    }
                }

                if (lazyData.option.HasValue)
                {
                    if (lazyData.option.Value != -1)
                    {
                        int CompanyID = lazyData.option.Value;
                        Query = Query.Where(x => x.TblTranHasCompany.Any(z => z.CompanyId == CompanyID));
                    }
                }

                switch (lazyData.sortField)
                {
                    case "TransportDateTime":
                        if (lazyData.sortOrder == -1)
                            Query = Query.OrderByDescending(x => x.TransDate).ThenByDescending(x => x.TransTime);
                        else
                            Query = Query.OrderBy(x => x.TransDate).ThenBy(x => x.TransTime);
                        break;

                    case "DestinationString":
                        if (lazyData.sortOrder == -1)
                            Query = Query.OrderByDescending(x => x.ContactDestinationNavigation.LocationNavigation.LocationCode);
                        else
                            Query = Query.OrderBy(x => x.ContactDestinationNavigation.LocationNavigation.LocationCode);
                        break;

                    case "CarNo":
                        if (lazyData.sortOrder == -1)
                            Query = Query.OrderByDescending(x => x.Trailer.TrailerNo);
                        else
                            Query = Query.OrderBy(x => x.Trailer.TrailerNo);
                        break;

                    case null:
                        Query = Query.OrderByDescending(x => x.TransDate);
                        break;

                    case "TransportNo":
                        if (lazyData.sortOrder == -1)
                            Query = Query.OrderByDescending(x => x.TransportNo);
                        else
                            Query = Query.OrderBy(x => x.TransportNo);
                        break;

                    default:
                        Query = Query.OrderByDescending(x => x.TransDate);
                        break;
                }

                var totalRow = Query.Count();
                // select with rownumber
                Query = Query.Skip(lazyData.first ?? 0).Take(lazyData.rows ?? 50);

                var hasData = Query.AsEnumerable<TblTransportData>().Select((x, i) => new TransportLazyViewModel
                {
                    RowNumber = (i + 1) + lazyData.first ?? 0,
                    TransportId = x.TransportId,
                    DestinationString = (x?.ContactDestinationNavigation?.LocationNavigation?.LocationName ?? ""),
                    TransportDateTime = (x?.TransDate?.ToString("dd/MM/yy") ?? "") + $" :{x.TransTime ?? ""}",
                    TransportNo = x.TransportNo,
                    CarNo = (x?.Trailer?.TrailerNo ?? "") + " - " + (x?.Trailer?.TrailerBrandNavigation?.BrandName ?? "")
                });

                return new DataViewModel<TransportLazyViewModel>(hasData, totalRow);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Has error {ex.ToString()}");
            }
            return null;
        }

        #endregion TblTransportData of Truck

        public ScheduleViewModel<IDictionary<String, Object>> GetTransportScheduleByDaily(DateTime pickDate)
        {
            try
            {
                if (pickDate != null && pickDate > DateTime.MinValue)
                {
                    var hasData = new List<IDictionary<String, Object>>();
                    var transportDatas = this.GetTblTransportDatas.Where(x => x.TransDate.Value.Date == pickDate.Date).ToList();

                    if (transportDatas.Any())
                    {
                        List<string> columnNames = new List<string>() { "Time" };
                        foreach (var item in transportDatas.Where(x => x.Car != null).GroupBy(x => x.Car)
                                                            .Select(x => x.Key.CarNo + " - " + x.Key.CarBrandNavigation.BrandName))
                        {
                            columnNames.Add("Car " + item);
                        }

                        foreach (var item in transportDatas.Where(x => x.Trailer != null).GroupBy(x => x.Trailer)
                                                            .Select(x => x.Key.TrailerNo + " - " + x.Key.TrailerBrandNavigation.BrandName))
                        {
                            columnNames.Add("Truck " + item);
                        }

                        var timeCon = new TimeSpan(0, 0, 0);

                        foreach (var item in transportDatas.Where(x => x.Car != null).OrderBy(x => x.TransDate).ThenBy(x => x.Car.CarNo))
                        {
                            //var TrailerInfo = item.Trailer == null ? "" : $" | Trailer : {item?.Trailer?.TrailerNo} - {item?.Trailer?.TrailerBrandNavigation?.BrandName}";
                            var infomation = string.IsNullOrEmpty(item.TransportInformation) ? "" : $" | Info : { item.TransportInformation}";
                            var dataSchedule = new TransportDayScheduleViewModel()
                            {
                                CarInfo = $"Car {(item?.Car?.CarNo ?? "")} - {(item?.Car?.CarBrandNavigation?.BrandName ?? "")}",
                                Information = $"From : {item?.ContactSourceNavigation?.LocationNavigation?.LocationCode ?? "no-data"}  To : {item?.ContactDestinationNavigation?.LocationNavigation?.LocationCode ?? "no-data"} {infomation}",
                                TimeString = $"{item.TransDate.Value.ToString("dd/MM/yy | HH:mm")} 24hr",
                                TransportId = item.TransportId
                            };

                            var data = new ExpandoObject();
                            // add column time
                            ((IDictionary<String, Object>)data).Add(columnNames[0], dataSchedule.TimeString);
                            ((IDictionary<String, Object>)data).Add(columnNames.Where(x => x.Contains(dataSchedule.CarInfo)).FirstOrDefault(),
                                $"{dataSchedule.Information}");
                            hasData.Add(data);
                        }

                        foreach (var item in transportDatas.Where(x => x.Trailer != null).OrderBy(x => x.TransDate).ThenBy(x => x.Trailer.TrailerNo))
                        {
                            var infomation = string.IsNullOrEmpty(item.TransportInformation) ? "" : $" | Info : { item.TransportInformation}";
                            var dataSchedule = new TransportDayScheduleViewModel()
                            {
                                CarInfo = $"Truck {item?.Trailer?.TrailerNo} - {item?.Trailer?.TrailerBrandNavigation?.BrandName}",
                                Information = $"From : {item?.ContactSourceNavigation?.LocationNavigation?.LocationCode ?? " - "}  To : {item?.ContactDestinationNavigation?.LocationNavigation?.LocationCode ?? " - "} {infomation}",
                                TimeString = $"{item.TransDate.Value.ToString("dd/MM/yy | HH:mm")} 24hr",
                                TransportId = item.TransportId
                            };

                            var data = new ExpandoObject();
                            // add column time
                            ((IDictionary<String, Object>)data).Add(columnNames[0], dataSchedule.TimeString);
                            ((IDictionary<String, Object>)data).Add(columnNames.Where(x => x.Contains(dataSchedule.CarInfo)).FirstOrDefault(),
                                $"{dataSchedule.Information}");
                            hasData.Add(data);
                        }
                        //return new ScheduleViewModel<ExpandoObject>(hasData.ToList(), columnNames);
                        return new ScheduleViewModel<IDictionary<string, object>>(hasData, columnNames);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.Write($"Has error {ex.ToString()}");
            }
            return null;
        }

        public ScheduleViewModel<IDictionary<String, Object>> GetTransportScheduleByWeeklyV2(ScheduleWeeklyViewModel scheduleWeekly)
        {
            try
            {
                if (scheduleWeekly != null)
                {
                    var StartDate = scheduleWeekly.StartDate2.Value;
                    var EndDate = scheduleWeekly.EndDate2.Value;

                    var hasData = new List<IDictionary<String, Object>>();
                    var transportDatas = this.GetTblTransportDatas
                                                .Where(x => x.TransDate.Value.Date >= StartDate.Date &&
                                                            x.TransDate.Value.Date <= EndDate.Date)
                                                .AsQueryable();

                    if (scheduleWeekly.CompanyID.HasValue)
                    {
                        if (scheduleWeekly.CompanyID.Value != -1)
                            transportDatas = transportDatas
                                .Where(x => x.TblTranHasCompany.Any(z => z.CompanyId == scheduleWeekly.CompanyID.Value));
                    }

                    if (transportDatas.Any())
                    {
                        List<string> columnNames = new List<string>() { "Car" };
                        foreach (var item in transportDatas.OrderBy(x => x.TransDate).GroupBy(x => x.TransDate.Value.Date)
                                                            .Select(x => x.Key))
                        {
                            columnNames.Add(item.ToString("dd/MM/yy"));
                        }

                        var timeCon = new TimeSpan(0, 0, 0);

                        // For car
                        foreach (var transport in transportDatas.Where(x => x.Car != null).GroupBy(x => x.Car).Select(x => x.Key))
                        {
                            if (transport == null)
                                continue;

                            var CarHasCom = this.Context.TblCarHasCompany
                                                .Include(x => x.Company)
                                                .SingleOrDefault(x => x.CarId == transport.CarId);

                            var data = new ExpandoObject();
                            var Driver = this.Context.TblDriver.Where(x => x.CarId == transport.CarId).Include(x => x.EmployeeDriveCodeNavigation).FirstOrDefault();
                            var CarInfo =
                                $"Car {(transport?.CarNo ?? "")} {(Driver != null ? "| คุณ" + Driver.EmployeeDriveCodeNavigation.NameThai : "")}"
                                + (CarHasCom != null ? $" ({CarHasCom.Company.CompanyName})" : "");
                            // add column time
                            ((IDictionary<String, Object>)data).Add(columnNames[0], CarInfo);
                            foreach (var item in transportDatas.Where(x => x.CarId == transport.CarId).OrderBy(x => x.TransDate).ThenBy(x => x.Car.CarNo))
                            {
                                var dateString = item.TransDate.Value.ToString("dd/MM/yy");

                                var key = columnNames.Where(y => y.Contains(dateString)).FirstOrDefault();
                                if (((IDictionary<String, Object>)data).Keys.Any(x => x == key))
                                    ((IDictionary<String, Object>)data)[key] += $"#{item.TransportId}";
                                else
                                    ((IDictionary<String, Object>)data).Add(key, $"{item.TransportId}");
                            }
                            hasData.Add(data);
                        }

                        // For Trailer
                        foreach (var transport in transportDatas.Where(x => x.Trailer != null).GroupBy(x => x.Trailer).Select(x => x.Key))
                        {
                            if (transport == null)
                                continue;

                            var TruckHasCom = this.Context.TblTruckHasCompany
                                                    .Include(x => x.Company)
                                                    .SingleOrDefault(x => x.TrailerId == transport.TrailerId);

                            var data = new ExpandoObject();
                            var Driver = this.Context.TblDriver.Where(x => x.TrailerId == transport.TrailerId).Include(x => x.EmployeeDriveCodeNavigation).FirstOrDefault();
                            var TrailerInfo = $"Truck {(transport?.TrailerNo ?? "")} {(Driver != null ? "| คุณ" + Driver.EmployeeDriveCodeNavigation.NameThai : "")}"
                                 + (TruckHasCom != null ? $" ({TruckHasCom.Company.CompanyName})" : "");
                            // add column time
                            ((IDictionary<String, Object>)data).Add(columnNames[0], TrailerInfo);
                            foreach (var item in transportDatas.Where(x => x.TrailerId == transport.TrailerId).OrderBy(x => x.TransDate).ThenBy(x => x.Trailer.TrailerId))
                            {
                                var dateString = item.TransDate.Value.ToString("dd/MM/yy");

                                var key = columnNames.Where(y => y.Contains(dateString)).FirstOrDefault();
                                if (((IDictionary<String, Object>)data).Keys.Any(x => x == key))
                                    ((IDictionary<String, Object>)data)[key] += $"#{item.TransportId}";
                                else
                                    ((IDictionary<String, Object>)data).Add(key, $"{item.TransportId}");
                            }
                            hasData.Add(data);
                        }
                        return new ScheduleViewModel<IDictionary<string, object>>(hasData, columnNames);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Has error {ex.ToString()}");
            }

            return null;
        }

        public ScheduleViewModel<IDictionary<String, Object>> GetTransportScheduleByWeekly(DateTime startDate, DateTime endDate)
        {
            try
            {
                if (startDate != null && startDate >= DateTime.MinValue && endDate != null && endDate >= DateTime.MinValue)
                {
                    var hasData = new List<IDictionary<String, Object>>();
                    var transportDatas = this.GetTblTransportDatas.Where(x => x.TransDate.Value.Date >= startDate.Date &&
                                                                              x.TransDate.Value.Date <= endDate.Date).ToList();

                    if (transportDatas.Any())
                    {
                        List<string> columnNames = new List<string>() { "Car" };
                        foreach (var item in transportDatas.OrderBy(x => x.TransDate).GroupBy(x => x.TransDate.Value.Date)
                                                            .Select(x => x.Key))
                        {
                            columnNames.Add(item.ToString("dd/MM/yy"));
                        }

                        var timeCon = new TimeSpan(0, 0, 0);

                        // For car
                        foreach (var transport in transportDatas.Where(x => x.Car != null).GroupBy(x => x.Car).Select(x => x.Key))
                        {
                            if (transport == null)
                                continue;

                            var data = new ExpandoObject();
                            var CarInfo = $"Car {(transport?.CarNo ?? "")} - {(transport?.CarBrandNavigation?.BrandName ?? "")}";
                            // add column time
                            ((IDictionary<String, Object>)data).Add(columnNames[0], CarInfo);
                            foreach (var item in transportDatas.Where(x => x.CarId == transport.CarId).OrderBy(x => x.TransDate).ThenBy(x => x.Car.CarNo))
                            {
                                //var TrailerInfo = item.Trailer == null ? "" : $" | Trailer : {item?.Trailer?.TrailerNo} - {item?.Trailer?.TrailerBrandNavigation?.BrandName}";
                                var infomation = string.IsNullOrEmpty(item.TransportInformation) ? "" : $" | Info : { item.TransportInformation}";
                                var dateString = item.TransDate.Value.ToString("dd/MM/yy");

                                var dataSchedule = new TransportDayScheduleViewModel()
                                {
                                    CarInfo = CarInfo,
                                    Information = $"From : {item?.ContactSourceNavigation?.LocationNavigation?.LocationCode ?? " - "}  To : {item?.ContactDestinationNavigation?.LocationNavigation?.LocationCode ?? " - "} {infomation} ", // {TrailerInfo}
                                    TimeString = $"Date : {item.TransDate.Value.ToString("dd/MM/yy | HH:mm")}",
                                    TransportId = item.TransportId
                                };

                                var key = columnNames.Where(y => y.Contains(dateString)).FirstOrDefault();
                                if (((IDictionary<String, Object>)data).Keys.Any(x => x == key))
                                    ((IDictionary<String, Object>)data)[key] += $" || {dataSchedule.Information + " " + dataSchedule.TimeString}#{item.TransportId}";
                                else
                                    ((IDictionary<String, Object>)data).Add(key, $"{dataSchedule.Information + " " + dataSchedule.TimeString}#{item.TransportId}");
                            }
                            hasData.Add(data);
                        }

                        // For Trailer
                        foreach (var transport in transportDatas.Where(x => x.Trailer != null).GroupBy(x => x.Trailer).Select(x => x.Key))
                        {
                            if (transport == null)
                                continue;

                            var data = new ExpandoObject();
                            var TrailerInfo = $"Truck {(transport?.TrailerNo ?? "")} - {(transport?.TrailerBrandNavigation?.BrandName ?? "")}";
                            // add column time
                            ((IDictionary<String, Object>)data).Add(columnNames[0], TrailerInfo);
                            foreach (var item in transportDatas.Where(x => x.TrailerId == transport.TrailerId).OrderBy(x => x.TransDate).ThenBy(x => x.Trailer.TrailerId))
                            {
                                //var TrailerInfo = item.Trailer == null ? "" : $" | Trailer : {item?.Trailer?.TrailerNo} - {item?.Trailer?.TrailerBrandNavigation?.BrandName}";
                                var infomation = string.IsNullOrEmpty(item.TransportInformation) ? "" : $" | Info : { item.TransportInformation}";
                                var dateString = item.TransDate.Value.ToString("dd/MM/yy");

                                var dataSchedule = new TransportDayScheduleViewModel()
                                {
                                    CarInfo = TrailerInfo,
                                    Information = $"From : {item?.ContactSourceNavigation?.LocationNavigation?.LocationCode ?? " - "}  To : {item?.ContactDestinationNavigation?.LocationNavigation?.LocationCode ?? " - "} {infomation} ", // {TrailerInfo}
                                    TimeString = $"Date : {item.TransDate.Value.ToString("dd/MM/yy | HH:mm")}",
                                    TransportId = item.TransportId
                                };

                                var key = columnNames.Where(y => y.Contains(dateString)).FirstOrDefault();
                                if (((IDictionary<String, Object>)data).Keys.Any(x => x == key))
                                    ((IDictionary<String, Object>)data)[key] += $" || {dataSchedule.Information + " " + dataSchedule.TimeString}#{item.TransportId}";
                                else
                                    ((IDictionary<String, Object>)data).Add(key, $"{dataSchedule.Information + " " + dataSchedule.TimeString}#{item.TransportId}");
                            }
                            hasData.Add(data);
                        }
                        //return new ScheduleViewModel<ExpandoObject>(hasData.ToList(), columnNames);
                        return new ScheduleViewModel<IDictionary<string, object>>(hasData, columnNames);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Has error {ex.ToString()}");
            }

            return null;
        }

        public ScheduleViewModel<IDictionary<String, Object>> GetTransportScheduleDailyWithTransportData(string stringData)
        {
            try
            {
                if (!string.IsNullOrEmpty(stringData))
                {
                    var hasData = new List<IDictionary<String, Object>>();
                    List<string> columnNames = new List<string>() { "To" };
                    List<int> keys = this.ConveterStringToInt(stringData.Split('#').ToList());
                    var listTransport = this.GetTblTransportDatas.Where(x => keys.Any(y => y == x.TransportId)).OrderBy(x => x.TransDate).ToList();

                    foreach (var location in listTransport.Where(x => x.ContactDestinationNavigation != null && x.ContactDestinationNavigation.LocationNavigation != null)
                        .GroupBy(x => x.ContactDestinationNavigation.LocationNavigation).Select(x => x.Key))
                    {
                        // add row data for column To
                        var data = new ExpandoObject();
                        var rowHeader = $"{location.LocationName}";
                        ((IDictionary<String, Object>)data).Add(columnNames[0], rowHeader);

                        foreach (var tranSport in listTransport.Where(x => x.ContactDestinationNavigation.LocationNavigation.LocationId == location.LocationId))
                        {
                            var timeDiff = tranSport.FinalDate - tranSport.TransDate;
                            // add column of time
                            var col = $"{tranSport.TransTime} - {(timeDiff.Value.TotalDays >= 1 ? "(+" + timeDiff.Value.Days + ")" : "")}{tranSport.FinalTime}";
                            columnNames.Add(string.IsNullOrEmpty(col) ? "-" : col);
                            // add row data for column time
                            var key = columnNames.Where(y => y.Contains(string.IsNullOrEmpty(col) ? "-" : col)).FirstOrDefault();
                            if (((IDictionary<String, Object>)data).Keys.Any(x => x == key))
                                ((IDictionary<String, Object>)data)[key] += $"Data#{tranSport.TransportId}";
                            else
                                ((IDictionary<String, Object>)data).Add(string.IsNullOrEmpty(col) ? "-" : col, $"Data#{tranSport.TransportId}");
                        }

                        hasData.Add(data);

                    }
                    return new ScheduleViewModel<IDictionary<string, object>>(hasData, columnNames);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Has error  {ex.ToString()}");
            }
            return null;
        }

        public IEnumerable<TblTransportData> GetTransportDataSameTimeOfTransportRequestByID(int TransportRequestID)
        {
            try
            {
                if (TransportRequestID > 0)
                {
                    var dbTransportReq = this.GetTblTransportRequestionWithKey(TransportRequestID);
                    if (dbTransportReq != null)
                    {
                        DateTime SDate = dbTransportReq.TransportDate.Value.AddMinutes(-30);
                        DateTime EDate = dbTransportReq.TransportDate.Value.AddMinutes(30);

                        var dbListTransportReq = this.GetTblTransportDatas.Where(x => x.TransDate.Value >= SDate &&
                                                                                 x.TransDate.Value <= EDate &&
                                                                                 x.CarTypeId == dbTransportReq.CarTypeId &&
                                                                                 x.ContactSource == dbTransportReq.ContactSource &&
                                                                                 x.ContactDestination == dbTransportReq.ContactDestination);

                        return dbListTransportReq.Any() ? dbListTransportReq : null;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Has error {ex.ToString()}");
            }

            return null;
        }

        public IEnumerable<TblTransportData> GetTransportDataViewModelForReportByTrnasportID(int TransportID)
        {
            try
            {
                if (TransportID > 0)
                {
                    return this.GetTblTransportDatas.Where(x => x.TransportId == TransportID).AsEnumerable<TblTransportData>();
                }
            }
            catch(Exception ex)
            {
                Debug.WriteLine($"Has error {ex.ToString()}");
            }
            return null;
        }

        public TblTransportData GetTransportDataByTransportRequestedID(int TransportRequestID)
        {
            try
            {
                if (TransportRequestID > 0)
                {
                    var TransportID = (this.Context.TblRequestHasDataTransport.Where(y => y.TransportRequestId == TransportRequestID)?.FirstOrDefault().TransportId ?? 0);
                    return this.GetTblTransportDatas.Where(x => x.TransportId == TransportID).FirstOrDefault();
                }
            }
            catch(Exception ex)
            {
                Debug.WriteLine($"Has error {ex.ToString()}");
            }

            return null;
        }

        public TblTransportData GetTblTransportDataWithKey(int TransportID)
        {
            try
            {
                if (TransportID > 0)
                    return this.GetTblTransportDatas.Where(x => x.TransportId == TransportID).FirstOrDefault();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Has error " + ex.ToString());
            }

            return null;
        }

        public TblTransportData InsertTblTransportDataToDataBase(TblTransportData nTransport,int TransportRequestedId)
        {
            try
            {
                if (nTransport != null)
                {
                    nTransport.CreateDate = this.DateOfServer;
                    nTransport.Creator = nTransport.Creator ?? "someone";

                    var runNumber = this.Context.TblTransportData.Count(x => x.TransDate.Value.Date == nTransport.TransDate.Value.Date) + 1;
                    var locationID = this.Context.TblContact.Where(x => x.ContactId == nTransport.ContactSource).FirstOrDefault().Location;
                    var location = this.Context.TblLocation.Find(locationID);
                    if (location == null)
                        return null;

                    nTransport.TransportNo = $"T{(nTransport.TransDate.Value.ToString("yyyy-MM-dd"))}/{runNumber.ToString("000")}/{location.LocationCode}";

                    // set driver code
                    if (nTransport.EmployeeDrive == 0)
                    {
                        nTransport.EmployeeDrive = null;
                        nTransport.EmployeeDriveCode = null;
                    }
                    else if (nTransport.EmployeeDrive > 0)
                        nTransport.EmployeeDriveCode = this.Context.TblDriver.Find(nTransport.EmployeeDrive)?.EmployeeDriveCode ?? "";

                    // set emplpyee code
                    if (string.IsNullOrEmpty(nTransport.EmployeeRequestCode))
                        nTransport.EmployeeRequestCode = null;
                    else
                        nTransport.EmployeeRequestCode = this.Context.VEmployee.Find(nTransport.EmployeeRequestCode)?.EmpCode ?? "";

                    // set car null
                    if (nTransport.CarId == 0)
                        nTransport.CarId = null;

                    // set trailte
                    if (nTransport.TrailerId == 0)
                        nTransport.TrailerId = null;

                    this.Context.TblTransportData.Add(nTransport);

                    if (this.Context.SaveChanges() > 0)
                    {
                        if (nTransport.TransportId > 0 && TransportRequestedId > 0)
                            return this.UpdateTransportReqestionSameTransportData(nTransport.TransportId, TransportRequestedId,nTransport.Creator) ?
                                this.GetTblTransportDataWithKey(nTransport.TransportId) : null;
                        else
                            return this.GetTblTransportDataWithKey(nTransport.TransportId);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Has error " + ex.ToString());
            }
            return null;
        }

        public TblTransportData UpdateTblTransportDataToDataBase(int TransportID, TblTransportData uTransport)
        {
            try
            {
                var dbTransport = this.Context.TblTransportData.Find(TransportID);
                if (dbTransport != null)
                {
                    uTransport.ModifyDate = this.DateOfServer;
                    uTransport.CreateDate = dbTransport.CreateDate;
                    uTransport.Modifyer = uTransport.Modifyer ?? "someone";

                    // set driver code
                    if (uTransport.EmployeeDrive == 0)
                    {
                        uTransport.EmployeeDrive = null;
                        uTransport.EmployeeDriveCode = null;
                    }
                    else if (uTransport.EmployeeDrive > 0)
                        uTransport.EmployeeDriveCode = this.Context.TblDriver.Find(uTransport.EmployeeDrive)?.EmployeeDriveCode ?? "";

                    // set emplpyee code
                    if (string.IsNullOrEmpty(uTransport.EmployeeRequestCode))
                        uTransport.EmployeeRequestCode = null;
                    else
                        uTransport.EmployeeRequestCode = this.Context.VEmployee.Find(uTransport.EmployeeRequestCode)?.EmpCode ?? "";

                    // set car null
                    if (uTransport.CarId == 0)
                        uTransport.CarId = null;

                    // set trailte
                    if (uTransport.TrailerId == 0)
                        uTransport.TrailerId = null;

                    this.Context.Entry(dbTransport).CurrentValues.SetValues(uTransport);
                    if (this.Context.SaveChanges() > 0)
                    {
                        // update all transportRequested in transportData
                        foreach(var item in this.Context.TblRequestHasDataTransport.Where(x => x.TransportId == TransportID).ToList())
                            this.UpdateTransportReqestionSameTransportData(item.TransportId, item.TransportRequestId,uTransport.Modifyer);

                        return this.GetTblTransportDataWithKey(TransportID);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Has error " + ex.ToString());
            }
            return null;
        }

        public bool UpdateTransportReqestionSameTransportData(int TransportID,int TransportRequestionID,string Create = "")
        {
            try
            {
                if (TransportID > 0 && TransportRequestionID > 0)
                {
                    if (!this.Context.TblRequestHasDataTransport.Any(x => x.TransportId == TransportID &&
                                                                          x.TransportRequestId == TransportRequestionID))
                    {
                        var nReqHasTransport = new TblRequestHasDataTransport()
                        {
                            CreateDate = this.DateOfServer,
                            Creator = Create ?? "someone",
                            TransportId = TransportID,
                            TransportRequestId = TransportRequestionID
                        };
                        this.Context.TblRequestHasDataTransport.Add(nReqHasTransport);
                    }

                    var dbReqTransport = this.Context.TblTransportRequestion.Find(TransportRequestionID);
                    var dbTransport = this.Context.TblTransportData.Find(TransportID);

                    if (dbReqTransport != null && dbTransport != null)
                    {
                        var stratTime = dbReqTransport.TransportDate.Value.AddMinutes(-5);
                        var endTime = dbReqTransport.TransportDate.Value.AddMinutes(5);

                        if (dbTransport.TransDate >= stratTime && dbTransport.TransDate <= endTime)
                            dbReqTransport.TransportStatus = 3;
                        else
                            dbReqTransport.TransportStatus = 2;

                        //Send mail
                        this.SendMail(TransportRequestionID,TransportID, Create);
                    }


                    return this.Context.SaveChanges() > 0;
                }
            }
            catch(Exception ex)
            {
                Debug.WriteLine($"Has error {ex.ToString()}");
            }

            return false;
        }

        #endregion TblTransportData

        #region TblTransportRequestion

        public IQueryable<TblTransportRequestion> GetTblTransportRequestions =>
                this.Context.TblTransportRequestion.Include(x => x.ContactSourceNavigation.LocationNavigation)
                                                   .Include(x => x.ContactDestinationNavigation.LocationNavigation)
                                                   .Include(x => x.EmployeeRequestCodeNavigation)
                                                   .Include(x => x.CarType)
                                                   .Include(x => x.TblReqHasCompany)
                                                        .ThenInclude(x => x.Company)
                                                   .AsNoTracking()
                                                   .OrderByDescending(x => x.TransportDate).ThenBy(x => x.TransportTime);

        public DataViewModel<TransportRequestionLazyViewModel> GetTransportRequestionViewModelWithLazyLoad(LazyViewModel lazyData)
        {
            try
            {
                var Query = this.GetTblTransportRequestions;

                if (!string.IsNullOrEmpty(lazyData.filter))
                {
                    foreach (var keyword in lazyData.filter.Trim().ToLower().Split(null))
                    {
                        Query = Query.Where(x => x.TransportReqNo.ToLower().Contains(keyword) ||
                                                 x.EmployeeRequestCode.ToLower().Contains(keyword) ||
                                                 x.TransportInformation.ToLower().Contains(keyword) ||
                                                 x.ContactDestinationNavigation.ContactName.ToLower().Contains(keyword) ||
                                                 x.ContactDestinationNavigation.ContactPhone.ToLower().Contains(keyword) ||
                                                 x.ContactDestinationNavigation.LocationNavigation.LocationCode.ToLower().Contains(keyword) ||
                                                 x.ContactDestinationNavigation.LocationNavigation.LocationName.ToLower().Contains(keyword) ||
                                                 x.ContactSourceNavigation.ContactName.ToLower().Contains(keyword) ||
                                                 x.ContactSourceNavigation.ContactPhone.ToLower().Contains(keyword) ||
                                                 x.ContactSourceNavigation.LocationNavigation.LocationCode.ToLower().Contains(keyword) ||
                                                 x.ContactSourceNavigation.LocationNavigation.LocationName.ToLower().Contains(keyword) ||
                                                 x.EmployeeRequestCodeNavigation.EmpCode.ToLower().Contains(keyword) ||
                                                 x.EmployeeRequestCodeNavigation.NameThai.ToLower().Contains(keyword));
                    }
                }

                if (lazyData.option.HasValue)
                {
                    if (lazyData.option.Value != -1)
                    {
                        int CompanyID = lazyData.option.Value;
                        Query = Query.Where(x => x.TblReqHasCompany.Any(z => z.CompanyId == CompanyID));
                    }
                }

                var list = Query.ToList();

                switch (lazyData.sortField)
                {
                    case "TransportDateTime":
                        if (lazyData.sortOrder == -1)
                            Query = Query.OrderByDescending(x => x.TransportDate).ThenByDescending(x => x.TransportTime);
                        else
                            Query = Query.OrderBy(x => x.TransportDate).ThenBy(x => x.TransportTime);
                        break;

                    case "DestinationString":
                        if (lazyData.sortOrder == -1)
                            Query = Query.OrderByDescending(x => x.ContactDestinationNavigation.LocationNavigation.LocationCode);
                        else
                            Query = Query.OrderBy(x => x.ContactDestinationNavigation.LocationNavigation.LocationCode);
                        break;

                    case "EmployeeRequest":
                        if (lazyData.sortOrder == -1)
                            Query = Query.OrderByDescending(x => x.EmployeeRequestCodeNavigation.NameThai);
                        else
                            Query = Query.OrderBy(x => x.EmployeeRequestCodeNavigation.NameThai);
                        break;
                    case "RequestDateTime":
                        if (lazyData.sortOrder == -1)
                            Query = Query.OrderByDescending(x => x.TransportReqDate);
                        else
                            Query = Query.OrderBy(x => x.TransportReqDate);
                        break;
                    case "TransportReqNo":
                        if (lazyData.sortOrder == -1)
                            Query = Query.OrderByDescending(x => x.TransportReqDate);
                        else
                            Query = Query.OrderBy(x => x.TransportReqDate);
                        break;
                    default:
                            Query = Query.OrderByDescending(x => x.TransportReqDate);
                        break;
                }

                var totalRow = Query.Count();
                // select with rownumber
                Query = Query.Skip(lazyData.first ?? 0).Take(lazyData.rows ?? 50);

                var hasData = Query.AsEnumerable<TblTransportRequestion>().Select((x, i) => new TransportRequestionLazyViewModel
                {
                    RowNumber = (i + 1) + lazyData.first ?? 0,
                    TransportRequestId = x.TransportRequestId,
                    DestinationString = (x?.ContactDestinationNavigation?.ContactName ?? "") + " - " + (x?.ContactDestinationNavigation?.LocationNavigation?.LocationCode ?? ""),
                    TransportDateTime = x.TransportDate != null ? (x.TransportDate?.ToString("dd/MM/yy") ?? "") + $" :{x.TransportTime ?? ""}" : "-",
                    RequestDateTime = x.TransportReqDate != null ? (x.TransportReqDate?.ToString("dd/MM/yy") ?? "") : "-",
                    TransportReqNo = x.TransportReqNo,
                    EmployeeRequest = x.EmployeeRequestCodeNavigation?.NameThai ?? "-",
                    // status : 1 : Waitting | 2 : Complete with Change | 3 : Complete | 4 : Cancel
                    Color = x.TransportStatus == 1 ? "gold" : (x.TransportStatus == 2 ? "blue" : (x.TransportStatus == 3 ? "forestgreen" : "red")),
                    Creator = x.Creator,
                    TransportStatus = x.TransportStatus ?? 1
                });

                return new DataViewModel<TransportRequestionLazyViewModel>(hasData, totalRow);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Has error {ex.ToString()}");
            }
            return null;
        }

        public TblTransportRequestion GetTblTransportRequestionWithKey(int TransportReqID)
        {
            try
            {
                if (TransportReqID > 0)
                    return this.GetTblTransportRequestions.Where(x => x.TransportRequestId == TransportReqID).FirstOrDefault();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Has error " + ex.ToString());
            }

            return null;
        }

        public IEnumerable<TblTransportRequestion> GetTblTranspotRequestedWithTransportDataID(int TransportDataID)
        {
            try
            {
                if (TransportDataID > 0)
                {
                    List<int> ListReqId = this.Context.TblRequestHasDataTransport
                                            .Where(x => x.TransportId == TransportDataID)
                                            .Select(x => x.TransportRequestId).ToList<int>();

                    return this.GetTblTransportRequestions.Where(x => ListReqId.Contains(x.TransportRequestId));
                }
            }
            catch(Exception ex)
            {
                Debug.WriteLine($"Has error {ex.ToString()}");
            }

            return null;
        }

        public ScheduleViewModel<IDictionary<String, Object>> GetTransportRequestionScheduleWaiting(bool dateCondition = false)
        {
            try
            {
                var hasData = new List<IDictionary<String, Object>>();
                var transportRequestes = this.GetTblTransportRequestions.Where(x => x.TransportStatus == 1);

                // Date Condition
                if (dateCondition)
                {
                    DateTime curDate = DateTime.Today;
                    transportRequestes = transportRequestes.Where(x => x.TransportDate.Value.Date >= curDate);
                }

                if (transportRequestes.Any())
                {
                    List<string> columnNames = new List<string>() {"Type", "Employee" };

                    foreach (var item in transportRequestes.Where(x => x.TransportDate != null).OrderBy(x => x.TransportDate).GroupBy(x => x.TransportDate.Value.Date)
                                                             .Select(x => x.Key))
                    {
                        columnNames.Add(item.ToString("dd/MM/yy"));
                    }

                    var queryCar = transportRequestes.Where(x => x.CarType.Type == 1);
                    var queryTruck = transportRequestes.Where(x => x.CarType.Type == 2);

                    foreach (var employee in queryCar.GroupBy(x => x.EmployeeRequestCodeNavigation).Select(x => x.Key))
                    {
                        if (employee == null)
                            continue;
                        else
                        {
                            var rowData = new ExpandoObject();
                            var EmployeeReq = employee != null ? $"{(employee?.NameThai ?? "")}" : "No-Data";
                            // add column time
                            ((IDictionary<String, Object>)rowData).Add(columnNames[1], EmployeeReq);
                            foreach (var item in queryCar.Where(x => x.EmployeeRequestCode == employee.EmpCode).OrderBy(x => x.TransportDate))
                            {
                                // if don't have type add item to rowdata
                                if (!((IDictionary<String, Object>)rowData).Keys.Any(x => x == "Type"))
                                {
                                    ((IDictionary<String, Object>)rowData).Add(columnNames[0], item.CarType.Type == 1 ? "Car" : "Truck");
                                }

                                var key = columnNames.Where(y => y.Contains(item.TransportDate.Value.ToString("dd/MM/yy"))).FirstOrDefault();
                                // if don't have data add it to rowData
                                if (!((IDictionary<String, Object>)rowData).Keys.Any(x => x == key))
                                {
                                    ((IDictionary<String, Object>)rowData).Add(key, $"{(item.CarType.Type == 1 ? "For Car" : "For Truck")}#{item.TransportRequestId}");
                                }
                            }

                            hasData.Add(rowData);
                        }
                    }

                    foreach (var employee in queryTruck.GroupBy(x => x.EmployeeRequestCodeNavigation).Select(x => x.Key))
                    {
                        if (employee == null)
                            continue;
                        else
                        {
                            var rowData = new ExpandoObject();
                            var EmployeeReq = employee != null ? $"{(employee?.NameThai ?? "")}" : "No-Data";
                            // add column time
                            ((IDictionary<String, Object>)rowData).Add(columnNames[1], EmployeeReq);
                            foreach (var item in queryTruck.Where(x => x.EmployeeRequestCode == employee.EmpCode).OrderBy(x => x.TransportDate))
                            {
                                // if don't have type add item to rowdata
                                if (!((IDictionary<String, Object>)rowData).Keys.Any(x => x == "Type"))
                                {
                                    ((IDictionary<String, Object>)rowData).Add(columnNames[0], item.CarType.Type == 1 ? "Car" : "Truck");
                                }

                                var key = columnNames.Where(y => y.Contains(item.TransportDate.Value.ToString("dd/MM/yy"))).FirstOrDefault();
                                // if don't have data add it to rowData
                                if (!((IDictionary<String, Object>)rowData).Keys.Any(x => x == key))
                                {
                                    ((IDictionary<String, Object>)rowData).Add(key, $"{(item.CarType.Type == 1 ? "For Car" : "For Truck")}#{item.TransportRequestId}");
                                }
                            }

                            hasData.Add(rowData);
                        }
                    }

                    //return new ScheduleViewModel<ExpandoObject>(hasData.ToList(), columnNames);
                    return new ScheduleViewModel<IDictionary<string, object>>(hasData, columnNames);
                }
            }
            catch (Exception ex)
            {
                Debug.Write($"Has error {ex.ToString()}");
            }

            return null;
        }

        public bool CancelTblTransportRequestionToDataBase(int transportRequestID, string UserName)
        {
            try
            {
                var dbTransportReq = this.Context.TblTransportRequestion.Find(transportRequestID);
                if (dbTransportReq != null)
                {
                    dbTransportReq.TransportStatus = 4;
                    dbTransportReq.Modifyer = UserName ?? "someone";
                    dbTransportReq.ModifyDate = this.DateOfServer;

                    if (this.Context.SaveChanges() > 0)
                    {
                        this.SendMail(transportRequestID, 0, UserName);
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Has error " + ex.ToString());
            }
            return false;
        }

        public TblTransportRequestion InsertTblTransportRequestionToDataBase(TblTransportRequestion nTranReq)
        {
            try
            {
                if (nTranReq != null)
                {
                    nTranReq.CreateDate = this.DateOfServer;
                    nTranReq.TransportReqDate = this.DateOfServer;
                    nTranReq.Creator = nTranReq.Creator ?? "someone";
                    nTranReq.TransportStatus = 1;

                    // Gen code
                    var runNumber = this.Context.TblTransportRequestion.Count(x => x.TransportDate.Value.Date == nTranReq.TransportDate.Value.Date) + 1;
                    var locationID = this.Context.TblContact.Where(x => x.ContactId == nTranReq.ContactSource).FirstOrDefault().Location;
                    var location = this.Context.TblLocation.Find(locationID);
                    if (location == null)
                        return null;

                    nTranReq.TransportReqNo = $"R{(nTranReq.TransportDate.Value.ToString("yyyy-MM-dd"))}/{runNumber.ToString("000")}/{location.LocationCode}";


                    if (nTranReq.TransportTypeId != null)
                        nTranReq.TransportType = Context.TblTransportType.Find(nTranReq.TransportTypeId)?.TransportTypeDesc ?? "";

                    if (nTranReq.CarTypeId == 0)
                        nTranReq.CarTypeId = null;

                    if (string.IsNullOrEmpty(nTranReq.EmployeeRequestCode))
                        nTranReq.EmployeeRequestCode = null;
                    else
                        nTranReq.EmployeeRequestCode = this.Context.VEmployee.Find(nTranReq.EmployeeRequestCode)?.EmpCode ?? "";

                    this.Context.TblTransportRequestion.Add(nTranReq);
                    return this.Context.SaveChanges() > 0 ? this.GetTblTransportRequestionWithKey(nTranReq.TransportRequestId) : null;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Has error " + ex.ToString());
            }
            return null;
        }

        public TblTransportRequestion UpdateTblTransportRequestionToDataBase(int TransportReqID, TblTransportRequestion uTranReq)
        {
            try
            {
                var dbTranReq = this.Context.TblTransportRequestion.Find(TransportReqID);
                if (dbTranReq != null)
                {
                    uTranReq.ModifyDate = this.DateOfServer;
                    uTranReq.Modifyer = uTranReq.Modifyer ?? "someone";

                    uTranReq.CreateDate = dbTranReq.CreateDate;
                    uTranReq.TransportReqDate = dbTranReq.TransportReqDate;

                    uTranReq.TransportStatus = uTranReq.TransportStatus ?? 1;

                    if (uTranReq.TransportTypeId != null)
                        uTranReq.TransportType = Context.TblTransportType.Find(uTranReq.TransportTypeId)?.TransportTypeDesc ?? "";

                    if (uTranReq.CarTypeId == 0)
                        uTranReq.CarTypeId = null;

                    if (string.IsNullOrEmpty(uTranReq.EmployeeRequestCode))
                        uTranReq.EmployeeRequestCode = null;
                    else
                        uTranReq.EmployeeRequestCode = this.Context.VEmployee.Find(uTranReq.EmployeeRequestCode)?.EmpCode ?? "";

                    this.Context.Entry(dbTranReq).CurrentValues.SetValues(uTranReq);
                    return this.Context.SaveChanges() > 0 ? this.GetTblTransportRequestionWithKey(TransportReqID) : null;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Has error " + ex.ToString());
            }
            return null;
        }

        #endregion TblTransportRequestion

        #region TblTransportType

        public IEnumerable<TblTransportType> GetTblTransportTypes => this.Context.TblTransportType.OrderBy(x => x.TransportTypeId);

        #endregion TblTransportType

        #region TblAttachFile

        public IEnumerable<TblAttachFile> GetTblAttachFilesByTransportRequestionID(int TransportReqID)
        {
            try
            {
                if (TransportReqID > 0)
                {
                   var listAttach = this.Context.TblTransportRequestHasAttach
                                                .Where(x => x.TransportRequestId == TransportReqID)
                                                .Select(x => x.AttachId).ToList();

                    if (listAttach.Any())
                        return this.Context.TblAttachFile.Where(x => listAttach.Contains(x.AttachId))
                                                         .AsNoTracking().AsEnumerable<TblAttachFile>();
                }
            }
            catch(Exception ex)
            {
                Debug.WriteLine($"Has error {ex.ToString()}");
            }
            return null;
        }

        public bool InsertTblAttachFileToDataBase(int TransportReqID,TblAttachFile nAttachFile)
        {
            try
            {
                if (nAttachFile != null)
                {
                    nAttachFile.CreateDate = this.DateOfServer;
                    nAttachFile.Creator = nAttachFile.Creator ?? "someone";

                    this.Context.TblAttachFile.Add(nAttachFile);

                    if (this.Context.SaveChanges() > 0)
                    {
                        var nTrasnsportReq = new TblTransportRequestHasAttach()
                        {
                            AttachId = nAttachFile.AttachId,
                            CreateDate = this.DateOfServer,
                            Creator = nAttachFile.Creator,
                            TransportRequestId = TransportReqID
                        };

                        this.Context.TblTransportRequestHasAttach.Add(nTrasnsportReq);
                        return this.Context.SaveChanges() > 0;
                    }
                }
            }
            catch(Exception ex)
            {
                Debug.WriteLine($"Has error {ex.ToString()}");
            }

            return false;
        }

        public TblAttachFile GetTblAttachFileByAttachID(int AttachID)
        {
            try
            {
                if (AttachID > 0)
                {
                    var AttachModel = this.Context.TblAttachFile.Where(x => x.AttachId == AttachID).FirstOrDefault();
                    return AttachModel;
                }
            }
            catch(Exception ex)
            {
                Debug.WriteLine($"Has error {ex.ToString()}");
            }
            return null;
        }

        public bool RemoveTblTransportRequestHasAttachFromDataBase(TblAttachFile attachFile)
        {
            try
            {
                var dbTransReqHasAtt = this.Context.TblTransportRequestHasAttach.Where(x => x.AttachId == attachFile.AttachId).FirstOrDefault();
                if (dbTransReqHasAtt != null)
                    this.Context.TblTransportRequestHasAttach.Remove(dbTransReqHasAtt);

                var dbAttachFile = this.Context.TblAttachFile.Find(attachFile.AttachId);
                if (dbAttachFile != null)
                    this.Context.TblAttachFile.Remove(dbAttachFile);

                return this.Context.SaveChanges() > 0;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"has error\r\n{ex.ToString()}");
            }

            return false;
        }

        #endregion

        #region Notification-Email
        private bool SendMail(int TransportRequestedId,int TransportDataId,string UserName)
        {
            try
            {
                var transportRequestedData = this.GetTblTransportRequestionWithKey(TransportRequestedId);
                var userData = this.Context.TblUser.Where(x => x.Username == UserName).Include(x => x.EmployeeCodeNavigation).FirstOrDefault();

                if (transportRequestedData != null && userData != null)
                {
                    // Check Mail address From and to
                    if (string.IsNullOrEmpty(userData.MailAddress) || string.IsNullOrEmpty(transportRequestedData.EmailResponse))
                        return false;

                    string[] listEmail;
                    transportRequestedData.EmailResponse = transportRequestedData.EmailResponse.TrimEnd();

                    if (transportRequestedData.EmailResponse.IndexOf(',') > -1)
                        listEmail = transportRequestedData.EmailResponse.Split(',');
                    else if (transportRequestedData.EmailResponse.IndexOf('/') > -1)
                        listEmail = transportRequestedData.EmailResponse.Split('/');
                    else if (transportRequestedData.EmailResponse.IndexOf(' ') > -1)
                        listEmail = transportRequestedData.EmailResponse.Split(' ');
                    else if (transportRequestedData.EmailResponse.IndexOf(';') > -1)
                        listEmail = transportRequestedData.EmailResponse.Split(';');
                    else
                        listEmail = transportRequestedData.EmailResponse.Split(null);

                    var message = new MimeMessage();
                    message.From.Add(new MailboxAddress($"คุณ{userData.EmployeeCodeNavigation.NameThai}", userData.MailAddress));
                    foreach(var email in listEmail)
                    {
                        message.To.Add(new MailboxAddress(email));
                    }
                    message.Subject = "Notification mail from Transportation System.";

                    BodyBuilder builder;

                    if (transportRequestedData.TransportStatus == 4)
                    {
                        builder = new BodyBuilder
                        {
                            HtmlBody = "<body style=font-size:11pt;font-family:Tahoma>" +
                                    "<h3 style='color:steelblue;'>เมล์ฉบับนี้เป็นแจ้งเตือนจากระบบงานขนส่ง</h3>" +
                                    $"เรียน คุณ{transportRequestedData?.EmployeeRequestCodeNavigation?.NameThai ?? "-"}" +
                                    $"<p>ตามที่ได้มีการร้องขอใช้งานระบบงานขนส่ง เอกสารเลขที่ <i>{ transportRequestedData?.TransportReqNo ?? "-" }</i> เมื่อวันที่ {transportRequestedData?.TransportReqDate.Value.ToString("dd-MM-yyyy เวลา HH:mm") ?? "-"}<br>" +
                                    $"เดินทางไปยัง \"{transportRequestedData?.ContactDestinationNavigation?.LocationNavigation?.LocationName ?? "-"}\" วันที่ {transportRequestedData?.TransportDate.Value.ToString("dd-MM-yyyy เวลา HH:mm") ?? "-"} " +
                                    $"โดยรถยนต์ประเภท \"{transportRequestedData?.CarType?.CarTypeDesc ?? "-"}\"</p>" +
                                    $"<p style='color:red;'>ขณะนี้ทางหน่วยงานขนส่งได้ดำเนินการ ยกเลิกคำขอใช้งานดังกล่าว หากมีข้อสงสัยโปรดติดต่อ หน่วยงานขนส่ง </p>" +
                                    "<span style='color:steelblue;'>This mail auto generated by Transportation system of VIPCO.</span>" +
                                    "</body>",
                        };
                    }
                    else
                    {
                        var transportData = this.GetTblTransportDataWithKey(TransportDataId);
                        if (transportData == null)
                            return false;

                        builder = new BodyBuilder
                        {
                            HtmlBody = "<body style=font-size:11pt;font-family:Tahoma>" +
                                    "<h3 style='color:steelblue;'>เมล์ฉบับนี้เป็นแจ้งเตือนจากระบบงานขนส่ง</h3>" +
                                    $"เรียน คุณ{transportRequestedData?.EmployeeRequestCodeNavigation?.NameThai ?? "-"}" +
                                    $"<p>ตามที่ได้มีการร้องขอใช้งานระบบงานขนส่ง เอกสารเลขที่ <i>{ transportRequestedData?.TransportReqNo ?? "-" }</i> เมื่อวันที่ {transportRequestedData?.TransportReqDate.Value.ToString("dd-MM-yyyy เวลา HH:mm") ?? "-"}<br>" +
                                    $"เดินทางไปยัง \"{transportRequestedData?.ContactDestinationNavigation?.LocationNavigation?.LocationName ?? "-"}\" วันที่ {transportRequestedData?.TransportDate.Value.ToString("dd-MM-yyyy เวลา HH:mm") ?? "-"} " +
                                    $"โดยรถยนต์ประเภท \"{transportRequestedData?.CarType?.CarTypeDesc ?? "-"}\"</p>" +
                                    $"<p>ขณะนี้ทางหน่วยงานขนส่งได้ดำเนินการเรียบร้อย โดยมีเลขที่เอกสารคือ <i>{transportData?.TransportNo ?? "-"}</i><br>" +
                                    $"คุณสามารถเข้าไปตรวจสอบข้อมูลได้ที่ <a href='http://192.168.2.162:803/transport-data-report/{transportData.TransportId}'>ที่นี้</a></p><br>" +
                                    "<span style='color:steelblue;'>This mail auto generated by Transportation system of VIPCO.</span>" +
                                    "</body>",
                        };
                    }

                    //Fetch the attachments from db
                    //considering one or more attachments
                    //byte[] attachment = System.IO.File.ReadAllBytes(this.appEnvironment.WebRootPath + "\\Files\\MyPDF.pdf");
                    //builder.Attachments.Add("MyPDF.pdf", attachment, ContentType.Parse("application/pdf"));

                    message.Body = builder.ToMessageBody();

                    using (var client = new SmtpClient())
                    {
                        // For demo-purposes, accept all SSL certificates (in case the server supports STARTTLS)
                        //client.ServerCertificateValidationCallback = (s, c, h, e) => true;
                        //client.Connect("mail.vipco-thai.com", 25, SecureSocketOptions.Auto);
                        client.Connect("mail.vipco-thai.com", 25, SecureSocketOptions.None);

                        // Note: since we don't have an OAuth2 token, disable
                        // the XOAUTH2 authentication mechanism.
                        //client.AuthenticationMechanisms.Remove("XOAUTH2");

                        // Note: only needed if the SMTP server requires authentication
                        // client.Authenticate(userData.MailAddress, userData.MailPassword);

                        client.Send(message);
                        client.Disconnect(true);

                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Has error {ex.ToString()}");
            }

            return false;

        }

        #endregion
    }
}