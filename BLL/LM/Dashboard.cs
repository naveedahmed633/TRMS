using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewModels;
using DLL.Models;
using System.Globalization;
using System.Web;
using System.Web.UI.WebControls;
namespace TimeTune
{
    public class Dashboard
    {
        public static AttendenceCounter getAttendenceCounterForHR()
        {
            AttendenceCounter result = new AttendenceCounter();

            DateTime dtCurrentMonth = DateTime.Now;
            DateTime month_start_date = new DateTime(dtCurrentMonth.Year, dtCurrentMonth.Month, 1);
            DateTime month_end_date = new DateTime(dtCurrentMonth.Year, dtCurrentMonth.Month, System.DateTime.DaysInMonth(dtCurrentMonth.Year, dtCurrentMonth.Month));

            using (var db = new Context())
            {
                string query = string.Format("SP_AttendanceCounterForHR @FromDate='{0}', @ToDate='{1}'", month_start_date.ToString("yyyy-MM-dd 00:00:00.000"), month_end_date.ToString("yyyy-MM-dd 23:59:59.999"));
                result = db.Database.SqlQuery<AttendenceCounter>(query).FirstOrDefault();
            }

            return result;
        }

        public static ViewModels.Dashboard getDashboardValuesMyTeamLM(string empCode)
        {
            int onTime = 0, present = 0, late = 0, earlyGone = 0, absent = 0, leave = 0;
            string headName = "";
            ViewModels.Dashboard dashboard = new ViewModels.Dashboard();
            string employeeName = "";
            string supervisor_name = "";
            using (var db = new Context())
            {
                int thisMonth = DateTime.Now.Month;
                int thisYear = DateTime.Now.Year;
                var emp = db.employee.Where(p => p.employee_code == empCode).FirstOrDefault();

                if (emp != null && emp.Group != null)
                {


                    int sup_id = emp.Group.GroupId;
                    var tagEmp = db.employee.Where(c => c.Group.GroupId == sup_id).ToList();

                    for (int i = 0; i < tagEmp.Count; i++)
                    {
                        string Id = tagEmp[i].employee_code;

                        // Get all the consolidated logs for this employee, from this month and year.
                        var consolidateLog = db.consolidated_attendance.Where(m => m.active &&
                            m.employee.active && m.employee.timetune_active && m.employee.employee_code != "000000" &&
                            m.date.Value.Month.Equals(thisMonth) &&
                            m.date.Value.Year.Equals(thisYear) &&
                            m.employee.employee_code.Equals(Id)).ToList();

                        onTime += consolidateLog.Where(m => m.final_remarks.Equals("PO") || m.final_remarks.Equals("POE") || m.final_remarks.Equals("POM")).Count();
                        late += consolidateLog.Where(m => m.final_remarks.Equals("PLO") || m.final_remarks.Equals("PLE") || m.final_remarks.Equals("PLM")).Count();
                        absent += consolidateLog.Where(m => m.final_remarks.Equals("AB")).Count();
                        leave += consolidateLog.Where(m => m.final_remarks.Equals("LV")).Count();
                        earlyGone += consolidateLog.Where(m => m.final_remarks.Equals("PLE") || m.final_remarks.Equals("POE") || m.final_remarks.Equals("PME")).Count();
                        present += consolidateLog.Where(m => m.final_remarks != "AB" && m.final_remarks != "LV" && m.final_remarks != "OFF").Count();
                        // var emp = db.employee.Where(c => c.employee_code == empCode).FirstOrDefault();
                        // employeeName = emp.first_name + " " + emp.last_name;
                    }

                }


                dashboard.headName = headName;
                dashboard.supervisorName = employeeName;
                dashboard.dashboardElement.onTime = onTime;
                dashboard.dashboardElement.absent = absent;
                dashboard.dashboardElement.leave = leave;
                dashboard.dashboardElement.late = late;
                dashboard.dashboardElement.earlyGone = earlyGone;
                dashboard.dashboardElement.present = present;
                dashboard.supervisorName = supervisor_name;
                dashboard.name = employeeName;

                //return dashboard;
            }

            return dashboard;
        }

        public static ViewModels.Dashboard getDashboardValues(string empCode)
        {
            int user_id = 0;
            int onTime = 0, late = 0, earlyGone = 0, absent = 0, off = 0;
            int present = 0, presentLast = 0, dThisLeaves = 0, dLastLeaves = 0;
            int onTimeLast = 0, lateLast = 0, earlyGoneLast = 0, absentLast = 0, offLast = 0;

            int iTotalDaysThisMonth = (DateTime.Now.Day - 1); //DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month);
            int iTotalDaysLastMonth = DateTime.DaysInMonth(DateTime.Now.AddMonths(-1).Year, DateTime.Now.AddMonths(-1).Month);

            string headName = "";
            string employeeName = "";
            string supervisor_name = "";
            using (var db = new Context())
            {
                int thisMonth = DateTime.Now.Month;
                int thisYear = DateTime.Now.Year;

                //leaves
                var data_user = db.employee.Where(e => e.active && e.employee_code == empCode).FirstOrDefault();
                if (data_user != null)
                {
                    user_id = data_user.EmployeeId;

                    DateTime dtThisStart = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1, 0, 0, 0);
                    DateTime dtThisEnd = DateTime.Now;

                    if (dtThisEnd.Day == 1)
                    {
                        dtThisEnd = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1, 23, 59, 0);
                    }
                    else
                    {
                        dtThisEnd = new DateTime(DateTime.Now.Year, DateTime.Now.Month, (DateTime.Now.Day - 1), 23, 59, 0);
                    }

                    DateTime dtLastStart = new DateTime(DateTime.Now.AddMonths(-1).Year, DateTime.Now.AddMonths(-1).Month, 1, 0, 0, 0);
                    DateTime dtLastEnd = new DateTime(DateTime.Now.AddMonths(-1).Year, DateTime.Now.AddMonths(-1).Month, DateTime.DaysInMonth(DateTime.Now.AddMonths(-1).Year, DateTime.Now.AddMonths(-1).Month), 23, 59, 0);

                    if (dtThisStart != null && dtThisEnd != null && dtLastStart != null && dtLastEnd != null)
                    {
                        //this
                        var listThisLeaves = db.leave_application.Where(l => l.EmployeeId == user_id && l.IsActive && l.LeaveStatusId == 2 && (l.LeaveTypeId >= 1 && l.LeaveTypeId <= 15) && (l.FromDate >= dtThisStart && l.ToDate < dtThisEnd)).ToList();
                        if (listThisLeaves != null && listThisLeaves.Count > 0)
                        {
                            foreach (var i in listThisLeaves)
                            {
                                dThisLeaves += i.DaysCount;
                            }
                        }

                        //last
                        var listLastLeaves = db.leave_application.Where(l => l.EmployeeId == user_id && l.IsActive && l.LeaveStatusId == 2 && (l.LeaveTypeId >= 1 && l.LeaveTypeId <= 15) && (l.FromDate >= dtLastStart && l.ToDate <= dtLastEnd)).ToList();
                        if (listLastLeaves != null && listLastLeaves.Count > 0)
                        {
                            foreach (var i in listLastLeaves)
                            {
                                dLastLeaves += i.DaysCount;
                            }
                        }
                    }
                }

                // Get all the consolidated logs for this employee, from this month and year.
                var consolidateLog = db.consolidated_attendance.Where(m => m.active &&
                    m.date.Value.Month.Equals(thisMonth) &&
                    m.date.Value.Year.Equals(thisYear) &&
                    m.employee.employee_code.Equals(empCode)).ToList();

                var data_ot = consolidateLog.Where(m => m.final_remarks.Equals("PO") || m.final_remarks.Equals("POE") || m.final_remarks.Equals("POM")).ToList();
                if (data_ot != null && data_ot.Count > 0)
                    onTime = data_ot.Count();
                else
                    onTime = 0;

                var data_lt = consolidateLog.Where(m => m.final_remarks.Equals("PLO") || m.final_remarks.Equals("PLE") || m.final_remarks.Equals("PLM")).ToList();
                if (data_lt != null && data_lt.Count > 0)
                    late = data_lt.Count();
                else
                    late = 0;

                var data_ab = consolidateLog.Where(m => m.final_remarks.Equals("AB")).ToList();
                if (data_ab != null && data_ab.Count > 0)
                    absent = data_ab.Count();
                else
                    absent = 0;

                var data_off = consolidateLog.Where(m => m.final_remarks.Equals("OFF")).ToList();
                if (data_off != null && data_off.Count > 0)
                    off = data_off.Count();
                else
                    off = 0;

                //var data_lv = consolidateLog.Where(m => m.final_remarks.Equals("LV")).ToList();
                //if (data_lv != null && data_lv.Count > 0)
                //    leave = data_lv.Count();
                //else
                //    leave = 0;

                var data_eg = consolidateLog.Where(m => m.final_remarks.Equals("PLE") || m.final_remarks.Equals("POE") || m.final_remarks.Equals("PME")).ToList();
                if (data_eg != null && data_eg.Count > 0)
                    earlyGone = data_eg.Count();
                else
                    earlyGone = 0;

                ////present = decimal.Parse(iTotalDaysThisMonth.ToString()) - (decimal.Parse(absent.ToString()) + decimal.Parse(off.ToString()) + dThisLeaves);

                //var data_pr = consolidateLog.Where(m => m.final_remarks != "AB" && m.final_remarks != "LV" && m.final_remarks != "OFF").ToList();
                var data_pr = consolidateLog.Where(m => (m.final_remarks.Contains("P") || m.final_remarks.Equals("OV") || m.final_remarks.Equals("OD") || m.final_remarks.Equals("OT") || m.final_remarks.Equals("POOD") || m.final_remarks.Equals("ODPO"))).ToList();
                if (data_pr != null && data_pr.Count > 0)
                    present = data_pr.Count();
                else
                    present = 0;

                //onTime = consolidateLog.Where(m => m.final_remarks.Equals("PO") || m.final_remarks.Equals("POE") || m.final_remarks.Equals("POM")).Count();
                //late = consolidateLog.Where(m => m.final_remarks.Equals("PLO") || m.final_remarks.Equals("PLE") || m.final_remarks.Equals("PLM")).Count();
                //absent = consolidateLog.Where(m => m.final_remarks.Equals("AB")).Count();
                //leave = consolidateLog.Where(m => m.final_remarks.Equals("LV")).Count();
                //earlyGone = consolidateLog.Where(m => m.final_remarks.Equals("PLE") || m.final_remarks.Equals("POE") || m.final_remarks.Equals("PME")).Count();
                //present = consolidateLog.Where(m => m.final_remarks != "AB" && m.final_remarks != "LV" && m.final_remarks != "OFF").Count();



                var emp = db.employee.Where(c => c.employee_code == empCode).FirstOrDefault();
                employeeName = emp.first_name + " " + emp.last_name;
                //if (emp.Group != null)
                //{
                //    int sup_id = emp.Group.supervisor_id;
                //    var supervisor = db.employee.Where(c => c.EmployeeId == sup_id).FirstOrDefault();
                //    if(supervisor!=null)
                //    {
                //        supervisor_name = supervisor.first_name + " " + supervisor.last_name;
                //    }
                //    else
                //    {
                //        supervisor_name = "";
                //    }

                //}
                //else{
                //    supervisor_name="";
                //}
                //    var tagging = db.super_line_manager_tagging.Where(c => c.taggedEmployee.employee_code == empCode).FirstOrDefault();
                //    if (tagging != null)
                //    {
                //        headName = tagging.superLineManager.first_name + " " + tagging.superLineManager.last_name;
                //    }
                //    else
                //    {
                //        headName = "";
                //    }

                int lastMonth = DateTime.Now.AddMonths(-1).Month;
                int lastYear = DateTime.Now.AddMonths(-1).Year;

                // Get all the consolidated logs for this employee, from this month and year.
                var consolidateLogLastMonth = db.consolidated_attendance.Where(m => m.active &&
                    m.date.Value.Month.Equals(lastMonth) &&
                    m.date.Value.Year.Equals(lastYear) &&
                    m.employee.employee_code.Equals(empCode)).ToList();

                var data_ot_last = consolidateLogLastMonth.Where(m => m.final_remarks.Equals("PO") || m.final_remarks.Equals("POE") || m.final_remarks.Equals("POM")).ToList();
                if (data_ot_last != null && data_ot_last.Count > 0)
                    onTimeLast = data_ot_last.Count();
                else
                    onTimeLast = 0;

                var data_lt_last = consolidateLogLastMonth.Where(m => m.final_remarks.Equals("PLO") || m.final_remarks.Equals("PLE") || m.final_remarks.Equals("PLM")).ToList();
                if (data_lt_last != null && data_lt_last.Count > 0)
                    lateLast = data_lt_last.Count();
                else
                    lateLast = 0;

                var data_ab_last = consolidateLogLastMonth.Where(m => m.final_remarks.Equals("AB")).ToList();
                if (data_ab_last != null && data_ab_last.Count > 0)
                    absentLast = data_ab_last.Count();
                else
                    absentLast = 0;

                var data_off_last = consolidateLogLastMonth.Where(m => m.final_remarks.Equals("OFF")).ToList();
                if (data_off_last != null && data_off_last.Count > 0)
                    offLast = data_off_last.Count();
                else
                    offLast = 0;

                //var data_lv_last = consolidateLogLastMonth.Where(m => m.final_remarks.Equals("LV")).ToList();
                //if (data_lv_last != null && data_lv_last.Count > 0)
                //    leaveLast = data_lv_last.Count();
                //else
                //    leaveLast = 0;

                var data_eg_last = consolidateLogLastMonth.Where(m => m.final_remarks.Equals("PLE") || m.final_remarks.Equals("POE") || m.final_remarks.Equals("PME")).ToList();
                if (data_eg_last != null && data_eg_last.Count > 0)
                    earlyGoneLast = data_eg_last.Count();
                else
                    earlyGoneLast = 0;

                ////presentLast = decimal.Parse(iTotalDaysLastMonth.ToString()) - (decimal.Parse(absentLast.ToString()) + decimal.Parse(offLast.ToString()) + dLastLeaves);

                //var data_pr_last = consolidateLogLastMonth.Where(m => m.final_remarks != "AB" && m.final_remarks != "LV" && m.final_remarks != "OFF").ToList();
                var data_pr_last = consolidateLogLastMonth.Where(m => (m.final_remarks.Contains("P") || m.final_remarks.Equals("OV") || m.final_remarks.Equals("OD") || m.final_remarks.Equals("OT") || m.final_remarks.Equals("POOD") || m.final_remarks.Equals("ODPO"))).ToList();
                if (data_pr_last != null && data_pr_last.Count > 0)
                    presentLast = data_pr_last.Count();
                else
                    presentLast = 0;
            }

            ViewModels.Dashboard dashboard = new ViewModels.Dashboard();
            dashboard.headName = headName;
            dashboard.supervisorName = employeeName;
            dashboard.supervisorName = supervisor_name;
            dashboard.name = employeeName;

            if (DateTime.Now.Day == 1)
            {
                dashboard.dashboardElement.present = 0;
                dashboard.dashboardElement.absent = 0;
                dashboard.dashboardElement.leave = 0;
                dashboard.dashboardElement.off = 0;
                dashboard.dashboardElement.onTime = 0;
                dashboard.dashboardElement.late = 0;
                dashboard.dashboardElement.earlyGone = 0;
            }
            else
            {
                dashboard.dashboardElement.present = present;
                dashboard.dashboardElement.absent = absent;
                dashboard.dashboardElement.leave = dThisLeaves;
                dashboard.dashboardElement.off = off;
                dashboard.dashboardElement.onTime = onTime;
                dashboard.dashboardElement.late = late;
                dashboard.dashboardElement.earlyGone = earlyGone;
            }

            dashboard.dashboardElement.presentLast = presentLast;
            dashboard.dashboardElement.absentLast = absentLast;
            dashboard.dashboardElement.leaveLast = dLastLeaves;
            dashboard.dashboardElement.offLast = offLast;
            dashboard.dashboardElement.onTimeLast = onTimeLast;
            dashboard.dashboardElement.lateLast = lateLast;
            dashboard.dashboardElement.earlyGoneLast = earlyGoneLast;

            /*
            if (DateTime.Now.Day >= 1 && DateTime.Now.Day <= 5)
            {
                ShortLeaveWaveProcess();

                LeavesDeductionOnLate();
            }
            */

            return dashboard;

        }

        public static ViewModels.Dashboard getDashboardValuesLM(string empCode)
        {
            int user_id = 0;
            int onTime = 0, late = 0, earlyGone = 0, absent = 0, off = 0;
            int present = 0, dThisLeaves = 0;

            int iTotalDaysThisMonth = (DateTime.Now.Day - 1); //DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month);
            int iTotalDaysLastMonth = DateTime.DaysInMonth(DateTime.Now.AddMonths(-1).Year, DateTime.Now.AddMonths(-1).Month);

            string headName = "";
            string employeeName = "";
            string supervisor_name = "";
            using (var db = new Context())
            {
                int thisMonth = DateTime.Now.Month;
                int thisYear = DateTime.Now.Year;

                //leaves
                var data_user = db.employee.Where(e => e.active && e.employee_code == empCode).FirstOrDefault();
                if (data_user != null)
                {
                    user_id = data_user.EmployeeId;

                    DateTime dtThisStart = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1, 0, 0, 0);
                    DateTime dtThisEnd = DateTime.Now;

                    if (dtThisEnd.Day == 1)
                    {
                        dtThisEnd = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1, 23, 59, 0);
                    }
                    else
                    {
                        dtThisEnd = new DateTime(DateTime.Now.Year, DateTime.Now.Month, (DateTime.Now.Day - 1), 23, 59, 0);
                    }

                   
                    if (dtThisStart != null && dtThisEnd != null)
                    {
                        //this
                        var listThisLeaves = db.leave_application.Where(l => l.EmployeeId == user_id && l.IsActive && l.LeaveStatusId == 2 && (l.LeaveTypeId >= 1 && l.LeaveTypeId <= 15) && (l.FromDate >= dtThisStart && l.ToDate < dtThisEnd)).ToList();
                        if (listThisLeaves != null && listThisLeaves.Count > 0)
                        {
                            foreach (var i in listThisLeaves)
                            {
                                dThisLeaves += i.DaysCount;
                            }
                        }

                    }
                }

                // Get all the consolidated logs for this employee, from this month and year.
                var consolidateLog = db.consolidated_attendance.Where(m => m.active &&
                    m.date.Value.Month.Equals(thisMonth) &&
                    m.date.Value.Year.Equals(thisYear) &&
                    m.employee.employee_code.Equals(empCode)).ToList();

                var data_ot = consolidateLog.Where(m => m.final_remarks.Equals("PO") || m.final_remarks.Equals("POE") || m.final_remarks.Equals("POM")).ToList();
                if (data_ot != null && data_ot.Count > 0)
                    onTime = data_ot.Count();
                else
                    onTime = 0;

                var data_lt = consolidateLog.Where(m => m.final_remarks.Equals("PLO") || m.final_remarks.Equals("PLE") || m.final_remarks.Equals("PLM")).ToList();
                if (data_lt != null && data_lt.Count > 0)
                    late = data_lt.Count();
                else
                    late = 0;

                var data_ab = consolidateLog.Where(m => m.final_remarks.Equals("AB")).ToList();
                if (data_ab != null && data_ab.Count > 0)
                    absent = data_ab.Count();
                else
                    absent = 0;

                var data_off = consolidateLog.Where(m => m.final_remarks.Equals("OFF")).ToList();
                if (data_off != null && data_off.Count > 0)
                    off = data_off.Count();
                else
                    off = 0;

                //var data_lv = consolidateLog.Where(m => m.final_remarks.Equals("LV")).ToList();
                //if (data_lv != null && data_lv.Count > 0)
                //    leave = data_lv.Count();
                //else
                //    leave = 0;

                var data_eg = consolidateLog.Where(m => m.final_remarks.Equals("PLE") || m.final_remarks.Equals("POE") || m.final_remarks.Equals("PME")).ToList();
                if (data_eg != null && data_eg.Count > 0)
                    earlyGone = data_eg.Count();
                else
                    earlyGone = 0;

                ////present = decimal.Parse(iTotalDaysThisMonth.ToString()) - (decimal.Parse(absent.ToString()) + decimal.Parse(off.ToString()) + dThisLeaves);

                //var data_pr = consolidateLog.Where(m => m.final_remarks != "AB" && m.final_remarks != "LV" && m.final_remarks != "OFF").ToList();
                var data_pr = consolidateLog.Where(m => (m.final_remarks.Contains("P") || m.final_remarks.Equals("OV") || m.final_remarks.Equals("OD") || m.final_remarks.Equals("OT") || m.final_remarks.Equals("POOD") || m.final_remarks.Equals("ODPO"))).ToList();
                if (data_pr != null && data_pr.Count > 0)
                    present = data_pr.Count();
                else
                    present = 0;

               
            }

            ViewModels.Dashboard dashboard = new ViewModels.Dashboard();
            dashboard.headName = headName;
            dashboard.supervisorName = employeeName;
            dashboard.supervisorName = supervisor_name;
            dashboard.name = employeeName;

            if (DateTime.Now.Day == 1)
            {
                dashboard.dashboardElement.present = 0;
                dashboard.dashboardElement.absent = 0;
                dashboard.dashboardElement.leave = 0;
                dashboard.dashboardElement.off = 0;
                dashboard.dashboardElement.onTime = 0;
                dashboard.dashboardElement.late = 0;
                dashboard.dashboardElement.earlyGone = 0;
            }
            else
            {
                dashboard.dashboardElement.present = present;
                dashboard.dashboardElement.absent = absent;
                dashboard.dashboardElement.leave = dThisLeaves;
                dashboard.dashboardElement.off = off;
                dashboard.dashboardElement.onTime = onTime;
                dashboard.dashboardElement.late = late;
                dashboard.dashboardElement.earlyGone = earlyGone;
            }

            return dashboard;
        }

        public static ViewModels.Dashboard getDashboardValues_BK(string empCode)
        {
            int onTime = 0, present = 0, late = 0, earlyGone = 0, absent = 0, leave = 0;
            string headName = "";
            string employeeName = "";
            string supervisor_name = "";
            using (var db = new Context())
            {
                int thisMonth = DateTime.Now.Month;
                int thisYear = DateTime.Now.Year;

                // Get all the consolidated logs for this employee, from this month and year.
                var consolidateLog = db.consolidated_attendance.Where(m => m.active && m.employee.active
                    && m.employee.timetune_active && m.employee.employee_code != "000000" &&
                    m.date.Value.Month.Equals(thisMonth) &&
                    m.date.Value.Year.Equals(thisYear) &&
                    m.employee.employee_code.Equals(empCode)).ToList();

                if (consolidateLog != null && consolidateLog.Count > 0)
                {
                    //foreach (var c in consolidateLog)
                    //{
                    //    var mAttendance = db.manual_attendance.Where(m => m.active &&
                    //    m.employee.EmployeeId == c.employee.EmployeeId &&
                    //    m.ConsolidatedAttendance.ConsolidatedAttendanceId == c.ConsolidatedAttendanceId)
                    //    .OrderByDescending(o => o.ManualAttendanceId).FirstOrDefault();

                    //    if (mAttendance != null)
                    //    {
                    //        if (c.final_remarks != mAttendance.remarks)
                    //        {
                    //            c.final_remarks = mAttendance.remarks;
                    //        }
                    //    }
                    //}

                    onTime = consolidateLog.Where(m => m.final_remarks.Equals("PO") || m.final_remarks.Equals("POE") || m.final_remarks.Equals("POM")).Count();
                    late = consolidateLog.Where(m => m.final_remarks.Equals("PLO") || m.final_remarks.Equals("PLE") || m.final_remarks.Equals("PLM")).Count();
                    absent = consolidateLog.Where(m => m.final_remarks.Equals("AB")).Count();
                    leave = consolidateLog.Where(m => m.final_remarks.Equals("LV")).Count();
                    earlyGone = consolidateLog.Where(m => m.final_remarks.Equals("PLE") || m.final_remarks.Equals("POE") || m.final_remarks.Equals("PME")).Count();
                    present = consolidateLog.Where(m => m.final_remarks != "AB" && m.final_remarks != "LV" && m.final_remarks != "OFF").Count();

                    //if (emp.Group != null)
                    //{
                    //    int sup_id = emp.Group.supervisor_id;
                    //    var supervisor = db.employee.Where(c => c.EmployeeId == sup_id).FirstOrDefault();
                    //    if(supervisor!=null)
                    //    {
                    //        supervisor_name = supervisor.first_name + " " + supervisor.last_name;
                    //    }
                    //    else
                    //    {
                    //        supervisor_name = "";
                    //    }

                    //}
                    //else{
                    //    supervisor_name="";
                    //}
                    //    var tagging = db.super_line_manager_tagging.Where(c => c.taggedEmployee.employee_code == empCode).FirstOrDefault();
                    //    if (tagging != null)
                    //    {
                    //        headName = tagging.superLineManager.first_name + " " + tagging.superLineManager.last_name;
                    //    }
                    //    else
                    //    {
                    //        headName = "";
                    //    }
                }

                var emp = db.employee.Where(c => c.employee_code == empCode).FirstOrDefault();
                employeeName = emp.first_name + " " + emp.last_name;
            }

            ViewModels.Dashboard dashboard = new ViewModels.Dashboard();
            dashboard.headName = headName;
            dashboard.supervisorName = employeeName;
            dashboard.dashboardElement.onTime = onTime;
            dashboard.dashboardElement.absent = absent;
            dashboard.dashboardElement.leave = leave;
            dashboard.dashboardElement.late = late;
            dashboard.dashboardElement.earlyGone = earlyGone;
            dashboard.dashboardElement.present = present;
            dashboard.supervisorName = supervisor_name;
            dashboard.name = employeeName;

            return dashboard;

        }

        public static ViewModels.Dashboard getDashboardValuesForHR()
        {
            int onTime = 0, present = 0, late = 0, earlyGone = 0, absent = 0, leave = 0;
            string headName = "";
            string employeeName = "";
            string supervisor_name = "";
            using (var db = new Context())
            {
                int thisMonth = DateTime.Now.Month;
                int thisYear = DateTime.Now.Year;

                // Get all the consolidated logs for this employee, from this month and year.
                var consolidateLog = db.consolidated_attendance.Where(m => m.active
                    && m.employee.active
                    && m.employee.timetune_active && m.employee.employee_code != "000000" &&
                    m.date.Value.Month.Equals(thisMonth) &&
                    m.date.Value.Year.Equals(thisYear)).ToList();

                if (consolidateLog != null && consolidateLog.Count > 0)
                {
                    //foreach (var c in consolidateLog)
                    //{
                    //    var mAttendance = db.manual_attendance.Where(m => m.active &&
                    //    m.employee.EmployeeId == c.employee.EmployeeId &&
                    //    m.ConsolidatedAttendance.ConsolidatedAttendanceId == c.ConsolidatedAttendanceId)
                    //    .OrderByDescending(o => o.ManualAttendanceId).FirstOrDefault();

                    //    if (mAttendance != null)
                    //    {
                    //        if (c.final_remarks != mAttendance.remarks)
                    //        {
                    //            c.final_remarks = mAttendance.remarks;
                    //        }
                    //    }
                    //}

                    onTime = consolidateLog.Where(m => m.final_remarks.Equals("PO") || m.final_remarks.Equals("POE") || m.final_remarks.Equals("POM")).Count();
                    late = consolidateLog.Where(m => m.final_remarks.Equals("PLO") || m.final_remarks.Equals("PLE") || m.final_remarks.Equals("PLM")).Count();
                    absent = consolidateLog.Where(m => m.final_remarks.Equals("AB")).Count();
                    leave = consolidateLog.Where(m => m.final_remarks.Equals("LV")).Count();
                    earlyGone = consolidateLog.Where(m => m.final_remarks.Equals("PLE") || m.final_remarks.Equals("POE") || m.final_remarks.Equals("PME")).Count();
                    present = consolidateLog.Where(m => m.final_remarks != "AB" && m.final_remarks != "LV" && m.final_remarks != "OFF").Count();
                }

                var emp = db.employee.Where(c => c.active).FirstOrDefault();
                employeeName = emp.first_name + " " + emp.last_name;
            }

            ViewModels.Dashboard dashboard = new ViewModels.Dashboard();
            dashboard.headName = headName;
            dashboard.supervisorName = employeeName;
            dashboard.dashboardElement.onTime = onTime;
            dashboard.dashboardElement.absent = absent;
            dashboard.dashboardElement.leave = leave;
            dashboard.dashboardElement.late = late;
            dashboard.dashboardElement.earlyGone = earlyGone;
            dashboard.dashboardElement.present = present;
            dashboard.supervisorName = supervisor_name;
            dashboard.name = employeeName;

            return dashboard;
        }

        public static List<DashboardEvents> getDashboardEventsForHR()
        {
            string strDayName = "";
            bool isJAdded = false, isBAdded = false;

            List<DashboardEvents> listEvents = new List<DashboardEvents>();
            DateTime todaysDate = DateTime.Now.Date, nextDate = DateTime.Now.AddMonths(11);
            DateTime empJoiningDate = DateTime.Now.Date, empCurrentYearJoiningDate = DateTime.Now.Date;
            DateTime empBirthDate = DateTime.Now.Date, empCurrentYearBirthDate = DateTime.Now.Date;

            using (var db = new Context())
            {
                int counter = 0;

                var data_emp = db.employee.Where(e => e.active && (e.date_of_birth != null || e.date_of_joining != null)).ToList();
                if (data_emp != null && data_emp.Count > 0)
                {
                    for (int i = 0; i < data_emp.Count(); i++)
                    {
                        counter++;
                        if (counter == 100)
                        {
                            //break;
                        }

                        //joining date
                        if (data_emp[i].date_of_joining.HasValue && data_emp[i].date_of_joining.Value != null)
                        {
                            empJoiningDate = data_emp[i].date_of_joining.Value;//actual joining date

                            //if current year

                            //if next year started
                            try
                            {
                                empCurrentYearJoiningDate = new DateTime(DateTime.Now.Year, empJoiningDate.Month, empJoiningDate.Day);//joining date by current year
                            }
                            catch (Exception)
                            {
                                continue;
                            }
                            if (empCurrentYearJoiningDate >= todaysDate && empCurrentYearJoiningDate <= nextDate)
                            {
                                if (empJoiningDate.ToString("dd MMM") == DateTime.Now.ToString("dd MMM"))
                                {
                                    strDayName = "Today";
                                    //strDayName = "Today (" + empJoiningDate.ToString("ddd, dd MMM yyyy") + ")";
                                }
                                else if (empJoiningDate.ToString("dd MMM") == DateTime.Now.AddDays(1).ToString("dd MMM"))
                                {
                                    strDayName = "Tomorrow";
                                    //strDayName = "Tomorrow (" + empJoiningDate.ToString("ddd, dd MMM yyyy") + ")";
                                }
                                else
                                {
                                    strDayName = empCurrentYearJoiningDate.ToString("ddd, ") + empJoiningDate.ToString("dd-MM-") + empCurrentYearJoiningDate.ToString("yyyy");
                                }

                                listEvents.Add(new ViewModels.DashboardEvents()
                                {
                                    employee_code = data_emp[i].employee_code,
                                    employee_name = data_emp[i].first_name + " " + data_emp[i].last_name,
                                    event_actual_date = empCurrentYearJoiningDate,
                                    event_date = strDayName,
                                    event_title = "Work Anniversary"
                                });

                                isJAdded = true;
                            }
                        }

                        //if next year started
                        try
                        {
                            empCurrentYearJoiningDate = new DateTime(DateTime.Now.Year + 1, empJoiningDate.Month, empJoiningDate.Day);//joining date by current year
                        }
                        catch (Exception)
                        {
                            continue;
                        }

                        if (!isJAdded && empCurrentYearJoiningDate >= todaysDate && empCurrentYearJoiningDate <= nextDate)
                        {
                            if (empCurrentYearJoiningDate >= todaysDate && empCurrentYearJoiningDate <= nextDate)
                            {
                                if (empJoiningDate.ToString("dd MMM") == DateTime.Now.ToString("dd MMM"))
                                {
                                    strDayName = "Today";
                                    //strDayName = "Today (" + empJoiningDate.ToString("ddd, dd MMM yyyy") + ")";
                                }
                                else if (empJoiningDate.ToString("dd MMM") == DateTime.Now.AddDays(1).ToString("dd MMM"))
                                {
                                    strDayName = "Tomorrow";
                                    //strDayName = "Tomorrow (" + empJoiningDate.ToString("ddd, dd MMM yyyy") + ")";
                                }
                                else
                                {
                                    strDayName = empCurrentYearJoiningDate.ToString("ddd, ") + empJoiningDate.ToString("dd-MM-") + empCurrentYearJoiningDate.ToString("yyyy");
                                }

                                listEvents.Add(new ViewModels.DashboardEvents()
                                {
                                    employee_code = data_emp[i].employee_code,
                                    employee_name = data_emp[i].first_name + " " + data_emp[i].last_name,
                                    event_actual_date = empCurrentYearJoiningDate,
                                    event_date = strDayName,
                                    event_title = "Work Anniversary"
                                });
                            }
                        }


                        isJAdded = false;

                        strDayName = "";

                        //birth date
                        if (data_emp[i].date_of_birth.HasValue && data_emp[i].date_of_birth.Value != null)
                        {
                            empBirthDate = data_emp[i].date_of_birth.Value;//actual birthdate

                            //if current year
                            empCurrentYearBirthDate = new DateTime(DateTime.Now.Year, empBirthDate.Month, empBirthDate.Day);//birthdate by current year
                            if (empCurrentYearBirthDate >= todaysDate && empCurrentYearBirthDate <= nextDate)
                            {
                                if (empBirthDate.ToString("dd MMM") == DateTime.Now.ToString("dd MMM"))
                                {
                                    strDayName = "Today";
                                    //strDayName = "Today (" + empBirthDate.ToString("ddd, dd MMM yyyy") + ")";
                                }
                                else if (empBirthDate.ToString("dd MMM") == DateTime.Now.AddDays(1).ToString("dd MMM"))
                                {
                                    strDayName = "Tomorrow";
                                    //strDayName = "Tomorrow (" + empBirthDate.ToString("ddd, dd MMM yyyy") + ")";
                                }
                                else
                                {
                                    strDayName = empCurrentYearBirthDate.ToString("ddd, ") + empBirthDate.ToString("dd-MM-") + empCurrentYearBirthDate.ToString("yyyy");
                                }

                                listEvents.Add(new ViewModels.DashboardEvents()
                                {
                                    employee_code = data_emp[i].employee_code,
                                    employee_name = data_emp[i].first_name + " " + data_emp[i].last_name,
                                    event_actual_date = empCurrentYearBirthDate,
                                    event_date = strDayName,
                                    event_title = "Birthday"
                                });

                                isBAdded = true;
                            }

                            //if next year has started
                            empCurrentYearBirthDate = new DateTime(DateTime.Now.Year + 1, empBirthDate.Month, empBirthDate.Day);//birthdate by current year
                            if (!isBAdded && empCurrentYearBirthDate >= todaysDate && empCurrentYearBirthDate <= nextDate)
                            {
                                if (empBirthDate.ToString("dd MMM") == DateTime.Now.ToString("dd MMM"))
                                {
                                    strDayName = "Today";
                                    //strDayName = "Today (" + empBirthDate.ToString("ddd, dd MMM yyyy") + ")";
                                }
                                else if (empBirthDate.ToString("dd MMM") == DateTime.Now.AddDays(1).ToString("dd MMM"))
                                {
                                    strDayName = "Tomorrow";
                                    //strDayName = "Tomorrow (" + empBirthDate.ToString("ddd, dd MMM yyyy") + ")";
                                }
                                else
                                {
                                    strDayName = empCurrentYearBirthDate.ToString("ddd, ") + empBirthDate.ToString("dd-MM-") + empCurrentYearBirthDate.ToString("yyyy");
                                }

                                listEvents.Add(new ViewModels.DashboardEvents()
                                {
                                    employee_code = data_emp[i].employee_code,
                                    employee_name = data_emp[i].first_name + " " + data_emp[i].last_name,
                                    event_actual_date = empCurrentYearBirthDate,
                                    event_date = strDayName,
                                    event_title = "Birthday"
                                });
                            }
                            isBAdded = false;

                        }
                    }

                    listEvents = listEvents.OrderBy(o => o.event_title).OrderBy(o => o.event_actual_date).ToList();
                }
            }

            return listEvents;
        }

        public static List<DashboardEvents> getDashboardEventsForEMP(string emp_code)
        {
            int counter = 0;
            string strDayName = "";
            bool isJAdded = false, isBAdded = false;

            List<DashboardEvents> listEvents = new List<DashboardEvents>();
            DateTime todaysDate = DateTime.Now.Date, nextDate = DateTime.Now.AddMonths(11);
            DateTime empJoiningDate = DateTime.Now.Date, empCurrentYearJoiningDate = DateTime.Now.Date;
            DateTime empBirthDate = DateTime.Now.Date, empCurrentYearBirthDate = DateTime.Now.Date;

            using (var db = new Context())
            {
                var data_all_employee = db.employee.Where(e => e.active).ToList();
                if (data_all_employee != null && data_all_employee.Count > 0)
                {
                    var data_emp = data_all_employee.Where(e => e.employee_code == emp_code && e.Group != null).FirstOrDefault();
                    if (data_emp != null && data_emp.Group != null)
                    {
                        int group_id = data_emp.Group.GroupId;

                        var data_group_lm = db.group.Where(g => g.GroupId == group_id).ToList();
                        if (data_group_lm != null && data_group_lm.Count > 0)
                        {
                            foreach (var g in data_group_lm)
                            {
                                var data_emp_lms = data_all_employee.Where(e => e.EmployeeId == g.supervisor_id && (e.date_of_birth != null || e.date_of_joining != null)).ToList();

                                //add employee itself as well
                                if (counter == 0)
                                {
                                    var data_emp_main = data_all_employee.Where(e => e.employee_code == emp_code && (e.date_of_birth != null || e.date_of_joining != null)).FirstOrDefault();
                                    if (data_emp_main != null)
                                    {
                                        data_emp_lms.Add(new DLL.Models.Employee()
                                        {
                                            EmployeeId = data_emp_main.EmployeeId,
                                            employee_code = data_emp_main.employee_code,
                                            date_of_joining = data_emp_main.date_of_joining,
                                            date_of_birth = data_emp_main.date_of_birth,
                                            first_name = data_emp_main.first_name,
                                            last_name = data_emp_main.last_name
                                        });
                                    }
                                }
                                counter++;

                                if (data_emp_lms != null && data_emp_lms.Count > 0)
                                {
                                    #region LOOP
                                    for (int i = 0; i < data_emp_lms.Count(); i++)
                                    {
                                        //joining date
                                        if (data_emp_lms[i].date_of_joining.HasValue && data_emp_lms[i].date_of_joining.Value != null)
                                        {
                                            empJoiningDate = data_emp_lms[i].date_of_joining.Value;//actual joining date

                                            //if current year
                                            empCurrentYearJoiningDate = new DateTime(DateTime.Now.Year, empJoiningDate.Month, empJoiningDate.Day);//joining date by current year
                                            if (empCurrentYearJoiningDate >= todaysDate && empCurrentYearJoiningDate <= nextDate)
                                            {
                                                if (empJoiningDate.ToString("dd MMM") == DateTime.Now.ToString("dd MMM"))
                                                {
                                                    strDayName = "Today";
                                                    //strDayName = "Today (" + empJoiningDate.ToString("ddd, dd MMM yyyy") + ")";
                                                }
                                                else if (empJoiningDate.ToString("dd MMM") == DateTime.Now.AddDays(1).ToString("dd MMM"))
                                                {
                                                    strDayName = "Tomorrow";
                                                    //strDayName = "Tomorrow (" + empJoiningDate.ToString("ddd, dd MMM yyyy") + ")";
                                                }
                                                else
                                                {
                                                    strDayName = empCurrentYearJoiningDate.ToString("ddd, ") + empJoiningDate.ToString("dd-MM-") + empCurrentYearJoiningDate.ToString("yyyy");
                                                }

                                                listEvents.Add(new ViewModels.DashboardEvents()
                                                {
                                                    employee_code = data_emp_lms[i].employee_code,
                                                    employee_name = data_emp_lms[i].first_name + " " + data_emp_lms[i].last_name,
                                                    event_actual_date = empCurrentYearJoiningDate,
                                                    event_date = strDayName,
                                                    event_title = "Work Anniversary"
                                                });

                                                isJAdded = true;
                                            }

                                            //if next year
                                            empCurrentYearJoiningDate = new DateTime(DateTime.Now.Year + 1, empJoiningDate.Month, empJoiningDate.Day);//joining date by current year
                                            if (!isJAdded && empCurrentYearJoiningDate >= todaysDate && empCurrentYearJoiningDate <= nextDate)
                                            {
                                                if (empJoiningDate.ToString("dd MMM") == DateTime.Now.ToString("dd MMM"))
                                                {
                                                    strDayName = "Today";
                                                    //strDayName = "Today (" + empJoiningDate.ToString("ddd, dd MMM yyyy") + ")";
                                                }
                                                else if (empJoiningDate.ToString("dd MMM") == DateTime.Now.AddDays(1).ToString("dd MMM"))
                                                {
                                                    strDayName = "Tomorrow";
                                                    //strDayName = "Tomorrow (" + empJoiningDate.ToString("ddd, dd MMM yyyy") + ")";
                                                }
                                                else
                                                {
                                                    strDayName = empCurrentYearJoiningDate.ToString("ddd, ") + empJoiningDate.ToString("dd-MM-") + empCurrentYearJoiningDate.ToString("yyyy");
                                                }

                                                listEvents.Add(new ViewModels.DashboardEvents()
                                                {
                                                    employee_code = data_emp_lms[i].employee_code,
                                                    employee_name = data_emp_lms[i].first_name + " " + data_emp_lms[i].last_name,
                                                    event_actual_date = empCurrentYearJoiningDate,
                                                    event_date = strDayName,
                                                    event_title = "Work Anniversary"
                                                });
                                            }
                                            isJAdded = false;

                                        }

                                        strDayName = "";

                                        //birth date
                                        if (data_emp_lms[i].date_of_birth.HasValue && data_emp_lms[i].date_of_birth.Value != null)
                                        {
                                            empBirthDate = data_emp_lms[i].date_of_birth.Value;//actual birthdate

                                            //if current year
                                            empCurrentYearBirthDate = new DateTime(DateTime.Now.Year, empBirthDate.Month, empBirthDate.Day);//birthdate by current year
                                            if (empCurrentYearBirthDate >= todaysDate && empCurrentYearBirthDate <= nextDate)
                                            {
                                                if (empBirthDate.ToString("dd MMM") == DateTime.Now.ToString("dd MMM"))
                                                {
                                                    strDayName = "Today";
                                                    //strDayName = "Today (" + empBirthDate.ToString("ddd, dd MMM yyyy") + ")";
                                                }
                                                else if (empBirthDate.ToString("dd MMM") == DateTime.Now.AddDays(1).ToString("dd MMM"))
                                                {
                                                    strDayName = "Tomorrow";
                                                    //strDayName = "Tomorrow (" + empBirthDate.ToString("ddd, dd MMM yyyy") + ")";
                                                }
                                                else
                                                {
                                                    strDayName = empCurrentYearBirthDate.ToString("ddd, ") + empBirthDate.ToString("dd-MM-") + empCurrentYearBirthDate.ToString("yyyy");
                                                }

                                                listEvents.Add(new ViewModels.DashboardEvents()
                                                {
                                                    employee_code = data_emp_lms[i].employee_code,
                                                    employee_name = data_emp_lms[i].first_name + " " + data_emp_lms[i].last_name,
                                                    event_actual_date = empCurrentYearBirthDate,
                                                    event_date = strDayName,
                                                    event_title = "Birthday"
                                                });

                                                isBAdded = true;
                                            }

                                            //if next year
                                            empCurrentYearBirthDate = new DateTime(DateTime.Now.Year + 1, empBirthDate.Month, empBirthDate.Day);//birthdate by current year
                                            if (!isBAdded && empCurrentYearBirthDate >= todaysDate && empCurrentYearBirthDate <= nextDate)
                                            {
                                                if (empBirthDate.ToString("dd MMM") == DateTime.Now.ToString("dd MMM"))
                                                {
                                                    strDayName = "Today";
                                                    //strDayName = "Today (" + empBirthDate.ToString("ddd, dd MMM yyyy") + ")";
                                                }
                                                else if (empBirthDate.ToString("dd MMM") == DateTime.Now.AddDays(1).ToString("dd MMM"))
                                                {
                                                    strDayName = "Tomorrow";
                                                    //strDayName = "Tomorrow (" + empBirthDate.ToString("ddd, dd MMM yyyy") + ")";
                                                }
                                                else
                                                {
                                                    strDayName = empCurrentYearBirthDate.ToString("ddd, ") + empBirthDate.ToString("dd-MM-") + empCurrentYearBirthDate.ToString("yyyy");
                                                }

                                                listEvents.Add(new ViewModels.DashboardEvents()
                                                {
                                                    employee_code = data_emp_lms[i].employee_code,
                                                    employee_name = data_emp_lms[i].first_name + " " + data_emp_lms[i].last_name,
                                                    event_actual_date = empCurrentYearBirthDate,
                                                    event_date = strDayName,
                                                    event_title = "Birthday"
                                                });
                                            }
                                            isBAdded = false;

                                        }
                                    }
                                    #endregion
                                }
                            }
                        }
                    }
                }

                listEvents = listEvents.OrderBy(o => o.event_title).OrderBy(o => o.event_actual_date).ToList();
            }

            return listEvents;
        }

        public static List<DashboardEvents> getDashboardEventsForLM(string emp_code)
        {
            string strDayName = "";
            bool isJAdded = false, isBAdded = false;

            List<DashboardEvents> listEvents = new List<DashboardEvents>();
            DateTime todaysDate = DateTime.Now.Date, nextDate = DateTime.Now.AddMonths(11);
            DateTime empJoiningDate = DateTime.Now.Date, empCurrentYearJoiningDate = DateTime.Now.Date;
            DateTime empBirthDate = DateTime.Now.Date, empCurrentYearBirthDate = DateTime.Now.Date;

            using (var db = new Context())
            {
                var data_all_employee = db.employee.Where(e => e.active).ToList();
                if (data_all_employee != null && data_all_employee.Count > 0)
                {
                    var data_emp = data_all_employee.Where(e => e.employee_code == emp_code && e.Group != null).FirstOrDefault();
                    if (data_emp != null && data_emp.Group != null)
                    {
                        int group_id = data_emp.Group.GroupId;

                        var data_group_lm = db.group.Where(g => g.GroupId == group_id).ToList();
                        if (data_group_lm != null && data_group_lm.Count > 0)
                        {
                            foreach (var g in data_group_lm)
                            {
                                var data_emp_lms = data_all_employee.Where(e => e.Group != null && e.Group.GroupId == g.GroupId && (e.date_of_birth != null || e.date_of_joining != null)).ToList();
                                if (data_emp_lms != null && data_emp_lms.Count > 0)
                                {
                                    #region LOOP
                                    for (int i = 0; i < data_emp_lms.Count(); i++)
                                    {
                                        //joining date
                                        if (data_emp_lms[i].date_of_joining.HasValue && data_emp_lms[i].date_of_joining.Value != null)
                                        {
                                            empJoiningDate = data_emp_lms[i].date_of_joining.Value;//actual joining date

                                            //if current year
                                            empCurrentYearJoiningDate = new DateTime(DateTime.Now.Year, empJoiningDate.Month, empJoiningDate.Day);//joining date by current year
                                            if (empCurrentYearJoiningDate >= todaysDate && empCurrentYearJoiningDate <= nextDate)
                                            {
                                                if (empJoiningDate.ToString("dd MMM") == DateTime.Now.ToString("dd MMM"))
                                                {
                                                    strDayName = "Today";
                                                    //strDayName = "Today (" + empJoiningDate.ToString("ddd, dd MMM yyyy") + ")";
                                                }
                                                else if (empJoiningDate.ToString("dd MMM") == DateTime.Now.AddDays(1).ToString("dd MMM"))
                                                {
                                                    strDayName = "Tomorrow";
                                                    //strDayName = "Tomorrow (" + empJoiningDate.ToString("ddd, dd MMM yyyy") + ")";
                                                }
                                                else
                                                {
                                                    strDayName = empCurrentYearJoiningDate.ToString("ddd, ") + empJoiningDate.ToString("dd-MM-") + empCurrentYearJoiningDate.ToString("yyyy");
                                                }

                                                listEvents.Add(new ViewModels.DashboardEvents()
                                                {
                                                    employee_code = data_emp_lms[i].employee_code,
                                                    employee_name = data_emp_lms[i].first_name + " " + data_emp_lms[i].last_name,
                                                    event_actual_date = empCurrentYearJoiningDate,
                                                    event_date = strDayName,
                                                    event_title = "Work Anniversary"
                                                });

                                                isJAdded = true;
                                            }

                                            //if next year
                                            empCurrentYearJoiningDate = new DateTime(DateTime.Now.Year + 1, empJoiningDate.Month, empJoiningDate.Day);//joining date by current year
                                            if (!isJAdded && empCurrentYearJoiningDate >= todaysDate && empCurrentYearJoiningDate <= nextDate)
                                            {
                                                if (empJoiningDate.ToString("dd MMM") == DateTime.Now.ToString("dd MMM"))
                                                {
                                                    strDayName = "Today";
                                                    //strDayName = "Today (" + empJoiningDate.ToString("ddd, dd MMM yyyy") + ")";
                                                }
                                                else if (empJoiningDate.ToString("dd MMM") == DateTime.Now.AddDays(1).ToString("dd MMM"))
                                                {
                                                    strDayName = "Tomorrow";
                                                    //strDayName = "Tomorrow (" + empJoiningDate.ToString("ddd, dd MMM yyyy") + ")";
                                                }
                                                else
                                                {
                                                    strDayName = empCurrentYearJoiningDate.ToString("ddd, ") + empJoiningDate.ToString("dd-MM-") + empCurrentYearJoiningDate.ToString("yyyy");
                                                }

                                                listEvents.Add(new ViewModels.DashboardEvents()
                                                {
                                                    employee_code = data_emp_lms[i].employee_code,
                                                    employee_name = data_emp_lms[i].first_name + " " + data_emp_lms[i].last_name,
                                                    event_actual_date = empCurrentYearJoiningDate,
                                                    event_date = strDayName,
                                                    event_title = "Work Anniversary"
                                                });
                                            }
                                            isJAdded = false;

                                        }

                                        strDayName = "";

                                        //birth date
                                        if (data_emp_lms[i].date_of_birth.HasValue && data_emp_lms[i].date_of_birth.Value != null)
                                        {
                                            empBirthDate = data_emp_lms[i].date_of_birth.Value;//actual birthdate

                                            //if current year
                                            empCurrentYearBirthDate = new DateTime(DateTime.Now.Year, empBirthDate.Month, empBirthDate.Day);//birthdate by current year
                                            if (empCurrentYearBirthDate >= todaysDate && empCurrentYearBirthDate <= nextDate)
                                            {
                                                if (empBirthDate.ToString("dd MMM") == DateTime.Now.ToString("dd MMM"))
                                                {
                                                    strDayName = "Today";
                                                    //strDayName = "Today (" + empBirthDate.ToString("ddd, dd MMM yyyy") + ")";
                                                }
                                                else if (empBirthDate.ToString("dd MMM") == DateTime.Now.AddDays(1).ToString("dd MMM"))
                                                {
                                                    strDayName = "Tomorrow";
                                                    //strDayName = "Tomorrow (" + empBirthDate.ToString("ddd, dd MMM yyyy") + ")";
                                                }
                                                else
                                                {
                                                    strDayName = empCurrentYearBirthDate.ToString("ddd, ") + empBirthDate.ToString("dd-MM-") + empCurrentYearBirthDate.ToString("yyyy");
                                                }

                                                listEvents.Add(new ViewModels.DashboardEvents()
                                                {
                                                    employee_code = data_emp_lms[i].employee_code,
                                                    employee_name = data_emp_lms[i].first_name + " " + data_emp_lms[i].last_name,
                                                    event_actual_date = empCurrentYearBirthDate,
                                                    event_date = strDayName,
                                                    event_title = "Birthday"
                                                });

                                                isBAdded = true;
                                            }

                                            //if next year
                                            empCurrentYearBirthDate = new DateTime(DateTime.Now.Year + 1, empBirthDate.Month, empBirthDate.Day);//birthdate by current year
                                            if (!isBAdded && empCurrentYearBirthDate >= todaysDate && empCurrentYearBirthDate <= nextDate)
                                            {
                                                if (empBirthDate.ToString("dd MMM") == DateTime.Now.ToString("dd MMM"))
                                                {
                                                    strDayName = "Today";
                                                    //strDayName = "Today (" + empBirthDate.ToString("ddd, dd MMM yyyy") + ")";
                                                }
                                                else if (empBirthDate.ToString("dd MMM") == DateTime.Now.AddDays(1).ToString("dd MMM"))
                                                {
                                                    strDayName = "Tomorrow";
                                                    //strDayName = "Tomorrow (" + empBirthDate.ToString("ddd, dd MMM yyyy") + ")";
                                                }
                                                else
                                                {
                                                    strDayName = empCurrentYearBirthDate.ToString("ddd, ") + empBirthDate.ToString("dd-MM-") + empCurrentYearBirthDate.ToString("yyyy");
                                                }

                                                listEvents.Add(new ViewModels.DashboardEvents()
                                                {
                                                    employee_code = data_emp_lms[i].employee_code,
                                                    employee_name = data_emp_lms[i].first_name + " " + data_emp_lms[i].last_name,
                                                    event_actual_date = empCurrentYearBirthDate,
                                                    event_date = strDayName,
                                                    event_title = "Birthday"
                                                });
                                            }
                                            isBAdded = false;

                                        }
                                    }
                                    #endregion
                                }
                            }
                        }
                    }
                }

                isJAdded = false; isBAdded = false;

                ///////////// SLMs 

                if (data_all_employee != null && data_all_employee.Count > 0)
                {
                    var data_emp = data_all_employee.Where(e => e.employee_code == emp_code).FirstOrDefault();
                    if (data_emp != null)
                    {
                        int emp_id = data_emp.EmployeeId;

                        var data_group_slm = db.super_line_manager_tagging.Where(s => s.superLineManager.EmployeeId == emp_id).ToList();

                        //var lm_main = db.super_line_manager_tagging.Where(s => s.superLineManager.EmployeeId == emp_id).FirstOrDefault();
                        //if (lm_main != null)
                        //    data_group_slm.Add(lm_main);

                        if (data_group_slm != null && data_group_slm.Count > 0)
                        {
                            foreach (var g in data_group_slm)
                            {
                                var data_emp_slms = data_all_employee.Where(e => e.EmployeeId == g.taggedEmployee.EmployeeId && (e.date_of_birth != null || e.date_of_joining != null)).ToList();
                                if (data_emp_slms != null && data_emp_slms.Count > 0)
                                {
                                    #region LOOP
                                    for (int i = 0; i < data_emp_slms.Count(); i++)
                                    {
                                        //joining date
                                        if (data_emp_slms[i].date_of_joining.HasValue && data_emp_slms[i].date_of_joining.Value != null)
                                        {
                                            empJoiningDate = data_emp_slms[i].date_of_joining.Value;//actual joining date

                                            //if current year
                                            empCurrentYearJoiningDate = new DateTime(DateTime.Now.Year, empJoiningDate.Month, empJoiningDate.Day);//joining date by current year
                                            if (empCurrentYearJoiningDate >= todaysDate && empCurrentYearJoiningDate <= nextDate)
                                            {
                                                if (empJoiningDate.ToString("dd MMM") == DateTime.Now.ToString("dd MMM"))
                                                {
                                                    strDayName = "Today";
                                                    //strDayName = "Today (" + empJoiningDate.ToString("ddd, dd MMM yyyy") + ")";
                                                }
                                                else if (empJoiningDate.ToString("dd MMM") == DateTime.Now.AddDays(1).ToString("dd MMM"))
                                                {
                                                    strDayName = "Tomorrow";
                                                    //strDayName = "Tomorrow (" + empJoiningDate.ToString("ddd, dd MMM yyyy") + ")";
                                                }
                                                else
                                                {
                                                    strDayName = empCurrentYearJoiningDate.ToString("ddd, ") + empJoiningDate.ToString("dd-MM-") + empCurrentYearJoiningDate.ToString("yyyy");
                                                }

                                                listEvents.Add(new ViewModels.DashboardEvents()
                                                {
                                                    employee_code = data_emp_slms[i].employee_code,
                                                    employee_name = data_emp_slms[i].first_name + " " + data_emp_slms[i].last_name,
                                                    event_actual_date = empCurrentYearJoiningDate,
                                                    event_date = strDayName,
                                                    event_title = "Work Anniversary"
                                                });

                                                isJAdded = true;
                                            }

                                            //if next year
                                            empCurrentYearJoiningDate = new DateTime(DateTime.Now.Year + 1, empJoiningDate.Month, empJoiningDate.Day);//joining date by current year
                                            if (!isJAdded && empCurrentYearJoiningDate >= todaysDate && empCurrentYearJoiningDate <= nextDate)
                                            {
                                                if (empJoiningDate.ToString("dd MMM") == DateTime.Now.ToString("dd MMM"))
                                                {
                                                    strDayName = "Today";
                                                    //strDayName = "Today (" + empJoiningDate.ToString("ddd, dd MMM yyyy") + ")";
                                                }
                                                else if (empJoiningDate.ToString("dd MMM") == DateTime.Now.AddDays(1).ToString("dd MMM"))
                                                {
                                                    strDayName = "Tomorrow";
                                                    //strDayName = "Tomorrow (" + empJoiningDate.ToString("ddd, dd MMM yyyy") + ")";
                                                }
                                                else
                                                {
                                                    strDayName = empCurrentYearJoiningDate.ToString("ddd, ") + empJoiningDate.ToString("dd-MM-") + empCurrentYearJoiningDate.ToString("yyyy");
                                                }

                                                listEvents.Add(new ViewModels.DashboardEvents()
                                                {
                                                    employee_code = data_emp_slms[i].employee_code,
                                                    employee_name = data_emp_slms[i].first_name + " " + data_emp_slms[i].last_name,
                                                    event_actual_date = empCurrentYearJoiningDate,
                                                    event_date = strDayName,
                                                    event_title = "Work Anniversary"
                                                });
                                            }
                                            isJAdded = false;

                                        }

                                        strDayName = "";

                                        //birth date
                                        if (data_emp_slms[i].date_of_birth.HasValue && data_emp_slms[i].date_of_birth.Value != null)
                                        {
                                            empBirthDate = data_emp_slms[i].date_of_birth.Value;//actual birthdate

                                            //if current year
                                            empCurrentYearBirthDate = new DateTime(DateTime.Now.Year, empBirthDate.Month, empBirthDate.Day);//birthdate by current year
                                            if (empCurrentYearBirthDate >= todaysDate && empCurrentYearBirthDate <= nextDate)
                                            {
                                                if (empBirthDate.ToString("dd MMM") == DateTime.Now.ToString("dd MMM"))
                                                {
                                                    strDayName = "Today";
                                                    //strDayName = "Today (" + empBirthDate.ToString("ddd, dd MMM yyyy") + ")";
                                                }
                                                else if (empBirthDate.ToString("dd MMM") == DateTime.Now.AddDays(1).ToString("dd MMM"))
                                                {
                                                    strDayName = "Tomorrow";
                                                    //strDayName = "Tomorrow (" + empBirthDate.ToString("ddd, dd MMM yyyy") + ")";
                                                }
                                                else
                                                {
                                                    strDayName = empCurrentYearBirthDate.ToString("ddd, ") + empBirthDate.ToString("dd-MM-") + empCurrentYearBirthDate.ToString("yyyy");
                                                }

                                                listEvents.Add(new ViewModels.DashboardEvents()
                                                {
                                                    employee_code = data_emp_slms[i].employee_code,
                                                    employee_name = data_emp_slms[i].first_name + " " + data_emp_slms[i].last_name,
                                                    event_actual_date = empCurrentYearBirthDate,
                                                    event_date = strDayName,
                                                    event_title = "Birthday"
                                                });

                                                isBAdded = true;
                                            }

                                            //if next year
                                            empCurrentYearBirthDate = new DateTime(DateTime.Now.Year + 1, empBirthDate.Month, empBirthDate.Day);//birthdate by current year
                                            if (!isBAdded && empCurrentYearBirthDate >= todaysDate && empCurrentYearBirthDate <= nextDate)
                                            {
                                                if (empBirthDate.ToString("dd MMM") == DateTime.Now.ToString("dd MMM"))
                                                {
                                                    strDayName = "Today";
                                                    //strDayName = "Today (" + empBirthDate.ToString("ddd, dd MMM yyyy") + ")";
                                                }
                                                else if (empBirthDate.ToString("dd MMM") == DateTime.Now.AddDays(1).ToString("dd MMM"))
                                                {
                                                    strDayName = "Tomorrow";
                                                    //strDayName = "Tomorrow (" + empBirthDate.ToString("ddd, dd MMM yyyy") + ")";
                                                }
                                                else
                                                {
                                                    strDayName = empCurrentYearBirthDate.ToString("ddd, ") + empBirthDate.ToString("dd-MM-") + empCurrentYearBirthDate.ToString("yyyy");
                                                }

                                                listEvents.Add(new ViewModels.DashboardEvents()
                                                {
                                                    employee_code = data_emp_slms[i].employee_code,
                                                    employee_name = data_emp_slms[i].first_name + " " + data_emp_slms[i].last_name,
                                                    event_actual_date = empCurrentYearBirthDate,
                                                    event_date = strDayName,
                                                    event_title = "Birthday"
                                                });
                                            }
                                            isBAdded = false;

                                        }
                                    }
                                    #endregion
                                }
                            }
                        }
                    }
                }

                listEvents = listEvents.OrderBy(o => o.event_title).OrderBy(o => o.event_actual_date).ToList();
            }

            return listEvents;
        }

        public static List<DashboardHolidays> getDashboardHolidaysForHR()
        {
            List<DashboardHolidays> listHolidays = new List<DashboardHolidays>();
            DateTime todaysDate = DateTime.Now.Date, nextDate = DateTime.Now.AddMonths(11);

            using (var db = new Context())
            {
                var data_holidays = db.general_calender_override.Where(e => e.active && e.date != null).ToList();
                if (data_holidays != null && data_holidays.Count > 0)
                {
                    for (int i = 0; i < data_holidays.Count(); i++)
                    {
                        if (data_holidays[i].date.HasValue && data_holidays[i].date.Value != null)
                        {
                            if (data_holidays[i].date.Value >= todaysDate && data_holidays[i].date.Value <= nextDate)
                            {
                                listHolidays.Add(new ViewModels.DashboardHolidays()
                                {
                                    holiday_actual_date = data_holidays[i].date.Value,
                                    holiday_date = data_holidays[i].date.Value.ToString("ddd, dd-MM-yyyy"),
                                    holiday_title = data_holidays[i].reason
                                });
                            }
                        }
                    }

                    listHolidays = listHolidays.OrderBy(o => o.holiday_actual_date).ToList();
                }
            }

            return listHolidays;
        }

        public static List<DashboardLeavesCounter> getDashboardLeavesForHR()
        {
            List<DashboardLeavesCounter> listLeaves = new List<DashboardLeavesCounter>();
            DateTime todaysDate = DateTime.Now.Date;
            DateTime l3Date = DateTime.Now.AddMonths(-3), l2Date = DateTime.Now.AddMonths(-2), l1Date = DateTime.Now.AddMonths(-1);

            //last 3 Month back
            DateTime lFrom3Date = new DateTime(l3Date.Year, l3Date.Month, 1);
            DateTime lTo3Date = new DateTime(l3Date.Year, l3Date.Month, DateTime.DaysInMonth(l3Date.Year, l3Date.Month));

            //last 2 Month back
            DateTime lFrom2Date = new DateTime(l2Date.Year, l2Date.Month, 1);
            DateTime lTo2Date = new DateTime(l2Date.Year, l2Date.Month, DateTime.DaysInMonth(l2Date.Year, l2Date.Month));

            //last 1 Month back
            DateTime lFrom1Date = new DateTime(l1Date.Year, l1Date.Month, 1);
            DateTime lTo1Date = new DateTime(l1Date.Year, l1Date.Month, DateTime.DaysInMonth(l1Date.Year, l1Date.Month));

            //this Month
            DateTime lFromTDate = new DateTime(todaysDate.Year, todaysDate.Month, 1);
            DateTime lToTDate = new DateTime(todaysDate.Year, todaysDate.Month, DateTime.DaysInMonth(todaysDate.Year, todaysDate.Month));

            using (var db = new Context())
            {
                string query = string.Format("SP_GetDashboardLeavesSummaryForHR @FromDateL3Month='{0}', @ToDateL3Month='{1}', @FromDateL2Month='{2}', @ToDateL2Month='{3}', @FromDateL1Month='{4}', @ToDateL1Month='{5}', @FromDateTMonth='{6}', @ToDateTMonth='{7}'", lFrom3Date.ToString("yyyy-MM-dd"), lTo3Date.ToString("yyyy-MM-dd"), lFrom2Date.ToString("yyyy-MM-dd"), lTo2Date.ToString("yyyy-MM-dd"), lFrom1Date.ToString("yyyy-MM-dd"), lTo1Date.ToString("yyyy-MM-dd"), lFromTDate.ToString("yyyy-MM-dd"), lToTDate.ToString("yyyy-MM-dd"));
                listLeaves = db.Database.SqlQuery<DashboardLeavesCounter>(query).ToList();
            }

            return listLeaves;
        }

        public static List<DashboardLeavesCounter> getDashboardLeavesForEMP(string emp_code)
        {
            List<DashboardLeavesCounter> listLeaves = new List<DashboardLeavesCounter>();
            DateTime todaysDate = DateTime.Now.Date;
            DateTime l3Date = DateTime.Now.AddMonths(-3), l2Date = DateTime.Now.AddMonths(-2), l1Date = DateTime.Now.AddMonths(-1);

            //last 3 Month back
            DateTime lFrom3Date = new DateTime(l3Date.Year, l3Date.Month, 1);
            DateTime lTo3Date = new DateTime(l3Date.Year, l3Date.Month, DateTime.DaysInMonth(l3Date.Year, l3Date.Month));

            //last 2 Month back
            DateTime lFrom2Date = new DateTime(l2Date.Year, l2Date.Month, 1);
            DateTime lTo2Date = new DateTime(l2Date.Year, l2Date.Month, DateTime.DaysInMonth(l2Date.Year, l2Date.Month));

            //last 1 Month back
            DateTime lFrom1Date = new DateTime(l1Date.Year, l1Date.Month, 1);
            DateTime lTo1Date = new DateTime(l1Date.Year, l1Date.Month, DateTime.DaysInMonth(l1Date.Year, l1Date.Month));

            //this Month
            DateTime lFromTDate = new DateTime(todaysDate.Year, todaysDate.Month, 1);
            DateTime lToTDate = new DateTime(todaysDate.Year, todaysDate.Month, DateTime.DaysInMonth(todaysDate.Year, todaysDate.Month));

            using (var db = new Context())
            {
                string query = string.Format("SP_GetDashboardLeavesSummaryForEMP @EmployeeCode='{0}', @FromDateL3Month='{1}', @ToDateL3Month='{2}', @FromDateL2Month='{3}', @ToDateL2Month='{4}', @FromDateL1Month='{5}', @ToDateL1Month='{6}', @FromDateTMonth='{7}', @ToDateTMonth='{8}'", emp_code, lFrom3Date.ToString("yyyy-MM-dd"), lTo3Date.ToString("yyyy-MM-dd"), lFrom2Date.ToString("yyyy-MM-dd"), lTo2Date.ToString("yyyy-MM-dd"), lFrom1Date.ToString("yyyy-MM-dd"), lTo1Date.ToString("yyyy-MM-dd"), lFromTDate.ToString("yyyy-MM-dd"), lToTDate.ToString("yyyy-MM-dd"));
                listLeaves = db.Database.SqlQuery<DashboardLeavesCounter>(query).ToList();
            }

            return listLeaves;
        }

        public static List<DashboardLeavesCounter> getDashboardLeavesForLM(string emp_code)
        {
            List<DashboardLeavesCounter> listLeaves = new List<DashboardLeavesCounter>();
            DateTime todaysDate = DateTime.Now.Date;
            DateTime l3Date = DateTime.Now.AddMonths(-3), l2Date = DateTime.Now.AddMonths(-2), l1Date = DateTime.Now.AddMonths(-1);

            //last 3 Month back
            DateTime lFrom3Date = new DateTime(l3Date.Year, l3Date.Month, 1);
            DateTime lTo3Date = new DateTime(l3Date.Year, l3Date.Month, DateTime.DaysInMonth(l3Date.Year, l3Date.Month));

            //last 2 Month back
            DateTime lFrom2Date = new DateTime(l2Date.Year, l2Date.Month, 1);
            DateTime lTo2Date = new DateTime(l2Date.Year, l2Date.Month, DateTime.DaysInMonth(l2Date.Year, l2Date.Month));

            //last 1 Month back
            DateTime lFrom1Date = new DateTime(l1Date.Year, l1Date.Month, 1);
            DateTime lTo1Date = new DateTime(l1Date.Year, l1Date.Month, DateTime.DaysInMonth(l1Date.Year, l1Date.Month));

            //this Month
            DateTime lFromTDate = new DateTime(todaysDate.Year, todaysDate.Month, 1);
            DateTime lToTDate = new DateTime(todaysDate.Year, todaysDate.Month, DateTime.DaysInMonth(todaysDate.Year, todaysDate.Month));

            using (var db = new Context())
            {
                string query = string.Format("SP_GetDashboardLeavesSummaryForLM @EmployeeCode='{0}', @FromDateL3Month='{1}', @ToDateL3Month='{2}', @FromDateL2Month='{3}', @ToDateL2Month='{4}', @FromDateL1Month='{5}', @ToDateL1Month='{6}', @FromDateTMonth='{7}', @ToDateTMonth='{8}'", emp_code, lFrom3Date.ToString("yyyy-MM-dd"), lTo3Date.ToString("yyyy-MM-dd"), lFrom2Date.ToString("yyyy-MM-dd"), lTo2Date.ToString("yyyy-MM-dd"), lFrom1Date.ToString("yyyy-MM-dd"), lTo1Date.ToString("yyyy-MM-dd"), lFromTDate.ToString("yyyy-MM-dd"), lToTDate.ToString("yyyy-MM-dd"));
                listLeaves = db.Database.SqlQuery<DashboardLeavesCounter>(query).ToList();
            }

            return listLeaves;
        }

        public static ViewModels.Dashboard getDashboardValues(string empCode, int department_id)
        {
            int onTime = 0, present = 0, late = 0, earlyGone = 0, absent = 0, leave = 0;
            using (var db = new Context())
            {
                int thisMonth = DateTime.Now.Month;
                int thisYear = DateTime.Now.Year;

                // Get all the consolidated logs for this employee, from this month and year.
                var consolidateLog = db.consolidated_attendance.Where(m => m.active
                    && m.employee.active && m.employee.timetune_active && m.employee.employee_code != "000000" &&
                    m.date.Value.Month.Equals(thisMonth) &&
                    m.date.Value.Year.Equals(thisYear) && m.employee.department.DepartmentId == department_id).ToList();

                onTime = consolidateLog.Where(m => m.final_remarks.Equals("PO") || m.final_remarks.Equals("POE") || m.final_remarks.Equals("POM")).Count();
                late = consolidateLog.Where(m => m.final_remarks.Equals("PLO") || m.final_remarks.Equals("PLE") || m.final_remarks.Equals("PLM")).Count();
                absent = consolidateLog.Where(m => m.final_remarks.Equals("AB")).Count();
                leave = consolidateLog.Where(m => m.final_remarks.Equals("LV")).Count();
                earlyGone = consolidateLog.Where(m => m.final_remarks.Equals("PLE") || m.final_remarks.Equals("POE") || m.final_remarks.Equals("PME")).Count();
                present = consolidateLog.Where(m => m.final_remarks != "AB" && m.final_remarks != "LV" && m.final_remarks != "OFF").Count();

            }
            ViewModels.Dashboard dashboard = new ViewModels.Dashboard();

            dashboard.dashboardElement.onTime = onTime;
            dashboard.dashboardElement.late = late;
            dashboard.dashboardElement.absent = absent;
            dashboard.dashboardElement.earlyGone = earlyGone;
            dashboard.dashboardElement.present = present;
            dashboard.dashboardElement.leave = leave;

            return dashboard;

        }

        public static List<string> getSupervisor(string empCode)
        {
            List<string> toReturn = new List<string>();
            using (var db = new Context())
            {
                var emp = db.employee.Where(c => c.employee_code.Equals(empCode)).FirstOrDefault();
                string code = emp.employee_code;
                if (emp.Group != null)
                {

                    int supervisor_id = emp.Group.supervisor_id;
                    var sup = db.employee.Where(c => c.EmployeeId == supervisor_id).FirstOrDefault();

                    if (sup != null)
                        toReturn.Add(sup.first_name + " " + sup.last_name);
                }

                var tag = db.super_line_manager_tagging.Where(c => c.taggedEmployee.employee_code == code).FirstOrDefault();
                if (tag != null)
                {
                    toReturn.Add(tag.superLineManager.first_name + " " + tag.superLineManager.last_name);
                }

                return toReturn;
            }
        }

        public class LMAttendanceCount
        {
            public List<string> getSupervisorData { get; set; }
            public int todayPresent { get; set; }
            public int todayLeave { get; set; }
            public int todayAbsent { get; set; }
            public int todayOnTime { get; set; }
            public LMAttendanceCount data { get; set; }

        }


        public static LMAttendanceCount getTodayMyTeamValues(string empCode)
        {
            int present = 0, absent = 0, leave = 0;

            DateTime todayDate = DateTime.Now;
            LMAttendanceCount dashboard = new LMAttendanceCount();

            using (var db = new Context())
            {
                int thisMonth = DateTime.Now.Month;
                int thisYear = DateTime.Now.Year;

                var getAllemp = db.employee.Where(e => e.active).ToList();
                if (getAllemp != null && getAllemp.Count > 0)
                {
                    var emp = getAllemp.Where(p => p.employee_code == empCode).FirstOrDefault();
                    if (emp != null && emp.Group != null)
                    {
                        int sup_id = emp.Group.GroupId;

                        var tagEmp = getAllemp.Where(c => c.Group != null && c.Group.GroupId == sup_id).ToList();
                        if (tagEmp != null && tagEmp.Count > 0)
                        {
                            for (int i = 0; i < tagEmp.Count; i++)
                            {
                                int strEmpId = tagEmp[i].EmployeeId;
                                string strCode = tagEmp[i].employee_code;

                                // Get all the consolidated logs for this employee, from this month and year.

                                var today_Present = db.persistent_attendance_log.Where(m => m.active &&
                                    m.time_in.Value.Year == todayDate.Year && m.time_in.Value.Month == todayDate.Month && m.time_in.Value.Day == todayDate.Day &&
                                    m.employee_code.Equals(strCode)).ToList();
                                if (today_Present != null && today_Present.Count > 0)
                                {
                                    present += today_Present.Count();
                                }

                                var today_Absent = db.persistent_attendance_log.Where(m => m.active &&
                                   m.time_in == null &&
                                   m.employee_code.Equals(strCode)).ToList();
                                if (today_Absent != null && today_Absent.Count > 0)
                                {
                                    absent += today_Absent.Count();
                                }

                                var today_Leave = db.leave_application.Where(c => c.IsActive && c.EmployeeId == strEmpId && (c.FromDate <= DateTime.Now && c.ToDate >= DateTime.Now) && c.LeaveStatusId == 2).ToList();
                                if (today_Leave != null && today_Leave.Count > 0)
                                {
                                    leave += today_Leave.Count();
                                }
                            }
                        }
                    }
                }

                dashboard.todayAbsent = absent;
                dashboard.todayPresent = present;
                dashboard.todayLeave = leave;
            }

            return dashboard;
        }

        public static int getTotalActiveEmp()
        {
            int result = 0;
            using (var db = new Context())
            {
                var emp = db.employee.Where(c => c.active && c.timetune_active).ToList();
                if (emp != null && emp.Count > 0)
                {
                    result = emp.Count();
                }
            }
            return result;
        }

        public static int getTotalRegisteredEmp()
        {
            int result = 0;
            using (var db = new Context())
            {
                var emp = db.employee.Where(e => e.active).ToList();
                if (emp != null && emp.Count > 0)
                {
                    result = emp.Count();
                }
            }
            return result;
        }


        public class ReturnData
        {
            public List<string> dashboard { get; set; }
            public int totalPresent { get; set; }
            public int totalActive { get; set; }
            public int totalRegistered { get; set; }
            public string totalRegCards { get; set; }
            public string totalRegFingers { get; set; }

            public int today_Leaves { get; set; }
            public int today_OnTime { get; set; }
            public int today_Late { get; set; }
            public int today_Absents { get; set; }
            public int today_EarlyOut { get; set; }
            public int today_OnlineDevices { get; set; }



        }
        public static int getTotalPresentToday()
        {
            int result = 0;
            DateTime todayDate = DateTime.Now.Date;
            //DateTime getDate = Convert.ToDateTime(todayDate).Date;
            using (var db = new Context())
            {
                var pers = db.persistent_attendance_log.Where(p => p.active && p.time_in != null && p.time_in >= todayDate).ToList();
                if (pers != null && pers.Count > 0)
                {
                    result = pers.Count();
                }
            }

            return result;
        }

        public static int getTodayLeaves()
        {
            int result = 0;
            DateTime todayDate = DateTime.Now.Date;
            //DateTime getDate = Convert.ToDateTime(todayDate).Date;
            using (var db = new Context())
            {
                var pers = db.leave_application.Where(p => p.IsActive && p.LeaveStatusId == 2 && (p.FromDate <= todayDate && p.ToDate >= todayDate)).ToList();
                if (pers != null && pers.Count > 0)
                {
                    result = pers.Count();
                }
            }

            return result;
        }

        public static int getTodayAbsents()
        {
            int result = 0;
            DateTime todayDate = DateTime.Now;
            //DateTime getDate = Convert.ToDateTime(todayDate).Date;
            using (var db = new Context())
            {
                var pers = db.persistent_attendance_log.Where(p => p.active && p.time_in == null).ToList();
                if (pers != null && pers.Count > 0)
                {
                    result = pers.Count();
                }
            }

            return result;
        }


        public static int getTodayOnTime()
        {
            int result = 0;
            DateTime todayDate = DateTime.Now;
            //DateTime getDate = Convert.ToDateTime(todayDate).Date;
            using (var db = new Context())
            {
                var pers = db.persistent_attendance_log.Where(p => p.active && p.time_in != null && p.time_in < p.late_time).ToList();
                if (pers != null && pers.Count > 0)
                {
                    result = pers.Count();
                }
            }
            return result;
        }

        public static int getTodayLate()
        {
            int result = 0;
            DateTime todayDate = DateTime.Now;
            //DateTime getDate = Convert.ToDateTime(todayDate).Date;
            using (var db = new Context())
            {
                var pers = db.persistent_attendance_log.Where(p => p.active && p.time_in != null && p.time_in > p.late_time).ToList();
                if (pers != null && pers.Count > 0)
                {
                    result = pers.Count();
                }
            }
            return result;
        }

        public static int getTodayEarlyOut()
        {
            int result = 0;
            DateTime todayDate = DateTime.Now;
            //DateTime getDate = Convert.ToDateTime(todayDate).Date;
            using (var db = new Context())
            {
                var pers = db.persistent_attendance_log.Where(p => p.active && p.time_out < p.shift_end && p.time_out != null).ToList();
                if (pers != null && pers.Count > 0)
                {
                    result = pers.Count();
                }
            }
            return result;
        }






        public static ViewModels.Dashboard getDashboardValuesOfEmployee(string empCode)
        {
            int onTime = 0, present = 0, late = 0, earlyGone = 0, absent = 0, leave = 0;
            ViewModels.Dashboard dashboard = new ViewModels.Dashboard();
            List<EmployeeCount> employeeCountList = new List<EmployeeCount>();
            using (var db = new Context())
            {
                int thisMonth = DateTime.Now.Month;
                int thisYear = DateTime.Now.Year;
                var supervisor = db.employee.Where(c => c.employee_code == empCode).FirstOrDefault();
                dashboard.supervisorName = supervisor.first_name + " " + supervisor.last_name;

                var group = db.group.Where(c => c.supervisor_id == supervisor.EmployeeId).FirstOrDefault();
                var employee = db.employee.Where(c => c.timetune_active == true && c.active == true && c.Group.GroupId == group.GroupId).ToList();
                // Get all the consolidated logs for this employee, from this month and year.
                foreach (var entity in employee)
                {
                    EmployeeCount employeeCount = new EmployeeCount();
                    employeeCount.employee = entity.first_name + " " + entity.last_name;
                    var consolidateLog = db.consolidated_attendance.Where(m => m.active &&
                        m.date.Value.Month.Equals(thisMonth) &&
                        m.date.Value.Year.Equals(thisYear) &&
                        m.employee.employee_code.Equals(entity.employee_code)).ToList();

                    onTime += consolidateLog.Where(m => m.final_remarks.Equals("PO") || m.final_remarks.Equals("POE") || m.final_remarks.Equals("POM")).Count();
                    late += consolidateLog.Where(m => m.final_remarks.Equals("PLO") || m.final_remarks.Equals("PLE") || m.final_remarks.Equals("PLM")).Count();
                    absent += consolidateLog.Where(m => m.final_remarks.Equals("AB")).Count();
                    leave += consolidateLog.Where(m => m.final_remarks.Equals("LV")).Count();
                    earlyGone += consolidateLog.Where(m => m.final_remarks.Equals("PLE") || m.final_remarks.Equals("POE") || m.final_remarks.Equals("PME")).Count();
                    present += consolidateLog.Where(m => m.final_remarks != "AB" && m.final_remarks != "LV" && m.final_remarks != "OFF").Count();
                    DateTime date = DateTime.Now.AddDays(-1).Date;
                    var log = db.persistent_attendance_log.Where(c => c.active == true && c.employee_code == entity.employee_code).FirstOrDefault();
                    if (log == null)
                    {
                        employeeCount.status = 0;
                    }
                    else
                    {
                        employeeCount.status = (log.time_in == null) ? 0 : 1;
                    }

                    employeeCountList.Add(employeeCount);
                }

                dashboard.employeeCount = employeeCountList;
                dashboard.dashboardElement.onTime = onTime;
                dashboard.dashboardElement.late = late;
                dashboard.dashboardElement.absent = absent;
                dashboard.dashboardElement.earlyGone = earlyGone;
                dashboard.dashboardElement.present = present;
                dashboard.dashboardElement.leave = leave;
            }

            return dashboard;
        }

        public static int searchEmployeeDashboard(string searchValue, string sortOrder, string empCode, int start, int length, out List<ViewModels.Employee> toReturn)
        {
            // If sort order does not contain any of the allowed
            // columns, then it must contain the ones that are not allowed.
            // So change sort order to something that is allowed.
            if (!(sortOrder.Contains("first_name") || sortOrder.Contains("last_name") || sortOrder.Contains("employee_code") || sortOrder.Contains("time_tune_status")))
            {
                sortOrder = "first_name";
            }



            int count = 0;
            using (Context db = new Context())
            {
                List<DLL.Models.Employee> employee = null;
                try
                {
                    int thisMonth = DateTime.Now.Month;
                    int thisYear = DateTime.Now.Year;
                    var supervisor = db.employee.Where(c => c.employee_code == empCode).FirstOrDefault();


                    var group = db.group.Where(c => c.supervisor_id == supervisor.EmployeeId).FirstOrDefault();
                    if (group == null)
                    {
                        toReturn = new List<ViewModels.Employee>();
                        return 0;
                    }
                    var employees = db.employee.Where(c => c.timetune_active == true && c.active == true && c.Group.GroupId == group.GroupId).AsQueryable();
                    if (searchValue != null && !searchValue.Equals(""))
                    {

                        // query 

                        // query the DB for count.
                        count = employees.Where(m => m.active &&
                        (m.first_name.Contains(searchValue) ||
                        m.last_name.Contains(searchValue) ||
                        m.employee_code.Contains(searchValue) ||
                        ((m.function != null && m.function.name != null) && m.function.name.Contains(searchValue)) ||
                        ((m.designation != null && m.designation.name != null) && m.designation.name.Contains(searchValue)) ||
                        ((m.location != null && m.location.name != null) && m.location.name.Contains(searchValue))
                        )).Count();

                        // load actual content.
                        employee = employees.Where(m => m.active &&
                        m.first_name.Contains(searchValue) ||
                        m.last_name.Contains(searchValue) ||
                        m.employee_code.Contains(searchValue) ||
                        m.function.name.Contains(searchValue) ||
                        m.designation.name.Contains(searchValue) ||
                        m.location.name.Contains(searchValue)).SortBy(sortOrder).Skip(start).Take(length).ToList();

                    }
                    else
                    {
                        // query the db for count
                        count = employees.Where(m => m.active).Count();

                        // load actual data.
                        employee = employees.Where(m => m.active)
                            .SortBy(sortOrder).Skip(start).Take(length).ToList();
                    }

                }
                catch (System.Data.Entity.Core.EntityCommandExecutionException ex)
                {
                    employee = new List<DLL.Models.Employee>();
                }



                toReturn = new List<ViewModels.Employee>();

                for (int i = 0; i < employee.Count(); i++)
                {
                    ViewModels.Employee employees = new ViewModels.Employee();
                    DLL.Models.Employee currentEmployee = employee[i];

                    employees.id = currentEmployee.EmployeeId;

                    employees.first_name = currentEmployee.first_name;

                    employees.last_name = currentEmployee.last_name;

                    employees.employee_code = currentEmployee.employee_code;

                    employees.department_name = (currentEmployee.department != null) ? currentEmployee.department.name : "";
                    employees.time_tune_status = (currentEmployee.timetune_active == true) ? "Active" : "Deactive";

                    employees.function_id = (currentEmployee.function != null) ? currentEmployee.function.FunctionId : -1;
                    employees.function_name = (currentEmployee.function != null) ? currentEmployee.function.name : "";

                    employees.designation_id = (currentEmployee.designation != null) ? currentEmployee.designation.DesignationId : -1;
                    employees.designation_name = (currentEmployee.designation != null) ? currentEmployee.designation.name : "";

                    employees.location_id = (currentEmployee.location != null) ? currentEmployee.location.LocationId : -1;
                    employees.location_name = (currentEmployee.location != null) ? currentEmployee.location.name : "";




                    toReturn.Add(employees);
                }

                return count;
            }
        }

        public static int searchEmployeeForLMTeam(string searchValue, string sortOrder, string empCode, int start, int length, out List<ViewModels.Employee> toReturn)
        {
            // If sort order does not contain any of the allowed
            // columns, then it must contain the ones that are not allowed.
            // So change sort order to something that is allowed.
            if (!(sortOrder.Contains("first_name") || sortOrder.Contains("last_name") || sortOrder.Contains("employee_code") || sortOrder.Contains("time_tune_status")))
            {
                sortOrder = "first_name";
            }



            int count = 0;
            using (Context db = new Context())
            {
                List<DLL.Models.Employee> employee = null;
                try
                {
                    int thisMonth = DateTime.Now.Month;
                    int thisYear = DateTime.Now.Year;
                    var supervisor = db.employee.Where(c => c.employee_code == empCode).FirstOrDefault();


                    var group = db.group.Where(c => c.supervisor_id == supervisor.EmployeeId).FirstOrDefault();
                    if (group == null)
                    {
                        toReturn = new List<ViewModels.Employee>();
                        return 0;
                    }
                    var employees = db.employee.Where(c => c.timetune_active == true && c.active == true && c.Group.GroupId == group.GroupId).AsQueryable();
                    if (searchValue != null && !searchValue.Equals(""))
                    {

                        // query 

                        // query the DB for count.
                        count = employees.Where(m => m.active &&
                        (m.first_name.Contains(searchValue) ||
                        m.last_name.Contains(searchValue) ||
                        m.employee_code.Contains(searchValue) ||
                        ((m.function != null && m.function.name != null) && m.function.name.Contains(searchValue)) ||
                        ((m.designation != null && m.designation.name != null) && m.designation.name.Contains(searchValue)) ||
                        ((m.location != null && m.location.name != null) && m.location.name.Contains(searchValue))
                        )).Count();

                        // load actual content.
                        employee = employees.Where(m => m.active &&
                        m.first_name.Contains(searchValue) ||
                        m.last_name.Contains(searchValue) ||
                        m.employee_code.Contains(searchValue) ||
                        m.function.name.Contains(searchValue) ||
                        m.designation.name.Contains(searchValue) ||
                        m.location.name.Contains(searchValue)).SortBy(sortOrder).Skip(start).Take(length).ToList();

                    }
                    else
                    {
                        // query the db for count
                        count = employees.Where(m => m.active).Count();

                        // load actual data.
                        employee = employees.Where(m => m.active)
                            .SortBy(sortOrder).Skip(start).Take(length).ToList();
                    }

                }
                catch (System.Data.Entity.Core.EntityCommandExecutionException ex)
                {
                    employee = new List<DLL.Models.Employee>();
                }



                toReturn = new List<ViewModels.Employee>();

                if (HttpContext.Current.Session["LMPresentCounter"] == null)
                {
                    HttpContext.Current.Session["LMPresentCounter"] = 0;
                }

                if (HttpContext.Current.Session["LMLeaveCounter"] == null)
                {
                    HttpContext.Current.Session["LMLeaveCounter"] = 0;
                }

                if (HttpContext.Current.Session["LMAbsentCounter"] == null)
                {
                    HttpContext.Current.Session["LMAbsentCounter"] = 0;
                }

                if (HttpContext.Current.Session["LMOnTimeCounter"] == null)
                {
                    HttpContext.Current.Session["LMOnTimeCounter"] = 0;
                }

                if (HttpContext.Current.Session["LMLateCounter"] == null)
                {
                    HttpContext.Current.Session["LMLateCounter"] = 0;
                }

                if (HttpContext.Current.Session["LMEarlyCounter"] == null)
                {
                    HttpContext.Current.Session["LMEarlyCounter"] = 0;
                }

                for (int i = 0; i < employee.Count(); i++)
                {
                    ViewModels.Employee employees = new ViewModels.Employee();
                    DLL.Models.Employee currentEmployee = employee[i];

                    employees.id = currentEmployee.EmployeeId;

                    employees.first_name = currentEmployee.first_name;

                    employees.last_name = currentEmployee.last_name;

                    employees.employee_code = currentEmployee.employee_code;

                    ViewModels.Dashboard lmDashboard = TimeTune.Dashboard.getDashboardValuesLM(currentEmployee.employee_code);
                    if (lmDashboard != null)
                    {
                        int present = lmDashboard.dashboardElement.present;
                        present = present + int.Parse(HttpContext.Current.Session["LMPresentCounter"].ToString() ?? "0");
                        HttpContext.Current.Session["LMPresentCounter"] = present;

                        int leave = lmDashboard.dashboardElement.leave;
                        leave = leave + int.Parse(HttpContext.Current.Session["LMLeaveCounter"].ToString() ?? "0");
                        HttpContext.Current.Session["LMLeaveCounter"] = leave;

                        int absent = lmDashboard.dashboardElement.absent;
                        absent = absent + int.Parse(HttpContext.Current.Session["LMAbsentCounter"].ToString() ?? "0");
                        HttpContext.Current.Session["LMAbsentCounter"] = absent;

                        int ontime = lmDashboard.dashboardElement.onTime;
                        ontime = ontime + int.Parse(HttpContext.Current.Session["LMOnTimeCounter"].ToString() ?? "0");
                        HttpContext.Current.Session["LMOnTimeCounter"] = ontime;

                        int late = lmDashboard.dashboardElement.late;
                        late = late + int.Parse(HttpContext.Current.Session["LMLateCounter"].ToString() ?? "0");
                        HttpContext.Current.Session["LMLateCounter"] = late;

                        int early = lmDashboard.dashboardElement.earlyGone;
                        early = early + int.Parse(HttpContext.Current.Session["LMEarlyCounter"].ToString() ?? "0");
                        HttpContext.Current.Session["LMEarlyCounter"] = early;
                    }


                    employees.department_name = (currentEmployee.department != null) ? currentEmployee.department.name : "";
                    employees.time_tune_status = (currentEmployee.timetune_active == true) ? "Active" : "Deactive";

                    employees.function_id = (currentEmployee.function != null) ? currentEmployee.function.FunctionId : -1;
                    employees.function_name = (currentEmployee.function != null) ? currentEmployee.function.name : "";

                    employees.designation_id = (currentEmployee.designation != null) ? currentEmployee.designation.DesignationId : -1;
                    employees.designation_name = (currentEmployee.designation != null) ? currentEmployee.designation.name : "";

                    employees.location_id = (currentEmployee.location != null) ? currentEmployee.location.LocationId : -1;
                    employees.location_name = (currentEmployee.location != null) ? currentEmployee.location.name : "";




                    toReturn.Add(employees);
                }

                return count;
            }
        }


        public static int searchEmployeeForHRTeam(string searchValue, string sortOrder, string empCode, int start, int length, out List<ViewModels.Employee> toReturn)
        {
            // If sort order does not contain any of the allowed
            // columns, then it must contain the ones that are not allowed.
            // So change sort order to something that is allowed.
            if (!(sortOrder.Contains("first_name") || sortOrder.Contains("last_name") || sortOrder.Contains("employee_code") || sortOrder.Contains("time_tune_status")))
            {
                sortOrder = "first_name";
            }



            int count = 0;
            using (Context db = new Context())
            {
                List<DLL.Models.Employee> employee = null;
                try
                {
                    //int thisMonth = DateTime.Now.Month;
                    //int thisYear = DateTime.Now.Year;
                    //var supervisor = db.employee.Where(c => c.employee_code == empCode).FirstOrDefault();


                    //var group = db.group.Where(c => c.supervisor_id == supervisor.EmployeeId).FirstOrDefault();
                    //if (group == null)
                    //{
                    //    toReturn = new List<ViewModels.Employee>();
                    //    return 0;
                    // }
                    var employees = db.employee.Where(c => c.timetune_active == true && c.active == true).AsQueryable();
                    if (searchValue != null && !searchValue.Equals(""))
                    {

                        // query 

                        // query the DB for count.
                        count = employees.Where(m => m.active &&
                        (m.first_name.Contains(searchValue) ||
                        m.last_name.Contains(searchValue) ||
                        m.employee_code.Contains(searchValue) ||
                        ((m.function != null && m.function.name != null) && m.function.name.Contains(searchValue)) ||
                        ((m.designation != null && m.designation.name != null) && m.designation.name.Contains(searchValue)) ||
                        ((m.location != null && m.location.name != null) && m.location.name.Contains(searchValue))
                        )).Count();

                        // load actual content.
                        employee = employees.Where(m => m.active &&
                        m.first_name.Contains(searchValue) ||
                        m.last_name.Contains(searchValue) ||
                        m.employee_code.Contains(searchValue) ||
                        m.function.name.Contains(searchValue) ||
                        m.designation.name.Contains(searchValue) ||
                        m.location.name.Contains(searchValue)).SortBy(sortOrder).Skip(start).Take(length).ToList();

                    }
                    else
                    {
                        // query the db for count
                        count = employees.Where(m => m.active).Count();

                        // load actual data.
                        employee = employees.Where(m => m.active)
                            .SortBy(sortOrder).Skip(start).Take(length).ToList();
                    }

                }
                catch (System.Data.Entity.Core.EntityCommandExecutionException ex)
                {
                    employee = new List<DLL.Models.Employee>();
                }



                toReturn = new List<ViewModels.Employee>();

                for (int i = 0; i < employee.Count(); i++)
                {
                    ViewModels.Employee employees = new ViewModels.Employee();
                    DLL.Models.Employee currentEmployee = employee[i];

                    employees.id = currentEmployee.EmployeeId;

                    employees.first_name = currentEmployee.first_name;

                    employees.last_name = currentEmployee.last_name;

                    employees.employee_code = currentEmployee.employee_code;


                    employees.department_name = (currentEmployee.department != null) ? currentEmployee.department.name : "";
                    employees.time_tune_status = (currentEmployee.timetune_active == true) ? "Active" : "Deactive";

                    employees.function_id = (currentEmployee.function != null) ? currentEmployee.function.FunctionId : -1;
                    employees.function_name = (currentEmployee.function != null) ? currentEmployee.function.name : "";

                    employees.designation_id = (currentEmployee.designation != null) ? currentEmployee.designation.DesignationId : -1;
                    employees.designation_name = (currentEmployee.designation != null) ? currentEmployee.designation.name : "";

                    employees.location_id = (currentEmployee.location != null) ? currentEmployee.location.LocationId : -1;
                    employees.location_name = (currentEmployee.location != null) ? currentEmployee.location.name : "";

                    toReturn.Add(employees);
                }

                return count;
            }
        }


        public static int generatePDF_EMP(string searchValue, string empCode, out List<ViewModels.Employee> toReturn)
        {
            // If sort order does not contain any of the allowed
            // columns, then it must contain the ones that are not allowed.
            // So change sort order to something that is allowed.
            //if (!(sortOrder.Contains("first_name") || sortOrder.Contains("last_name") || sortOrder.Contains("employee_code") || sortOrder.Contains("time_tune_status")))
            //{
            //    sortOrder = "first_name";
            //}



            int count = 0;
            using (Context db = new Context())
            {
                List<DLL.Models.Employee> employee = null;
                try
                {
                    int thisMonth = DateTime.Now.Month;
                    int thisYear = DateTime.Now.Year;
                    var supervisor = db.employee.Where(c => c.employee_code == empCode).FirstOrDefault();


                    var group = db.group.Where(c => c.supervisor_id == supervisor.EmployeeId).FirstOrDefault();
                    if (group == null)
                    {
                        toReturn = new List<ViewModels.Employee>();
                        return 0;
                    }
                    var employees = db.employee.Where(c => c.timetune_active == true && c.active == true && c.Group.GroupId == group.GroupId).AsQueryable();
                    if (searchValue != null && !searchValue.Equals(""))
                    {

                        // query 

                        // query the DB for count.
                        count = employees.Where(m => m.active &&
                        (m.first_name.Contains(searchValue) ||
                        m.last_name.Contains(searchValue) ||
                        m.employee_code.Contains(searchValue) ||
                        ((m.function != null && m.function.name != null) && m.function.name.Contains(searchValue)) ||
                        ((m.designation != null && m.designation.name != null) && m.designation.name.Contains(searchValue)) ||
                        ((m.location != null && m.location.name != null) && m.location.name.Contains(searchValue))
                        )).Count();

                        // load actual content.
                        employee = employees.Where(m => m.active &&
                        m.first_name.Contains(searchValue) ||
                        m.last_name.Contains(searchValue) ||
                        m.employee_code.Contains(searchValue) ||
                        m.function.name.Contains(searchValue) ||
                        m.designation.name.Contains(searchValue) ||
                        m.location.name.Contains(searchValue)).ToList();//.SortBy(sortOrder).Skip(start).Take(length).ToList();

                    }
                    else
                    {
                        // query the db for count
                        count = employees.Where(m => m.active).Count();

                        // load actual data.
                        employee = employees.Where(m => m.active).ToList();
                        //.SortBy(sortOrder).Skip(start).Take(length).ToList();
                    }

                }
                catch (System.Data.Entity.Core.EntityCommandExecutionException ex)
                {
                    employee = new List<DLL.Models.Employee>();
                }



                toReturn = new List<ViewModels.Employee>();

                for (int i = 0; i < employee.Count(); i++)
                {
                    ViewModels.Employee employees = new ViewModels.Employee();
                    DLL.Models.Employee currentEmployee = employee[i];

                    employees.id = currentEmployee.EmployeeId;

                    employees.first_name = currentEmployee.first_name;

                    employees.last_name = currentEmployee.last_name;

                    employees.employee_code = currentEmployee.employee_code;

                    employees.department_name = (currentEmployee.department != null) ? currentEmployee.department.name : "";
                    employees.time_tune_status = (currentEmployee.timetune_active == true) ? "Active" : "Deactive";

                    employees.function_id = (currentEmployee.function != null) ? currentEmployee.function.FunctionId : -1;
                    employees.function_name = (currentEmployee.function != null) ? currentEmployee.function.name : "";

                    employees.designation_id = (currentEmployee.designation != null) ? currentEmployee.designation.DesignationId : -1;
                    employees.designation_name = (currentEmployee.designation != null) ? currentEmployee.designation.name : "";

                    employees.location_id = (currentEmployee.location != null) ? currentEmployee.location.LocationId : -1;
                    employees.location_name = (currentEmployee.location != null) ? currentEmployee.location.name : "";




                    toReturn.Add(employees);
                }

                return count;
            }
        }

        public static int generatePDF_LM(string searchValue, string empCode, out List<ViewModels.Employee> toReturn)
        {
            // If sort order does not contain any of the allowed
            // columns, then it must contain the ones that are not allowed.
            // So change sort order to something that is allowed.
            //if (!(sortOrder.Contains("first_name") || sortOrder.Contains("last_name") || sortOrder.Contains("employee_code") || sortOrder.Contains("time_tune_status")))
            //{
            //    sortOrder = "first_name";
            //}



            int count = 0;
            using (Context db = new Context())
            {
                List<DLL.Models.Employee> employee = null;
                try
                {
                    int thisMonth = DateTime.Now.Month;
                    int thisYear = DateTime.Now.Year;
                    var supervisor = db.employee.Where(c => c.employee_code == empCode).FirstOrDefault();


                    var group = db.group.Where(c => c.supervisor_id == supervisor.EmployeeId).FirstOrDefault();
                    if (group == null)
                    {
                        toReturn = new List<ViewModels.Employee>();
                        return 0;
                    }
                    var employees = db.employee.Where(c => c.timetune_active == true && c.active == true && c.Group.GroupId == group.GroupId).AsQueryable();
                    if (searchValue != null && !searchValue.Equals(""))
                    {

                        // query 

                        // query the DB for count.
                        count = employees.Where(m => m.active &&
                        (m.first_name.Contains(searchValue) ||
                        m.last_name.Contains(searchValue) ||
                        m.employee_code.Contains(searchValue) ||
                        ((m.function != null && m.function.name != null) && m.function.name.Contains(searchValue)) ||
                        ((m.designation != null && m.designation.name != null) && m.designation.name.Contains(searchValue)) ||
                        ((m.location != null && m.location.name != null) && m.location.name.Contains(searchValue))
                        )).Count();

                        // load actual content.
                        employee = employees.Where(m => m.active &&
                        m.first_name.Contains(searchValue) ||
                        m.last_name.Contains(searchValue) ||
                        m.employee_code.Contains(searchValue) ||
                        m.function.name.Contains(searchValue) ||
                        m.designation.name.Contains(searchValue) ||
                        m.location.name.Contains(searchValue)).ToList();//.SortBy(sortOrder).Skip(start).Take(length).ToList();

                    }
                    else
                    {
                        // query the db for count
                        count = employees.Where(m => m.active).Count();

                        // load actual data.
                        employee = employees.Where(m => m.active).ToList();
                        //.SortBy(sortOrder).Skip(start).Take(length).ToList();
                    }

                }
                catch (System.Data.Entity.Core.EntityCommandExecutionException ex)
                {
                    employee = new List<DLL.Models.Employee>();
                }



                toReturn = new List<ViewModels.Employee>();

                for (int i = 0; i < employee.Count(); i++)
                {
                    ViewModels.Employee employees = new ViewModels.Employee();
                    DLL.Models.Employee currentEmployee = employee[i];

                    employees.id = currentEmployee.EmployeeId;

                    employees.first_name = currentEmployee.first_name;

                    employees.last_name = currentEmployee.last_name;

                    employees.employee_code = currentEmployee.employee_code;

                    employees.department_name = (currentEmployee.department != null) ? currentEmployee.department.name : "";
                    employees.time_tune_status = (currentEmployee.timetune_active == true) ? "Active" : "Deactive";

                    employees.function_id = (currentEmployee.function != null) ? currentEmployee.function.FunctionId : -1;
                    employees.function_name = (currentEmployee.function != null) ? currentEmployee.function.name : "";

                    employees.designation_id = (currentEmployee.designation != null) ? currentEmployee.designation.DesignationId : -1;
                    employees.designation_name = (currentEmployee.designation != null) ? currentEmployee.designation.name : "";

                    employees.location_id = (currentEmployee.location != null) ? currentEmployee.location.LocationId : -1;
                    employees.location_name = (currentEmployee.location != null) ? currentEmployee.location.name : "";




                    toReturn.Add(employees);
                }

                return count;
            }
        }


        //
        public static int searchEmployeeDashboardSLM(string searchValue, string sortOrder, string empCode, int start, int length, out List<ViewModels.Employee> toReturn)
        {
            // If sort order does not contain any of the allowed
            // columns, then it must contain the ones that are not allowed.
            // So change sort order to something that is allowed.
            if (!(sortOrder.Contains("first_name") || sortOrder.Contains("last_name") || sortOrder.Contains("employee_code") || sortOrder.Contains("time_tune_status")))
            {
                sortOrder = "first_name";
            }



            int count = 0;
            using (Context db = new Context())
            {
                List<DLL.Models.Employee> employee = null;
                try
                {
                    int thisMonth = DateTime.Now.Month;
                    int thisYear = DateTime.Now.Year;
                    var supervisor = db.employee.Where(c => c.employee_code == empCode).FirstOrDefault();


                    var employees = db.super_line_manager_tagging.Where(c => c.superLineManager.EmployeeId == supervisor.EmployeeId).ToList();
                    if (searchValue != null && !searchValue.Equals(""))
                    {

                        // query 

                        // query the DB for count.
                        count = employees.Where(m => m.active &&
                        (m.taggedEmployee.first_name.Contains(searchValue) ||
                        m.taggedEmployee.last_name.Contains(searchValue) ||
                        m.taggedEmployee.employee_code.Contains(searchValue) ||
                        ((m.taggedEmployee.function != null && m.taggedEmployee.function.name != null) && m.taggedEmployee.function.name.Contains(searchValue)) ||
                        ((m.taggedEmployee.designation != null && m.taggedEmployee.designation.name != null) && m.taggedEmployee.designation.name.Contains(searchValue)) ||
                        ((m.taggedEmployee.location != null && m.taggedEmployee.location.name != null) && m.taggedEmployee.location.name.Contains(searchValue))
                        )).Count();

                        // load actual content.
                        employee = employees.Where(m => m.active &&
                        m.taggedEmployee.first_name.Contains(searchValue) ||
                        m.taggedEmployee.last_name.Contains(searchValue) ||
                        m.taggedEmployee.employee_code.Contains(searchValue) ||
                        m.taggedEmployee.function.name.Contains(searchValue) ||
                        m.taggedEmployee.designation.name.Contains(searchValue) ||
                        m.taggedEmployee.location.name.Contains(searchValue)).AsQueryable().Select(c => c.taggedEmployee).SortBy(sortOrder).Skip(start).Take(length).ToList();

                    }
                    else
                    {
                        // query the db for count
                        count = employees.Count();

                        // load actual data.
                        employee = employees.AsQueryable().Select(c => c.taggedEmployee)
                            .SortBy(sortOrder).Skip(start).Take(length).ToList();
                    }

                }
                catch (System.Data.Entity.Core.EntityCommandExecutionException ex)
                {
                    employee = new List<DLL.Models.Employee>();
                }



                toReturn = new List<ViewModels.Employee>();

                for (int i = 0; i < employee.Count(); i++)
                {
                    ViewModels.Employee employees = new ViewModels.Employee();
                    DLL.Models.Employee currentEmployee = employee[i];

                    employees.id = currentEmployee.EmployeeId;

                    employees.first_name = currentEmployee.first_name;

                    employees.last_name = currentEmployee.last_name;

                    employees.employee_code = currentEmployee.employee_code;

                    employees.department_name = (currentEmployee.department != null) ? currentEmployee.department.name : "";
                    employees.time_tune_status = (currentEmployee.timetune_active == true) ? "Active" : "Deactive";

                    employees.function_id = (currentEmployee.function != null) ? currentEmployee.function.FunctionId : -1;
                    employees.function_name = (currentEmployee.function != null) ? currentEmployee.function.name : "";

                    employees.designation_id = (currentEmployee.designation != null) ? currentEmployee.designation.DesignationId : -1;
                    employees.designation_name = (currentEmployee.designation != null) ? currentEmployee.designation.name : "";

                    employees.location_id = (currentEmployee.location != null) ? currentEmployee.location.LocationId : -1;
                    employees.location_name = (currentEmployee.location != null) ? currentEmployee.location.name : "";




                    toReturn.Add(employees);
                }

                return count;
            }
        }

        public static int generatePDF_SLM(string searchValue, string empCode, out List<ViewModels.Employee> toReturn)
        {
            // If sort order does not contain any of the allowed
            // columns, then it must contain the ones that are not allowed.
            // So change sort order to something that is allowed.
            //if (!(sortOrder.Contains("first_name") || sortOrder.Contains("last_name") || sortOrder.Contains("employee_code") || sortOrder.Contains("time_tune_status")))
            //{
            //    sortOrder = "first_name";
            //}



            int count = 0;
            using (Context db = new Context())
            {
                List<DLL.Models.Employee> employee = null;
                try
                {
                    int thisMonth = DateTime.Now.Month;
                    int thisYear = DateTime.Now.Year;
                    var supervisor = db.employee.Where(c => c.employee_code == empCode).FirstOrDefault();


                    var employees = db.super_line_manager_tagging.Where(c => c.superLineManager.EmployeeId == supervisor.EmployeeId).ToList();
                    if (searchValue != null && !searchValue.Equals(""))
                    {

                        // query 

                        // query the DB for count.
                        count = employees.Where(m => m.active &&
                        (m.taggedEmployee.first_name.Contains(searchValue) ||
                        m.taggedEmployee.last_name.Contains(searchValue) ||
                        m.taggedEmployee.employee_code.Contains(searchValue) ||
                        ((m.taggedEmployee.function != null && m.taggedEmployee.function.name != null) && m.taggedEmployee.function.name.Contains(searchValue)) ||
                        ((m.taggedEmployee.designation != null && m.taggedEmployee.designation.name != null) && m.taggedEmployee.designation.name.Contains(searchValue)) ||
                        ((m.taggedEmployee.location != null && m.taggedEmployee.location.name != null) && m.taggedEmployee.location.name.Contains(searchValue))
                        )).Count();

                        // load actual content.
                        employee = employees.Where(m => m.active &&
                        m.taggedEmployee.first_name.Contains(searchValue) ||
                        m.taggedEmployee.last_name.Contains(searchValue) ||
                        m.taggedEmployee.employee_code.Contains(searchValue) ||
                        m.taggedEmployee.function.name.Contains(searchValue) ||
                        m.taggedEmployee.designation.name.Contains(searchValue) ||
                        m.taggedEmployee.location.name.Contains(searchValue)).AsQueryable().Select(c => c.taggedEmployee).ToList(); //.SortBy(sortOrder).Skip(start).Take(length).ToList();

                    }
                    else
                    {
                        // query the db for count
                        count = employees.Count();

                        // load actual data.
                        employee = employees.AsQueryable().Select(c => c.taggedEmployee).ToList();
                        //.SortBy(sortOrder).Skip(start).Take(length).ToList();
                    }

                }
                catch (System.Data.Entity.Core.EntityCommandExecutionException ex)
                {
                    employee = new List<DLL.Models.Employee>();
                }



                toReturn = new List<ViewModels.Employee>();

                for (int i = 0; i < employee.Count(); i++)
                {
                    ViewModels.Employee employees = new ViewModels.Employee();
                    DLL.Models.Employee currentEmployee = employee[i];

                    employees.id = currentEmployee.EmployeeId;

                    employees.first_name = currentEmployee.first_name;

                    employees.last_name = currentEmployee.last_name;

                    employees.employee_code = currentEmployee.employee_code;

                    employees.department_name = (currentEmployee.department != null) ? currentEmployee.department.name : "";
                    employees.time_tune_status = (currentEmployee.timetune_active == true) ? "Active" : "Deactive";

                    employees.function_id = (currentEmployee.function != null) ? currentEmployee.function.FunctionId : -1;
                    employees.function_name = (currentEmployee.function != null) ? currentEmployee.function.name : "";

                    employees.designation_id = (currentEmployee.designation != null) ? currentEmployee.designation.DesignationId : -1;
                    employees.designation_name = (currentEmployee.designation != null) ? currentEmployee.designation.name : "";

                    employees.location_id = (currentEmployee.location != null) ? currentEmployee.location.LocationId : -1;
                    employees.location_name = (currentEmployee.location != null) ? currentEmployee.location.name : "";




                    toReturn.Add(employees);
                }

                return count;
            }
        }



        public static ViewModels.Dashboard getDashboardValueOfLineManagers(string empCode)
        {
            int onTime = 0, present = 0, late = 0, earlyGone = 0, absent = 0, leave = 0;
            string headName = "";
            ViewModels.Dashboard dashboard = new ViewModels.Dashboard();
            using (var db = new Context())
            {
                int thisMonth = DateTime.Now.Month;
                int thisYear = DateTime.Now.Year;
                List<EmployeeCount> employeeCountList = new List<EmployeeCount>();

                var tagging = db.super_line_manager_tagging.Where(c => c.superLineManager.employee_code == empCode).ToList();
                if (tagging.Count > 0)
                {
                    dashboard.supervisorName = tagging.FirstOrDefault().superLineManager.first_name + " " + tagging.FirstOrDefault().superLineManager.first_name;
                }
                else
                {
                    dashboard.supervisorName = "";
                }

                // Get all the consolidated logs for this employee, from this month and year.
                foreach (var entity in tagging)
                {
                    EmployeeCount employeeCount = new EmployeeCount();
                    var consolidateLog = db.consolidated_attendance.Where(m => m.active &&
                        m.date.Value.Month.Equals(thisMonth) &&
                        m.date.Value.Year.Equals(thisYear) &&
                        m.employee.employee_code.Equals(entity.taggedEmployee.employee_code)).ToList();
                    employeeCount.employee = entity.taggedEmployee.first_name + " " + entity.taggedEmployee.last_name;
                    onTime += consolidateLog.Where(m => m.final_remarks.Equals("PO") || m.final_remarks.Equals("POE") || m.final_remarks.Equals("POM")).Count();
                    late += consolidateLog.Where(m => m.final_remarks.Equals("PLO") || m.final_remarks.Equals("PLE") || m.final_remarks.Equals("PLM")).Count();
                    absent += consolidateLog.Where(m => m.final_remarks.Equals("AB")).Count();
                    leave += consolidateLog.Where(m => m.final_remarks.Equals("LV")).Count();
                    earlyGone += consolidateLog.Where(m => m.final_remarks.Equals("PLE") || m.final_remarks.Equals("POE") || m.final_remarks.Equals("PME")).Count();
                    present += consolidateLog.Where(m => m.final_remarks != "AB" && m.final_remarks != "LV" && m.final_remarks != "OFF").Count();
                    // DateTime date = DateTime.Now.AddDays(-1).Date;
                    var log = db.persistent_attendance_log.Where(c => c.active == true && c.employee_code == entity.taggedEmployee.employee_code).FirstOrDefault();
                    if (log == null)
                    {
                        employeeCount.status = 0;
                    }
                    else
                    {

                        employeeCount.status = (log.time_in == null) ? 0 : 1;

                    }
                    employeeCountList.Add(employeeCount);
                    var tag = db.super_line_manager_tagging.Where(c => c.taggedEmployee.employee_code == empCode).FirstOrDefault();
                    if (tag != null)
                    {
                        headName = tag.superLineManager.first_name + " " + tag.superLineManager.last_name;
                    }
                    else
                    {
                        headName = "";
                    }
                }

                dashboard.headName = headName;
                dashboard.employeeCount = employeeCountList;
                dashboard.dashboardElement.onTime = onTime;
                dashboard.dashboardElement.late = late;
                dashboard.dashboardElement.absent = absent;
                dashboard.dashboardElement.earlyGone = earlyGone;
                dashboard.dashboardElement.present = present;
                dashboard.dashboardElement.leave = leave;
            }

            return dashboard;
        }

        public static ViewModels.Dashboard getGraphValues(string department_id, string designatino_id, string employeeCode, string location_id, string from_date, string to_date)
        {
            int onTime = 0, present = 0, late = 0, earlyGone = 0, absent = 0, leave = 0;
            int depart_id, des_id, fun_id, loc_id;
            if (!int.TryParse(department_id, out depart_id))
                depart_id = -1;
            if (!int.TryParse(designatino_id, out des_id))
                des_id = -1;

            if (!int.TryParse(location_id, out loc_id))
                loc_id = -1;

            if (from_date == null && to_date == null)
            {
                return new ViewModels.Dashboard();
            }
            DateTime? from = DateTime.ParseExact(from_date, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            DateTime? to = DateTime.ParseExact(to_date, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            using (var db = new Context())
            {
                var function = db.employee.Where(c => c.employee_code.Equals(employeeCode) && c.active == true).FirstOrDefault().function;
                if (function != null)
                {
                    fun_id = function.FunctionId;
                }
                else
                {
                    fun_id = -1;
                }
                List<ConsolidatedAttendance> consolidateLog = new List<ConsolidatedAttendance>();
                if (fun_id == -1)
                {


                    if (depart_id == -1 && loc_id == -1 && des_id == -1)
                    {
                        consolidateLog = db.consolidated_attendance.Where(m => m.active &&
                     m.date.Value >= from &&
                     m.date.Value <= to).ToList();
                    }
                    else if (depart_id != -1 && loc_id == -1 && des_id == -1)
                    {
                        consolidateLog = db.consolidated_attendance.Where(m => m.active &&
                      m.date.Value >= from &&
                      m.date.Value <= to && m.employee.department.DepartmentId == depart_id).ToList();

                    }
                    else if (depart_id == -1 && loc_id == -1 && des_id == -1)
                    {
                        consolidateLog = db.consolidated_attendance.Where(m => m.active &&
                      m.date.Value >= from &&
                      m.date.Value <= to).ToList();

                    }
                    else if (depart_id == -1 && loc_id != -1 && des_id == -1)
                    {
                        consolidateLog = db.consolidated_attendance.Where(m => m.active &&
                      m.date.Value >= from &&
                      m.date.Value <= to && m.employee.location.LocationId == loc_id).ToList();

                    }
                    else if (depart_id == -1 && loc_id == -1 && des_id != -1)
                    {
                        consolidateLog = db.consolidated_attendance.Where(m => m.active &&
                      m.date.Value >= from &&
                      m.date.Value <= to && m.employee.designation.DesignationId == des_id).ToList();

                    }
                    else if (depart_id != -1 && loc_id != -1 && des_id != -1)
                    {
                        consolidateLog = db.consolidated_attendance.Where(m => m.active &&
                      m.date.Value >= from &&
                      m.date.Value <= to && (m.employee.department.DepartmentId == depart_id || m.employee.designation.DesignationId == des_id
                        || m.employee.location.LocationId == loc_id)).ToList();

                    }
                    else
                        consolidateLog = db.consolidated_attendance.Where(m => m.active &&
                           m.date.Value >= from &&
                           m.date.Value <= to || (m.employee.department.DepartmentId == depart_id || m.employee.designation.DesignationId == des_id || m.employee.location.LocationId == loc_id)).ToList();
                }
                else
                {
                    if (depart_id == -1 && loc_id == -1 && des_id == -1)
                    {
                        consolidateLog = db.consolidated_attendance.Where(m => m.active &&
                     m.date.Value >= from &&
                     m.date.Value <= to && m.employee.function.FunctionId == fun_id).ToList();
                    }
                    else if (depart_id != -1 && loc_id == -1 && des_id == -1)
                    {
                        consolidateLog = db.consolidated_attendance.Where(m => m.active &&
                      m.date.Value >= from &&
                      m.date.Value <= to && m.employee.department.DepartmentId == depart_id && m.employee.function.FunctionId == fun_id).ToList();

                    }
                    else if (depart_id == -1 && loc_id == -1 && des_id == -1)
                    {
                        consolidateLog = db.consolidated_attendance.Where(m => m.active &&
                      m.date.Value >= from &&
                      m.date.Value <= to && m.employee.function.FunctionId == fun_id).ToList();

                    }
                    else if (depart_id == -1 && loc_id != -1 && des_id == -1)
                    {
                        consolidateLog = db.consolidated_attendance.Where(m => m.active &&
                      m.date.Value >= from &&
                      m.date.Value <= to && m.employee.location.LocationId == loc_id && m.employee.function.FunctionId == fun_id).ToList();

                    }
                    else if (depart_id == -1 && loc_id == -1 && des_id != -1)
                    {
                        consolidateLog = db.consolidated_attendance.Where(m => m.active &&
                      m.date.Value >= from &&
                      m.date.Value <= to && m.employee.designation.DesignationId == des_id && m.employee.function.FunctionId == fun_id).ToList();

                    }
                    else if (depart_id != -1 && loc_id != -1 && des_id != -1)
                    {
                        consolidateLog = db.consolidated_attendance.Where(m => m.active &&
                      m.date.Value >= from &&
                      m.date.Value <= to && (m.employee.department.DepartmentId == depart_id || m.employee.designation.DesignationId == des_id
                       || m.employee.function.FunctionId == fun_id || m.employee.location.LocationId == loc_id)).ToList();

                    }
                    else
                        consolidateLog = db.consolidated_attendance.Where(m => m.active &&
                           m.date.Value >= from &&
                           m.date.Value <= to || (m.employee.department.DepartmentId == depart_id || m.employee.designation.DesignationId == des_id
                           && m.employee.function.FunctionId == fun_id || m.employee.location.LocationId == loc_id)).ToList();
                }
                onTime = consolidateLog.Where(m => m.final_remarks.Equals("PO") || m.final_remarks.Equals("POE") || m.final_remarks.Equals("POM")).Count();
                late = consolidateLog.Where(m => m.final_remarks.Equals("PLO") || m.final_remarks.Equals("PLE") || m.final_remarks.Equals("PLM")).Count();
                absent = consolidateLog.Where(m => m.final_remarks.Equals("AB")).Count();
                leave = consolidateLog.Where(m => m.final_remarks.Equals("LV")).Count();
                earlyGone = consolidateLog.Where(m => m.final_remarks.Equals("PLE") || m.final_remarks.Equals("POE") || m.final_remarks.Equals("PME")).Count();
                present = consolidateLog.Where(m => m.final_remarks != "AB" && m.final_remarks != "LV" && m.final_remarks != "OFF").Count();

            }
            ViewModels.Dashboard dashboard = new ViewModels.Dashboard();
            dashboard.dashboardElement.onTime = onTime;
            dashboard.dashboardElement.late = late;
            dashboard.dashboardElement.absent = absent;
            dashboard.dashboardElement.earlyGone = earlyGone;
            dashboard.dashboardElement.present = present;
            dashboard.dashboardElement.leave = leave;

            return dashboard;

        }

        public static ViewModels.Dashboard graphForConsolidated(string employee_id, string from_date, string to_date)
        {
            int onTime, late, earlyGone, present, absent, leave;
            if (from_date == null && to_date == null)
            {
                return new ViewModels.Dashboard();
            }
            else
            {
                DateTime from = DateTime.ParseExact(from_date, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                DateTime to = DateTime.ParseExact(to_date, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                int employeeId = (employee_id != null) ? int.Parse(employee_id) : 0;
                List<ConsolidatedAttendance> logs = new List<ConsolidatedAttendance>();
                using (var db = new Context())
                {
                    if (employeeId == 0)
                    {
                        logs = db.consolidated_attendance.Where(c => c.active == true
                       && c.date >= from && c.date <= to).ToList();
                    }
                    else
                    {
                        logs = db.consolidated_attendance.Where(c => c.active == true && c.employee.EmployeeId == employeeId
                       && c.date >= from && c.date <= to).ToList();
                    }

                    onTime = logs.Where(m => m.final_remarks.Equals("PO") || m.final_remarks.Equals("POE") || m.final_remarks.Equals("POM")).Count();
                    late = logs.Where(m => m.final_remarks.Equals("PLO") || m.final_remarks.Equals("PLE") || m.final_remarks.Equals("PLM")).Count();
                    absent = logs.Where(m => m.final_remarks.Equals("AB")).Count();
                    leave = logs.Where(m => m.final_remarks.Equals("LV")).Count();
                    earlyGone = logs.Where(m => m.final_remarks.Equals("PLE") || m.final_remarks.Equals("POE") || m.final_remarks.Equals("PME")).Count();
                    present = logs.Where(m => m.final_remarks != "AB" && m.final_remarks != "LV" && m.final_remarks != "OFF").Count();
                }
                ViewModels.Dashboard dashboard = new ViewModels.Dashboard();
                dashboard.dashboardElement.onTime = onTime;
                dashboard.dashboardElement.late = late;
                dashboard.dashboardElement.absent = absent;
                dashboard.dashboardElement.earlyGone = earlyGone;
                dashboard.dashboardElement.present = present;
                dashboard.dashboardElement.leave = leave;

                return dashboard;
            }
        }

        public static ViewModels.Dashboard graphForConsolidatedForLm(string employee_id, string from_date, string to_date)
        {
            int onTime, late, earlyGone, present, absent, leave;
            if (from_date == null && to_date == null)
            {
                return new ViewModels.Dashboard();
            }
            else
            {
                DateTime from = DateTime.ParseExact(from_date, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                DateTime to = DateTime.ParseExact(to_date, "dd-MM-yyyy", CultureInfo.InvariantCulture);

                List<ConsolidatedAttendance> logs = new List<ConsolidatedAttendance>();
                using (var db = new Context())
                {
                    if (employee_id == null)
                    {
                        logs = db.consolidated_attendance.Where(c => c.active == true
                       && c.date >= from && c.date <= to).ToList();
                    }
                    else
                    {
                        logs = db.consolidated_attendance.Where(c => c.active == true && c.employee.employee_code == employee_id
                       && c.date >= from && c.date <= to).ToList();
                    }

                    onTime = logs.Where(m => m.final_remarks.Equals("PO") || m.final_remarks.Equals("POE") || m.final_remarks.Equals("POM")).Count();
                    late = logs.Where(m => m.final_remarks.Equals("PLO") || m.final_remarks.Equals("PLE") || m.final_remarks.Equals("PLM")).Count();
                    absent = logs.Where(m => m.final_remarks.Equals("AB")).Count();
                    leave = logs.Where(m => m.final_remarks.Equals("LV")).Count();
                    earlyGone = logs.Where(m => m.final_remarks.Equals("PLE") || m.final_remarks.Equals("POE") || m.final_remarks.Equals("PME")).Count();
                    present = logs.Where(m => m.final_remarks != "AB" && m.final_remarks != "LV" && m.final_remarks != "OFF").Count();
                }
                ViewModels.Dashboard dashboard = new ViewModels.Dashboard();
                dashboard.dashboardElement.onTime = onTime;
                dashboard.dashboardElement.late = late;
                dashboard.dashboardElement.absent = absent;
                dashboard.dashboardElement.earlyGone = earlyGone;
                dashboard.dashboardElement.present = present;
                dashboard.dashboardElement.leave = leave;

                return dashboard;
            }
        }

        public static void manageLeaveSessionForUsers(string User_Code) //actually Staff
        {
            using (Context db = new Context())
            {
                var data_user = db.employee.Where(e => e.active && e.employee_code == User_Code).FirstOrDefault();
                if (data_user != null)
                {
                    var data_leave_session = db.leave_session.Where(l => l.EmployeeId == data_user.EmployeeId).OrderByDescending(o => o.id).FirstOrDefault();// l.SessionEndDate >= DateTime.Now
                    if (data_leave_session == null)
                    {
                        DateTime dtStart = new DateTime(); DateTime dtEnd = new DateTime();

                        if (data_user.date_of_joining != null)
                        {
                            int stMonth = 0, stDay = 0, enMonth = 0, enDay = 0;
                            stMonth = int.Parse(System.Web.HttpContext.Current.Session["GV_SessionStartMonth"].ToString()); stDay = int.Parse(System.Web.HttpContext.Current.Session["GV_SessionStartDay"].ToString());
                            enMonth = int.Parse(System.Web.HttpContext.Current.Session["GV_SessionEndMonth"].ToString()); enDay = int.Parse(System.Web.HttpContext.Current.Session["GV_SessionEndDay"].ToString());


                            ////DateTime dtJoiningDate = Convert.ToDateTime(DateTime.Now.Year + "-" + data_user.date_of_joining.Value.Month + "-" + data_user.date_of_joining.Value.Day);
                            DateTime dtJoiningDate = new DateTime(DateTime.Now.Year, data_user.date_of_joining.Value.Month, data_user.date_of_joining.Value.Day);
                            if (dtJoiningDate.Year == DateTime.Now.Year)
                            {
                                dtStart = new DateTime(DateTime.Now.Year, data_user.date_of_joining.Value.Month, data_user.date_of_joining.Value.Day);
                                dtEnd = new DateTime(DateTime.Now.Year, enMonth, enDay);
                            }
                            else if (dtJoiningDate.Year < DateTime.Now.Year)
                            {
                                dtStart = new DateTime(DateTime.Now.Year - 1, data_user.date_of_joining.Value.Month, data_user.date_of_joining.Value.Day);
                                dtEnd = new DateTime(DateTime.Now.Year, enMonth, enDay);
                            }
                            else
                            {
                                dtStart = new DateTime(DateTime.Now.Year, data_user.date_of_joining.Value.Month, data_user.date_of_joining.Value.Day);
                                dtEnd = new DateTime(DateTime.Now.Year, enMonth, enDay);
                            }

                            //////DateTime dtJoiningDate = Convert.ToDateTime(DateTime.Now.Year + "-" + data_user.date_of_joining.Value.Month + "-" + data_user.date_of_joining.Value.Day);
                            //DateTime dtJoiningDate = new DateTime(DateTime.Now.Year, data_user.date_of_joining.Value.Month, data_user.date_of_joining.Value.Day);
                            //if (dtJoiningDate == new DateTime(DateTime.Now.Year, 6, 30) || dtJoiningDate == new DateTime(DateTime.Now.Year, 7, 1) || dtJoiningDate == new DateTime(DateTime.Now.Year, 7, 2))
                            //{
                            //    dtStart = new DateTime(DateTime.Now.Year, 7, 1);
                            //    dtEnd = new DateTime((DateTime.Now.Year + 1), 6, 30);
                            //}
                            //else if (dtJoiningDate < new DateTime(DateTime.Now.Year, 6, 30)) //like month less than June month - Feb 
                            //{
                            //    dtStart = new DateTime((DateTime.Now.Year - 1), data_user.date_of_joining.Value.Month, data_user.date_of_joining.Value.Day);
                            //    dtEnd = new DateTime(DateTime.Now.Year, 6, 30);
                            //}
                            //else //if (data_user.date_of_joining > new DateTime(DateTime.Now.Year, 6, 30)) //like month greater than June month - Aug 
                            //{
                            //    dtStart = new DateTime(DateTime.Now.Year, data_user.date_of_joining.Value.Month, data_user.date_of_joining.Value.Day);
                            //    dtEnd = new DateTime((DateTime.Now.Year + 1), 6, 30);
                            //}

                            DLL.Models.LeaveSession lsession = new DLL.Models.LeaveSession()
                            {
                                EmployeeId = data_user.EmployeeId,
                                YearId = dtEnd.Year,
                                SessionStartDate = dtStart,
                                SessionEndDate = dtEnd,
                                SickLeaves = data_user.sick_leaves,
                                CasualLeaves = data_user.casual_leaves,
                                AnnualLeaves = data_user.annual_leaves,
                                OtherLeaves = data_user.other_leaves,
                                LeaveType01 = data_user.leave_type01,
                                LeaveType02 = data_user.leave_type02,
                                LeaveType03 = data_user.leave_type03,
                                LeaveType04 = data_user.leave_type04,
                                LeaveType05 = data_user.leave_type05,
                                LeaveType06 = data_user.leave_type06,
                                LeaveType07 = data_user.leave_type07,
                                LeaveType08 = data_user.leave_type08,
                                LeaveType09 = data_user.leave_type09,
                                LeaveType10 = data_user.leave_type10,
                                LeaveType11 = data_user.leave_type11
                            };

                            db.leave_session.Add(lsession);
                            db.SaveChanges();
                        }
                    }

                    //when new session started and manage carry forward leaves as well
                    DateTime dtToday = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd") + " 00:00:00.000");
                    var data_leave_session_next = db.leave_session.Where(l => l.EmployeeId == data_user.EmployeeId && l.SessionEndDate >= dtToday).OrderByDescending(o => o.id).FirstOrDefault();
                    if (data_leave_session_next == null)
                    {
                        int sick_leaves = data_user.sick_leaves;
                        int casual_leaves = data_user.casual_leaves;
                        int annual_leaves = data_user.annual_leaves;
                        int other_leaves = data_user.other_leaves;
                        int leave_type01 = data_user.leave_type01;
                        int leave_type02 = data_user.leave_type02;
                        int leave_type03 = data_user.leave_type03;
                        int leave_type04 = data_user.leave_type04;
                        int leave_type05 = data_user.leave_type05;
                        int leave_type06 = data_user.leave_type06;
                        int leave_type07 = data_user.leave_type07;
                        int leave_type08 = data_user.leave_type08;
                        int leave_type09 = data_user.leave_type09;
                        int leave_type10 = data_user.leave_type10;
                        int leave_type11 = data_user.leave_type11;

                        ///////////////// Carry Forward - Last Session remaining Leaves //////////////////////

                        DateTime[] dt = new DateTime[2] { DateTime.Now, DateTime.Now };

                        int[] leaves = new int[30] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                        int last_sick_leaves = 0, last_casual_leaves = 0, last_annual_leaves = 0, last_other_leaves = 0;
                        int last_leave_type01 = 0, last_leave_type02 = 0, last_leave_type03 = 0, last_leave_type04 = 0, last_leave_type05 = 0, last_leave_type06 = 0, last_leave_type07 = 0, last_leave_type08 = 0, last_leave_type09 = 0, last_leave_type10 = 0, last_leave_type11 = 0;
                        int iAvailableSickLeaves = 0, iAvailableCasualLeaves = 0, iAvailableAnnualLeaves = 0, iAvailableOtherLeaves = 0;
                        int iAvailableLeaveType01 = 0, iAvailableLeaveType02 = 0, iAvailableLeaveType03 = 0, iAvailableLeaveType04 = 0, iAvailableLeaveType05 = 0, iAvailableLeaveType06 = 0, iAvailableLeaveType07 = 0, iAvailableLeaveType08 = 0, iAvailableLeaveType09 = 0, iAvailableLeaveType10 = 0, iAvailableLeaveType11 = 0;
                        int iAvailedSickLeaves = 0, iAvailedCasualLeaves = 0, iAvailedAnnualLeaves = 0, iAvailedOtherLeaves = 0;
                        int iAvailedLeaveType01 = 0, iAvailedLeaveType02 = 0, iAvailedLeaveType03 = 0, iAvailedLeaveType04 = 0, iAvailedLeaveType05 = 0, iAvailedLeaveType06 = 0, iAvailedLeaveType07 = 0, iAvailedLeaveType08 = 0, iAvailedLeaveType09 = 0, iAvailedLeaveType10 = 0, iAvailedLeaveType11 = 0;

                        leaves = ViewModels.LeaveApplicationResultSet.getUserLeavesByUserCode(User_Code);
                        iAvailableSickLeaves = leaves[0];
                        iAvailableCasualLeaves = leaves[1];
                        iAvailableAnnualLeaves = leaves[2];
                        iAvailableOtherLeaves = leaves[6];
                        iAvailableLeaveType01 = leaves[8];
                        iAvailableLeaveType02 = leaves[9];
                        iAvailableLeaveType03 = leaves[10];
                        iAvailableLeaveType04 = leaves[11];
                        iAvailableLeaveType05 = leaves[16];
                        iAvailableLeaveType06 = leaves[17];
                        iAvailableLeaveType07 = leaves[18];
                        iAvailableLeaveType08 = leaves[19];
                        iAvailableLeaveType09 = leaves[20];
                        iAvailableLeaveType10 = leaves[21];
                        iAvailableLeaveType11 = leaves[22];

                        iAvailedSickLeaves = leaves[3];
                        iAvailedCasualLeaves = leaves[4];
                        iAvailedAnnualLeaves = leaves[5];
                        iAvailedOtherLeaves = leaves[7];
                        iAvailedLeaveType01 = leaves[12];
                        iAvailedLeaveType02 = leaves[13];
                        iAvailedLeaveType03 = leaves[14];
                        iAvailedLeaveType04 = leaves[15];
                        iAvailedLeaveType05 = leaves[23];
                        iAvailedLeaveType06 = leaves[24];
                        iAvailedLeaveType07 = leaves[25];
                        iAvailedLeaveType08 = leaves[26];
                        iAvailedLeaveType09 = leaves[27];
                        iAvailedLeaveType10 = leaves[28];
                        iAvailedLeaveType11 = leaves[29];

                        last_sick_leaves = iAvailableSickLeaves - iAvailedSickLeaves;
                        last_casual_leaves = iAvailableCasualLeaves - iAvailedCasualLeaves;
                        last_annual_leaves = iAvailableAnnualLeaves - iAvailedAnnualLeaves;
                        last_other_leaves = iAvailableOtherLeaves - iAvailedOtherLeaves;
                        last_leave_type01 = iAvailableLeaveType01 - iAvailedLeaveType01;
                        last_leave_type02 = iAvailableLeaveType02 - iAvailedLeaveType02;
                        last_leave_type03 = iAvailableLeaveType03 - iAvailedLeaveType03;
                        last_leave_type04 = iAvailableLeaveType04 - iAvailedLeaveType04;
                        last_leave_type05 = iAvailableLeaveType05 - iAvailedLeaveType05;
                        last_leave_type06 = iAvailableLeaveType06 - iAvailedLeaveType06;
                        last_leave_type07 = iAvailableLeaveType07 - iAvailedLeaveType07;
                        last_leave_type08 = iAvailableLeaveType08 - iAvailedLeaveType08;
                        last_leave_type09 = iAvailableLeaveType01 - iAvailedLeaveType01;
                        last_leave_type10 = iAvailableLeaveType10 - iAvailedLeaveType10;
                        last_leave_type11 = iAvailableLeaveType11 - iAvailedLeaveType11;

                        if (last_sick_leaves > 0)
                            sick_leaves = sick_leaves + last_sick_leaves;

                        if (last_casual_leaves > 0)
                            casual_leaves = casual_leaves + last_casual_leaves;

                        if (last_annual_leaves > 0)
                            annual_leaves = annual_leaves + last_annual_leaves;

                        if (last_other_leaves > 0)
                            other_leaves = other_leaves + 0; // last_other_leaves;

                        if (last_leave_type01 > 0)
                            leave_type01 = leave_type01 + 0;

                        if (last_leave_type02 > 0)
                            leave_type02 = leave_type02 + 0;

                        if (last_leave_type03 > 0)
                            leave_type03 = leave_type03 + 0;

                        if (last_leave_type04 > 0)
                            leave_type04 = leave_type04 + 0;

                        if (last_leave_type05 > 0)
                            leave_type05 = leave_type05 + 0;

                        if (last_leave_type06 > 0)
                            leave_type06 = leave_type06 + 0;

                        if (last_leave_type07 > 0)
                            leave_type07 = leave_type07 + 0;

                        if (last_leave_type08 > 0)
                            leave_type08 = leave_type08 + 0;

                        if (last_leave_type09 > 0)
                            leave_type09 = leave_type09 + 0;

                        if (last_leave_type10 > 0)
                            leave_type10 = leave_type10 + 0;

                        if (last_leave_type11 > 0)
                            leave_type11 = leave_type11 + 0;

                        //////////////////////////////////////////////////////////////////////////////////////

                        if (data_leave_session != null)
                        {
                            DateTime dtSessionEndDate = data_leave_session.SessionEndDate == null ? DateTime.Now : data_leave_session.SessionEndDate;

                            DLL.Models.LeaveSession lsession_next = new DLL.Models.LeaveSession()
                            {
                                EmployeeId = data_user.EmployeeId,
                                YearId = dtSessionEndDate != null ? Convert.ToDateTime(dtSessionEndDate).AddDays(-1).AddYears(1).Year : DateTime.Now.AddDays(-1).AddYears(1).Year,
                                SessionStartDate = dtSessionEndDate != null ? Convert.ToDateTime(dtSessionEndDate).AddDays(1) : DateTime.Now.AddDays(1),
                                SessionEndDate = dtSessionEndDate != null ? Convert.ToDateTime(dtSessionEndDate).AddYears(1) : DateTime.Now.AddYears(1),
                                SickLeaves = sick_leaves,
                                CasualLeaves = casual_leaves,
                                AnnualLeaves = annual_leaves,
                                OtherLeaves = other_leaves,
                                LeaveType01 = leave_type01,
                                LeaveType02 = leave_type02,
                                LeaveType03 = leave_type03,
                                LeaveType04 = leave_type04,
                                LeaveType05 = leave_type05,
                                LeaveType06 = leave_type06,
                                LeaveType07 = leave_type07,
                                LeaveType08 = leave_type08,
                                LeaveType09 = leave_type09,
                                LeaveType10 = leave_type10,
                                LeaveType11 = leave_type11
                            };

                            db.leave_session.Add(lsession_next);
                            db.SaveChanges();
                        }
                    }
                }
            }
        }

        public static void manageLeaveSessionForUsers_BACKUP(string User_Code) //actually Staff
        {
            using (Context db = new Context())
            {
                var data_user = db.employee.Where(e => e.active && e.employee_code == User_Code).FirstOrDefault();
                if (data_user != null)
                {
                    //if NO entry found in LeaveSession table for this user
                    var data_leave_session = db.leave_session.Where(l => l.EmployeeId == data_user.EmployeeId).OrderByDescending(o => o.id).FirstOrDefault();// l.SessionEndDate >= DateTime.Now
                    if (data_leave_session == null)
                    {
                        DateTime dtStart = new DateTime(); DateTime dtEnd = new DateTime();

                        if (data_user.date_of_joining != null)
                        {
                            data_user.date_of_joining = Convert.ToDateTime(DateTime.Now.Year + "-" + data_user.date_of_joining.Value.Month + "-" + data_user.date_of_joining.Value.Day);

                            if (data_user.date_of_joining == new DateTime(DateTime.Now.Year, 6, 30) || data_user.date_of_joining == new DateTime(DateTime.Now.Year, 7, 1) || data_user.date_of_joining == new DateTime(DateTime.Now.Year, 7, 2))
                            {
                                dtStart = new DateTime(DateTime.Now.Year, 7, 1);
                                dtEnd = new DateTime((DateTime.Now.Year + 1), 6, 30);
                            }
                            else if (data_user.date_of_joining < new DateTime(DateTime.Now.Year, 6, 30)) //like month less than June month - Feb 
                            {
                                dtStart = Convert.ToDateTime(DateTime.Now.Year + "-" + data_user.date_of_joining.Value.Month + "-" + data_user.date_of_joining.Value.Day);
                                dtEnd = new DateTime(DateTime.Now.Year, 6, 30);
                            }
                            else if (data_user.date_of_joining > new DateTime(DateTime.Now.Year, 6, 30)) //like month greater than June month - Aug 
                            {
                                dtStart = Convert.ToDateTime((DateTime.Now.Year - 1) + "-" + data_user.date_of_joining.Value.Month + "-" + data_user.date_of_joining.Value.Day);
                                dtEnd = new DateTime(DateTime.Now.Year, 6, 30);
                            }

                            DLL.Models.LeaveSession lsession = new DLL.Models.LeaveSession()
                            {
                                EmployeeId = data_user.EmployeeId,
                                YearId = dtEnd.Year,
                                SessionStartDate = dtStart,
                                SessionEndDate = dtEnd,
                                SickLeaves = data_user.sick_leaves,
                                CasualLeaves = data_user.casual_leaves,
                                AnnualLeaves = data_user.annual_leaves
                            };

                            db.leave_session.Add(lsession);
                            db.SaveChanges();
                        }

                    }

                    //when new session started and manage carry forward leaves as well
                    DateTime dtToday = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd") + " 00:00:00.000");
                    var data_leave_session_next = db.leave_session.Where(l => l.EmployeeId == data_user.EmployeeId && l.SessionEndDate >= dtToday).OrderByDescending(o => o.id).FirstOrDefault();
                    if (data_leave_session_next == null)
                    {
                        int sick_leaves = data_user.sick_leaves;
                        int casual_leaves = data_user.casual_leaves;
                        int annual_leaves = data_user.annual_leaves;

                        ///////////////// Carry Forward - Last Session remaining Leaves //////////////////////

                        DateTime[] dt = new DateTime[2] { DateTime.Now, DateTime.Now };

                        int[] leaves = new int[6] { 0, 0, 0, 0, 0, 0 };
                        int last_sick_leaves = 0, last_casual_leaves = 0, last_annual_leaves = 0;
                        int iAvailableSickLeaves = 0, iAvailableCasualLeaves = 0, iAvailableAnnualLeaves = 0;
                        int iAvailedSickLeaves = 0, iAvailedCasualLeaves = 0, iAvailedAnnualLeaves = 0;

                        leaves = LeaveApplicationResultSet.getUserLeavesByUserCode(User_Code);
                        iAvailableSickLeaves = leaves[0];
                        iAvailableCasualLeaves = leaves[1];
                        iAvailableAnnualLeaves = leaves[2];
                        iAvailedSickLeaves = leaves[3];
                        iAvailedCasualLeaves = leaves[4];
                        iAvailedAnnualLeaves = leaves[5];

                        last_sick_leaves = iAvailableSickLeaves - iAvailedSickLeaves;
                        last_casual_leaves = iAvailableCasualLeaves - iAvailedCasualLeaves;
                        last_annual_leaves = iAvailableAnnualLeaves - iAvailedAnnualLeaves;

                        if (last_sick_leaves > 0)
                            sick_leaves = sick_leaves + last_sick_leaves;

                        if (last_casual_leaves > 0)
                            casual_leaves = casual_leaves + last_casual_leaves;

                        if (last_annual_leaves > 0)
                            annual_leaves = annual_leaves + last_annual_leaves;

                        //////////////////////////////////////////////////////////////////////////////////////

                        DLL.Models.LeaveSession lsession_next = new DLL.Models.LeaveSession()
                        {
                            EmployeeId = data_user.EmployeeId,
                            YearId = data_leave_session.SessionEndDate != null ? Convert.ToDateTime(data_leave_session.SessionEndDate).AddDays(-1).AddYears(1).Year : DateTime.Now.AddDays(-1).AddYears(1).Year,
                            SessionStartDate = data_leave_session.SessionEndDate != null ? Convert.ToDateTime(data_leave_session.SessionEndDate).AddDays(1) : DateTime.Now.AddDays(1),
                            SessionEndDate = data_leave_session.SessionEndDate != null ? Convert.ToDateTime(data_leave_session.SessionEndDate).AddYears(1) : DateTime.Now.AddYears(1),
                            SickLeaves = sick_leaves,
                            CasualLeaves = casual_leaves,
                            AnnualLeaves = annual_leaves
                        };

                        db.leave_session.Add(lsession_next);
                        db.SaveChanges();
                    }
                }
            }
        }
    }

}

