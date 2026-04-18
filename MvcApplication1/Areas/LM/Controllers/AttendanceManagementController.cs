using BLL.ViewModels;
using MVCDatatableApp.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TimeTune;
using ViewModels;

namespace MvcApplication1.Areas.LM.Controllers
{
    [Authorize(Roles = BLL.TimeTuneRoles.ROLE_LM)]
    public class AttendanceManagementController : Controller
    {

        #region Today's Attendance DataTable
        [HttpPost]
        public JsonResult PersistentDataHandler(DTParameters param)
        {
            try
            {
                var data = new List<ReportPersistentAttendanceLog>();

                // get all employee view models
                int count = TimeTune.LmReports.getAllPersistentLogMatching(
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
                return Json(new { error = ex.Message + " The session has been timed out please try again" });

            }
        }

        #endregion


        #region Manaul Attendance
        public ActionResult ManualAttendance(string error)
        {
            if (error != null)
            {
                ModelState.AddModelError("", error);
            }

            ViewModels.CreateManualAttendance manualAttendance = new CreateManualAttendance();
            manualAttendance.employee = TimeTune.LMManualAttendance.getEmployeeBySupervisor(User.Identity.Name);

            return RedirectPermanent("/LM/LM/Dashboard");

            //loading the employee code in view
            //return View(manualAttendance);
        }

        [HttpPost]
        public ActionResult AddManualAttendance(ViewModels.ManualAttendance manualAttendance)
        {
            string strFromDate = "", strToDate = "";

            //if (manualAttendance.employee_code == null)
            //{
            //    return RedirectToAction("ManualAttendance", new { Error = "Invalid 'Employee Code'" });
            //}

            if (Request["time_in_from"] == null)
            {
                return RedirectToAction("ManualAttendance", new { Error = "Invalid 'From Date'" });
            }

            if (Request["time_in_to"] == null)
            {
                return RedirectToAction("ManualAttendance", new { Error = "Invalid 'To Date'" });
            }

            strFromDate = DateTime.ParseExact(Request["time_in_from"].ToString(), "dd-MM-yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");
            strToDate = DateTime.ParseExact(Request["time_in_to"].ToString(), "dd-MM-yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");

            manualAttendance.time_in_from = DateTime.ParseExact(strFromDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            manualAttendance.time_in_to = DateTime.ParseExact(strToDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);

            //data for view model is coming in this parameter 
            ///just need the login employee code to save in manual attendance table that employee
            /// 

            string message = TimeTune.LMManualAttendance.addManualAttendance(manualAttendance,User.Identity.Name);
            
            //Audit Trail
            var json = JsonConvert.SerializeObject(manualAttendance);
            AuditTrail.update(json,"Manual Attendance", User.Identity.Name);

            return RedirectToAction("ManualAttendance", new { Error = message });
        }

        public ActionResult ViewAttendance()
        {
            
            var Attendance = TimeTune.Attendance.getPersistanceLog(User.Identity.Name);
            return View(Attendance);
        }

        #endregion


        #region Future Manaul Attendance
        public ActionResult FutureManualAttendance(string result)
        {

            ViewModels.CreateManualAttendance manualAttendance = new CreateManualAttendance();
            manualAttendance.employee = TimeTune.LMManualAttendance.getEmployeeBySupervisor(User.Identity.Name);

            //loading the employee code in view
            ViewBag.Message = result;
            return View(manualAttendance);
        }

        [HttpPost]
        public ActionResult FutureManualAttendance(string employee_code, string time_in_from, string time_in_to, string remarks)
        {
            string strFromDate = "", strToDate = "";

            //if (employee_code == null)
            //{
            //    return RedirectToAction("FutureManualAttendance", new { Error = "Invalid 'Employee Code'" });
            //}

            if (time_in_from == null)
            {
                return RedirectToAction("FutureManualAttendance", new { Error = "Invalid 'From Date'" });
            }

            if (time_in_to == null)
            {
                return RedirectToAction("FutureManualAttendance", new { Error = "Invalid 'To Date'" });
            }

            strFromDate = DateTime.ParseExact(time_in_from, "dd-MM-yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");
            strToDate = DateTime.ParseExact(time_in_to, "dd-MM-yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");

            // string message = TimeTune.ManualAttendance.addManualAttendance(manualAttendance);
            //var json = JsonConvert.SerializeObject(message);
            //AuditTrail.update(json, "Manual Attendance", int.Parse(User.Identity.Name));
            string message = BLL.ManageHR.addManualAttendance(strFromDate, strToDate, employee_code, remarks);

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
                int count = BLL.ManageHR.getLmManualAttendance(User.Identity.Name, param.Search.Value, param.SortOrder, param.Start, param.Length, out data);
                data = data.OrderByDescending(o => o.Id).ToList();

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
                return Json(new { error = ex.Message + " The session has been timed out please try again" });

            }
        }

        [HttpPost]
        public JsonResult DeleteLeave(int id)
        {
            int response = 0;

            try
            {
                // get all employee view models
                response = BLL.ManageHR.DeleteLeave(id);

                return Json(new { data = response });
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
