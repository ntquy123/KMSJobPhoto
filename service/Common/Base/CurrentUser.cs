using erpsolution.dal.Context;
using erpsolution.entities.Common;
using erpsolution.service.Common.Base.Interface;
using System.Security.Claims;
using System.Security.Principal;

namespace erpsolution.service.Common.Base
{
    public class CurrentUser : ICurrentUser
    {

        private readonly IPrincipal _principal;
        private AmtContext _context;

        public CurrentUser(IPrincipal principal, AmtContext context)
        {
            _principal = principal;
            _context = context;
        }
        
        public string UserName => _principal?.Identity.Name;
        public int UserId => UserIdentity.UserId;
        public int CompanyId => UserIdentity.CompanyId;

        private List<Claim> claims => ((ClaimsPrincipal) _principal).Claims.ToList();
       // public IQueryable<decimal> Companys => _context.PkTbMasUserRoll.Where(i => i.UserMapId == UserId && i.AuthType == (byte)AuthType.Company).Select(i => i.WorkOrgId);
     //   public IQueryable<decimal> Factorys => _context.PkTbMasUserRoll.Where(i => i.UserMapId == UserId && i.AuthType == (byte)AuthType.Factory).Select(i => i.WorkOrgId);
        //public List<string> Buyers => JsonConvert.DeserializeObject<List<string>>(claims.FirstOrDefault(i => i.Type == "buyers").Value);
       // public IQueryable<string> Buyers => _context.PkTbMasUserRoll.Where(i=> i.UserMapId == UserId && i.AuthType == (byte)AuthType.Buyer).Select(i=>i.ComfacbuyCd);
        //public List<decimal> Orgs => JsonConvert.DeserializeObject<List<decimal>>(claims.FirstOrDefault(i => i.Type == "work_orgs").Value);

        private IdentityUserModel _UserIdentity { get; set; }
        public decimal? DeviceNo => UserIdentity.DeviceNo;
        public decimal? WHWorkOrgId => UserIdentity.WHWorkOrgId;
        public IdentityUserModel UserIdentity
        {
            get
            {
                if (_UserIdentity != null)
                    return _UserIdentity;
                if (_principal != null)
                {
                    //  var claims = ((ClaimsPrincipal) _principal).Claims.ToList();
                    string deviceNo = claims.FirstOrDefault(x => x.Type == "device_no")?.Value;
                    return new IdentityUserModel
                    {
                        UserId = Convert.ToInt32(claims.FirstOrDefault(x => x.Type == "user_id")?.Value),
                        UserName = _principal.Identity.Name,
                        FullName = claims.FirstOrDefault(x => x.Type == "full_name")?.Value,
                        CompanyId = Convert.ToInt32(claims.FirstOrDefault(x => x.Type == "company_id")?.Value),
                        //CompanyPackageId = Convert.ToInt32(claims.FirstOrDefault(x => x.Type == "company_pack_id")?.Value),
                        CompanyPackageId = 1,
                        OrgId = Convert.ToInt32(claims.FirstOrDefault(x => x.Type == "org_id")?.Value),
                        DeviceNo = deviceNo != "" ? Convert.ToDecimal(deviceNo) : 0,
                        WHWorkOrgId = Convert.ToDecimal(claims.FirstOrDefault(x => x.Type == "hr_cd")?.Value),
                    };
                    
                }
                return  new IdentityUserModel();
            }
        }
        public bool CheckPerssion(decimal? CompanyId = null, decimal? Factory = null, string Buyer=null)
        {
            //if (CompanyId != null && !Companys.Contains(CompanyId.Value))
            //    return false;
            //if (Factory != null && !Factorys.Contains(Factory.Value))
            //    return false;
            //if (Buyer != null && !Buyers.Contains(Buyer))
            //    return false;
            return true;
        }
        //private List<decimal> GetPermissionWorkOrg(int UserMapId)
        //{
        //    var lstWorkOrgByCompany = _context.PkTbMasBizorg.Where(i => GetPermissionCompany(UserMapId).Contains(i.WorkOrgId)).ToList();
        //    var lstWorkOrgByFactory = _context.PkTbMasBizorg.Where(i => GetPermissionFactory(UserMapId).Contains(i.WorkOrgId)).ToList();
        //    List<PkTbMasBizorg> lst = new List<PkTbMasBizorg>();
        //    lst.AddRange(lstWorkOrgByFactory);
        //    lst.AddRange(lstWorkOrgByCompany);
        //    if (lstWorkOrgByFactory.Count() > 0)
        //    {
        //        lstWorkOrgByFactory.ForEach(i => _pkTbMasBizorgService.GetListChild(i.WorkOrgId, lst));
        //        return lst.Select(i => i.WorkOrgId).Distinct().ToList();
        //    }
        //    lstWorkOrgByCompany.ForEach(i => _pkTbMasBizorgService.GetListChild(i.WorkOrgId, lst));
        //    return lst.Select(i => i.WorkOrgId).Distinct().ToList();
        //}


    }
}
