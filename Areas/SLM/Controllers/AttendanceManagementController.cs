using BLL.ViewModels;
using MVCDatatableApp.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TimeTune;
using ViewModels;

namespace MvcApplication1.Areas.SLM.Controllers
{
    [Authorize(Roles = BLL.TimeTuneRoles.ROLE_SLM)]
    public class AttendanceManagementController : Controller
    {

        #region ManualAttendance
        //
        // GET: /LM/AttendanceManagement/
        public ActionResult ManualAttendance(string error)
        {
            if (error != null)
            {
                ModelState.AddModelError("", error);
            }

            ViewModels.CreateManualAttendance manualAttendance = new CreateManualAttendance();
            manualAttendance.employee = TimeTune.SLMManualAttendance.getEmployeesBySLM(User.Identity.Name);
            
            //loading the employee code in view
            return View(manualAttendance);
        }

        [HttpPost]
        public ActionResult AddManualAttendance(ViewModels.ManualAttendance manualAttendance)
        {
            /// data for view model is coming in this parameter 
            /// just need the login employee code to save in manual attendance table that employee
            /// 

            string message = TimeTune.SLMManualAttendance.addManualAttendance(manualAttendance,User.Identity.Name);
            
            //Audit Trail
            var json = JsonConvert.SerializeObject(manualAttendance);
            AuditTrail.update(json,"Manual Attendance", User.Identity.Name);

            return RedirectToAction("ManualAttendance", new { Error = message });
        }

        #endregion

        # region ViewAttendance

        public ActionResult ViewAttendance()
        {
            
            var Attendance = TimeTune.Attendance.getPersistanceLog(User.Identity.Name);
            return View(Attendance);
        }


        #region Today's Attendance DataTable

        [HttpPost]
        public JsonResult PersistentDataHandler(DTParameters param)
        {
            try
            {
                var data = new List<ReportPersistentAttendanceLog>();

                // get all employee view models
                int count = TimeTune.SLMReports.getAllPersistentLogMatching(
                    User.Identity.Name,
                    param.Search.Value,
                    param.SortOrder,
                    param.Start, param.Length, out data);


                DTResult<ReportPersistentAttendanceLog> result = new DTResult<ReportPersistentAttendanceLog>
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

        #endregion

        #region Future Manaul Attendance
        public ActionResult FutureManualAttendance(string result)
        {
            ViewBag.Message = result;
            ViewModels.CreateManualAttendance manualAttendance = new CreateManualAttendance();
            manualAttendance.employee = TimeTune.SLMManualAttendance.getEmployeesBySLM(User.Identity.Name);
            return View(manualAttendance);  
        }

        [HttpPost]
        public ActionResult FutureManualAttendance(string employee_code, string time_in_from, string time_in_to, string remarks)
        {
            // string message = TimeTune.ManualAttendance.addManualAttendance(manualAttendance);
            //var json = JsonConvert.SerializeObject(message);
            //AuditTrail.update(json, "Manual Attendance", int.Parse(User.Identity.Name));
            string message = BLL.ManageHR.addManualAttendance(time_in_from, time_in_to, employee_code, remarks);

            return RedirectToAction("FutureManualAttendance", new { result = message });
        }

        [HttpPost]
        public JsonResult getEmployeeDropDown()
        {
            string q = Request.Form["data[q]"];

            // get all the employees that match the 
            // pattern 'q'

            ViewModels.Employee[] employees = TimeTune.EmployeeManagementHelper.getAllEmployeesMatchingLM(q, User.Identity.Name);


            ChosenAutoCompleteResults[] toSend = new ChosenAutoCompleteResults[employees.Length];
            for (int i = 0; i < employees.Length; i++)
            {
                toSend[i] = new ChosenAutoCompleteResults();

                toSend[i].id = employees[i].id.ToString();
                toSend[i].text = employees[i].employee_code;
            }

            var toReturn = Json(new
            {
                q = q,
                results = toSend
            });

            return toReturn;
        }

        [HttpPost]
        public JsonResult FutureManualAttendanceDataHandler(DTParameters param)
        {
            try
            {

                var data = new List<BLL.ViewModels.ViewFutureManualAttendance>();

                // get all employee view models
                int count = BLL.ManageHR.getSLMManualAttendance(User.Identity.Name, param.Search.Value, param.SortOrder, param.Start, param.Length, out data);


                DTResult<BLL.ViewModels.ViewFutureManualAttendance> result = new DTResult<BLL.ViewModels.ViewFutureManualAttendance>
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
