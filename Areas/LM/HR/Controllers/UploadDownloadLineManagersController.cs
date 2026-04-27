using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcApplication1.Areas.HR.Controllers
{
    [Authorize(Roles = BLL.TimeTuneRoles.ROLE_HR)]
    public class UploadDownloadLineManagersController : Controller
    {
        public ActionResult Manage(string result)
        {
            ViewBag.Message = result;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public FileResult DownloadCSVFile() {

            var toReturn = 
                
                new MvcApplication1.Utils.CSVWriter<TimeTune.LineManagersExportImport.LineManagersCSV>
                    (
                        TimeTune.LineManagersExportImport.getLineManagersCSV(),
                        DateTime.Now.ToString("yyyyddMMHHmmSS")+"-LineManagers.csv"
                    );


            return toReturn;
        }


        // Handle file upload and read/write etc.
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UploadFile(HttpPostedFileBase file)
        {
            if (file != null && file.ContentLength > 0)
            {
                try
                {
                    string path = Path.Combine(Server.MapPath("~/Uploads"),
                                       "lineManagers.csv");
                    file.SaveAs(path);


                    List<string> content = new List<string>();


                    using (StreamReader sr = new StreamReader(path))
                    {
                        while (sr.Peek() >= 0)
                        {
                            content.Add(sr.ReadLine());
                        }
                    }

                    TimeTune.LineManagersExportImport.setLineManagers(content);
                    return RedirectToAction("Manage", "UploadDownloadLineManagers", new { result = "Successful" });
                }
                catch (Exception ex)
                {
                    return RedirectToAction("Manage", "UploadDownloadLineManagers", new { result = "Failed" });
                }
               
            }
            return RedirectToAction("Manage", "UploadDownloadLineManagers", new { result = "Select File first" });
        }
    }


    [Authorize(Roles = BLL.TimeTuneRoles.ROLE_HR)]
    public class UploadDownloadSuperLineManagersController : Controller
    {
        public ActionResult Manage(string result)
        {
            ViewBag.Message = result;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public FileResult DownloadCSVFile()
        {

            var toReturn =

                new MvcApplication1.Utils.CSVWriter<TimeTune.SuperLineManagersExportImport.SuperLineManagersCSV>
                    (
                        TimeTune.SuperLineManagersExportImport.getLineManagersCSV(),
                        DateTime.Now.ToString("yyyyddMMHHmmSS") + "-LineManagers.csv"
                    );


            return toReturn;
        }


        // Handle file upload and read/write etc.

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UploadFile(HttpPostedFileBase file)
        {
            if (file != null && file.ContentLength > 0)
            {
                try
                {
                    string path = Path.Combine(Server.MapPath("~/Uploads"),
                                       "superLineManagers.csv");
                    file.SaveAs(path);


                    List<string> content = new List<string>();


                    using (StreamReader sr = new StreamReader(path))
                    {
                        while (sr.Peek() >= 0)
                        {
                            content.Add(sr.ReadLine());
                        }
                    }

                    TimeTune.SuperLineManagersExportImport.setSuperLineManagers(content);

                    return RedirectToAction("Manage", "UploadDownloadLineManagers", new { result = "Successful" });
                    
                }
                     
                catch (Exception ex)
                {
                    return RedirectToAction("Manage", "UploadDownloadLineManagers", new { result = "Failed" });
                }
                
            }
            return RedirectToAction("Manage", "UploadDownloadLineManagers", new { result = "Select File first" });
        }
    }
}
