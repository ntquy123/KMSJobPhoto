using System;
using System.Collections.Generic;
using System.Text;

namespace erpsolution.dal.EF
{
    public class ApiLogs
    {
        public int LogId { get; set; } // LOG_ID
        public int TransDate { get; set; } // TRANS_DATE
        public string? Method { get; set; } // API_NAME
        public string? Exception { get; set; } // API_NAME
        public string? Message { get; set; } // API_NAME
        public string? MenuName { get; set; } // API_NAME
        public string? System { get; set; } // API_NAME
        public string? ApiName { get; set; } // API_NAME
        public string? RequestJson { get; set; } // REQUEST_JSON
        public DateTime CreatedDate { get; set; } // CREATED_DATE
        public string? Description { get; set; } // DESCRIPTION
        public string? CreateId { get; set; } // CREATE_ID
    }
    public class ApiLogsForSearch
    {
        public int LogId { get; set; } // LOG_ID
        public int TransDate { get; set; } // TRANS_DATE
        public string Method { get; set; } // API_NAME
        public string MenuName { get; set; } // API_NAME
        public string System { get; set; } // API_NAME
        public string ApiName { get; set; } // API_NAME
        public string RequestJson { get; set; } // REQUEST_JSON
        public DateTime CreatedDate { get; set; } // CREATED_DATE
        public string Description { get; set; } // DESCRIPTION
        public string CreateId { get; set; } // CREATE_ID
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
    }
}
