using erpsolution.entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace erpsolution.service.Common.Base.Interface
{
    public interface ICurrentUser
    {
        //bool IsSystemAdmin { get; }
        string UserName { get; }
        int UserId { get; }
        int CompanyId { get; }
        IdentityUserModel UserIdentity { get; }
      //  IQueryable<decimal> Companys { get; }
     //   IQueryable<decimal> Factorys { get; }
       // IQueryable<string> Buyers { get; }
        //List<decimal> Orgs { get; }
        bool CheckPerssion(decimal? CompanyId = null, decimal? Factory = null, string Buyer = null);
        decimal? WHWorkOrgId { get; }
    }
}
