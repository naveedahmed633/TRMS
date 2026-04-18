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
    public class SLMReports
    {
        public class TestModel
        {
            public int LV { get; set; }
            public int PO { get; set; }
            public int AB { get; set; }
            public int PL { get; set; }
            public int PE { get; set; }
            public int Present { get; set; }
            public string first_name { get; set; }
            public string last_name { get; set; }
            public string employee_code { get; set; }
        }
        public static int getMonthlyReportSLM(string search, string sortOrder, int start, int length, out List<ViewModels.MonthlyReport> toReturn, string fromDate, string toDate ,string slmCode)
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
                string SLMId;
                try
                {
                   SLMId= db.employee.Where(m => m.employee_code.Equals(slmCode)).FirstOrDefault().EmployeeId.ToString();

                    string sql =
                        "SELECT\n" +
                    "sum(case when final_remarks like 'PO'or final_remarks like 'POM' then 1 else 0 end ) as PO, sum (case when final_remarks like 'LV' then 1 else 0 end ) as LV, sum (case when final_remarks like 'AB' then 1 else 0 end ) as AB\n" +
                    ",sum( case when final_remarks like 'PLE' or final_remarks like 'PLO' or final_remarks like 'PLM' then 1 else 0 end ) as PL ,employee_code,first_name,last_name,\n" +
                    "sum(case when final_remarks like 'POE' or final_remarks like 'PLE' then 1 else 0 end) as PE,\n" +
                    "sum (case when final_remarks not like 'AB' and final_remarks not like 'OFF'  then 1 else 0 end ) Present\n" +
                    "FROM ConsolidatedAttendances co, Employees\n" +
                    "where \n" +
                    "employee_EmployeeId=EmployeeId and\n" +
                    "[date] between @from and @to and\n" +
                    "(employee_code like @search or first_name like @search or last_name like @search) and\n" +
                    "(employee_EmployeeId  in \n" +
                    "(\n" +
                        "SELECT \n" +
                        "tags.taggedEmployee_EmployeeId\n" +
                        "from SLMs tags\n" +
                        "where\n" +
                        "tags.superLineManager_EmployeeId = @slmID	\n" +
                    ") or\n" +
                    "employee_EmployeeId in (@slmID))\n" +
                    "group by employee_code,first_name,last_name";




                    //string TO = to.Value.ToString("yyyy-MM-dd");
                    //string FRM = from.Value.ToString("yyyy-MM-dd");
                    var results = db.Database.SqlQuery<TestModel>(
                        sql,
                        new SqlParameter("@from", from),
                        new SqlParameter("@to", to),
                        new SqlParameter("@slmID", SLMId),
                        new SqlParameter("@search", (search == null) ? "%" : "%" + search + "%")).ToList();

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
        public static int getAllPersistentLogMatching(string slmEmployeeCode, string search, string sortOrder, int start, int length, out List<ViewModels.ReportPersistentAttendanceLog> toReturn)
        {
            using (Context db = new Context())
            {
                int count = 0;


                toReturn = new List<ViewModels.ReportPersistentAttendanceLog>();


                Employee slm = db.employee.Where(m =>
                    m.active &&
                    m.access_group.name.Equals(DLL.Commons.Roles.ROLE_SLM) &&
                    m.employee_code.Equals(slmEmployeeCode)).FirstOrDefault();

                if (slm == null)
                    return toReturn.Count;

                string[] employeeCodes =
                    db.super_line_manager_tagging.Where(m =>
                    m.superLineManager.EmployeeId.Equals(slm.EmployeeId) &&
                    m.taggedEmployee.active).Select(p => p.taggedEmployee.employee_code).ToArray();


                try
                {
                    var attendance = db.persistent_attendance_log
                       .Where(p =>
                           p.active &&
                           employeeCodes.Contains(p.employee_code) 
                           );
                    count = attendance
                        .Where(p =>
                            p.active &&
                            employeeCodes.Contains(p.employee_code) &&
                            search == null ||
                            search.Equals("") ||
                            p.employee_code.Contains(search.ToLower()) ||
                            db.employee.Where(e => e.active && e.employee_code.Equals(p.employee_code)).FirstOrDefault().first_name.ToLower().Contains(search.ToLower()) ||
                            db.employee.Where(e => e.active && e.employee_code.Equals(p.employee_code)).FirstOrDefault().last_name.ToLower().Contains(search.ToLower()) ||
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
                                terminal_in=p.terminal_in,
                                terminal_out=p.terminal_out
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
                            date = (log.date.HasValue) ? log.date.Value.Date.ToString("dd-MMMM-yyyy") : "",
                            time_in = (log.time_in.HasValue) ? log.time_in.Value.ToString("HH:mm:ss") : "",
                            time_out = (log.time_out.HasValue) ? log.time_out.Value.ToString("HH:mm:ss") : "",
                            terminal_in=(log.terminal_in!=null)?log.terminal_in:" ",
                            terminal_out=(log.terminal_out!=null)?log.terminal_out:" "
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
