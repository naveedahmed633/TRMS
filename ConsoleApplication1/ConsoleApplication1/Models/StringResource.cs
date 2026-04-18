namespace DLL.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;


    public class StringResource
    {
        public long Id { get; set; }
        public string ValueID { get; set; }
        public string ValueTextEn { get; set; }
        public string ValueTextAr { get; set; }
        
        public string SelectLanguage { get; set; }
        public bool Active_Status { get; set; }

    }
}
