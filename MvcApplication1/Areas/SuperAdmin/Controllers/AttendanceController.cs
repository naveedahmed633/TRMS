using BLL.ViewModels;
using MVCDatatableApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ViewModels;

namespace MvcApplication1.Areas.SuperAdmin.Controllers
{
    [Authorize(Roles = BLL.TimeTuneRoles.ROLE_SUPER_USER)]
    public class AttendanceController : Controller
    {
        //
        // GET: /SuperAdmin/Attendance/

        public ActionResult CorrectAttendance()
        {
            return View();
        }


        #region RawAttendance
        
        public ActionResult ViewRawAttendance()
        {

            return View();
        }

        [HttpPost]
        public JsonResult RawAttendanceDataHandler(DTParameters param)
        {
            try
            {
                var data = new List<ReportRawAttendanceLog>();
                
                // get all employee view models
                int count = TimeTune.Reports.getAllRawAttendanceLogsMatching(param.Search.Value, param.SortOrder, param.Start, param.Length, out data);



                DTResult<ReportRawAttendanceLog> result = new DTResult<ReportRawAttendanceLog>
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


        #region ViewAttendance
        public ActionResult ViewConsolidate()
        {
            //added by IR
            return RedirectToAction("ConfigurationManager", "SU");

            //commented by IR
            ////return View();
        }
        public class ConsolidatedReportTable : DTParameters
        {
            public string employee_id { get; set; }
            //public string month { get; set; }
            public string from_date { get; set; }
            public string to_date { get; set; }
        }
        [HttpPost]
        public JsonResult ViewConsolidate(ConsolidatedReportTable param)
        {
            try
            {


                var data = new List<ConsolidatedAttendanceLog>();

                // get all employee view models
                int count = TimeTune.Reports.getAllContractualStaffConsolidate(param.employee_id, param.from_date, param.to_date, param.Search.Value, param.SortOrder, param.Start, param.Length, out data);

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



        [HttpPost]
        public JsonResult getEmployee()
        {
            string q = Request.Form["data[q]"];

            // get all the employees that match the 
            // pattern 'q'
            BLL.ViewModels.VM_ContractualStaff[] employees = TimeTune.EmployeeManagementHelper.getContractualStaffMatching(q);


            ChosenAutoCompleteResults[] toSend = new ChosenAutoCompleteResults[employees.Length];
            for (int i = 0; i < employees.Length; i++)
            {
                toSend[i] = new ChosenAutoCompleteResults();

                toSend[i].id = employees[i].ContractualStaffId.ToString();
                toSend[i].text = employees[i].employee_code;
            }

            var toReturn = Json(new
            {
                q = q,
                results = toSend
            });

            return toReturn;
        }
        #endregion
    }
}
