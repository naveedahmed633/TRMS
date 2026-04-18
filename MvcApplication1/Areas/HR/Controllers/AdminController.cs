using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TimeTune;
using ViewModels;

namespace MvcApplication1.Areas.HR.Controllers
{
    [Authorize(Roles = BLL.TimeTuneRoles.ROLE_HR)]
    public class AdminController : Controller
    {

        // GET: /HR/Admin/
        public ActionResult ChangeSessionTimeOut()
        {
            return View();
        }
        [HttpPost]
        public ActionResult ChangeSessionTimeOut(string Time_Out_Time)
        {

            int timeOut = int.Parse(Time_Out_Time);
            System.Web.HttpContext.Current.Session.Timeout = timeOut;
            System.Web.HttpContext.Current.Response.Cache.SetExpires(DateTime.UtcNow.AddSeconds(timeOut));
            // Set both server and browser caching.
            System.Web.HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.ServerAndPrivate);

            // Prevent browser's default max-age=0 header on first request
            // from invalidating the server cache on each request.
            System.Web.HttpContext.Current.Response.Cache.SetValidUntilExpires(true);

            // Set an HTTP ETag header on the page using a random GUID.
            System.Web.HttpContext.Current.Response.Cache.SetETag(System.Guid.NewGuid()
                                                       .ToString().Replace("-", ""));
            // Set last modified time.
            System.Web.HttpContext.Current.Response.Cache.SetLastModified(DateTime.UtcNow);

            // Now here is the critical piece that forces revalidation on each request!:
            System.Web.HttpContext.Current.Response.Cache.AppendCacheExtension(
                "must-revalidate, proxy-revalidate, max-age=0");
            return View();
        }
        public ActionResult MarkAttendance(string result)
        {
            //ViewModels.CreateManualAttendance manualAttendance = new CreateManualAttendance();
            //manualAttendance.employee = EmployeeCrud.getAll();
            //ViewBag.Message = result;
            ////loading the employee code in view
            //return View(manualAttendance);
            return View();
        }
        [HttpPost]
        public ActionResult MarkAttendance(string from_date, string to_date, string employee_code)
        {
            int employeeCode;
            string today = DateTime.Now.Date.ToString("yyyy-MM-dd");
            if (from_date.Equals(today))
            {
                ViewBag.Message = "Please select date other than today's date";
                return View();
            }
            if (!int.TryParse(employee_code, out employeeCode))
            {
                employeeCode = -1;
            }
            BLL.MarkPreviousAttendance.createOrUpdateAttendance(from_date, to_date, employeeCode, User.Identity.Name);
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UploadFile(HttpPostedFileBase file)
        {
            string result = null;
            if (file != null && file.ContentLength > 0)
            {

                string path = Path.Combine(Server.MapPath("~/Uploads"), DateTime.Now.ToString("yyyyMMddHHmmss") + "_ReprocessAttendance.csv");
                file.SaveAs(path);


                List<string> content = new List<string>();


                using (StreamReader sr = new StreamReader(path))
                {
                    while (sr.Peek() >= 0)
                    {
                        content.Add(sr.ReadLine());
                    }
                }

                result = TimeTune.ReprocessAttendanceImportExport.reprocessAttendance(content, User.Identity.Name);
                return RedirectToAction("MarkAttendance", "Admin", new { result = "Successful" });
                //return JavaScript("displayToastrSuccessfull()");
                //}
                //catch (Exception ex)
                //{
                //    return RedirectToAction("ManageEmployee", "EmployeeManagement", new { result = "Failed" });
                //}

            }
            return RedirectToAction("MarkAttendance", "Admin", new { result = "Select File first" });
        }
    }
}
