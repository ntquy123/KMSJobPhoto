using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace erpsolution.entities.Common
{
    public class FileUploadData
    {
        [JsonProperty("file_name")]
        public string FileName { get; set; }
        [JsonProperty("file_type")]
        public string FileType { get; set; }
        public string Value { get; set; }
    }
}
