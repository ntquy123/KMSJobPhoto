using System;
using System.Collections.Generic;
using System.Text;

namespace erpsolution.entities.Common
{
    public class TokenLifeTime
    {
        public string userName { get; set; }
        public DateTime LoginTime { get; set; }
        public string RemoteIpAddress { get; set; }
    }
}
