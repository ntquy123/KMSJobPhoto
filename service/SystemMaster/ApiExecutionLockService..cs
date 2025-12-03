using erpsolution.dal.EF;
using erpsolution.service.Common.Base;
using erpsolution.service.Common.Cache;
using erpsolution.service.Interface.SystemMaster;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Concurrent;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using service.Common.Base;

namespace erpsolution.service.SystemMaster
{
    public class ApiExecutionLockService : ServiceBase<ApiLogs>, IApiExecutionLockService
    {
        private string _ScanCache = "_ScanCache";
        private ICacheService _cacheSevice;
        public override string PrimaryKey => nameof(ApiLogs.LogId);
        public ApiExecutionLockService(
           IServiceProvider serviceProvider,
           ICacheService cacheSevice
           ) : base(serviceProvider)
        {
            _cacheSevice = cacheSevice;
        }
        private static readonly ConcurrentDictionary<string, SemaphoreSlim> _semaphoreForFinalConfirm = new ConcurrentDictionary<string, SemaphoreSlim>();

        public async Task<ApiLogs> SaveLogError(ApiLogs modelSave)
        {
            #if DEBUG
                        return new ApiLogs
                        {
                            LogId = 0
                        };
            #endif
            try
            {
                var modelAdd = new ApiLogs
                {
                    Method = modelSave.Method,
                    ApiName = modelSave.ApiName,
                    MenuName = modelSave.MenuName,
                    System = modelSave.System,
                    RequestJson = modelSave.RequestJson,
                    Description = modelSave.Description,
                    Exception = modelSave.Exception,
                    Message = modelSave.Message,
                    TransDate = int.Parse(DateTime.Now.ToString("yyyyMMdd")),
                    CreatedDate = DateTime.Now,
                    CreateId = modelSave.CreateId ?? _currentUser.UserId.ToString(),

                };
                await _logContext.ApiLogs.AddAsync(modelAdd);
                await _logContext.SaveChangesAsync();
                return modelAdd;

            }
            catch (Exception ex) {
                //return null;
                throw new Exception("Can't save Log " + ex.Message);
            }
        }
        public async Task WaitForLockWithDelayAsync(int delayMilliseconds = 5)
        {
            while (true)
            {
                try
                {
                    var key = _currentUser.WHWorkOrgId.ToString();
                    var semaphore = _semaphoreForFinalConfirm.GetOrAdd(key, _ => new SemaphoreSlim(1, 1));
                    await semaphore.WaitAsync();
                    return;
                }
                catch
                {
                    await Task.Delay(delayMilliseconds);
                }
            }
        }
        public void ReleaseLock()
        {
            var key = _currentUser.WHWorkOrgId.ToString();
            if (_semaphoreForFinalConfirm.TryGetValue(key, out var semaphore))
            {
                semaphore.Release();
            }
        }

        //pending scan
        public bool IsRequestScanQRPending(string pQrCode)
        {
            bool isPending = false;
            string QRText = pQrCode.ToString();
            _cacheSevice.Get(_ScanCache + QRText, out isPending);
            return isPending;
        }
        public void MarkRequestScanQRAsPending(string pQrCode)
        {
            bool isPending = true;
            string QRText = pQrCode.ToString();
            _cacheSevice.Add(isPending, _ScanCache + QRText, 3);
        }
        public void ClearPendingScanQRRequest(string pQrCode)
        {
            string QRText = pQrCode.ToString();
            _cacheSevice.Remove(_ScanCache + QRText);
        }
    }
}
