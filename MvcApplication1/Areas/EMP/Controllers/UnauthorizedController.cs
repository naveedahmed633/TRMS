using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ViewModels;

namespace MvcApplication1.Areas.EMP.Controllers
{
    [Authorize(Roles = BLL.TimeTuneRoles.ROLE_EMP)]
    public class UnauthorizedController : Controller
    {
        public ActionResult AccessDenied()
        {
            return View();
        }
    }
}
