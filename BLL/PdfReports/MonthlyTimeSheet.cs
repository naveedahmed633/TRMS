using System;
using System.Collections.Generic;
using System.Linq;
using DLL.Models;
using System.Globalization;
using System.Web.UI.WebControls;
using DLL;
using System.Globalization;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Text;
using System.IO;

namespace BLL.PdfReports
{
    public class MonthlyTimeSheetLog
    {
        internal double totalHours;

        public string date { get; set; }
        public string timeIn { get; set; }
        public string remarksIn { get; set; }
        public string timeOut { get; set; }
        public string remarksOut { get; set; }
        public string finalRemarks { get; set; }
        public string description { get; set; }
        public string terminalIn { get; set; }
        public string terminalOut { get; set; }
        public bool hasManualAttendance { get; set; }

        public int overtime { get; set; }

        public double totalShfit_Mins { get; set; }
        public double totalShfit_Hour { get; set; }
        public double GrandTotal_Hour { get; set; }
        public double totalLateHours { get; set; }
        public double totalLateMins { get; set; }

        public double totalOvertimeHour { get; set; }
        public double totalOvertimeMins { get; set; }

        public string inPhoto { get; set; }
        public string outPhoto { get; set; }

        public string overtime2 { get; set; }
        public string overtime_status { get; set; }
        public string day { get; set; }
        public string status { get; set; }
        public string remarks { get; set; }

    }

    public class MonthlyEmpShiftLog
    {
        public int id { get; set; }
        public string date { get; set; }

        public string employee_code { get; set; }
        public string employee_first_name { get; set; }
        public string employee_last_name { get; set; }

        public string shift_name { get; set; }
        public string shift_start_time { get; set; }
        public string shift_end_time { get; set; }
        public string shift_late_time { get; set; }
        public string shift_half_time { get; set; }



        public DateTime dtshift_start_time { get; set; }
        public int shift_end_sec { get; set; }
        public int shift_late_sec { get; set; }
        public int shift_half_sec { get; set; }



        public string timeIn { get; set; }
        public string remarksIn { get; set; }
        public string timeOut { get; set; }
        public string remarksOut { get; set; }
        public string finalRemarks { get; set; }
        public string terminalIn { get; set; }
        public string terminalOut { get; set; }
        public bool hasManualAttendance { get; set; }

        public int overtime { get; set; }

        public double totalShfit_Hour { get; set; }
        public double GrandTotal_Hour { get; set; }
        public double totalLateHours { get; set; }
        public double totalOvertimeHour { get; set; }
        public string inPhoto { get; set; }
        public string outPhoto { get; set; }

        public string overtime2 { get; set; }
        public string overtime_status { get; set; }

    }

    public class MonthlyPunchedPhotoLog
    {
        public string regionID { get; set; }
        public string eCode { get; set; }
        public string eFirstName { get; set; }
        public string eLastName { get; set; }
        public string date { get; set; }
        public string timeInOut { get; set; }
        public string date_time_in_out { get; set; }
        public string photoName { get; set; }
        public string photoRelativePath { get; set; }
        public string photoFullPath { get; set; }
        public string terminalId { get; set; }
        public string terminalName { get; set; }
        public string actions { get; set; }

    }


    public class TestClass
    {

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
    }

    public class MonthlyTimeSheetDataAll
    {
        public string year { get; set; }
        public string month { get; set; }
        public string employeeName { get; set; }
        public string employeeCode { get; set; }
        public string location { get; set; }
        public string totalPresent { get; set; }
        public string totalAbsent { get; set; }
        public string totalLate { get; set; }
        public string totalEarlyOut { get; set; }
        public string totalTime { get; set; }
    }

    public class MonthlyTimeSheetData
    {
        public string year { get; set; }
        public string month { get; set; }
        public string employeeName { get; set; }
        public string employeeCode { get; set; }


        public string realPhoto { get; set; }

        public MonthlyTimeSheetLog[] logs { get; set; }

        public string totalPresent { get; set; }
        public string totalLate { get; set; }
        public string totalAbsent { get; set; }
        public string totalEarlyOut { get; set; }
        public string MissPunch { get; set; }
        public string totalLeave { get; set; }

        public string totalDays { get; set; }
        public string totalOvertime { get; set; }
        public string FinalOvertime { get; set; }

        public string ApprovedOvertime { get; set; }

        public string DiscartOvertime { get; set; }

        public string UnapprovedOvertime { get; set; }
    }

    public class CustomRangeTimeSheetData
    {
        public string toYear { get; set; }
        public string toMonth { get; set; }
        public string toDay { get; set; }
        public string fromYear { get; set; }
        public string fromMonth { get; set; }
        public string fromDay { get; set; }
        public string employeeName { get; set; }
        public string employeeCode { get; set; }


        public string realPhoto { get; set; }

        public MonthlyTimeSheetLog[] logs { get; set; }

        public string totalPresent { get; set; }
        public string totalLate { get; set; }
        public string totalAbsent { get; set; }
        public string totalEarlyOut { get; set; }
        public string MissPunch { get; set; }
        public string totalLeave { get; set; }

        public string totalDays { get; set; }
        public string totalOvertime { get; set; }
        public string FinalOvertime { get; set; }

        public string ApprovedOvertime { get; set; }

        public string DiscartOvertime { get; set; }

        public string UnapprovedOvertime { get; set; }
    }

    public class MonthlyEmpShiftData
    {
        public string year { get; set; }
        public string month { get; set; }
        public string employeeName { get; set; }
        public string employeeCode { get; set; }

        public string realPhoto { get; set; }

        public MonthlyEmpShiftLog[] logs { get; set; }

        public string totalPresent { get; set; }
        public string totalLate { get; set; }
        public string totalAbsent { get; set; }
        public string totalEarlyOut { get; set; }
        public string totalLeave { get; set; }

        public string totalDays { get; set; }
        public string totalOvertime { get; set; }
        public string FinalOvertime { get; set; }

        public string ApprovedOvertime { get; set; }

        public string DiscartOvertime { get; set; }

        public string UnapprovedOvertime { get; set; }
    }

    public class MonthlyPunchedPhotoData
    {
        public string year { get; set; }
        public string month { get; set; }
        public string employeeName { get; set; }
        public string employeeCode { get; set; }
        public byte[] originalPhoto { get; set; }//UNIS
        public string realPhoto { get; set; }//Content Folder

        public MonthlyPunchedPhotoLog[] logs { get; set; }

    }


    public class MonthlyDepartmentalTimeSheetData
    {
        public string year { get; set; }
        public string month { get; set; }
        public string employeeName { get; set; }
        public string employeeCode { get; set; }
        public string departmentName { get; set; }
        public string functionName { get; set; }
        public string regionName { get; set; }
        public string locationName { get; set; }
        public string designationName { get; set; }

        public MonthlyTimeSheetLog[] logs { get; set; }

        public string totalPresent { get; set; }
        public string totalLate { get; set; }
        public string totalAbsent { get; set; }
        public string totalEarlyOut { get; set; }
        public string totalLeave { get; set; }

        public string totalDays { get; set; }
        public string totalOvertime { get; set; }
        public string FinalOvertime { get; set; }

        public string ApprovedOvertime { get; set; }

        public string DiscartOvertime { get; set; }

        public string UnapprovedOvertime { get; set; }


    }



    public class EmployeeEvaluationPDF
    {

        public string GeneratePDF()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("<table width='762'><tbody><tr><td width='372'>");
            sb.Append("<p>Employee:<u>" + sb + "</u></p>");
            sb.Append("<p>Review Period: _____________________________________________________</p>");
            sb.Append("<p>1) Evaluate performance by circling the appropriate response:</p><p>&nbsp;&nbsp;&nbsp;&nbsp; 1 = substandard, needs constant supervision</p><p>&nbsp;&nbsp;&nbsp;&nbsp; 2 = below average, needs improvement</p><p>&nbsp;&nbsp;&nbsp;&nbsp; 3 = average, satisfactorily meets criteria</p><p>&nbsp;&nbsp;&nbsp;&nbsp; 4 =above average, exceeds criteria</p><p>&nbsp;&nbsp;&nbsp;&nbsp; 5 = exemplary, deserving of unusual recognition</p><p>2) Enter comments as necessary.</p><p>3) Set goals for the next review period</p><p>4) Complete the back side (supervisor only).</p>");
            sb.Append("</td><td width='30'></td><td width='360>");
            sb.Append("<p>Position: ___________________________________________________</p>");
            sb.Append("<p>Requirements / attributes: _______________________________</p>");
            sb.Append("<p>______________________________________________________________</p>");
            sb.Append("<p>______________________________________________________________</p>");
            sb.Append("<p>Primary responsibilities: _________________________________</p>");
            sb.Append("<p>______________________________________________________________</p>");
            sb.Append("<p>______________________________________________________________</p>");
            sb.Append("<p>Secondary responsibilities: ______________________________</p>");
            sb.Append("<p>______________________________________________________________</p>");
            sb.Append("<p>______________________________________________________________</p>");
            sb.Append("<p>Career path: _______________________________________________</p>");
            sb.Append("<p>______________________________________________________________</p>");
            sb.Append("</td>");
            sb.Append("</tr></tbody></table>");




            return sb.ToString();
        }
    }

    public class LeavesTypesTitles
    {
        public string getLeavesTypesTitles()
        {
            string strLeavesTypesTitles = "";
            List<LeaveType> leavesTypes = new List<LeaveType>();

            using (Context db = new Context())
            {
                leavesTypes = db.leave_type.Where(o => o.Id > 0).ToList();
                if (leavesTypes != null && leavesTypes.Count > 0)
                {
                    foreach (var l in leavesTypes)
                    {
                        strLeavesTypesTitles += l.LeaveTypeText + ",";
                    }

                    strLeavesTypesTitles = strLeavesTypesTitles.TrimEnd(',');
                }
            }

            return strLeavesTypesTitles;
        }

        public string getLeavesTypesCountsStatus()
        {
            string strLeavesTypesCS = "";
            List<LeaveType> leavesTypes = new List<LeaveType>();

            using (Context db = new Context())
            {
                leavesTypes = db.leave_type.Where(o => o.Id > 0).ToList();
                if (leavesTypes != null && leavesTypes.Count > 0)
                {
                    foreach (var l in leavesTypes)
                    {
                        strLeavesTypesCS += l.LeaveDefaultCount + "-" + l.LeaveMaxCount + "-" + l.IsActive + ",";
                    }

                    strLeavesTypesCS = strLeavesTypesCS.TrimEnd(',');
                }
            }

            return strLeavesTypesCS;
        }


    }

    public class MonthlyTimeSheet
    {


        private string userRole;
        private string userEmployeeCode;

        public double totalShfit_Hour { get; private set; }

        public static Shift getShift(Employee employee, DateTime? attendanceDate, Context db)
        {
            // if the employee group is null, it means that the employee lies in the general group.
            if (employee.Group == null || employee.Group.follows_general_calendar)
            {
                Shift toReturn = null;

                int year = attendanceDate.Value.Year;
                var calendar = db.general_calender.Where(m => m.year.Equals(year)).FirstOrDefault();

                if (calendar == null)
                {
                    Console.WriteLine("General calendar for year " + year + " does not exist.");

                    return null;
                }

                else
                {

                    if (employee.Group != null)
                    {
                        DateTime? date = attendanceDate;
                        // 1)
                        var manualShiftAssingned = db.manual_group_shift_assigned.Where(m =>
                            m.date.Value.Equals(date.Value) &&
                            m.Employee.EmployeeId.Equals(employee.EmployeeId) &&
                            m.Group.GroupId.Equals(employee.Group.GroupId))
                            .FirstOrDefault();

                        if (manualShiftAssingned != null)
                        {
                            toReturn = manualShiftAssingned.Shift;
                        }
                    }
                    // FOR GENERAL SHIFT EMPLOYEES:
                    // 1) first check for a general calendar override.
                    // 2) then check for day shift
                    // 3) then check for general shift.

                    //IR added 'active' condition
                    var generalCalendarOverride = calendar.calendarOverrides.Where(m => m.active && m.date.Value.Equals(attendanceDate.Value.Date)).FirstOrDefault();

                    // 1)
                    if (generalCalendarOverride != null)
                    {
                        Shift forceLoad = generalCalendarOverride.Shift;

                        toReturn = generalCalendarOverride.Shift;
                    }
                    else
                    {
                        // 2) && 3)
                        string day = attendanceDate.Value.DayOfWeek.ToString();
                        switch (day)
                        {
                            case "Monday":
                                toReturn = (calendar.Shift1 != null) ? calendar.Shift1 : calendar.generalShift;
                                break;
                            case "Tuesday":
                                toReturn = (calendar.Shift2 != null) ? calendar.Shift2 : calendar.generalShift;
                                break;
                            case "Wednesday":
                                toReturn = (calendar.Shift3 != null) ? calendar.Shift3 : calendar.generalShift;
                                break;
                            case "Thursday":
                                toReturn = (calendar.Shift4 != null) ? calendar.Shift4 : calendar.generalShift;
                                break;
                            case "Friday":
                                toReturn = (calendar.Shift5 != null) ? calendar.Shift5 : calendar.generalShift;
                                break;
                            case "Saturday":
                                toReturn = (calendar.Shift6 != null) ? calendar.Shift6 : calendar.generalShift;
                                break;
                            case "Sunday":
                                toReturn = (calendar.Shift != null) ? calendar.Shift : calendar.generalShift;
                                break;
                        }
                    }

                }

                if (toReturn == null)
                {
                    Console.WriteLine("Cannot find shift for employee " + employee.employee_code + " on date." + attendanceDate);

                }

                return toReturn;
            }
            else
            {
                Shift toReturn = null;

                DateTime? date = attendanceDate;

                // FOR GROUP EMPLOYEES:
                // 1) first check for manual shift assigned
                // 2) then check for group calendar overrides
                // 3) then check for group calendar day shift
                // 4) then check for group calendar general shift


                // 1)
                var manualShiftAssingned = db.manual_group_shift_assigned.Where(m =>
                    m.date.Value.Equals(date.Value) &&
                    m.Employee.EmployeeId.Equals(employee.EmployeeId) &&
                    m.Group.GroupId.Equals(employee.Group.GroupId))
                    .FirstOrDefault();

                if (manualShiftAssingned != null)
                {
                    toReturn = manualShiftAssingned.Shift;
                }
                else
                {
                    // 2) 
                    var groupOverride = db.group_calendar_overrides.Where(m =>
                        m.active &&
                        m.date.Value.Equals(date.Value) &&
                        m.GroupCalendar.Group.GroupId.Equals(employee.Group.GroupId))
                        .FirstOrDefault();

                    if (groupOverride != null)
                    {
                        toReturn = groupOverride.Shift;
                    }
                    else
                    {
                        // 3) & 4)
                        string day = attendanceDate.Value.DayOfWeek.ToString();

                        var groupGeneralcalendar = db.group_calendar.Where(m =>
                            m.active &&
                            m.year.Equals(attendanceDate.Value.Year) &&
                            m.Group.GroupId.Equals(employee.Group.GroupId))
                            .FirstOrDefault();

                        if (groupGeneralcalendar == null)
                        {
                            Console.WriteLine("Calendar for group " + employee.Group.GroupId + ", for year " + attendanceDate.Value.Year + " does not exist.");
                            return null;
                        }
                        switch (day)
                        {
                            case "Monday":
                                toReturn = (groupGeneralcalendar.monday != null) ? groupGeneralcalendar.monday : groupGeneralcalendar.generalShift;
                                break;
                            case "Tuesday":
                                toReturn = (groupGeneralcalendar.tuesday != null) ? groupGeneralcalendar.tuesday : groupGeneralcalendar.generalShift;
                                break;
                            case "Wednesday":
                                toReturn = (groupGeneralcalendar.wednesday != null) ? groupGeneralcalendar.wednesday : groupGeneralcalendar.generalShift;
                                break;
                            case "Thursday":
                                toReturn = (groupGeneralcalendar.thursday != null) ? groupGeneralcalendar.thursday : groupGeneralcalendar.generalShift;
                                break;
                            case "Friday":
                                toReturn = (groupGeneralcalendar.friday != null) ? groupGeneralcalendar.friday : groupGeneralcalendar.generalShift;
                                break;
                            case "Saturday":
                                toReturn = (groupGeneralcalendar.saturday != null) ? groupGeneralcalendar.saturday : groupGeneralcalendar.generalShift;
                                break;
                            case "Sunday":
                                toReturn = (groupGeneralcalendar.sunday != null) ? groupGeneralcalendar.sunday : groupGeneralcalendar.generalShift;
                                break;
                        }
                    }

                }

                if (toReturn == null)
                {
                    Console.WriteLine("Cannot find shift for employee " + employee.employee_code + ", of group " + employee.Group.GroupId + ", on date." + attendanceDate);
                }
                return toReturn;
            }
        }
        public int GetEmpId(string empCode)
        {
            int EmpId = 0;
            using (Context db = new Context())
            {
                EmpId = db.employee.Where(o => o.employee_code == empCode).FirstOrDefault().EmployeeId;
            }
            return EmpId;
        }


        public MonthlyTimeSheet()
        {
            userEmployeeCode = HttpContext.Current.User.Identity.Name;

            using (var db = new Context())
            {
                Employee emp = db.employee.Where(m =>
                    m.active &&
                    m.employee_code.Equals(userEmployeeCode)).FirstOrDefault();

                if (emp != null)
                {
                    userRole = emp.access_group.name;
                }
                else
                {
                    userRole = "unAuthorised";
                }

            }
        }

        private bool isAuthorised(int employeeId)
        {
            using (var db = new Context())
            {
                Employee requestedEmployee = db.employee.Where(m =>
                    m.EmployeeId.Equals(employeeId) &&
                    m.active).FirstOrDefault();

                if (requestedEmployee == null)
                    return false;

                switch (userRole)
                {
                    case DLL.Commons.Roles.ROLE_HR:
                        // HR can view all the reports.
                        return true;
                    case DLL.Commons.Roles.ROLE_REPORT:
                        // report can view all the reports.
                        return true;
                    case DLL.Commons.Roles.ROLE_EMP:
                        // EMP can only view his report, therefore
                        // only return true if the current session
                        // has the same employee code as the requested
                        // employee code.
                        return requestedEmployee.employee_code.Equals(userEmployeeCode);
                    case DLL.Commons.Roles.ROLE_LM:
                        // Line manager can view the reports for the employees
                        // in his group.

                        Employee lineManager = db.employee.Where(m =>
                            m.active &&
                            m.employee_code.Equals(userEmployeeCode)).FirstOrDefault();

                        return
                            (lineManager.Group != null &&
                            requestedEmployee.Group != null &&
                            lineManager.Group.GroupId == requestedEmployee.Group.GroupId);
                    case DLL.Commons.Roles.ROLE_SLM:
                        // SLM can view his own reports.
                        if (requestedEmployee.employee_code.Equals(userEmployeeCode))
                            return true;


                        // SLM can also view the reports for the employees, 
                        // that are tagged under him.
                        int[] empIds = { };


                        empIds = db.super_line_manager_tagging.Where(m =>
                            m.superLineManager.employee_code.Equals(userEmployeeCode) &&
                            m.taggedEmployee.active).Select(p => p.taggedEmployee.EmployeeId).ToArray();



                        return (empIds != null && empIds.Length != 0 && empIds.Contains(employeeId));

                    default:
                        return false;

                }
            }
        }

        public CustomRangeTimeSheetData getReportAlt(int employeeID, string from, string to)
        {
            int leavesCount = 0;
            CustomRangeTimeSheetData toReturn = null;


            if (!isAuthorised(employeeID))
                return toReturn;

            using (var db = new Context())
            {
                toReturn = new CustomRangeTimeSheetData();

                Employee emp = db.employee.Find(employeeID);

                if (emp == null)
                    return null;

                toReturn.employeeCode = emp.employee_code;
                toReturn.employeeName = emp.first_name + " " + emp.last_name;

                DateTime fromDate = DateTime.ParseExact(from, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                toReturn.fromYear = fromDate.ToString("yyyy");
                toReturn.fromMonth = fromDate.ToString("MM");
                toReturn.fromDay = fromDate.ToString("dd");

                DateTime toDate = DateTime.ParseExact(to, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                toReturn.toYear = toDate.ToString("yyyy");
                toReturn.toMonth = toDate.ToString("MM");
                toReturn.toDay = toDate.ToString("dd");

                ConsolidatedAttendance[] attendanceLogs = db.consolidated_attendance.Where(m =>
                    m.employee.EmployeeId.Equals(emp.EmployeeId) &&
                                m.date >= fromDate &&
                                m.date <= toDate)
                    .OrderBy(m => m.date)
                    .ToArray();

                List<LeaveApplication> lstLeaveApps = db.leave_application.Where(m => m.IsActive &&
                  m.EmployeeId.Equals(emp.EmployeeId) &&
                  (m.FromDate >= fromDate && m.ToDate <= toDate)).ToList();
                if (lstLeaveApps != null && lstLeaveApps.Count > 0)
                {
                    foreach (var l in lstLeaveApps)
                    {
                        leavesCount += l.DaysCount;
                    }
                }

                List<MonthlyTimeSheetLog> tempLogs = new List<MonthlyTimeSheetLog>();
                string newfinalremarks = "";
                string OvertimeStatusNew = "";
                string OvertimeNew = "";
                TimeSpan time;
                string startTime2 = "", shiftEnd3 = ""; string timein = ""; string timeout = "";
                string hours = ""; string hours1 = ""; string halfhours1 = ""; TimeSpan duration; TimeSpan duration1; TimeSpan half;
                string halfhours = "";

                double totalShfit_Hour = 0;
                double GrandTotal_Hour = 0;
                double totalOvertimeHour = 0;
                double totalLateHours = 0;

                foreach (ConsolidatedAttendance log in attendanceLogs)
                {
                    newfinalremarks = log.final_remarks;
                    if (log.final_remarks == "PLO" || log.final_remarks == "PLE" || log.final_remarks == "PLO" || log.final_remarks == "POE" || log.final_remarks == "PLO") { newfinalremarks = "L&E"; }
                    if (log.final_remarks == "PLM" || log.final_remarks == "PME" || log.final_remarks == "POM") { newfinalremarks = "MissPunch"; }

                    int empId = Convert.ToInt32(employeeID);
                    var getGroupId = db.employee.Where(e => e.EmployeeId == empId).FirstOrDefault();
                    if (getGroupId != null)
                    {
                        if (getGroupId.Group != null)
                        {
                            if (getGroupId.Group.follows_general_calendar)
                            {
                                int gC = db.general_calender.Where(s => s.year == DateTime.Now.Year).FirstOrDefault().Shift1.ShiftId;
                                var test = db.shift.Where(s => s.ShiftId == gC).FirstOrDefault();
                                startTime2 = test.start_time.ToString("hh:mm tt");
                                shiftEnd3 = test.start_time.AddSeconds(test.shift_end).AddMinutes(30).ToString("hh:mm tt");
                                //string start = fromForm.start_time;
                                //string end = fromForm.shift_end;
                                duration = DateTime.Parse(shiftEnd3).Subtract(DateTime.Parse(startTime2));
                                hours = duration.ToString(@"hh\:mm\:ss");
                                half = new TimeSpan(duration.Ticks / 2);
                                halfhours = half.ToString(@"hh\:mm\:ss");
                                //int aa = int.Parse(hours);
                            }
                        }
                    }
                    if (log.time_in != null)
                    {
                        timein = (log.time_in.HasValue) ? log.time_in.Value.ToString("hh:mm tt") : "";
                        timeout = (log.time_out.HasValue) ? log.time_out.Value.ToString("hh:mm tt") : "";

                        if (timein == "" || timeout == "")
                        { }
                        else
                        {
                            try
                            {
                                duration1 = DateTime.Parse(timeout).Subtract(DateTime.Parse(timein));
                                hours1 = duration1.ToString(@"hh\:mm\:ss");
                                if (log.final_remarks == "PO")
                                {
                                    if (DateTime.Parse(hours1) < DateTime.Parse(hours) && DateTime.Parse(hours1) > DateTime.Parse(halfhours))
                                    {
                                        newfinalremarks = "L&E";

                                    }
                                }
                                if (DateTime.Parse(hours1) < DateTime.Parse(halfhours))
                                {
                                    newfinalremarks = "HD";
                                }
                            }
                            catch (Exception ex)
                            {

                            }
                        }
                        //duration1 = DateTime.Parse(timeout).Subtract(DateTime.Parse(timein));
                        //hours1 = duration1.ToString(@"hh\:mm\:ss");
                        //int a = int.Parse(hours1);
                        //int b = a / 2;
                        //if (log.final_remarks == "PO")
                        //{
                        //    if (DateTime.Parse(hours1) <= DateTime.Parse(hours))
                        //    {
                        //        newfinalremarks = "L&E";

                        //    }
                        //}
                    }

                    for (int i = 0; i < lstLeaveApps.Count(); i++)
                    {
                        if (log.date >= lstLeaveApps[i].FromDate && log.date <= lstLeaveApps[i].ToDate && lstLeaveApps[i].LeaveStatusId == 2 && lstLeaveApps[i].LeaveTypeId == 1) { newfinalremarks = "MeL"; }
                        else if (log.date >= lstLeaveApps[i].FromDate && log.date <= lstLeaveApps[i].ToDate && lstLeaveApps[i].LeaveStatusId == 2 && lstLeaveApps[i].LeaveTypeId == 2) { newfinalremarks = "CL"; }
                        else if (log.date >= lstLeaveApps[i].FromDate && log.date <= lstLeaveApps[i].ToDate && lstLeaveApps[i].LeaveStatusId == 2 && lstLeaveApps[i].LeaveTypeId == 3) { newfinalremarks = "EL"; }
                        else if (log.date >= lstLeaveApps[i].FromDate && log.date <= lstLeaveApps[i].ToDate && lstLeaveApps[i].LeaveStatusId == 2 && lstLeaveApps[i].LeaveTypeId == 4) { newfinalremarks = "E.PL"; }
                        else if (log.date >= lstLeaveApps[i].FromDate && log.date <= lstLeaveApps[i].ToDate && lstLeaveApps[i].LeaveStatusId == 2 && lstLeaveApps[i].LeaveTypeId == 5) { newfinalremarks = "MaL"; }
                        else if (log.date >= lstLeaveApps[i].FromDate && log.date <= lstLeaveApps[i].ToDate && lstLeaveApps[i].LeaveStatusId == 2 && lstLeaveApps[i].LeaveTypeId == 6) { newfinalremarks = "TL"; }
                        else if (log.date >= lstLeaveApps[i].FromDate && log.date <= lstLeaveApps[i].ToDate && lstLeaveApps[i].LeaveStatusId == 2 && lstLeaveApps[i].LeaveTypeId == 7) { newfinalremarks = "VL"; }
                        else if (log.date >= lstLeaveApps[i].FromDate && log.date <= lstLeaveApps[i].ToDate && lstLeaveApps[i].LeaveStatusId == 2 && lstLeaveApps[i].LeaveTypeId == 8) { newfinalremarks = "MeetL"; }
                        else if (log.date >= lstLeaveApps[i].FromDate && log.date <= lstLeaveApps[i].ToDate && lstLeaveApps[i].LeaveStatusId == 2 && lstLeaveApps[i].LeaveTypeId == 9) { newfinalremarks = "HL"; }
                        else if (log.date >= lstLeaveApps[i].FromDate && log.date <= lstLeaveApps[i].ToDate && lstLeaveApps[i].LeaveStatusId == 2 && lstLeaveApps[i].LeaveTypeId == 17) { newfinalremarks = "PO*"; }
                        else if (log.date >= lstLeaveApps[i].FromDate && log.date <= lstLeaveApps[i].ToDate && lstLeaveApps[i].LeaveStatusId == 2 && lstLeaveApps[i].LeaveTypeId == 18) { newfinalremarks = "PLO*"; }
                        else if (log.date >= lstLeaveApps[i].FromDate && log.date <= lstLeaveApps[i].ToDate && lstLeaveApps[i].LeaveStatusId == 2 && lstLeaveApps[i].LeaveTypeId == 19) { newfinalremarks = "PLE*"; }
                        else if (log.date >= lstLeaveApps[i].FromDate && log.date <= lstLeaveApps[i].ToDate && lstLeaveApps[i].LeaveStatusId == 2 && lstLeaveApps[i].LeaveTypeId == 20) { newfinalremarks = "POE*"; }
                        else if (log.date >= lstLeaveApps[i].FromDate && log.date <= lstLeaveApps[i].ToDate && lstLeaveApps[i].LeaveStatusId == 2 && lstLeaveApps[i].LeaveTypeId == 21) { newfinalremarks = "OFF*"; }
                        else if (log.date >= lstLeaveApps[i].FromDate && log.date <= lstLeaveApps[i].ToDate && lstLeaveApps[i].LeaveStatusId == 2 && lstLeaveApps[i].LeaveTypeId == 22) { newfinalremarks = "OV*"; }
                        else if (log.date >= lstLeaveApps[i].FromDate && log.date <= lstLeaveApps[i].ToDate && lstLeaveApps[i].LeaveStatusId == 2 && lstLeaveApps[i].LeaveTypeId == 23) { newfinalremarks = "OM*"; }
                        else if (log.date >= lstLeaveApps[i].FromDate && log.date <= lstLeaveApps[i].ToDate && lstLeaveApps[i].LeaveStatusId == 2 && lstLeaveApps[i].LeaveTypeId == 24) { newfinalremarks = "OT*"; }

                    }


                    if (log.overtime < 0)
                    {
                        time = TimeSpan.FromSeconds(log.overtime);
                        OvertimeNew = "-" + time.ToString(@"hh\:mm\:ss");
                    }
                    else
                    {
                        time = TimeSpan.FromSeconds(log.overtime);
                        OvertimeNew = time.ToString(@"hh\:mm\:ss");
                    }


                    if (log.overtime_status == 1) { OvertimeStatusNew = "Unapproved"; }
                    else if (log.overtime_status == 2) { OvertimeStatusNew = "Approved"; }
                    else if (log.overtime_status == 3) { OvertimeStatusNew = "Discard"; }

                    var empModel = db.employee.Where(e => e.EmployeeId == employeeID).FirstOrDefault();

                    var shift = getShift(empModel, log.date, db);
                    TimeSpan totalShiftHours = TimeSpan.FromSeconds(shift.shift_end);

                    totalShfit_Hour += totalShiftHours.TotalHours;


                    string ShiftEndTime = shift.start_time.AddSeconds(shift.shift_end).ToString("HH:mm");

                    if (log.time_in != null && log.time_out != null)
                    {
                        string LateHour = DateTime.Parse(log.time_in.Value.ToString("HH:mm")).Subtract(DateTime.Parse(shift.start_time.ToString("HH:mm"))).ToString();

                        if (!LateHour.Contains("-"))
                        {
                            totalLateHours += TimeSpan.Parse(LateHour).TotalHours;
                        }
                        string GrandTotal = "";
                        if (LateHour.Contains("-"))
                        {
                            LateHour = "00:00:00";
                        }


                        string OverTime = DateTime.Parse(log.time_out.Value.ToString("HH:mm")).Subtract(DateTime.Parse(ShiftEndTime)).ToString();
                        if (!OverTime.Contains("-"))
                        {
                            totalOvertimeHour += TimeSpan.Parse(OverTime).TotalHours;
                        }


                        string Total = DateTime.Parse(totalShiftHours.ToString()).Subtract(DateTime.Parse(LateHour)).ToString();
                        if (Total.Contains("-"))
                        {
                            Total = Total.Replace("-", "");
                        }
                        if (OverTime.Contains("-"))
                        {

                            totalOvertimeHour += TimeSpan.Parse(OverTime).TotalHours;

                        }
                        GrandTotal = TimeTune.Reports.getWorkHourDurationAttendance(employeeID.ToString(), log.date.Value.Date.ToString("dd-MM-yyyy"), log.date.Value.Date.ToString("dd-MM-yyyy"));
                        GrandTotal_Hour += TimeSpan.Parse(GrandTotal).TotalHours;

                    }

                    tempLogs.Add(new MonthlyTimeSheetLog()
                    {
                        date = log.date.Value.ToString("dd-MM-yyyy"),
                        day = log.date.Value.ToString("dddd"),
                        timeIn = (log.time_in.HasValue) ? log.time_in.Value.ToString("hh:mm tt") : "",
                        remarksIn = log.status_in,
                        timeOut = (log.time_out.HasValue) ? log.time_out.Value.ToString("hh:mm tt") : "",
                        remarksOut = log.status_out,
                        //finalRemarks = log.final_remarks,
                        finalRemarks = newfinalremarks,
                        description = log.description,
                        terminalIn = log.terminal_in,
                        terminalOut = log.terminal_out,
                        hasManualAttendance = log.manualAttendances.Count > 0,
                        overtime = log.overtime,
                        overtime2 = OvertimeNew,
                        overtime_status = OvertimeStatusNew,
                        status = log.status_in + "-"+log.status_in,
                        remarks = log.manualAttendances.Count() > 0 
                        ? String.Join(", ", log.manualAttendances.Select(m => m.remarks).ToArray()) 
                        : (log.final_remarks == "AB" ? "غياب" : (log.final_remarks == "OFF" ? "عطلة الاسبوع" : "")),
                        totalLateHours = Math.Round(totalLateHours, 2),
                        GrandTotal_Hour = Math.Round(GrandTotal_Hour, 2),
                        totalOvertimeHour = Math.Round(totalOvertimeHour, 2),
                        totalShfit_Hour = Math.Round(totalShfit_Hour, 2)
                    });

                }

                toReturn.logs = tempLogs.AsQueryable().ToArray();


                //Overtime
                int tOvertime = tempLogs.Sum(m => m.overtime);
                TimeSpan time1;


                time1 = TimeSpan.FromSeconds(tOvertime);
                toReturn.totalOvertime = time1.ToString();

                int App_Overtime = tempLogs.Where(m => m.overtime_status == "Approved").Sum(m => m.overtime);
                TimeSpan time2 = TimeSpan.FromSeconds(App_Overtime);
                toReturn.ApprovedOvertime = time2.ToString();


                int Unapproved_Overtime = tempLogs.Where(m => m.overtime_status == "Unapproved").Sum(m => m.overtime);
                TimeSpan time3 = TimeSpan.FromSeconds(Unapproved_Overtime);
                toReturn.UnapprovedOvertime = time3.ToString();



                int Discart_Overtime = tempLogs.Where(m => m.overtime_status == "Discard").Sum(m => m.overtime);
                TimeSpan time4 = TimeSpan.FromSeconds(Discart_Overtime);
                toReturn.DiscartOvertime = time4.ToString();


                //Present
                toReturn.totalPresent = tempLogs.Where(m =>
                    !m.finalRemarks.Equals(DLL.Commons.FinalRemarks.ABSENT) && !m.finalRemarks.Equals(DLL.Commons.FinalRemarks.LV) && !m.finalRemarks.Equals(DLL.Commons.FinalRemarks.OFF)
                    ).Count() + "";

                //Late

                //toReturn.totalLate = tempLogs.Where(m =>
                //    m.finalRemarks.Equals(DLL.Commons.FinalRemarks.PLE) ||
                //    m.finalRemarks.Equals(DLL.Commons.FinalRemarks.PLM) ||
                //    m.finalRemarks.Equals(DLL.Commons.FinalRemarks.PLO)
                //    ).Count() + "";

                //Late And Early
                toReturn.totalLate = tempLogs.Where(m =>
                    m.finalRemarks.Equals("L&E")
                    ).Count() + "";
                //Absent
                toReturn.totalAbsent = tempLogs.Where(m => m.finalRemarks.Equals(DLL.Commons.FinalRemarks.ABSENT)).Count() + "";

                //Leave
                toReturn.totalLeave = leavesCount.ToString(); ////tempLogs.Where(m => m.finalRemarks.Equals(DLL.Commons.FinalRemarks.LV)).Count() + "";

                //Early Out
                //toReturn.totalEarlyOut = tempLogs.Where(m =>
                //    m.finalRemarks.Equals(DLL.Commons.FinalRemarks.PLE) ||
                //    //m.finalRemarks.Equals(DLL.Commons.FinalRemarks.PLM) ||
                //    //m.finalRemarks.Equals(DLL.Commons.FinalRemarks.PME) ||
                //    m.finalRemarks.Equals(DLL.Commons.FinalRemarks.POE)
                //    ).Count() + "";

                //HalfDay
                toReturn.totalEarlyOut = tempLogs.Where(m =>
                    m.finalRemarks.Equals("HD")
                    ).Count() + "";

                //MissPunch
                toReturn.MissPunch = tempLogs.Where(m =>
                    m.finalRemarks.Equals("MissPunch")
                    ).Count() + "";

                //Total Days
                toReturn.totalDays = tempLogs.Where(m =>
                    !m.finalRemarks.Equals(DLL.Commons.FinalRemarks.OFF)
                    ).Count() + "";
            }






            return toReturn;

        }


        public MonthlyTimeSheetData getReport(int employeeID, string month)
        {
            int leavesCount = 0;
            MonthlyTimeSheetData toReturn = null;


            if (!isAuthorised(employeeID))
                return toReturn;

            using (var db = new Context())
            {
                toReturn = new MonthlyTimeSheetData();

                Employee emp = db.employee.Find(employeeID);

                if (emp == null)
                    return null;

                toReturn.employeeCode = emp.employee_code;
                toReturn.employeeName = emp.first_name + " " + emp.last_name;

                DateTime monthDate = DateTime.ParseExact(month, "yyyy-MM", CultureInfo.InvariantCulture);

                DateTime dtThisStart = new DateTime(monthDate.Year, monthDate.Month, 1, 0, 0, 0);
                DateTime dtThisEnd = new DateTime(monthDate.Year, monthDate.Month, DateTime.DaysInMonth(monthDate.Year, monthDate.Month), 23, 59, 0);//(DateTime.Now.Day - 1)

                toReturn.year = monthDate.ToString("yyyy");
                toReturn.month = monthDate.ToString("MMM");


                ConsolidatedAttendance[] attendanceLogs = db.consolidated_attendance.Where(m =>
                    m.employee.EmployeeId.Equals(emp.EmployeeId) &&
                    m.date.Value.Year.Equals(monthDate.Year) &&
                    m.date.Value.Month.Equals(monthDate.Month)).ToArray();

                List<LeaveApplication> lstLeaveApps = db.leave_application.Where(m => m.IsActive &&
                  m.EmployeeId.Equals(emp.EmployeeId) &&
                  (m.FromDate >= dtThisStart && m.ToDate <= dtThisEnd)).ToList();
                if (lstLeaveApps != null && lstLeaveApps.Count > 0)
                {
                    foreach (var l in lstLeaveApps)
                    {
                        leavesCount += l.DaysCount;
                    }
                }

                List<MonthlyTimeSheetLog> tempLogs = new List<MonthlyTimeSheetLog>();
                string newfinalremarks = "";
                string OvertimeStatusNew = "";
                string OvertimeNew = "";
                TimeSpan time;
                string startTime2 = "", shiftEnd3 = ""; string timein = ""; string timeout = "";
                string hours = ""; string hours1 = ""; string halfhours1 = ""; TimeSpan duration; TimeSpan duration1; TimeSpan half;
                string halfhours = "";
                foreach (ConsolidatedAttendance log in attendanceLogs)
                {
                    newfinalremarks = log.final_remarks;
                    if (log.final_remarks == "PLO" || log.final_remarks == "PLE" || log.final_remarks == "PLO" || log.final_remarks == "POE" || log.final_remarks == "PLO") { newfinalremarks = "L&E"; }
                    if (log.final_remarks == "PLM" || log.final_remarks == "PME" || log.final_remarks == "POM") { newfinalremarks = "MissPunch"; }

                    int empId = Convert.ToInt32(employeeID);

                    var getGroupId = db.employee.Where(e => e.EmployeeId == empId).FirstOrDefault();
                    if (getGroupId != null)
                    {
                        if (getGroupId.Group != null)
                        {
                            if (getGroupId.Group.follows_general_calendar)
                            {
                                int gC = db.general_calender.Where(s => s.year == DateTime.Now.Year).FirstOrDefault().Shift1.ShiftId;
                                var test = db.shift.Where(s => s.ShiftId == gC).FirstOrDefault();
                                startTime2 = test.start_time.ToString("hh:mm tt");
                                shiftEnd3 = test.start_time.AddSeconds(test.shift_end).AddMinutes(30).ToString("hh:mm tt");
                                //string start = fromForm.start_time;
                                //string end = fromForm.shift_end;
                                duration = DateTime.Parse(shiftEnd3).Subtract(DateTime.Parse(startTime2));
                                hours = duration.ToString(@"hh\:mm\:ss");
                                half = new TimeSpan(duration.Ticks / 2);
                                halfhours = half.ToString(@"hh\:mm\:ss");
                                //int aa = int.Parse(hours);
                            }
                        }
                    }
                    if (log.time_in != null)
                    {
                        timein = (log.time_in.HasValue) ? log.time_in.Value.ToString("hh:mm tt") : "";
                        timeout = (log.time_out.HasValue) ? log.time_out.Value.ToString("hh:mm tt") : "";

                        if (timein == "" || timeout == "")
                        { }
                        else
                        {
                            try
                            {
                                duration1 = DateTime.Parse(timeout).Subtract(DateTime.Parse(timein));
                                hours1 = duration1.ToString(@"hh\:mm\:ss");
                                if (log.final_remarks == "PO")
                                {
                                    if (DateTime.Parse(hours1) < DateTime.Parse(hours) && DateTime.Parse(hours1) > DateTime.Parse(halfhours))
                                    {
                                        newfinalremarks = "L&E";

                                    }
                                }
                                if (DateTime.Parse(hours1) < DateTime.Parse(halfhours))
                                {
                                    newfinalremarks = "HD";
                                }
                            }
                            catch (Exception ex)
                            {

                            }
                        }
                        //duration1 = DateTime.Parse(timeout).Subtract(DateTime.Parse(timein));
                        //hours1 = duration1.ToString(@"hh\:mm\:ss");
                        //int a = int.Parse(hours1);
                        //int b = a / 2;
                        //if (log.final_remarks == "PO")
                        //{
                        //    if (DateTime.Parse(hours1) <= DateTime.Parse(hours))
                        //    {
                        //        newfinalremarks = "L&E";

                        //    }
                        //}
                    }

                    for (int i = 0; i < lstLeaveApps.Count(); i++)
                    {
                        if (log.date >= lstLeaveApps[i].FromDate && log.date <= lstLeaveApps[i].ToDate && lstLeaveApps[i].LeaveStatusId == 2 && lstLeaveApps[i].LeaveTypeId == 1) { newfinalremarks = "MeL"; }
                        else if (log.date >= lstLeaveApps[i].FromDate && log.date <= lstLeaveApps[i].ToDate && lstLeaveApps[i].LeaveStatusId == 2 && lstLeaveApps[i].LeaveTypeId == 2) { newfinalremarks = "CL"; }
                        else if (log.date >= lstLeaveApps[i].FromDate && log.date <= lstLeaveApps[i].ToDate && lstLeaveApps[i].LeaveStatusId == 2 && lstLeaveApps[i].LeaveTypeId == 3) { newfinalremarks = "EL"; }
                        else if (log.date >= lstLeaveApps[i].FromDate && log.date <= lstLeaveApps[i].ToDate && lstLeaveApps[i].LeaveStatusId == 2 && lstLeaveApps[i].LeaveTypeId == 4) { newfinalremarks = "E.PL"; }
                        else if (log.date >= lstLeaveApps[i].FromDate && log.date <= lstLeaveApps[i].ToDate && lstLeaveApps[i].LeaveStatusId == 2 && lstLeaveApps[i].LeaveTypeId == 5) { newfinalremarks = "MaL"; }
                        else if (log.date >= lstLeaveApps[i].FromDate && log.date <= lstLeaveApps[i].ToDate && lstLeaveApps[i].LeaveStatusId == 2 && lstLeaveApps[i].LeaveTypeId == 6) { newfinalremarks = "TL"; }
                        else if (log.date >= lstLeaveApps[i].FromDate && log.date <= lstLeaveApps[i].ToDate && lstLeaveApps[i].LeaveStatusId == 2 && lstLeaveApps[i].LeaveTypeId == 7) { newfinalremarks = "VL"; }
                        else if (log.date >= lstLeaveApps[i].FromDate && log.date <= lstLeaveApps[i].ToDate && lstLeaveApps[i].LeaveStatusId == 2 && lstLeaveApps[i].LeaveTypeId == 8) { newfinalremarks = "MeetL"; }
                        else if (log.date >= lstLeaveApps[i].FromDate && log.date <= lstLeaveApps[i].ToDate && lstLeaveApps[i].LeaveStatusId == 2 && lstLeaveApps[i].LeaveTypeId == 9) { newfinalremarks = "HL"; }
                        else if (log.date >= lstLeaveApps[i].FromDate && log.date <= lstLeaveApps[i].ToDate && lstLeaveApps[i].LeaveStatusId == 2 && lstLeaveApps[i].LeaveTypeId == 17) { newfinalremarks = "PO*"; }
                        else if (log.date >= lstLeaveApps[i].FromDate && log.date <= lstLeaveApps[i].ToDate && lstLeaveApps[i].LeaveStatusId == 2 && lstLeaveApps[i].LeaveTypeId == 18) { newfinalremarks = "PLO*"; }
                        else if (log.date >= lstLeaveApps[i].FromDate && log.date <= lstLeaveApps[i].ToDate && lstLeaveApps[i].LeaveStatusId == 2 && lstLeaveApps[i].LeaveTypeId == 19) { newfinalremarks = "PLE*"; }
                        else if (log.date >= lstLeaveApps[i].FromDate && log.date <= lstLeaveApps[i].ToDate && lstLeaveApps[i].LeaveStatusId == 2 && lstLeaveApps[i].LeaveTypeId == 20) { newfinalremarks = "POE*"; }
                        else if (log.date >= lstLeaveApps[i].FromDate && log.date <= lstLeaveApps[i].ToDate && lstLeaveApps[i].LeaveStatusId == 2 && lstLeaveApps[i].LeaveTypeId == 21) { newfinalremarks = "OFF*"; }
                        else if (log.date >= lstLeaveApps[i].FromDate && log.date <= lstLeaveApps[i].ToDate && lstLeaveApps[i].LeaveStatusId == 2 && lstLeaveApps[i].LeaveTypeId == 22) { newfinalremarks = "OV*"; }
                        else if (log.date >= lstLeaveApps[i].FromDate && log.date <= lstLeaveApps[i].ToDate && lstLeaveApps[i].LeaveStatusId == 2 && lstLeaveApps[i].LeaveTypeId == 23) { newfinalremarks = "OM*"; }
                        else if (log.date >= lstLeaveApps[i].FromDate && log.date <= lstLeaveApps[i].ToDate && lstLeaveApps[i].LeaveStatusId == 2 && lstLeaveApps[i].LeaveTypeId == 24) { newfinalremarks = "OT*"; }

                    }


                    if (log.overtime < 0)
                    {
                        time = TimeSpan.FromSeconds(log.overtime);
                        OvertimeNew = "-" + time.ToString(@"hh\:mm\:ss");
                    }
                    else
                    {
                        time = TimeSpan.FromSeconds(log.overtime);
                        OvertimeNew = time.ToString(@"hh\:mm\:ss");
                    }


                    if (log.overtime_status == 1) { OvertimeStatusNew = "Unapproved"; }
                    else if (log.overtime_status == 2) { OvertimeStatusNew = "Approved"; }
                    else if (log.overtime_status == 3) { OvertimeStatusNew = "Discard"; }


                    tempLogs.Add(new MonthlyTimeSheetLog()
                    {
                        date = log.date.Value.ToString("dd-MM-yyyy"),
                        timeIn = (log.time_in.HasValue) ? log.time_in.Value.ToString("hh:mm tt") : "",
                        remarksIn = log.status_in,
                        timeOut = (log.time_out.HasValue) ? log.time_out.Value.ToString("hh:mm tt") : "",
                        remarksOut = log.status_out,
                        //finalRemarks = log.final_remarks,
                        finalRemarks = newfinalremarks,
                        description = log.description,
                        terminalIn = log.terminal_in,
                        terminalOut = log.terminal_out,
                        hasManualAttendance = log.manualAttendances.Count > 0,
                        overtime = log.overtime,
                        overtime2 = OvertimeNew,
                        overtime_status = OvertimeStatusNew
                    });

                }

                toReturn.logs = tempLogs.AsQueryable().SortBy("date").ToArray();


                //Overtime
                int tOvertime = tempLogs.Sum(m => m.overtime);
                TimeSpan time1;


                time1 = TimeSpan.FromSeconds(tOvertime);
                toReturn.totalOvertime = time1.ToString();






                int App_Overtime = tempLogs.Where(m => m.overtime_status == "Approved").Sum(m => m.overtime);
                TimeSpan time2 = TimeSpan.FromSeconds(App_Overtime);
                toReturn.ApprovedOvertime = time2.ToString();


                int Unapproved_Overtime = tempLogs.Where(m => m.overtime_status == "Unapproved").Sum(m => m.overtime);
                TimeSpan time3 = TimeSpan.FromSeconds(Unapproved_Overtime);
                toReturn.UnapprovedOvertime = time3.ToString();



                int Discart_Overtime = tempLogs.Where(m => m.overtime_status == "Discard").Sum(m => m.overtime);
                TimeSpan time4 = TimeSpan.FromSeconds(Discart_Overtime);
                toReturn.DiscartOvertime = time4.ToString();


                //Present
                toReturn.totalPresent = tempLogs.Where(m =>
                    !m.finalRemarks.Equals(DLL.Commons.FinalRemarks.ABSENT) && !m.finalRemarks.Equals(DLL.Commons.FinalRemarks.LV) && !m.finalRemarks.Equals(DLL.Commons.FinalRemarks.OFF)
                    ).Count() + "";

                //Late

                //toReturn.totalLate = tempLogs.Where(m =>
                //    m.finalRemarks.Equals(DLL.Commons.FinalRemarks.PLE) ||
                //    m.finalRemarks.Equals(DLL.Commons.FinalRemarks.PLM) ||
                //    m.finalRemarks.Equals(DLL.Commons.FinalRemarks.PLO)
                //    ).Count() + "";

                //Late And Early
                toReturn.totalLate = tempLogs.Where(m =>
                    m.finalRemarks.Equals("L&E")
                    ).Count() + "";
                //Absent
                toReturn.totalAbsent = tempLogs.Where(m => m.finalRemarks.Equals(DLL.Commons.FinalRemarks.ABSENT)).Count() + "";

                //Leave
                toReturn.totalLeave = leavesCount.ToString(); ////tempLogs.Where(m => m.finalRemarks.Equals(DLL.Commons.FinalRemarks.LV)).Count() + "";

                //Early Out
                //toReturn.totalEarlyOut = tempLogs.Where(m =>
                //    m.finalRemarks.Equals(DLL.Commons.FinalRemarks.PLE) ||
                //    //m.finalRemarks.Equals(DLL.Commons.FinalRemarks.PLM) ||
                //    //m.finalRemarks.Equals(DLL.Commons.FinalRemarks.PME) ||
                //    m.finalRemarks.Equals(DLL.Commons.FinalRemarks.POE)
                //    ).Count() + "";

                //HalfDay
                toReturn.totalEarlyOut = tempLogs.Where(m =>
                    m.finalRemarks.Equals("HD")
                    ).Count() + "";

                //MissPunch
                toReturn.MissPunch = tempLogs.Where(m =>
                    m.finalRemarks.Equals("MissPunch")
                    ).Count() + "";

                //Total Days
                toReturn.totalDays = tempLogs.Where(m =>
                    !m.finalRemarks.Equals(DLL.Commons.FinalRemarks.OFF)
                    ).Count() + "";
            }






            return toReturn;

        }

        public MonthlyEmpShiftData getReportMonthlyEmpShiftData(int employeeID, string month)
        {
            int leavesCount = 0;
            MonthlyEmpShiftData toReturn = null;


            if (!isAuthorised(employeeID))
                return toReturn;

            using (var db = new Context())
            {
                toReturn = new MonthlyEmpShiftData();

                Employee emp = db.employee.Find(employeeID);

                if (emp == null)
                    return null;

                toReturn.employeeCode = emp.employee_code;
                toReturn.employeeName = emp.first_name + " " + emp.last_name;

                DateTime monthDate = DateTime.ParseExact(month, "yyyy-MM", CultureInfo.InvariantCulture);

                DateTime dtThisStart = new DateTime(monthDate.Year, monthDate.Month, 1, 0, 0, 0);
                DateTime dtThisEnd = new DateTime(monthDate.Year, monthDate.Month, DateTime.DaysInMonth(monthDate.Year, monthDate.Month), 23, 59, 0);//(DateTime.Now.Day - 1)

                toReturn.year = monthDate.ToString("yyyy");
                toReturn.month = monthDate.ToString("MMM");


                ManualGroupShiftAssigned[] attendanceLogs = db.manual_group_shift_assigned.Where(m =>
                    m.Employee.EmployeeId.Equals(emp.EmployeeId) &&
                    m.date.Value.Year.Equals(monthDate.Year) &&
                    m.date.Value.Month.Equals(monthDate.Month)).ToArray();

                //List<LeaveApplication> lstLeaveApps = db.leave_application.Where(m => m.IsActive &&
                //  m.EmployeeId.Equals(emp.EmployeeId) &&
                //  (m.FromDate >= dtThisStart && m.ToDate <= dtThisEnd)).ToList();
                //if (lstLeaveApps != null && lstLeaveApps.Count > 0)
                //{
                //    foreach (var l in lstLeaveApps)
                //    {
                //        leavesCount += l.DaysCount;
                //    }
                //}

                List<MonthlyEmpShiftLog> tempLogs = new List<MonthlyEmpShiftLog>();

                foreach (ManualGroupShiftAssigned log in attendanceLogs)
                {
                    //if (log.overtime < 0)
                    //{
                    //    time = TimeSpan.FromSeconds(log.overtime);
                    //    OvertimeNew = "-" + time.ToString(@"hh\:mm\:ss");
                    //}
                    //else
                    //{
                    //    time = TimeSpan.FromSeconds(log.overtime);
                    //    OvertimeNew = time.ToString(@"hh\:mm\:ss");
                    //}

                    //if (log.overtime_status == 1) { OvertimeStatusNew = "Unapproved"; }
                    //else if (log.overtime_status == 2) { OvertimeStatusNew = "Approved"; }
                    //else if (log.overtime_status == 3) { OvertimeStatusNew = "Discard"; }

                    tempLogs.Add(new MonthlyEmpShiftLog()
                    {
                        date = log.date.Value.ToString("dd-MM-yyyy"),
                        shift_name = log.Shift.name,
                        shift_start_time = log.Shift.start_time.ToString("hh:mm tt"),
                        shift_late_time = log.Shift.start_time.AddSeconds(log.Shift.late_time).ToString("hh:mm tt"),
                        shift_half_time = log.Shift.start_time.AddSeconds(log.Shift.half_day).ToString("hh:mm tt"),
                        shift_end_time = log.Shift.start_time.AddSeconds(log.Shift.shift_end).ToString("hh:mm tt")
                        //remarksIn = log.status_in,
                        //timeOut = (log.time_out.HasValue) ? log.time_out.Value.ToString("hh:mm tt") : "",
                        //remarksOut = log.status_out,
                        //finalRemarks = log.final_remarks,
                        //terminalIn = log.terminal_in,
                        //terminalOut = log.terminal_out,
                        //hasManualAttendance = log.manualAttendances.Count > 0,
                        //overtime = log.overtime,
                        //overtime2 = OvertimeNew,
                        //overtime_status = OvertimeStatusNew
                    });
                }

                toReturn.logs = tempLogs.AsQueryable().SortBy("date").ToArray();
            }

            return toReturn;
        }

        public List<MonthlyEmpShiftData> getReportMonthlyEmpShiftData(int employeeID, string from_date, string to_date)
        {
            MonthlyEmpShiftData toReturn = null;
            List<MonthlyEmpShiftData> toReturnFinal = new List<MonthlyEmpShiftData>();

            //if (!isAuthorised(employeeID))
            //    return toReturn;

            if (employeeID == -1)
            {
                TimeTune.EmployeeCalendar cal = new TimeTune.EmployeeCalendar();
                var employees = cal.getAllGroupEmployees();
                if (employees != null && employees.Count > 0)
                {
                    foreach (var e in employees)
                    {
                        toReturn = getReportData(e.id, from_date, to_date);
                        toReturnFinal.Add(toReturn);
                    }
                }
            }
            else
            {
                toReturn = getReportData(employeeID, from_date, to_date);
                toReturnFinal.Add(toReturn);
            }

            return toReturnFinal;
        }

        private MonthlyEmpShiftData getReportData(int employeeID, string from_date, string to_date)
        {
            MonthlyEmpShiftData toReturn = null;
            List<MonthlyEmpShiftLog> calFinal = new List<MonthlyEmpShiftLog>();
            List<MonthlyEmpShiftLog> toReturnFinal = new List<MonthlyEmpShiftLog>();

            using (var db = new Context())
            {
                toReturn = new MonthlyEmpShiftData();

                Employee emp = db.employee.Find(employeeID);

                if (emp == null)
                    return null;

                toReturn.employeeCode = emp.employee_code;
                toReturn.employeeName = emp.first_name + " " + emp.last_name;

                DateTime fromDate = DateTime.ParseExact(from_date, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                DateTime toDate = DateTime.ParseExact(to_date, "dd-MM-yyyy", CultureInfo.InvariantCulture);

                //DateTime dtThisStart = new DateTime(fromDate.Year, fromDate.Month, 1, 0, 0, 0);
                //DateTime dtThisEnd = new DateTime(toDate.Year, toDate.Month, DateTime.DaysInMonth(fromDate.Year, fromDate.Month), 23, 59, 0);//(DateTime.Now.Day - 1)

                toReturn.month = fromDate.ToString("dd-MM-yyyy");
                toReturn.year = toDate.ToString("dd-MM-yyyy");

                ManualGroupShiftAssigned[] attendanceLogs = db.manual_group_shift_assigned.Where(m =>
                    m.Employee.EmployeeId.Equals(emp.EmployeeId) &&
                    (m.date.Value >= fromDate && m.date.Value <= toDate)).ToArray();

                //List<LeaveApplication> lstLeaveApps = db.leave_application.Where(m => m.IsActive &&
                //  m.EmployeeId.Equals(emp.EmployeeId) &&
                //  (m.FromDate >= dtThisStart && m.ToDate <= dtThisEnd)).ToList();
                //if (lstLeaveApps != null && lstLeaveApps.Count > 0)
                //{
                //    foreach (var l in lstLeaveApps)
                //    {
                //        leavesCount += l.DaysCount;
                //    }
                //}

                List<MonthlyEmpShiftLog> tempLogs = new List<MonthlyEmpShiftLog>();

                foreach (ManualGroupShiftAssigned log in attendanceLogs)
                {
                    //if (log.overtime < 0)
                    //{
                    //    time = TimeSpan.FromSeconds(log.overtime);
                    //    OvertimeNew = "-" + time.ToString(@"hh\:mm\:ss");
                    //}
                    //else
                    //{
                    //    time = TimeSpan.FromSeconds(log.overtime);
                    //    OvertimeNew = time.ToString(@"hh\:mm\:ss");
                    //}

                    //if (log.overtime_status == 1) { OvertimeStatusNew = "Unapproved"; }
                    //else if (log.overtime_status == 2) { OvertimeStatusNew = "Approved"; }
                    //else if (log.overtime_status == 3) { OvertimeStatusNew = "Discard"; }

                    tempLogs.Add(new MonthlyEmpShiftLog()
                    {
                        date = log.date.Value.ToString("dd-MM-yyyy"),
                        shift_name = log.Shift.name,
                        shift_start_time = log.Shift.start_time.ToString("hh:mm tt"),
                        shift_late_time = log.Shift.start_time.AddSeconds(log.Shift.late_time).ToString("hh:mm tt"),
                        shift_half_time = log.Shift.start_time.AddSeconds(log.Shift.half_day).ToString("hh:mm tt"),
                        shift_end_time = log.Shift.start_time.AddSeconds(log.Shift.shift_end).ToString("hh:mm tt")
                        //remarksIn = log.status_in,
                        //timeOut = (log.time_out.HasValue) ? log.time_out.Value.ToString("hh:mm tt") : "",
                        //remarksOut = log.status_out,
                        //finalRemarks = log.final_remarks,
                        //terminalIn = log.terminal_in,
                        //terminalOut = log.terminal_out,
                        //hasManualAttendance = log.manualAttendances.Count > 0,
                        //overtime = log.overtime,
                        //overtime2 = OvertimeNew,
                        //overtime_status = OvertimeStatusNew
                    });
                }


                /////////////////////////////////////////////////////

                string startTime2 = "", shiftEnd3 = "";

                int groupId = 0;
                if (tempLogs != null)
                {
                    foreach (var log in tempLogs)
                    {

                        //int empId = Convert.ToInt32(employeeID);

                        var getGroupId = db.employee.Where(e => e.EmployeeId == employeeID).FirstOrDefault();
                        if (getGroupId != null)
                        {
                            if (getGroupId.Group != null)
                            {
                                groupId = getGroupId.Group.GroupId;

                                if (getGroupId.Group.follows_general_calendar)
                                {
                                    int gC = db.general_calender.Where(s => s.year == DateTime.Now.Year).FirstOrDefault().Shift1.ShiftId;
                                    var test = db.shift.Where(s => s.ShiftId == gC).FirstOrDefault();
                                    startTime2 = test.start_time.ToString("hh:mm tt");
                                    shiftEnd3 = test.start_time.AddSeconds(test.shift_end).AddMinutes(30).ToString("hh:mm tt");
                                }
                            }
                        }

                        calFinal.Add(new MonthlyEmpShiftLog()
                        {
                            id = log.id,
                            date = log.date,
                            employee_code = log.employee_code,
                            employee_first_name = log.employee_first_name,
                            employee_last_name = log.employee_last_name,
                            shift_name = log.shift_name,
                            shift_start_time = log.dtshift_start_time.ToString("hh:mm tt"),
                            shift_late_time = log.dtshift_start_time.AddSeconds(log.shift_late_sec).ToString("hh:mm tt"),
                            shift_half_time = log.dtshift_start_time.AddSeconds(log.shift_half_sec).ToString("hh:mm tt"),
                            shift_end_time = log.dtshift_start_time.AddSeconds(log.shift_end_sec).ToString("hh:mm tt")
                            //time_in = (log.time_in.HasValue) ? log.time_in.Value.ToString("hh:mm tt") : "",
                            //status_in = log.status_in,
                            //time_out = (log.time_out.HasValue) ? log.time_out.Value.ToString("hh:mm tt") : "",
                            //status_out = log.status_out,
                            //final_remarks = log.final_remarks,
                            //terminal_in = log.terminal_in,
                            //terminal_out = log.terminal_out,
                            //overtime = log.overtime,
                            //overtime_status = OvertimeStatusNew,
                            //str_overtime = OvertimeNew,
                            //action =
                            //    @"<div data-id='" + log.id + @"'>
                            //    <a href='javascript:void(editStatus(" + log.id + "," + log.overtime_status + @"));'>Edit</a>
                            //</div>"
                        });
                    }

                    DateTime dtMonthDate = fromDate;
                    int iDaysCount = (toDate - fromDate).Days + 1;

                    for (int i = 0; i < iDaysCount; i++)
                    {
                        var findShift = calFinal.Find(s => s.date == dtMonthDate.Date.ToString("dd-MM-yyyy"));
                        if (findShift != null)
                        {
                            toReturnFinal.Add(findShift);
                        }
                        else
                        {
                            var dbCalGroup = db.group_calendar.Where(g => g.Group.GroupId == groupId).FirstOrDefault();
                            if (dbCalGroup != null)
                            {
                                //monday
                                if (dtMonthDate.DayOfWeek.ToString().ToLower() == "monday")
                                {
                                    int monShift = 0;
                                    if (dbCalGroup.monday == null)
                                    {
                                        monShift = dbCalGroup.generalShift.ShiftId;
                                    }
                                    else
                                    {
                                        monShift = dbCalGroup.monday.ShiftId;
                                    }

                                    var dbShift = db.shift.Where(s => s.ShiftId == monShift).FirstOrDefault();
                                    if (dbShift != null)
                                    {
                                        var dbEmployee = db.employee.Where(e => e.EmployeeId == employeeID).FirstOrDefault();
                                        if (dbEmployee != null)
                                        {
                                            toReturnFinal.Add(new MonthlyEmpShiftLog()
                                            {
                                                id = -1,
                                                date = dtMonthDate.ToString("dd-MM-yyyy"), //(log.date.HasValue) ? log.date.Value.ToString("dd-MM-yyyy") : "",
                                                employee_code = dbEmployee.employee_code,
                                                employee_first_name = dbEmployee.first_name,
                                                employee_last_name = dbEmployee.last_name,
                                                shift_name = dbShift.name,
                                                shift_start_time = dbShift.start_time.ToString("hh:mm tt"),
                                                shift_late_time = dbShift.start_time.AddSeconds(dbShift.late_time).ToString("hh:mm tt"),
                                                shift_half_time = dbShift.start_time.AddSeconds(dbShift.half_day).ToString("hh:mm tt"),
                                                shift_end_time = dbShift.start_time.AddSeconds(dbShift.shift_end).ToString("hh:mm tt")
                                            });
                                        }
                                    }
                                }//tuesday
                                else if (dtMonthDate.DayOfWeek.ToString().ToLower() == "tuesday")
                                {
                                    int tueShift = 0;
                                    if (dbCalGroup.tuesday == null)
                                    {
                                        tueShift = dbCalGroup.generalShift.ShiftId;
                                    }
                                    else
                                    {
                                        tueShift = dbCalGroup.tuesday.ShiftId;
                                    }

                                    var dbShift = db.shift.Where(s => s.ShiftId == tueShift).FirstOrDefault();
                                    if (dbShift != null)
                                    {
                                        var dbEmployee = db.employee.Where(e => e.EmployeeId == employeeID).FirstOrDefault();
                                        if (dbEmployee != null)
                                        {
                                            toReturnFinal.Add(new MonthlyEmpShiftLog()
                                            {
                                                id = -1,
                                                date = dtMonthDate.ToString("dd-MM-yyyy"), //(log.date.HasValue) ? log.date.Value.ToString("dd-MM-yyyy") : "",
                                                employee_code = dbEmployee.employee_code,
                                                employee_first_name = dbEmployee.first_name,
                                                employee_last_name = dbEmployee.last_name,
                                                shift_name = dbShift.name,
                                                shift_start_time = dbShift.start_time.ToString("hh:mm tt"),
                                                shift_late_time = dbShift.start_time.AddSeconds(dbShift.late_time).ToString("hh:mm tt"),
                                                shift_half_time = dbShift.start_time.AddSeconds(dbShift.half_day).ToString("hh:mm tt"),
                                                shift_end_time = dbShift.start_time.AddSeconds(dbShift.shift_end).ToString("hh:mm tt")
                                            });
                                        }
                                    }
                                }//wednesday
                                else if (dtMonthDate.DayOfWeek.ToString().ToLower() == "wednesday")
                                {
                                    int wedShift = 0;
                                    if (dbCalGroup.wednesday == null)
                                    {
                                        wedShift = dbCalGroup.generalShift.ShiftId;
                                    }
                                    else
                                    {
                                        wedShift = dbCalGroup.wednesday.ShiftId;
                                    }

                                    var dbShift = db.shift.Where(s => s.ShiftId == wedShift).FirstOrDefault();
                                    if (dbShift != null)
                                    {
                                        var dbEmployee = db.employee.Where(e => e.EmployeeId == employeeID).FirstOrDefault();
                                        if (dbEmployee != null)
                                        {
                                            toReturnFinal.Add(new MonthlyEmpShiftLog()
                                            {
                                                id = -1,
                                                date = dtMonthDate.ToString("dd-MM-yyyy"), //(log.date.HasValue) ? log.date.Value.ToString("dd-MM-yyyy") : "",
                                                employee_code = dbEmployee.employee_code,
                                                employee_first_name = dbEmployee.first_name,
                                                employee_last_name = dbEmployee.last_name,
                                                shift_name = dbShift.name,
                                                shift_start_time = dbShift.start_time.ToString("hh:mm tt"),
                                                shift_late_time = dbShift.start_time.AddSeconds(dbShift.late_time).ToString("hh:mm tt"),
                                                shift_half_time = dbShift.start_time.AddSeconds(dbShift.half_day).ToString("hh:mm tt"),
                                                shift_end_time = dbShift.start_time.AddSeconds(dbShift.shift_end).ToString("hh:mm tt")
                                            });
                                        }
                                    }
                                }//thursday
                                else if (dtMonthDate.DayOfWeek.ToString().ToLower() == "thursday")
                                {
                                    int thuShift = 0;
                                    if (dbCalGroup.thursday == null)
                                    {
                                        thuShift = dbCalGroup.generalShift.ShiftId;
                                    }
                                    else
                                    {
                                        thuShift = dbCalGroup.thursday.ShiftId;
                                    }

                                    var dbShift = db.shift.Where(s => s.ShiftId == thuShift).FirstOrDefault();
                                    if (dbShift != null)
                                    {
                                        var dbEmployee = db.employee.Where(e => e.EmployeeId == employeeID).FirstOrDefault();
                                        if (dbEmployee != null)
                                        {
                                            toReturnFinal.Add(new MonthlyEmpShiftLog()
                                            {
                                                id = -1,
                                                date = dtMonthDate.ToString("dd-MM-yyyy"), //(log.date.HasValue) ? log.date.Value.ToString("dd-MM-yyyy") : "",
                                                employee_code = dbEmployee.employee_code,
                                                employee_first_name = dbEmployee.first_name,
                                                employee_last_name = dbEmployee.last_name,
                                                shift_name = dbShift.name,
                                                shift_start_time = dbShift.start_time.ToString("hh:mm tt"),
                                                shift_late_time = dbShift.start_time.AddSeconds(dbShift.late_time).ToString("hh:mm tt"),
                                                shift_half_time = dbShift.start_time.AddSeconds(dbShift.half_day).ToString("hh:mm tt"),
                                                shift_end_time = dbShift.start_time.AddSeconds(dbShift.shift_end).ToString("hh:mm tt")
                                            });
                                        }
                                    }
                                }//friday
                                else if (dtMonthDate.DayOfWeek.ToString().ToLower() == "friday")
                                {
                                    int friShift = 0;
                                    if (dbCalGroup.friday == null)
                                    {
                                        friShift = dbCalGroup.generalShift.ShiftId;
                                    }
                                    else
                                    {
                                        friShift = dbCalGroup.friday.ShiftId;
                                    }

                                    var dbShift = db.shift.Where(s => s.ShiftId == friShift).FirstOrDefault();
                                    if (dbShift != null)
                                    {
                                        var dbEmployee = db.employee.Where(e => e.EmployeeId == employeeID).FirstOrDefault();
                                        if (dbEmployee != null)
                                        {
                                            toReturnFinal.Add(new MonthlyEmpShiftLog()
                                            {
                                                id = -1,
                                                date = dtMonthDate.ToString("dd-MM-yyyy"), //(log.date.HasValue) ? log.date.Value.ToString("dd-MM-yyyy") : "",
                                                employee_code = dbEmployee.employee_code,
                                                employee_first_name = dbEmployee.first_name,
                                                employee_last_name = dbEmployee.last_name,
                                                shift_name = dbShift.name,
                                                shift_start_time = dbShift.start_time.ToString("hh:mm tt"),
                                                shift_late_time = dbShift.start_time.AddSeconds(dbShift.late_time).ToString("hh:mm tt"),
                                                shift_half_time = dbShift.start_time.AddSeconds(dbShift.half_day).ToString("hh:mm tt"),
                                                shift_end_time = dbShift.start_time.AddSeconds(dbShift.shift_end).ToString("hh:mm tt")
                                            });
                                        }
                                    }
                                }//saturday
                                else if (dtMonthDate.DayOfWeek.ToString().ToLower() == "saturday")
                                {
                                    int satShift = 0;
                                    if (dbCalGroup.saturday == null)
                                    {
                                        satShift = dbCalGroup.generalShift.ShiftId;
                                    }
                                    else
                                    {
                                        satShift = dbCalGroup.saturday.ShiftId;
                                    }

                                    var dbShift = db.shift.Where(s => s.ShiftId == satShift).FirstOrDefault();
                                    if (dbShift != null)
                                    {
                                        var dbEmployee = db.employee.Where(e => e.EmployeeId == employeeID).FirstOrDefault();
                                        if (dbEmployee != null)
                                        {
                                            toReturnFinal.Add(new MonthlyEmpShiftLog()
                                            {
                                                id = -1,
                                                date = dtMonthDate.ToString("dd-MM-yyyy"), //(log.date.HasValue) ? log.date.Value.ToString("dd-MM-yyyy") : "",
                                                employee_code = dbEmployee.employee_code,
                                                employee_first_name = dbEmployee.first_name,
                                                employee_last_name = dbEmployee.last_name,
                                                shift_name = dbShift.name,
                                                shift_start_time = dbShift.start_time.ToString("hh:mm tt"),
                                                shift_late_time = dbShift.start_time.AddSeconds(dbShift.late_time).ToString("hh:mm tt"),
                                                shift_half_time = dbShift.start_time.AddSeconds(dbShift.half_day).ToString("hh:mm tt"),
                                                shift_end_time = dbShift.start_time.AddSeconds(dbShift.shift_end).ToString("hh:mm tt")
                                            });
                                        }
                                    }
                                }//sunday
                                else if (dtMonthDate.DayOfWeek.ToString().ToLower() == "sunday")
                                {
                                    int sunShift = 0;
                                    if (dbCalGroup.sunday == null)
                                    {
                                        sunShift = dbCalGroup.generalShift.ShiftId;
                                    }
                                    else
                                    {
                                        sunShift = dbCalGroup.sunday.ShiftId;
                                    }

                                    var dbShift = db.shift.Where(s => s.ShiftId == sunShift).FirstOrDefault();
                                    if (dbShift != null)
                                    {
                                        var dbEmployee = db.employee.Where(e => e.EmployeeId == employeeID).FirstOrDefault();
                                        if (dbEmployee != null)
                                        {
                                            toReturnFinal.Add(new MonthlyEmpShiftLog()
                                            {
                                                id = -1,
                                                date = dtMonthDate.ToString("dd-MM-yyyy"), //(log.date.HasValue) ? log.date.Value.ToString("dd-MM-yyyy") : "",
                                                employee_code = dbEmployee.employee_code,
                                                employee_first_name = dbEmployee.first_name,
                                                employee_last_name = dbEmployee.last_name,
                                                shift_name = dbShift.name,
                                                shift_start_time = dbShift.start_time.ToString("hh:mm tt"),
                                                shift_late_time = dbShift.start_time.AddSeconds(dbShift.late_time).ToString("hh:mm tt"),
                                                shift_half_time = dbShift.start_time.AddSeconds(dbShift.half_day).ToString("hh:mm tt"),
                                                shift_end_time = dbShift.start_time.AddSeconds(dbShift.shift_end).ToString("hh:mm tt")
                                            });
                                        }
                                    }
                                }
                            }
                        }

                        dtMonthDate = dtMonthDate.AddDays(1);
                    }
                }

                ////////////////////////////////////////////////////

                toReturn.logs = toReturnFinal.AsQueryable().SortBy("date").ToArray();
            }

            return toReturn;
        }

        public MonthlyTimeSheetData getCapturedPhotoReport(int employeeID, string month, string rPath, string uPath, DateTime dtFDate, DateTime dtTDate)
        {
            int leavesCount = 0;
            int daysCount = 0;
            string empCode = "", empFirstName = "", empLastName = "", regID = "";
            string uPath1 = "", uPath2 = "";
            uPath1 = uPath.Replace("RRR", "R1");//regID
            uPath2 = uPath.Replace("RRR", "R2");//regID
            DateTime dtFromDate = dtFDate; DateTime dtToDate = dtTDate;

            MonthlyTimeSheetData toReturn = null;


            if (!isAuthorised(employeeID))
                return toReturn;

            using (var db = new Context())
            {

                toReturn = new MonthlyTimeSheetData();

                Employee emp = db.employee.Find(employeeID);

                if (emp == null)
                    return null;


                toReturn.employeeCode = emp.employee_code;
                toReturn.employeeName = emp.first_name + " " + emp.last_name;

                DateTime monthDate = DateTime.ParseExact(month, "yyyy-MM", CultureInfo.InvariantCulture);

                DateTime dtThisStart = new DateTime(monthDate.Year, monthDate.Month, 1, 0, 0, 0);
                DateTime dtThisEnd = new DateTime(monthDate.Year, monthDate.Month, DateTime.DaysInMonth(monthDate.Year, monthDate.Month), 23, 59, 0);//(DateTime.Now.Day - 1)

                toReturn.year = monthDate.ToString("yyyy");
                toReturn.month = monthDate.ToString("MMM");


                ConsolidatedAttendance[] attendanceLogs = db.consolidated_attendance.Where(m =>
                    m.employee.EmployeeId.Equals(emp.EmployeeId) &&
                    m.date.Value.Year.Equals(monthDate.Year) &&
                    m.date.Value.Month.Equals(monthDate.Month)).ToArray();

                List<LeaveApplication> lstLeaveApps = db.leave_application.Where(m => m.IsActive &&
                  m.EmployeeId.Equals(emp.EmployeeId) &&
                  (m.FromDate >= dtThisStart && m.ToDate <= dtThisEnd)).ToList();
                if (lstLeaveApps != null && lstLeaveApps.Count > 0)
                {
                    foreach (var l in lstLeaveApps)
                    {
                        leavesCount += l.DaysCount;
                    }
                }

                List<MonthlyTimeSheetLog> tempLogs = new List<MonthlyTimeSheetLog>();

                string OvertimeStatusNew = "";
                string OvertimeNew = "";
                TimeSpan time;

                foreach (ConsolidatedAttendance log in attendanceLogs)
                {

                    string capt = "", real = "", strRealPhoto1 = "", reltvPath1 = "", strRealPhoto2 = "", reltvPath2 = "";
                    real = getRealPhoto(rPath, log.employee.employee_code, log.date.Value);
                    toReturn.realPhoto = real;

                    //time in
                    strRealPhoto1 = ""; reltvPath1 = "";
                    if (log.time_in.HasValue)
                    {
                        capt = getCapturedPhoto(uPath1, log.employee.employee_code, log.date.Value, log.time_in.Value.ToString("HHmmss"));
                        if (capt != "" && capt.Contains("~"))
                        {
                            string[] arrPaths = capt.Split('~');
                            if (arrPaths.Length > 0)
                            {
                                strRealPhoto1 = arrPaths[0];
                                reltvPath1 = arrPaths[1];
                            }
                        }
                    }
                    else
                    {
                        if (log.time_in.HasValue)
                        {
                            capt = getCapturedPhoto(uPath2, log.employee.employee_code, log.date.Value, log.time_in.Value.ToString("HHmmss"));
                            if (capt != "" && capt.Contains("~"))
                            {
                                string[] arrPaths = capt.Split('~');
                                if (arrPaths.Length > 0)
                                {
                                    strRealPhoto1 = arrPaths[0];
                                    reltvPath1 = arrPaths[1];
                                }
                            }
                        }
                    }

                    //time out
                    strRealPhoto2 = ""; reltvPath2 = "";
                    if (log.time_out.HasValue)
                    {
                        capt = getCapturedPhoto(uPath1, log.employee.employee_code, log.date.Value, log.time_out.Value.ToString("HHmmss"));
                        if (capt != "" && capt.Contains("~"))
                        {
                            string[] arrPaths = capt.Split('~');
                            if (arrPaths.Length > 0)
                            {
                                strRealPhoto2 = arrPaths[0];
                                reltvPath2 = arrPaths[1];
                            }
                        }
                    }
                    else
                    {
                        if (log.time_out.HasValue)
                        {
                            capt = getCapturedPhoto(uPath2, log.employee.employee_code, log.date.Value, log.time_out.Value.ToString("HHmmss"));
                            if (capt != "" && capt.Contains("~"))
                            {
                                string[] arrPaths = capt.Split('~');
                                if (arrPaths.Length > 0)
                                {
                                    strRealPhoto2 = arrPaths[0];
                                    reltvPath2 = arrPaths[1];
                                }
                            }
                        }
                    }

                    //if (log.overtime < 0)
                    //{
                    //    time = TimeSpan.FromSeconds(log.overtime);
                    //    OvertimeNew = "-" + time.ToString(@"hh\:mm\:ss");
                    //}
                    //else
                    //{
                    //    time = TimeSpan.FromSeconds(log.overtime);
                    //    OvertimeNew = time.ToString(@"hh\:mm\:ss");
                    //}


                    //if (log.overtime_status == 1) { OvertimeStatusNew = "Unapproved"; }
                    //else if (log.overtime_status == 2) { OvertimeStatusNew = "Approved"; }
                    //else if (log.overtime_status == 3) { OvertimeStatusNew = "Discard"; }


                    tempLogs.Add(new MonthlyTimeSheetLog()
                    {
                        date = log.date.Value.ToString("dd-MM-yyyy"),
                        timeIn = (log.time_in.HasValue) ? log.time_in.Value.ToString("hh:mm tt") : "",
                        remarksIn = log.status_in,
                        timeOut = (log.time_out.HasValue) ? log.time_out.Value.ToString("hh:mm tt") : "",
                        remarksOut = log.status_out,
                        finalRemarks = log.final_remarks,
                        terminalIn = log.terminal_in,
                        terminalOut = log.terminal_out,
                        hasManualAttendance = log.manualAttendances.Count > 0,
                        overtime = log.overtime,
                        inPhoto = reltvPath1,
                        outPhoto = reltvPath2
                    });

                }

                toReturn.logs = tempLogs.AsQueryable().SortBy("date").ToArray();


                ////Overtime
                //int tOvertime = tempLogs.Sum(m => m.overtime);
                //TimeSpan time1;


                //time1 = TimeSpan.FromSeconds(tOvertime);
                //toReturn.totalOvertime = time1.ToString();






                //int App_Overtime = tempLogs.Where(m => m.overtime_status == "Approved").Sum(m => m.overtime);
                //TimeSpan time2 = TimeSpan.FromSeconds(App_Overtime);
                //toReturn.ApprovedOvertime = time2.ToString();


                //int Unapproved_Overtime = tempLogs.Where(m => m.overtime_status == "Unapproved").Sum(m => m.overtime);
                //TimeSpan time3 = TimeSpan.FromSeconds(Unapproved_Overtime);
                //toReturn.UnapprovedOvertime = time3.ToString();



                //int Discart_Overtime = tempLogs.Where(m => m.overtime_status == "Discard").Sum(m => m.overtime);
                //TimeSpan time4 = TimeSpan.FromSeconds(Discart_Overtime);
                //toReturn.DiscartOvertime = time4.ToString();


                //Present
                toReturn.totalPresent = tempLogs.Where(m =>
                    !m.finalRemarks.Equals(DLL.Commons.FinalRemarks.ABSENT) && !m.finalRemarks.Equals(DLL.Commons.FinalRemarks.LV) && !m.finalRemarks.Equals(DLL.Commons.FinalRemarks.OFF)
                    ).Count() + "";

                //Late
                toReturn.totalLate = tempLogs.Where(m =>
                    m.finalRemarks.Equals(DLL.Commons.FinalRemarks.PLE) ||
                    m.finalRemarks.Equals(DLL.Commons.FinalRemarks.PLM) ||
                    m.finalRemarks.Equals(DLL.Commons.FinalRemarks.PLO)
                    ).Count() + "";

                //Absent
                toReturn.totalAbsent = tempLogs.Where(m => m.finalRemarks.Equals(DLL.Commons.FinalRemarks.ABSENT)).Count() + "";

                //Leave
                toReturn.totalLeave = leavesCount.ToString(); ////tempLogs.Where(m => m.finalRemarks.Equals(DLL.Commons.FinalRemarks.LV)).Count() + "";

                //Early Out
                toReturn.totalEarlyOut = tempLogs.Where(m =>
                    m.finalRemarks.Equals(DLL.Commons.FinalRemarks.PLE) ||
                    //m.finalRemarks.Equals(DLL.Commons.FinalRemarks.PLM) ||
                    //m.finalRemarks.Equals(DLL.Commons.FinalRemarks.PME) ||
                    m.finalRemarks.Equals(DLL.Commons.FinalRemarks.POE)
                    ).Count() + "";

                //Total Days
                toReturn.totalDays = tempLogs.Where(m =>
                    !m.finalRemarks.Equals(DLL.Commons.FinalRemarks.OFF)
                    ).Count() + "";
            }






            return toReturn;

        }

        public static string getRealPhoto(string photoPath, string empCode, DateTime dt)
        {
            //Get User Original Photo
            string strRealPhoto = photoPath + "/demo-user.png";
            List<FileInfo> photoDir = new List<FileInfo>();
            DirectoryInfo phInfo = new DirectoryInfo(photoPath);
            if (phInfo.Exists)
            {
                photoDir = phInfo.GetFiles("photo_*" + empCode + ".jpg").ToList();
                if (photoDir != null && photoDir.Count > 0)
                {
                    foreach (var p in photoDir)
                    {
                        string[] arrPPath = photoPath.Split('/');
                        string reltvPPath = "/" + arrPPath[arrPPath.Length - 3] + "/" + arrPPath[arrPPath.Length - 2] + "/" + arrPPath[arrPPath.Length - 1];

                        strRealPhoto = reltvPPath + "/" + p.Name;//photo_001234.jpg
                        break;
                    }
                }
                else
                {
                    string[] arrPPath = photoPath.Split('/');
                    string reltvPPath = "/" + arrPPath[arrPPath.Length - 3] + "/" + arrPPath[arrPPath.Length - 2] + "/" + arrPPath[arrPPath.Length - 1];

                    strRealPhoto = reltvPPath + "/demo-user.png";//photo_001234.jpg
                }
            }

            return strRealPhoto;
        }

        public static string getCapturedPhoto(string dirPath, string empCode, DateTime dt, string strTime)
        {
            string imgPaths = "";
            List<FileInfo> fileinDir = new List<FileInfo>();

            dirPath = dirPath + dt.ToString("yyyyMMdd") + "/";
            DirectoryInfo dirInfo = new DirectoryInfo(dirPath);
            if (dirInfo.Exists)
            {
                //taking the first (FirstOrDefault()), considering that all files have a unique name with respect to the input value that you are giving. so it should fetch only one file every time you query
                fileinDir = dirInfo.GetFiles(dt.ToString("yyyyMMdd") + strTime + "*" + empCode + ".jpg").ToList();
                if (fileinDir != null && fileinDir.Count > 0)
                {
                    foreach (var f in fileinDir)
                    {
                        string strFileName = f.Name;//20200108070354_00000080_-0000001.jpg
                        if (strFileName != null && strFileName.IndexOf("_-") < 0 && strFileName.Length > 25)
                        {
                            string[] arrPath = dirPath.Split('/');
                            string reltvPath = "/" + arrPath[arrPath.Length - 4] + "/" + arrPath[arrPath.Length - 3] + "/" + arrPath[arrPath.Length - 2] + "/" + arrPath[arrPath.Length - 1] + "/" + strFileName;

                            string fullPath = dirPath + "/" + strFileName;

                            imgPaths = reltvPath + "~" + fullPath;

                            //string sTrmName = "", sTrmID = "";
                            //sTrmID = strFileName.Substring(15, 8);
                            //if (lstTerminals != null && lstTerminals.Count > 0 && sTrmID != "")
                            //{
                            //    int iTermID = 0; iTermID = int.Parse(sTrmID);
                            //    sTrmName = lstTerminals.Find(t => t.L_ID == iTermID).C_Name;
                            //}

                            break;
                        }
                    }
                }
            }

            return imgPaths;
        }

        public MonthlyTimeSheetData getGeoPhencedReport(int employeeID, string month)
        {
            int leavesCount = 0;
            MonthlyTimeSheetData toReturn = null;


            if (!isAuthorised(employeeID))
                return toReturn;

            using (var db = new Context())
            {

                toReturn = new MonthlyTimeSheetData();

                Employee emp = db.employee.Find(employeeID);

                if (emp == null)
                    return null;


                toReturn.employeeCode = emp.employee_code;
                toReturn.employeeName = emp.first_name + " " + emp.last_name;

                DateTime monthDate = DateTime.ParseExact(month, "yyyy-MM", CultureInfo.InvariantCulture);

                DateTime dtThisStart = new DateTime(monthDate.Year, monthDate.Month, 1, 0, 0, 0);
                DateTime dtThisEnd = new DateTime(monthDate.Year, monthDate.Month, DateTime.DaysInMonth(monthDate.Year, monthDate.Month), 23, 59, 0);//(DateTime.Now.Day - 1)

                toReturn.year = monthDate.ToString("yyyy");
                toReturn.month = monthDate.ToString("MMM");


                ConsolidatedAttendance[] attendanceLogs = db.consolidated_attendance.Where(m =>
                    m.employee.EmployeeId.Equals(emp.EmployeeId) &&
                    m.date.Value.Year.Equals(monthDate.Year) &&
                    m.date.Value.Month.Equals(monthDate.Month)).ToArray();

                List<LeaveApplication> lstLeaveApps = db.leave_application.Where(m => m.IsActive &&
                  m.EmployeeId.Equals(emp.EmployeeId) &&
                  (m.FromDate >= dtThisStart && m.ToDate <= dtThisEnd)).ToList();
                if (lstLeaveApps != null && lstLeaveApps.Count > 0)
                {
                    foreach (var l in lstLeaveApps)
                    {
                        leavesCount += l.DaysCount;
                    }
                }

                List<MonthlyTimeSheetLog> tempLogs = new List<MonthlyTimeSheetLog>();

                string OvertimeStatusNew = "";
                string OvertimeNew = "";
                TimeSpan time;

                foreach (ConsolidatedAttendance log in attendanceLogs)
                {
                    //if (log.overtime < 0)
                    //{
                    //    time = TimeSpan.FromSeconds(log.overtime);
                    //    OvertimeNew = "-" + time.ToString(@"hh\:mm\:ss");
                    //}
                    //else
                    //{
                    //    time = TimeSpan.FromSeconds(log.overtime);
                    //    OvertimeNew = time.ToString(@"hh\:mm\:ss");
                    //}


                    //if (log.overtime_status == 1) { OvertimeStatusNew = "Unapproved"; }
                    //else if (log.overtime_status == 2) { OvertimeStatusNew = "Approved"; }
                    //else if (log.overtime_status == 3) { OvertimeStatusNew = "Discard"; }

                    string isAllowedInTerminal = "", isAllowedOutTerminal = "";
                    var dbGPTerminals = db.geo_phencing_terminal.Where(t => t.EmployeeId == employeeID).FirstOrDefault();
                    if (dbGPTerminals != null)
                    {
                        if (log.terminal_in != null && log.terminal_in != "")
                        {
                            if (dbGPTerminals.TerminalsList.ToLower().Contains(log.terminal_in.ToLower()))
                            {
                                isAllowedInTerminal = "*";
                            }
                            else
                            {
                                isAllowedInTerminal = "";
                            }

                            if (dbGPTerminals.TerminalsList.ToLower().Contains(log.terminal_out.ToLower()))
                            {
                                isAllowedOutTerminal = "*";
                            }
                            else
                            {
                                isAllowedOutTerminal = "";
                            }

                        }
                    }


                    tempLogs.Add(new MonthlyTimeSheetLog()
                    {
                        date = log.date.Value.ToString("dd-MM-yyyy"),
                        timeIn = (log.time_in.HasValue) ? log.time_in.Value.ToString("hh:mm tt") : "",
                        remarksIn = log.status_in,
                        timeOut = (log.time_out.HasValue) ? log.time_out.Value.ToString("hh:mm tt") : "",
                        remarksOut = log.status_out,
                        finalRemarks = log.final_remarks,
                        terminalIn = log.terminal_in,
                        terminalOut = log.terminal_out,
                        hasManualAttendance = log.manualAttendances.Count > 0,
                        overtime = log.overtime,
                        overtime_status = isAllowedOutTerminal,
                        overtime2 = isAllowedInTerminal

                    });

                }

                toReturn.logs = tempLogs.AsQueryable().SortBy("date").ToArray();


                //Overtime
                int tOvertime = tempLogs.Sum(m => m.overtime);
                TimeSpan time1;


                time1 = TimeSpan.FromSeconds(tOvertime);
                toReturn.totalOvertime = time1.ToString();






                int App_Overtime = tempLogs.Where(m => m.overtime_status == "Approved").Sum(m => m.overtime);
                TimeSpan time2 = TimeSpan.FromSeconds(App_Overtime);
                toReturn.ApprovedOvertime = time2.ToString();


                int Unapproved_Overtime = tempLogs.Where(m => m.overtime_status == "Unapproved").Sum(m => m.overtime);
                TimeSpan time3 = TimeSpan.FromSeconds(Unapproved_Overtime);
                toReturn.UnapprovedOvertime = time3.ToString();



                int Discart_Overtime = tempLogs.Where(m => m.overtime_status == "Discard").Sum(m => m.overtime);
                TimeSpan time4 = TimeSpan.FromSeconds(Discart_Overtime);
                toReturn.DiscartOvertime = time4.ToString();


                //Present
                toReturn.totalPresent = tempLogs.Where(m =>
                    !m.finalRemarks.Equals(DLL.Commons.FinalRemarks.ABSENT) && !m.finalRemarks.Equals(DLL.Commons.FinalRemarks.LV) && !m.finalRemarks.Equals(DLL.Commons.FinalRemarks.OFF)
                    ).Count() + "";

                //Late
                toReturn.totalLate = tempLogs.Where(m =>
                    m.finalRemarks.Equals(DLL.Commons.FinalRemarks.PLE) ||
                    m.finalRemarks.Equals(DLL.Commons.FinalRemarks.PLM) ||
                    m.finalRemarks.Equals(DLL.Commons.FinalRemarks.PLO)
                    ).Count() + "";

                //Absent
                toReturn.totalAbsent = tempLogs.Where(m => m.finalRemarks.Equals(DLL.Commons.FinalRemarks.ABSENT)).Count() + "";

                //Leave
                toReturn.totalLeave = leavesCount.ToString(); ////tempLogs.Where(m => m.finalRemarks.Equals(DLL.Commons.FinalRemarks.LV)).Count() + "";

                //Early Out
                toReturn.totalEarlyOut = tempLogs.Where(m =>
                    m.finalRemarks.Equals(DLL.Commons.FinalRemarks.PLE) ||
                    //m.finalRemarks.Equals(DLL.Commons.FinalRemarks.PLM) ||
                    //m.finalRemarks.Equals(DLL.Commons.FinalRemarks.PME) ||
                    m.finalRemarks.Equals(DLL.Commons.FinalRemarks.POE)
                    ).Count() + "";

                //Total Days
                toReturn.totalDays = tempLogs.Where(m =>
                    !m.finalRemarks.Equals(DLL.Commons.FinalRemarks.OFF)
                    ).Count() + "";
            }






            return toReturn;

        }

        public MonthlyPunchedPhotoData getPunchedPhotoOld(int employeeID, DateTime dtFromDate, DateTime dtToDate, string uPath, string regID)
        {
            int daysCount = 0;
            string empCode = "", empFirstName = "", empLastName = "";
            uPath = uPath.Replace("RRR", regID);

            Employee empData = new Employee();
            using (Context db = new Context())
            {
                empData = db.employee.Where(e => e.EmployeeId == employeeID).FirstOrDefault();
                if (empData != null)
                {
                    empCode = empData.employee_code;
                    empFirstName = empData.first_name;
                    empLastName = empData.last_name;
                }
            }

            byte[] pData = null;
            if (empData != null)
            {
                using (DLL_UNIS.Models.UNISContext dbUNIS = new DLL_UNIS.Models.UNISContext())
                {
                    var empPhoto = dbUNIS.iUserPictures    // your starting point - table in the "from" statement
                                   .Join(dbUNIS.tUsers, // the source table of the inner join
                                      p => p.L_UID,        // Select the primary key (the first part of the "on" clause in an sql "join" statement)
                                      u => u.L_ID,   // Select the foreign key (the second part of the "on" clause)
                                      (p, u) => new { Picture = p, User = u }) // selection
                                   .Where(pu => pu.User.C_Unique == empCode).FirstOrDefault();
                    if (empPhoto != null && empPhoto.Picture != null && empPhoto.Picture.B_Picture != null)
                    {
                        pData = empPhoto.Picture.B_Picture;
                    }
                    else
                    {
                        pData = null;
                    }
                }
            }

            List<FileInfo> fileinDir = new List<FileInfo>();
            BLL.PdfReports.MonthlyPunchedPhotoData toRender = new BLL.PdfReports.MonthlyPunchedPhotoData();
            List<BLL.PdfReports.MonthlyPunchedPhotoLog> lstPunched = new List<BLL.PdfReports.MonthlyPunchedPhotoLog>();

            daysCount = (dtToDate - dtFromDate).Days + 1;

            toRender.employeeCode = empCode;
            toRender.employeeName = empFirstName + " " + empLastName;
            toRender.originalPhoto = pData;

            string[] directories = Directory.GetDirectories(uPath);
            for (int i = 0; i < daysCount; i++)
            {
                DateTime dt = dtFromDate;
                string dirPath = uPath + dt.ToString("yyyyMMdd");

                toRender.month = dt.ToString("MMM yyyy");

                DirectoryInfo dirInfo = new DirectoryInfo(dirPath);
                if (dirInfo.Exists)
                {
                    //taking the first (FirstOrDefault()), considering that all files have a unique name with respect to the input value that you are giving. so it should fetch only one file every time you query
                    fileinDir = dirInfo.GetFiles(dt.ToString("yyyyMMdd") + "*" + empCode + ".jpg").ToList();
                    if (fileinDir != null && fileinDir.Count > 0)
                    {
                        foreach (var f in fileinDir)
                        {
                            string strFileName = f.Name;//20200108070354_00000080_-0000001.jpg
                            if (strFileName != null && strFileName.IndexOf("_-") < 0 && strFileName.Length > 25)
                            {
                                string[] arrPath = dirPath.Split('/');
                                string reltvPath = "/" + arrPath[arrPath.Length - 3] + "/" + arrPath[arrPath.Length - 2] + "/" + arrPath[arrPath.Length - 1] + "/" + strFileName;

                                string fullPath = dirPath + "/" + strFileName;

                                lstPunched.Add(new BLL.PdfReports.MonthlyPunchedPhotoLog()
                                {
                                    regionID = regID,
                                    eCode = empCode,
                                    eFirstName = empFirstName,
                                    eLastName = empLastName,
                                    date = dt.ToString("dd-MM-yyyy"),
                                    timeInOut = strFileName.Substring(8, 2) + ":" + strFileName.Substring(10, 2) + ":" + strFileName.Substring(12, 2),
                                    photoName = strFileName,
                                    photoRelativePath = reltvPath,
                                    photoFullPath = fullPath,
                                    terminalId = strFileName.Substring(15, 8),
                                    terminalName = strFileName.Substring(15, 8),
                                    actions = "<a class=\"waves-effect waves-light text-danger text-center\" href=\"javascript:void(popup('" + reltvPath.Trim() + "'));\">Photo</a>"
                                });

                                //Debug.WriteLine(f.FullName);
                            }
                        }
                    }
                }

                dtFromDate = dtFromDate.AddDays(1);
            }

            toRender.logs = lstPunched.ToArray();

            return toRender;
        }

        public MonthlyPunchedPhotoData getPunchedPhoto(int employeeID, DateTime dtFDate, DateTime dtTDate, string photoPath, string punchedPath)
        {
            int daysCount = 0;
            string empCode = "", empFirstName = "", empLastName = "", regID = "";
            string uPath1 = "", uPath2 = "";
            uPath1 = punchedPath.Replace("RRR", "R1");//regID
            uPath2 = punchedPath.Replace("RRR", "R2");//regID
            DateTime dtFromDate = dtFDate; DateTime dtToDate = dtTDate;
            List<Terminals> lstTerminals = new List<Terminals>();

            Employee empData = new Employee();
            using (Context db = new Context())
            {
                empData = db.employee.Where(e => e.EmployeeId == employeeID).FirstOrDefault();
                if (empData != null)
                {
                    empCode = empData.employee_code;
                    empFirstName = empData.first_name;
                    empLastName = empData.last_name;
                }

                lstTerminals = db.termainal.ToList();
            }

            byte[] pData = null;
            if (empData != null)
            {
                using (DLL_UNIS.Models.UNISContext dbUNIS = new DLL_UNIS.Models.UNISContext())
                {
                    var empPhoto = dbUNIS.iUserPictures    // your starting point - table in the "from" statement
                                   .Join(dbUNIS.tUsers, // the source table of the inner join
                                      p => p.L_UID,        // Select the primary key (the first part of the "on" clause in an sql "join" statement)
                                      u => u.L_ID,   // Select the foreign key (the second part of the "on" clause)
                                      (p, u) => new { Picture = p, User = u }) // selection
                                   .Where(pu => pu.User.C_Unique == empCode).FirstOrDefault();
                    if (empPhoto != null && empPhoto.Picture != null && empPhoto.Picture.B_Picture != null)
                    {
                        pData = empPhoto.Picture.B_Picture;
                    }
                    else
                    {
                        pData = null;
                    }
                }
            }

            //Get User Original Photo
            string strRealPhoto = photoPath + "/demo-user.png";
            List<FileInfo> photoDir = new List<FileInfo>();
            DirectoryInfo phInfo = new DirectoryInfo(photoPath);
            if (phInfo.Exists)
            {
                photoDir = phInfo.GetFiles("photo_*" + empCode + ".jpg").ToList();
                if (photoDir != null && photoDir.Count > 0)
                {
                    foreach (var p in photoDir)
                    {
                        string[] arrPPath = photoPath.Split('/');
                        string reltvPPath = "/" + arrPPath[arrPPath.Length - 3] + "/" + arrPPath[arrPPath.Length - 2] + "/" + arrPPath[arrPPath.Length - 1];

                        strRealPhoto = reltvPPath + "/" + p.Name;//photo_001234.jpg
                        break;
                    }
                }
                else
                {
                    string[] arrPPath = photoPath.Split('/');
                    string reltvPPath = "/" + arrPPath[arrPPath.Length - 3] + "/" + arrPPath[arrPPath.Length - 2] + "/" + arrPPath[arrPPath.Length - 1];

                    strRealPhoto = reltvPPath + "/demo-user.png";//photo_001234.jpg
                }
            }

            //Get User Punched Photos
            List<FileInfo> fileinDir = new List<FileInfo>();

            BLL.PdfReports.MonthlyPunchedPhotoData toRender = new BLL.PdfReports.MonthlyPunchedPhotoData();
            List<BLL.PdfReports.MonthlyPunchedPhotoLog> lstPunched = new List<BLL.PdfReports.MonthlyPunchedPhotoLog>();

            toRender.employeeCode = empCode;
            toRender.employeeName = empFirstName + " " + empLastName;
            toRender.originalPhoto = pData;

            daysCount = (dtTDate - dtFDate).Days + 1;

            //Region R1 (Master) - Directory JPGs
            dtFromDate = dtFDate;

            string[] directories01 = Directory.GetDirectories(uPath1);
            for (int i = 0; i < daysCount; i++)
            {
                DateTime dt = dtFromDate;
                string dirPath = uPath1 + dt.ToString("yyyyMMdd");

                toRender.month = dt.ToString("MMM yyyy");
                toRender.realPhoto = strRealPhoto;

                DirectoryInfo dirInfo = new DirectoryInfo(dirPath);
                if (dirInfo.Exists)
                {
                    //taking the first (FirstOrDefault()), considering that all files have a unique name with respect to the input value that you are giving. so it should fetch only one file every time you query
                    fileinDir = dirInfo.GetFiles(dt.ToString("yyyyMMdd") + "*" + empCode + ".jpg").ToList();
                    if (fileinDir != null && fileinDir.Count > 0)
                    {
                        foreach (var f in fileinDir)
                        {
                            string strFileName = f.Name;//20200108070354_00000080_-0000001.jpg
                            if (strFileName != null && strFileName.IndexOf("_-") < 0 && strFileName.Length > 25)
                            {
                                string[] arrPath = dirPath.Split('/');
                                string reltvPath = "/" + arrPath[arrPath.Length - 3] + "/" + arrPath[arrPath.Length - 2] + "/" + arrPath[arrPath.Length - 1] + "/" + strFileName;

                                string fullPath = dirPath + "/" + strFileName;

                                string sTrmName = "", sTrmID = "";
                                sTrmID = strFileName.Substring(15, 8);
                                if (lstTerminals != null && lstTerminals.Count > 0 && sTrmID != "")
                                {
                                    int iTermID = 0; iTermID = int.Parse(sTrmID);
                                    sTrmName = lstTerminals.Find(t => t.L_ID == iTermID).C_Name;
                                }

                                string time_in_out = strFileName.Substring(8, 2) + ":" + strFileName.Substring(10, 2) + ":" + strFileName.Substring(12, 2);

                                lstPunched.Add(new BLL.PdfReports.MonthlyPunchedPhotoLog()
                                {
                                    regionID = regID,
                                    eCode = empCode,
                                    eFirstName = empFirstName,
                                    eLastName = empLastName,
                                    date = dt.ToString("dd-MM-yyyy"),
                                    timeInOut = time_in_out,
                                    date_time_in_out = dt.ToString("dd-MM-yyyy") + " " + time_in_out,
                                    photoName = strFileName,
                                    photoRelativePath = reltvPath,
                                    photoFullPath = fullPath,
                                    terminalId = sTrmID,
                                    terminalName = sTrmID + " - " + sTrmName, //strFileName.Substring(15, 8),
                                    actions = "<a class=\"waves-effect waves-light text-danger text-center\" href=\"javascript:void(popup('" + strRealPhoto + "','" + reltvPath.Trim() + "'));\">Photo</a>"
                                });

                                //Debug.WriteLine(f.FullName);
                            }
                        }
                    }
                }

                dtFromDate = dtFromDate.AddDays(1);
            }


            //Region R2 - Directory JPGs
            dtFromDate = dtFDate;

            string[] directories02 = Directory.GetDirectories(uPath2);
            for (int i = 0; i < daysCount; i++)
            {
                DateTime dt = dtFromDate;
                string dirPath = uPath2 + dt.ToString("yyyyMMdd");

                toRender.month = dt.ToString("MMM yyyy");

                DirectoryInfo dirInfo = new DirectoryInfo(dirPath);
                if (dirInfo.Exists)
                {
                    //taking the first (FirstOrDefault()), considering that all files have a unique name with respect to the input value that you are giving. so it should fetch only one file every time you query
                    fileinDir = dirInfo.GetFiles(dt.ToString("yyyyMMdd") + "*" + empCode + ".jpg").ToList();
                    if (fileinDir != null && fileinDir.Count > 0)
                    {
                        foreach (var f in fileinDir)
                        {
                            string strFileName = f.Name;//20200108070354_00000080_-0000001.jpg
                            if (strFileName != null && strFileName.IndexOf("_-") < 0 && strFileName.Length > 25)
                            {
                                string[] arrPath = dirPath.Split('/');
                                string reltvPath = "/" + arrPath[arrPath.Length - 3] + "/" + arrPath[arrPath.Length - 2] + "/" + arrPath[arrPath.Length - 1] + "/" + strFileName;

                                string fullPath = dirPath + "/" + strFileName;

                                string sTrmName = "", sTrmID = "";
                                sTrmID = strFileName.Substring(15, 8);
                                if (lstTerminals != null && lstTerminals.Count > 0 && sTrmID != "")
                                {
                                    int iTermID = 0; iTermID = int.Parse(sTrmID);
                                    sTrmName = lstTerminals.Find(t => t.L_ID == iTermID).C_Name;
                                }

                                string time_in_out = strFileName.Substring(8, 2) + ":" + strFileName.Substring(10, 2) + ":" + strFileName.Substring(12, 2);

                                lstPunched.Add(new BLL.PdfReports.MonthlyPunchedPhotoLog()
                                {
                                    regionID = regID,
                                    eCode = empCode,
                                    eFirstName = empFirstName,
                                    eLastName = empLastName,
                                    date = dt.ToString("dd-MM-yyyy"),
                                    timeInOut = time_in_out,
                                    date_time_in_out = dt.ToString("dd-MM-yyyy") + " " + time_in_out,
                                    photoName = strFileName,
                                    photoRelativePath = reltvPath,
                                    photoFullPath = fullPath,
                                    terminalId = sTrmID,
                                    terminalName = sTrmID + " - " + sTrmName,
                                    actions = "<a class=\"waves-effect waves-light text-danger text-center\" href=\"javascript:void(popup('" + strRealPhoto + "','" + reltvPath.Trim() + "'));\">Photo</a>"
                                });

                                //Debug.WriteLine(f.FullName);
                            }
                        }
                    }
                }

                dtFromDate = dtFromDate.AddDays(1);
            }


            //Add them to DB
            toRender.logs = lstPunched.ToArray();

            return toRender;
        }


        public string getOrganizationLogoTitle()
        {
            string strLogoTitle = "";

            using (var db = new Context())
            {
                var orgLogoTitle = db.organization.FirstOrDefault();
                if (orgLogoTitle != null)
                {
                    strLogoTitle = "~" + orgLogoTitle.Logo + "^" + orgLogoTitle.OrganizationTitle;
                }
                else
                {
                    strLogoTitle = "~/Content/Logos/logo-default.png" + "^" + "DUHS - DOW University of Health Sciences";
                }
            }

            return strLogoTitle;
        }

        public List<MonthlyDepartmentalTimeSheetData> getMonthlyDepartmentalReport(int departmentID, int functionID, int regionID, int locationID, int desginationID, string month)
        {
            string deptName = "";
            List<MonthlyDepartmentalTimeSheetData> listReturn = new List<MonthlyDepartmentalTimeSheetData>();
            MonthlyDepartmentalTimeSheetData toReturn = null;

            if (departmentID != 0)
            {
                using (var db = new Context())
                {
                    var data_dept = db.department.Where(d => d.active && d.DepartmentId == departmentID).FirstOrDefault();
                    if (data_dept != null)
                    {
                        deptName = data_dept.name;
                    }

                    //-----------------------------
                    var data_emp_dept = db.employee.Where(c => c.active == true
                                    &&
                                (departmentID.Equals(-1) || c.department.DepartmentId.Equals(departmentID))
                                     &&
                                (desginationID.Equals(-1) || c.designation.DesignationId.Equals(desginationID))
                                     &&
                                (functionID.Equals(-1) || c.function.FunctionId.Equals(functionID))
                                      &&
                                (regionID.Equals(-1) || c.region.RegionId.Equals(regionID))
                                      &&
                                (locationID.Equals(-1) || c.location.LocationId.Equals(locationID))

                        ).ToList();


                    if (data_emp_dept != null && data_emp_dept.Count > 0)
                    {
                        foreach (var i in data_emp_dept)
                        {
                            toReturn = new MonthlyDepartmentalTimeSheetData();

                            Employee emp = db.employee.Where(e => e.active && e.EmployeeId == i.EmployeeId).FirstOrDefault();
                            if (emp != null)
                            {
                                toReturn.departmentName = deptName;
                                toReturn.employeeCode = emp.employee_code;
                                toReturn.employeeName = emp.first_name + " " + emp.last_name;

                                if (emp.department != null)
                                    toReturn.departmentName = emp.department.name;
                                else toReturn.departmentName = "";

                                if (emp.function != null)
                                    toReturn.functionName = emp.function.name;
                                else toReturn.functionName = "";

                                if (emp.region != null)
                                    toReturn.regionName = emp.region.name;
                                else toReturn.regionName = "";

                                if (emp.location != null)
                                    toReturn.locationName = emp.location.name;
                                else toReturn.locationName = "";

                                if (emp.department != null)
                                    toReturn.designationName = emp.designation.name;
                                else toReturn.designationName = "";

                                DateTime monthDate = DateTime.ParseExact(month, "yyyy-MM", CultureInfo.InvariantCulture);

                                toReturn.year = monthDate.ToString("yyyy");
                                toReturn.month = monthDate.ToString("MMMM");


                                ConsolidatedAttendance[] attendanceLogs = db.consolidated_attendance.Where(m =>
                                    m.employee.EmployeeId.Equals(emp.EmployeeId) &&
                                    m.date.Value.Year.Equals(monthDate.Year) &&
                                    m.date.Value.Month.Equals(monthDate.Month)).ToArray();


                                List<MonthlyTimeSheetLog> tempLogs = new List<MonthlyTimeSheetLog>();

                                string OvertimeStatusNew = "";
                                string OvertimeNew = "";
                                TimeSpan time;

                                foreach (ConsolidatedAttendance log in attendanceLogs)
                                {
                                    if (log.overtime < 0)
                                    {
                                        time = TimeSpan.FromSeconds(log.overtime);
                                        OvertimeNew = "-" + time.ToString(@"hh\:mm\:ss");
                                    }
                                    else
                                    {
                                        time = TimeSpan.FromSeconds(log.overtime);
                                        OvertimeNew = time.ToString(@"hh\:mm\:ss");
                                    }


                                    if (log.overtime_status == 1) { OvertimeStatusNew = "Unapproved"; }
                                    else if (log.overtime_status == 2) { OvertimeStatusNew = "Approved"; }
                                    else if (log.overtime_status == 3) { OvertimeStatusNew = "Discard"; }


                                    tempLogs.Add(new MonthlyTimeSheetLog()
                                    {
                                        date = log.date.Value.ToString("dd-MM-yyyy"),
                                        timeIn = (log.time_in.HasValue) ? log.time_in.Value.ToString("hh:mm tt") : "",
                                        remarksIn = log.status_in,
                                        timeOut = (log.time_out.HasValue) ? log.time_out.Value.ToString("hh:mm tt") : "",
                                        remarksOut = log.status_out,
                                        finalRemarks = log.final_remarks,
                                        terminalIn = log.terminal_in,
                                        terminalOut = log.terminal_out,
                                        hasManualAttendance = log.manualAttendances.Count > 0,
                                        overtime = log.overtime,
                                        overtime2 = OvertimeNew,
                                        overtime_status = OvertimeStatusNew
                                    });
                                }

                                toReturn.logs = tempLogs.AsQueryable().SortBy("date").ToArray();

                                //Overtime
                                int tOvertime = tempLogs.Sum(m => m.overtime);
                                TimeSpan time1;


                                time1 = TimeSpan.FromSeconds(tOvertime);
                                toReturn.totalOvertime = time1.ToString();

                                int App_Overtime = tempLogs.Where(m => m.overtime_status == "Approved").Sum(m => m.overtime);
                                TimeSpan time2 = TimeSpan.FromSeconds(App_Overtime);
                                toReturn.ApprovedOvertime = time2.ToString();


                                int Unapproved_Overtime = tempLogs.Where(m => m.overtime_status == "Unapproved").Sum(m => m.overtime);
                                TimeSpan time3 = TimeSpan.FromSeconds(Unapproved_Overtime);
                                toReturn.UnapprovedOvertime = time3.ToString();



                                int Discart_Overtime = tempLogs.Where(m => m.overtime_status == "Discard").Sum(m => m.overtime);
                                TimeSpan time4 = TimeSpan.FromSeconds(Discart_Overtime);
                                toReturn.DiscartOvertime = time4.ToString();


                                //Present
                                toReturn.totalPresent = tempLogs.Where(m =>
                                    !m.finalRemarks.Equals(DLL.Commons.FinalRemarks.ABSENT) && !m.finalRemarks.Equals(DLL.Commons.FinalRemarks.LV) && !m.finalRemarks.Equals(DLL.Commons.FinalRemarks.OFF)
                                    ).Count() + "";

                                //Late
                                toReturn.totalLate = tempLogs.Where(m =>
                                    m.finalRemarks.Equals(DLL.Commons.FinalRemarks.PLE) ||
                                    m.finalRemarks.Equals(DLL.Commons.FinalRemarks.PLM) ||
                                    m.finalRemarks.Equals(DLL.Commons.FinalRemarks.PLO)
                                    ).Count() + "";

                                //Absent
                                toReturn.totalAbsent = tempLogs.Where(m => m.finalRemarks.Equals(DLL.Commons.FinalRemarks.ABSENT)).Count() + "";

                                //Leave
                                toReturn.totalLeave = tempLogs.Where(m => m.finalRemarks.Equals(DLL.Commons.FinalRemarks.LV)).Count() + "";

                                //Early Out
                                toReturn.totalEarlyOut = tempLogs.Where(m =>
                                    m.finalRemarks.Equals(DLL.Commons.FinalRemarks.PLE) ||
                                    //m.finalRemarks.Equals(DLL.Commons.FinalRemarks.PLM) ||
                                    //m.finalRemarks.Equals(DLL.Commons.FinalRemarks.PME) ||
                                    m.finalRemarks.Equals(DLL.Commons.FinalRemarks.POE)
                                    ).Count() + "";

                                //Total Days
                                toReturn.totalDays = tempLogs.Where(m =>
                                    !m.finalRemarks.Equals(DLL.Commons.FinalRemarks.OFF)
                                    ).Count() + "";

                                listReturn.Add(toReturn);
                            }
                        }
                    }
                }
            }

            return listReturn;

        }

        public List<MonthlyTimeSheetDataAll> getWorkingHourReportAll(string month, string location, string department, string func)
        {
            using (var db = new Context())
            {
                List<MonthlyTimeSheetDataAll> data = new List<MonthlyTimeSheetDataAll>();
                List<Employee> employees = db.employee.ToList();

                if (employees.Count() == 0)
                    return null;

                foreach (Employee emp in employees)
                {
                    if (location.Count() > 0 && (emp.location == null || emp.location.LocationId.ToString() != location))
                    {
                        continue;
                    }
                    if (department.Count() > 0 && (emp.department == null || emp.department.DepartmentId.ToString() != department))
                    {
                        continue;
                    }
                    if (func.Count() > 0 && (emp.function == null || emp.function.FunctionId.ToString() != func))
                    {
                        continue;
                    }

                    MonthlyTimeSheetDataAll toReturn = new MonthlyTimeSheetDataAll();
                    toReturn.employeeCode = emp.employee_code;
                    toReturn.employeeName = emp.first_name + " " + emp.last_name;
                    toReturn.location = emp.location?.name.ToString();

                    DateTime monthDate = DateTime.ParseExact(month, "yyyy-MM", CultureInfo.InvariantCulture);
                    toReturn.year = monthDate.ToString("yyyy");
                    toReturn.month = monthDate.ToString("MMMM");

                    ConsolidatedAttendance[] attendanceLogs = db.consolidated_attendance.Where(m =>
                        m.employee.EmployeeId.Equals(emp.EmployeeId) &&
                        m.date.Value.Year.Equals(monthDate.Year) &&
                        m.date.Value.Month.Equals(monthDate.Month)).ToArray();

                    double totalShfit_Minutes = 0;
                    double totalLateMinutes = 0;
                    double totalEarlyOutMinutes = 0;

                    foreach (ConsolidatedAttendance log in attendanceLogs)
                    {
                        if (log.final_remarks != "AB" && log.final_remarks != "OFF" && log.final_remarks != "LV")
                        {
                            if (log.time_in != null && log.time_out != null)
                            {
                                var empModel = emp;
                                var shift = getShift(empModel, log.date, db);

                                // Calculate shift duration in minutes
                                TimeSpan shiftDuration = log.time_out.Value - log.time_in.Value;
                                totalShfit_Minutes += shiftDuration.TotalMinutes;

                                string ShiftEndTime = shift.start_time.AddSeconds(shift.shift_end).ToString("HH:mm");
                                string LateHour = DateTime.Parse(log.time_in.Value.ToString("HH:mm")).Subtract(DateTime.Parse(shift.start_time.ToString("HH:mm"))).ToString();
                                string EarlyOutHour = DateTime.Parse(ShiftEndTime).Subtract(DateTime.Parse(log.time_out.Value.ToString("HH:mm"))).ToString();

                                if (!LateHour.Contains("-"))
                                    totalLateMinutes += TimeSpan.Parse(LateHour).TotalMinutes;

                                if (!EarlyOutHour.Contains("-"))
                                    totalEarlyOutMinutes += TimeSpan.Parse(EarlyOutHour).TotalMinutes;
                            }
                        }
                    }

                    // Convert total minutes to hours and minutes
                    int totalShiftHours = (int)(totalShfit_Minutes / 60);
                    int totalShiftMinutes = (int)(totalShfit_Minutes % 60);
                    toReturn.totalTime = $"{totalShiftHours:D2} : {totalShiftMinutes:D2}";

                    int totalLateHours = (int)(totalLateMinutes / 60);
                    int totalLateMinutesRemainder = (int)(totalLateMinutes % 60);
                    toReturn.totalLate = $"{totalLateHours:D2} : {totalLateMinutesRemainder:D2}";

                    int totalEarlyOutHours = (int)(totalEarlyOutMinutes / 60);
                    int totalEarlyOutMinutesRemainder = (int)(totalEarlyOutMinutes % 60);
                    toReturn.totalEarlyOut = $"{totalEarlyOutHours:D2} : {totalEarlyOutMinutesRemainder:D2}";

                    toReturn.totalPresent = attendanceLogs.Count(m => m.final_remarks != "AB" && m.final_remarks != "OFF").ToString();

                    int ab = attendanceLogs.Count(m => m.final_remarks == "AB");
                    int totalAbsentHours = (ab * 410) / 60;
                    int totalAbsentMinutes = (ab * 410) % 60;
                    //toReturn.totalAbsent = $"{ab.ToString()} ({totalAbsentHours:D2}hrs {totalAbsentMinutes:D2}mins)";
                    toReturn.totalAbsent = $"{ab.ToString()} ({totalAbsentHours:D2}:{totalAbsentMinutes:D2})";

                    data.Add(toReturn);
                }

                return data;
            }
        }

        public MonthlyTimeSheetData getWorkingHourReport(int employeeID, string month)
        {
            MonthlyTimeSheetData toReturn = null;


            if (!isAuthorised(employeeID))
                return toReturn;

            using (var db = new Context())
            {

                toReturn = new MonthlyTimeSheetData();

                Employee emp = db.employee.Find(employeeID);

                if (emp == null)
                    return null;


                toReturn.employeeCode = emp.employee_code;
                toReturn.employeeName = emp.first_name + " " + emp.last_name;

                DateTime monthDate = DateTime.ParseExact(month, "yyyy-MM", CultureInfo.InvariantCulture);

                toReturn.year = monthDate.ToString("yyyy");
                toReturn.month = monthDate.ToString("MMMM");


                ConsolidatedAttendance[] attendanceLogs = db.consolidated_attendance.Where(m =>
                    m.employee.EmployeeId.Equals(emp.EmployeeId) &&
                    m.date.Value.Year.Equals(monthDate.Year) &&
                    m.date.Value.Month.Equals(monthDate.Month)).ToArray();


                List<MonthlyTimeSheetLog> tempLogs = new List<MonthlyTimeSheetLog>();

                double totalShfit_Hour = 0;
                double GrandTotal_Hour = 0;
                double totalLateHours = 0;
                double totalOvertimeHour = 0;

                foreach (ConsolidatedAttendance log in attendanceLogs)
                {
                    if (log.final_remarks != "AB" && log.final_remarks != "OFF" && log.final_remarks != "LV")
                    {
                        var empModel = db.employee.Where(e => e.EmployeeId == employeeID).FirstOrDefault();
                        var shift = getShift(empModel, log.date, db);
                        TimeSpan totalShiftHours = TimeSpan.FromSeconds(shift.shift_end);
                        totalShfit_Hour += totalShiftHours.TotalHours;

                        string LateHour = "";
                        if (log.time_in != null)
                        {
                            LateHour = DateTime.Parse(log.time_in.Value.ToString("HH:mm")).Subtract(DateTime.Parse(shift.start_time.ToString("HH:mm"))).ToString();

                            if (!LateHour.Contains("-"))
                            {
                                totalLateHours += TimeSpan.Parse(LateHour).TotalHours;
                            }

                            if (LateHour.Contains("-"))
                            {
                                LateHour = "00:00:00";
                            }
                        }

                        string OverTime = "";
                        if (log.time_out != null) {
                            string ShiftEndTime = shift.start_time.AddSeconds(shift.shift_end).ToString("HH:mm");
                            OverTime = DateTime.Parse(log.time_out.Value.ToString("HH:mm")).Subtract(DateTime.Parse(ShiftEndTime)).ToString();
                            if (!OverTime.Contains("-"))
                            {
                                totalOvertimeHour += TimeSpan.Parse(OverTime).TotalHours;
                            }


                            string Total = DateTime.Parse(totalShiftHours.ToString()).Subtract(DateTime.Parse(LateHour)).ToString();
                            if (Total.Contains("-"))
                            {
                                Total = Total.Replace("-", "");
                            }
                            if (OverTime.Contains("-"))
                            {

                                totalOvertimeHour += TimeSpan.Parse(OverTime).TotalHours;

                            }
                        }

                        string GrandTotal = "";
                        GrandTotal = TimeTune.Reports.getWorkHourDurationAttendance(employeeID.ToString(), log.date.Value.Date.ToString("dd-MM-yyyy"), log.date.Value.Date.ToString("dd-MM-yyyy"));
                        GrandTotal_Hour += TimeSpan.Parse(GrandTotal).TotalHours;

                        tempLogs.Add(new MonthlyTimeSheetLog()
                        {
                            remarksIn = log.date.Value.ToString("MMM yyyy"),
                            date = log.date.Value.ToString("dd-MM-yyyy"),
                            overtime_status = empModel.employee_code, //Emp Code
                            finalRemarks = empModel.first_name + " " + empModel.last_name, //First Name
                            timeIn = (log.time_in.HasValue) ? log.time_in.Value.ToString("hh:mm tt") : "",
                            timeOut = (log.time_out.HasValue) ? log.time_out.Value.ToString("hh:mm tt") : "",

                            overtime2 = totalShiftHours.ToString(), // Totoal Hours
                            remarksOut = LateHour, // Late Hour
                            terminalIn = OverTime,//Overtime
                            terminalOut = GrandTotal,// grand totoal

                            totalLateHours = Math.Round(totalLateHours, 2),
                            GrandTotal_Hour = Math.Round(GrandTotal_Hour, 2),
                            totalOvertimeHour = Math.Round(totalOvertimeHour, 2),
                            totalShfit_Hour = Math.Round(totalShfit_Hour, 2)


                        });
                    }
                    else if (log.final_remarks == "AB")
                    {
                        var empModel = db.employee.Where(e => e.EmployeeId == employeeID).FirstOrDefault();
                        var shift = getShift(empModel, log.date, db);
                        TimeSpan totalShiftHours = TimeSpan.FromSeconds(shift.shift_end);
                        totalShfit_Hour += totalShiftHours.TotalHours;
                        tempLogs.Add(new MonthlyTimeSheetLog()
                        {
                            remarksIn = log.date.Value.ToString("MMM yyyy"),
                            date = log.date.Value.ToString("dd-MM-yyyy"),
                            overtime_status = empModel.employee_code, //Emp Code
                            finalRemarks = empModel.first_name + " " + empModel.last_name, //First Name
                            timeIn = "AB",
                            timeOut = "AB",

                            overtime2 = totalShiftHours.ToString(), // Totoal Hours
                            remarksOut = "00:00:00", // Late Hour
                            terminalIn = "00:00:00",//Overtime
                            terminalOut = "00:00:00",// grand totoal

                            totalLateHours = tempLogs.Count > 0 ? tempLogs[tempLogs.Count - 1].totalLateHours : 0,
                            GrandTotal_Hour = tempLogs.Count > 0 ? tempLogs[tempLogs.Count - 1].GrandTotal_Hour : 0,
                            totalOvertimeHour = tempLogs.Count > 0 ? tempLogs[tempLogs.Count - 1].totalOvertimeHour : 0,
                            totalShfit_Hour = totalShfit_Hour


                        });
                    }
                    else if (log.final_remarks == "OFF")
                    {
                        var empModel = db.employee.Where(e => e.EmployeeId == employeeID).FirstOrDefault();
                        tempLogs.Add(new MonthlyTimeSheetLog()
                        {
                            remarksIn = log.date.Value.ToString("MMM yyyy"),
                            date = log.date.Value.ToString("dd-MM-yyyy"),
                            overtime_status = empModel.employee_code, //Emp Code
                            finalRemarks = empModel.first_name + " " + empModel.last_name, //First Name
                            timeIn = "OFF",
                            timeOut = "OFF",

                            overtime2 = "00:00:00", // Totoal Hours
                            remarksOut = "00:00:00", // Late Hour
                            terminalIn = "00:00:00",//Overtime
                            terminalOut = "00:00:00",// grand totoal

                            totalLateHours = tempLogs.Count > 0 ? tempLogs[tempLogs.Count - 1].totalLateHours : 0,
                            GrandTotal_Hour = tempLogs.Count > 0 ? tempLogs[tempLogs.Count - 1].GrandTotal_Hour : 0,
                            totalOvertimeHour = tempLogs.Count > 0 ? tempLogs[tempLogs.Count - 1].totalOvertimeHour : 0,
                            totalShfit_Hour = tempLogs.Count > 0 ? tempLogs[tempLogs.Count - 1].totalShfit_Hour : 0


                        });
                    }
                    else if (log.final_remarks == "LV")
                    {
                        var empModel = db.employee.Where(e => e.EmployeeId == employeeID).FirstOrDefault();
                        var shift = getShift(empModel, log.date, db);
                        TimeSpan totalShiftHours = TimeSpan.FromSeconds(shift.shift_end);
                        totalShfit_Hour += totalShiftHours.TotalHours;
                        tempLogs.Add(new MonthlyTimeSheetLog()
                        {
                            remarksIn = log.date.Value.ToString("MMM yyyy"),
                            date = log.date.Value.ToString("dd-MM-yyyy"),
                            overtime_status = empModel.employee_code, //Emp Code
                            finalRemarks = empModel.first_name + " " + empModel.last_name, //First Name
                            timeIn = "LV",
                            timeOut = "LV",

                            overtime2 = totalShiftHours.ToString(), // Totoal Hours
                            remarksOut = "00:00:00", // Late Hour
                            terminalIn = "00:00:00",//Overtime
                            terminalOut = "00:00:00",// grand totoal

                            totalLateHours = tempLogs.Count > 0 ? tempLogs[tempLogs.Count - 1].totalLateHours : 0,
                            GrandTotal_Hour = tempLogs.Count > 0 ? tempLogs[tempLogs.Count - 1].GrandTotal_Hour : 0,
                            totalOvertimeHour = tempLogs.Count > 0 ? tempLogs[tempLogs.Count - 1].totalOvertimeHour : 0,
                            totalShfit_Hour = totalShfit_Hour


                        });
                    }

                    toReturn.logs = tempLogs.AsQueryable().SortBy("date").ToArray();
                }

                return toReturn;

            }


        }

        public MonthlyTimeSheetData getWorkingHourReportNew(int employeeID, string month)
        {
            MonthlyTimeSheetData toReturn = null;


            if (!isAuthorised(employeeID))
                return toReturn;

            using (var db = new Context())
            {

                toReturn = new MonthlyTimeSheetData();

                Employee emp = db.employee.Find(employeeID);

                if (emp == null)
                    return null;


                toReturn.employeeCode = emp.employee_code;
                toReturn.employeeName = emp.first_name + " " + emp.last_name;

                DateTime monthDate = DateTime.ParseExact(month, "yyyy-MM", CultureInfo.InvariantCulture);

                toReturn.year = monthDate.ToString("yyyy");
                toReturn.month = monthDate.ToString("MMMM");


                ConsolidatedAttendance[] attendanceLogs = db.consolidated_attendance.Where(m =>
                    m.employee.EmployeeId.Equals(emp.EmployeeId) &&
                    m.date.Value.Year.Equals(monthDate.Year) &&
                    m.date.Value.Month.Equals(monthDate.Month)).ToArray();


                List<MonthlyTimeSheetLog> tempLogs = new List<MonthlyTimeSheetLog>();

                double totalShfit_Hour = 0;
                double GrandTotal_Hour = 0;
                double totalLateHours = 0;
                double totalOvertimeHour = 0;


                foreach (ConsolidatedAttendance log in attendanceLogs)
                {
                    if (log.final_remarks != "AB" && log.final_remarks != "OFF" && log.final_remarks != "LV")
                    {
                        if (log.time_in != null && log.time_out != null)
                        {

                            var empModel = db.employee.Where(e => e.EmployeeId == employeeID).FirstOrDefault();

                            var shift = getShift(empModel, log.date, db);
                            TimeSpan totalShiftHours = TimeSpan.FromSeconds(shift.shift_end);

                            totalShfit_Hour += totalShiftHours.TotalHours;


                            string ShiftEndTime = shift.start_time.AddSeconds(shift.shift_end).ToString("HH:mm");

                            string LateHour = DateTime.Parse(log.time_in.Value.ToString("HH:mm")).Subtract(DateTime.Parse(shift.start_time.ToString("HH:mm"))).ToString();

                            if (!LateHour.Contains("-"))
                            {
                                totalLateHours += TimeSpan.Parse(LateHour).TotalHours;
                            }


                            string GrandTotal = "";
                            if (LateHour.Contains("-"))
                            {
                                LateHour = "00:00:00";
                            }
                            string OverTime = DateTime.Parse(log.time_out.Value.ToString("HH:mm")).Subtract(DateTime.Parse(ShiftEndTime)).ToString();
                            if (!OverTime.Contains("-"))
                            {
                                totalOvertimeHour += TimeSpan.Parse(OverTime).TotalHours;
                            }


                            string Total = DateTime.Parse(totalShiftHours.ToString()).Subtract(DateTime.Parse(LateHour)).ToString();
                            if (Total.Contains("-"))
                            {
                                Total = Total.Replace("-", "");
                            }
                            if (OverTime.Contains("-"))
                            {

                                totalOvertimeHour += TimeSpan.Parse(OverTime).TotalHours;

                            }
                            GrandTotal = TimeTune.Reports.getWorkHourDurationAttendance(employeeID.ToString(), log.date.Value.Date.ToString("dd-MM-yyyy"), log.date.Value.Date.ToString("dd-MM-yyyy"));
                            GrandTotal_Hour += TimeSpan.Parse(GrandTotal).TotalHours;

                            if (shift != null && totalShiftHours != null && LateHour != null && ShiftEndTime != null && OverTime != null && GrandTotal != null && Total != null)
                            {

                            }

                            tempLogs.Add(new MonthlyTimeSheetLog()
                            {
                                remarksIn = log.date.Value.ToString("MMM yyyy"),
                                date = log.date.Value.ToString("dd-MM-yyyy"),
                                day = log.date.Value.ToString("dddd"),
                                status = (log.status_in + "-" + log.status_out),
                                overtime_status = empModel.employee_code, //Emp Code
                                finalRemarks = log.final_remarks, //First Name
                                timeIn = (log.time_in.HasValue) ? log.time_in.Value.ToString("hh:mm tt") : "",
                                timeOut = (log.time_out.HasValue) ? log.time_out.Value.ToString("hh:mm tt") : "",

                                overtime2 = totalShiftHours.ToString(), // Totoal Hours
                                remarksOut = LateHour, // Late Hour
                                terminalIn = OverTime,//Overtime
                                terminalOut = GrandTotal,// grand totoal

                                totalLateHours = Math.Round(totalLateHours, 2),
                                GrandTotal_Hour = Math.Round(GrandTotal_Hour, 2),
                                totalOvertimeHour = Math.Round(totalOvertimeHour, 2),
                                totalShfit_Hour = Math.Round(totalShfit_Hour, 2)


                            });
                        }
                    }
                    else if (log.final_remarks == "AB")
                    {
                        var empModel = db.employee.Where(e => e.EmployeeId == employeeID).FirstOrDefault();
                        var shift = getShift(empModel, log.date, db);
                        TimeSpan totalShiftHours = TimeSpan.FromSeconds(shift.shift_end);
                        totalShfit_Hour += totalShiftHours.TotalHours;
                        tempLogs.Add(new MonthlyTimeSheetLog()
                        {
                            remarksIn = log.date.Value.ToString("MMM yyyy"),
                            date = log.date.Value.ToString("dd-MM-yyyy"),
                            day = log.date.Value.ToString("dddd"),
                            status = (log.status_in),
                            overtime_status = empModel.employee_code, //Emp Code
                            finalRemarks = log.final_remarks,
                            timeIn = "AB",
                            timeOut = "AB",

                            overtime2 = totalShiftHours.ToString(), // Totoal Hours
                            remarksOut = "00:00:00", // Late Hour
                            terminalIn = "00:00:00",//Overtime
                            terminalOut = "00:00:00",// grand totoal

                            totalLateHours = tempLogs.Count > 0 ? tempLogs[tempLogs.Count - 1].totalLateHours : 0,
                            GrandTotal_Hour = tempLogs.Count > 0 ? tempLogs[tempLogs.Count - 1].GrandTotal_Hour : 0,
                            totalOvertimeHour = tempLogs.Count > 0 ? tempLogs[tempLogs.Count - 1].totalOvertimeHour : 0,
                            totalShfit_Hour = totalShfit_Hour


                        });
                    }
                    else if (log.final_remarks == "OFF")
                    {
                        var empModel = db.employee.Where(e => e.EmployeeId == employeeID).FirstOrDefault();
                        tempLogs.Add(new MonthlyTimeSheetLog()
                        {
                            remarksIn = log.date.Value.ToString("MMM yyyy"),
                            date = log.date.Value.ToString("dd-MM-yyyy"),
                            day = log.date.Value.ToString("dddd"),
                            status = (log.status_in),
                            overtime_status = empModel.employee_code, //Emp Code
                            finalRemarks = log.final_remarks, //First Name
                            timeIn = "OFF",
                            timeOut = "OFF",

                            overtime2 = "00:00:00", // Totoal Hours
                            remarksOut = "00:00:00", // Late Hour
                            terminalIn = "00:00:00",//Overtime
                            terminalOut = "00:00:00",// grand totoal

                            totalLateHours = tempLogs.Count > 0 ? tempLogs[tempLogs.Count - 1].totalLateHours : 0,
                            GrandTotal_Hour = tempLogs.Count > 0 ? tempLogs[tempLogs.Count - 1].GrandTotal_Hour : 0,
                            totalOvertimeHour = tempLogs.Count > 0 ? tempLogs[tempLogs.Count - 1].totalOvertimeHour : 0,
                            totalShfit_Hour = tempLogs.Count > 0 ? tempLogs[tempLogs.Count - 1].totalShfit_Hour : 0


                        });
                    }
                    else if (log.final_remarks == "LV")
                    {
                        var empModel = db.employee.Where(e => e.EmployeeId == employeeID).FirstOrDefault();
                        var shift = getShift(empModel, log.date, db);
                        TimeSpan totalShiftHours = TimeSpan.FromSeconds(shift.shift_end);
                        totalShfit_Hour += totalShiftHours.TotalHours;
                        tempLogs.Add(new MonthlyTimeSheetLog()
                        {
                            remarksIn = log.date.Value.ToString("MMM yyyy"),
                            date = log.date.Value.ToString("dd-MM-yyyy"),
                            day = log.date.Value.ToString("dddd"),
                            status = (log.status_in),
                            overtime_status = empModel.employee_code, //Emp Code
                            finalRemarks = log.final_remarks,
                            timeIn = "LV",
                            timeOut = "LV",

                            overtime2 = totalShiftHours.ToString(), // Totoal Hours
                            remarksOut = "00:00:00", // Late Hour
                            terminalIn = "00:00:00",//Overtime
                            terminalOut = "00:00:00",// grand totoal

                            totalLateHours = tempLogs.Count > 0 ? tempLogs[tempLogs.Count - 1].totalLateHours : 0,
                            GrandTotal_Hour = tempLogs.Count > 0 ? tempLogs[tempLogs.Count - 1].GrandTotal_Hour : 0,
                            totalOvertimeHour = tempLogs.Count > 0 ? tempLogs[tempLogs.Count - 1].totalOvertimeHour : 0,
                            totalShfit_Hour = totalShfit_Hour


                        });
                    }

                }

                //Present
                toReturn.totalPresent = tempLogs.Where(m =>
                   m.timeIn != "AB" && m.timeIn != "OFF"
                    ).Count() + "";


                //Absent
                toReturn.totalAbsent = tempLogs.Where(m => m.timeIn == "AB").Count() + "";



                toReturn.logs = tempLogs.AsQueryable().SortBy("date").ToArray();
                return toReturn;

            }


        }
        //public CustomRangeTimeSheetData getWorkingHourReportAlt(int employeeID, string from, string to)
        //{
        //    CustomRangeTimeSheetData toReturn = null;


        //    if (!isAuthorised(employeeID))
        //        return toReturn;

        //    using (var db = new Context())
        //    {

        //        toReturn = new CustomRangeTimeSheetData();

        //        Employee emp = db.employee.Find(employeeID);

        //        if (emp == null)
        //            return null;


        //        toReturn.employeeCode = emp.employee_code;
        //        toReturn.employeeName = emp.first_name + " " + emp.last_name;

        //        DateTime fromDate = DateTime.ParseExact(from, "yyyy-MM-dd", CultureInfo.InvariantCulture);
        //        toReturn.fromYear = fromDate.ToString("yyyy");
        //        toReturn.fromMonth = fromDate.ToString("MM");
        //        toReturn.fromDay = fromDate.ToString("dd");

        //        DateTime toDate = DateTime.ParseExact(to, "yyyy-MM-dd", CultureInfo.InvariantCulture);
        //        toReturn.toYear = toDate.ToString("yyyy");
        //        toReturn.toMonth = toDate.ToString("MM");
        //        toReturn.toDay = toDate.ToString("dd");


        //        ConsolidatedAttendance[] attendanceLogs = db.consolidated_attendance
        //             .Where(m => m.employee.EmployeeId.Equals(emp.EmployeeId) &&
        //                 m.date >= fromDate &&
        //                 m.date <= toDate)
        //             .ToArray();


        //        List<MonthlyTimeSheetLog> tempLogs = new List<MonthlyTimeSheetLog>();

        //        double totalShfit_Hour = 0;
        //        double GrandTotal_Hour = 0;
        //        double totalLateHours = 0;
        //        double totalOvertimeHour = 0;


        //        foreach (ConsolidatedAttendance log in attendanceLogs)
        //        {
        //            if (log.final_remarks != "AB" && log.final_remarks != "OFF" && log.final_remarks != "LV")
        //            {
        //                if (log.time_in != null && log.time_out != null)
        //                {

        //                    var empModel = db.employee.Where(e => e.EmployeeId == employeeID).FirstOrDefault();

        //                    var shift = getShift(empModel, log.date, db);
        //                    TimeSpan totalShiftHours = TimeSpan.FromSeconds(shift.shift_end);

        //                    totalShfit_Hour += totalShiftHours.TotalHours;


        //                    string ShiftEndTime = shift.start_time.AddSeconds(shift.shift_end).ToString("HH:mm");

        //                    string LateHour = DateTime.Parse(log.time_in.Value.ToString("HH:mm")).Subtract(DateTime.Parse(shift.start_time.ToString("HH:mm"))).ToString();

        //                    if (!LateHour.Contains("-"))
        //                    {
        //                        totalLateHours += TimeSpan.Parse(LateHour).TotalHours;
        //                    }


        //                    string GrandTotal = "";
        //                    if (LateHour.Contains("-"))
        //                    {
        //                        LateHour = "00:00:00";
        //                    }
        //                    string OverTime = DateTime.Parse(log.time_out.Value.ToString("HH:mm")).Subtract(DateTime.Parse(ShiftEndTime)).ToString();
        //                    if (!OverTime.Contains("-"))
        //                    {
        //                        totalOvertimeHour += TimeSpan.Parse(OverTime).TotalHours;
        //                    }


        //                    string Total = DateTime.Parse(totalShiftHours.ToString()).Subtract(DateTime.Parse(LateHour)).ToString();
        //                    if (Total.Contains("-"))
        //                    {
        //                        Total = Total.Replace("-", "");
        //                    }
        //                    if (OverTime.Contains("-"))
        //                    {

        //                        totalOvertimeHour += TimeSpan.Parse(OverTime).TotalHours;

        //                    }
        //                    GrandTotal = TimeTune.Reports.getWorkHourDurationAttendance(employeeID.ToString(), log.date.Value.Date.ToString("dd-MM-yyyy"), log.date.Value.Date.ToString("dd-MM-yyyy"));
        //                    GrandTotal_Hour += TimeSpan.Parse(GrandTotal).TotalHours;

        //                    if (shift != null && totalShiftHours != null && LateHour != null && ShiftEndTime != null && OverTime != null && GrandTotal != null && Total != null)
        //                    {

        //                    }

        //                    tempLogs.Add(new MonthlyTimeSheetLog()
        //                    {
        //                        remarksIn = log.date.Value.ToString("MMM yyyy"),
        //                        date = log.date.Value.ToString("dd-MM-yyyy"),
        //                        day = log.date.Value.ToString("dddd"),
        //                        status = (log.status_in + "-" + log.status_out),
        //                        overtime_status = empModel.employee_code, //Emp Code
        //                        finalRemarks = log.final_remarks, //First Name
        //                        timeIn = (log.time_in.HasValue) ? log.time_in.Value.ToString("hh:mm tt") : "",
        //                        timeOut = (log.time_out.HasValue) ? log.time_out.Value.ToString("hh:mm tt") : "",

        //                        overtime2 = totalShiftHours.ToString(), // Totoal Hours
        //                        remarksOut = LateHour, // Late Hour
        //                        terminalIn = log.terminal_in,
        //                        terminalOut = log.terminal_in,

        //                        totalLateHours = Math.Round(totalLateHours, 2),
        //                        GrandTotal_Hour = Math.Round(GrandTotal_Hour, 2),
        //                        totalOvertimeHour = Math.Round(totalOvertimeHour, 2),
        //                        totalShfit_Hour = Math.Round(totalShfit_Hour, 2)


        //                    });
        //                }
        //            }
        //            else if (log.final_remarks == "AB")
        //            {
        //                var empModel = db.employee.Where(e => e.EmployeeId == employeeID).FirstOrDefault();
        //                var shift = getShift(empModel, log.date, db);
        //                TimeSpan totalShiftHours = TimeSpan.FromSeconds(shift.shift_end);
        //                totalShfit_Hour += totalShiftHours.TotalHours;
        //                tempLogs.Add(new MonthlyTimeSheetLog()
        //                {
        //                    remarksIn = log.date.Value.ToString("MMM yyyy"),
        //                    date = log.date.Value.ToString("dd-MM-yyyy"),
        //                    day = log.date.Value.ToString("dddd"),
        //                    status = (log.status_in + "-" + log.status_out),
        //                    overtime_status = empModel.employee_code, //Emp Code
        //                    finalRemarks = log.final_remarks,
        //                    timeIn = "AB",
        //                    timeOut = "AB",

        //                    overtime2 = totalShiftHours.ToString(), // Totoal Hours
        //                    remarksOut = "00:00:00", // Late Hour
        //                    terminalIn = log.terminal_in,
        //                    terminalOut = log.terminal_in,

        //                    totalLateHours = tempLogs.Count > 0 ? tempLogs[tempLogs.Count - 1].totalLateHours : 0,
        //                    GrandTotal_Hour = tempLogs.Count > 0 ? tempLogs[tempLogs.Count - 1].GrandTotal_Hour : 0,
        //                    totalOvertimeHour = tempLogs.Count > 0 ? tempLogs[tempLogs.Count - 1].totalOvertimeHour : 0,
        //                    totalShfit_Hour = totalShfit_Hour


        //                });
        //            }
        //            else if (log.final_remarks == "OFF")
        //            {
        //                var empModel = db.employee.Where(e => e.EmployeeId == employeeID).FirstOrDefault();
        //                tempLogs.Add(new MonthlyTimeSheetLog()
        //                {
        //                    remarksIn = log.date.Value.ToString("MMM yyyy"),
        //                    date = log.date.Value.ToString("dd-MM-yyyy"),
        //                    day = log.date.Value.ToString("dddd"),
        //                    status = (log.status_in + "-" + log.status_out),
        //                    overtime_status = empModel.employee_code, //Emp Code
        //                    finalRemarks = log.final_remarks, //First Name
        //                    timeIn = "OFF",
        //                    timeOut = "OFF",

        //                    overtime2 = "00:00:00", // Totoal Hours
        //                    remarksOut = "00:00:00", // Late Hour
        //                    terminalIn = log.terminal_in,
        //                    terminalOut = log.terminal_in,

        //                    totalLateHours = tempLogs.Count > 0 ? tempLogs[tempLogs.Count - 1].totalLateHours : 0,
        //                    GrandTotal_Hour = tempLogs.Count > 0 ? tempLogs[tempLogs.Count - 1].GrandTotal_Hour : 0,
        //                    totalOvertimeHour = tempLogs.Count > 0 ? tempLogs[tempLogs.Count - 1].totalOvertimeHour : 0,
        //                    totalShfit_Hour = tempLogs.Count > 0 ? tempLogs[tempLogs.Count - 1].totalShfit_Hour : 0


        //                });
        //            }
        //            else if (log.final_remarks == "LV")
        //            {
        //                var empModel = db.employee.Where(e => e.EmployeeId == employeeID).FirstOrDefault();
        //                var shift = getShift(empModel, log.date, db);
        //                TimeSpan totalShiftHours = TimeSpan.FromSeconds(shift.shift_end);
        //                totalShfit_Hour += totalShiftHours.TotalHours;
        //                tempLogs.Add(new MonthlyTimeSheetLog()
        //                {
        //                    remarksIn = log.date.Value.ToString("MMM yyyy"),
        //                    date = log.date.Value.ToString("dd-MM-yyyy"),
        //                    day = log.date.Value.ToString("dddd"),
        //                    status = (log.status_in + "-" + log.status_out),
        //                    overtime_status = empModel.employee_code, //Emp Code
        //                    finalRemarks = log.final_remarks,
        //                    timeIn = "LV",
        //                    timeOut = "LV",

        //                    overtime2 = totalShiftHours.ToString(), // Totoal Hours
        //                    remarksOut = "00:00:00", // Late Hour
        //                    terminalIn = log.terminal_in,
        //                    terminalOut = log.terminal_in,

        //                    totalLateHours = tempLogs.Count > 0 ? tempLogs[tempLogs.Count - 1].totalLateHours : 0,
        //                    GrandTotal_Hour = tempLogs.Count > 0 ? tempLogs[tempLogs.Count - 1].GrandTotal_Hour : 0,
        //                    totalOvertimeHour = tempLogs.Count > 0 ? tempLogs[tempLogs.Count - 1].totalOvertimeHour : 0,
        //                    totalShfit_Hour = totalShfit_Hour


        //                });
        //            }

        //        }

        //        //Present
        //        toReturn.totalPresent = tempLogs.Where(m =>
        //           m.timeIn != "AB" && m.timeIn != "OFF"
        //            ).Count() + "";


        //        //Absent
        //        toReturn.totalAbsent = tempLogs.Where(m => m.timeIn == "AB").Count() + "";



        //        toReturn.logs = tempLogs.AsQueryable().SortBy("date").ToArray();
        //        return toReturn;

        //    }


        //}

        public CustomRangeTimeSheetData getWorkingHourReportAlt(int employeeID, string from, string to)
        {
        CustomRangeTimeSheetData toReturn = null;

            if (!isAuthorised(employeeID))
                return toReturn;

            using (var db = new Context())
            {
                toReturn = new CustomRangeTimeSheetData();
                Employee emp = db.employee.Find(employeeID);

                if (emp == null)
                    return null;

                toReturn.employeeCode = emp.employee_code;
                toReturn.employeeName = emp.first_name + " " + emp.last_name;

                DateTime fromDate = DateTime.ParseExact(from, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                toReturn.fromYear = fromDate.ToString("yyyy");
                toReturn.fromMonth = fromDate.ToString("MM");
                toReturn.fromDay = fromDate.ToString("dd");

                DateTime toDate = DateTime.ParseExact(to, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                toReturn.toYear = toDate.ToString("yyyy");
                toReturn.toMonth = toDate.ToString("MM");
                toReturn.toDay = toDate.ToString("dd");

                ConsolidatedAttendance[] attendanceLogs = db.consolidated_attendance
                    .Where(m => m.employee.EmployeeId.Equals(emp.EmployeeId) &&
                                m.date >= fromDate &&
                                m.date <= toDate)
                    .ToArray();

                List<MonthlyTimeSheetLog> tempLogs = new List<MonthlyTimeSheetLog>();
                double totalShfit_Hour = 0;
                double GrandTotal_Hour = 0;
                double totalLateHours = 0;
                double totalOvertimeHour = 0;

                foreach (ConsolidatedAttendance log in attendanceLogs)
                {
                    if (log.final_remarks != "AB" && log.final_remarks != "OFF" && log.final_remarks != "LV")
                    {
                        var empModel = emp;
                        var shift = getShift(empModel, log.date, db);
                        TimeSpan shiftDuration = TimeSpan.FromSeconds(shift.shift_end);
                        string ShiftEndTime = shift.start_time.AddSeconds(shift.shift_end).ToString("HH:mm");

                        string statusOut = ""; 
                        int totalShiftHoursInt = (int)Math.Floor(totalShfit_Hour);
                        int totalShiftMinutesInt = (int)Math.Round((totalShfit_Hour - totalShiftHoursInt) * 60);
                        int totalLateHoursInt = (int)Math.Floor(totalLateHours); // Extract integer part (hours)
                        int totalLateMinutesInt = (int)Math.Round((totalLateHours - totalLateHoursInt) * 60); // Extract minutes
                        int totalOvertimeHoursInt = (int)Math.Floor(totalOvertimeHour); // Extract integer part (hours)
                        int totalOvertimeMinutesInt = (int)Math.Round((totalOvertimeHour - totalOvertimeHoursInt) * 60); // Extract minutes
                        string LateHour = "-";
                        double totalShiftHours = 0;

                        if (log.time_in != null && log.time_out != null) 
                        {
                            TimeSpan inTime = DateTime.Parse(log.time_out.Value.ToString("HH:mm")).Subtract(DateTime.Parse(log.time_in.Value.ToString("HH:mm")));
                            statusOut = TimeSpan.Compare(shiftDuration, inTime) == 1 ? "Early Out" : "";

                            totalShiftHours = inTime.TotalHours;
                            totalShfit_Hour += totalShiftHours;

                            LateHour = DateTime.Parse(log.time_in.Value.ToString("HH:mm")).Subtract(DateTime.Parse(shift.start_time.ToString("HH:mm"))).ToString();

                            if (!LateHour.Contains("-"))
                            {
                                totalLateHours += TimeSpan.Parse(LateHour).TotalHours;
                            }

                            string OverTime = (log.time_out.HasValue) ? (DateTime.Parse(log.time_out.Value.ToString("HH:mm")) - DateTime.Parse(ShiftEndTime)).ToString() : "";

                            TimeSpan overtimeTimeSpan;
                            if (TimeSpan.TryParse(OverTime, out overtimeTimeSpan))
                            {
                                if (overtimeTimeSpan.TotalHours >= 0)
                                {
                                    totalOvertimeHour += overtimeTimeSpan.TotalHours;
                                }
                            }

                            // Calculate total shift hours
                            totalShiftHoursInt = (int)Math.Floor(totalShfit_Hour); // Extract integer part (hours)
                            totalShiftMinutesInt = (int)Math.Round((totalShfit_Hour - totalShiftHoursInt) * 60); // Extract minutes

                            // Calculate total late hours
                            totalLateHoursInt = (int)Math.Floor(totalLateHours); // Extract integer part (hours)
                            totalLateMinutesInt = (int)Math.Round((totalLateHours - totalLateHoursInt) * 60); // Extract minutes

                            // Calculate total overtime hours
                            totalOvertimeHoursInt = (int)Math.Floor(totalOvertimeHour); // Extract integer part (hours)
                            totalOvertimeMinutesInt = (int)Math.Round((totalOvertimeHour - totalOvertimeHoursInt) * 60); // Extract minutes

                            string totalOvertimeHoursFormatted = $"{totalOvertimeHoursInt} : {totalOvertimeMinutesInt:00}"; // Format hours and minutes
                        }

                        double GrandTotal = shiftDuration.TotalHours;
                        GrandTotal_Hour += GrandTotal;

                        tempLogs.Add(new MonthlyTimeSheetLog()
                        {
                            remarksIn = log.date.Value.ToString("MMM yyyy"),
                            date = log.date.Value.ToString("dd-MM-yyyy"),
                            day = log.date.Value.ToString("dddd"),
                            status = (log.status_in + "$" + statusOut),
                            overtime_status = empModel.employee_code, //Emp Code
                            finalRemarks = log.final_remarks, //First Name
                            timeIn = log.time_in != null ? log.time_in.Value.ToString("hh:mm tt") : "",
                            timeOut = log.time_out != null ? log.time_out.Value.ToString("hh:mm tt") : "",

                            overtime2 = totalShiftHours.ToString(), // Total Hours
                            remarksOut = LateHour, // Late Hour
                            terminalIn = log.time_in != null ? log.terminal_in : "",
                            terminalOut = log.time_out != null ? log.terminal_out : "",

                            totalLateHours = totalLateHoursInt,
                            totalLateMins = totalLateMinutesInt,
                            GrandTotal_Hour = Math.Round(GrandTotal_Hour, 2),
                            totalOvertimeHour = totalOvertimeHoursInt,
                            totalOvertimeMins = totalOvertimeMinutesInt,
                            totalShfit_Hour = totalShiftHoursInt,
                            totalShfit_Mins = totalShiftMinutesInt,
                            remarks = log.manualAttendances.Count() > 0 ? String.Join(", ", log.manualAttendances.Select(m => m.remarks).ToArray()) : "",
                        });
                    }
                    else if (log.final_remarks == "AB")
                    {
                        var empModel = db.employee.Where(e => e.EmployeeId == employeeID).FirstOrDefault();
                        var shift = getShift(empModel, log.date, db);
                        TimeSpan totalShiftHours = TimeSpan.FromSeconds(shift.shift_end);
                        double GrandTotal = totalShiftHours.TotalHours;
                        GrandTotal_Hour += GrandTotal;
                        // totalShfit_Hour += totalShiftHours.TotalHours;
                        tempLogs.Add(new MonthlyTimeSheetLog()
                        {
                            remarksIn = log.date.Value.ToString("MMM yyyy"),
                            date = log.date.Value.ToString("dd-MM-yyyy"),
                            day = log.date.Value.ToString("dddd"),
                            status = (log.status_in + "$" + log.status_out),
                            overtime_status = empModel.employee_code, //Emp Code
                            finalRemarks = log.final_remarks,
                            timeIn = "",
                            timeOut = "",
                            remarks = log.manualAttendances.Count() > 0 ? String.Join(", ", log.manualAttendances.Select(m => m.remarks).ToArray()) : "غياب",
                            overtime2 = totalShiftHours.ToString(), // Totoal Hours
                            remarksOut = "00:00:00", // Late Hour
                           
                            terminalIn = "",
                            terminalOut = "",

                            totalLateHours = tempLogs.Count > 0 ? tempLogs[tempLogs.Count - 1].totalLateHours : 0,
                            totalLateMins= tempLogs.Count > 0 ? tempLogs[tempLogs.Count - 1].totalLateMins : 0,
                            GrandTotal_Hour = Math.Round(GrandTotal_Hour, 2),
                            totalOvertimeHour = tempLogs.Count > 0 ? tempLogs[tempLogs.Count - 1].totalOvertimeHour : 0,
                            totalOvertimeMins = tempLogs.Count > 0 ? tempLogs[tempLogs.Count - 1].totalOvertimeMins : 0,
                            totalShfit_Hour = tempLogs.Count > 0 ? tempLogs[tempLogs.Count - 1].totalShfit_Hour : 0,
                            totalShfit_Mins = tempLogs.Count > 0 ? tempLogs[tempLogs.Count - 1].totalShfit_Mins : 0

                        });

                    }
                    else if (log.final_remarks == "OFF")
                    {
                        var empModel = db.employee.Where(e => e.EmployeeId == employeeID).FirstOrDefault();
                        tempLogs.Add(new MonthlyTimeSheetLog()
                        {
                            remarksIn = log.date.Value.ToString("MMM yyyy"),
                            date = log.date.Value.ToString("dd-MM-yyyy"),
                            day = log.date.Value.ToString("dddd"),
                            status = (log.status_in + "$" + log.status_out),
                            overtime_status = empModel.employee_code, //Emp Code
                            finalRemarks = log.final_remarks, //First Name
                            timeIn = "",
                            timeOut = "",

                            overtime2 = "00:00:00", // Totoal Hours
                            remarksOut = "00:00:00", // Late Hour
                                                     //terminalIn = log.terminal_in,
                                                     //terminalOut = log.terminal_in,
                            remarks = log.manualAttendances.Count() > 0 ? String.Join(", ", log.manualAttendances.Select(m => m.remarks).ToArray()) : "عطلة الاسبوع",
                            terminalIn = "",
                            terminalOut = "",

                            totalLateHours = tempLogs.Count > 0 ? tempLogs[tempLogs.Count - 1].totalLateHours : 0,
                            totalLateMins = tempLogs.Count > 0 ? tempLogs[tempLogs.Count - 1].totalLateMins : 0,
                            GrandTotal_Hour = tempLogs.Count > 0 ? tempLogs[tempLogs.Count - 1].GrandTotal_Hour : 0,
                            totalOvertimeHour = tempLogs.Count > 0 ? tempLogs[tempLogs.Count - 1].totalOvertimeHour : 0,
                            totalOvertimeMins = tempLogs.Count > 0 ? tempLogs[tempLogs.Count - 1].totalOvertimeMins : 0,
                            totalShfit_Hour = tempLogs.Count > 0 ? tempLogs[tempLogs.Count - 1].totalShfit_Hour : 0,
                            totalShfit_Mins = tempLogs.Count > 0 ? tempLogs[tempLogs.Count - 1].totalShfit_Mins : 0


                        });
                    }
                    else if (log.final_remarks == "LV")
                    {
                        var empModel = db.employee.Where(e => e.EmployeeId == employeeID).FirstOrDefault();
                        var shift = getShift(empModel, log.date, db);
                        TimeSpan totalShiftHours = TimeSpan.FromSeconds(shift.shift_end);
                        //  totalShfit_Hour += totalShiftHours.TotalHours;
                        tempLogs.Add(new MonthlyTimeSheetLog()
                        {
                            remarksIn = log.date.Value.ToString("MMM yyyy"),
                            date = log.date.Value.ToString("dd-MM-yyyy"),
                            day = log.date.Value.ToString("dddd"),
                            status = (log.status_in + "$" + log.status_out),
                            overtime_status = empModel.employee_code, //Emp Code
                            finalRemarks = log.final_remarks,
                            timeIn = "LV",
                            timeOut = "LV",

                            overtime2 = totalShiftHours.ToString(), // Totoal Hours
                            remarksOut = "00:00:00", // Late Hour
                            //terminalIn = log.terminal_in,
                            //terminalOut = log.terminal_in,
                            terminalIn = "",
                            terminalOut = "",
                            remarks = log.manualAttendances.Count() > 0 ? String.Join(", ", log.manualAttendances.Select(m => m.remarks).ToArray()) : "",
                            totalLateHours = tempLogs.Count > 0 ? tempLogs[tempLogs.Count - 1].totalLateHours : 0,
                            totalLateMins = tempLogs.Count > 0 ? tempLogs[tempLogs.Count - 1].totalLateMins : 0,
                            GrandTotal_Hour = tempLogs.Count > 0 ? tempLogs[tempLogs.Count - 1].GrandTotal_Hour : 0,
                            totalOvertimeHour = tempLogs.Count > 0 ? tempLogs[tempLogs.Count - 1].totalOvertimeHour : 0,
                            totalOvertimeMins = tempLogs.Count > 0 ? tempLogs[tempLogs.Count - 1].totalOvertimeMins : 0,
                            totalShfit_Hour = tempLogs.Count > 0 ? tempLogs[tempLogs.Count - 1].totalShfit_Hour : 0,
                            totalShfit_Mins = tempLogs.Count > 0 ? tempLogs[tempLogs.Count - 1].totalShfit_Mins : 0


                        });
                    }
                }
                //Present
                toReturn.totalPresent = tempLogs.Where(m =>
                   //m.timeIn != "AB" && m.timeIn != "OFF"
                   m.timeIn != ""
                    ).Count() + "";


                //        //Absent
                toReturn.totalAbsent = tempLogs.Where(m => m.remarks == "غياب").Count() + "";

                toReturn.logs = tempLogs.ToArray();
                return toReturn;
            }
        }




    }
}