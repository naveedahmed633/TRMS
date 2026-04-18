using MVCDatatableApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ViewModels;

namespace MvcApplication1.Areas.EMP.Controllers
{
    [Authorize(Roles = BLL.TimeTuneRoles.ROLE_EMP)]
    public class AttendanceManagementController : Controller
    {
        //
        // GET: /EMP/AttendanceManagement/

        public ActionResult ViewAttendance()
        {
            string empCode= User.Identity.Name;
            var Attendance = TimeTune.Attendance.getPersistanceLog(empCode);
            return View(Attendance);
        }

        public ActionResult ViewFutureManualAttendance()
        {
            return View();
        }

        [HttpPost]
        public JsonResult FutureManualAttendanceDataHandler(DTParameters param)
        {
            try
            {

                var data = new List<BLL.ViewModels.ViewFutureManualAttendance>();

                // get all employee view models
                int count = BLL.ManageHR.getEmpManualAttendance(User.Identity.Name, param.Search.Value, param.SortOrder, param.Start, param.Length, out data);


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

    }
}
