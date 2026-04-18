namespace DLL.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public class LeaveApplication
    {
        public int Id { get; set; }

        public int EmployeeId { get; set; }

        public int LeaveTypeId { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public int DaysCount { get; set; }

        public int LeaveReasonId { get; set; }

        public string ReasonDetail { get; set; }

        public int ApproverId { get; set; }

        public string ApproverDetail { get; set; }

        public int LeaveStatusId { get; set; }

        public int LeaveStatusHODId { get; set; }
        public int LeaveStatusPRNId { get; set; }
        public int LeaveStatusHRId { get; set; }
        public int LeaveStatusVCId { get; set; }

        public string AttachmentFilePath { get; set; }

        public bool IsActive { get; set; }

        public int LeaveValidityId { get; set; }

        public string LeaveValidityRemarks { get; set; }

        public DateTime CreateDateTime { get; set; }

        public DateTime UpdateDateTime { get; set; }

        public virtual LeaveType LeaveType { get; set; }

        public virtual LeaveStatus LeaveStatus { get; set; }

        public virtual LeaveReason LeaveReason { get; set; }
    }
}
