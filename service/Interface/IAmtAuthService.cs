 
using erpsolution.dal.EF;
using erpsolution.entities;
using service.Common.Base.Interface;

namespace erpsolution.service.Interface
{
    public interface IAmtAuthService  
    {
        new string PrimaryKey { get; }
        TCMUSMT CheckLoginERP(LoginModel pUser);
    }
}
