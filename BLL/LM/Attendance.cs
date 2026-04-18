using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewModels;
using DLL.Models;
using System.Globalization;
using System.Web.UI.WebControls;

namespace TimeTune
{
    public class AttendanceLM
    {
        public static List<ViewModels.ConsolidatedAttendanceLog> getAllConsolidateAttendanceBySupervisor(string search, string employee_code, string from, string to, string supervisorCode)
        {

            /************ PARSING MONTH AND EMPLOYEE CODE *******************************************/




            DateTime? firstDay = null;
            DateTime? lastDay = null;


            if (from != null && to != null && !from.Equals("") && !to.Equals(""))
            {

                try
                {
                    firstDay = DateTime.ParseExact(from, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                    lastDay = DateTime.ParseExact(to, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                }
                catch
                {

                }
                /*string[] components = "saf".Split('-');

                int year;
                int mnth;

                if (components.Length == 2)
                {
                    if (int.TryParse(components[0], out year) && int.TryParse(components[1],out mnth))
                    {
                        firstDayOfMonth = new DateTime(year, mnth, 1);
                        lastDayOfMonth = firstDayOfMonth.Value.AddMonths(1).AddDays(-1);
                    }
                }*/

            }
            else
            {
                firstDay = DateTime.ParseExact(DateTime.Now.ToString("dd-MM-yyyy"), "dd-MM-yyyy", CultureInfo.InvariantCulture);
                lastDay = DateTime.ParseExact(DateTime.Now.AddDays(1).ToString("dd-MM-yyyy"), "dd-MM-yyyy", CultureInfo.InvariantCulture);
            }
            /***************************************************************************/


            using (Context db = new Context())
            {

                //string employee_id = db.;
                //int emp_id;
                //if (!int.TryParse(employee_id, out emp_id))
                //{
                //    emp_id = -1;
                //}


                List<ConsolidatedAttendance> consolidateAttendance = null;
                try
                {

                    DateTime? times;
                    string sdd = "";
                    if (search != null && search.Split(':').Count() == 2 && search.Length == 8)
                    {
                        times = DateTime.Parse(search);
                        var pp = times.Value.TimeOfDay.ToString();
                        sdd = pp.Remove(pp.Length - 3);
                        //sdd = pp;
                    }
                    var supervisor = db.employee.Where(m => m.employee_code.Equals(supervisorCode)).FirstOrDefault().Group;
                    if (supervisor == null)
                    {
                        return null;
                    }

                    int groupId = supervisor.GroupId;

                    consolidateAttendance = db.consolidated_attendance.Where(m =>
                        m.active &&
                        m.employee.Group.GroupId.Equals(groupId)

                        &&


                        (string.IsNullOrEmpty(employee_code) ||
                        m.employee.employee_code.Equals(employee_code))
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
                        m.terminal_in.ToLower().Contains(search.ToLower()) ||
                        System.Data.Entity.DbFunctions.TruncateTime(m.date).ToString().Contains(search.ToLower()) ||
                        System.Data.Entity.DbFunctions.CreateTime(m.time_in.Value.Hour, m.time_in.Value.Minute, m.time_in.Value.Second).ToString().Contains(sdd.ToLower()) ||
                        System.Data.Entity.DbFunctions.CreateTime(m.time_out.Value.Hour, m.time_out.Value.Minute, m.time_out.Value.Second).ToString().Contains(sdd.ToLower()) ||
                        m.terminal_out.ToLower().Contains(search.ToLower())
                        )

                        &&
                        (!firstDay.HasValue ||
                        (m.date.Value >= firstDay.Value && m.date.Value <= lastDay))

                        ).ToList();

                }
                catch (System.Data.Entity.Core.EntityCommandExecutionException ex)
                {
                    consolidateAttendance = new List<ConsolidatedAttendance>();
                }



                List<ViewModels.ConsolidatedAttendanceLog> toReturn = new List<ViewModels.ConsolidatedAttendanceLog>();

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

                    vmConsilidateAttendance.time_out = (dbConsolidateAttendance.time_out.HasValue) ? dbConsolidateAttendance.time_out.Value.ToString("hh:mm tt") : "";
                    //vmConsilidateAttendance.time_out = (dbConsolidateAttendance.time_out != null && dbConsolidateAttendance.time_out.HasValue)
                    // ? dbConsolidateAttendance.time_out.Value.TimeOfDay.ToString("c") : "";
                    vmConsilidateAttendance.status_out = dbConsolidateAttendance.status_out;
                    vmConsilidateAttendance.terminal_in = (dbConsolidateAttendance.terminal_in != null) ? dbConsolidateAttendance.terminal_in : " ";
                    vmConsilidateAttendance.terminal_out = (dbConsolidateAttendance.terminal_out != null) ? dbConsolidateAttendance.terminal_out : " ";
                    vmConsilidateAttendance.final_remarks = (dbConsolidateAttendance.manualAttendances.Count > 0) ?
                        dbConsolidateAttendance.final_remarks + "*" : dbConsolidateAttendance.final_remarks;

                    vmConsilidateAttendance.overtime = strOvertime;
                    vmConsilidateAttendance.overtime_status = strOvertimeStatus;


                    vmConsilidateAttendance.action =
                           @"<div data-id='" + dbConsolidateAttendance.ConsolidatedAttendanceId + @"'>
                                <a href='javascript:void(editStatus(" + dbConsolidateAttendance.ConsolidatedAttendanceId + "," + dbConsolidateAttendance.overtime_status + @"));'>Edit</a>
                            </div>";



                    /*
                    // check if there are manual attendances for this log.
                    if (dbConsolidateAttendance.manualAttendances.Count > 0)
                    {

                        int latestManualAttendanceID =
                            dbConsolidateAttendance.manualAttendances.Max(m => m.ManualAttendanceId);

                        DLL.Models.ManualAttendance manualAttendance =
                            dbConsolidateAttendance.manualAttendances.Where(m => m.ManualAttendanceId == latestManualAttendanceID).FirstOrDefault();


                        vmConsilidateAttendance.final_remarks = manualAttendance.remarks;
                        for (int ind = 0; ind < dbConsolidateAttendance.manualAttendances.Count; ind++)
                        {
                            vmConsilidateAttendance.final_remarks += '*';
                        }




                    }
                    else
                    {
                        vmConsilidateAttendance.final_remarks = dbConsolidateAttendance.final_remarks;
                    }*/






                    toReturn.Add(vmConsilidateAttendance);
                }

                //toReturn = toReturn.OrderBy(o => o.dt_date).ToList();

                return toReturn;
            }
        }

        public static List<ViewModels.ConsolidatedAttendanceLog> getAllConsolidateAttendanceLMBySupervisor(string employee_code, string from, string to, string supervisorCode, string search, string sortOrder, int start, int length)
        {
            if (sortOrder == null)
            {
                sortOrder = "date desc";
            }

            if (sortOrder.Contains("action"))
            {
                sortOrder = "date desc";
            }

            if (sortOrder.ToLower() == "date desc")
            {
                sortOrder = "date desc";
            }

            if (sortOrder.Contains("str_overtime"))
            {
                sortOrder = sortOrder.Replace("str_overtime", "overtime");
            }


            if (sortOrder.Contains("overtime_status") || sortOrder.Contains("overtime"))
            {
                sortOrder = "date desc";
            }

            /************ PARSING MONTH AND EMPLOYEE CODE *******************************************/

            DateTime? firstDay = null;
            DateTime? lastDay = null;


            if (from != null && to != null && !from.Equals("") && !to.Equals(""))
            {

                try
                {
                    firstDay = DateTime.ParseExact(from, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                    lastDay = DateTime.ParseExact(to, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                }
                catch
                {

                }
                /*string[] components = "saf".Split('-');

                int year;
                int mnth;

                if (components.Length == 2)
                {
                    if (int.TryParse(components[0], out year) && int.TryParse(components[1],out mnth))
                    {
                        firstDayOfMonth = new DateTime(year, mnth, 1);
                        lastDayOfMonth = firstDayOfMonth.Value.AddMonths(1).AddDays(-1);
                    }
                }*/

            }
            else
            {
                firstDay = DateTime.ParseExact(DateTime.Now.ToString("dd-MM-yyyy"), "dd-MM-yyyy", CultureInfo.InvariantCulture);
                lastDay = DateTime.ParseExact(DateTime.Now.AddDays(1).ToString("dd-MM-yyyy"), "dd-MM-yyyy", CultureInfo.InvariantCulture);
            }
            /***************************************************************************/


            using (Context db = new Context())
            {
                string camname = ""; string newfinalremarks = "";
                List<LeaveApplication> lApplication = new List<LeaveApplication>();
                var campusempp = db.employee.Where(m => m.employee_code.Equals(supervisorCode)).FirstOrDefault();
                lApplication = db.leave_application.Where(l => l.IsActive && l.ApproverId == campusempp.EmployeeId && l.FromDate >= firstDay && l.ToDate <= lastDay).ToList();

                //string employee_id = db.;
                //int emp_id;
                //if (!int.TryParse(employee_id, out emp_id))
                //{
                //    emp_id = -1;
                //}


                List<ConsolidatedAttendance> consolidateAttendance = null;
                try
                {

                    DateTime? times;
                    string sdd = "";
                    if (search != null && search.Split(':').Count() == 2 && search.Length == 8)
                    {
                        times = DateTime.Parse(search);
                        var pp = times.Value.TimeOfDay.ToString();
                        sdd = pp.Remove(pp.Length - 3);
                        //sdd = pp;
                    }
                    var supervisor = db.employee.Where(m => m.employee_code.Equals(supervisorCode)).FirstOrDefault().Group;
                    if (supervisor == null)
                    {
                        return null;
                    }

                    int groupId = supervisor.GroupId;

                    consolidateAttendance = db.consolidated_attendance.Where(m =>
                        m.active &&
                        m.employee.Group.GroupId.Equals(groupId)

                        &&


                        (string.IsNullOrEmpty(employee_code) ||
                        m.employee.employee_code.Equals(employee_code))
                        &&
                        (
                        search == null ||
                        search.Equals("") ||
                        m.employee.employee_code.Contains(search) ||
                         m.employee.first_name.ToLower().Contains(search.ToLower()) ||
                        m.employee.last_name.ToLower().Contains(search.ToLower()) ||
                        m.employee.department.name.ToLower().Contains(search.ToLower()) ||
                        m.employee.designation.name.ToLower().Contains(search.ToLower()) ||
                        m.campusname.Contains(search.ToLower()) ||
                        // m.employee.campus_id.ToString().Contains(search.ToLower()) ||
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

                        &&
                        (!firstDay.HasValue ||
                        (m.date.Value >= firstDay.Value && m.date.Value <= lastDay))

                        ).SortBy(sortOrder).Skip(start).Take(length).ToList();

                }
                catch (System.Data.Entity.Core.EntityCommandExecutionException ex)
                {
                    consolidateAttendance = new List<ConsolidatedAttendance>();
                }



                List<ViewModels.ConsolidatedAttendanceLog> toReturn = new List<ViewModels.ConsolidatedAttendanceLog>();

                for (int i = 0; i < consolidateAttendance.Count(); i++)
                {
                    for (int j = 0; j < lApplication.Count(); j++)
                    {
                        if (consolidateAttendance[i].employee.EmployeeId == lApplication[j].EmployeeId)
                        {
                            if (consolidateAttendance[i].date >= lApplication[j].FromDate && consolidateAttendance[i].date <= lApplication[j].ToDate)
                            {
                                if (lApplication[j].LeaveStatusId == 2 && lApplication[j].LeaveTypeId == 1) { consolidateAttendance[i].final_remarks = "MeL"; }
                                else if (lApplication[j].LeaveStatusId == 2 && lApplication[j].LeaveTypeId == 2) { consolidateAttendance[i].final_remarks = "CL"; }
                                else if (lApplication[j].LeaveStatusId == 2 && lApplication[j].LeaveTypeId == 3) { consolidateAttendance[i].final_remarks = "EL"; }
                                else if (lApplication[j].LeaveStatusId == 2 && lApplication[j].LeaveTypeId == 4) { consolidateAttendance[i].final_remarks = "E.PL"; }
                                else if (lApplication[j].LeaveStatusId == 2 && lApplication[j].LeaveTypeId == 5) { consolidateAttendance[i].final_remarks = "MaL"; }
                                else if (lApplication[j].LeaveStatusId == 2 && lApplication[j].LeaveTypeId == 6) { consolidateAttendance[i].final_remarks = "TL"; }
                                else if (lApplication[j].LeaveStatusId == 2 && lApplication[j].LeaveTypeId == 7) { consolidateAttendance[i].final_remarks = "VL"; }
                                else if (lApplication[j].LeaveStatusId == 2 && lApplication[j].LeaveTypeId == 8) { consolidateAttendance[i].final_remarks = "MeetL"; }
                                else if (lApplication[j].LeaveStatusId == 2 && lApplication[j].LeaveTypeId == 9) { consolidateAttendance[i].final_remarks = "HL"; }
                                else if (lApplication[j].LeaveStatusId == 2 && lApplication[j].LeaveTypeId == 17) { consolidateAttendance[i].final_remarks = "PO*"; }
                                else if (lApplication[j].LeaveStatusId == 2 && lApplication[j].LeaveTypeId == 18) { consolidateAttendance[i].final_remarks = "PLO*"; }
                                else if (lApplication[j].LeaveStatusId == 2 && lApplication[j].LeaveTypeId == 19) { consolidateAttendance[i].final_remarks = "PLE*"; }
                                else if (lApplication[j].LeaveStatusId == 2 && lApplication[j].LeaveTypeId == 20) { consolidateAttendance[i].final_remarks = "POE*"; }
                                else if (lApplication[j].LeaveStatusId == 2 && lApplication[j].LeaveTypeId == 21) { consolidateAttendance[i].final_remarks = "OFF*"; }
                                else if (lApplication[j].LeaveStatusId == 2 && lApplication[j].LeaveTypeId == 22) { consolidateAttendance[i].final_remarks = "OV*"; }
                                else if (lApplication[j].LeaveStatusId == 2 && lApplication[j].LeaveTypeId == 23) { consolidateAttendance[i].final_remarks = "OM*"; }
                                else if (lApplication[j].LeaveStatusId == 2 && lApplication[j].LeaveTypeId == 24) { consolidateAttendance[i].final_remarks = "OT*"; }
                            }
                        }
                    }
                    string strOvertime = "", strOvertimeStatus = "";

                    TimeSpan timeOver = TimeSpan.FromSeconds(consolidateAttendance[i].overtime);
                    if (consolidateAttendance[i].employee.employee_code != "000000")
                    {
                        string ecode = consolidateAttendance[i].employee.employee_code;
                        var campusemp = db.employee.Where(m => m.employee_code == ecode).FirstOrDefault();
                        int campusID = campusemp.campus_id;
                        var orgcampus = db.organization_campus.Where(o => o.Id.Equals(campusID)).FirstOrDefault();
                        camname = orgcampus.CampusCode;
                    }
                    consolidateAttendance[i].campusname = camname;
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
                    //dbConsolidateAttendance.campusname = camname;
                    vmConsilidateAttendance.id = dbConsolidateAttendance.ConsolidatedAttendanceId;
                    vmConsilidateAttendance.date = (dbConsolidateAttendance.date != null && dbConsolidateAttendance.date.HasValue)
                         ? dbConsolidateAttendance.date.Value.ToString("dd-MM-yyyy") : "";
                    vmConsilidateAttendance.dt_date = (dbConsolidateAttendance.date != null && dbConsolidateAttendance.date.HasValue)
                        ? dbConsolidateAttendance.date.Value : DateTime.Now;
                    vmConsilidateAttendance.employee_code = dbConsolidateAttendance.employee.employee_code;
                    vmConsilidateAttendance.employee_first_name = dbConsolidateAttendance.employee.first_name;
                    vmConsilidateAttendance.employee_last_name = dbConsolidateAttendance.employee.last_name;
                    vmConsilidateAttendance.employee_department_name = dbConsolidateAttendance.employee.department.name;
                    vmConsilidateAttendance.employee_designation_name = dbConsolidateAttendance.employee.designation.name;
                    //vmConsilidateAttendance.employee_campus_name = dbConsolidateAttendance.employee.campus_id.ToString();
                    //vmConsilidateAttendance.employee_campus_name = dbConsolidateAttendance.orgcampus.Id.Equals(dbConsolidateAttendance.employee.campus_id) ? dbConsolidateAttendance.orgcampus.CampusCode:"";
                    vmConsilidateAttendance.employee_campus_name = dbConsolidateAttendance.campusname;
                    vmConsilidateAttendance.time_in = (dbConsolidateAttendance.time_in.HasValue) ? dbConsolidateAttendance.time_in.Value.ToString("hh:mm tt") : "";
                    //vmConsilidateAttendance.time_in = (dbConsolidateAttendance.time_in != null && dbConsolidateAttendance.time_in.HasValue)
                    //     ? dbConsolidateAttendance.time_in.Value.TimeOfDay.ToString("c") : "";
                    vmConsilidateAttendance.status_in = dbConsolidateAttendance.status_in;
                    //vmConsilidateAttendance.employee_campus_name = "";
                    vmConsilidateAttendance.time_out = (dbConsolidateAttendance.time_out.HasValue) ? dbConsolidateAttendance.time_out.Value.ToString("hh:mm tt") : "";
                    //vmConsilidateAttendance.time_out = (dbConsolidateAttendance.time_out != null && dbConsolidateAttendance.time_out.HasValue)
                    // ? dbConsolidateAttendance.time_out.Value.TimeOfDay.ToString("c") : "";
                    vmConsilidateAttendance.status_out = dbConsolidateAttendance.status_out;
                    vmConsilidateAttendance.terminal_in = (dbConsolidateAttendance.terminal_in != null) ? dbConsolidateAttendance.terminal_in : " ";
                    vmConsilidateAttendance.terminal_out = (dbConsolidateAttendance.terminal_out != null) ? dbConsolidateAttendance.terminal_out : " ";
                    vmConsilidateAttendance.final_remarks = (dbConsolidateAttendance.manualAttendances.Count > 0) ?
                        dbConsolidateAttendance.final_remarks + "*" : dbConsolidateAttendance.final_remarks;
                    vmConsilidateAttendance.description = dbConsolidateAttendance.description;
                    vmConsilidateAttendance.overtime = strOvertime;
                    vmConsilidateAttendance.overtime_status = strOvertimeStatus;


                    vmConsilidateAttendance.action =
                           @"<div data-id='" + dbConsolidateAttendance.ConsolidatedAttendanceId + @"'>
                                <a href='javascript:void(editStatus(" + dbConsolidateAttendance.ConsolidatedAttendanceId + "," + dbConsolidateAttendance.overtime_status + @"));'>Edit</a>
                            </div>";



                    /*
                    // check if there are manual attendances for this log.
                    if (dbConsolidateAttendance.manualAttendances.Count > 0)
                    {

                        int latestManualAttendanceID =
                            dbConsolidateAttendance.manualAttendances.Max(m => m.ManualAttendanceId);

                        DLL.Models.ManualAttendance manualAttendance =
                            dbConsolidateAttendance.manualAttendances.Where(m => m.ManualAttendanceId == latestManualAttendanceID).FirstOrDefault();


                        vmConsilidateAttendance.final_remarks = manualAttendance.remarks;
                        for (int ind = 0; ind < dbConsolidateAttendance.manualAttendances.Count; ind++)
                        {
                            vmConsilidateAttendance.final_remarks += '*';
                        }




                    }
                    else
                    {
                        vmConsilidateAttendance.final_remarks = dbConsolidateAttendance.final_remarks;
                    }*/






                    toReturn.Add(vmConsilidateAttendance);
                }

                //toReturn = toReturn.OrderBy(o => o.dt_date).ToList();

                return toReturn;
            }
        }


        public static List<ViewModels.ConsolidatedAttendanceExport> getAllConsolidateAttendanceBySupervisorExport(string search, string employee_code, string from, string to, string supervisorCode)
        {

            /************ PARSING MONTH AND EMPLOYEE CODE *******************************************/




            DateTime? firstDay = null;
            DateTime? lastDay = null;


            if (from != null && to != null && !from.Equals("") && !to.Equals(""))
            {

                try
                {
                    firstDay = DateTime.ParseExact(from, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                    lastDay = DateTime.ParseExact(to, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                }
                catch
                {

                }
                /*string[] components = "saf".Split('-');

                int year;
                int mnth;

                if (components.Length == 2)
                {
                    if (int.TryParse(components[0], out year) && int.TryParse(components[1],out mnth))
                    {
                        firstDayOfMonth = new DateTime(year, mnth, 1);
                        lastDayOfMonth = firstDayOfMonth.Value.AddMonths(1).AddDays(-1);
                    }
                }*/

            }
            else
            {
                firstDay = DateTime.ParseExact(DateTime.Now.ToString("dd-MM-yyyy"), "dd-MM-yyyy", CultureInfo.InvariantCulture);
                lastDay = DateTime.ParseExact(DateTime.Now.AddDays(1).ToString("dd-MM-yyyy"), "dd-MM-yyyy", CultureInfo.InvariantCulture);
            }
            /***************************************************************************/


            using (Context db = new Context())
            {

                //string employee_id = db.;
                //int emp_id;
                //if (!int.TryParse(employee_id, out emp_id))
                //{
                //    emp_id = -1;
                //}


                List<ConsolidatedAttendance> consolidateAttendance = null;
                try
                {

                    DateTime? times;
                    string sdd = "";
                    if (search != null && search.Split(':').Count() == 2 && search.Length == 8)
                    {
                        times = DateTime.Parse(search);
                        var pp = times.Value.TimeOfDay.ToString();
                        sdd = pp.Remove(pp.Length - 3);
                        //sdd = pp;
                    }
                    var supervisor = db.employee.Where(m => m.employee_code.Equals(supervisorCode)).FirstOrDefault().Group;
                    if (supervisor == null)
                    {
                        return null;
                    }

                    int groupId = supervisor.GroupId;

                    consolidateAttendance = db.consolidated_attendance.Where(m =>
                        m.active && m.employee.timetune_active &&
                        m.employee.Group.GroupId.Equals(groupId)

                        &&


                        (string.IsNullOrEmpty(employee_code) ||
                        m.employee.employee_code.Equals(employee_code))
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

                        &&
                        (!firstDay.HasValue ||
                        (m.date.Value >= firstDay.Value && m.date.Value <= lastDay))

                        ).ToList();

                }
                catch (System.Data.Entity.Core.EntityCommandExecutionException ex)
                {
                    consolidateAttendance = new List<ConsolidatedAttendance>();
                }



                List<ViewModels.ConsolidatedAttendanceExport> toReturn = new List<ViewModels.ConsolidatedAttendanceExport>();

                for (int i = 0; i < consolidateAttendance.Count(); i++)
                {
                    ViewModels.ConsolidatedAttendanceExport vmConsilidateAttendance = new ViewModels.ConsolidatedAttendanceExport();
                    DLL.Models.ConsolidatedAttendance dbConsolidateAttendance = new DLL.Models.ConsolidatedAttendance();
                    //Force Load
                    dbConsolidateAttendance = consolidateAttendance[i];

                    ////vmConsilidateAttendance.id = dbConsolidateAttendance.ConsolidatedAttendanceId;
                    vmConsilidateAttendance.Date = (dbConsolidateAttendance.date != null && dbConsolidateAttendance.date.HasValue)
                         ? dbConsolidateAttendance.date.Value.ToString("dd-MM-yyyy") : "";
                    vmConsilidateAttendance.Employee_Number = dbConsolidateAttendance.employee.employee_code;
                    vmConsilidateAttendance.First_Name = dbConsolidateAttendance.employee.first_name;
                    vmConsilidateAttendance.Last_Name = dbConsolidateAttendance.employee.last_name;
                    vmConsilidateAttendance.Time_In = (dbConsolidateAttendance.time_in.HasValue) ? dbConsolidateAttendance.time_in.Value.ToString("hh:mm tt") : "";
                    //vmConsilidateAttendance.time_in = (dbConsolidateAttendance.time_in != null && dbConsolidateAttendance.time_in.HasValue)
                    //     ? dbConsolidateAttendance.time_in.Value.TimeOfDay.ToString("c") : "";
                    vmConsilidateAttendance.Remarks_In = dbConsolidateAttendance.status_in;

                    vmConsilidateAttendance.Time_Out = (dbConsolidateAttendance.time_out.HasValue) ? dbConsolidateAttendance.time_out.Value.ToString("hh:mm tt") : "";
                    //vmConsilidateAttendance.time_out = (dbConsolidateAttendance.time_out != null && dbConsolidateAttendance.time_out.HasValue)
                    // ? dbConsolidateAttendance.time_out.Value.TimeOfDay.ToString("c") : "";
                    vmConsilidateAttendance.Remarks_Out = dbConsolidateAttendance.status_out;
                    vmConsilidateAttendance.Device_In = (dbConsolidateAttendance.terminal_in != null) ? dbConsolidateAttendance.terminal_in : " ";
                    vmConsilidateAttendance.Device_Out = (dbConsolidateAttendance.terminal_out != null) ? dbConsolidateAttendance.terminal_out : " ";
                    vmConsilidateAttendance.Final_Remarks = (dbConsolidateAttendance.manualAttendances.Count > 0) ?
                        dbConsolidateAttendance.final_remarks + "*" : dbConsolidateAttendance.final_remarks;
                    vmConsilidateAttendance.description = dbConsolidateAttendance.description;
                    /*
                    // check if there are manual attendances for this log.
                    if (dbConsolidateAttendance.manualAttendances.Count > 0)
                    {

                        int latestManualAttendanceID =
                            dbConsolidateAttendance.manualAttendances.Max(m => m.ManualAttendanceId);

                        DLL.Models.ManualAttendance manualAttendance =
                            dbConsolidateAttendance.manualAttendances.Where(m => m.ManualAttendanceId == latestManualAttendanceID).FirstOrDefault();


                        vmConsilidateAttendance.final_remarks = manualAttendance.remarks;
                        for (int ind = 0; ind < dbConsolidateAttendance.manualAttendances.Count; ind++)
                        {
                            vmConsilidateAttendance.final_remarks += '*';
                        }




                    }
                    else
                    {
                        vmConsilidateAttendance.final_remarks = dbConsolidateAttendance.final_remarks;
                    }*/






                    toReturn.Add(vmConsilidateAttendance);
                }

                return toReturn;
            }
        }


        public static List<ViewModels.ConsolidatedAttendanceExportOvertime> getAllConsolidateAttendanceBySupervisorExportOvertime(string search, string employee_code, string from, string to, string supervisorCode)
        {

            /************ PARSING MONTH AND EMPLOYEE CODE *******************************************/




            DateTime? firstDay = null;
            DateTime? lastDay = null;


            if (from != null && to != null && !from.Equals("") && !to.Equals(""))
            {

                try
                {
                    firstDay = DateTime.ParseExact(from, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                    lastDay = DateTime.ParseExact(to, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                }
                catch
                {

                }
                /*string[] components = "saf".Split('-');

                int year;
                int mnth;

                if (components.Length == 2)
                {
                    if (int.TryParse(components[0], out year) && int.TryParse(components[1],out mnth))
                    {
                        firstDayOfMonth = new DateTime(year, mnth, 1);
                        lastDayOfMonth = firstDayOfMonth.Value.AddMonths(1).AddDays(-1);
                    }
                }*/

            }
            else
            {
                firstDay = DateTime.ParseExact(DateTime.Now.ToString("dd-MM-yyyy"), "dd-MM-yyyy", CultureInfo.InvariantCulture);
                lastDay = DateTime.ParseExact(DateTime.Now.AddDays(1).ToString("dd-MM-yyyy"), "dd-MM-yyyy", CultureInfo.InvariantCulture);
            }
            /***************************************************************************/


            using (Context db = new Context())
            {

                //string employee_id = db.;
                //int emp_id;
                //if (!int.TryParse(employee_id, out emp_id))
                //{
                //    emp_id = -1;
                //}


                List<ConsolidatedAttendance> consolidateAttendance = null;
                try
                {

                    DateTime? times;
                    string sdd = "";
                    if (search != null && search.Split(':').Count() == 2 && search.Length == 8)
                    {
                        times = DateTime.Parse(search);
                        var pp = times.Value.TimeOfDay.ToString();
                        sdd = pp.Remove(pp.Length - 3);
                        //sdd = pp;
                    }
                    var supervisor = db.employee.Where(m => m.employee_code.Equals(supervisorCode)).FirstOrDefault().Group;
                    if (supervisor == null)
                    {
                        return null;
                    }

                    int groupId = supervisor.GroupId;

                    consolidateAttendance = db.consolidated_attendance.Where(m =>
                        m.active && m.employee.timetune_active &&
                        m.employee.Group.GroupId.Equals(groupId)

                        &&


                        (string.IsNullOrEmpty(employee_code) ||
                        m.employee.employee_code.Equals(employee_code))
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
                        m.terminal_in.ToLower().Contains(search.ToLower()) ||
                        System.Data.Entity.DbFunctions.TruncateTime(m.date).ToString().Contains(search.ToLower()) ||
                        System.Data.Entity.DbFunctions.CreateTime(m.time_in.Value.Hour, m.time_in.Value.Minute, m.time_in.Value.Second).ToString().Contains(sdd.ToLower()) ||
                        System.Data.Entity.DbFunctions.CreateTime(m.time_out.Value.Hour, m.time_out.Value.Minute, m.time_out.Value.Second).ToString().Contains(sdd.ToLower()) ||
                        m.terminal_out.ToLower().Contains(search.ToLower())
                        )

                        &&
                        (!firstDay.HasValue ||
                        (m.date.Value >= firstDay.Value && m.date.Value <= lastDay))

                        ).ToList();

                }
                catch (System.Data.Entity.Core.EntityCommandExecutionException ex)
                {
                    consolidateAttendance = new List<ConsolidatedAttendance>();
                }



                List<ViewModels.ConsolidatedAttendanceExportOvertime> toReturn = new List<ViewModels.ConsolidatedAttendanceExportOvertime>();

                string OvertimeNew = "";
                string OvertimeStatusNew = "";
                TimeSpan time;

                for (int i = 0; i < consolidateAttendance.Count(); i++)
                {



                    ViewModels.ConsolidatedAttendanceExportOvertime vmConsilidateAttendance = new ViewModels.ConsolidatedAttendanceExportOvertime();
                    DLL.Models.ConsolidatedAttendance dbConsolidateAttendance = new DLL.Models.ConsolidatedAttendance();

                    //Force Load
                    dbConsolidateAttendance = consolidateAttendance[i];


                    if (dbConsolidateAttendance.overtime < 0)
                    {
                        time = TimeSpan.FromSeconds(dbConsolidateAttendance.overtime);
                        OvertimeNew = "-" + time.ToString(@"hh\:mm\:ss");
                    }
                    else
                    {
                        time = TimeSpan.FromSeconds(dbConsolidateAttendance.overtime);
                        OvertimeNew = time.ToString(@"hh\:mm\:ss");
                    }

                    if (dbConsolidateAttendance.overtime_status == 1) { OvertimeStatusNew = "Unapproved"; }
                    else if (dbConsolidateAttendance.overtime_status == 2) { OvertimeStatusNew = "Approved"; }
                    else if (dbConsolidateAttendance.overtime_status == 3) { OvertimeStatusNew = "Discard"; }

                    ////vmConsilidateAttendance.id = dbConsolidateAttendance.ConsolidatedAttendanceId;
                    vmConsilidateAttendance.Date = (dbConsolidateAttendance.date != null && dbConsolidateAttendance.date.HasValue)
                         ? dbConsolidateAttendance.date.Value.ToString("dd-MM-yyyy") : "";
                    vmConsilidateAttendance.Employee_Number = dbConsolidateAttendance.employee.employee_code;
                    vmConsilidateAttendance.First_Name = dbConsolidateAttendance.employee.first_name;
                    vmConsilidateAttendance.Last_Name = dbConsolidateAttendance.employee.last_name;
                    vmConsilidateAttendance.Time_In = (dbConsolidateAttendance.time_in.HasValue) ? dbConsolidateAttendance.time_in.Value.ToString("hh:mm tt") : "";
                    vmConsilidateAttendance.Time_Out = (dbConsolidateAttendance.time_out.HasValue) ? dbConsolidateAttendance.time_out.Value.ToString("hh:mm tt") : "";
                    vmConsilidateAttendance.overtime = OvertimeNew;
                    vmConsilidateAttendance.overtime_status = OvertimeStatusNew;


                    toReturn.Add(vmConsilidateAttendance);
                }

                return toReturn;
            }
        }



    }




    public class AttendanceSLM
    {

        public static List<ViewModels.ConsolidatedAttendanceLog> getAllConsolidateAttendanceBySLM(string search, string employeeId, string from, string to, string slmEmployeeCode)
        {

            /************ PARSING MONTH AND EMPLOYEE CODE *******************************************/
            int employeeID;
            if (!int.TryParse(employeeId, out employeeID))
            {
                employeeID = -1;
            }


            DateTime? firstDay = null;
            DateTime? lastDay = null;


            if (from != null && to != null && !from.Equals("") && !to.Equals(""))
            {

                try
                {
                    firstDay = DateTime.ParseExact(from, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                    lastDay = DateTime.ParseExact(to, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                }
                catch
                {

                }
                /*string[] components = "saf".Split('-');

                int year;
                int mnth;

                if (components.Length == 2)
                {
                    if (int.TryParse(components[0], out year) && int.TryParse(components[1],out mnth))
                    {
                        firstDayOfMonth = new DateTime(year, mnth, 1);
                        lastDayOfMonth = firstDayOfMonth.Value.AddMonths(1).AddDays(-1);
                    }
                }*/

            }
            else
            {

                firstDay = DateTime.ParseExact(DateTime.Now.ToString("dd-MM-yyyy"), "dd-MM-yyyy", CultureInfo.InvariantCulture);
                lastDay = DateTime.ParseExact(DateTime.Now.AddDays(1).ToString("dd-MM-yyyy"), "dd-MM-yyyy", CultureInfo.InvariantCulture);

            }
            /***************************************************************************/


            using (Context db = new Context())
            {
                List<ConsolidatedAttendance> consolidateAttendance = null;
                try
                {
                    DateTime? times;
                    string sdd = "";
                    if (search != null && search.Split(':').Count() == 2 && search.Length == 8)
                    {
                        times = DateTime.Parse(search);
                        var pp = times.Value.TimeOfDay.ToString();
                        sdd = pp.Remove(pp.Length - 3);
                    }

                    int empId = 0;
                    try
                    {
                        empId = db.employee.Where(m => m.employee_code.Equals(employeeId)).FirstOrDefault().EmployeeId;
                    }
                    catch (Exception ex)
                    {
                        employeeID = -1;
                    }
                    List<int> emps = db.super_line_manager_tagging.Where(m =>
                        m.superLineManager.employee_code.Equals(slmEmployeeCode) &&
                        m.taggedEmployee.active &&

                        (
                        employeeID.Equals(-1) ||
                        m.taggedEmployee.EmployeeId.Equals(empId)
                        )

                        ).Select(p =>


                            p.taggedEmployee.EmployeeId

                            ).ToList();



                    DLL.Models.Employee superLineManager = db.employee.Where(m =>
                        m.active &&
                        m.employee_code.Equals(slmEmployeeCode)
                        ).FirstOrDefault();

                    if (superLineManager != null)
                    {
                        if (employeeID == superLineManager.EmployeeId)
                        {
                            emps.Add(superLineManager.EmployeeId);
                        }
                    }


                    int[] employeeIDs = emps.ToArray();

                    /*
                    var group = db.employee.Where(m => m.employee_code.Equals(supervisorCode)).FirstOrDefault().Group;
                    if (group == null)
                    {
                        return null;
                    }
                    int groupId = group.GroupId;*/

                    consolidateAttendance =

                        db.consolidated_attendance.Where(m =>
                            m.active &&
                            employeeIDs.Contains(m.employee.EmployeeId) &&

                            (
                        search == null ||
                        search.Equals("") ||
                        m.employee.employee_code.Contains(search) ||
                         m.employee.first_name.ToLower().Contains(search.ToLower()) ||
                        m.employee.last_name.ToLower().Contains(search.ToLower()) ||
                        m.status_in.ToLower().Contains(search.ToLower()) ||
                        m.status_out.ToLower().Contains(search.ToLower()) ||
                        m.final_remarks.ToLower().Contains(search.ToLower()) ||
                        m.terminal_in.ToLower().Contains(search.ToLower()) ||
                        System.Data.Entity.DbFunctions.TruncateTime(m.date).ToString().Contains(search.ToLower()) ||
                        System.Data.Entity.DbFunctions.CreateTime(m.time_in.Value.Hour, m.time_in.Value.Minute, m.time_in.Value.Second).ToString().Contains(sdd.ToLower()) ||
                        System.Data.Entity.DbFunctions.CreateTime(m.time_out.Value.Hour, m.time_out.Value.Minute, m.time_out.Value.Second).ToString().Contains(sdd.ToLower()) ||
                        m.terminal_out.ToLower().Contains(search.ToLower())
                        )
                            &&
                            (!firstDay.HasValue || (m.date.Value >= firstDay.Value && m.date.Value <= lastDay))
                        ).ToList();
                    /*
                    db.consolidated_attendance.Where(m =>
                    m.active &&
                    m.employee.Group.GroupId.Equals(groupId)

                    &&

                    (emp_id.Equals(-1) ||
                    m.employee.EmployeeId.Equals(emp_id))


                    &&
                    (!firstDayOfMonth.HasValue ||
                    (m.date.Value >= firstDayOfMonth.Value && m.date.Value <= lastDayOfMonth))

                    ).ToList();*/
                }
                catch (System.Data.Entity.Core.EntityCommandExecutionException ex)
                {
                    consolidateAttendance = new List<ConsolidatedAttendance>();
                }



                List<ViewModels.ConsolidatedAttendanceLog> toReturn = new List<ViewModels.ConsolidatedAttendanceLog>();

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
                    vmConsilidateAttendance.employee_code = dbConsolidateAttendance.employee.employee_code;
                    vmConsilidateAttendance.employee_first_name = dbConsolidateAttendance.employee.first_name;
                    vmConsilidateAttendance.employee_last_name = dbConsolidateAttendance.employee.last_name;
                    vmConsilidateAttendance.time_in = (dbConsolidateAttendance.time_in.HasValue) ? dbConsolidateAttendance.time_in.Value.ToString("hh:mm tt") : "";
                    //vmConsilidateAttendance.time_in = (dbConsolidateAttendance.time_in != null && dbConsolidateAttendance.time_in.HasValue)
                    //? dbConsolidateAttendance.time_in.Value.TimeOfDay.ToString("hh:mm tt") : "";
                    vmConsilidateAttendance.status_in = dbConsolidateAttendance.status_in;
                    vmConsilidateAttendance.time_out = (dbConsolidateAttendance.time_out.HasValue) ? dbConsolidateAttendance.time_out.Value.ToString("hh:mm tt") : "";
                    //vmConsilidateAttendance.time_out = (dbConsolidateAttendance.time_out != null && dbConsolidateAttendance.time_out.HasValue)
                    // ? dbConsolidateAttendance.time_out.Value.TimeOfDay.ToString("c") : "";
                    vmConsilidateAttendance.status_out = dbConsolidateAttendance.status_out;
                    vmConsilidateAttendance.terminal_in = (dbConsolidateAttendance.terminal_in != null) ? dbConsolidateAttendance.terminal_in : " ";
                    vmConsilidateAttendance.terminal_out = (dbConsolidateAttendance.terminal_out != null) ? dbConsolidateAttendance.terminal_out : " ";
                    vmConsilidateAttendance.final_remarks = (dbConsolidateAttendance.manualAttendances.Count > 0) ?
                        dbConsolidateAttendance.final_remarks + "*" : dbConsolidateAttendance.final_remarks;
                    vmConsilidateAttendance.overtime = strOvertime;
                    vmConsilidateAttendance.overtime_status = strOvertimeStatus;

                    vmConsilidateAttendance.action =
                           @"<div data-id='" + dbConsolidateAttendance.ConsolidatedAttendanceId + @"'>
                                <a href='javascript:void(editStatus(" + dbConsolidateAttendance.ConsolidatedAttendanceId + "," + dbConsolidateAttendance.overtime_status + @"));'>Edit</a>
                            </div>";

                    toReturn.Add(vmConsilidateAttendance);
                }

                return toReturn;
            }
        }

        public static List<ViewModels.ConsolidatedAttendanceExport> getAllConsolidateAttendanceBySLMExport(string search, string employeeId, string from, string to, string slmEmployeeCode)
        {

            /************ PARSING MONTH AND EMPLOYEE CODE *******************************************/
            int employeeID;
            if (!int.TryParse(employeeId, out employeeID))
            {
                employeeID = -1;
            }


            DateTime? firstDay = null;
            DateTime? lastDay = null;


            if (from != null && to != null && !from.Equals("") && !to.Equals(""))
            {

                try
                {
                    firstDay = DateTime.ParseExact(from, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                    lastDay = DateTime.ParseExact(to, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                }
                catch
                {

                }
                /*string[] components = "saf".Split('-');

                int year;
                int mnth;

                if (components.Length == 2)
                {
                    if (int.TryParse(components[0], out year) && int.TryParse(components[1],out mnth))
                    {
                        firstDayOfMonth = new DateTime(year, mnth, 1);
                        lastDayOfMonth = firstDayOfMonth.Value.AddMonths(1).AddDays(-1);
                    }
                }*/

            }
            else
            {

                firstDay = DateTime.ParseExact(DateTime.Now.ToString("dd-MM-yyyy"), "dd-MM-yyyy", CultureInfo.InvariantCulture);
                lastDay = DateTime.ParseExact(DateTime.Now.AddDays(1).ToString("dd-MM-yyyy"), "dd-MM-yyyy", CultureInfo.InvariantCulture);

            }
            /***************************************************************************/


            using (Context db = new Context())
            {
                List<ConsolidatedAttendance> consolidateAttendance = null;
                try
                {
                    DateTime? times;
                    string sdd = "";
                    if (search != null && search.Split(':').Count() == 2 && search.Length == 8)
                    {
                        times = DateTime.Parse(search);
                        var pp = times.Value.TimeOfDay.ToString();
                        sdd = pp.Remove(pp.Length - 3);
                    }

                    int empId = 0;
                    try
                    {
                        empId = db.employee.Where(m => m.employee_code.Equals(employeeId)).FirstOrDefault().EmployeeId;
                    }
                    catch (Exception ex)
                    {
                        employeeID = -1;
                    }
                    List<int> emps = db.super_line_manager_tagging.Where(m =>
                        m.superLineManager.employee_code.Equals(slmEmployeeCode) &&
                        m.taggedEmployee.active &&

                        (
                        employeeID.Equals(-1) ||
                        m.taggedEmployee.EmployeeId.Equals(empId)
                        )

                        ).Select(p =>


                            p.taggedEmployee.EmployeeId

                            ).ToList();



                    DLL.Models.Employee superLineManager = db.employee.Where(m =>
                        m.active &&
                        m.employee_code.Equals(slmEmployeeCode)
                        ).FirstOrDefault();

                    if (superLineManager != null)
                    {
                        if (employeeID == superLineManager.EmployeeId)
                        {
                            emps.Add(superLineManager.EmployeeId);
                        }
                    }


                    int[] employeeIDs = emps.ToArray();

                    /*
                    var group = db.employee.Where(m => m.employee_code.Equals(supervisorCode)).FirstOrDefault().Group;
                    if (group == null)
                    {
                        return null;
                    }
                    int groupId = group.GroupId;*/

                    consolidateAttendance =

                        db.consolidated_attendance.Where(m =>
                            m.active &&
                            employeeIDs.Contains(m.employee.EmployeeId) &&

                            (
                        search == null ||
                        search.Equals("") ||
                        m.employee.employee_code.Contains(search) ||
                         m.employee.first_name.ToLower().Contains(search.ToLower()) ||
                        m.employee.last_name.ToLower().Contains(search.ToLower()) ||
                        m.status_in.ToLower().Contains(search.ToLower()) ||
                        m.status_out.ToLower().Contains(search.ToLower()) ||
                        m.final_remarks.ToLower().Contains(search.ToLower()) ||
                        m.terminal_in.ToLower().Contains(search.ToLower()) ||
                        System.Data.Entity.DbFunctions.TruncateTime(m.date).ToString().Contains(search.ToLower()) ||
                        System.Data.Entity.DbFunctions.CreateTime(m.time_in.Value.Hour, m.time_in.Value.Minute, m.time_in.Value.Second).ToString().Contains(sdd.ToLower()) ||
                        System.Data.Entity.DbFunctions.CreateTime(m.time_out.Value.Hour, m.time_out.Value.Minute, m.time_out.Value.Second).ToString().Contains(sdd.ToLower()) ||
                        m.terminal_out.ToLower().Contains(search.ToLower())
                        )
                            &&
                            (!firstDay.HasValue || (m.date.Value >= firstDay.Value && m.date.Value <= lastDay))
                        ).ToList();
                    /*
                    db.consolidated_attendance.Where(m =>
                    m.active &&
                    m.employee.Group.GroupId.Equals(groupId)

                    &&

                    (emp_id.Equals(-1) ||
                    m.employee.EmployeeId.Equals(emp_id))


                    &&
                    (!firstDayOfMonth.HasValue ||
                    (m.date.Value >= firstDayOfMonth.Value && m.date.Value <= lastDayOfMonth))

                    ).ToList();*/
                }
                catch (System.Data.Entity.Core.EntityCommandExecutionException ex)
                {
                    consolidateAttendance = new List<ConsolidatedAttendance>();
                }



                List<ViewModels.ConsolidatedAttendanceExport> toReturn = new List<ViewModels.ConsolidatedAttendanceExport>();

                for (int i = 0; i < consolidateAttendance.Count(); i++)
                {
                    ViewModels.ConsolidatedAttendanceExport vmConsilidateAttendance = new ViewModels.ConsolidatedAttendanceExport();
                    DLL.Models.ConsolidatedAttendance dbConsolidateAttendance = new DLL.Models.ConsolidatedAttendance();
                    //Force Load
                    dbConsolidateAttendance = consolidateAttendance[i];

                    ////vmConsilidateAttendance.id = dbConsolidateAttendance.ConsolidatedAttendanceId;
                    vmConsilidateAttendance.Date = (dbConsolidateAttendance.date != null && dbConsolidateAttendance.date.HasValue)
                         ? dbConsolidateAttendance.date.Value.ToString("dd-MM-yyyy") : "";
                    vmConsilidateAttendance.Employee_Number = dbConsolidateAttendance.employee.employee_code;
                    vmConsilidateAttendance.First_Name = dbConsolidateAttendance.employee.first_name;
                    vmConsilidateAttendance.Last_Name = dbConsolidateAttendance.employee.last_name;
                    vmConsilidateAttendance.Time_In = (dbConsolidateAttendance.time_in.HasValue) ? dbConsolidateAttendance.time_in.Value.ToString("hh:mm tt") : "";
                    //vmConsilidateAttendance.time_in = (dbConsolidateAttendance.time_in != null && dbConsolidateAttendance.time_in.HasValue)
                    //? dbConsolidateAttendance.time_in.Value.TimeOfDay.ToString("hh:mm tt") : "";
                    vmConsilidateAttendance.Remarks_In = dbConsolidateAttendance.status_in;
                    vmConsilidateAttendance.Time_Out = (dbConsolidateAttendance.time_out.HasValue) ? dbConsolidateAttendance.time_out.Value.ToString("hh:mm tt") : "";
                    //vmConsilidateAttendance.time_out = (dbConsolidateAttendance.time_out != null && dbConsolidateAttendance.time_out.HasValue)
                    // ? dbConsolidateAttendance.time_out.Value.TimeOfDay.ToString("c") : "";
                    vmConsilidateAttendance.Remarks_Out = dbConsolidateAttendance.status_out;
                    vmConsilidateAttendance.Device_In = (dbConsolidateAttendance.terminal_in != null) ? dbConsolidateAttendance.terminal_in : " ";
                    vmConsilidateAttendance.Device_Out = (dbConsolidateAttendance.terminal_out != null) ? dbConsolidateAttendance.terminal_out : " ";
                    vmConsilidateAttendance.Final_Remarks = (dbConsolidateAttendance.manualAttendances.Count > 0) ?
                        dbConsolidateAttendance.final_remarks + "*" : dbConsolidateAttendance.final_remarks;

                    toReturn.Add(vmConsilidateAttendance);
                }

                return toReturn;
            }
        }

        public static List<ViewModels.ConsolidatedAttendanceExportOvertime> getAllConsolidateAttendanceBySLMExportOvertime(string search, string employeeId, string from, string to, string slmEmployeeCode)
        {

            /************ PARSING MONTH AND EMPLOYEE CODE *******************************************/
            int employeeID;
            if (!int.TryParse(employeeId, out employeeID))
            {
                employeeID = -1;
            }


            DateTime? firstDay = null;
            DateTime? lastDay = null;


            if (from != null && to != null && !from.Equals("") && !to.Equals(""))
            {

                try
                {
                    firstDay = DateTime.ParseExact(from, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                    lastDay = DateTime.ParseExact(to, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                }
                catch
                {

                }
                /*string[] components = "saf".Split('-');

                int year;
                int mnth;

                if (components.Length == 2)
                {
                    if (int.TryParse(components[0], out year) && int.TryParse(components[1],out mnth))
                    {
                        firstDayOfMonth = new DateTime(year, mnth, 1);
                        lastDayOfMonth = firstDayOfMonth.Value.AddMonths(1).AddDays(-1);
                    }
                }*/

            }
            else
            {

                firstDay = DateTime.ParseExact(DateTime.Now.ToString("dd-MM-yyyy"), "dd-MM-yyyy", CultureInfo.InvariantCulture);
                lastDay = DateTime.ParseExact(DateTime.Now.AddDays(1).ToString("dd-MM-yyyy"), "dd-MM-yyyy", CultureInfo.InvariantCulture);

            }
            /***************************************************************************/


            using (Context db = new Context())
            {
                List<ConsolidatedAttendance> consolidateAttendance = null;
                try
                {
                    DateTime? times;
                    string sdd = "";
                    if (search != null && search.Split(':').Count() == 2 && search.Length == 8)
                    {
                        times = DateTime.Parse(search);
                        var pp = times.Value.TimeOfDay.ToString();
                        sdd = pp.Remove(pp.Length - 3);
                    }

                    int empId = 0;
                    try
                    {
                        empId = db.employee.Where(m => m.employee_code.Equals(employeeId)).FirstOrDefault().EmployeeId;
                    }
                    catch (Exception ex)
                    {
                        employeeID = -1;
                    }
                    List<int> emps = db.super_line_manager_tagging.Where(m =>
                        m.superLineManager.employee_code.Equals(slmEmployeeCode) &&
                        m.taggedEmployee.active &&

                        (
                        employeeID.Equals(-1) ||
                        m.taggedEmployee.EmployeeId.Equals(empId)
                        )

                        ).Select(p =>


                            p.taggedEmployee.EmployeeId

                            ).ToList();



                    DLL.Models.Employee superLineManager = db.employee.Where(m =>
                        m.active &&
                        m.employee_code.Equals(slmEmployeeCode)
                        ).FirstOrDefault();

                    if (superLineManager != null)
                    {
                        if (employeeID == superLineManager.EmployeeId)
                        {
                            emps.Add(superLineManager.EmployeeId);
                        }
                    }


                    int[] employeeIDs = emps.ToArray();

                    /*
                    var group = db.employee.Where(m => m.employee_code.Equals(supervisorCode)).FirstOrDefault().Group;
                    if (group == null)
                    {
                        return null;
                    }
                    int groupId = group.GroupId;*/

                    consolidateAttendance =

                        db.consolidated_attendance.Where(m =>
                            m.active &&
                            employeeIDs.Contains(m.employee.EmployeeId) &&

                            (
                        search == null ||
                        search.Equals("") ||
                        m.employee.employee_code.Contains(search) ||
                         m.employee.first_name.ToLower().Contains(search.ToLower()) ||
                        m.employee.last_name.ToLower().Contains(search.ToLower()) ||
                        m.status_in.ToLower().Contains(search.ToLower()) ||
                        m.status_out.ToLower().Contains(search.ToLower()) ||
                        m.final_remarks.ToLower().Contains(search.ToLower()) ||
                        m.terminal_in.ToLower().Contains(search.ToLower()) ||
                        System.Data.Entity.DbFunctions.TruncateTime(m.date).ToString().Contains(search.ToLower()) ||
                        System.Data.Entity.DbFunctions.CreateTime(m.time_in.Value.Hour, m.time_in.Value.Minute, m.time_in.Value.Second).ToString().Contains(sdd.ToLower()) ||
                        System.Data.Entity.DbFunctions.CreateTime(m.time_out.Value.Hour, m.time_out.Value.Minute, m.time_out.Value.Second).ToString().Contains(sdd.ToLower()) ||
                        m.terminal_out.ToLower().Contains(search.ToLower())
                        )
                            &&
                            (!firstDay.HasValue || (m.date.Value >= firstDay.Value && m.date.Value <= lastDay))
                        ).ToList();
                    /*
                    db.consolidated_attendance.Where(m =>
                    m.active &&
                    m.employee.Group.GroupId.Equals(groupId)

                    &&

                    (emp_id.Equals(-1) ||
                    m.employee.EmployeeId.Equals(emp_id))


                    &&
                    (!firstDayOfMonth.HasValue ||
                    (m.date.Value >= firstDayOfMonth.Value && m.date.Value <= lastDayOfMonth))

                    ).ToList();*/
                }
                catch (System.Data.Entity.Core.EntityCommandExecutionException ex)
                {
                    consolidateAttendance = new List<ConsolidatedAttendance>();
                }


                string OvertimeNew = "";
                string OvertimeStatusNew = "";
                TimeSpan time;
                List<ViewModels.ConsolidatedAttendanceExportOvertime> toReturn = new List<ViewModels.ConsolidatedAttendanceExportOvertime>();

                for (int i = 0; i < consolidateAttendance.Count(); i++)
                {
                    ViewModels.ConsolidatedAttendanceExportOvertime vmConsilidateAttendance = new ViewModels.ConsolidatedAttendanceExportOvertime();
                    DLL.Models.ConsolidatedAttendance dbConsolidateAttendance = new DLL.Models.ConsolidatedAttendance();
                    //Force Load
                    dbConsolidateAttendance = consolidateAttendance[i];


                    if (dbConsolidateAttendance.overtime < 0)
                    {
                        time = TimeSpan.FromSeconds(dbConsolidateAttendance.overtime);
                        OvertimeNew = "-" + time.ToString(@"hh\:mm\:ss");
                    }
                    else
                    {
                        time = TimeSpan.FromSeconds(dbConsolidateAttendance.overtime);
                        OvertimeNew = time.ToString(@"hh\:mm\:ss");
                    }

                    if (dbConsolidateAttendance.overtime_status == 1) { OvertimeStatusNew = "Unapproved"; }
                    else if (dbConsolidateAttendance.overtime_status == 2) { OvertimeStatusNew = "Approved"; }
                    else if (dbConsolidateAttendance.overtime_status == 3) { OvertimeStatusNew = "Discard"; }


                    ////vmConsilidateAttendance.id = dbConsolidateAttendance.ConsolidatedAttendanceId;
                    vmConsilidateAttendance.Date = (dbConsolidateAttendance.date != null && dbConsolidateAttendance.date.HasValue)
                         ? dbConsolidateAttendance.date.Value.ToString("dd-MM-yyyy") : "";
                    vmConsilidateAttendance.Employee_Number = dbConsolidateAttendance.employee.employee_code;
                    vmConsilidateAttendance.First_Name = dbConsolidateAttendance.employee.first_name;
                    vmConsilidateAttendance.Last_Name = dbConsolidateAttendance.employee.last_name;
                    vmConsilidateAttendance.Time_In = (dbConsolidateAttendance.time_in.HasValue) ? dbConsolidateAttendance.time_in.Value.ToString("hh:mm tt") : "";

                    vmConsilidateAttendance.Time_Out = (dbConsolidateAttendance.time_out.HasValue) ? dbConsolidateAttendance.time_out.Value.ToString("hh:mm tt") : "";

                    vmConsilidateAttendance.overtime = OvertimeNew;
                    vmConsilidateAttendance.overtime_status = OvertimeStatusNew;
                    toReturn.Add(vmConsilidateAttendance);
                }

                return toReturn;
            }
        }



    }
}
