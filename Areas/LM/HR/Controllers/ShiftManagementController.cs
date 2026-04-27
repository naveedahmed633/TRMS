using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using TimeTune;

namespace MvcApplication1.Areas.HR.Controllers
{
    [Authorize(Roles = BLL.TimeTuneRoles.ROLE_HR)]
    public class ShiftManagementController : Controller
    {
        //
        // GET: /ShiftManagement/
        /*
        #region Groups
        public ActionResult AddGroup()
        {
            ViewModels.CreateGroup createGroup = new ViewModels.CreateGroup();
            createGroup.supervisor = EmployeeCrud.getAll();
            return View(createGroup);
        }
        [HttpPost]
        public ActionResult AddGroup(ViewModels.Group toCreate)
        {
            TimeTune.EmployeeGroup.add(toCreate);
            return View();
        }
        #endregion*/

        # region Shifts
        public ActionResult ManageShift()
        {
            List<ViewModels.Shift> shifts = TimeTune.EmployeeShift.getAll();

            return View(shifts);
        }

        [HttpPost]
        public ActionResult AddShift(ViewModels.Shift fromForm)
        {
            TimeTune.EmployeeShift.add(fromForm);
            return RedirectToAction("ManageShift");
        }

        [HttpPost]
        public JsonResult EditShift(ViewModels.Shift fromForm)
        {
            TimeTune.EmployeeShift.update(fromForm);
            return Json(new { status = "success" });
        }

        [HttpPost]
        public JsonResult DeleteShift(ViewModels.Shift fromForm)
        {
            TimeTune.EmployeeShift.remove(fromForm);
            return Json(new { status = "success" });
        }


        public ActionResult ShiftAuthorization()
        {
            return View();
        }

        #endregion


        #region GeneralCalendar
        public ActionResult Calendar()
        {
            return View();
        }

        [HttpPost]
        public JsonResult getGeneralCalendarForYear(int year)
        {
            ViewModels.GeneralCalendar viewModel = TimeTune.GeneralCalendarCrud.getCalendarForYear(year);
            string toSend = (viewModel == null)?"null":System.Web.Helpers.Json.Encode(viewModel);

            return Json( new { data = toSend } );
        }


        [HttpPost]
        public JsonResult getShiftsForGeneralCalendar()
        {
            List<ViewModels.Shift> shifts = TimeTune.EmployeeShift.getAll();
            string toSend = System.Web.Helpers.Json.Encode(shifts);


            return Json(new { data = toSend });
        }

        [HttpPost]
        public JsonResult addOrUpdateCalendar(string calendarData)
        {
            dynamic data = System.Web.Helpers.Json.Decode(calendarData);


            bool returnVal = TimeTune.GeneralCalendarCrud.addOrUpdateCalendar(data);

            
            //ViewModels.GeneralCalendar viewModel = TimeTune.GeneralCalendarCrud.getCalendarForYear(10);
            //string toSend = (viewModel == null) ? "null" : System.Web.Helpers.Json.Encode(viewModel);

            if(returnVal)
                return Json(new { success = true });
            else
                return Json(new { success = false });
        }
        #endregion
    }
}
