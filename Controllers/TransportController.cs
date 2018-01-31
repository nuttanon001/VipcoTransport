using AutoMapper;
using Newtonsoft.Json;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;

using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.Collections.Generic;

using VipcoTransport.Models;
using VipcoTransport.ViewModels;
using VipcoTransport.Services.Interfaces;
using System.Net.Http;
using ReportClasses;

using MimeKit;
using MailKit.Security;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.NodeServices;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace VipcoTransport.Controllers
{
    [Route("api/[controller]")]
    public class TransportController : Controller
    {
        #region PrivateMenbers

        private readonly ITransportRepository repository;
        private readonly IRepository<TblReqHasCompany> repositoryReqCompany;
        private readonly IRepository<TblTranHasCompany> repositoryTranCompany;
        private readonly IRepository<TblUserHasCompany> repositoryUserCompany;
        private readonly IRepository<TblUser> repositoryUser;
        private readonly IHostingEnvironment appEnvironment;
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

        public TransportController(
            ITransportRepository repo,
            IRepository<TblReqHasCompany> repo2nd,
            IRepository<TblTranHasCompany> repo3rd,
            IRepository<TblUserHasCompany> repo4th,
            IRepository<TblUser> repo5th,
            IHostingEnvironment appEnv,
            IMapper map)
        {
            this.repository = repo;
            this.repositoryReqCompany = repo2nd;
            this.repositoryTranCompany = repo3rd;
            this.repositoryUserCompany = repo4th;
            this.repositoryUser = repo5th;
            this.appEnvironment = appEnv;
            this.mapper = map;
        }

        #endregion Constructor

        #region TblTransportData

        #region GetMethod
        [HttpGet]
        [HttpGet("{key}")]
        public IActionResult GetTblTransportDataFromDataBase(int key = 0)
        {
            if (key < 1)
                return new JsonResult(this.repository.GetTblTransportDatas, this.DefaultJsonSettings);
            else
            {
                var hasData = this.mapper.Map<TblTransportData, TransportData2ViewModel>(this.repository.GetTblTransportDataWithKey(key));
                return new JsonResult(hasData, this.DefaultJsonSettings);
            }
        }

        [HttpGet("GetTransportDataSameTransportReq/{TransportReqID}")]
        public IActionResult GetTblTransportDataSameTransportRequestionFromDataBase(int TransportReqID)
        {
            if (TransportReqID > 0)
            {
                List<TransportDataViewModel> sendData = new List<TransportDataViewModel>();
                var hasData = this.repository.GetTransportDataSameTimeOfTransportRequestByID(TransportReqID);
                if (hasData != null)
                {
                    foreach (var item in hasData)
                        sendData.Add(this.mapper.Map<TblTransportData, TransportDataViewModel>(item));

                    return new JsonResult(sendData, this.DefaultJsonSettings);
                }
            }

            return NotFound(new { Error = " data for this transport request" });
        }

        [HttpGet("GetTransportDataByTransportRequested/{TransportReqID}")]
        public IActionResult GetTblTransprotDataByTransportRequestedIDFromDataBase(int TransportReqID)
        {
            if (TransportReqID > 0)
            {
                var hasData = this.repository.GetTransportDataByTransportRequestedID(TransportReqID);
                if (hasData != null)
                    return new JsonResult(this.mapper.Map<TblTransportData, TransportDataViewModel>(hasData), this.DefaultJsonSettings);
            }
            return NotFound(new { Error = "TransportRequested ID is request." });
        }
        #endregion

        #region PostMethod
        [HttpPost("GetTransportWithLazyLoad")]
        public IActionResult GetTransportDataWithLazyLoadFromDataBase([FromBody] LazyViewModel lazyData)
        {
            if (lazyData != null)
                return new JsonResult(this.repository.GetTransportViewModelWithLazyLoad(lazyData), this.DefaultJsonSettings);
            return NotFound(new { Error = "lazy data has not been found" });
        }

        [HttpPost("GetTransportTruckWithLazyLoad")]
        public IActionResult GetTransportTruckDataWithLazyLoadFromDataBase([FromBody] LazyViewModel lazyData)
        {
            if (lazyData != null)
                return new JsonResult(this.repository.GetTransportTruckViewModelWithLazyLoad(lazyData), this.DefaultJsonSettings);
            return NotFound(new { Error = "lazy data has not been found" });
        }

        #region Schedule

        [HttpPost("GetTranspotDailySchedule")]
        public IActionResult GetTransportDailyScheduleFromDataBase([FromBody] string pickDate)
        {
            if (!string.IsNullOrEmpty(pickDate))
            {
                DateTime dt = new DateTime();
                if (!DateTime.TryParseExact(pickDate,
                                       "dd/MM/yyyy",
                                       System.Globalization.CultureInfo.InvariantCulture,
                                       System.Globalization.DateTimeStyles.None,
                                       out dt))
                {
                    dt = DateTime.Today;
                }


                var hasData = this.repository.GetTransportScheduleByDaily(dt);
                if (hasData != null)
                    return new JsonResult(hasData, this.DefaultJsonSettings);
            }
            return NotFound(new { Error = "transport data this day has not been found" });
        }

        [HttpPost("GetTranspotWeeklySchedule")]
        public IActionResult GetTransportWeeklyScheduleFromDataBase([FromBody] ScheduleWeeklyViewModel scheduleWeekly)
        {
            if (scheduleWeekly != null)
            {
                if (scheduleWeekly.StartDate2 != null && scheduleWeekly.EndDate2 != null)
                {
                    var hasData = this.repository.GetTransportScheduleByWeeklyV2(scheduleWeekly);
                    if (hasData != null)
                        return new JsonResult(hasData, this.DefaultJsonSettings);
                }
            }
            return NotFound(new { Error = "transport data this week has not been found" });
        }

        [HttpPost("GetTransportDailyWithTransportID")]
        public IActionResult GetTransportDailyWithTransportFromDataBase([FromBody] string stringData)
        {
            if (!string.IsNullOrEmpty(stringData))
            {
                var hasData = this.repository.GetTransportScheduleDailyWithTransportData(stringData);
                if (hasData != null)
                    return new JsonResult(hasData, this.DefaultJsonSettings);
            }

            return NotFound(new { Error = "data of transport is request" });
        }
        #endregion

        [HttpPost("{TransportReqID}")]
        public IActionResult PostTblTransportDataToDataBase(int TransportReqID,[FromBody] TransportData2ViewModel nTransport)
        {
            if (nTransport != null)
            {
                if ((nTransport?.RoutineTransport ?? false) && nTransport?.RoutineCount > 0 && nTransport?.RoutineDay > 0)
                {
                    TblTransportData lastData = null;
                    var dateCheck = nTransport.TransDate.Value;
                    System.TimeSpan time = System.TimeSpan.Parse(nTransport.TransTime);

                    int Counter = 1;

                    while (dateCheck <= nTransport.TransDate.Value.AddDays((nTransport?.RoutineDay ?? 0) * (nTransport?.RoutineCount ?? 0)))
                    {
                        var routineData = this.mapper.Map<TransportData2ViewModel, TblTransportData>(nTransport);
                        routineData.TransDate = new System.DateTime(dateCheck.Year, dateCheck.Month, dateCheck.Day, time.Hours, time.Minutes, 0);

                        routineData.TransportNo = nTransport.TransportNo + "/R" + Counter.ToString("00");

                        lastData = this.repository.InsertTblTransportDataToDataBase(routineData, TransportReqID);
                        dateCheck = dateCheck.AddDays(nTransport?.RoutineDay ?? 0);
                        Counter++;
                    }
                    if (lastData != null)
                        return new JsonResult(lastData, DefaultJsonSettings);
                }
                else
                {
                    if (nTransport.TransDate != null)
                    {
                        var tempDate = nTransport.TransDate.Value;
                        System.TimeSpan time = System.TimeSpan.Parse(nTransport.TransTime);

                        nTransport.TransDate = new System.DateTime(tempDate.Year, tempDate.Month, tempDate.Day, time.Hours, time.Minutes, 0);
                    }

                    if (nTransport.FinalDate != null)
                    {
                        var tempDate2 = nTransport.FinalDate.Value;
                        var time2 = System.TimeSpan.Parse(nTransport.FinalTime);

                        nTransport.FinalDate = new System.DateTime(tempDate2.Year, tempDate2.Month, tempDate2.Day, time2.Hours, time2.Minutes, 0);
                    }

                    // Update to company
                    int? CompanyId = null;
                    if (nTransport.CompanyID.HasValue)
                        CompanyId = nTransport.CompanyID.Value;

                    var hasData = this.repository.InsertTblTransportDataToDataBase(
                        this.mapper.Map<TransportData2ViewModel, TblTransportData>(nTransport), TransportReqID);

                    if (hasData != null)
                    {
                        if (CompanyId.HasValue)
                        {
                            this.repositoryTranCompany.Add(new TblTranHasCompany()
                            {
                                CompanyId = CompanyId,
                                CreateDate = DateTime.Now,
                                Creator = hasData.Creator,
                                TransportId = hasData.TransportId
                            });
                        }
                        return new JsonResult(hasData, DefaultJsonSettings);
                    }
                }
            }
            return new StatusCodeResult(500);
        }
        #endregion

        #region PutMethod
        // put tblTransportData to database
        [HttpPut("{key}")]
        public IActionResult PutTblTransportDataToDataBase(int key, [FromBody] TransportData2ViewModel uTransport)
        {
            if (uTransport != null)
            {
                //if (!string.IsNullOrEmpty(uTransport.StringTransDate))
                //{
                //    DateTime dt = new DateTime();
                //    DateTime.TryParseExact(uTransport.StringTransDate,
                //                           "dd/MM/yyyy",
                //                           System.Globalization.CultureInfo.InvariantCulture,
                //                           System.Globalization.DateTimeStyles.None,
                //                           out dt);
                //    uTransport.TransDate = dt;
                //}

                if (uTransport.TransDate != null)
                {
                    var tempDate = uTransport.TransDate.Value;
                    System.TimeSpan time = System.TimeSpan.Parse(uTransport.TransTime);

                    uTransport.TransDate = new System.DateTime(tempDate.Year, tempDate.Month, tempDate.Day, time.Hours, time.Minutes, 0);
                }

                if (uTransport.FinalDate != null)
                {
                    var tempDate2 = uTransport.FinalDate.Value;
                    var time2 = System.TimeSpan.Parse(uTransport.FinalTime);

                    uTransport.FinalDate = new System.DateTime(tempDate2.Year, tempDate2.Month, tempDate2.Day, time2.Hours, time2.Minutes, 0);
                }

                // Update to company
                int? CompanyId = null;
                if (uTransport.CompanyID.HasValue)
                    CompanyId = uTransport.CompanyID.Value;

                var hasData = this.repository.UpdateTblTransportDataToDataBase(key, uTransport);
                if (hasData != null)
                {
                    if (CompanyId.HasValue)
                    {
                        Expression<Func<TblTranHasCompany, bool>> condition = c => c.TransportId == hasData.TransportId;
                        var hasTranHasCompany = this.repositoryTranCompany.Find(condition);
                        if (hasTranHasCompany != null)
                        {
                            hasTranHasCompany.CompanyId = CompanyId;
                            hasTranHasCompany.Modifyer = hasData.Modifyer;
                            hasTranHasCompany.ModifyDate = hasData.ModifyDate;
                            // update to table car has company
                            this.repositoryTranCompany.Update(hasTranHasCompany, hasTranHasCompany.TranHasCompanyId);
                        }
                        else
                        {
                            this.repositoryTranCompany.Add(new TblTranHasCompany()
                            {
                                CompanyId = CompanyId,
                                CreateDate = DateTime.Now,
                                Creator = hasData.Creator,
                                TransportId = hasData.TransportId
                            });
                        }
                    }

                    return new JsonResult(hasData, DefaultJsonSettings);
                }
            }
            return new StatusCodeResult(500);
        }

        // put tblTransportRequestId and tblTransportDataId condition TransportData many TransportRequest
        [HttpPut("PutUpdateTransportReqSameTransportData/{TransportID}/{TransportReqID}")]
        public IActionResult PutUpdateTransportReqSameTransportData(int TransportID, int TransportReqID,[FromBody] UserViewModel user)
        {
            if (TransportID > 0 && TransportReqID > 0)
            {
                var hasData = this.repository.UpdateTransportReqestionSameTransportData(TransportID, TransportReqID,user?.Username ?? "");
                if (hasData)
                    return new StatusCodeResult(200);
            }
            return new StatusCodeResult(500);
        }

        #endregion

        #region ReportMethod

        [HttpGet("GetReportTransportation/{TransportId}")]
        public IActionResult GetReportTransportation(int TransportId)
        {
            try
            {
                var hasData = this.mapper.Map<TblTransportData, TransportDataViewModel>(this.repository.GetTblTransportDataWithKey(TransportId));
                var onePage = new OnePage()
                {
                    TemplateFolder = this.appEnvironment.WebRootPath + "\\reports\\"
                };//general class for work

                var stream = onePage.Export<TransportDataViewModel>(hasData, "ReportTransport");

                stream.Seek(0, SeekOrigin.Begin);
                // "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Reports.xlsx"
                // "application/vnd.openxmlformats-officedocument.wordprocessingml.document", "Reports.docx"
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Reports.xlsx");
            }
            catch (Exception ex)
            {
                return NotFound(new { Error = "Transportation report not found " + ex.ToString() });
            }
        }

        [HttpGet("GetReportTransportationPdf/{TransportId}")]
        public async Task<IActionResult> Index(int TransportId ,[FromServices] INodeServices nodeServices)
        {
            var hasData = this.mapper.Map<TblTransportData, TransportData2ViewModel>(this.repository.GetTblTransportDataWithKey(TransportId));

            HttpClient hc = new HttpClient();
            var htmlContent = await hc.GetStringAsync($"http://{Request.Host}/reports/report.html");

            //Class to Json
            var Json = JsonConvert.SerializeObject(hasData,this.DefaultJsonSettings);

            var result = await nodeServices.InvokeAsync<byte[]>("./JS/exportpdf", htmlContent, Json);
            return File(result, "application/pdf", "report.pdf");

            //return new ContentResult();
        }
        #endregion

        #endregion TblTransportData

        #region TblTransportRequestion

        private string AlertToHighLevel(int CompanyID,string From)
        {
            string status = "";
            try
            {
                var FromUser = this.repositoryUser.GetAll()
                                    .Include(x => x.EmployeeCodeNavigation)
                                    .SingleOrDefault(x => x.Username == From);

                if (FromUser != null)
                {
                    if (!string.IsNullOrEmpty(FromUser.MailAddress))
                    {
                        var MailAddress = this.repositoryUserCompany.GetAll()
                                .Include(x => x.User)
                                .Where(x => x.CompanyId == CompanyID && x.EmailAlert.Value)
                                .Select(x => x.User)
                                .Where(z => !string.IsNullOrEmpty(z.MailAddress) && z.Role == 2)
                                .Select(x => x.MailAddress).Distinct().ToList();

                        if (MailAddress.Any())
                        {
                            var builder = new BodyBuilder
                            {
                                HtmlBody = "<body style=font-size:11pt;font-family:Tahoma>" +
                                        "<h3 style='color:steelblue;'>เมล์ฉบับนี้เป็นแจ้งเตือนจากระบบงานขนส่ง</h3>" +
                                        $"เรียน ผู้ดูแลระบบขนส่ง" +
                                        $"<p>ณ.ขณะนี้ระบบได้ตรวจพบการขอใช้งานระบบขนส่งจากทาง" +
                                        $"\"คุณ{FromUser.EmployeeCodeNavigation.NameThai}\" "+
                                        $"คุณสามารถเข้าไปทำการดำเนินการได้ <a href='http://192.168.2.162:803/login/'>ที่นี้</a> </p>" +
                                        "<span style='color:steelblue;'>This mail auto generated by Transportation system of VIPCO.</span>" +
                                        "</body>",
                            };

                            if (this.SendMail(MailAddress, builder,
                                    FromUser.EmployeeCodeNavigation.NameThai, FromUser.MailAddress))
                                status = "Complate";
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                status = ex.ToString();
            }

            return status;
        }

        #region Method Get

        [HttpGet("GetTransportReq")]
        [HttpGet("GetTransportReq/{key}")]
        public IActionResult GetTblTransportRequestionFromDataBase(int key = 0)
        {
            if (key < 1)
                return new JsonResult(this.repository.GetTblTransportRequestions, this.DefaultJsonSettings);
            else
            {
                var hasData = this.mapper.Map<TblTransportRequestion, TransportRequestion2ViewModel>(this.repository.GetTblTransportRequestionWithKey(key));
                return new JsonResult(hasData, this.DefaultJsonSettings);
            }
        }

        [HttpGet("GetTransportReqAttachFile/{TransportReqID}")]
        public IActionResult GetTblAttactFileByTransportRequestionIDFromDataBase(int TransportReqID)
        {
            if (TransportReqID > 0)
            {
                var hasData = this.repository.GetTblAttachFilesByTransportRequestionID(TransportReqID);
                return new JsonResult(hasData, this.DefaultJsonSettings);
            }

            return NotFound(new { Error = "transport requestion id is req" });
        }

        [HttpGet("GetTransportReqByTransportData/{TransportID}")]
        public IActionResult GetTblTransportRequestedByTransportDataIDFromDataBase(int TransportID)
        {
            if (TransportID > 0)
            {
                List<TransportRequestionViewModel> listData = new List<TransportRequestionViewModel>();
                var hasData = this.repository.GetTblTranspotRequestedWithTransportDataID(TransportID);
                foreach(var item in hasData)
                {
                    listData.Add(this.mapper.Map<TblTransportRequestion, TransportRequestionViewModel>(item));
                }
                return new JsonResult(listData, this.DefaultJsonSettings);
            }

            return NotFound(new { Error = "transport requestion id is req" });
        }
        #endregion

        #region Method Post

        [HttpPost("GetTransportReqWithLazyLoad")]
        public IActionResult GetTransportRequestionWithLazyLoadFromDataBase([FromBody] LazyViewModel lazyData)
        {
            if (lazyData != null)
                return new JsonResult(this.repository.GetTransportRequestionViewModelWithLazyLoad(lazyData), this.DefaultJsonSettings);
            return NotFound(new { Error = "lazy data has not been found" });
        }

        [HttpPost("PostTransportReq")]
        public IActionResult PostTblTransportRequestionToDataBase([FromBody] TransportRequestion2ViewModel nTranReq)
        {
            if (nTranReq != null)
            {
                if (!string.IsNullOrEmpty(nTranReq.EmployeeRequestCode))
                {
                    var tempDate = nTranReq.TransportDate.Value;
                    System.TimeSpan time = System.TimeSpan.Parse(nTranReq.TransportTime);

                    nTranReq.TransportDate = new System.DateTime(tempDate.Year, tempDate.Month, tempDate.Day, time.Hours, time.Minutes, 0);

                    // Update to company
                    int? CompanyId = null;
                    if (nTranReq.CompanyID.HasValue)
                        CompanyId = nTranReq.CompanyID.Value;

                    var hasData = this.repository.InsertTblTransportRequestionToDataBase
                        (this.mapper.Map<TransportRequestion2ViewModel, TblTransportRequestion>(nTranReq));

                    if (hasData != null)
                    {
                        if (CompanyId.HasValue)
                        {
                            this.repositoryReqCompany.Add(new TblReqHasCompany()
                            {
                                CompanyId = CompanyId,
                                CreateDate = DateTime.Now,
                                Creator = hasData.Creator,
                                TransportRequestId = hasData.TransportRequestId
                            });

                            //----\\
                            this.AlertToHighLevel(CompanyId.Value, hasData.Creator);
                            //----\\
                        }
                        return new JsonResult(hasData, DefaultJsonSettings);
                    }
                }
            }
            return new StatusCodeResult(500);
        }

        [HttpPost("PostTransportReqAttach/{TransportReqID}")]
        public async Task<IActionResult> PostUpload(int TransportReqID, IEnumerable<IFormFile> files)
        {
            long size = files.Sum(f => f.Length);

            // full path to file in temp location
            var filePath1 = Path.GetTempFileName();

            foreach (var formFile in files)
            {
                string FileName = Path.GetFileName(formFile.FileName).ToLower();
                // create file name for file
                string FileNameForRef = $"{DateTime.Now.ToString("ddMMyyhhmmssfff")}{ Path.GetExtension(FileName).ToLower()}";
                // full path to file in temp location
                var filePath = Path.Combine(this.appEnvironment.WebRootPath + "\\files", FileNameForRef);

                if (formFile.Length > 0)
                {
                    using (var stream = new FileStream(filePath, FileMode.Create))
                        await formFile.CopyToAsync(stream);
                }

                this.repository.InsertTblAttachFileToDataBase(TransportReqID, new TblAttachFile()
                {
                    AttachAddress = $"/files/{FileNameForRef}",
                    AttachFileName = FileName,
                });
            }

            return Ok(new { count = 1, size, filePath1 });
        }

        #endregion

        #region Method Put

        [HttpPut("PutTransportReq/{key}")]
        public IActionResult PutTblTransportRequestionToDataBase(int key, [FromBody] TransportRequestion2ViewModel uTranReq)
        {

            if (uTranReq != null)
            {
                if (!string.IsNullOrEmpty(uTranReq.EmployeeRequestCode))
                {
                    var tempDate = uTranReq.TransportDate.Value;
                    System.TimeSpan time = System.TimeSpan.Parse(uTranReq.TransportTime);

                    uTranReq.TransportDate = new System.DateTime(tempDate.Year, tempDate.Month, tempDate.Day, time.Hours, time.Minutes, 0);

                    // Update to company
                    int? CompanyId = null;
                    if (uTranReq.CompanyID.HasValue)
                        CompanyId = uTranReq.CompanyID.Value;

                    var hasData = this.repository.UpdateTblTransportRequestionToDataBase(key, uTranReq);
                    if (hasData != null)
                    {
                        if (CompanyId.HasValue)
                        {
                            Expression<Func<TblReqHasCompany, bool>> condition = c => c.TransportRequestId == hasData.TransportRequestId;
                            var hasReqHasCompany = this.repositoryReqCompany.Find(condition);
                            if (hasReqHasCompany != null)
                            {
                                hasReqHasCompany.CompanyId = CompanyId;
                                hasReqHasCompany.Modifyer = hasData.Modifyer;
                                hasReqHasCompany.ModifyDate = hasData.ModifyDate;
                                // update to table car has company
                                this.repositoryReqCompany.Update(hasReqHasCompany, hasReqHasCompany.ReqHasCompanyId);
                            }
                            else
                            {
                                this.repositoryReqCompany.Add(new TblReqHasCompany()
                                {
                                    CompanyId = CompanyId,
                                    CreateDate = DateTime.Now,
                                    Creator = hasData.Creator,
                                    TransportRequestId = hasData.TransportRequestId
                                });
                            }

                            //----\\
                            // this.AlertToHighLevel(CompanyId.Value, hasData.Creator);
                            //----\\
                        }
                        return new JsonResult(hasData, DefaultJsonSettings);
                    }
                }
            }
            return new StatusCodeResult(500);
        }
        #endregion

        #region Method Delete

        [HttpDelete("CancelTranspotRequest/{key}/{user}")]
        public IActionResult CancelTblTransportRequestionToDataBase(int key,string user)
        {

            if (key > 0)
            {
                if (this.repository.CancelTblTransportRequestionToDataBase(key, user))
                    return new OkResult();
            }
            // return a HTTP Status 404 (Not Found) if we couldn't find a suitable item.
            return NotFound(new
            {
                Error = String.Format("Transport request ID {0} has not been found", key)
            });
        }

        [HttpDelete("DeleteTransportReqAttach/{key}")]
        public void DeleteTransportRequestAttach(int key)
        {
            if (key > 0)
            {
                var attachFile = this.repository.GetTblAttachFileByAttachID(key);
                if (attachFile != null)
                {
                    var filePath = Path.Combine(this.appEnvironment.WebRootPath + attachFile.AttachAddress);
                    FileInfo delFile = new FileInfo(filePath);

                    if (delFile.Exists)
                        delFile.Delete();

                    this.repository.RemoveTblTransportRequestHasAttachFromDataBase(attachFile);
                }
            }
        }

        #endregion

        #endregion TblTransportRequestion

        #region TblTransportType

        [HttpGet("GetTransportTypes")]
        public IActionResult GetTblTransportTypeFromDataBase()
        {
            return new JsonResult(this.repository.GetTblTransportTypes, this.DefaultJsonSettings);
        }

        #endregion

        #region Mail

        private bool SendMail( List<string> MailAddress, BodyBuilder BodyMessage,
                                string FromUser,string FromAddress)
        {
            try
            {
                if (!MailAddress.Any())
                    return false;

                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(FromUser, FromAddress));
                foreach (var email in MailAddress)
                    message.To.Add(new MailboxAddress(email));

                message.Subject = "Notification mail from Transportation System.";

                message.Body = BodyMessage.ToMessageBody();
                using (var client = new SmtpClient())
                {
                    client.Connect("mail.vipco-thai.com", 25, SecureSocketOptions.None);
                    client.Send(message);
                    client.Disconnect(true);
                    return true;
                }
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
            return false;
        }

        #endregion
    }
}