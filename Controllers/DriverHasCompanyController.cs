using AutoMapper;
using Newtonsoft.Json;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;


using VipcoTransport.Models;
using VipcoTransport.ViewModels;
using VipcoTransport.Services.Interfaces;
using System.Linq.Expressions;

namespace VipcoTransport.Controllers
{
    [Produces("application/json")]
    [Route("api/DriverHasCompany")]
    public class DriverHasCompanyController : Controller
    {

        #region PrivateMenbers

        private readonly IRepository<TblDriverHasCompany> repository;
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

        public DriverHasCompanyController(IRepository<TblDriverHasCompany> repo, IMapper map)
        {
            this.repository = repo;
            this.mapper = map;
        }

        #endregion

        // GET: api/DriverHasCompany
        [HttpGet]
        public IActionResult Get()
        {
            return new JsonResult(this.repository.GetAllAsync().Result, this.DefaultJsonSettings);
        }

        // GET: api/DriverHasCompany/5
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            return new JsonResult(this.repository.GetAsync(id).Result, this.DefaultJsonSettings);
        }
        [HttpGet("Find/{id}")]
        public IActionResult GetFind(int id)
        {
            // filter
            Expression<Func<TblDriverHasCompany, bool>> condition = m => m.EmployeeDriveId == id;
            return new JsonResult(this.repository.FindAsync(condition), this.DefaultJsonSettings);
        }

        // POST: api/DriverHasCompany
        [HttpPost]
        public IActionResult Post([FromBody]TblDriverHasCompany nDriverHasCompany)
        {
            return new JsonResult(this.repository.AddAsync(nDriverHasCompany).Result, this.DefaultJsonSettings);
        }

        // PUT: api/DriverHasCompany/5
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody]TblDriverHasCompany uDriverHasCompany)
        {
            return new JsonResult(this.repository.UpdateAsync(uDriverHasCompany, id).Result, this.DefaultJsonSettings);
        }

        // DELETE: api/DriverHasCompany/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            return new JsonResult(this.repository.DeleteAsync(id).Result, this.DefaultJsonSettings);
        }
    }
}
