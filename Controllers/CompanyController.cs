using AutoMapper;
using Newtonsoft.Json;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.Collections.Generic;

using VipcoTransport.Models;
using VipcoTransport.ViewModels;
using VipcoTransport.Services.Interfaces;

namespace VipcoTransport.Controllers
{
    [Produces("application/json")]
    [Route("api/Company")]
    public class CompanyController : Controller
    {
        #region PrivateMenbers

        private readonly IRepository<TblCompany> repository;
        private readonly IMapper mapper;

        private JsonSerializerSettings DefaultJsonSettings =>
            new JsonSerializerSettings()
            {
                Formatting = Formatting.Indented,
                PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };

        #endregion PrivateMenbers

        #region Constructor

        public CompanyController(IRepository<TblCompany> repo, IMapper map)
        {
            this.repository = repo;
            this.mapper = map;
        }

        #endregion

        // GET: api/Company
        [HttpGet]
        public IActionResult Get()
        {
            return new JsonResult(this.repository.GetAllAsync().Result, this.DefaultJsonSettings);
        }

        // GET: api/Company/5
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            return new JsonResult(this.repository.GetAsync(id).Result, this.DefaultJsonSettings);
        }

        // POST: api/Company/GetWithLazyLoad
        [HttpPost("FindAllLayzLoad")]
        public IActionResult GetWithLazyLoad([FromBody]LazyViewModel LazyLoad)
        {
            // Relate
            // Filter
            var filters = string.IsNullOrEmpty(LazyLoad.filter) ? new string[] { "" }
                    : LazyLoad.filter.ToLower().Split(null);

            Expression<Func<TblCompany, bool>> condition = e =>
              filters.Any(x => (e.Address1).ToLower().Contains(x) ||
                               (e.Address2).ToLower().Contains(x) ||
                               (e.CompanyCode).ToLower().Contains(x) ||
                               (e.CompanyName).ToLower().Contains(x) ||
                               (e.Telephone).ToLower().Contains(x));
            // Order
            Expression<Func<TblCompany, string>> Order = null;
            Expression<Func<TblCompany, string>> OrderDesc = null;

            switch (LazyLoad.sortField)
            {
                case "CompanyCode":
                    if (LazyLoad.sortOrder == -1)
                        OrderDesc = e => e.CompanyCode;
                    else
                        Order = e => e.CompanyCode;
                    break;
                case "CompanyName":
                    if (LazyLoad.sortOrder == -1)
                        OrderDesc = e => e.CompanyName;
                    else
                        Order = e => e.CompanyName;
                    break;
                case "Address1":
                    if (LazyLoad.sortOrder == -1)
                        OrderDesc = e => e.Address1;
                    else
                        Order = e => e.Address1;
                    break;
                case "Address2":
                    if (LazyLoad.sortOrder == -1)
                        OrderDesc = e => e.Address2;
                    else
                        Order = e => e.Address2;
                    break;
                default:
                    Order = e => e.CompanyCode;
                    break;
            }

            return new JsonResult(new
            {
                Data = this.repository.FindAllWithLazyLoadAsync(condition, null, LazyLoad.first ?? 0, LazyLoad.rows ?? 25,
                       Order, OrderDesc).Result,
                TotalRow = this.repository.CountWithMatch(condition)
            }, this.DefaultJsonSettings);
        }

        // POST: api/Company
        [HttpPost]
        public IActionResult Post([FromBody]TblCompany nCompany)
        {
            return new JsonResult(this.repository.AddAsync(nCompany).Result, this.DefaultJsonSettings);
        }

        // PUT: api/Company/5
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody]TblCompany uCompany)
        {
            return new JsonResult(this.repository.UpdateAsync(uCompany, id).Result, this.DefaultJsonSettings);
        }

        // DELETE: api/Company/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            return new JsonResult(this.repository.DeleteAsync(id).Result, this.DefaultJsonSettings);
        }
    }
}
