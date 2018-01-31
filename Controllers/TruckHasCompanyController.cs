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
    [Route("api/TruckHasCompany")]
    public class TruckHasCompanyController : Controller
    {
        #region PrivateMenbers

        private readonly IRepository<TblTruckHasCompany> repository;
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

        public TruckHasCompanyController(IRepository<TblTruckHasCompany> repo, IMapper map)
        {
            this.repository = repo;
            this.mapper = map;
        }

        #endregion

        // GET: api/TruckHasCompany
        [HttpGet]
        public IActionResult Get()
        {
            return new JsonResult(this.repository.GetAllAsync().Result, this.DefaultJsonSettings);
        }

        // GET: api/TruckHasCompany/5
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            return new JsonResult(this.repository.GetAsync(id).Result, this.DefaultJsonSettings);
        }
        [HttpGet("Find/{id}")]
        public IActionResult GetFind(int id)
        {
            // filter
            Expression<Func<TblTruckHasCompany, bool>> condition = m => m.TrailerId == id;
            return new JsonResult(this.repository.FindAsync(condition), this.DefaultJsonSettings);
        }
        // POST: api/TruckHasCompany
        [HttpPost]
        public IActionResult Post([FromBody]TblTruckHasCompany nTruckHasCompany)
        {
            return new JsonResult(this.repository.AddAsync(nTruckHasCompany).Result, this.DefaultJsonSettings);
        }

        // PUT: api/TruckHasCompany/5
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody]TblTruckHasCompany uTruckHasCompany)
        {
            return new JsonResult(this.repository.UpdateAsync(uTruckHasCompany, id).Result, this.DefaultJsonSettings);
        }

        // DELETE: api/TruckHasCompany/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            return new JsonResult(this.repository.DeleteAsync(id).Result, this.DefaultJsonSettings);
        }
    }
}
