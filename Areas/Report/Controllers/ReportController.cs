using BLL.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ViewModels;

namespace MvcApplication1.Areas.Report.Controllers
{
    [Authorize(Roles = BLL.TimeTuneRoles.ROLE_REPORT)] 
    public class ReportController : Controller
    {
        //
        // GET: /Report/Report/

        public ActionResult Dashboard()
        {
            //ViewModels.Dashboard dashboard = TimeTune.Dashboard.getDashboardValues(User.Identity.Name);


            var dashboardData = GetDataForDashboard();


            return View(dashboardData);
        }

        private DashboardData GetDataForDashboard()
        {
            string strAttendanceChartData = "", strLeavesChartData = "";
            ViewModels.DashboardData dashboardData = new ViewModels.DashboardData();
            var elements = TimeTune.Dashboard.getDashboardValues(User.Identity.Name);
            dashboardData.dashboardElement = elements.dashboardElement;

            if (elements.dashboardElement != null)
            {
                decimal sumPresence = 0, sumAvaiabilityPunc = 0, sumAvaiabilityExit = 0;
                sumPresence = elements.dashboardElement.present + elements.dashboardElement.leave + elements.dashboardElement.absent;
                sumAvaiabilityPunc = elements.dashboardElement.onTime + elements.dashboardElement.late;
                sumAvaiabilityExit = elements.dashboardElement.onTime + elements.dashboardElement.earlyGone;

                decimal perPresent = 0, perLeave = 0, perAbsent = 0;
                decimal perOnTime = 0, perLate = 0, perOutTime = 0, perEarlyGone = 0;

                if (sumPresence != 0)
                {
                    perPresent = Math.Round(Math.Floor(elements.dashboardElement.present / sumPresence * 100));
                    perLeave = Math.Round(Math.Floor(elements.dashboardElement.leave / sumPresence * 100));
                    perAbsent = Math.Round(Math.Ceiling(elements.dashboardElement.absent / sumPresence * 100));
                }

                if (sumAvaiabilityPunc != 0)
                {
                    perOnTime = Math.Round(Math.Floor(elements.dashboardElement.onTime / sumAvaiabilityPunc * 100));
                    perLate = Math.Round(Math.Floor(elements.dashboardElement.late / sumAvaiabilityPunc * 100));
                }

                if (sumAvaiabilityExit != 0)
                {
                    perOutTime = Math.Round(Math.Floor(elements.dashboardElement.onTime / sumAvaiabilityExit * 100));
                    perEarlyGone = Math.Round(Math.Ceiling(elements.dashboardElement.earlyGone / sumAvaiabilityExit * 100));
                }

                strAttendanceChartData = "P=" + perPresent + ",L=" + perLeave + ",A=" + perAbsent + ",O=" + perOnTime + ",T=" + perLate + ",U=" + perOutTime + ",E=" + perEarlyGone;
            }

            dashboardData.dashboardAttendanceChart = strAttendanceChartData;

            var leaves = TimeTune.Dashboard.getDashboardLeavesForHR();
            if (leaves != null && leaves.Count > 0)
            {
                foreach (var l in leaves)
                {
                    strLeavesChartData += l.Applied + "," + l.Approved + "," + l.Rejected + ";";
                }
                strLeavesChartData = strLeavesChartData.TrimEnd(';');
            }
            dashboardData.dashboardLeavesChart = strLeavesChartData;

            //events list
            var events = TimeTune.Dashboard.getDashboardEventsForHR();
            dashboardData.dashboardEvents = events;

            //holidays list
            var holidays = TimeTune.Dashboard.getDashboardHolidaysForHR();
            dashboardData.dashboardHolidays = holidays;

            return dashboardData;
        }

      

        [HttpPost]
        public ActionResult Dashboard(int department_id)
        {
            ViewModels.Dashboard dashboard = TimeTune.Dashboard.getDashboardValues(User.Identity.Name,department_id);
            return View(dashboard);
        }
        [HttpPost]
        public JsonResult DepartmentdDataHandler()
        {
            string q = Request.Form["data[q]"];

            // get all the employees that match the 
            // pattern 'q'
            ViewModels.Department[] department = TimeTune.EmployeeManagementHelper.getAllDepartmentsMatching(q);


            ChosenAutoCompleteResults[] toSend = new ChosenAutoCompleteResults[department.Length];
            for (int i = 0; i < department.Length; i++)
            {
                toSend[i] = new ChosenAutoCompleteResults();

                toSend[i].id = department[i].id.ToString();
                toSend[i].text = department[i].name;
            }

            var toReturn = Json(new
            {
                q = q,
                results = toSend
            });

            return toReturn;
        }

        public ActionResult ChangePassword()
        {
            return View();
        }

    }
}
