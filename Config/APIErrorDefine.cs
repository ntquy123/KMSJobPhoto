using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace erpsolution.api.Config
{
    public enum ErrorCode
    {
        UNKNOW_ERROR = 99,
        SYSTEM_ERROR = 999,
        UNAUTHORIZED = 1000,
        DATA_INVALID = 1001,
        DATA_NOT_FOUND = 1002,
        SAVE_FAILED = 1003,
        LOGIN_FAILED = 2000,
        FORBIDDEN = 403,
    }
}
