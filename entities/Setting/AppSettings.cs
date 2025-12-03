using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace entities.Setting
{
    public class AppSettings
    {
        public string Surfix { get; set; }
        public string UploadRootPath { get; set; }
        public string UploadRootDomain { get; set; }
        public List<string> WareHouse { get; set; }
        public string ServerCountry { get; set; }
        public List<string> Factory { get; set; }
        public decimal DefaultCompany { get; set; }
        public List<string> Buyer { get; set; }
        public List<string> SyncAOList { get; set; }
    }
}
