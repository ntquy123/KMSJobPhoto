using erpsolution.dal.EF;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace erpsolution.service.Interface.SystemMaster
{
    public interface IApiExecutionLockService
    {
        public Task WaitForLockWithDelayAsync(int delayMilliseconds = 5);
        public void ReleaseLock();
       
       

        public bool IsRequestScanQRPending(string pQrCode);
        public void MarkRequestScanQRAsPending(string pQrCode);
        public void ClearPendingScanQRRequest(string pQrCode);

        Task<ApiLogs> SaveLogError(ApiLogs modelSave);
    }
}
