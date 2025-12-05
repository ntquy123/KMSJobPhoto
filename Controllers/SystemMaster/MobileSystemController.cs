using System;
using System.Linq;
using System.Threading.Tasks;
using entities.Common;
using erpsolution.dal.Context;
using erpsolution.entities.SystemMaster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace erpsolution.api.Controllers.SystemMaster
{
    [Route("[controller]")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "systemmaster")]
    public class MobileSystemController : ControllerBase
    {
        private readonly AmtContext _context;

        public MobileSystemController(AmtContext context)
        {
            _context = context;
        }

        [AllowAnonymous]
        [HttpPost(nameof(CheckMobileVersion))]
        public async Task<HandleResponse<ZmMasMobileVersionModel>> CheckMobileVersion(int VersionId)
        {
            try
            {
                var getVersion = await _context.ZmMasMobileVersions.Where(x => x.UseYn == '1').ToListAsync();
                if (getVersion.Count() > 1)
                {
                    getVersion = getVersion.OrderByDescending(x => x.VersionId).ToList();
                }

                var lastVersion = getVersion.Select(x => x.VersionId).FirstOrDefault();
                if (VersionId < lastVersion)
                {
                    throw new Exception("New version available, Please update your mobile app");
                }

                return new HandleResponse<ZmMasMobileVersionModel>(true, "Success");
            }
            catch (Exception ex)
            {
                return new HandleResponse<ZmMasMobileVersionModel>(false, ex.Message);
            }
        }
    }
}
