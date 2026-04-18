namespace DLL.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public class LoanStatusType
    {
        public int Id { get; set; }

        public string LoanStatusTypeText { get; set; }
    }
}
