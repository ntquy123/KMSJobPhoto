using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dal.Context.Setting
{
    public class ConnectionSetting : IConnectionSetting
    {
        public string MySQLConnection { get; set; }
        public string MsSQLConnection { get; set; }
        public string OracleConnection { get; set; }
        public string RedisConnection { get; set; }
        public string ErpConnection { get; set; }
        public string AmtConnection { get; set; }
    }
}
