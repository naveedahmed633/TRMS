using MVCDatatableApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ViewModels;
using TimeTune;
using Rotativa;
using BLL.ViewModels;

namespace MvcApplication1.Areas.HR.Controllers
{
    [Authorize(Roles = BLL.TimeTuneRoles.ROLE_HR)]
    public class ReportsController : Controller
    {
        #region MonthlyTimeSheetReport
        public ActionResult GenerateReport()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult MonthlyTimeSheet()
        {
            int employeeID;
            if (!int.TryParse(Request.Form["employee_id"], out employeeID))
                return RedirectToAction("GenerateReport");

            string month = Request.Form["month"];

            BLL.PdfReports.MonthlyTimeSheet reportMaker = new BLL.PdfReports.MonthlyTimeSheet();

            BLL.PdfReports.MonthlyTimeSheetData toRender = reportMaker.getReport(employeeID, month);

            if(toRender == null)
                return RedirectToAction("GenerateReport");

            return new Rotativa.ViewAsPdf("MonthlyTimeSheet", toRender) { FileName = "report.pdf" };
            
        }
        #endregion

        #region SummaryReport

        public ActionResult AttendanceSummary()
        {
            return View();
        }

        public class CustomDTParameters : DTParameters
        {
            public string from_date { get; set; }
            public string to_date { get; set; }
        }

        [HttpPost]
        public JsonResult AttendanceSummaryDataHandler(CustomDTParameters param)
        {
            try
            {


                var data = new List<ViewModels.MonthlyReport>();
                int count = TimeTune.Reports.getMonthlyReport(param.Search.Value, param.SortOrder, param.Start, param.Length, out data, param.from_date, param.to_date);

                DTResult<ViewModels.MonthlyReport> result = new DTResult<ViewModels.MonthlyReport>
                {
                    draw = param.Draw,
                    data = data,
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

        #endregion

        #region ViewConsolidatedAttendance

        public ActionResult ConsolidateAttendance()
        {
            return View();
        }

        public class ConsolidatedReportTable : DTParameters
        {
            public string employee_id { get; set; }
            //public string month { get; set; }
            public string from_date { get; set; }
            public string to_date { get; set; }
        }
        [HttpPost]
        public JsonResult graphForConsolidated (ConsolidatedReportTable param)
        {
            try
            {
                ViewModels.Dashboard dashboard = TimeTune.Dashboard.graphForConsolidated(param.employee_id,param.from_date,param.to_date);
                return Json(dashboard);
            }
            catch(Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
                return Json(new { error = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult ConsolidatedReportDataHandler(ConsolidatedReportTable param)
        {
            try
            {
                var data = new List<ConsolidatedAttendanceLog>();

                // get all employee view models
                int count = TimeTune.Reports.getAllConsolidateAttendanceMatching(param.employee_id, param.from_date, param.to_date, param.Search.Value, param.SortOrder, param.Start, param.Length, out data);

                DTResult<ConsolidatedAttendanceLog> result = new DTResult<ConsolidatedAttendanceLog>
                {
                    draw = param.Draw,
                    data = data,
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


        #endregion

        #region Department Wise Report
        public ActionResult DepartmentReport()
        {
            return View();
        }
        public class ConsolidatedDepartmentReportTable : DTParameters
        {
            public string department_id { get; set; }
            public string designation_id { get; set; }
            public string function_id { get; set; }
            public string location_id { get; set; }
            //public string month { get; set; }
            public string from_date { get; set; }
            public string to_date { get; set; }
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
        [HttpPost]
        public JsonResult DesignationDataHandler()
        {
            string q = Request.Form["data[q]"];

            // get all the employees that match the 
            // pattern 'q'
            ViewModels.Designation[] designation = TimeTune.EmployeeManagementHelper.getAllDesignationsMatching(q);


            ChosenAutoCompleteResults[] toSend = new ChosenAutoCompleteResults[designation.Length];
            for (int i = 0; i < designation.Length; i++)
            {
                toSend[i] = new ChosenAutoCompleteResults();

                toSend[i].id = designation[i].id.ToString();
                toSend[i].text = designation[i].name;
            }

            var toReturn = Json(new
            {
                q = q,
                results = toSend
            });

            return toReturn;
        }
        [HttpPost]
        public JsonResult LocationDataHandler()
        {
            string q = Request.Form["data[q]"];

            // get all the employees that match the 
            // pattern 'q'
            ViewModels.Location[] location = TimeTune.EmployeeManagementHelper.getAllLocationsMatching(q);


            ChosenAutoCompleteResults[] toSend = new ChosenAutoCompleteResults[location.Length];
            for (int i = 0; i < location.Length; i++)
            {
                toSend[i] = new ChosenAutoCompleteResults();

                toSend[i].id = location[i].id.ToString();
                toSend[i].text = location[i].name;
            }

            var toReturn = Json(new
            {
                q = q,
                results = toSend
            });

            return toReturn;
        }
        [HttpPost]
        public JsonResult FunctionDataHandler()
        {
            string q = Request.Form["data[q]"];

            // get all the employees that match the 
            // pattern 'q'
            ViewModels.Function[] function = TimeTune.EmployeeManagementHelper.getAllFunctionsMatching(q);


            ChosenAutoCompleteResults[] toSend = new ChosenAutoCompleteResults[function.Length];
            for (int i = 0; i < function.Length; i++)
            {
                toSend[i] = new ChosenAutoCompleteResults();

                toSend[i].id = function[i].id.ToString();
                toSend[i].text = function[i].name;
            }

            var toReturn = Json(new
            {
                q = q,
                results = toSend
            });

            return toReturn;
        }

        [HttpPost]
        public JsonResult DepartmentReportHandler(ConsolidatedDepartmentReportTable param)
        {
            try
            {
                var data = new List<ConsolidatedAttendanceLog>();
                int count = TimeTune.Reports.getAllDepartmentLogs(param.department_id, param.designation_id, User.Identity.Name , param.location_id, param.from_date, param.to_date, param.Search.Value, param.SortOrder, param.Start, param.Length, out data);

                DTResult<ViewModels.ConsolidatedAttendanceLog> result = new DTResult<ViewModels.ConsolidatedAttendanceLog>
                {
                    draw = param.Draw,
                    data = data,
                    recordsFiltered = count,
                    recordsTotal = count
                };
              //  PdfReport(result);
               
                return Json(result);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
                return Json(new { error = ex.Message });

            }

        }
        [HttpPost]
        public JsonResult graph(string dept, string des, string loc, string from, string to)
        {
            ViewModels.Dashboard dashboard = TimeTune.Dashboard.getGraphValues(dept, des, User.Identity.Name, loc, from, to);
            return Json(dashboard);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ConsolidatedFilteredReports()
        {
            //string department_id, string designation_id, string location_id, string function_id, string from_date, string to_date
            //Request.Form["department_id"]
            //Request.Form["from_date"];
            int depart_id,des_id,fun_id,loc_id;
            if (!int.TryParse(Request.Form["department_id"], out depart_id))
                depart_id = -1;
            if (!int.TryParse(Request.Form["designation_id"], out des_id))
                des_id = -1;
            if (!int.TryParse(Request.Form["function_id"], out fun_id))
                fun_id = -1;
            if (!int.TryParse(Request.Form["location_id"], out loc_id))
                loc_id = -1;

            string from_date = Request.Form["from_date"];
            string to_date = Request.Form["to_date"];

            if (from_date == null && to_date == null)
            {
                return RedirectToAction("GenerateReport");
            }

            BLL.ViewModels.FilteredAttendanceReport reportMaker = new BLL.ViewModels.FilteredAttendanceReport();

            reportMaker.logs = TimeTune.Reports.getAttendance(depart_id, des_id, loc_id, fun_id, from_date, to_date);

            if (reportMaker == null)
                return RedirectToAction("GenerateReport");
            return new Rotativa.ViewAsPdf("ConsolidatedFilteredReports", reportMaker) { FileName = "reports.pdf" };
        }
      


        #endregion

        #region Present and absent report
        public ActionResult PaReport()
        {
            return View();
        }
        public class PaReportTable : DTParameters
        {
            public string final_remarks { get; set; }
            public string from_date { get; set; }
            public string to_date { get; set; }
        }
        [HttpPost]
        public JsonResult PaReportDataHandler(PaReportTable param)
        {
            try
            {
                var data = new List<ViewModels.PaAttendanceLog>();

                // get all employee view models
                int count = TimeTune.Reports.getAllPaAttendanceMatching(param.final_remarks,param.from_date, param.to_date, param.Search.Value, param.SortOrder, param.Start, param.Length, out data);

                DTResult<PaAttendanceLog> result = new DTResult<PaAttendanceLog>
                {
                    draw = param.Draw,
                    data = data,
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
        #endregion
    }
}
