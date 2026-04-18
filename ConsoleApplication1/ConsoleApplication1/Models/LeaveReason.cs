namespace DLL.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public class LeaveReason
    {
        public int Id { get; set; }

        public string LeaveReasonText { get; set; }

        public bool IsActive { get; set; }
    }
}
