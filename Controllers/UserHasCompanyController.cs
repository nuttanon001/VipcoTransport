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
    [Route("api/UserHasCompany")]
    public class UserHasCompanyController : Controller
    {
        #region PrivateMenbers

        private readonly IRepository<TblUserHasCompany> repository;
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

        public UserHasCompanyController(IRepository<TblUserHasCompany> repo, IMapper map)
        {
            this.repository = repo;
            this.mapper = map;
        }

        #endregion

        // GET: api/CarHasCompany
        [HttpGet]
        public IActionResult Get()
        {
            return new JsonResult(this.repository.GetAllAsync().Result, this.DefaultJsonSettings);
        }

        // GET: api/CarHasCompany/5
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            return new JsonResult(this.repository.GetAsync(id).Result, this.DefaultJsonSettings);
        }

        [HttpGet("Find/{id}")]
        public IActionResult GetFind(int id)
        {
            // filter
            Expression<Func<TblUserHasCompany, bool>> condition = m => m.UserId == id;
            return new JsonResult(this.repository.FindAsync(condition), this.DefaultJsonSettings);
        }


        // POST: api/CarHasCompany
        [HttpPost]
        public IActionResult Post([FromBody]TblUserHasCompany nUserHCom)
        {
            nUserHCom.CreateDate = DateTime.Now;
            nUserHCom.Creator = nUserHCom.Creator ?? "Someone";

            return new JsonResult(this.repository.AddAsync(nUserHCom).Result, this.DefaultJsonSettings);
        }

        // PUT: api/CarHasCompany/5
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody]TblUserHasCompany uUserHCom)
        {
            uUserHCom.ModifyDate = DateTime.Now;
            uUserHCom.Modifyer = uUserHCom.Modifyer ?? "Someone";

            return new JsonResult(this.repository.UpdateAsync(uUserHCom, id).Result, this.DefaultJsonSettings);
        }

        // DELETE: api/CarHasCompany/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            return new JsonResult(this.repository.DeleteAsync(id).Result, this.DefaultJsonSettings);
        }
    }
}
