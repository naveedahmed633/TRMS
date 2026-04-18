using DLL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels
{
    // This class is just a Big View Model for sending multiple models to the
    // employee CRUD view.
    public class CreateLeaveApplication
    {
        public DateTime SessionStartDate;
        public DateTime SessionEndDate;

        public string strSessionStartDate;
        public string strSessionEndDate;

        public int AvailableSickLeaves;
        public int AvailableCasualLeaves;
        public int AvailableAnnualLeaves;
        public int AvailableOtherLeaves;
        public int AvailableLeaveType01;
        public int AvailableLeaveType02;
        public int AvailableLeaveType03;
        public int AvailableLeaveType04;
        public int AvailableLeaveType05;
        public int AvailableLeaveType06;
        public int AvailableLeaveType07;
        public int AvailableLeaveType08;
        public int AvailableLeaveType09;
        public int AvailableLeaveType10;
        public int AvailableLeaveType11;

        public int AvailedSickLeaves;
        public int AvailedCasualLeaves;
        public int AvailedAnnualLeaves;
        public int AvailedOtherLeaves;
        public int AvailedLeaveType01;
        public int AvailedLeaveType02;
        public int AvailedLeaveType03;
        public int AvailedLeaveType04;
        public int AvailedLeaveType05;
        public int AvailedLeaveType06;
        public int AvailedLeaveType07;
        public int AvailedLeaveType08;
        public int AvailedLeaveType09;
        public int AvailedLeaveType10;
        public int AvailedLeaveType11;

        public List<LeaveTypeInfo> leave_types;
        public List<LeaveReasonInfo> leave_reasons;
        public List<LeaveStatusInfo> leave_status;
        public List<LeaveApproverInfo> leave_approver;
    }

    public class LeaveTypesCountStatus
    {
        public int DefaultSickLeaves;
        public int DefaultCasualLeaves;
        public int DefaultAnnualLeaves;
        public int DefaultOtherLeaves;
        public int DefaultLeaveType01;
        public int DefaultLeaveType02;
        public int DefaultLeaveType03;
        public int DefaultLeaveType04;
        public int DefaultLeaveType05;
        public int DefaultLeaveType06;
        public int DefaultLeaveType07;
        public int DefaultLeaveType08;
        public int DefaultLeaveType09;
        public int DefaultLeaveType10;
        public int DefaultLeaveType11;

        public int MaxSickLeaves;
        public int MaxCasualLeaves;
        public int MaxAnnualLeaves;
        public int MaxOtherLeaves;
        public int MaxLeaveType01;
        public int MaxLeaveType02;
        public int MaxLeaveType03;
        public int MaxLeaveType04;
        public int MaxLeaveType05;
        public int MaxLeaveType06;
        public int MaxLeaveType07;
        public int MaxLeaveType08;
        public int MaxLeaveType09;
        public int MaxLeaveType10;
        public int MaxLeaveType11;

        public int StatusSickLeaves;
        public int StatusCasualLeaves;
        public int StatusAnnualLeaves;
        public int StatusOtherLeaves;
        public int StatusLeaveType01;
        public int StatusLeaveType02;
        public int StatusLeaveType03;
        public int StatusLeaveType04;
        public int StatusLeaveType05;
        public int StatusLeaveType06;
        public int StatusLeaveType07;
        public int StatusLeaveType08;
        public int StatusLeaveType09;
        public int StatusLeaveType10;
        public int StatusLeaveType11;
    }
}
