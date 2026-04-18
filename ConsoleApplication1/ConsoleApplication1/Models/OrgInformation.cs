namespace DLL.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class OrgInformation
    {
        public int id { get; set; }

        public string OrgName { get; set; }

        public string OrgEmail { get; set; }

        public string OrgAddress { get; set; }

        public string OrgWebsite { get; set; }

        public string OrgContact { get; set; }

        public string OrgLogo { get; set; }

    }
}
