namespace DLL.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public class PayrollAmount
    {
        public int Id { get; set; }
        public int DesignationId { get; set; }
        public int GradeId { get; set; }

        public int BasicPay { get; set; }
        public int Increment { get; set; }
        public int Transport { get; set; }
        public int Mobile { get; set; }
        public int Medical { get; set; }
        public int CashAllowance { get; set; }
        public int Commission { get; set; }
        public int Food { get; set; }
        public int Night { get; set; }
        public int Rent { get; set; }
        public int GroupAllowance { get; set; }

        public DateTime CreateDateTime { get; set; }
        public DateTime UpdateDateTime { get; set; }
    }
}
