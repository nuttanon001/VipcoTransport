using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;

using Newtonsoft.Json;
using AutoMapper;

using VipcoTransport.Models;
using VipcoTransport.ViewModels;
using VipcoTransport.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using System.Linq.Expressions;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace VipcoTransport.Controllers
{
    [Route("api/[controller]")]
    public class EmployeeController : Controller
    {
        #region PrivateMenbers

        readonly IEmployeeRepository repository;
        readonly IRepository<TblDriver> repositoryDriver;
        readonly IRepository<TblDriverHasCompany> repositoryDriverCompany;
        readonly IHostingEnvironment appEnvironment;
        readonly IMapper mapper;
        private JsonSerializerSettings DefaultJsonSettings =>
            new JsonSerializerSettings()
            {
                Formatting = Formatting.Indented,
                PreserveReferencesHandling = PreserveReferencesHandling.None,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
        private List<DriverViewModel> ToDriverViewModelList(IEnumerable<TblDriver> drivers)
        {
            var listData = new List<DriverViewModel>();
            int RowNumber = 1;
            foreach (var item in drivers)
            {
                listData.Add(this.mapper.Map<TblDriver, DriverViewModel>(item, new DriverViewModel(RowNumber)));
                RowNumber++;
            }
            return listData;
        }
        private List<ContactViewModel> ToContactViewModelList(IEnumerable<TblContact> contacts)
        {
            var listData = new List<ContactViewModel>();
            int RowNumber = 1;
            foreach (var item in contacts)
            {
                listData.Add(this.mapper.Map<TblContact, ContactViewModel>(item, new ContactViewModel(RowNumber)));
                RowNumber++;
            }
            return listData;
        }
        private List<LocationViewModel> ToLocationViewModelList(IEnumerable<TblLocation> locations)
        {
            var listData = new List<LocationViewModel>();
            int RowNumber = 1;
            foreach (var item in locations)
            {
                listData.Add(this.mapper.Map<TblLocation, LocationViewModel>(item, new LocationViewModel(RowNumber)));
                RowNumber++;
            }
            return listData;
        }
        private List<UserViewModel> ToUserViewModelList(IEnumerable<TblUser> users)
        {
            var listData = new List<UserViewModel>();

            foreach (var item in users)
                listData.Add(this.mapper.Map<TblUser, UserViewModel>(item, new UserViewModel(item)));

            return listData;
        }
        #endregion

        #region Constructor
        public EmployeeController(
            IEmployeeRepository repo, IHostingEnvironment appEnv,
            IRepository<TblDriver> repo2nd, IRepository<TblDriverHasCompany> repo3rd,
            IMapper map)
        {
            this.repository = repo;
            this.repositoryDriver = repo2nd;
            this.repositoryDriverCompany = repo3rd;
            this.appEnvironment = appEnv;
            this.mapper = map;
        }
        #endregion

        #region TblEmployee

        [HttpPost("GetEmployeeWithLazyLoad")]
        public IActionResult GetEmployeeWithLazyLoadFromDataBase([FromBody] LazyViewModel lazyData)
        {
            if (lazyData != null)
                return new JsonResult(this.repository.GetEmployeeViewModelWithLazyLoad(lazyData), this.DefaultJsonSettings);
            return NotFound(new { Error = "lazy data has not been found" });
        }

        [HttpPost]
        public IActionResult PostTblEmployeeToDataBase([FromBody] TblEmployee nEmployee)
        {
            if (nEmployee != null)
            {
                var hasData = this.repository.InsertTblEmployeeToDataBase(nEmployee);
                if (hasData != null)
                    return new JsonResult(hasData, DefaultJsonSettings);
            }
            return new StatusCodeResult(500);
        }

        [HttpPut("{key}")]
        public IActionResult PutTblEmployeeToDataBase(int key, [FromBody] TblEmployee uEmploye)
        {
            if (uEmploye != null)
            {
                var hasData = this.repository.UpdateTblEmployeeToDataBase(key, uEmploye);
                if (hasData != null)
                    return new JsonResult(hasData, DefaultJsonSettings);
            }
            return new StatusCodeResult(500);
        }
        #endregion

        #region TblDriver

        [HttpGet("GetDriver")]
        [HttpGet("GetDriver/{key}")]
        public IActionResult GetTblDriverFromDataBase(int key = 0)
        {
            if (key < 1)
                return new JsonResult(this.repository.GetTblDrivers, this.DefaultJsonSettings);
            else
            {
                var HasData = this.repository.GetTblDriverWithKey(key);
                //this.repositoryDriver.GetAsync(key).Result;
                return new JsonResult(this.mapper.Map<TblDriver,DriverViewModel2>
                    (HasData)
                    , this.DefaultJsonSettings);
            }
        }

        [HttpGet("GetDrivers")]
        public IActionResult GetDriversViewModelFromDataBase()
        {
            var hasData = new List<DriverViewModel>();
            int i = 0;
            foreach (var item in this.repository.GetTblDrivers)
                hasData.Add(this.mapper.Map<TblDriver, DriverViewModel>(item,new DriverViewModel(i++)));

            return new JsonResult(hasData, this.DefaultJsonSettings);
        }

        [HttpGet("GetDriverFilter")]
        [HttpGet("GetDriverFilter/{filter}")]
        public IActionResult GetTblDriverWithFilterFromDataBase(string filter = "")
        {
            var hasData = this.repository.GetTblDriverWithFilter(filter);
            return new JsonResult(new DataViewModel<DriverViewModel>(this.ToDriverViewModelList(hasData.Data), hasData.TotalRecordCount), this.DefaultJsonSettings);
        }

        [HttpPost("PostDriver")]
        public IActionResult PostTblDriverToDataBase([FromBody] DriverViewModel2 nDriver)
        {
            if (nDriver != null)
            {
                // Update to company
                int? CompanyId = null;
                if (nDriver.CompanyID.HasValue)
                    CompanyId = nDriver.CompanyID.Value;

                var hasData = this.repository
                    .InsertTblDriverToDataBase(this.mapper.Map<DriverViewModel2,TblDriver>(nDriver));
                if (hasData != null)
                {
                    if (CompanyId.HasValue)
                    {
                        this.repositoryDriverCompany.Add(new TblDriverHasCompany()
                        {
                            CompanyId = CompanyId,
                            CreateDate = DateTime.Now,
                            Creator = hasData.Creator,
                            EmployeeDriveId = hasData.EmployeeDriveId,
                        });
                    }
                    return new JsonResult(hasData, DefaultJsonSettings);
                }
            }
            return new StatusCodeResult(500);
        }

        [HttpPut("PutDriver/{key}")]
        public IActionResult PutTblDriverToDataBase(int key, [FromBody] DriverViewModel2 uDriver)
        {
            if (uDriver != null)
            {
                var hasData = this.repository.UpdateTblDriverToDataBase(key,
                    this.mapper.Map<DriverViewModel2, TblDriver>(uDriver));
                // Update to company
                int? CompanyId = null;
                if (uDriver.CompanyID.HasValue)
                    CompanyId = uDriver.CompanyID.Value;

                if (hasData != null)
                {
                    if (CompanyId.HasValue)
                    {
                        Expression<Func<TblDriverHasCompany, bool>> condition = c => c.EmployeeDriveId == hasData.EmployeeDriveId;
                        var hasDriveCompany = this.repositoryDriverCompany.Find(condition);
                        if (hasDriveCompany != null)
                        {
                            hasDriveCompany.CompanyId = CompanyId;
                            hasDriveCompany.Modifyer = hasData.Modifyer;
                            hasDriveCompany.ModifyDate = hasData.ModifyDate;
                            // update to table car has company
                            this.repositoryDriverCompany.Update(hasDriveCompany, hasDriveCompany.DriverHasCompanyId);
                        }
                        else
                        {
                            this.repositoryDriverCompany.Add(new TblDriverHasCompany()
                            {
                                CompanyId = CompanyId,
                                CreateDate = DateTime.Now,
                                Creator = hasData.Creator,
                                EmployeeDriveId = hasData.EmployeeDriveId,
                            });
                        }
                    }
                    return new JsonResult(hasData, DefaultJsonSettings);
                }
            }
            return new StatusCodeResult(500);
        }
        #endregion

        #region TblLocation

        [HttpGet("GetLocation")]
        [HttpGet("GetLocation/{key}")]
        public IActionResult GetTblLocationFromDataBase(int key = 0)
        {
            if (key < 1)
                return new JsonResult(this.repository.GetTblLocations, this.DefaultJsonSettings);
            else
                return new JsonResult(this.repository.GetTblLocationWithKey(key), this.DefaultJsonSettings);
        }

        [HttpGet("GetLocationWithFilter")]
        [HttpGet("GetLocationWithFilter/{filter}")]
        public IActionResult GetTblLocationWithFilterFromDataBase(string filter = "")
        {
            var hasData = this.repository.GetTblLocationWithFilter(filter);
            return new JsonResult(new DataViewModel<LocationViewModel>(this.ToLocationViewModelList(hasData.Data), hasData.TotalRecordCount), this.DefaultJsonSettings);
        }

        [HttpPost("PostLocation")]
        public IActionResult PostTblLocationToDataBase([FromBody] TblLocation nLocation)
        {
            if (nLocation != null)
            {
                var hasData = this.repository.InsertTblLocationToDataBase(nLocation);
                if (hasData != null)
                    return new JsonResult(hasData, DefaultJsonSettings);
            }
            return new StatusCodeResult(500);
        }

        [HttpPut("PutLocation/{key}")]
        public IActionResult PutTblLocationToDataBase(int key, [FromBody] TblLocation uLocation)
        {
            if (uLocation != null)
            {
                var hasData = this.repository.UpdateTblLocationToDataBase(key, uLocation);
                if (hasData != null)
                    return new JsonResult(hasData, DefaultJsonSettings);
            }
            return new StatusCodeResult(500);
        }
        #endregion

        #region TblContact

        [HttpGet("GetTblContact")]
        [HttpGet("GetTblContact/{key}")]
        public IActionResult GetTblContactFromDataBase(int key = 0)
        {
            if (key < 1)
                return new JsonResult(this.repository.GetTblContacts, this.DefaultJsonSettings);
            else
            {
                var hasData = this.mapper.Map<TblContact, ContactViewModel>(this.repository.GetTblContactWithKey(key), new ContactViewModel(0));
                return new JsonResult(hasData, this.DefaultJsonSettings);
            }
        }

        [HttpGet("GetTblContacts")]
        public IActionResult GetContactsViewModelFromDataBase()
        {
            var hasData = new List<ContactViewModel>();
            int i = 0;
            foreach (var item in this.repository.GetTblContacts)
                hasData.Add(this.mapper.Map<TblContact, ContactViewModel>(item, new ContactViewModel(i++)));

            return new JsonResult(hasData, this.DefaultJsonSettings);
        }

        [HttpGet("GetTblContactWithFilter")]
        [HttpGet("GetTblContactWithFilter/{filter}")]
        public IActionResult GetTblContactWithFilterFromDataBase(string filter = "")
        {
            var hasData = this.repository.GetTblContactWithFilter(filter);
            return new JsonResult(new DataViewModel<ContactViewModel>(this.ToContactViewModelList(hasData.Data), hasData.TotalRecordCount), this.DefaultJsonSettings);
        }

        [HttpPost("PostContact")]
        public IActionResult PostTblContactToDataBase([FromBody] TblContact nContact)
        {
            if (nContact != null)
            {
                var hasData = this.repository.InsertTblContactToDataBase(nContact);
                if (hasData != null)
                    return new JsonResult(hasData, DefaultJsonSettings);
            }
            return new StatusCodeResult(500);
        }

        [HttpPost("PostContactOnlyLocation")]
        public IActionResult PostTblContactOnlyLocation([FromBody] TblLocation nContactOnlyLocation)
        {
            if (nContactOnlyLocation != null)
            {
                var hasData = this.repository.InsertTblContactOnlyLocationToDataBase(nContactOnlyLocation);
                if (hasData != null)
                    return new JsonResult(hasData, DefaultJsonSettings);
            }
            return new StatusCodeResult(500);
        }

        [HttpPut("PutContact/{key}")]
        public IActionResult PutTblContactToDataBase(int key, [FromBody] TblContact uContact)
        {
            if (uContact != null)
            {
                var hasData = this.repository.UpdateTblContactToDataBase(key, uContact);
                if (hasData != null)
                    return new JsonResult(hasData, DefaultJsonSettings);
            }
            return new StatusCodeResult(500);
        }
        #endregion

        #region VEmployee

        [HttpGet]
        [HttpGet("{key}")]
        public IActionResult GetVEmployeeFromDataBase(string key = "")
        {
            if (string.IsNullOrEmpty(key))
                return new JsonResult(this.repository.GetVEmployees, this.DefaultJsonSettings);
            else
                return new JsonResult(this.repository.GetVEmployeeWithKey(key), this.DefaultJsonSettings);
        }

        [HttpGet("GetEmployeeByUserName/{UserName}")]
        public IActionResult GetVEmployeeByUserNameFromDataBase(string UserName)
        {
            if (!string.IsNullOrEmpty(UserName))
            {
                var hasdata = this.repository.GetVEmployeeByUserName(UserName);
                return new JsonResult(hasdata, this.DefaultJsonSettings);
            }

            else
                return NotFound(new { Error = "Employee id has not been found" }); ;

        }

        #endregion

        #region TblUser

        [HttpGet("GetUser2")]
        [HttpGet("GetUser2/{key}")]
        public IActionResult GetTblUserFromDataBase(int key = 0)
        {
            if (key < 1)
            {
                var hasData = this.repository.GetTblUserWithFilter("");
                return new JsonResult(new DataViewModel<UserViewModel>(this.ToUserViewModelList(hasData.Data), hasData.TotalRecordCount), this.DefaultJsonSettings);
            }
            else
            {
                var hasData = this.repository.GetTblUserWithKey(key);
                return new JsonResult(this.mapper.Map<TblUser, UserViewModel>(hasData, new UserViewModel(hasData)), this.DefaultJsonSettings);
            }
        }

        [HttpGet("GetUserWithFilter")]
        [HttpGet("GetUserWithFilter/{filter}")]
        public IActionResult GetTblUserWithFilterFromDataBase(string filter = "")
        {
            var hasData = this.repository.GetTblUserWithFilter(filter);
            return new JsonResult(new DataViewModel<UserViewModel>(this.ToUserViewModelList(hasData.Data), hasData.TotalRecordCount), this.DefaultJsonSettings);
        }

        [HttpPost("PostUser")]
        public IActionResult PostTblUserToDataBase([FromBody] TblUser nUser)
        {
            if (nUser != null)
            {
                var hasData = this.repository.InsertTblUserToDataBase(nUser);
                if (hasData != null)
                    return new JsonResult(this.mapper.Map<TblUser, UserViewModel>(hasData, new UserViewModel(hasData)), this.DefaultJsonSettings);
            }
            return new StatusCodeResult(500);
        }

        [HttpPut("PutUser/{key}")]
        public IActionResult PutTblUserToDataBase(int key, [FromBody] TblUser uUser)
        {
            if (uUser != null)
            {
                var hasData = this.repository.UpdateTblUserToDataBase(key, uUser);
                if (hasData != null) { }
                    return new JsonResult(this.mapper.Map<TblUser, UserViewModel>(hasData, new UserViewModel(hasData)), this.DefaultJsonSettings);
            }
            return new StatusCodeResult(500);
        }

        #region Don't user
        //[HttpGet("GetUser/{username}")]
        //[Authorize(Policy = "SystemUser",Roles = "Administrator")]
        //public IActionResult GetTblUserNameFromDataBase1(string username)
        //{
        //    if (!string.IsNullOrEmpty(username))
        //    {
        //        var hasData = this.repository.GetTblUserByUserName(username);
        //        return new JsonResult(new UserViewModel(hasData), this.DefaultJsonSettings);
        //    }

        //    return NotFound(new { Error = "User name has not been found" }); ;
        //}
        #endregion

        [HttpGet("GetUser/{username}")]
        [Authorize(Policy = "SystemUser")]
        public IActionResult GetTblUserNameFromDataBase(string username)
        {
            if (!string.IsNullOrEmpty(username))
            {
                var hasData = this.repository.GetTblUserByUserName(username);
                return new JsonResult(this.mapper.Map<TblUser, UserViewModel>(hasData, new UserViewModel(hasData)), this.DefaultJsonSettings);
            }

            return NotFound(new { Error = "User name has not been found" });
        }
    }
    #endregion
}

