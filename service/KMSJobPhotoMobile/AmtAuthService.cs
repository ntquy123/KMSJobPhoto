 
using erpsolution.dal.EF;
using erpsolution.entities;
using erpsolution.service.Interface;
using Microsoft.EntityFrameworkCore;
using service.Common.Base;
using System;
using System.Linq;



namespace erpsolution.service.KMSJobPhotoMobile
{
    public class AmtAuthService : ServiceBase<TCMUSMT>, IAmtAuthService
    {
        public AmtAuthService(IServiceProvider serviceProvider) : base(serviceProvider)
        {

        }
        public override string PrimaryKey => throw new NotImplementedException();

        public TCMUSMT CheckLoginERP(LoginModel pUser)
        {
            try
            {
                var userERP = _amtContext.TCMUSMT
     .FromSqlInterpolated($@"
        select 
            USERID  as ""UserId"",
            NAME    as ""Name"",
            EPASSWD as ""EPassWd""
        from T_CM_USMT@ERP_LINK
        where USERID = {pUser.Username} AND STATUS = 'OK' ")
     .AsNoTracking()
     .FirstOrDefault();


                if (userERP != null)
                {
                    bool Istrue = lib.HashHelper.IsEqualHashValue512(pUser.Password, userERP.EPassWd);
                    if (Istrue)
                    {
                        return userERP;
                    }
                    else
                        throw new Exception("Incorrect password");

                }
                else
                    throw new Exception("This account was not found.");

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
    }
}
