using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DLL.Models;
using ViewModels;
using System.Globalization;
using System.Collections;
using System.Web.UI.WebControls;

namespace TimeTune
{
    public class Attendance
    {
        public static ViewModels.ReportPersistentAttendanceLog getPersistanceLog(string empCode)
        {
            using (var db = new Context())
            {
                ReportPersistentAttendanceLog attendanceLog;
                var employee = db.employee.Where(m => m.employee_code.Equals(empCode) && m.active).FirstOrDefault();
                return attendanceLog = new ViewModels.ReportPersistentAttendanceLog()
                {
                    id = employee.persistent_attendance_log.PersistentAttendanceLogId,
                    employee_code = employee.employee_code,
                    employee_first_name = employee.first_name,
                    employee_last_name = employee.last_name,
                    date = (employee.persistent_attendance_log.time_start != null && employee.persistent_attendance_log.time_start.HasValue)
                         ? employee.persistent_attendance_log.time_start.Value.Date.ToString("dd-MM-yyyy") : "",

                    time_in = (employee.persistent_attendance_log.time_in != null && employee.persistent_attendance_log.time_in.HasValue)
                         ? employee.persistent_attendance_log.time_in.Value.ToString("hh:mm:ss tt") : "",

                    time_out = (employee.persistent_attendance_log.time_out != null && employee.persistent_attendance_log.time_out.HasValue)
                     ? employee.persistent_attendance_log.time_out.Value.ToString("hh:mm:ss tt") : "",
                    terminal_in = (employee.persistent_attendance_log.terminal_in != null) ? employee.persistent_attendance_log.terminal_in : " ",
                    terminal_out = (employee.persistent_attendance_log.terminal_out != null) ? employee.persistent_attendance_log.terminal_out : " "
                };

            }
        }

        public static List<ViewModels.ConsolidatedAttendanceLog> getConsolidatedLog(string empCode)
        {
            using (Context db = new Context())
            {
                List<ConsolidatedAttendance> consolidateAttendance = null;
                try
                {
                    DateTime? date = DateTime.Now.AddDays(-30).Date;
                    consolidateAttendance = db.consolidated_attendance.Where(m => m.employee.employee_code.Equals(empCode) && m.date > date.Value && m.active).ToList();
                }
                catch (System.Data.Entity.Core.EntityCommandExecutionException ex)
                {
                    consolidateAttendance = new List<ConsolidatedAttendance>();
                }



                List<ViewModels.ConsolidatedAttendanceLog> toReturn = new List<ViewModels.ConsolidatedAttendanceLog>();

                for (int i = 0; i < consolidateAttendance.Count(); i++)
                {
                    ViewModels.ConsolidatedAttendanceLog vmConsilidateAttendance = new ViewModels.ConsolidatedAttendanceLog();
                    DLL.Models.ConsolidatedAttendance dbConsolidateAttendance = new DLL.Models.ConsolidatedAttendance();
                    //Force Load
                    dbConsolidateAttendance = consolidateAttendance[i];

                    vmConsilidateAttendance.id = dbConsolidateAttendance.ConsolidatedAttendanceId;
                    vmConsilidateAttendance.date = (dbConsolidateAttendance.date != null && dbConsolidateAttendance.date.HasValue)
                         ? dbConsolidateAttendance.date.Value.ToString("dd-MM-yyyy") : "";
                    vmConsilidateAttendance.employee_code = dbConsolidateAttendance.employee.employee_code;
                    vmConsilidateAttendance.employee_first_name = dbConsolidateAttendance.employee.first_name;
                    vmConsilidateAttendance.employee_last_name = dbConsolidateAttendance.employee.last_name;
                    vmConsilidateAttendance.time_in = (dbConsolidateAttendance.time_in != null && dbConsolidateAttendance.time_in.HasValue)
                         ? dbConsolidateAttendance.time_in.Value.TimeOfDay.ToString("c") : "";
                    vmConsilidateAttendance.status_in = dbConsolidateAttendance.status_in;
                    vmConsilidateAttendance.time_out = (dbConsolidateAttendance.time_out != null && dbConsolidateAttendance.time_out.HasValue)
                     ? dbConsolidateAttendance.time_out.Value.TimeOfDay.ToString("c") : "";
                    vmConsilidateAttendance.status_out = dbConsolidateAttendance.status_out;
                    vmConsilidateAttendance.final_remarks = dbConsolidateAttendance.final_remarks;
                    vmConsilidateAttendance.terminal_in = (dbConsolidateAttendance.terminal_in != null) ? dbConsolidateAttendance.terminal_in : " ";
                    vmConsilidateAttendance.terminal_out = (dbConsolidateAttendance.terminal_out != null) ? dbConsolidateAttendance.terminal_out : " ";
                    toReturn.Add(vmConsilidateAttendance);

                }

                return toReturn;
            }
        }


        public static int getConsolidatedLogForEmp(string search, string from, string to, string empCode, string sortOrder, int start, int length, out List<ViewModels.ConsolidatedAttendanceLog> toReturn)
        {
            int count = 0;
            DateTime? firstDayOfMonth = null;
            DateTime? lastDayOfMonth = null;


            try
            {

                if (from == null && to == null)
                {
                    firstDayOfMonth = DateTime.ParseExact(DateTime.Now.ToString("dd-MM-yyyy"), "dd-MM-yyyy", CultureInfo.InvariantCulture);
                    lastDayOfMonth = DateTime.ParseExact(DateTime.Now.AddDays(1).ToString("dd-MM-yyyy"), "dd-MM-yyyy", CultureInfo.InvariantCulture);
                }
                else
                {
                    firstDayOfMonth = DateTime.ParseExact(from, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                    lastDayOfMonth = DateTime.ParseExact(to, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                }
            }
            catch
            {

            }
            /*
            if (month != null && !month.Equals(""))
            {

                string[] components = month.Split('-');

                int year;
                int mnth;

                if (components.Length == 2)
                {
                    if (int.TryParse(components[0], out year) && int.TryParse(components[1],out mnth))
                    {
                        firstDayOfMonth = new DateTime(year, mnth, 1);
                        lastDayOfMonth = firstDayOfMonth.Value.AddMonths(1).AddDays(-1);
                    }
                }

            }*/


            using (Context db = new Context())
            {
                List<ConsolidatedAttendance> consolidateAttendance = null;
                try
                {
                    /*DateTime? date = DateTime.Now.AddDays(-30).Date;*/
                    DateTime? times;
                    string sdd = "";
                    if (search != null && search.Split(':').Count() == 2 && search.Length == 8)
                    {
                        times = DateTime.Parse(search);
                        var pp = times.Value.TimeOfDay.ToString();
                        sdd = pp.Remove(pp.Length - 3);
                    }

                    count = db.consolidated_attendance.Where(m =>
                        m.active &&
                        m.employee.employee_code.Equals(empCode)

                        &&

                        (!firstDayOfMonth.HasValue ||
                        (m.date.Value >= firstDayOfMonth.Value && m.date.Value <= lastDayOfMonth))

                        &&

                        (
                        search == null ||
                        search.Equals("") ||
                        m.employee.employee_code.Contains(search) ||
                        m.employee.first_name.ToLower().Contains(search.ToLower()) ||
                        m.employee.last_name.ToLower().Contains(search.ToLower()) ||
                        m.status_in.ToLower().Contains(search.ToLower()) ||
                        m.status_out.ToLower().Contains(search.ToLower()) ||
                        m.final_remarks.ToLower().Contains(search.ToLower()) ||
                        m.description.ToLower().Contains(search.ToLower())||
                        m.terminal_in.ToLower().Contains(search.ToLower()) ||
                        System.Data.Entity.DbFunctions.TruncateTime(m.date).ToString().Contains(search.ToLower()) ||
                        System.Data.Entity.DbFunctions.CreateTime(m.time_in.Value.Hour, m.time_in.Value.Minute, m.time_in.Value.Second).ToString().Contains(sdd.ToLower()) ||
                        System.Data.Entity.DbFunctions.CreateTime(m.time_out.Value.Hour, m.time_out.Value.Minute, m.time_out.Value.Second).ToString().Contains(sdd.ToLower()) ||
                        m.terminal_out.ToLower().Contains(search.ToLower())
                        )






                            ).Count();

                    consolidateAttendance = db.consolidated_attendance.Where(m =>
                        m.active &&
                        m.employee.employee_code.Equals(empCode)

                        &&

                        (!firstDayOfMonth.HasValue ||
                        (m.date.Value >= firstDayOfMonth.Value && m.date.Value <= lastDayOfMonth))

                        &&

                        (
                        search == null ||
                        search.Equals("") ||
                        m.employee.employee_code.Contains(search) ||
                         m.employee.first_name.ToLower().Contains(search.ToLower()) ||
                        m.employee.last_name.ToLower().Contains(search.ToLower()) ||
                        m.status_in.ToLower().Contains(search.ToLower()) ||
                        m.status_out.ToLower().Contains(search.ToLower()) ||
                        m.final_remarks.ToLower().Contains(search.ToLower()) ||
                        m.description.ToLower().Contains(search.ToLower()) ||
                        m.terminal_in.ToLower().Contains(search.ToLower()) ||
                        System.Data.Entity.DbFunctions.TruncateTime(m.date).ToString().Contains(search.ToLower()) ||
                        System.Data.Entity.DbFunctions.CreateTime(m.time_in.Value.Hour, m.time_in.Value.Minute, m.time_in.Value.Second).ToString().Contains(sdd.ToLower()) ||
                        System.Data.Entity.DbFunctions.CreateTime(m.time_out.Value.Hour, m.time_out.Value.Minute, m.time_out.Value.Second).ToString().Contains(sdd.ToLower()) ||
                        m.terminal_out.ToLower().Contains(search.ToLower())
                        )
                        ).SortBy(sortOrder).Skip(start).Take(length).ToList();



                }
                catch (System.Data.Entity.Core.EntityCommandExecutionException ex)
                {
                    consolidateAttendance = new List<ConsolidatedAttendance>();
                }



                toReturn = new List<ViewModels.ConsolidatedAttendanceLog>();

                for (int i = 0; i < consolidateAttendance.Count(); i++)
                {
                    string strOvertime = "", strOvertimeStatus = "";

                    TimeSpan timeOver = TimeSpan.FromSeconds(consolidateAttendance[i].overtime);

                    //here backslash is must to tell that colon is not the part of format, it just a character that we want in output
                    if (consolidateAttendance[i].overtime < 0)
                    {
                        strOvertime = "-" + timeOver.ToString(@"hh\:mm\:ss");
                    }
                    else
                    {
                        strOvertime = timeOver.ToString(@"hh\:mm\:ss");
                    }

                    if (consolidateAttendance[i].overtime_status == 1)
                    {
                        strOvertimeStatus = "Unapproved";
                    }
                    else if (consolidateAttendance[i].overtime_status == 2)
                    {
                        strOvertimeStatus = "Approved";
                    }
                    else
                    {
                        strOvertimeStatus = "Discard";
                    }

                    ViewModels.ConsolidatedAttendanceLog vmConsilidateAttendance = new ViewModels.ConsolidatedAttendanceLog();
                    DLL.Models.ConsolidatedAttendance dbConsolidateAttendance = new DLL.Models.ConsolidatedAttendance();
                    //Force Load
                    dbConsolidateAttendance = consolidateAttendance[i];

                    vmConsilidateAttendance.id = dbConsolidateAttendance.ConsolidatedAttendanceId;
                    vmConsilidateAttendance.date = (dbConsolidateAttendance.date != null && dbConsolidateAttendance.date.HasValue)
                         ? dbConsolidateAttendance.date.Value.ToString("dd-MM-yyyy") : "";
                    vmConsilidateAttendance.dt_date = (dbConsolidateAttendance.date != null && dbConsolidateAttendance.date.HasValue)
                         ? dbConsolidateAttendance.date.Value : DateTime.Now;
                    vmConsilidateAttendance.employee_code = dbConsolidateAttendance.employee.employee_code;
                    vmConsilidateAttendance.employee_first_name = dbConsolidateAttendance.employee.first_name;
                    vmConsilidateAttendance.employee_last_name = dbConsolidateAttendance.employee.last_name;
                    vmConsilidateAttendance.time_in = (dbConsolidateAttendance.time_in.HasValue) ? dbConsolidateAttendance.time_in.Value.ToString("hh:mm tt") : "";
                    //vmConsilidateAttendance.time_in = (dbConsolidateAttendance.time_in != null && dbConsolidateAttendance.time_in.HasValue)
                    //     ? dbConsolidateAttendance.time_in.Value.TimeOfDay.ToString("c") : "";
                    vmConsilidateAttendance.status_in = dbConsolidateAttendance.status_in;
                    //vmConsilidateAttendance.time_out = (dbConsolidateAttendance.time_out != null && dbConsolidateAttendance.time_out.HasValue)
                    // ? dbConsolidateAttendance.time_out.Value.TimeOfDay.ToString("c") : "";
                    vmConsilidateAttendance.time_out = (dbConsolidateAttendance.time_out.HasValue) ? dbConsolidateAttendance.time_out.Value.ToString("hh:mm tt") : "";
                    vmConsilidateAttendance.status_out = dbConsolidateAttendance.status_out;
                    vmConsilidateAttendance.terminal_in = (dbConsolidateAttendance.terminal_in != null) ? dbConsolidateAttendance.terminal_in : " ";
                    vmConsilidateAttendance.terminal_out = (dbConsolidateAttendance.terminal_out != null) ? dbConsolidateAttendance.terminal_out : " ";
                    vmConsilidateAttendance.final_remarks = (dbConsolidateAttendance.manualAttendances.Count > 0) ?
                        dbConsolidateAttendance.final_remarks + "*" : dbConsolidateAttendance.final_remarks;
                    vmConsilidateAttendance.description = dbConsolidateAttendance.description;
                    vmConsilidateAttendance.overtime = strOvertime;
                    vmConsilidateAttendance.overtime_status = strOvertimeStatus;

                    toReturn.Add(vmConsilidateAttendance);
                }

                //toReturn = toReturn.OrderBy(o => o.dt_date).ToList();

                return count;
            }
        }

        public static int getLeavesCountReportByEmpCode(string empCode, string session_year, out List<ViewModels.LeavesCountReportLog> toReturn)
        {
            List<ViewModels.LeavesCountReportLog> result = new List<ViewModels.LeavesCountReportLog>();

            try
            {
                using (Context db = new Context())
                {
                    string query = string.Format("SP_GetAllLeavesCountByEmployeeCode @EmployeeCode='{0}',@SessionYear='{1}'", empCode, session_year.ToString());
                    result = db.Database.SqlQuery<ViewModels.LeavesCountReportLog>(query).ToList();

                    toReturn = result;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result.Count;
        }

        public static int getLeavesBalanceReportByEmpID(int empID, string session_year, out List<ViewModels.LeavesBalanceReportLog> toReturn)
        {
            List<ViewModels.LeavesBalanceReportLog> result = new List<ViewModels.LeavesBalanceReportLog>();

            try
            {
                using (Context db = new Context())
                {
                    var data_emp = db.employee.Where(e => e.active && e.EmployeeId == empID).FirstOrDefault();
                    if (data_emp != null)
                    {
                        string query = string.Format("SP_GetAllLeavesBalanceByEmployeeCode @EmployeeCode='{0}',@SessionYear='{1}'", data_emp.employee_code, session_year.ToString());
                        result = db.Database.SqlQuery<ViewModels.LeavesBalanceReportLog>(query).ToList();

                    }

                    toReturn = result;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result.Count;
        }

        public static ArrayList getSessionYearsListByEmployeeId(int emp_id)
        {
            ArrayList yearList = new ArrayList();

            try
            {
                using (Context db = new Context())
                {
                    if (emp_id == -1)
                    {
                        var lSession = db.leave_session.GroupBy(n => new { n.YearId })
                            .Select(g => new
                            {
                                g.Key.YearId
                            }).OrderByDescending(o => o.YearId).ToList();

                        if (lSession != null && lSession.Count > 0)
                        {
                            foreach (var item in lSession)
                            {
                                if (item.YearId > 0)
                                {
                                    yearList.Add(item.YearId);
                                }
                            }
                        }
                        else
                        {
                            yearList.Add(DateTime.Now.Year);
                        }
                    }
                    else
                    {
                        var lSession = db.leave_session.Where(l => l.EmployeeId == emp_id).GroupBy(n => new { n.YearId })
                            .Select(g => new
                            {
                                g.Key.YearId
                            }).OrderByDescending(o => o.YearId).ToList();

                        if (lSession != null && lSession.Count > 0)
                        {
                            foreach (var item in lSession)
                            {
                                if (item.YearId > 0)
                                {
                                    yearList.Add(item.YearId);
                                }
                            }
                        }
                        else
                        {
                            yearList.Add(DateTime.Now.Year);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return yearList;
        }

        public static List<ViewModels.LeaveApplicationInfo> getLeaveApplicationsByEmpCode(string empCode, int session_year)
        {
            DateTime start_date = DateTime.Now; DateTime end_date = DateTime.Now;

            List<ViewModels.LeaveApplicationInfo> result = new List<ViewModels.LeaveApplicationInfo>();

            try
            {
                using (Context db = new Context())
                {
                    if (Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss.000")) > Convert.ToDateTime(DateTime.Now.Year.ToString() + "-12-31 23:59:00.000"))
                    {
                        if (Convert.ToDateTime(DateTime.Now.ToString(session_year.ToString() + "-MM-dd hh:mm:ss.000")) > Convert.ToDateTime(session_year.ToString() + "-12-31 23:59:00.000"))
                        {
                            start_date = Convert.ToDateTime(DateTime.Now.ToString((session_year - 1) + "-01-01 00:00:00.000"));
                            end_date = Convert.ToDateTime(DateTime.Now.ToString((session_year) + "-12-31 23:59:00.000"));
                        }
                        else
                        {
                            start_date = Convert.ToDateTime(DateTime.Now.ToString((session_year) + "-01-01 00:00:00.000"));
                            end_date = Convert.ToDateTime(DateTime.Now.ToString((session_year + 1) + "-12-31 23:59:00.000"));
                            session_year = session_year + 1;
                        }
                    }
                    else
                    {
                        start_date = Convert.ToDateTime(DateTime.Now.ToString((session_year) + "-01-01 00:00:00.000"));
                        end_date = Convert.ToDateTime(DateTime.Now.ToString((session_year) + "-12-31 23:59:00.000"));
                        //yearSession = yearSession + 1;
                    }

                    ////if July is started
                    //if (Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss.000")) > Convert.ToDateTime(session_year.ToString() + "-06-30 23:59:59.999"))
                    //{
                    //    start_date = Convert.ToDateTime(DateTime.Now.ToString(session_year + "-01-01 00:00:00.000"));
                    //    end_date = Convert.ToDateTime(DateTime.Now.ToString((session_year + 1) + "-06-30 23:59:59.000"));
                    //    session_year = session_year + 1;
                    //}
                    //else
                    //{
                    //    start_date = Convert.ToDateTime(DateTime.Now.ToString((session_year - 1) + "-01-01 00:00:00.000"));
                    //    end_date = Convert.ToDateTime(DateTime.Now.ToString(session_year + "-06-30 23:59:59.000"));
                    //}

                    var data_emp = db.employee.Where(e => e.active && e.employee_code == empCode).FirstOrDefault();
                    if (data_emp != null)
                    {
                        var lApplications = db.leave_application.Where(l => l.IsActive && l.EmployeeId == data_emp.EmployeeId && l.LeaveStatusId == 2 && (l.FromDate >= start_date && l.ToDate <= end_date)).OrderByDescending(o => o.Id).ToList();
                        if (lApplications != null && lApplications.Count > 0)
                        {
                            foreach (LeaveApplication item in lApplications)
                            {
                                result.Add(new LeaveApplicationInfo
                                {
                                    EmployeeId = item.EmployeeId,
                                    FromDate = item.FromDate,
                                    ToDate = item.ToDate,
                                    LeaveTypeId = item.LeaveTypeId,
                                    DaysCount = item.DaysCount
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }

            return result;
        }

        public static List<ViewModels.LeaveApplicationInfo> getLeaveApplicationsByEmployeeCodeANDSessionYear(string empCode, int session_year)
        {
            DateTime start_date = DateTime.Now; DateTime end_date = DateTime.Now;

            List<ViewModels.LeaveApplicationInfo> result = new List<ViewModels.LeaveApplicationInfo>();

            try
            {
                using (Context db = new Context())
                {
                    if (Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss.000")) > Convert.ToDateTime(DateTime.Now.Year.ToString() + "-12-31 23:59:00.000"))
                    {
                        if (Convert.ToDateTime(DateTime.Now.ToString(session_year.ToString() + "-MM-dd hh:mm:ss.000")) > Convert.ToDateTime(session_year.ToString() + "-12-31 23:59:00.000"))
                        {
                            start_date = Convert.ToDateTime(DateTime.Now.ToString((session_year - 1) + "-01-01 00:00:00.000"));
                            end_date = Convert.ToDateTime(DateTime.Now.ToString((session_year) + "-12-31 23:59:00.000"));
                        }
                        else
                        {
                            start_date = Convert.ToDateTime(DateTime.Now.ToString((session_year) + "-01-01 00:00:00.000"));
                            end_date = Convert.ToDateTime(DateTime.Now.ToString((session_year + 1) + "-12-31 23:59:00.000"));
                            session_year = session_year + 1;
                        }
                    }
                    else
                    {
                        start_date = Convert.ToDateTime(DateTime.Now.ToString((session_year) + "-01-01 00:00:00.000"));
                        end_date = Convert.ToDateTime(DateTime.Now.ToString((session_year) + "-12-31 23:59:00.000"));
                        //yearSession = yearSession + 1;
                    }

                    ////if July is started
                    //if (Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss.000")) > Convert.ToDateTime(session_year.ToString() + "-06-30 23:59:59.999"))
                    //{
                    //    start_date = Convert.ToDateTime(DateTime.Now.ToString(session_year + "-01-01 00:00:00.000"));
                    //    end_date = Convert.ToDateTime(DateTime.Now.ToString((session_year + 1) + "-06-30 23:59:59.000"));
                    //    session_year = session_year + 1;
                    //}
                    //else
                    //{
                    //    start_date = Convert.ToDateTime(DateTime.Now.ToString((session_year - 1) + "-01-01 00:00:00.000"));
                    //    end_date = Convert.ToDateTime(DateTime.Now.ToString(session_year + "-06-30 23:59:59.000"));
                    //}

                    var data_emp = db.employee.Where(e => e.active && e.employee_code == empCode).FirstOrDefault();
                    if (data_emp != null)
                    {
                        var lApplications = db.leave_application.Where(l => l.IsActive && l.EmployeeId == data_emp.EmployeeId && l.LeaveStatusId == 2 && (l.FromDate >= start_date && l.ToDate <= end_date)).OrderByDescending(o => o.Id).ToList();
                        if (lApplications != null && lApplications.Count > 0)
                        {
                            //Titles - Leave Types
                            string TitleLeaves = "", TitleLeave01 = "", TitleLeave02 = "", TitleLeave03 = "", TitleLeave04 = "", TitleLeave05 = "", TitleLeave06 = "", TitleLeave07 = "", TitleLeave08 = "";
                            string TitleLeave09 = "", TitleLeave10 = "", TitleLeave11 = "", TitleLeave12 = "", TitleLeave13 = "", TitleLeave14 = "", TitleLeave15 = "";
                            BLL.PdfReports.LeavesTypesTitles leavesTitles = new BLL.PdfReports.LeavesTypesTitles();
                            TitleLeaves = leavesTitles.getLeavesTypesTitles();
                            if (TitleLeaves != null && TitleLeaves.Contains(','))
                            {
                                string[] strSplit = TitleLeaves.Split(',');
                                if (strSplit != null && strSplit.Length > 0)
                                {
                                    TitleLeave01 = strSplit[0].ToString(); TitleLeave02 = strSplit[1].ToString(); TitleLeave03 = strSplit[2].ToString(); TitleLeave04 = strSplit[3].ToString();
                                    TitleLeave05 = strSplit[4].ToString(); TitleLeave06 = strSplit[5].ToString(); TitleLeave07 = strSplit[6].ToString(); TitleLeave08 = strSplit[7].ToString();
                                    TitleLeave09 = strSplit[8].ToString(); TitleLeave10 = strSplit[9].ToString(); TitleLeave11 = strSplit[10].ToString(); TitleLeave12 = strSplit[11].ToString();
                                    TitleLeave13 = strSplit[12].ToString(); TitleLeave14 = strSplit[13].ToString(); TitleLeave15 = strSplit[14].ToString();
                                }
                            }
                            else
                            {
                                TitleLeave01 = "Sick"; TitleLeave02 = "Casual"; TitleLeave03 = "Annual"; TitleLeave04 = "Other";
                                TitleLeave05 = "Tour"; TitleLeave06 = "Visit"; TitleLeave07 = "Meeting"; TitleLeave08 = "Maternity";
                                TitleLeave09 = "A"; TitleLeave10 = "B"; TitleLeave11 = "C"; TitleLeave04 = "D";
                                TitleLeave12 = "E"; TitleLeave13 = "F"; TitleLeave14 = "G"; TitleLeave15 = "H";
                            }
                            //////////////////////////////////////////////////////////

                            foreach (LeaveApplication item in lApplications)
                            {
                                string strLeaveType = "";
                                if (item.LeaveTypeId == 1)
                                {
                                    strLeaveType = TitleLeave01 + " Leave";
                                }
                                else if (item.LeaveTypeId == 2)
                                {
                                    strLeaveType = TitleLeave02 + " Leave";
                                }
                                else if (item.LeaveTypeId == 3)
                                {
                                    strLeaveType = TitleLeave03 + " Leave";
                                }
                                else if (item.LeaveTypeId == 4)
                                {
                                    strLeaveType = TitleLeave04 + " Leave";
                                }
                                else if (item.LeaveTypeId == 5)
                                {
                                    strLeaveType = TitleLeave05 + " Leave";
                                }
                                else if (item.LeaveTypeId == 6)
                                {
                                    strLeaveType = TitleLeave06 + " Leave";
                                }
                                else if (item.LeaveTypeId == 7)
                                {
                                    strLeaveType = TitleLeave07 + " Leave";
                                }
                                else if (item.LeaveTypeId == 8)
                                {
                                    strLeaveType = TitleLeave08 + " Leave";
                                }
                                else if (item.LeaveTypeId == 9)
                                {
                                    strLeaveType = TitleLeave09 + " Leave";
                                }
                                else if (item.LeaveTypeId == 10)
                                {
                                    strLeaveType = TitleLeave10 + " Leave";
                                }
                                else if (item.LeaveTypeId == 11)
                                {
                                    strLeaveType = TitleLeave11 + " Leave";
                                }
                                else if (item.LeaveTypeId == 12)
                                {
                                    strLeaveType = TitleLeave12 + " Leave";
                                }
                                else if (item.LeaveTypeId == 13)
                                {
                                    strLeaveType = TitleLeave13 + " Leave";
                                }
                                else if (item.LeaveTypeId == 14)
                                {
                                    strLeaveType = TitleLeave14 + " Leave";
                                }
                                else if (item.LeaveTypeId == 15)
                                {
                                    strLeaveType = TitleLeave15 + " Leave";
                                }
                                
                                string strLeaveStatus = "";
                                if (item.LeaveStatusId == 1)
                                {
                                    strLeaveStatus = "Pending";
                                }
                                else if (item.LeaveStatusId == 2)
                                {
                                    strLeaveStatus = "Approved";
                                }
                                else if (item.LeaveStatusId == 3)
                                {
                                    strLeaveStatus = "Rejected";
                                }

                                result.Add(new LeaveApplicationInfo
                                {
                                    Id = item.Id,
                                    EmployeeId = item.EmployeeId,
                                    FromDateText = item.FromDate.ToString("dd-MM-yyyy"),
                                    ToDateText = item.ToDate.ToString("dd-MM-yyyy"),
                                    LeaveTypeText = strLeaveType,
                                    DaysCount = item.DaysCount,
                                    LeaveStatusText = strLeaveStatus,
                                    LeaveReasonText = item.LeaveReason.LeaveReasonText
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }

            return result;
        }

        public static List<ViewModels.LeaveApplicationInfo> getLeaveApplicationsByEmpID(int empId, int yearSession)
        {
            DateTime start_date = DateTime.Now; DateTime end_date = DateTime.Now;

            List<ViewModels.LeaveApplicationInfo> result = new List<ViewModels.LeaveApplicationInfo>();

            try
            {
                using (Context db = new Context())
                {
                    //if July is started - 2nd July 2018 > 2018-06-30
                    if (Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss.000")) > Convert.ToDateTime(DateTime.Now.Year.ToString() + "-12-31 23:59:00.000"))
                    {
                        if (Convert.ToDateTime(DateTime.Now.ToString(yearSession.ToString() + "-MM-dd hh:mm:ss.000")) > Convert.ToDateTime(yearSession.ToString() + "-12-31 23:59:00.000"))
                        {
                            start_date = Convert.ToDateTime(DateTime.Now.ToString((yearSession - 1) + "-01-01 00:00:00.000"));
                            end_date = Convert.ToDateTime(DateTime.Now.ToString((yearSession) + "-12-31 23:59:00.000"));
                        }
                        else
                        {
                            start_date = Convert.ToDateTime(DateTime.Now.ToString((yearSession) + "-01-01 00:00:00.000"));
                            end_date = Convert.ToDateTime(DateTime.Now.ToString((yearSession + 1) + "-12-31 23:59:00.000"));
                            yearSession = yearSession + 1;
                        }
                    }
                    else
                    {
                        start_date = Convert.ToDateTime(DateTime.Now.ToString((yearSession) + "-01-01 00:00:00.000"));
                        end_date = Convert.ToDateTime(DateTime.Now.ToString((yearSession) + "-12-31 23:59:00.000"));
                        //yearSession = yearSession + 1;
                    }

                    var data_emp = db.employee.Where(e => e.active && e.EmployeeId == empId).FirstOrDefault();
                    if (data_emp != null)
                    {
                        var lApplications = db.leave_application.Where(l => l.IsActive && l.EmployeeId == data_emp.EmployeeId && l.LeaveStatusId == 2 && (l.FromDate >= start_date && l.ToDate <= end_date)).OrderByDescending(o => o.Id).ToList();
                        if (lApplications != null && lApplications.Count > 0)
                        {
                            foreach (LeaveApplication item in lApplications)
                            {
                                result.Add(new LeaveApplicationInfo
                                {
                                    EmployeeId = item.EmployeeId,
                                    FromDate = item.FromDate,
                                    ToDate = item.ToDate,
                                    LeaveTypeId = item.LeaveTypeId,
                                    DaysCount = item.DaysCount
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }

            return result;
        }


        public static int getLeavesCountReportForLM(string empCode, out List<ViewModels.LeavesCountReportLog> toReturn)
        {

            List<ViewModels.LeavesCountReportLog> result = new List<ViewModels.LeavesCountReportLog>();
            try
            {
                using (Context db = new Context())
                {
                    string query = string.Format("SP_GetAllLeavesCountForLM @EmployeeCode='{0}'", empCode);
                    result = db.Database.SqlQuery<ViewModels.LeavesCountReportLog>(query).ToList();

                    toReturn = result;
                }

            }
            catch (Exception ex)
            {

                throw ex;
            }

            return result.Count;

        }

    }
}
