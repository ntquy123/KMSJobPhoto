using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace erpsolution.entities.Common
{
    public class ErrorResponseModel
    {
        [JsonProperty("error")]
        public ErrorModel Error { get; set; }
    }

    public class ErrorModel
    {
        [JsonProperty("code")]
        public int ErrorCode { get; set; }
        [JsonProperty("message")]
        public string ErrorMessage { get; set; }
    }
}
