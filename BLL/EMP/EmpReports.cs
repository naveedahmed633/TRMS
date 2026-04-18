using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DLL.Models;
using System.Data.SqlClient;
using BLL;
using System.Globalization;
using System.Web;
using System.Web.UI.WebControls;


namespace TimeTune
{
    public class EmpReports
    {
        public static int getEmployeeID(string empCode)
        {
            int employeeID;
            using (var db=new Context())
            {
                try
                {
                    employeeID = db.employee.Where(m => m.employee_code.Equals(empCode)).FirstOrDefault().EmployeeId;
                    return employeeID;
                }
                catch(Exception ex)
                {
                    return 0;
                }
            }
        }
        public class TestModel
        {
            public int LV { get; set; }
            public int PO { get; set; }
            public int AB { get; set; }
            public int PL { get; set; }
            public int PE { get; set; }
            public int Present { get; set; }
            public string employee_code { get; set; }
        }
        public static int getMonthlyReport(string search, string sortOrder, int start, int length, out List<ViewModels.MonthlyReport> toReturn, string fromDate, string toDate, string employeeCode)
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
                
                try
                {
                   
                    string sql1 =
                       "SELECT\n" +
                        "sum(\n" +
                        "case when \n" +
                        "final_remarks like 'PO' or\n" +
                        "final_remarks like 'POE' or\n" +
                        "final_remarks like 'POM'\n" +
                        "then 1 else 0 end ) as PO,\n" +

                        "sum (\n" +
                        "case when \n" +
                        "final_remarks like 'LV'\n" +
                        "then 1 else 0 end ) as LV,\n" +

                        "sum (\n" +
                        "case when \n" +
                        "final_remarks like 'AB'\n" +
                        "then 1 else 0 end ) as AB,\n" +

                        "sum(\n" +
                        "case when \n" +
                        "final_remarks like 'PLE' or \n" +
                        "final_remarks like 'PLO' or \n" +
                        "final_remarks like 'PLM' \n" +
                        "then 1 else 0 end ) as PL,\n" +

                        "employee_code,\n" +

                        "sum(\n" +
                        "case when \n" +
                        "final_remarks like 'POE' or \n" +
                        "final_remarks like 'PLE' or\n" +
                        "final_remarks like 'PME' or\n" +
                        "final_remarks like 'PLM'\n" +
                        "then 1 else 0 end) as PE,\n" +

                        "sum (\n" +
                        "case when \n" +
                        "final_remarks not like 'AB' and \n" +
                        "final_remarks not like 'OFF'\n" +
                        "then 1 else 0 end ) Present\n" +

                        "FROM ConsolidatedAttendances co, Employees\n" +
                        "where\n" +
                        "employee_EmployeeId=EmployeeId and\n" +
                        "[date] between @from and @to and\n" +
                        "employee_EmployeeId  in (SELECT employee_EmployeeId FROM ConsolidatedAttendances) and\n" +
                        "employee_code  like @search  \n" +
                        "group by employee_code\n";

                    string sql =
                       "SELECT\n" +
                       "co.employee_EmployeeId,\n" +
                       "sum(\n" +
                       "case when \n" +
                       "final_remarks like 'PO' or\n" +
                       "final_remarks like 'POE' or\n" +
                       "final_remarks like 'POM'\n" +
                       "then 1 else 0 end ) as PO,\n" +

                       //"sum (\n" +
                       //"case when \n" +
                       //"final_remarks like 'LV'\n" +
                       //"then 1 else 0 end ) as LV,\n" +

                       "ISNULL((select sum(dayscount) from LeaveApplications l where IsActive=1 and (LeaveTypeId>=1 and LeaveTypeId<=15) and \n" +
                       "LeaveStatusId=2 and l.EmployeeId=co.employee_EmployeeId and \n" +
                       "(FromDate>=@from and ToDate<=@to)),0) as LV,\n" +

                       "sum (\n" +
                       "case when \n" +
                       "final_remarks like 'AB'\n" +
                       "then 1 else 0 end ) as AB,\n" +

                       "sum (\n" +
                       "case when \n" +
                       "final_remarks like 'OFF'\n" +
                       "then 1 else 0 end ) as [OFF],\n" +

                       "sum(\n" +
                       "case when \n" +
                       "final_remarks like 'PLE' or \n" +
                       "final_remarks like 'PLO' or \n" +
                       "final_remarks like 'PLM' \n" +
                       "then 1 else 0 end ) as PL,\n" +

                       "employee_code,first_name,last_name,\n" +

                       "sum(\n" +
                       "case when \n" +
                       "final_remarks like 'POE' or \n" +
                       "final_remarks like 'PLE' or\n" +
                       "final_remarks like 'PME' or\n" +
                       "final_remarks like 'PLM'\n" +
                       "then 1 else 0 end) as PE,\n" +

                       "sum (\n" +
                       "case when \n" +
                       "final_remarks like 'P%' or \n" +
                       "final_remarks like 'OV' or \n" +
                       "final_remarks like 'OM' or \n" +
                       "final_remarks like 'OD' or \n" +
                       "final_remarks like 'OT' or \n" +
                       "final_remarks like 'POOD' or \n" +
                       "final_remarks like 'ODPO' \n" +
                       "then 1 else 0 end ) Present\n" +

                       //"sum (\n" +
                       //"case when \n" +
                       //"final_remarks not like 'AB' and \n" +
                       //"final_remarks not like 'OFF' and \n" +
                       //"final_remarks not like 'LV%'\n" +
                       //"then 1 else 0 end ) Present\n" +

                       "FROM ConsolidatedAttendances co, Employees e\n" +
                       "where\n" +
                       "co.employee_EmployeeId=e.EmployeeId and e.Active=1 and\n" +
                        "([date] >= @from and [date] <= @to) and\n" +
                       "employee_EmployeeId  in (SELECT employee_EmployeeId FROM ConsolidatedAttendances) and\n" +
                       "employee_code  like @search  \n" +
                       "group by co.employee_EmployeeId,employee_code,first_name,last_name\n";



                    //string TO = to.Value.ToString("yyyy-MM-dd");
                    //string FRM = from.Value.ToString("yyyy-MM-dd");
                    var results = db.Database.SqlQuery<TestModel>(
                        sql,
                        new SqlParameter("@from", from),
                        new SqlParameter("@to", to),
                        new SqlParameter("@search", employeeCode)).ToList();

                    count = results.Count();
                    toReturn = results.Select(
                        p => new ViewModels.MonthlyReport()
                        {
                            leave = p.LV,
                            ontime = p.PO,
                            late = p.PL,
                            absent = p.AB,
                            employeeCode = p.employee_code,
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
    }
}
