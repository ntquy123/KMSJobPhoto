using System;
using System.Collections.Generic;
using System.Text;

namespace erpsolution.entities.Setting
{
    public class Jwt
    {
        public string Key { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public int? TimeLife { get; set; }
        public bool AllowMultiDevice { get; set; }
        public bool DisabledExpire { get; set; }
    //    "Key": "dd%88*388f6d&f?$?FdddFF33fssDG^!3",
    //"Issuer": "http://localhost:50808",
    //"Audience": "http://localhost:50808",
    //"TimeLife": 1//Min
    }
}
