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
    [Route("api/UserEdition")]
    public class UserEditionController : Controller
    {
        #region PrivateMenbers

        private readonly IRepository<TblUser> repositoryUser;
        private readonly IRepository<VEmployee> repositoryEmployee;
        private readonly IRepository<TblUserHasCompany> repositoryUserHasCom;
        private readonly IRepository<TblCompany> repositoryCom;
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

        public UserEditionController(
            IRepository<TblUser> repo,
            IRepository<VEmployee> repo2nd,
            IRepository<TblUserHasCompany> repo3rd,
            IRepository<TblCompany> repo4th,
            IMapper map)
        {
            this.repositoryUser = repo;
            this.repositoryEmployee = repo2nd;
            this.repositoryUserHasCom = repo3rd;
            this.repositoryCom = repo4th;
            this.mapper = map;
        }

        #endregion

        [HttpGet("Find/{id}")]
        public IActionResult GetFind(int id)
        {
            var GetUser = this.repositoryUser.Get(id);
            if (GetUser != null)
            {
                Expression<Func<TblUserHasCompany, bool>> condition = m => m.UserId == GetUser.UserId;

                var UserHasCompany = this.repositoryUserHasCom.Find(condition);
                var Employee = this.repositoryEmployee.Get(GetUser.EmployeeCode);
                var User = this.repositoryCom.Get(GetUser.CompanyId.Value);

                var UserEdition = new UserEditionViewModel()
                {
                    CompanyID = GetUser.CompanyId ?? 2,
                    CompanyName = GetUser.CompanyId.HasValue ? User.CompanyName : "-",
                    EmployeeName = Employee != null ? Employee.NameThai : "-",
                    MailAddress = GetUser.MailAddress,
                    PasswordOld = GetUser.Password,
                    PasswordConfirm = "",
                    PasswordNew = "",
                    UserId = GetUser.UserId,
                    UserName = GetUser.Username,
                    EmailAlert = UserHasCompany != null ? (UserHasCompany.EmailAlert ?? false) : false,
                    UserHasCompanyId = UserHasCompany != null ? UserHasCompany.UserHasCompanyId : 0
                };

                return new JsonResult(UserEdition, this.DefaultJsonSettings);
            }
            return NotFound(new { Error = "User name has not been found" });
        }

        [HttpPost]
        public IActionResult PostUserEdition([FromBody]UserEditionViewModel userEdition)
        {
            string exception = "";
            try
            {
                if (userEdition != null)
                {
                    var updateUser = this.repositoryUser.Get(userEdition.UserId);
                    if (updateUser != null)
                    {
                        if (!string.IsNullOrEmpty(userEdition.PasswordConfirm))
                            updateUser.Password = userEdition.PasswordConfirm;

                        updateUser.MailAddress = userEdition.MailAddress;
                        updateUser.ModifyDate = DateTime.Now;
                        updateUser.Modifyer = userEdition.UserName;

                        this.repositoryUser.Update(updateUser, updateUser.UserId);
                    }
                    if (userEdition.UserHasCompanyId > 0)
                    {
                        var updateUserHasCom = this.repositoryUserHasCom.Get(userEdition.UserHasCompanyId);
                        if (updateUserHasCom != null)
                        {
                            updateUserHasCom.ModifyDate = DateTime.Now;
                            updateUserHasCom.Modifyer = userEdition.UserName;
                            updateUserHasCom.EmailAlert = userEdition.EmailAlert;

                            this.repositoryUserHasCom.Update(updateUserHasCom, userEdition.UserHasCompanyId);
                        }
                        else
                        {
                            var newUserHasCom = new TblUserHasCompany()
                            {
                                CompanyId = userEdition.CompanyID,
                                CreateDate = DateTime.Now,
                                Creator = userEdition.UserName,
                                EmailAlert = userEdition.EmailAlert,
                                UserId = userEdition.UserId
                            };
                            this.repositoryUserHasCom.Add(newUserHasCom);
                        }
                    }
                    else
                    {
                        var newUserHasCom = new TblUserHasCompany()
                        {
                            CompanyId = userEdition.CompanyID,
                            CreateDate = DateTime.Now,
                            Creator = userEdition.UserName,
                            EmailAlert = userEdition.EmailAlert,
                            UserId = userEdition.UserId
                        };
                        this.repositoryUserHasCom.Add(newUserHasCom);
                    }
                    return new JsonResult(userEdition, this.DefaultJsonSettings);
                }
            }
            catch(Exception ex)
            {
                exception = ex.ToString();
            }
            return NotFound(new { Error = exception });
        }
    }
}
