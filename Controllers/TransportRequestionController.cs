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
    [Route("api/TransportRequestion")]
    public class TransportRequestionController : Controller
    {
        #region PrivateMenbers

        private readonly IRepository<TblTransportRequestion> repository;
        private readonly ITransportRequestRepository repositoryTransportReq;
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

        public TransportRequestionController(
            IRepository<TblTransportRequestion> repo,
            ITransportRequestRepository repoTranReq,
            IMapper map)
        {
            this.repository = repo;
            this.repositoryTransportReq = repoTranReq;
            this.mapper = map;
        }

        #endregion Constructor

        #region TranspotRequestWaiting

        [HttpPost("GetTranspotRequestWaiting")]
        public IActionResult GetTranspotRequestWaitingFromDataBase([FromBody] ConditionViewModel condition)
        {
            var hasData = this.repositoryTransportReq.RequestionWaiting(condition);
            if (hasData != null)
                return new JsonResult(hasData, this.DefaultJsonSettings);

            return NotFound(new { Error = "transport data this week has not been found" });
        }

        #endregion
    }
}
