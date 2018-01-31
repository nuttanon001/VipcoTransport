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
    public class TransportRequestRepository: ITransportRequestRepository
    {
        #region PrivateMembers
        private DbSet<TblTransportRequestion> entities;
        private ApplicationContext Context;
        string errorMessage = string.Empty;
        #endregion

        #region Constructor
        /// <summary>
        /// The contructor requires an open DataContext to work with
        /// </summary>
        /// <param name="context">An open DataContext</param>

        public TransportRequestRepository(ApplicationContext context)
        {
            this.Context = context;
            entities = context.Set<TblTransportRequestion>();
        }
        #endregion

        public ScheduleViewModel<IDictionary<String, Object>> RequestionWaiting(ConditionViewModel condition)
        {
            try
            {
                var ResultData = new List<IDictionary<String, Object>>();
                var ReqWait = this.Context.TblTransportRequestion
                                            .Where(x => x.TransportStatus == 1)
                                            .Include(x => x.CarType)
                                            .Include(x => x.EmployeeRequestCodeNavigation)
                                            .AsQueryable();
                // Data Company
                if (condition.CompanyID.HasValue)
                {
                    if (condition.CompanyID.Value != -1)
                    {
                        ReqWait = ReqWait.Where(x => x.TblReqHasCompany
                                                      .Any(z => z.CompanyId == condition.CompanyID.Value));
                    }
                }
                // Date Condition
                if (condition.ConditionBool.HasValue)
                {
                    if (condition.ConditionBool.Value)
                    {
                        DateTime curDate = DateTime.Today;
                        ReqWait = ReqWait.Where(x => x.TransportDate.Value.Date >= curDate);
                    }
                }
                // Get Data
                if (ReqWait.Any())
                {
                    // Set column name
                    List<string> columnNames = new List<string>() { "Type", "Employee" };

                    // Set column name of date
                    foreach (var item in ReqWait.Where(x => x.TransportDate != null)
                                                .OrderBy(x => x.TransportDate).GroupBy(x => x.TransportDate.Value.Date)
                                                .Select(x => x.Key))
                    {
                        columnNames.Add(item.ToString("dd/MM/yy"));
                    }

                    // Type of Car
                    var queryCar = ReqWait.Where(x => x.CarType.Type == 1);
                    foreach (var employee in queryCar.GroupBy(x => x.EmployeeRequestCodeNavigation).Select(x => x.Key))
                    {
                        if (employee == null)
                            continue;
                        else
                        {
                            var rowData = new ExpandoObject();
                            var EmployeeReq = employee != null ? $"คุณ {(employee?.NameThai ?? "")}" : "No-Data";
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
                                    ((IDictionary<String, Object>)rowData)
                                        .Add(key, $"Total 1 / {(item.CarType.Type == 1 ? "For Car" : "For Truck")}#{item.TransportRequestId}");
                                }
                                else
                                {
                                    var getData = (string)((IDictionary<String, Object>)rowData)[key];
                                    var split = getData.Split('/');
                                    if (split.Any())
                                    {
                                        if (int.TryParse(split[0].Replace("Total ",""),out int result))
                                            ((IDictionary<String, Object>)rowData)[key] = $"Total {result + 1} / {split[1]}";
                                    }
                                }
                            }
                            ResultData.Add(rowData);
                        }
                    }

                    // Type of Truck
                    var queryTruck = ReqWait.Where(x => x.CarType.Type == 2);
                    foreach (var employee in queryTruck.GroupBy(x => x.EmployeeRequestCodeNavigation).Select(x => x.Key))
                    {
                        if (employee == null)
                            continue;
                        else
                        {
                            var rowData = new ExpandoObject();
                            var EmployeeReq = employee != null ? $"คุณ {(employee?.NameThai ?? "")}" : "No-Data";
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
                                    ((IDictionary<String, Object>)rowData).
                                        Add(key, $"Total 1 / {(item.CarType.Type == 1 ? "For Car" : "For Truck")}#{item.TransportRequestId}");
                                }
                                else
                                {
                                    var getData = (string)((IDictionary<String, Object>)rowData)[key];
                                    var split = getData.Split('/');
                                    if (split.Any())
                                    {
                                        if (int.TryParse(split[0].Replace("Total ", ""), out int result))
                                            ((IDictionary<String, Object>)rowData)[key] = $"{result + 1}/{split[1]}";
                                    }
                                }
                            }
                            ResultData.Add(rowData);
                        }
                    }

                    return new ScheduleViewModel<IDictionary<string, object>>(ResultData, columnNames);
                }
            }
            catch (Exception ex)
            {
                Debug.Write($"Has error {ex.ToString()}");
            }

            return null;
        }
    }
}
