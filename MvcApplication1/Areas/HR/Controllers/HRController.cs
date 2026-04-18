using MVCDatatableApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ViewModels;

namespace MvcApplication1.Areas.HR.Controllers
{
    [Authorize(Roles = BLL.TimeTuneRoles.ROLE_HR)]
    public class HRController : Controller
    {
        public ActionResult Dashboard()
        {
            var dashboardData = GetDataForDashboard();

            //IR: check if new academic year started
            TimeTune.Dashboard.manageLeaveSessionForUsers(User.Identity.Name);

            UpdateLeavesCountSessions();

            return View(dashboardData);
        }

        public ActionResult DashboardEventsHolidays()
        {
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
        public JsonResult DashboardPAChartData()
        {
            float present = 0.0f, absent = 0.0f, leave = 0.0f, plaSum = 0.0f;
            float pPercent = 0.0f, lPercent = 0.0f, aPercent = 0.0f;

            ViewModels.Dashboard dashboard = new ViewModels.Dashboard();
            var elements = TimeTune.Dashboard.getDashboardValuesForHR();
            dashboard.dashboardElement = elements.dashboardElement;

            present = float.Parse(elements.dashboardElement.present.ToString());
            absent = float.Parse(elements.dashboardElement.absent.ToString());
            leave = float.Parse(elements.dashboardElement.leave.ToString());

            //present = 20;
            //absent = 3;
            //leave = 3;

            plaSum = present + leave + absent;
            if (plaSum != 0)
            {
                pPercent = (present / plaSum) * 100.0f;
                lPercent = (leave / plaSum) * 100.0f;
                aPercent = (absent / plaSum) * 100.0f;
            }

            List<DashboardDonutChart> dc = new List<DashboardDonutChart>();
            dc.Add(new DashboardDonutChart { label = "Present", value = pPercent, color = "#52bb56" });
            dc.Add(new DashboardDonutChart { label = "Leave", value = lPercent, color = "#fbef97" });
            dc.Add(new DashboardDonutChart { label = "Absent", value = aPercent, color = "#f76397" });

            return Json(new { data = dc });
        }

        [HttpPost]
        public JsonResult DashboardOLChartData()
        {
            float ontime = 0.0f, late = 0.0f, olSum = 0.0f;
            float oPercent = 0.0f, lPercent = 0.0f;

            ViewModels.Dashboard dashboard = new ViewModels.Dashboard();
            var elements = TimeTune.Dashboard.getDashboardValuesForHR();
            dashboard.dashboardElement = elements.dashboardElement;

            ontime = float.Parse(elements.dashboardElement.onTime.ToString());
            late = float.Parse(elements.dashboardElement.late.ToString());

            //ontime = 12;
            //late = 8;

            olSum = ontime + late;
            if (olSum != 0)
            {
                oPercent = (ontime / olSum) * 100.0f;
                lPercent = (late / olSum) * 100.0f;
            }

            List<DashboardDonutChart> dc = new List<DashboardDonutChart>();
            dc.Add(new DashboardDonutChart { label = "Present - On Time", value = oPercent, color = "#67d1f8" });
            dc.Add(new DashboardDonutChart { label = "Present - Late", value = lPercent, color = "#ffaa00" });

            return Json(new { data = dc });
        }

        //public ActionResult Testing()
        //{
        //    return View();
        //}

        public ActionResult ChangePassword()
        {
            return View();
        }
        public ActionResult ConfigurationManager()
        {
            return View();
        }

        [HttpPost]
        public string DashboardPALChartDataMyTeam()
        {
            string strPercent = "";

            float present = 0.0f, absent = 0.0f, leave = 0.0f, plaSum = 0.0f;
            float pPercent = 0.0f, lPercent = 0.0f, aPercent = 0.0f;

            float ontime = 0.0f, late = 0.0f, early = 0.0f, oleSum = 0.0f;
            float oPercent = 0.0f, tPercent = 0.0f, ePercent = 0.0f;

            var elements = TimeTune.Dashboard.getAttendenceCounterForHR();

            present = float.Parse(elements.PresentCount.ToString());
            absent = float.Parse(elements.AbsentCount.ToString());
            leave = float.Parse(elements.LeaveCount.ToString());

            plaSum = present + leave + absent;
            if (plaSum != 0)
            {
                pPercent = (present / plaSum) * 100.0f;
                lPercent = (leave / plaSum) * 100.0f;
                aPercent = (absent / plaSum) * 100.0f;
            }

            ontime = float.Parse(elements.OnTimeCount.ToString());
            late = float.Parse(elements.LateCount.ToString());
            early = float.Parse(elements.EarlyCount.ToString());

            oleSum = ontime + late + early;
            if (oleSum != 0)
            {
                oPercent = (ontime / oleSum) * 100.0f;
                tPercent = (late / oleSum) * 100.0f;
                ePercent = (early / oleSum) * 100.0f;
            }

            //List<DashboardDonutChart> dc = new List<DashboardDonutChart>();
            //dc.Add(new DashboardDonutChart { label = "Present", value = pPercent, color = "#52bb56" });
            //dc.Add(new DashboardDonutChart { label = "Leave", value = lPercent, color = "#fbef97" });
            //dc.Add(new DashboardDonutChart { label = "Absent", value = aPercent, color = "#f76397" });

            strPercent = "P=" + pPercent + ",L=" + lPercent + ",A=" + aPercent + ",O=" + oPercent + ",T=" + tPercent + ",E=" + ePercent;

            return strPercent;

            //return Json(new { data = dc });
        }

    
        public ActionResult MyTeam()
        {
            ViewModels.DashboardManager dashboard = new ViewModels.DashboardManager();
            //var model = TimeTune.Dashboard.getSupervisor(User.Identity.Name);
            TimeTune.Dashboard.ReturnData model = new TimeTune.Dashboard.ReturnData();
            model.dashboard = TimeTune.Dashboard.getSupervisor(User.Identity.Name);
            model.totalActive = TimeTune.Dashboard.getTotalActiveEmp();
            model.totalRegistered = TimeTune.Dashboard.getTotalRegisteredEmp();
            model.totalRegCards = BLL_UNIS.UNISQueryRun.getALLCardsCount();
            model.totalRegFingers = BLL_UNIS.UNISQueryRun.getALLFingersCount();

            model.totalPresent = TimeTune.Dashboard.getTotalPresentToday();
            model.today_Absents = model.totalRegistered - model.totalPresent; // TimeTune.Dashboard.getTodayAbsents();
            model.today_EarlyOut = TimeTune.Dashboard.getTodayEarlyOut();
            model.today_Late = TimeTune.Dashboard.getTodayLate();
            model.today_Leaves = TimeTune.Dashboard.getTodayLeaves();
            model.today_OnTime = TimeTune.Dashboard.getTodayOnTime();
            model.today_OnlineDevices = BLL_UNIS.UNIS_Reports.getOnlineDevicesCount();

            TimeTune.Dashboard.getTotalPresentToday();
            return View(model);
        }

        [HttpPost]
        public JsonResult DataHandler(DTParameters param)
        {
            try
            {
                var dtsource = new List<Employee>();

                // get all employee view models
                int count = TimeTune.Dashboard.searchEmployeeForHRTeam(param.Search.Value, param.SortOrder, User.Identity.Name, param.Start, param.Length, out dtsource);

                //List<Employee> data = ResultSet.GetResult(param.SortOrder, param.Start, param.Length, dtsource);

                //int count = ResultSet.Count(param.Search.Value, dtsource);

                DTResult<Employee> result = new DTResult<Employee>
                {
                    draw = param.Draw,
                    data = dtsource,
                    recordsFiltered = count,
                    recordsTotal = count
                };
                return Json(result);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
                return Json(new { error = ex.Message });

            }
        }

        [HttpPost]
        public JsonResult VerfiyDataHandler(string empId)
        {
            try
            {
                var dtsource = new List<Employee>();
                var data = TimeTune.User_Permission.verfiyReturn(empId);

                return Json(data);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
                return Json(new { error = ex.Message });
            }
        }

        public void UpdateLeavesCountSessions()
        {
            //////////////////////leaves counter - get all employee view models /////////////////////////////////////////
            int session_year = DateTime.Now.Year;
            var data = new List<ViewModels.LeavesCountReportLog>();

            if (ViewModel.GlobalVariables.GV_EmployeeId != null && ViewModel.GlobalVariables.GV_EmployeeId != "")
            {
                System.Collections.ArrayList yearList = TimeTune.Attendance.getSessionYearsListByEmployeeId(int.Parse(ViewModel.GlobalVariables.GV_EmployeeId));
                session_year = int.Parse(yearList[0].ToString());
                TimeTune.Attendance.getLeavesCountReportByEmpCode(ViewModel.GlobalVariables.GV_EmployeeCode, session_year.ToString(), out data);

                if (data != null && data.Count > 0)
                {
                    foreach (ViewModels.LeavesCountReportLog item in data)
                    {
                        Session["GV_AllocatedSickLeaves"] = item.AllocatedSickLeaves;
                        Session["GV_AllocatedCasualLeaves"] = item.AllocatedCasualLeaves;
                        Session["GV_AllocatedAnnualLeaves"] = item.AllocatedAnnualLeaves;
                        Session["GV_AllocatedOtherLeaves"] = item.AllocatedOtherLeaves;

                        Session["GV_AvailedSickLeaves"] = item.AvailedSickLeaves;
                        Session["GV_AvailedCasualLeaves"] = item.AvailedCasualLeaves;
                        Session["GV_AvailedAnnualLeaves"] = item.AvailedAnnualLeaves;
                        Session["GV_AvailedOtherLeaves"] = item.AvailedOtherLeaves;
                    }
                }
                else
                {
                    Session["GV_AllocatedSickLeaves"] = 0;
                    Session["GV_AllocatedCasualLeaves"] = 0;
                    Session["GV_AllocatedAnnualLeaves"] = 0;
                    Session["GV_AllocatedOtherLeaves"] = 0;

                    Session["GV_AvailedSickLeaves"] = 0;
                    Session["GV_AvailedCasualLeaves"] = 0;
                    Session["GV_AvailedAnnualLeaves"] = 0;
                    Session["GV_AvailedOtherLeaves"] = 0;
                }
            }
            ////////////////////////////////////////////////////////////
        }
    }
}
