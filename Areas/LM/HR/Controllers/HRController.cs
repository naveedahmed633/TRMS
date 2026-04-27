using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcApplication1.Areas.HR.Controllers
{
    [Authorize(Roles = BLL.TimeTuneRoles.ROLE_HR)]
    public class HRController : Controller
    {
        //
        // GET: /HR/

        public ActionResult Dashboard()
        {
            ViewModels.Dashboard dashboard=new ViewModels.Dashboard();
             var elements= TimeTune.Dashboard.getDashboardValues(User.Identity.Name);
             dashboard.supervisorName = elements.supervisorName;
             dashboard.dashboardElement = elements.dashboardElement;
             dashboard.headName = elements.headName;
            return View(dashboard);
        }
        public ActionResult ChangePassword()
        {
            return View();
        }
        public ActionResult ConfigurationManager()
        {
            return View();
        }

    }
}
