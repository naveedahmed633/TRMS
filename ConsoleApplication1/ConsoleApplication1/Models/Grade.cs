namespace DLL.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public class Grade
    {
        public int GradeId { get; set; }
        
        public int dbID { get; set; }

        public string name { get; set; }

        public string description { get; set; }

        public bool active { get; set; }
    }
}
