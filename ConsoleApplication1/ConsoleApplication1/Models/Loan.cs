namespace DLL.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public class Loan
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public DateTime LoanAllocatedDate { get; set; }

        public int LoanTypeId { get; set; }
        public int LoanAmount { get; set; }
        public int InstallmentNumbers { get; set; }
        public int InstallmentAmount { get; set; }
        public int DeductableAmount { get; set; }
        public int BalanceAmount { get; set; }
        public int LoanStatusId { get; set; }

        public string Remarks { get; set; }
        public string AttachmentFilePath { get; set; }
        public DateTime CreateDateTime { get; set; }
        public DateTime UpdateDateTime { get; set; }
    }
}
