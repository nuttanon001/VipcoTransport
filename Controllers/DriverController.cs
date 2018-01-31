using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Newtonsoft.Json;

using System;
using System.Linq;
using System.Linq.Expressions;
using System.Collections.Generic;

using VipcoTransport.Models;
using VipcoTransport.ViewModels;
using VipcoTransport.Services.Interfaces;

namespace VipcoTransport.Controllers
{
    [Produces("application/json")]
    [Route("api/Driver")]
    public class DriverController : Controller
    {
        #region PrivateMenbers

        private readonly IRepository<TblDriver> repository;
        private readonly IMapper mapper;

        private JsonSerializerSettings DefaultJsonSettings =>
            new JsonSerializerSettings()
            {
                Formatting = Formatting.Indented,
                PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };

        private List<MapType> ConverterMap<MapType, TableType>(ICollection<TableType> tables)
        {
            var listData = new List<MapType>();
            foreach (var item in tables)
                listData.Add(this.mapper.Map<TableType, MapType>(item));
            return listData;
        }

        #endregion PrivateMenbers

        #region Constructor

        public DriverController(IRepository<TblDriver> repo, IMapper map)
        {
            this.repository = repo;
            this.mapper = map;
        }

        #endregion Constructor

        // POST: api/Driver/GetWithLazyLoad
        [HttpPost("FindAllLayzLoad")]
        public IActionResult GetWithLazyLoad([FromBody]LazyViewModel LazyLoad)
        {
            var Query = this.repository.GetAll()
                .Include(x => x.EmployeeDriveCodeNavigation)
                .Include(x => x.TblDriverHasCompany).AsQueryable();

            // Filter
            var filters = string.IsNullOrEmpty(LazyLoad.filter) ? new string[] { "" }
                    : LazyLoad.filter.ToLower().Split(null);

            foreach(var keyword in filters)
            {
                Query = Query.Where(x => x.EmployeeDriveCode.Contains(keyword) ||
                                         x.PhoneNumber.Contains(keyword) ||
                                         x.EmployeeDriveCodeNavigation.NameThai.ToLower().Contains(keyword) ||
                                         x.EmployeeDriveCodeNavigation.NameEng.ToLower().Contains(keyword));
            }

            // Option
            if (LazyLoad.option.HasValue)
            {
                if (LazyLoad.option.Value != -1)
                {
                    Query = Query.Where(x => x.TblDriverHasCompany.Any(z => z.CompanyId == LazyLoad.option.Value));
                }
            }

            // Order
            switch (LazyLoad.sortField)
            {
                case "EmployeeDriveCode":
                    if (LazyLoad.sortOrder == -1)
                        Query.OrderByDescending(e => e.EmployeeDriveCode);
                    else
                        Query.OrderBy(e => e.EmployeeDriveCode);
                    break;

                case "EmployeeName":
                    if (LazyLoad.sortOrder == -1)
                        Query.OrderByDescending(e => e.EmployeeDriveCodeNavigation.NameThai);
                    else
                        Query.OrderBy(e => e.EmployeeDriveCodeNavigation.NameThai);
                    break;
                default:
                    Query.OrderBy(e => e.EmployeeDriveCode);
                    break;
            }
            int count = Query.Count();
            // Skip and Take
            Query = Query.Skip(LazyLoad.first ?? 0).Take(LazyLoad.rows ?? 25);

            return new JsonResult(new
            {
                Data = this.ConverterMap<DriverViewModel2, TblDriver>(Query.ToList()),
                TotalRow = count
            }, this.DefaultJsonSettings);
        }
    }
}