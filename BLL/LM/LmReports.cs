using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DLL.Models;
using System.Globalization;
using System.Data.SqlClient;
using System.Globalization;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using System.Web;

namespace TimeTune
{
    public class LmReports
    {
        public class TestModel
        {
            public int LV { get; set; }
            public int PO { get; set; }
            public int AB { get; set; }
            public int PL { get; set; }
            public int PE { get; set; }
            public int Present { get; set; }
            public string employee_code { get; set; }
            public string first_name { get; set; }
            public string last_name { get; set; }
            public string dept_name { get; set; }
            public string desg_name { get; set; }
            public string camp_name { get; set; }
        }
        public static int getMonthlyReportLM(string search, string sortOrder, int start, int length, out List<ViewModels.MonthlyReport> toReturn, string fromDate, string toDate, string supervisorCode)
        {
            int count = 0;
            toReturn = null;
            if (fromDate == null || toDate == null || toDate.Equals("") || fromDate.Equals(""))
            {
                toReturn = new List<ViewModels.MonthlyReport>();
                return 0;
            }
            DateTime? from = DateTime.ParseExact(fromDate, "dd-MM-yyyy", CultureInfo.InvariantCulture).Date;
            DateTime? to = DateTime.ParseExact(toDate, "dd-MM-yyyy", CultureInfo.InvariantCulture).Date;
            using (var db = new Context())
            {
                string supervisorId;
                try
                {
                    supervisorId = db.employee.Where(m => m.employee_code.Equals(supervisorCode)).FirstOrDefault().EmployeeId.ToString();

                    string sql1 =
                        "  SELECT \n" +
                    "sum( case when final_remarks like 'PO'or final_remarks like 'POM' then 1 else 0 end ) as PO, sum (case when final_remarks like 'LV' then 1 else 0 end ) as LV, sum (case when final_remarks like 'AB' then 1 else 0 end ) as AB\n" +
                    ",sum( case when final_remarks like 'PLE' or final_remarks like 'PLO' or final_remarks like 'PLM' then 1 else 0 end ) as PL ,employee_code, \n" +
                    "sum(case when final_remarks like 'POE' or final_remarks like 'PLE' then 1 else 0 end) as PE,\n" +
                    "sum (case when final_remarks not like 'AB' and final_remarks not like 'OFF'  then 1 else 0 end ) Present\n" +
                    "FROM ConsolidatedAttendances co, Employees\n" +
                    "where \n" +
                    "employee_EmployeeId=EmployeeId and\n" +
                    " [date] between @from and @to and \n" +
                    "employee_EmployeeId  in (SELECT distinct employee_EmployeeId FROM ConsolidatedAttendances c, Employees e, Groups g \n" +
                    " where e.Group_GroupId = g.GroupId and e.EmployeeId=c.employee_EmployeeId and  supervisor_id like @supervisor) and\n" +
                    "employee_code  like @search\n" +
                    "group by employee_code\n";

                    string sql =
                        "  SELECT \n" +
                    "co.employee_EmployeeId, sum( case when final_remarks like 'PO' or final_remarks like 'POE' or final_remarks like 'POM' then 1 else 0 end ) as PO, " +
                    //"sum (case when final_remarks like 'LV' then 1 else 0 end ) as LV, " + 
                    "ISNULL((select sum(dayscount) from LeaveApplications l where IsActive=1 and (LeaveTypeId>=1 and LeaveTypeId<=15) and LeaveStatusId=2 and l.EmployeeId=co.employee_EmployeeId and (FromDate>=@from and ToDate<=@to)),0) as LV," +
                     " ISNULL((select top 1 ISNULL(name,'-') from Departments d where d.DepartmentId=e.department_DepartmentId),'--') as dept_name, \n" +
                        " ISNULL((select top 1 ISNULL(name,'-') from Designations d where d.DesignationId=e.designation_DesignationId),'--') as desg_name, \n" +
                        " ISNULL((select top 1 ISNULL(CampusCode,'-') from OrganizationCampus o where o.Id=e.campus_id),'--') as camp_name, \n" +
                    "sum (case when final_remarks like 'AB' then 1 else 0 end ) as AB, sum (case when final_remarks like 'OFF' then 1 else 0 end ) as [OFF]\n" +
                    ",sum( case when final_remarks like 'PLE' or final_remarks like 'PLO' or final_remarks like 'PLM' then 1 else 0 end ) as PL ,employee_code,first_name,last_name, \n" +
                    "sum(case when final_remarks like 'POE' or final_remarks like 'PLE' then 1 else 0 end) as PE,\n" +
                    //"sum (case when final_remarks not like 'AB' and final_remarks not like 'OFF' and final_remarks not like 'LV%'  then 1 else 0 end ) Present\n" +
                    "sum (case when final_remarks like 'P%' or final_remarks like 'OV' or final_remarks like 'OD' or final_remarks like 'OM' or final_remarks like 'OT' or final_remarks like 'POOD' or final_remarks like 'ODPO' then 1 else 0 end ) Present\n" +
                    "FROM ConsolidatedAttendances co, Employees e\n" +
                    "where \n" +
                    "co.employee_EmployeeId=e.EmployeeId and e.Active=1 and\n" +
                    "([date] >= @from and [date] <= @to) and\n" + //" [date] between @from and @to and \n" +
                    "employee_EmployeeId  in (SELECT distinct employee_EmployeeId FROM ConsolidatedAttendances c, Employees e, Groups g \n" +
                    " where e.Group_GroupId = g.GroupId and e.EmployeeId=c.employee_EmployeeId and  supervisor_id like @supervisor) and\n" +
                    "(employee_code like @search or first_name like @search or last_name like @search)\n" +
                    "group by co.employee_EmployeeId,employee_code,first_name,last_name,e.department_DepartmentId,e.designation_DesignationId,e.campus_id\n";

                    //string TO = to.Value.ToString("yyyy-MM-dd");
                    //string FRM = from.Value.ToString("yyyy-MM-dd");
                    var results = db.Database.SqlQuery<TestModel>(
                        sql,
                        new SqlParameter("@from", from),
                        new SqlParameter("@to", to),
                        new SqlParameter("@supervisor", supervisorId),
                        new SqlParameter("@search", (search == null) ? "%%" : "%" + search + "%")).ToList();

                    count = results.Count();
                    toReturn = results.Select(
                        p => new ViewModels.MonthlyReport()
                        {
                            leave = p.LV,
                            ontime = p.PO,
                            late = p.PL,
                            absent = p.AB,
                            employeeCode = p.employee_code,
                            first_name = p.first_name,
                            last_name = p.last_name,
                            dept_name = p.dept_name,
                            desg_name = p.desg_name,
                            camp_name = p.camp_name,
                            earlyGone = p.PE,
                            present = p.Present
                        }).AsQueryable().SortBy(sortOrder).Skip(start).Take(length).ToList();
                }
                catch (System.Data.Entity.Core.EntityCommandExecutionException ex)
                {
                    toReturn = new List<ViewModels.MonthlyReport>();
                }
            }

            return count;
        }

        public static int getMonthlyReportLMExcel(out List<ViewModels.AttendanceSummaryExport> toReturn, string fromDate, string toDate, string supervisorCode)
        {
            int count = 0;
            toReturn = null;
            if (fromDate == null || toDate == null || toDate.Equals("") || fromDate.Equals(""))
            {
                toReturn = new List<ViewModels.AttendanceSummaryExport>();
                return 0;
            }
            DateTime? from = DateTime.ParseExact(fromDate, "dd-MM-yyyy", CultureInfo.InvariantCulture).Date;
            DateTime? to = DateTime.ParseExact(toDate, "dd-MM-yyyy", CultureInfo.InvariantCulture).Date;
            using (var db = new Context())
            {
                string supervisorId;
                try
                {
                    supervisorId = db.employee.Where(m => m.employee_code.Equals(supervisorCode)).FirstOrDefault().EmployeeId.ToString();

                    string sql1 =
                        "  SELECT \n" +
                    "sum( case when final_remarks like 'PO'or final_remarks like 'POM' then 1 else 0 end ) as PO, sum (case when final_remarks like 'LV' then 1 else 0 end ) as LV, sum (case when final_remarks like 'AB' then 1 else 0 end ) as AB\n" +
                    ",sum( case when final_remarks like 'PLE' or final_remarks like 'PLO' or final_remarks like 'PLM' then 1 else 0 end ) as PL ,employee_code, \n" +
                    "sum(case when final_remarks like 'POE' or final_remarks like 'PLE' then 1 else 0 end) as PE,\n" +
                    "sum (case when final_remarks not like 'AB' and final_remarks not like 'OFF'  then 1 else 0 end ) Present\n" +
                    "FROM ConsolidatedAttendances co, Employees\n" +
                    "where \n" +
                    "employee_EmployeeId=EmployeeId and\n" +
                    " [date] between @from and @to and \n" +
                    "employee_EmployeeId  in (SELECT distinct employee_EmployeeId FROM ConsolidatedAttendances c, Employees e, Groups g \n" +
                    " where e.Group_GroupId = g.GroupId and e.EmployeeId=c.employee_EmployeeId and  supervisor_id like @supervisor) and\n" +
                    "employee_code  like @search\n" +
                    "group by employee_code\n";

                    string sql =
                        "  SELECT \n" +
                    "co.employee_EmployeeId, sum( case when final_remarks like 'PO' or final_remarks like 'POE' or final_remarks like 'POM' then 1 else 0 end ) as PO, " +
                    //"sum (case when final_remarks like 'LV' then 1 else 0 end ) as LV, " + 
                    "ISNULL((select sum(dayscount) from LeaveApplications l where IsActive=1 and (LeaveTypeId>=1 and LeaveTypeId<=15) and LeaveStatusId=2 and l.EmployeeId=co.employee_EmployeeId and (FromDate>=@from and ToDate<=@to)),0) as LV," +
                     " ISNULL((select top 1 ISNULL(name,'-') from Departments d where d.DepartmentId=e.department_DepartmentId),'--') as dept_name, \n" +
                        " ISNULL((select top 1 ISNULL(name,'-') from Designations d where d.DesignationId=e.designation_DesignationId),'--') as desg_name, \n" +
                        " ISNULL((select top 1 ISNULL(CampusCode,'-') from OrganizationCampus o where o.Id=e.campus_id),'--') as camp_name, \n" +
                    "sum (case when final_remarks like 'AB' then 1 else 0 end ) as AB, sum (case when final_remarks like 'OFF' then 1 else 0 end ) as [OFF]\n" +
                    ",sum( case when final_remarks like 'PLE' or final_remarks like 'PLO' or final_remarks like 'PLM' then 1 else 0 end ) as PL ,employee_code,first_name,last_name, \n" +
                    "sum(case when final_remarks like 'POE' or final_remarks like 'PLE' then 1 else 0 end) as PE,\n" +
                    //"sum (case when final_remarks not like 'AB' and final_remarks not like 'OFF' and final_remarks not like 'LV%'  then 1 else 0 end ) Present\n" +
                    "sum (case when final_remarks like 'P%' or final_remarks like 'OV' or final_remarks like 'OD' or final_remarks like 'OM' or final_remarks like 'OT' or final_remarks like 'POOD' or final_remarks like 'ODPO' then 1 else 0 end ) Present\n" +
                    "FROM ConsolidatedAttendances co, Employees e\n" +
                    "where \n" +
                    "co.employee_EmployeeId=e.EmployeeId and e.Active=1 and\n" +
                    "([date] >= @from and [date] <= @to) and\n" + //" [date] between @from and @to and \n" +
                    "employee_EmployeeId  in (SELECT distinct employee_EmployeeId FROM ConsolidatedAttendances c, Employees e, Groups g \n" +
                    " where e.Group_GroupId = g.GroupId and e.EmployeeId=c.employee_EmployeeId and  supervisor_id like @supervisor) and\n" +
                    //"(employee_code like @search or first_name like @search or last_name like @search)\n" +
                    //"group by co.employee_EmployeeId,employee_code,first_name,last_name,e.department_DepartmentId,e.designation_DesignationId,e.campus_id\n";
                    "group by co.employee_EmployeeId,employee_code,first_name,last_name,e.department_DepartmentId,e.designation_DesignationId,e.campus_id\n";
                    //string TO = to.Value.ToString("yyyy-MM-dd");
                    //string FRM = from.Value.ToString("yyyy-MM-dd");
                    var results = db.Database.SqlQuery<TestModel>(
                        sql,
                        new SqlParameter("@from", from),
                        new SqlParameter("@to", to),
                        new SqlParameter("@supervisor", supervisorId)).ToList();

                    count = results.Count();
                    toReturn = results.Select(
                        p => new ViewModels.AttendanceSummaryExport()
                        {
                            Leave = p.LV,
                            OnTime = p.PO,
                            Late = p.PL,
                            Absent = p.AB,
                            EmployeeCode = p.employee_code,
                            First_Name = p.first_name,
                            Last_Name = p.last_name,
                            Department = p.dept_name,
                            Designation = p.desg_name,
                            Institute = p.camp_name,
                            EarlyGone = p.PE,
                            Present = p.Present
                        }).ToList();
                }
                catch (System.Data.Entity.Core.EntityCommandExecutionException ex)
                {
                    toReturn = new List<ViewModels.AttendanceSummaryExport>();
                }
            }

            return count;
        }
        public static int getAllPersistentLogMatching(string lineManagerEmployeeCode,string search, string sortOrder, int start, int length, out List<ViewModels.ReportPersistentAttendanceLog> toReturn)
        {
            using (Context db = new Context())
            {
                int count = 0;
                toReturn = new List<ViewModels.ReportPersistentAttendanceLog>();

                Employee lineManager = db.employee.Where(m =>
                    m.active &&
                    m.access_group.name.Equals(DLL.Commons.Roles.ROLE_LM) &&
                    m.employee_code.Equals(lineManagerEmployeeCode)).FirstOrDefault();

                if (lineManager == null)
                    return toReturn.Count;

                string[] employeeCodes =
                    lineManager.Group.Employees.Select(p => p.employee_code).ToArray();

                try
                {
                    var attendance = db.persistent_attendance_log
                        .Where(p =>
                            p.active &&
                            employeeCodes.Contains(p.employee_code) 
                            );
                    count = attendance.Where(p =>
                            p.active &&
                            employeeCodes.Contains(p.employee_code) &&
                            search == null ||
                            search.Equals("") ||
                            employeeCodes.Contains(search.ToLower()) ||
                            db.employee.Where(e => e.active && p.employee_code.Contains(search.ToLower())).FirstOrDefault().first_name.Contains(search.ToLower()) ||
                            db.employee.Where(e => e.active && p.employee_code.Contains(search.ToLower())).FirstOrDefault().last_name.Contains(search.ToLower()) ||
                            p.terminal_in.ToLower().Contains(search.ToLower()) ||
                            p.terminal_out.ToLower().Contains(search.ToLower()) 
                            ).Count();

                    List<ViewModels.ReportPersistentAttendanceLogDD> temp = attendance
                        .Where(p =>
                            p.active &&
                            employeeCodes.Contains(p.employee_code) &&
                            search == null ||
                            search.Equals("") ||
                            p.employee_code.Contains(search.ToLower()) ||
                            db.employee.Where(e => e.active && e.employee_code.Equals(p.employee_code)).FirstOrDefault().first_name.Contains(search.ToLower()) ||
                            db.employee.Where(e => e.active && e.employee_code.Equals(p.employee_code)).FirstOrDefault().last_name.Contains(search.ToLower()) ||
                             p.terminal_in.ToLower().Contains(search.ToLower()) ||
                            p.terminal_out.ToLower().Contains(search.ToLower())
                            )
                        .Select(p =>
                            new ViewModels.ReportPersistentAttendanceLogDD()
                            {
                                id = p.PersistentAttendanceLogId,
                                employee_code = p.employee_code,
                                employee_first_name = db.employee.Where(e => e.active && e.employee_code.Equals(p.employee_code)).FirstOrDefault().first_name,
                                employee_last_name = db.employee.Where(e => e.active && e.employee_code.Equals(p.employee_code)).FirstOrDefault().last_name,
                                date = p.time_start.Value,
                                time_in = p.time_in.Value,
                                time_out = p.time_out.Value,
                                terminal_in = (p.terminal_in != null) ? p.terminal_in : "",
                                terminal_out = (p.terminal_out != null) ? p.terminal_out : ""
                            })
                        .SortBy(sortOrder).Skip(start).Take(length).ToList();

                    foreach (var log in temp)
                    {
                        toReturn.Add(new ViewModels.ReportPersistentAttendanceLog()
                        {
                            id = log.id,
                            employee_code = log.employee_code,
                            employee_first_name = log.employee_first_name,
                            employee_last_name = log.employee_last_name,
                            date = (log.date.HasValue) ? log.date.Value.Date.ToString("dd-MM-yyyy") : "",
                            time_in = (log.time_in.HasValue) ? log.time_in.Value.ToString("HH:mm:ss") : "",
                            time_out = (log.time_out.HasValue) ? log.time_out.Value.ToString("HH:mm:ss") : "",
                            terminal_in = (log.terminal_in != null) ? log.terminal_in : " ",
                            terminal_out = (log.terminal_out != null) ? log.terminal_out : " "
                        });

                    }
                }
                catch (System.Data.Entity.Core.EntityCommandExecutionException ex)
                {
                    toReturn = new List<ViewModels.ReportPersistentAttendanceLog>();
                }

                return count;
            }
        }
    }
}
