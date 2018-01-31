using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using VipcoTransport.Models;
using VipcoTransport.ViewModels;

namespace VipcoTransport.Services.Interfaces
{
    public interface ITransportRepository
    {
        //TblTransportData
        IQueryable<TblTransportData> GetTblTransportDatas { get; }
        IQueryable<TblTransportData> GetTblTransportDatasTruck { get; }
        DataViewModel<TransportLazyViewModel> GetTransportViewModelWithLazyLoad(LazyViewModel lazyData);
        DataViewModel<TransportLazyViewModel> GetTransportTruckViewModelWithLazyLoad(LazyViewModel lazyData);
        ScheduleViewModel<IDictionary<String, Object>> GetTransportScheduleByDaily(DateTime pickDate);
        ScheduleViewModel<IDictionary<String, Object>> GetTransportScheduleByWeekly(DateTime startDate, DateTime endDate);
        ScheduleViewModel<IDictionary<String, Object>> GetTransportScheduleByWeeklyV2(ScheduleWeeklyViewModel scheduleWeekly);
        ScheduleViewModel<IDictionary<String, Object>> GetTransportScheduleDailyWithTransportData(string stringData);
        IEnumerable<TblTransportData> GetTransportDataSameTimeOfTransportRequestByID(int TransportRequestID);
        IEnumerable<TblTransportData> GetTransportDataViewModelForReportByTrnasportID(int TransportID);
        TblTransportData GetTransportDataByTransportRequestedID(int TransportRequestID);
        TblTransportData GetTblTransportDataWithKey(int TransportID);
        TblTransportData InsertTblTransportDataToDataBase(TblTransportData nTransport, int TransportRequestedId);
        TblTransportData UpdateTblTransportDataToDataBase(int TransportID, TblTransportData uTransport);
        bool UpdateTransportReqestionSameTransportData(int TransportID, int TransportRequestionID, string Create = "");

        //TblTransportRequestion
        IQueryable<TblTransportRequestion> GetTblTransportRequestions { get; }
        DataViewModel<TransportRequestionLazyViewModel> GetTransportRequestionViewModelWithLazyLoad(LazyViewModel lazyData);
        ScheduleViewModel<IDictionary<String, Object>> GetTransportRequestionScheduleWaiting(bool dateCondition = false);
        TblTransportRequestion GetTblTransportRequestionWithKey(int TransportReqID);
        IEnumerable<TblTransportRequestion> GetTblTranspotRequestedWithTransportDataID(int TransportDataID);
        bool CancelTblTransportRequestionToDataBase(int transportRequestID,string UserName);
        TblTransportRequestion InsertTblTransportRequestionToDataBase(TblTransportRequestion nTranReq);
        TblTransportRequestion UpdateTblTransportRequestionToDataBase(int TransportReqID, TblTransportRequestion uTranReq);

        //TblTransportType
        IEnumerable<TblTransportType> GetTblTransportTypes { get; }

        //TblAttachFile
        TblAttachFile GetTblAttachFileByAttachID(int AttachID);
        IEnumerable<TblAttachFile> GetTblAttachFilesByTransportRequestionID(int TransportReqID);
        bool InsertTblAttachFileToDataBase(int TransportID, TblAttachFile nAttachFile);
        bool RemoveTblTransportRequestHasAttachFromDataBase(TblAttachFile attachFile);
    }
}
