using BLL.ViewModels;
using MVCDatatableApp.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcApplication1.Areas.HR.Controllers
{
    [Authorize(Roles = BLL.TimeTuneRoles.ROLE_HR)]
    public class SuperLineManagersController : Controller
    {
        //
        // GET: /HR/SuperLineManagers/

        public ActionResult Manage()
        {
            return View();
        }

        #region DataTable

        // Do not be confused by the name this is a data table handler
        // for displaying groups.
        [HttpPost]
        public JsonResult getAllSLMsDataHandler(DTParameters param)
        {
            try
            {
                var data = new List<ViewModels.SuperLineManagersTableViewModel>();
                // get all employee view models
                int count = TimeTune.SuperLineManagersHelper.getAllSLMsMatching(param.Search.Value, param.SortOrder, param.Start, param.Length, out data);


                DTResult<ViewModels.SuperLineManagersTableViewModel> result =
                    new DTResult<ViewModels.SuperLineManagersTableViewModel>
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


        #region Ajax



        [HttpPost]
        public JsonResult getSLM()
        {
            int employeeID = 0;

            if (!int.TryParse(Request.Form["id"], out employeeID))
            {
                return Json(new { success = false });
            }

            var obj = TimeTune.SuperLineManagersHelper.getSLM(employeeID);


            return Json(obj);
        }


        [HttpPost]
        public JsonResult editSLM(ViewModels.EditSLMViewModel fromForm)
        {
            var from = fromForm;

            dynamic jsonToSend = TimeTune.SuperLineManagersHelper.editSLM(fromForm);
            var json = JsonConvert.SerializeObject(fromForm);
            TimeTune.AuditTrail.update(json, "SuperLineManager", int.Parse(User.Identity.Name));
            return Json(jsonToSend);
        }




        [HttpPost]
        public JsonResult getAllEmployees()
        {
            string q = Request.Form["data[q]"];

            // get all the employees that match the 
            // pattern 'q'
            ViewModels.Employee[] employees = TimeTune.SuperLineManagersHelper.getAllEmployeesMatching(q);


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
