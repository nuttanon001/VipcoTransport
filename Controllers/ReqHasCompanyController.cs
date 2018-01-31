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
    [Route("api/ReqHasCompany")]
    public class ReqHasCompanyController : Controller
    {
        #region PrivateMenbers

        private readonly IRepository<TblReqHasCompany> repository;
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

        public ReqHasCompanyController(IRepository<TblReqHasCompany> repo, IMapper map)
        {
            this.repository = repo;
            this.mapper = map;
        }

        #endregion

        // GET: api/ReqHasCompany
        [HttpGet]
        public IActionResult Get()
        {
            return new JsonResult(this.repository.GetAllAsync().Result, this.DefaultJsonSettings);
        }

        // GET: api/ReqHasCompany/5
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            return new JsonResult(this.repository.GetAsync(id).Result, this.DefaultJsonSettings);
        }

        [HttpGet("Find/{id}")]
        public IActionResult GetFind(int id)
        {
            // filter
            Expression<Func<TblReqHasCompany, bool>> condition = m => m.TransportRequestId == id;
            return new JsonResult(this.repository.FindAsync(condition), this.DefaultJsonSettings);
        }
        // POST: api/ReqHasCompany
        [HttpPost]
        public IActionResult Post([FromBody]TblReqHasCompany nReqHasCompany)
        {
            return new JsonResult(this.repository.AddAsync(nReqHasCompany).Result, this.DefaultJsonSettings);
        }

        // PUT: api/ReqHasCompany/5
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody]TblReqHasCompany uReqHasCompany)
        {
            return new JsonResult(this.repository.UpdateAsync(uReqHasCompany, id).Result, this.DefaultJsonSettings);
        }

        // DELETE: api/ReqHasCompany/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            return new JsonResult(this.repository.DeleteAsync(id).Result, this.DefaultJsonSettings);
        }
    }
}
