using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels
{
    /*
     * This View is used to load the finalized present
     * and absent report
     */

    public class JobsLog
    {
        [Key]
        public long id { get; set; }
        public string str_id { get; set; }
        public DateTime log_date { get; set; }
        public string str_log_date { get; set; }
        public string log_title { get; set; }
        public string log_message { get; set; }
    }


    public class JobsLogIntermediate
    {
        [Key]
        public long id { get; set; }
        public string str_id { get; set; }
        public DateTime log_date { get; set; }
        public string str_log_date { get; set; }
        public string log_title { get; set; }
        public string log_message { get; set; }
    }

    public class PaAttendanceLog
    {
        [Key]
        public int id { get; set; }
        public string date { get; set; }
        public string employee_code { get; set; }
        public string employee_first_name { get; set; }
        public string employee_last_name { get; set; }
        public string final_remarks { get; set; }
    }

    public class PayrollReportByMonthLog
    {
        [Key]
        public int Id { get; set; }

        public int EmployeeId { get; set; }
        public string EmployeeCode { get; set; }
        public string EmployeeName { get; set; }
        public DateTime SalaryMonthYear { get; set; }
        public string SalaryMonthYearText { get; set; }

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
        public string PaymentModeText { get; set; }
        public int BankNameId { get; set; } //TABLE: BankNames
        public string BankNameText { get; set; }
        public string BankAccTitle { get; set; }
        public string BankAccNo { get; set; }

        public int AckStatusId { get; set; } //TABLE: PaymentStatusTypes
        public string AckStatusText { get; set; }
        public int PayStatusId { get; set; } //TABLE: PaymentStatusTypes
        public string PayStatusText { get; set; }

        public string UserHRComments { get; set; }
        public string AttachmentFilePath { get; set; }
        public string IsFirstMonthText { get; set; }

        public DateTime PaymentDateTime { get; set; }
        public string PaymentDateTimeText { get; set; }
        public DateTime CreateDateTime { get; set; }
        public string CreateDateTimeText { get; set; }
        public DateTime UpdateDateTime { get; set; }
        public string UpdateDateTimeText { get; set; }

        public string actions { get; set; }
    }

    public class LoanReportByMonthLog
    {
        [Key]
        public int Id { get; set; }

        public int EmployeeId { get; set; }
        public string EmployeeCode { get; set; }
        public string EmployeeName { get; set; }

        public DateTime LoanAllocatedDate { get; set; }
        public string LoanAllocatedDateText { get; set; }

        public int LoanTypeId { get; set; }
        public string LoanTypeText { get; set; }
        public int LoanAmount { get; set; }
        public int InstallmentNumbers { get; set; }
        public int InstallmentAmount { get; set; }

        public int DeductableAmount { get; set; }
        public int BalanceAmount { get; set; }
        public int LoanStatusId { get; set; }
        public string LoanStatusText { get; set; }
        public string Remarks { get; set; }
        public string AttachmentFilePath { get; set; }

        public DateTime CreateDateTime { get; set; }
        public string CreateDateTimeText { get; set; }
        public DateTime UpdateDateTime { get; set; }
        public string UpdateDateTimeText { get; set; }

        public string actions { get; set; }
    }

    /*
    *This View Model is used to load finilized report of attendance
    *The View Model contains all attribute that will show on report page
    */
    public class MonthlyTimesheetAttendanceLog
    {
        [Key]
        public int id { get; set; }
        public string date { get; set; }
        public string time_in { get; set; }
        public string time_out { get; set; }
        public string status_in { get; set; }
        public string status_out { get; set; }
        public string final_remarks { get; set; }
        public string description { get; set; }
        public string employee_code { get; set; }
        public string employee_first_name { get; set; }
        public string employee_last_name { get; set; }
        public string terminal_in { get; set; }
        public string terminal_out { get; set; }
        public bool active { get; set; }
        //  public virtual ManualAttendance manualAttendance { get; set; }


        public string departmentName { get; set; }
        public string funcationName { get; set; }
        public string regionName { get; set; }
        public string locationName { get; set; }
        public string designationName { get; set; }

        public int overtime { get; set; }
        public string overtime_status { get; set; }
        public string str_overtime { get; set; }

        public string overtime2 { get; set; }

        public string action { get; set; }

        public string day { get; set; }

        public string status { get; set; }

    }

    public class MonthlyEmployeeShiftLog
    {
        [Key]
        public int id { get; set; }
        public string date { get; set; }
        public string time_in { get; set; }
        public string time_out { get; set; }
        public string status_in { get; set; }
        public string status_out { get; set; }
        public string final_remarks { get; set; }
        public string employee_code { get; set; }
        public string employee_first_name { get; set; }
        public string employee_last_name { get; set; }

        public string shift_name { get; set; }
        public string shift_start_time { get; set; }
        public string shift_end_time { get; set; }
        public string shift_late_time { get; set; }
        public string shift_half_time { get; set; }


        public string terminal_in { get; set; }
        public string terminal_out { get; set; }
        public bool active { get; set; }
        //  public virtual ManualAttendance manualAttendance { get; set; }


        public string departmentName { get; set; }
        public string funcationName { get; set; }
        public string regionName { get; set; }
        public string locationName { get; set; }
        public string designationName { get; set; }

        public int overtime { get; set; }
        public string overtime_status { get; set; }
        public string str_overtime { get; set; }

        public string overtime2 { get; set; }

        public string action { get; set; }
    }


    /*
    *This View Model is used to load finilized report of attendance
    *The View Model contains all attribute that will show on report page
    */
    public class ConsolidatedAttendanceLog
    {
        [Key]
        public int id { get; set; }
        public string date { get; set; }
        public DateTime dt_date { get; set; }
        public string time_in { get; set; }
        public string time_out { get; set; }
        public string status_in { get; set; }
        public string status_out { get; set; }
        public string final_remarks { get; set; }
        public string description { get; set; }
        public string employee_code { get; set; }
        public string employee_first_name { get; set; }
        public string employee_last_name { get; set; }
        public string employee_department_name { get; set; }
        public string employee_designation_name { get; set; }
        public string employee_campus_name { get; set; }
        public string terminal_in { get; set; }
        public string terminal_out { get; set; }
        public bool active { get; set; }
        //  public virtual ManualAttendance manualAttendance { get; set; }
        public string WhoMark { get; set; }
        public string overtime { get; set; }
        public string overtime_status { get; set; }
        public string hours { get; set; }
        public string action { get; set; }

    }

    public class ConsolidatedAttendanceArchiveLog
    {
        [Key]
        public int id { get; set; }
        public int ConsolidatedAttendanceId { get; set; }
        public string date { get; set; }
        public string time_in { get; set; }
        public string time_out { get; set; }
        public string status_in { get; set; }
        public string status_out { get; set; }
        public string final_remarks { get; set; }
        public string employee_code { get; set; }
        public string employee_first_name { get; set; }
        public string employee_last_name { get; set; }
        public string terminal_in { get; set; }
        public string terminal_out { get; set; }
        public bool active { get; set; }
        //  public virtual ManualAttendance manualAttendance { get; set; }
    }

    public class DepartmentAttendanceCountReport
    {
        [Key]
        public int EmployeeId { get; set; }
        public string EmployeeCode { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string DeptName { get; set; }
        public string DesgName { get; set; }
        public string LoctName { get; set; }
        public string RegnName { get; set; }
        public string FuncName { get; set; }

        public decimal PresentPercent { get; set; }
        public decimal LeavePercent { get; set; }
        public decimal AbsentPercent { get; set; }

        public decimal OFFPercent { get; set; }

        public decimal OnTimePercent { get; set; }
        public decimal LatePercent { get; set; }

        public decimal OutTimePercent { get; set; }
        public decimal EarlyPercent { get; set; }

        public bool DeptVisb { get; set; }
        public bool DesgVisb { get; set; }
        public bool LoctVisb { get; set; }

    }


    public class ConsolidatedAttendanceDepartmentWise
    {
        [Key]
        public int id { get; set; }
        public string date { get; set; }
        public string time_in { get; set; }
        public string time_out { get; set; }
        public string status_in { get; set; }
        public string status_out { get; set; }
        public string final_remarks { get; set; }
        public string employee_code { get; set; }
        public string employee_first_name { get; set; }
        public string employee_last_name { get; set; }
        public string terminal_in { get; set; }
        public string terminal_out { get; set; }
        public string function { get; set; }
        public string region { get; set; }
        public string department { get; set; }
        public string designation { get; set; }
        public string location { get; set; }
        public string reason { get; set; }
        public string WhoMark { get; set; }

    }

    public class ConsolidatedAttendanceDepartmentWiseExcelDownload
    {
        public string Date { get; set; }
        public string Employee_Code { get; set; }
        public string First_Name { get; set; }
        public string Last_Name { get; set; }
        public string Time_In { get; set; }
        public string Remarks_In { get; set; }
        public string Time_Out { get; set; }
        public string Remarks_Out { get; set; }
        public string Final_Remarks { get; set; }
        public string Device_In { get; set; }
        public string Device_Out { get; set; }
        public string Function { get; set; }
        public string Region { get; set; }
        public string Department { get; set; }
        public string Designation { get; set; }
        public string Location { get; set; }
    }

    public class ConsolidatedAttendanceExport
    {
        public string Date { get; set; }
        public string Employee_Number { get; set; }
        public string First_Name { get; set; }
        public string Last_Name { get; set; }
        public string employee_department_name { get; set; }
        public string employee_designation_name { get; set; }
        public string employee_campus_name { get; set; }
        public string Time_In { get; set; }
        public string Remarks_In { get; set; }
        public string Time_Out { get; set; }
        public string Remarks_Out { get; set; }
        public string Final_Remarks { get; set; }
        public string description { get; set; }
        public string Device_In { get; set; }
        public string Device_Out { get; set; }
    }
    public class HourlyAttendanceExport
    {
        public string Date { get; set; }
        public string Employee_Number { get; set; }
        public string First_Name { get; set; }
        public string Last_Name { get; set; }
        public string Time_In { get; set; }
        public string Remarks_In { get; set; }
        public string Time_Out { get; set; }
        public string Remarks_Out { get; set; }
        public string Hours { get; set; }

    }

    public class AttendanceSummaryExport
    {
        public string EmployeeCode{ get; set; }
        public string First_Name { get; set; }
        public string Last_Name { get; set; }
        public string Department { get; set; }
        public string Designation { get; set; }
        public string Institute { get; set; }
        public int Present { get; set; }
        public int Leave { get; set; }
        public int Absent { get; set; }
        public int OnTime { get; set; }
        public int Late { get; set; }
        public int EarlyGone { get; set; }

    }
    public class ConsolidatedAttendanceLogIntermediate
    {
        [Key]
        public int id { get; set; }
        public DateTime? date { get; set; }
        public DateTime? time_in { get; set; }
        public DateTime? time_out { get; set; }
        public string status_in { get; set; }
        public string status_out { get; set; }
        public string final_remarks { get; set; }
        public string description { get; set; }
        public string employee_code { get; set; }
        public string employee_first_name { get; set; }
        public string employee_last_name { get; set; }
        public string employee_department_name { get; set; }
        public string employee_designation_name { get; set; }
        public string employee_campus_name { get; set; }
        public string terminal_in { get; set; }
        public string terminal_out { get; set; }
        public bool active { get; set; }

        public Employee emp { get; set; }
        public BLL.ViewModels.Terminals tt { get; set; }
        //public virtual ManualAttendance obj { get; set; }
        public string WhoMark { get; set; }
        public int overtime { get; set; }
        public int overtime_status { get; set; }

    }
    public class HaTransitsUniqueAttendanceLog
    {
        [Key]
        public int HaTransitId { get; set; }
        public string Name { get; set; }
        public string DeviceName { get; set; }
        public string C_Unique { get; set; }
        public DateTime C_date { get; set; }
        public string C_Time { get; set; }
        public int L_Uid { get; set; }
        public bool active { get; set; }
        public int L_TID { get; set; }
    }

    public class ConsolidatedAttendanceExportOvertime
    {
        public string Date { get; set; }
        public string Employee_Number { get; set; }
        public string First_Name { get; set; }
        public string Last_Name { get; set; }
        public string Time_In { get; set; }

        public string Time_Out { get; set; }


        public string overtime { get; set; }
        public string overtime_status { get; set; }
    }

    //////////////////////// LEAVES REPORTS ////////////////////////////////

    public class LeavesReportByUserLog
    {
        [Key]
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public string UsersName { get; set; }
        public int LeaveTypeId { get; set; }
        public string LeaveTypeText { get; set; }
        public DateTime FromDate { get; set; }
        public string FromDateText { get; set; }
        public DateTime ToDate { get; set; }
        public string ToDateText { get; set; }
        public int DaysCount { get; set; }
        public int LeaveReasonId { get; set; }
        public string LeaveReasonText { get; set; }
        public string ReasonDetail { get; set; }
        public int ApproverId { get; set; }
        public string ApproverName { get; set; }
        public string ApproverDetail { get; set; }
        public int LeaveStatusId { get; set; }
        public string LeaveStatusText { get; set; }
        public string AttachmentFilePath { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreateDateTime { get; set; }
        public string CreateDateText { get; set; }
        public DateTime UpdateDateTime { get; set; }
        public string UpdateDateText { get; set; }

        public string actions { get; set; }
    }

    public class LeavesReportByUserLogIntermediate
    {
        [Key]
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public string UsersName { get; set; }
        public int LeaveTypeId { get; set; }
        public string LeaveTypeText { get; set; }
        public DateTime FromDate { get; set; }
        public string FromDateText { get; set; }
        public DateTime ToDate { get; set; }
        public string ToDateText { get; set; }
        public int DaysCount { get; set; }
        public int LeaveReasonId { get; set; }
        public string LeaveReasonText { get; set; }
        public string ReasonDetail { get; set; }
        public int ApproverId { get; set; }
        public string ApproverName { get; set; }
        public string ApproverDetail { get; set; }
        public int LeaveStatusId { get; set; }
        public string LeaveStatusText { get; set; }
        public string AttachmentFilePath { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreateDateTime { get; set; }
        public string CreateDateText { get; set; }
        public DateTime UpdateDateTime { get; set; }
        public string UpdateDateText { get; set; }
        public string actions { get; set; }
    }


    //////////////////////// LEAVES REPORTS ////////////////////////////////

    public class LeavesCountReportLog
    {
        [Key]
        public int id { get; set; }
        public int EmployeeId { get; set; }
        public string Employee_Code { get; set; }

        public int AvailedSickLeaves { get; set; }
        public int AvailedCasualLeaves { get; set; }
        public int AvailedAnnualLeaves { get; set; }
        public int AvailedOtherLeaves { get; set; }

        public int AvailedLeaveType01 { get; set; }
        public int AvailedLeaveType02 { get; set; }
        public int AvailedLeaveType03 { get; set; }
        public int AvailedLeaveType04 { get; set; }
        public int AvailedLeaveType05 { get; set; }
        public int AvailedLeaveType06 { get; set; }
        public int AvailedLeaveType07 { get; set; }
        public int AvailedLeaveType08 { get; set; }
        public int AvailedLeaveType09 { get; set; }
        public int AvailedLeaveType10 { get; set; }
        public int AvailedLeaveType11 { get; set; }

        public int AllocatedSickLeaves { get; set; }
        public int AllocatedCasualLeaves { get; set; }
        public int AllocatedAnnualLeaves { get; set; }
        public int AllocatedOtherLeaves { get; set; }

        public int AllocatedLeaveType01 { get; set; }
        public int AllocatedLeaveType02 { get; set; }
        public int AllocatedLeaveType03 { get; set; }
        public int AllocatedLeaveType04 { get; set; }
        public int AllocatedLeaveType05 { get; set; }
        public int AllocatedLeaveType06 { get; set; }
        public int AllocatedLeaveType07 { get; set; }
        public int AllocatedLeaveType08 { get; set; }
        public int AllocatedLeaveType09 { get; set; }
        public int AllocatedLeaveType10 { get; set; }
        public int AllocatedLeaveType11 { get; set; }
    }

    public class LeavesBalanceReportLog
    {
        [Key]
        public int id { get; set; }
        public int EmployeeId { get; set; }
        public string Employee_Code { get; set; }

        public int AvailedSickLeaves { get; set; }
        public int AvailedCasualLeaves { get; set; }
        public int AvailedAnnualLeaves { get; set; }
        public int AvailedOtherLeaves { get; set; }
        public int AvailedLeaveType01 { get; set; }
        public int AvailedLeaveType02 { get; set; }
        public int AvailedLeaveType03 { get; set; }
        public int AvailedLeaveType04 { get; set; }
        public int AvailedLeaveType05 { get; set; }
        public int AvailedLeaveType06 { get; set; }
        public int AvailedLeaveType07 { get; set; }
        public int AvailedLeaveType08 { get; set; }
        public int AvailedLeaveType09 { get; set; }
        public int AvailedLeaveType10 { get; set; }
        public int AvailedLeaveType11 { get; set; }

        public int AllocatedSickLeaves { get; set; }
        public int AllocatedCasualLeaves { get; set; }
        public int AllocatedAnnualLeaves { get; set; }
        public int AllocatedOtherLeaves { get; set; }
        public int AllocatedLeaveType01 { get; set; }
        public int AllocatedLeaveType02 { get; set; }
        public int AllocatedLeaveType03 { get; set; }
        public int AllocatedLeaveType04 { get; set; }
        public int AllocatedLeaveType05 { get; set; }
        public int AllocatedLeaveType06 { get; set; }
        public int AllocatedLeaveType07 { get; set; }
        public int AllocatedLeaveType08 { get; set; }
        public int AllocatedLeaveType09 { get; set; }
        public int AllocatedLeaveType10 { get; set; }
        public int AllocatedLeaveType11 { get; set; }
    }

    public class LeavesListCount
    {
        public int list_count { get; set; }
    }

    public class SearchEmployeeData
    {
        public int EmpID { get; set; }
        public string EmpCode { get; set; }
        public string EmpText { get; set; }
    }


    public class LeavesListReportLog
    {
        [Key]
        public int id { get; set; }
        public int EmployeeId { get; set; }
        public string Employee_Code { get; set; }
        public string First_Name { get; set; }
        public string Last_Name { get; set; }

        public int AvailedSickLeaves { get; set; }
        public int AvailedCasualLeaves { get; set; }
        public int AvailedAnnualLeaves { get; set; }
        public int AvailedOtherLeaves { get; set; }
        public int AvailedLeaveType01 { get; set; }
        public int AvailedLeaveType02 { get; set; }
        public int AvailedLeaveType03 { get; set; }
        public int AvailedLeaveType04 { get; set; }
        public int AvailedLeaveType05 { get; set; }
        public int AvailedLeaveType06 { get; set; }
        public int AvailedLeaveType07 { get; set; }
        public int AvailedLeaveType08 { get; set; }
        public int AvailedLeaveType09 { get; set; }
        public int AvailedLeaveType10 { get; set; }
        public int AvailedLeaveType11 { get; set; }

        public int AllocatedSickLeaves { get; set; }
        public int AllocatedCasualLeaves { get; set; }
        public int AllocatedAnnualLeaves { get; set; }
        public int AllocatedOtherLeaves { get; set; }
        public int AllocatedLeaveType01 { get; set; }
        public int AllocatedLeaveType02 { get; set; }
        public int AllocatedLeaveType03 { get; set; }
        public int AllocatedLeaveType04 { get; set; }
        public int AllocatedLeaveType05 { get; set; }
        public int AllocatedLeaveType06 { get; set; }
        public int AllocatedLeaveType07 { get; set; }
        public int AllocatedLeaveType08 { get; set; }
        public int AllocatedLeaveType09 { get; set; }
        public int AllocatedLeaveType10 { get; set; }
        public int AllocatedLeaveType11 { get; set; }

        public int TotalCount { get; set; }
        public string action { get; set; }
    }

    public class LeavesApplyStatusReportLog
    {
        [Key]
        public int id { get; set; }
        public int EmployeeId { get; set; }
        public string Employee_Code { get; set; }
        public string First_Name { get; set; }
        public string Last_Name { get; set; }

        public string From_Date { get; set; }
        public string To_Date { get; set; }
        public string Create_Date { get; set; }

        public int AvailedSickLeaves { get; set; }
        public int AvailedCasualLeaves { get; set; }
        public int AvailedAnnualLeaves { get; set; }
        public int AvailedOtherLeaves { get; set; }
        public int AvailedLeaveType01 { get; set; }
        public int AvailedLeaveType02 { get; set; }
        public int AvailedLeaveType03 { get; set; }
        public int AvailedLeaveType04 { get; set; }
        public int AvailedLeaveType05 { get; set; }
        public int AvailedLeaveType06 { get; set; }
        public int AvailedLeaveType07 { get; set; }
        public int AvailedLeaveType08 { get; set; }
        public int AvailedLeaveType09 { get; set; }
        public int AvailedLeaveType10 { get; set; }
        public int AvailedLeaveType11 { get; set; }

        public int AllocatedSickLeaves { get; set; }
        public int AllocatedCasualLeaves { get; set; }
        public int AllocatedAnnualLeaves { get; set; }
        public int AllocatedOtherLeaves { get; set; }
        public int AllocatedLeaveType01 { get; set; }
        public int AllocatedLeaveType02 { get; set; }
        public int AllocatedLeaveType03 { get; set; }
        public int AllocatedLeaveType04 { get; set; }
        public int AllocatedLeaveType05 { get; set; }
        public int AllocatedLeaveType06 { get; set; }
        public int AllocatedLeaveType07 { get; set; }
        public int AllocatedLeaveType08 { get; set; }
        public int AllocatedLeaveType09 { get; set; }
        public int AllocatedLeaveType10 { get; set; }
        public int AllocatedLeaveType11 { get; set; }

        public int TotalCount { get; set; }
        public string action { get; set; }
    }

    public class LeavesValidityReportLog
    {
        [Key]
        public int id { get; set; }
        public int EmployeeId { get; set; }
        public string Employee_Code { get; set; }
        public string First_Name { get; set; }
        public string Last_Name { get; set; }

        public string From_Date { get; set; }
        public string To_Date { get; set; }
        public string Create_Date { get; set; }

        public string ValidityText { get; set; }
        public string ValidityRemarks { get; set; }


        public int AvailedSickLeaves { get; set; }
        public int AvailedCasualLeaves { get; set; }
        public int AvailedAnnualLeaves { get; set; }
        public int AvailedOtherLeaves { get; set; }
        public int AvailedLeaveType01 { get; set; }
        public int AvailedLeaveType02 { get; set; }
        public int AvailedLeaveType03 { get; set; }
        public int AvailedLeaveType04 { get; set; }
        public int AvailedLeaveType05 { get; set; }
        public int AvailedLeaveType06 { get; set; }
        public int AvailedLeaveType07 { get; set; }
        public int AvailedLeaveType08 { get; set; }
        public int AvailedLeaveType09 { get; set; }
        public int AvailedLeaveType10 { get; set; }
        public int AvailedLeaveType11 { get; set; }

        public int AllocatedSickLeaves { get; set; }
        public int AllocatedCasualLeaves { get; set; }
        public int AllocatedAnnualLeaves { get; set; }
        public int AllocatedOtherLeaves { get; set; }
        public int AllocatedLeaveType01 { get; set; }
        public int AllocatedLeaveType02 { get; set; }
        public int AllocatedLeaveType03 { get; set; }
        public int AllocatedLeaveType04 { get; set; }
        public int AllocatedLeaveType05 { get; set; }
        public int AllocatedLeaveType06 { get; set; }
        public int AllocatedLeaveType07 { get; set; }
        public int AllocatedLeaveType08 { get; set; }
        public int AllocatedLeaveType09 { get; set; }
        public int AllocatedLeaveType10 { get; set; }
        public int AllocatedLeaveType11 { get; set; }

        public int TotalCount { get; set; }
        public string action { get; set; }
    }

  



    /////////////////////////////////////////////////////////////////////////

    public class LoanReportByMonthLogIntermediate
    {
        [Key]
        public int Id { get; set; }

        public int EmployeeId { get; set; }
        public string EmployeeCode { get; set; }
        public string EmployeeName { get; set; }

        public DateTime LoanAllocatedDate { get; set; }
        public string LoanAllocatedDateText { get; set; }

        public int LoanTypeId { get; set; }
        public string LoanTypeText { get; set; }
        public int LoanAmount { get; set; }
        public int InstallmentNumbers { get; set; }
        public int InstallmentAmount { get; set; }

        public int DeductableAmount { get; set; }
        public int BalanceAmount { get; set; }
        public int LoanStatusId { get; set; }
        public string LoanStatusText { get; set; }
        public string Remarks { get; set; }
        public string AttachmentFilePath { get; set; }

        public DateTime CreateDateTime { get; set; }
        public string CreateDateTimeText { get; set; }
        public DateTime UpdateDateTime { get; set; }
        public string UpdateDateTimeText { get; set; }

        public string actions { get; set; }

    }

    public class PayrollReportByMonthLogIntermediate
    {
        [Key]
        public int Id { get; set; }

        public int EmployeeId { get; set; }
        public string EmployeeCode { get; set; }
        public string EmployeeName { get; set; }
        public DateTime SalaryMonthYear { get; set; }
        public string SalaryMonthYearText { get; set; }

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
        public string PaymentModeText { get; set; }
        public int BankNameId { get; set; } //TABLE: BankNames
        public string BankNameText { get; set; }
        public string BankAccTitle { get; set; }
        public string BankAccNo { get; set; }

        public int AckStatusId { get; set; } //TABLE: PaymentStatusTypes
        public string AckStatusText { get; set; }
        public int PayStatusId { get; set; } //TABLE: PaymentStatusTypes
        public string PayStatusText { get; set; }

        public string UserHRComments { get; set; }
        public string AttachmentFilePath { get; set; }
        public string IsFirstMonthText { get; set; }

        public DateTime PaymentDateTime { get; set; }
        public string PaymentDateTimeText { get; set; }
        public DateTime CreateDateTime { get; set; }
        public string CreateDateTimeText { get; set; }
        public DateTime UpdateDateTime { get; set; }
        public string UpdateDateTimeText { get; set; }

        public string actions { get; set; }

    }

    public class MonthlyTimesheetAttendanceLogIntermediate
    {
        [Key]
        public int id { get; set; }
        public DateTime? date { get; set; }
        public DateTime? time_in { get; set; }
        public DateTime? time_out { get; set; }
        public string status_in { get; set; }
        public string status_out { get; set; }
        public string final_remarks { get; set; }
        public string description { get; set; }
        public string employee_code { get; set; }
        public string employee_first_name { get; set; }
        public string employee_last_name { get; set; }
        public string terminal_in { get; set; }
        public string terminal_out { get; set; }
        public bool active { get; set; }
        //  public virtual ManualAttendance manualAttendance { get; set; }

        //public int i_overtime { get; set; }
        //public int i_overtime_status { get; set; }

        public string departmentName { get; set; }
        public string funcationName { get; set; }
        public string regionName { get; set; }
        public string locationName { get; set; }
        public string designationName { get; set; }


        public int overtime { get; set; }
        public int overtime_status { get; set; }

        public string day { get; set; }
    }


    public class MonthlyEmployeeShiftLogIntermediate
    {
        [Key]
        public int id { get; set; }
        public DateTime? date { get; set; }
        public DateTime? time_in { get; set; }
        public DateTime? time_out { get; set; }
        public string status_in { get; set; }
        public string status_out { get; set; }
        public string final_remarks { get; set; }
        public string employee_code { get; set; }
        public string employee_first_name { get; set; }
        public string employee_last_name { get; set; }

        public string shift_name { get; set; }
        public DateTime dtshift_start_time { get; set; }
        public int shift_end_sec { get; set; }
        public int shift_late_sec { get; set; }
        public int shift_half_sec { get; set; }
        public string shift_start_time { get; set; }
        public string shift_end_time { get; set; }
        public string shift_late_time { get; set; }
        public string shift_half_time { get; set; }


        public string terminal_in { get; set; }
        public string terminal_out { get; set; }
        public bool active { get; set; }
        //  public virtual ManualAttendance manualAttendance { get; set; }

        //public int i_overtime { get; set; }
        //public int i_overtime_status { get; set; }

        public string departmentName { get; set; }
        public string funcationName { get; set; }
        public string regionName { get; set; }
        public string locationName { get; set; }
        public string designationName { get; set; }


        public int overtime { get; set; }
        public int overtime_status { get; set; }
    }

    public class ConsolidatedAttendanceLogIntermediateDepartmentWise
    {
        [Key]
        public int id { get; set; }
        public DateTime? date { get; set; }
        public DateTime? time_in { get; set; }
        public DateTime? time_out { get; set; }
        public string status_in { get; set; }
        public string status_out { get; set; }
        public string final_remarks { get; set; }
        public string employee_code { get; set; }
        public string employee_first_name { get; set; }
        public string employee_last_name { get; set; }
        public string terminal_in { get; set; }
        public string terminal_out { get; set; }
        public string function { get; set; }
        public string region { get; set; }
        public string department { get; set; }
        public string designation { get; set; }
        public string location { get; set; }
        public string reason { get; set; }
        public string WhoMark { get; set; }

    }
    public class PaAttendanceLogIntermediate
    {
        [Key]
        public int id { get; set; }
        public DateTime? date { get; set; }
        public string employee_code { get; set; }
        public string employee_first_name { get; set; }
        public string employee_last_name { get; set; }
        public string final_remarks { get; set; }
        public bool active { get; set; }


    }
}
