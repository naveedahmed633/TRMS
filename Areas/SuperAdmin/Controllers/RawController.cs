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

namespace MvcApplication1.Areas.SuperAdmin.Controllers
{
    [Authorize(Roles = BLL.TimeTuneRoles.ROLE_SUPER_USER)]
    public class RawController : Controller
    {
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

        /////////////////////////// RAW - SuperAdmin ////////////////////////////////////

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

        ////////////////////////////////////////////////////////////////////////////////


    }
}
