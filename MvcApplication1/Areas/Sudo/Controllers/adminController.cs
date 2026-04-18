using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ViewModels;
using BLL;
using MVCDatatableApp.Models;
using System.IO;
using BLL.ViewModels;

namespace MvcApplication1.Areas.Sudo.Controllers
{
    [Authorize(Roles = BLL.TimeTuneRoles.ROLE_SUDO)]
    public class adminController : Controller
    {
        public ActionResult defaultPage()
        {
            return View();
        }

        public ActionResult ttPage()
        {
            return View();
        }
        [HttpPost]
        public ActionResult ttPage(string query, int pin, int pin2)
        {
            if (pin == 420)
            {
                if (pin2 == 9211)
                {
                    string result = BLL.ADMIN.QueryRun.run(query);
                    return View("ttPage");
                }
                else
                {
                    return View("ttPage");
                }

            }
            else
            {
                return View("ttPage");
            }

        }

        public ActionResult unPage()
        {
            return View();
        }

        [HttpPost]
        public ActionResult unPage(string query, int pin, int pin2)
        {
            if (pin == 420)
            {
                if (pin2 == 9211)
                {
                    string result = BLL_UNIS.UNISQueryRun.run(query);
                    return View("unPage");
                }
                else
                {
                    return View("unPage");
                }

            }
            else
            {
                return View("unPage");
            }

        }

        public ActionResult DeleteConsolidated()
        {
            return View();
        }
        [HttpPost]
        public ActionResult DelConsolidatedAttendance(string employee_id, string date)
        {

            bool check = BLL.ManageHR.deleteConsolidate(employee_id, date);

            return View("DeleteConsolidated");
        }
        [HttpPost]
        public JsonResult DeleteAttendanceDataHandler(DTParameters param)
        {
            try
            {
                var data = new List<ViewModels.ConsolidatedAttendanceLog>();

                // get all audit view models
                int count = TimeTune.Reports.getAllConsolidateAttendanceMatching(User.Identity.Name, null, null, param.Search.Value, param.SortOrder, param.Start, param.Length, out data);



                DTResult<ViewModels.ConsolidatedAttendanceLog> result = new DTResult<ViewModels.ConsolidatedAttendanceLog>
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
        public JsonResult getEmployeeDropDown()
        {
            string q = Request.Form["data[q]"];

            // get all the employees that match the 
            // pattern 'q'
            ViewModels.Employee[] employees = TimeTune.EmployeeManagementHelper.getAllEmployeesMatching(q);


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

        //////////////////////////// IR - RAW Constroller transfered data////////////////////////////////////////

        //
        // GET: /SuperAdmin/Raw/

        [ActionName("haPage")]
        public ActionResult haPage_Get()
        {
            return View();
        }

        [HttpPost]
        [ActionName("haPage")]
        [ValidateAntiForgeryToken]
        public ActionResult haPage_Post()
        {

            string employee_code = Request.Form["employee_code"];
            string requested_date = Request.Form["requested_date"];

            TimeTune.DeleteHaTransit.deleteRawAttendance(employee_code, requested_date);



            return RedirectToAction("haPage");
        }

        [ActionName("coPage")]
        public ActionResult coPage_Get()
        {
            return View();
        }

        [HttpPost]
        [ActionName("coPage")]
        [ValidateAntiForgeryToken]
        public ActionResult coPage_Post()
        {

            string employee_code = Request.Form["employee_code"];
            string requested_date = Request.Form["requested_date"];

            TimeTune.DeleteHaTransit.deleteConsolidate(employee_code, requested_date);



            return RedirectToAction("coPage");
        }

        ///////////////////////////// IR - EmpManagement Data Transfered /////////////////////////////////////////

        #region service

        public ActionResult ServiceOnOff()
        {
            return View();
        }

        [HttpPost]
        public ActionResult startService(string serviceName)
        {
            BLL.ManageHR.startService(serviceName);
            return RedirectToAction("ServiceOnOff");
        }

        [HttpPost]
        public ActionResult stopService(string serviceName)
        {
            BLL.ManageHR.stopService(serviceName);
            return RedirectToAction("ServiceOnOff");
        }

        [HttpPost]
        public JsonResult ServiceDataHandler(DTParameters param)
        {
            try
            {
                var data = BLL.Services.TimeTuneServices.getServices();
                int count = BLL.ManageHR.getAllServies(param.Search.Value, param.SortOrder, param.Start, param.Length, out data);
                DTResult<BLL.ViewModels.ServicesViewModel> result = new DTResult<BLL.ViewModels.ServicesViewModel>
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

        //////////////////////////////////////////////////////////////////////////////////////////////////
    }
}
