using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dal.Context.Setting
{
    public interface IConnectionSetting
    {
        string MsSQLConnection { get; set; }
        string MySQLConnection { get; set; }
        string OracleConnection { get; set; }
        string ErpConnection { get; set; }
        string AmtConnection { get; set; }
    }
}
