using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using entities.Common;
using erpsolution.api.Base;
using erpsolution.dal.EF;
using erpsolution.entities;
using erpsolution.service.Common.Base.Interface;
using erpsolution.service.Interface;
using erpsolution.service.Interface.SystemMaster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace erpsolution.api.Controllers.KMSJobPhotoMobile
{
    public class AuditResultRegistrationController : ControllerBaseEx<IAuditResultRegistrationService, AuditTodoRow, decimal>
    {
        private readonly IApiExecutionLockService _ApiExcLockService;

        public AuditResultRegistrationController(
            IAuditResultRegistrationService service,
            IApiExecutionLockService ApiExcLockService,
            ICurrentUser currentUser) : base(service, currentUser)
        {
            _ApiExcLockService = ApiExcLockService;
        }
        /// <param name="request">
        /// Data input 
        /// Example:
        /// ```json
        /// {
        ///   "FromDate": "2025-06-19",
        ///   "ToDate": "2026-12-31",
        ///   "Status": "ALL", //TODO,COMPLETED, ALL
        ///   "PlayerId": "20240340"
        /// }
        /// ```
        /// </param>
        [ApiExplorerSettings(GroupName = "kmsjobphoto_mobile")]
        [HttpPost(nameof(GetTodoList))]
        [ProducesResponseType(typeof(HandleResponse<object>), 400)]
        [AllowAnonymous]
        public async Task<IActionResult> GetTodoList([FromBody] AmtTodoRequest request)
        {
            try
            {
                var rows = await _service.GetTodoListAsync(request);
                return Ok(new HandleResponse<List<AuditTodoRow>>(true, string.Empty, rows, null));
            }
            catch (Exception ex)
            {
                var message = await LogErrorAsync(ex, "Audit Result Registration", request);
                return Json(new HandleResponse<List<AuditTodoRow>>(false, message, null));
            }
        }

        [ApiExplorerSettings(GroupName = "kmsjobphoto_mobile")]
        [HttpPost(nameof(UploadPhoto))]
        [ProducesResponseType(typeof(HandleResponse<List<AuditResultPhotoUploadResponse>>), 200)]
        [ProducesResponseType(typeof(HandleResponse<object>), 400)]
        [Consumes("multipart/form-data")] // Good practice to specify content type

        // Summary describes what the API does
        /// <summary>
        /// Uploads one or more evidence photos for a specific audit correction.
        /// </summary>
        /// <remarks>
        /// **Usage Instructions:**
        /// * Ensure the `AudplnNo` exists in the system.
        /// * Supported file formats: .jpg, .png.
        /// * Provide photos as `Photos[]` in form-data.
        /// * Maximum total file size: 300MB (validated on client side).
        /// </remarks>
        public async Task<IActionResult> UploadPhoto([FromForm] AuditResultPhotoUploadRequest request)
        {
            try
            {
                var result = await _service.UploadPhotoAsync(request, GetAuditImageBaseUrl());
                return Ok(new HandleResponse<List<AuditResultPhotoUploadResponse>>(true, string.Empty, result, null));
            }
            catch (Exception ex)
            {
                var message = await LogErrorAsync(ex, "Audit Result Registration", new
                {
                    request.AudplnNo,
                    request.Catcode,
                    request.CorrectionNo,
                    request.CorrectiveAction,
                    request.PhotoDescription
                });
                return Json(new HandleResponse<List<AuditResultPhotoUploadResponse>>(false, message, null));
            }
        }

        [ApiExplorerSettings(GroupName = "kmsjobphoto_mobile")]
        [HttpGet(nameof(GetPhotoList))]
        [ProducesResponseType(typeof(HandleResponse<List<AuditResultPhotoListResponse>>), 200)]
        [ProducesResponseType(typeof(HandleResponse<object>), 400)]
        public async Task<IActionResult> GetPhotoList([FromQuery] AuditResultPhotoListRequest request)
        {
            try
            {
                var result = await _service.GetPhotoListAsync(request, GetAuditImageBaseUrl());
                return Ok(new HandleResponse<List<AuditResultPhotoListResponse>>(true, string.Empty, result, null));
            }
            catch (Exception ex)
            {
                var message = await LogErrorAsync(ex, "Audit Result Registration", request);
                return Json(new HandleResponse<List<AuditResultPhotoListResponse>>(false, message, null));
            }
        }

        private async Task<string> LogErrorAsync(Exception ex, string menuName, object vm = null)
        {
            string currentUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}{HttpContext.Request.Path}{HttpContext.Request.QueryString}";
            string jsonData = System.Text.Json.JsonSerializer.Serialize(vm);
            var modelAdd = new ApiLogs
            {
                Method = HttpContext.Request.Method,
                ApiName = currentUrl,
                RequestJson = jsonData,
                Message = ex.Message,
                Exception = ex.ToString().Length > 100 ? ex.ToString().Substring(0, 100) : ex.ToString(),
                System = "Mobile",
                MenuName = menuName,
            };
            var log = await _ApiExcLockService.SaveLogError(modelAdd);
            return "Error ID:" + log.LogId + ": " + ex.Message;
        }

        private string GetAuditImageBaseUrl()
        {
            var forwardedProto = Request.Headers["X-Forwarded-Proto"].FirstOrDefault();
            var forwardedHost = Request.Headers["X-Forwarded-Host"].FirstOrDefault();
            var scheme = string.IsNullOrWhiteSpace(forwardedProto) ? Request.Scheme : forwardedProto;
            var host = string.IsNullOrWhiteSpace(forwardedHost) ? Request.Host.Value : forwardedHost;

            return $"{scheme}://{host}/audit/img";
        }
    }
}
