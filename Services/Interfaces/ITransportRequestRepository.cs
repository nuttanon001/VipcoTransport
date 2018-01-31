using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VipcoTransport.ViewModels;

namespace VipcoTransport.Services.Interfaces
{
    public interface ITransportRequestRepository
    {
        ScheduleViewModel<IDictionary<String, Object>> RequestionWaiting(ConditionViewModel condition);
    }
}
