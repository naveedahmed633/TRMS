using MVCDatatableApp.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TimeTune;
using ViewModels;

namespace MvcApplication1.Areas.Report.Controllers
{
    [Authorize(Roles = BLL.TimeTuneRoles.ROLE_REPORT)]
    public class AttendanceController : Controller
    {

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
                //int count = TimeTune.LmReports.getAllPersistentLogMatching(
                //    User.Identity.Name,
                //    param.Search.Value,
                //    param.SortOrder,
                //    param.Start, param.Length, out data);
                // get all employee view models
                int count = TimeTune.Reports.getAllPersistentLogMatching(
                    param.Search.Value, param.SortOrder, param.Start, param.Length, out data);

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

    }
}
