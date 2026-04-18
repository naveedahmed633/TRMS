namespace DLL.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public class Payroll
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public DateTime SalaryMonthYear { get; set; }
        public string CNIC { get; set; }
        public string NTNNumber { get; set; }
        public string EOBINumber { get; set; }
        public string SESSINumber { get; set; }

        public int BasicPay { get; set; }
        public int Increment { get; set; }
        public int Transport { get; set; }
        public int Mobile { get; set; }
        public int Medical { get; set; }
        public int Food { get; set; }
        public int Night { get; set; }
        public int GroupAllowance { get; set; }
        public int Commission { get; set; }
        public int Rent { get; set; }
        public int CashAllowance { get; set; }
        public int AnnualBonus { get; set; }
        public int LeavesCount { get; set; }
        public int LeavesEncash { get; set; }
        public decimal OvertimeInHours { get; set; }
        public int OvertimeAmount { get; set; }

        public int IncomeTax { get; set; }
        public int FineExtraAmount { get; set; }
        public int MobileDeduction { get; set; }
        public int AbsentsCount { get; set; }
        public int AbsentsAmount { get; set; }
        public int EOBIEmployee { get; set; }
        public int EOBIEmployer { get; set; }
        public int SESSIEmployee { get; set; }
        public int SESSIEmployer { get; set; }
        public int LoanInstallment { get; set; }
        public int OtherDeduction { get; set; }

        public int GrossSalary { get; set; }
        public int TotalDeduction { get; set; }
        public int NetSalary { get; set; }

        public int PaymentModeId { get; set; } //TABLE: PaymentModeTypes
        public int BankNameId { get; set; } //TABLE: BankNames
        public string BankAccTitle { get; set; }
        public string BankAccNo { get; set; }

        public int AckStatusId { get; set; } //TABLE: PaymentStatusTypes
        public int PayStatusId { get; set; } //TABLE: PaymentStatusTypes

        public string UserHRComments { get; set; }
        public string AttachmentFilePath { get; set; }        
        public string IsFirstMonthText { get; set; }

        public DateTime PaymentDateTime { get; set; }
        public DateTime CreateDateTime { get; set; }
        public DateTime UpdateDateTime { get; set; }
    }
}
