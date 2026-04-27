using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using TimeTune;
using ViewModels;

namespace MvcApplication1.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            //return View();

            if (User.IsInRole(BLL.TimeTuneRoles.ROLE_EMP))
            {
                return RedirectToRoute(new
                {
                    Area = "EMP",
                    Controller = "EMP",
                    Action = "Dashboard"
                });
            }
            else if (User.IsInRole(BLL.TimeTuneRoles.ROLE_LM))
            {
                return RedirectToRoute(new
                {
                    Area = "LM",
                    Controller = "LM",
                    Action = "Dashboard"
                });
            }
            else if (User.IsInRole(BLL.TimeTuneRoles.ROLE_HR))
            {
                return RedirectToRoute(new
                {
                    Area = "HR",
                    Controller = "HR",
                    Action = "Dashboard"
                });
            }
            else if (User.IsInRole(BLL.TimeTuneRoles.ROLE_SLM))
            {
                return RedirectToRoute(new
                {
                    Area = "SLM",
                    Controller = "SLM",
                    Action = "Dashboard"
                });
            }
            else if (User.IsInRole(BLL.TimeTuneRoles.ROLE_REPORT))
            {
                return RedirectToRoute(new
                {
                    Area = "Report",
                    Controller = "Report",
                    Action = "Dashboard"
                });
            }
            else if (User.IsInRole(BLL.TimeTuneRoles.ROLE_SUPER_USER))
            {
                return RedirectToRoute(new
                {
                    Area = "SuperAdmin",
                    Controller = "SU",
                    Action = "ConfigurationManager"
                });
            }
            else if (User.IsInRole(BLL.TimeTuneRoles.ROLE_SUDO))
            {
                return RedirectToRoute(new
                {
                    Area = "Sudo",
                    Controller = "admin",
                    Action = "defaultPage"
                });
            }
            else
            {
                return RedirectToRoute(new
                {
                    Area = "",
                    Controller = "Account",
                    Action = "Login"
                });
            }

        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult Contact()
        {
            return View();
        }


    }
}
