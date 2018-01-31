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
    [Route("api/TranHasCompany")]
    public class TranHasCompanyController : Controller
    {
        #region PrivateMenbers

        private readonly IRepository<TblTranHasCompany> repository;
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

        public TranHasCompanyController(IRepository<TblTranHasCompany> repo, IMapper map)
        {
            this.repository = repo;
            this.mapper = map;
        }

        #endregion

        // GET: api/TranHasCompany
        [HttpGet]
        public IActionResult Get()
        {
            return new JsonResult(this.repository.GetAllAsync().Result, this.DefaultJsonSettings);
        }

        // GET: api/TranHasCompany/5
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            return new JsonResult(this.repository.GetAsync(id).Result, this.DefaultJsonSettings);
        }
        [HttpGet("Find/{id}")]
        public IActionResult GetFind(int id)
        {
            // filter
            Expression<Func<TblTranHasCompany, bool>> condition = m => m.TransportId == id;
            return new JsonResult(this.repository.FindAsync(condition), this.DefaultJsonSettings);
        }
        // POST: api/TranHasCompany
        [HttpPost]
        public IActionResult Post([FromBody]TblTranHasCompany nTranHasCompany)
        {
            return new JsonResult(this.repository.AddAsync(nTranHasCompany).Result, this.DefaultJsonSettings);
        }

        // PUT: api/TranHasCompany/5
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody]TblTranHasCompany uTranHasCompany)
        {
            return new JsonResult(this.repository.UpdateAsync(uTranHasCompany, id).Result, this.DefaultJsonSettings);
        }

        // DELETE: api/TranHasCompany/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            return new JsonResult(this.repository.DeleteAsync(id).Result, this.DefaultJsonSettings);
        }
    }
}
