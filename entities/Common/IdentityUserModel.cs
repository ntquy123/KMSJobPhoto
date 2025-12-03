using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace erpsolution.entities.Common
{
    public class IdentityUserModel
    {
        public int CompanyId { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public int? CompanyPackageId { get; set; }
        public int? OrgId { get; set; }
        public decimal? DeviceNo { get; set; }
        public decimal? WHWorkOrgId { get; set; }
    }
}
