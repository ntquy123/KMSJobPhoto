using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace erpsolution.entities.SystemMaster
{
    public class ZmMasMobileVersionModel
    {
        public int VersionId { get; set; }
        public string VersionName { get; set; }
        public DateTime ReleaseDate { get; set; }
        public string? Description { get; set; }
        public char Useyn { get; set; }
    }
}
