using System.Web.Mvc;

namespace MvcApplication1.Areas.HR
{
    public class HRAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "HR";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "HR_default",
                "HR/{controller}/{action}/{id}",
                new { controller = "HR", action = "Dashboard", id = UrlParameter.Optional },
                new[] { "MvcApplication1.Areas.HR.Controllers" }
            );
        }
    }
}
