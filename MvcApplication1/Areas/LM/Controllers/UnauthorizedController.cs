using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ViewModels;

namespace MvcApplication1.Areas.LM.Controllers
{
    [Authorize(Roles = BLL.TimeTuneRoles.ROLE_LM)]
    public class UnauthorizedController : Controller
    {
        public ActionResult AccessDenied()
        {
            return View();
        }
    }
}
