using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels
{
    //Dashboard for normal employee and HR
    public class Dashboard
    {
        public Dashboard()
        {
            dashboardElement = new DashboardElement();
        }
        public string headName { get; set; }
        public DashboardElement dashboardElement { get; set; }
        public List<EmployeeCount> employeeCount { get; set; }
        public string supervisorName { get; set; }
        public string name { get; set; }


    }
    //Dashboard element like present absent
    public class DashboardElement
    {
        public int onTime { get; set; }
        public int late { get; set; }
        public int off { get; set; }
        public int absent { get; set; }
        public int outTime { get; set; }
        public int earlyGone { get; set; }
        public int present { get; set; }
        public int leave { get; set; }

        public int onTimeLast { get; set; }
        public int lateLast { get; set; }
        public int absentLast { get; set; }
        public int offLast { get; set; }
        public decimal leaveLast { get; set; }
        public int outTimeLast { get; set; }
        public int earlyGoneLast { get; set; }
        public decimal presentLast { get; set; }
    }

    //Line Manager and Super Line Manager View Models
    public class DashboardManager
    {
        public DashboardManager()
        {
            employee = new Dashboard();
            lineManager = new Dashboard();
        }
        public string lineManagerName { get; set; }
        public Dashboard lineManager { get; set; }
        public Dashboard employee { get; set; }
        public List<EmployeeCount> employeeCount { get; set; }
    }

    public class DashboardManagerData
    {
        public DashboardManagerData()
        {
            employee = new Dashboard();
            lineManager = new Dashboard();
        }
        public string lineManagerName { get; set; }
        public Dashboard lineManager { get; set; }
        public Dashboard employee { get; set; }
        public List<EmployeeCount> employeeCount { get; set; }
        public string dashboardAttendanceChart { get; set; }
        public string dashboardLeavesChart { get; set; }
        public List<DashboardEvents> dashboardEvents { get; set; }
        public List<DashboardHolidays> dashboardHolidays { get; set; }
    }


    //DashboardData
    public class DashboardData
    {
        public DashboardData()
        {
            dashboardElement = new DashboardElement();
        }

        public string headName { get; set; }
        public string supervisorName { get; set; }
        public string name { get; set; }
        public string dashboardAttendanceChart { get; set; }
        public string dashboardLeavesChart { get; set; }
        public DashboardElement dashboardElement { get; set; }
        public List<DashboardEvents> dashboardEvents { get; set; }
        public List<DashboardHolidays> dashboardHolidays { get; set; }
    }

    // Get events 
    public class DashboardEvents
    {
        public string employee_code { get; set; }
        public string employee_name { get; set; }
        public DateTime event_actual_date { get; set; }
        public string event_date { get; set; }
        public string event_title { get; set; }
    }

    // Get holidays 
    public class DashboardHolidays
    {
        public DateTime holiday_actual_date { get; set; }
        public string holiday_date { get; set; }
        public string holiday_title { get; set; }
    }


    // Get the employee and their present and absent status 
    public class EmployeeCount
    {
        public string employee { get; set; }
        public int status { get; set; }
    }

    public class DashboardDonutChart
    {
        public string label { get; set; }
        public float value { get; set; }
        public string color { get; set; }
    }

    public class AttendenceCounter
    {
        public int PresentCount { get; set; }
        public int LeaveCount { get; set; }
        public int AbsentCount { get; set; }
        public int OnTimeCount { get; set; }
        public int LateCount { get; set; }
        public int EarlyCount { get; set; }
    }

    public class DashboardLeavesCounter
    {
        public int Applied { get; set; }
        public int Approved { get; set; }
        public int Rejected { get; set; }
    }
}
