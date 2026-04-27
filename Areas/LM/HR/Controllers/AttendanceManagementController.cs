using MVCDatatableApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ViewModels;
using TimeTune;
using Newtonsoft.Json;
using BLL.ViewModels;
using System.IO;

namespace MvcApplication1.Areas.HR.Controllers
{
    [Authorize(Roles = BLL.TimeTuneRoles.ROLE_HR)]
    public class AttendanceManagementController : Controller
    {
        //
        // GET: /AttendanceManagement/

        #region ManualAttendance
        public JsonResult DataHandler(DTParameters param)
        {
            try
            {



                var data = new List<ConsolidatedAttendanceLog>();

                // get all employee view models
                int count = TimeTune.Reports.getAllConsolidateAttendanceMatching(null,null,null,param.Search.Value, param.SortOrder, param.Start, param.Length, out data);

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

        public ActionResult ManualAttendance(string error)
        {
            if (error != null)
            {
                ModelState.AddModelError("", error);
            }

            ViewModels.CreateManualAttendance manualAttendance = new CreateManualAttendance();
            manualAttendance.employee = EmployeeCrud.getAll();
            //loading the employee code in view
            return View(manualAttendance);
        }
       [HttpPost]
       public ActionResult AddManualAttendance(ViewModels.ManualAttendance manualAttendance)
        {
            //data for view model is coming in this parameter 
            ///just need the login employee code to save in manual attendance table that employee
            /// 
           string message = TimeTune.ManualAttendance.addManualAttendance(manualAttendance);
           var json = JsonConvert.SerializeObject(manualAttendance);
           AuditTrail.update(json,"Manual Attendance",int.Parse(User.Identity.Name));
           return RedirectToAction("ManualAttendance", new { Error = message });
        }


       [HttpPost]
       [ValidateAntiForgeryToken]
       public ActionResult UploadFileManualAttendance(HttpPostedFileBase file)
       {
           string result = null;
           if (file != null && file.ContentLength > 0)
           {
               //try
               //{
               string path = Path.Combine(Server.MapPath("~/Uploads"),
                                  "ReprocessAttendance.csv");
               file.SaveAs(path);


               List<string> content = new List<string>();


               using (StreamReader sr = new StreamReader(path))
               {
                   while (sr.Peek() >= 0)
                   {
                       content.Add(sr.ReadLine());
                   }
               }
               result = TimeTune.ManualAttendance.uploadManualAttendance(content);
               return RedirectToAction("ManualAttendance", "AttendanceManagement", new { result = "Successful" });
               //return JavaScript("displayToastrSuccessfull()");
               //}
               //catch (Exception ex)
               //{
               //    return RedirectToAction("ManageEmployee", "EmployeeManagement", new { result = "Failed" });
               //}

           }
           return RedirectToAction("ManualAttendance", "AttendanceManagement", new { result = "Select File first" });
       }

        #endregion


       public ActionResult ViewAttendance()
        {
            return View();
        }
        [HttpPost]
        public JsonResult PersistentDataHandler(DTParameters param)
        {
            try
            {
                var data = new List<ReportPersistentAttendanceLog>();

                // get all employee view models
                int count = TimeTune.Reports.getAllPersistentLogMatching(param.Search.Value, param.SortOrder, param.Start, param.Length, out data);


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
        
        public ActionResult UploadRawAttendance()
        {
            return View();
        }


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
                int count = TimeTune.Reports.getAllRawAttendanceLogsMatching(param.Search.Value, param.SortOrder, param.Start, param.Length,out data);



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


        // TODO TODO TODO TODO
        #region Reprocess Attendance
        
        public ActionResult ReprocessAttendance(string result)
        {
            ViewBag.Message = result;
            return View();
        }


        // This method will recieve the form submisison
        // containing the employee_code and the to and from
        // dates.
        [HttpPost]
        public ActionResult RequestAttendanceReprocess(string from_date,string to_date,int employee_id)
        {
            BLL.MarkPreviousAttendance.createOrUpdateAttendance(from_date,to_date,employee_id);
            return RedirectToAction("ReprocessAttendance");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UploadFile(HttpPostedFileBase file)
        {
            string result = null;
            if (file != null && file.ContentLength > 0)
            {
                //try
                //{
                string path = Path.Combine(Server.MapPath("~/Uploads"),
                                   "ReprocessAttendance.csv");
                file.SaveAs(path);


                List<string> content = new List<string>();


                using (StreamReader sr = new StreamReader(path))
                {
                    while (sr.Peek() >= 0)
                    {
                        content.Add(sr.ReadLine());
                    }
                }

                result = TimeTune.ReprocessAttendanceImportExport.reprocessAttendance(content);
                return RedirectToAction("ReprocessAttendance", "AttendanceManagement", new { result = "Successful" });
                //return JavaScript("displayToastrSuccessfull()");
                //}
                //catch (Exception ex)
                //{
                //    return RedirectToAction("ManageEmployee", "EmployeeManagement", new { result = "Failed" });
                //}

            }
            return RedirectToAction("ReprocessAttendance", "AttendanceManagement", new { result = "Select File first" });
        }

        #endregion

        #region Future Manual Attendance
        public ActionResult FutureManualAttendance()
        {
            return View();
        }
        [HttpPost]
        public ActionResult FutureManualAttendance(string employee_code, string time_in_from, string time_in_to, string remarks)
        {
            // string message = TimeTune.ManualAttendance.addManualAttendance(manualAttendance);
            //var json = JsonConvert.SerializeObject(message);
            //AuditTrail.update(json, "Manual Attendance", int.Parse(User.Identity.Name));
            string message = BLL.ManageHR.addManualAttendance(time_in_from, time_in_to, employee_code, remarks);

            return RedirectToAction("FutureManualAttendance", new { message = message });
        }


        [HttpPost]
        public JsonResult FutureManualAttendanceDataHandler(DTParameters param)
        {
            try
            {

                var data = new List<BLL.ViewModels.ViewFutureManualAttendance>();

                // get all employee view models
                int count = BLL.ManageHR.getAllManualAttendance(param.Search.Value, param.SortOrder, param.Start, param.Length, out data);


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

        //EmployeeDataHandler
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
        #endregion

    }
}
