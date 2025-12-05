using System;
using System.Collections.Generic;

namespace erpsolution.dal.EF
{
    public class ZmMasMobileVersions
    {
        public int VersionId { get; set; }
        public string VersionName { get; set; }
        public DateTime ReleaseDate { get; set; }
        public string Description { get; set; }
        public char UseYn { get; set; }
    }
}
