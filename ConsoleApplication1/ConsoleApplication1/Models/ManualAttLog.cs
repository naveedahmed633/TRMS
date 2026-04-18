namespace DLL.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public class ManualAttLog
    {
        public int Id { get; set; }
        
       

        public string WhoMark { get; set; }

        public string WhoseMark { get; set; }

        public string FromDate { get; set; }

        public string ToDate { get; set; }

        public string Remarks { get; set; }
    }
}
